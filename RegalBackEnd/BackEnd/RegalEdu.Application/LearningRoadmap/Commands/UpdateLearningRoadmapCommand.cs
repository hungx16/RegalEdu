using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.LearningRoadMap.Commands
{
    public class UpdateLearningRoadMapCommand : IRequest<Result>
    {
        public required LearningRoadMapModel LearningRoadMapModel { get; set; }
    }

    public class UpdateLearningRoadMapCommandHandler : IRequestHandler<UpdateLearningRoadMapCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly AutoMapper.IMapper _Mapper;
        private readonly ILocalizationService _localizer;
        private readonly IFileService _fileService;

        public UpdateLearningRoadMapCommandHandler(IRegalEducationDbContext context, AutoMapper.IMapper Mapper, ILocalizationService localizer, IFileService fileService)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context)); ;
            _Mapper = Mapper ?? throw new ArgumentNullException (nameof (Mapper)); ;
            _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer)); ;
            _fileService = fileService ?? throw new ArgumentNullException (nameof (fileService));
        }

        public async Task<Result> Handle(UpdateLearningRoadMapCommand request, CancellationToken cancellationToken)
        {

            var entity = await _context.LearningRoadMaps.Include (t => t.Images).FirstOrDefaultAsync (x => x.Id == request.LearningRoadMapModel.Id);
            if (entity == null)
            {
                return Result.Failure (_localizer.Format (LocalizationKey.EntityNotFound, EntityName.LearningRoadMap));
            }
            var model = request.LearningRoadMapModel;
            // ====== CẬP NHẬT THUỘC TÍNH ======
            entity.LearningRoadMapName = model.LearningRoadMapName;
            entity.Status = model.Status;
            entity.LearningRoadMapCode = model.LearningRoadMapCode;
            entity.Description = model.Description;
            entity.AgeGrId = model.AgeGrId;
            entity.CommitmentOutput = model.CommitmentOutput;
            entity.Order = model.Order;
            entity.IsPublish = model.IsPublish;
            entity.Icon = model.Icon;
            entity.IsMultilingual = model.IsMultilingual;
            entity.VotingRate = model.VotingRate;
            entity.NumberOfSatisfiedStudents = model.NumberOfSatisfiedStudents;
            entity.NumberOfStudents = model.NumberOfStudents;
            // Flag song ngữ (tự động bật nếu có bản dịch EN)
            entity.IsMultilingual = model.IsMultilingual;


            if (entity.IsMultilingual)
            {
                entity.EnDescription = model.EnDescription;
                entity.EnLearningRoadMapName = model.EnLearningRoadMapName;
            }
            // ====== XỬ LÝ ẢNH ĐẠI DIỆN ======
            // 3.1 Xoá ảnh theo deletedImageIds (nếu có)
            var deletedIds = (model.DeletedImageIds ?? new List<string> ( )).Where (s => !string.IsNullOrWhiteSpace (s)).ToHashSet ( );

            // 3.1 Xoá ảnh theo deletedImageIds (nếu có)
            if (deletedIds.Count > 0 && entity.Images?.Any ( ) == true)
            {
                var toRemove = entity.Images.Where (ci => deletedIds.Contains (ci.Id.ToString ( ))).ToList ( );
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
            if (deletedIds.Count > 0 && entity.Images?.Any ( ) == true)
            {
                var toRemove = entity.Images.Where (ci => deletedIds.Contains (ci.Id.ToString ( ))).ToList ( );
                if (toRemove.Count > 0)
                {
                    // xoá file vật lý (best-effort)
                    foreach (var img in toRemove)
                    {
                        if (!string.IsNullOrWhiteSpace (img.Path))
                        {
                            try { await _fileService.DeleteFileAsync (img.Path); } catch { /* ignore */ }
                        }
                    }
                    _context.Images.RemoveRange (toRemove);
                }
            }

            // 3.2 Upsert danh sách ảnh hiện tại từ payload
            var incoming = model.Images ?? new List<ImageDto> ( );
            // Fixing the type mismatch issue by ensuring the dictionary key type matches
            var existingById = entity.Images?.ToDictionary (x => x.Id.ToString ( ))
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
                        LearningRoadMapId = entity.Id,
                        Path = finalPath ?? string.Empty,
                        IsCover = img.IsCover,

                    };
                    _context.Images.Add (newImg);
                }
                else
                {
                    // Cập nhật ảnh cũ
                    if (existingById.TryGetValue (img.Id.ToString ( )!, out var exist))
                    {
                        exist.Path = finalPath ?? exist.Path;
                        exist.IsCover = img.IsCover;
                        _context.Images.Update (exist);
                    }
                    // nếu payload gửi id không tồn tại trong DB -> thêm mới như thường (phòng ngừa)
                    else
                    {
                        var newImg = new Image
                        {
                            Id = img.Id!, // giữ id FE gửi nếu có
                            LearningRoadMapId = entity.Id,
                            Path = finalPath ?? string.Empty,
                            IsCover = img.IsCover,
                        };
                        _context.Images.Add (newImg);
                    }
                }
            }

            var success = await _context.SaveChangesAsync (cancellationToken) > 0;
            if (success)
            {
                return Result.Success (_localizer.Format (LocalizationKey.MSG_UPDATE_SUCCESS, EntityName.LearningRoadMap));
            }
            else
            {
                return Result.Failure (_localizer.Format (LocalizationKey.ERR_SAVE_NO_EFFECT, EntityName.LearningRoadMap));
            }
        }
    }
}