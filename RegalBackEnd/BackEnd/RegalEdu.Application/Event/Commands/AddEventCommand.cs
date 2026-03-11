using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Enums;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Event.Commands
{
    public class AddEventCommand : IRequest<Result>
    {
        public required EventModel EventModel { get; set; }
    }

    public class AddEventCommandHandler : IRequestHandler<AddEventCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly AutoMapper.IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public AddEventCommandHandler(
            IRegalEducationDbContext context,
            AutoMapper.IMapper mapper,
            ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
        }

        public async Task<Result> Handle(AddEventCommand request, CancellationToken cancellationToken)
        {
            // 1️ Xác định AutoCodeType dựa vào Category
            AutoCodeType autoCodeType = request.EventModel.Category switch
            {
                EventCategory.Event => AutoCodeType.Event_SK,
                EventCategory.Report => AutoCodeType.Event_BC,
                //EventCategory.News => AutoCodeType.Event_TT,
                //EventCategory.Link => AutoCodeType.Event_LK,
                _ => throw new ArgumentOutOfRangeException (
                        nameof (request.EventModel.Category),
                        request.EventModel.Category,
                        _localizer.Format (LocalizationKey.InvalidEventCategory, nameof (EventCategory)))
            };

            // 2️ Lấy cấu hình AutoCodeInfo
            var info = AutoCodeConfig.Get (autoCodeType);

            // 3️ Đảm bảo _context là DbContext hợp lệ
            if (_context is not DbContext dbContext)
            {
                throw new InvalidOperationException (
                    _localizer.Format (LocalizationKey.InvalidDbContextInstance, nameof (IRegalEducationDbContext)));
            }

            // 4️ Sinh code + lưu sự kiện
            var result = await AutoCodeHelper.CreateWithAutoCodeRetryAsync (
                info,
                async (code) =>
                {
                    try
                    {
                        // Map từ EventModel sang Entity
                        var entity = _mapper.Map<Domain.Entities.Event> (request.EventModel);

                        // Gán EventCode tự động sinh
                        entity.EventCode = code;

                        // Lưu vào DB
                        await _context.Events.AddAsync (entity, cancellationToken);
                        var success = await _context.SaveChangesAsync (cancellationToken) > 0;

                        if (success)
                        {
                            return Result.Success (_localizer.Format (
                                LocalizationKey.MSG_CREATE_SUCCESS, EntityName.Event));
                        }
                        else
                        {
                            return Result.Failure (_localizer.Format (
                                LocalizationKey.ERR_SAVE_NO_EFFECT, EntityName.Event));
                        }
                    }
                    catch (Exception ex)
                    {
                        // Bắt lỗi lưu DB hoặc mapping thất bại
                        return Result.Failure (_localizer.Format (
                            LocalizationKey.ERR_CREATE_FAILED, EntityName.Event, ex.Message));
                    }
                },
                dbContext
            );

            return result;
        }
    }
}
