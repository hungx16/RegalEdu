using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Event.Commands
{
    public class UpdateEventCommand : IRequest<Result>
    {
        public required EventModel EventModel { get; set; }
    }

    public class UpdateEventCommandHandler : IRequestHandler<UpdateEventCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly AutoMapper.IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public UpdateEventCommandHandler(
            IRegalEducationDbContext context,
            AutoMapper.IMapper mapper,
            ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        public async Task<Result> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
        {
            // 1️ Tìm sự kiện theo Id
            var existingEvent = await _context.Events
                .FirstOrDefaultAsync(x => x.Id == request.EventModel.Id && !x.IsDeleted, cancellationToken);

            if (existingEvent == null)
            {
                return Result.Failure(
                    _localizer.Format(LocalizationKey.EntityNotFound, EntityName.Event));
            }

            // 2️ Kiểm tra EventCode trùng (loại trừ bản ghi hiện tại)
            var duplicateCode = await _context.Events
                .AnyAsync(x => x.EventCode == request.EventModel.EventCode
                            && x.Id != request.EventModel.Id
                            && !x.IsDeleted,
                          cancellationToken);

            if (duplicateCode)
            {
                return Result.Failure(
                    _localizer.Format(LocalizationKey.ModelCodeAlreadyExists,
                        _localizer[EntityName.Event], request.EventModel.EventCode));
            }

            // 3️ Kiểm tra EventName trùng (loại trừ bản ghi hiện tại)
            var duplicateName = await _context.Events
                .AnyAsync(x => x.EventName == request.EventModel.EventName
                            && x.Id != request.EventModel.Id
                            && !x.IsDeleted,
                          cancellationToken);

            if (duplicateName)
            {
                return Result.Failure(
                    _localizer.Format(LocalizationKey.ModelNameAlreadyExists,
                        _localizer[EntityName.Event], request.EventModel.EventName));
            }

            // 4️ Map dữ liệu mới sang entity cũ
            _mapper.Map(request.EventModel, existingEvent);

            // 5️ Lưu thay đổi
            var success = await _context.SaveChangesAsync(cancellationToken) > 0;

            if (success)
            {
                return Result.Success(
                    _localizer.Format(LocalizationKey.MSG_UPDATE_SUCCESS, EntityName.Event));
            }
            else
            {
                return Result.Failure(
                    _localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, EntityName.Event));
            }
        }
    }
}
