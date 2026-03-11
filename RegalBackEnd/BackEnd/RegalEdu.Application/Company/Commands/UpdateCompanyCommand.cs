using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Company.Commands
{
    public class UpdateCompanyCommand : IRequest<Result>
    {
        public required CompanyModel CompanyModel { get; set; }

        public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, Result>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;
            private readonly ILocalizationService _localizer;
            private readonly IFileService _fileService; // ⬅️ thêm file service để move/xoá file

            public UpdateCompanyCommandHandler(
                IRegalEducationDbContext context,
                IMapper mapper,
                ILocalizationService localizer,
                IFileService fileService)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
                _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
                _fileService = fileService ?? throw new ArgumentNullException (nameof (fileService));
            }

            public async Task<Result> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
            {
                var model = request.CompanyModel;
                //if (string.IsNullOrWhiteSpace (model.Id))
                //    return Result.Failure (_localizer.Format (LocalizationKey.InvalidId, EntityName.Company));

                // Load kèm quan hệ cần thao tác
                var entity = await _context.Companies
                    .Include (x => x.LogRegionComs)
                    .Include (x => x.CompanyLearningRoadMaps)
                    .Include (x => x.CompanyImages)
                    .FirstOrDefaultAsync (x => x.Id == model.Id, cancellationToken);

                if (entity == null)
                    return Result.Failure (_localizer.Format (LocalizationKey.EntityNotFound, _localizer[EntityName.Company]));

                // Transaction cho an toàn
                using var tx = await (_context as DbContext)!.Database.BeginTransactionAsync (cancellationToken);

                // --------- 1) Update trường cơ bản ----------
                entity.Status = model.Status;
                entity.CompanyPhone = model.CompanyPhone;
                entity.CompanyName = model.CompanyName;
                entity.CompanyAddress = model.CompanyAddress;
                entity.EstablishmentDate = model.EstablishmentDate;
                entity.ProvinceCode = model.ProvinceCode ?? string.Empty;
                entity.ManagerId = string.IsNullOrWhiteSpace (model.ManagerId.ToString ( )) ? null : model.ManagerId;
                entity.CompanyCode = model.CompanyCode ?? string.Empty;
                entity.IsPublish = model.IsPublish;
                entity.WardCode = model.WardCode ?? string.Empty;
                entity.CompanyEmail = model.CompanyEmail;
                entity.NumberOfStudents = model.NumberOfStudents;
                entity.Convenience = model.Convenience;
                entity.VotingRate = model.VotingRate;
                entity.LeaningRoadMapTags = model.LeaningRoadMapTags;
                entity.Description = model.Description;
                entity.IsMultilingual = model.IsMultilingual;
                entity.IsHeadQuarters = model.IsHeadQuarters;
                entity.WorkingTime = model.WorkingTime;
                entity.Longitude = model.Longitude;
                entity.Latitude = model.Latitude;
                if (model.IsMultilingual == true)
                {
                    entity.EnCompanyAddress = model.EnCompanyAddress ?? string.Empty;
                    entity.EnConvenience = model.EnConvenience;
                    entity.EnDescription = model.EnDescription;
                    entity.EnCompanyName = model.EnCompanyName ?? string.Empty;
                    entity.EnLeaningRoadMapTags = model.EnLeaningRoadMapTags;
                    entity.EnDescription = model.EnDescription;
                    entity.EnWorkingTime = model.EnWorkingTime;
                }
                //_context.Companies.Update (entity);

                // --------- 2) Log vùng ----------
                if (model.LogRegionComs != null)
                {
                    // Xoá hết rồi add lại theo payload (đơn giản & đúng với FE đã chuẩn hoá)
                    if (entity.LogRegionComs?.Any ( ) == true)
                    {
                        _context.LogRegionComs.RemoveRange (entity.LogRegionComs);
                    }
                    var logRegionComs = model.LogRegionComs.Select (l => new RegalEdu.Domain.Entities.LogRegionCom
                    {
                        CompanyId = l.CompanyId,
                        RegionId = l.RegionId,
                        StartedDate = l.StartedDate,
                        EndDate = l.EndDate,
                        Description = l.Description,
                        Status = l.Status,
                        CreatedBy = l.CreatedBy,
                        CreatedAt = l.CreatedAt,
                        UpdatedAt = l.UpdatedAt,
                        UpdatedBy = l.UpdatedBy
                    }).ToList ( );
                    await _context.LogRegionComs.AddRangeAsync (logRegionComs, cancellationToken);
                }
                // CHỈ xử lý DepartmentPositions nếu FE truyền lên
                if (request.CompanyModel.CompanyLearningRoadMaps != null)
                {
                    // Xóa liên kết cũ
                    if (entity.CompanyLearningRoadMaps != null && entity.CompanyLearningRoadMaps.Any ( ))
                    {
                        _context.CompanyLearningRoadMaps.RemoveRange (entity.CompanyLearningRoadMaps);
                        await _context.SaveChangesAsync (cancellationToken);
                    }

                    // Tạo mới liên kết nếu có (nếu rỗng => sẽ không tạo mới gì)
                    var companyLearningRoadMaps
                        = request.CompanyModel.CompanyLearningRoadMaps
                        .Select (dp => new CompanyLearningRoadMap
                        {
                            Id = Guid.NewGuid ( ),
                            LearningRoadMapId = dp.LearningRoadMapId,
                            CompanyId = entity.Id
                        }).ToList ( );

                    _context.CompanyLearningRoadMaps.AddRange (companyLearningRoadMaps);
                }
                // --------- 3) Ảnh/Tệp đính kèm ----------
                // FE: companyImages[].imageUrl có thể là "temp/xxx.ext" (mới upload) hoặc "uploads/..." (cũ)
                // FE: deletedImageIds[] là id ảnh cần xoá
                var deletedIds = (model.DeletedImageIds ?? new List<string> ( )).Where (s => !string.IsNullOrWhiteSpace (s)).ToHashSet ( );

                // 3.1 Xoá ảnh theo deletedImageIds (nếu có)
                if (deletedIds.Count > 0 && entity.CompanyImages?.Any ( ) == true)
                {
                    var toRemove = entity.CompanyImages.Where (ci => deletedIds.Contains (ci.Id.ToString ( ))).ToList ( );
                    if (toRemove.Count > 0)
                    {
                        // xoá file vật lý (best-effort)
                        foreach (var img in toRemove)
                        {
                            if (!string.IsNullOrWhiteSpace (img.Path))
                            {
                                // không cần await từng cái; nhưng để chắc chắn thì await tuần tự
                                try { await _fileService.DeleteFileAsync (img.Path); } catch { /* ignore */ }
                            }
                        }
                        _context.Images.RemoveRange (toRemove);
                    }
                }

                // 3.2 Upsert danh sách ảnh hiện tại từ payload
                var incoming = model.CompanyImages ?? new List<ImageDto> ( );
                // Fixing the type mismatch issue by ensuring the dictionary key type matches
                var existingById = entity.CompanyImages?.ToDictionary (x => x.Id.ToString ( ))
                                   ?? new Dictionary<string, Image> ( );

                foreach (var img in incoming)
                {
                    var isNew = string.IsNullOrWhiteSpace (img.Id.ToString ( ));
                    string? finalPath = img.Path;

                    // Nếu là file mới (đường dẫn đang ở temp/...), move sang uploads/company
                    if (!string.IsNullOrWhiteSpace (finalPath) &&
                        finalPath.StartsWith ("temp/", StringComparison.OrdinalIgnoreCase))
                    {

                        try
                        {
                            finalPath = await _fileService.MoveFileAsync (finalPath, "images");
                        }
                        catch
                        {
                            // Nếu move lỗi, có thể trả failure hoặc bỏ qua theo nhu cầu
                            return Result.Failure (_localizer["ERR_FILE_MOVE_FAILED"]);
                        }
                    }

                    if (isNew)
                    {
                        // Thêm mới
                        var newImg = new Image
                        {
                            //Id = Guid.NewGuid ( ).ToString ( ),   // hoặc để DB tự sinh nếu là identity
                            CompanyId = entity.Id,
                            Path = finalPath ?? string.Empty,
                            Caption = img.Caption ?? string.Empty,
                            IsCover = img.IsCover,
                            SortOrder = img.SortOrder <= 0 ? 1 : img.SortOrder,

                        };
                        _context.Images.Add (newImg);
                    }
                    else
                    {
                        // Cập nhật ảnh cũ
                        if (existingById.TryGetValue (img.Id.ToString ( )!, out var exist))
                        {
                            exist.Path = finalPath ?? exist.Path;
                            exist.Caption = img.Caption ?? string.Empty;
                            exist.IsCover = img.IsCover;
                            exist.SortOrder = img.SortOrder <= 0 ? exist.SortOrder : img.SortOrder;
                            // exist.Status giữ nguyên hoặc cập nhật theo nhu cầu
                            _context.Images.Update (exist);
                        }
                        // nếu payload gửi id không tồn tại trong DB -> thêm mới như thường (phòng ngừa)
                        else
                        {
                            var newImg = new Image
                            {
                                Id = img.Id!, // giữ id FE gửi nếu có
                                CompanyId = entity.Id,
                                Path = finalPath ?? string.Empty,
                                Caption = img.Caption ?? string.Empty,
                                IsCover = img.IsCover,
                                SortOrder = img.SortOrder <= 0 ? 1 : img.SortOrder,

                            };
                            _context.Images.Add (newImg);
                        }
                    }
                }

                await _context.SaveChangesAsync (cancellationToken);


                var images = (entity.CompanyImages ?? new List<Image> ( ))
                    .OrderBy (x => x.SortOrder)
                    .ToList ( );

                if (images.Count > 0)
                {
                    // đảm bảo chỉ 1 cover
                    var firstCoverIdx = images.FindIndex (x => x.IsCover);
                    if (firstCoverIdx < 0) images[0].IsCover = true;
                    else
                    {
                        for (int i = 0; i < images.Count; i++)
                            images[i].IsCover = i == firstCoverIdx;
                    }

                    // chuẩn hoá sort
                    for (int i = 0; i < images.Count; i++)
                        images[i].SortOrder = i + 1;

                    _context.Images.UpdateRange (images);
                }
                entity.UpdatedAt = DateTime.Now;
                var success = await _context.SaveChangesAsync ( ) > 0;
                if (!success)
                {
                    await tx.RollbackAsync (cancellationToken);
                    return Result.Failure (_localizer.Format (LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer[EntityName.Company]));
                }

                await tx.CommitAsync (cancellationToken);
                return Result.Success (_localizer.Format (LocalizationKey.MSG_UPDATE_SUCCESS, _localizer[EntityName.Company]));
            }
        }
    }
}
