using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Position.Commands
{
    public class UpdatePositionCommand : IRequest<Result>
    {
        public required PositionModel PositionModel { get; set; }
    }
    public class UpdatePositionCommandHandler : IRequestHandler<UpdatePositionCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public UpdatePositionCommandHandler(
            IRegalEducationDbContext context,
            IMapper mapper,
            ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
        }

        public async Task<Result> Handle(UpdatePositionCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Positions
                .Include (t => t.DepartmentPositions)
                .FirstOrDefaultAsync (x => x.Id == request.PositionModel.Id, cancellationToken);

            if (entity == null)
                return Result.Failure (_localizer.Format (LocalizationKey.EntityNotFound, EntityName.Position));
            // Check trường hợp FE truyền mảng rỗng
            if (request.PositionModel.DepartmentPositions != null &&
                !request.PositionModel.DepartmentPositions.Any ( ))
            {
                // Sử dụng localizer cho message lỗi business
                return Result.Failure (_localizer[LocalizationKey.MustHasAtLeastOneDepartment]);
            }

            // Map only updatable fields
            entity.PositionCode = request.PositionModel.PositionCode;
            entity.PositionName = request.PositionModel.PositionName;
            entity.Description = request.PositionModel.Description;
            entity.Status = request.PositionModel.Status;
            entity.IsSale = request.PositionModel.IsSale;
            entity.IsSaleLead = request.PositionModel.IsSaleLead;
            entity.IsSupport = request.PositionModel.IsSupport;
            // CHỈ xử lý DepartmentPositions nếu FE truyền lên
            if (request.PositionModel.DepartmentPositions != null)
            {
                // Xóa liên kết cũ
                if (entity.DepartmentPositions != null && entity.DepartmentPositions.Any ( ))
                {
                    _context.DepartmentPositions.RemoveRange (entity.DepartmentPositions);
                    await _context.SaveChangesAsync (cancellationToken);
                }

                // Tạo mới liên kết nếu có (nếu rỗng => sẽ không tạo mới gì)
                var departmentPositions = request.PositionModel.DepartmentPositions
                    .Select (dp => new DepartmentPosition
                    {
                        Id = Guid.NewGuid ( ),
                        DepartmentId = dp.DepartmentId,
                        PositionId = entity.Id
                    }).ToList ( );

                _context.DepartmentPositions.AddRange (departmentPositions);
            }

            var success = await _context.SaveChangesAsync (cancellationToken) > 0;
            if (success)
                return Result.Success (_localizer.Format (LocalizationKey.MSG_UPDATE_SUCCESS, EntityName.Position));
            else
                return Result.Failure (_localizer.Format (LocalizationKey.ERR_SAVE_NO_EFFECT, EntityName.Position));
        }


    }
}
