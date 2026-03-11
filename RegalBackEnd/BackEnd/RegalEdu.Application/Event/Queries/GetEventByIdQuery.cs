using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Event.Queries
{
    // Query: Lấy thông tin sự kiện theo Id
    public class GetEventByIdQuery : IRequest<Result<EventModel>>
    {
        public required string Id { get; set; } // Id sự kiện cần lấy (chuỗi Guid dạng string)
    }

    // Handler: Xử lý lấy thông tin sự kiện theo Id
    public class GetEventByIdQueryHandler : IRequestHandler<GetEventByIdQuery, Result<EventModel>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public GetEventByIdQueryHandler(
            IRegalEducationDbContext context,
            IMapper mapper,
            ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        public async Task<Result<EventModel>> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
        {
            // Tìm sự kiện theo Id, chỉ lấy bản ghi chưa bị xóa mềm (IsDeleted == false)
            var eventEntity = await _context.Events
                //.Include(x => x.Departments) // Có thể Include nếu cần lấy thêm dữ liệu liên quan
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id.ToString() == request.Id && !x.IsDeleted,
                    cancellationToken);

            // Nếu không tìm thấy → trả về lỗi có thông báo localize
            if (eventEntity == null)
            {
                var msg = _localizer.Format(
                    LocalizationKey.EntityWithIdNotFound,
                    _localizer[EntityName.Event], // "Sự kiện"
                    request.Id
                );
                return Result<EventModel>.Failure(msg);
            }

            // Map sang EventModel để trả về DTO
            var result = _mapper.Map<EventModel>(eventEntity);

            return Result<EventModel>.Success(result);
        }
    }
}
