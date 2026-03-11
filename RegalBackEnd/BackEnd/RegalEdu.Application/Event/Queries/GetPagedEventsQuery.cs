using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enums;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Event.Queries
{
    // DTO chứa điều kiện tìm kiếm + phân trang
    public class EventQuery
    {
        public string? EventCode { get; set; }   // Lọc theo mã sự kiện (chứa)
        public string? EventName { get; set; }   // Lọc theo tên sự kiện (chứa)
        public byte? Status { get; set; }        // Lọc theo trạng thái (nếu có)
        public int Page { get; set; } = 1;       // Trang hiện tại (mặc định 1)
        public int PageSize { get; set; }        // Số bản ghi mỗi trang (sẽ set từ PagingOptions)
    }

    // Query yêu cầu lấy danh sách sự kiện phân trang
    public class GetPagedEventsQuery : IRequest<Result<PagedResult<EventModel>>>
    {
        public required EventQuery EventQuery { get; set; }
    }

    // Handler xử lý lấy danh sách sự kiện phân trang
    public class GetPagedEventsQueryHandler : IRequestHandler<GetPagedEventsQuery, Result<PagedResult<EventModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly PagingOptions _pagingOptions;
        private readonly IMapper _mapper;

        public GetPagedEventsQueryHandler(
            IRegalEducationDbContext context,
            PagingOptions pagingOptions,
            IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _pagingOptions = pagingOptions ?? throw new ArgumentNullException (nameof (pagingOptions));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
        }

        public async Task<Result<PagedResult<EventModel>>> Handle(GetPagedEventsQuery request, CancellationToken cancellationToken)
        {
            if (request.EventQuery == null)
                throw new ArgumentNullException (nameof (request.EventQuery));

            // Truy vấn sự kiện, chỉ lấy bản ghi chưa bị xoá mềm
            var query = _context.Events
                .AsNoTracking ( )
                .Where (x => !x.IsDeleted);

            // Áp dụng bộ lọc động (nếu có)
            if (!string.IsNullOrWhiteSpace (request.EventQuery.EventCode))
            {
                query = query.Where (d => d.EventCode.Contains (request.EventQuery.EventCode));
            }
            if (!string.IsNullOrWhiteSpace (request.EventQuery.EventName))
            {
                query = query.Where (d => d.EventName.Contains (request.EventQuery.EventName));
            }
            if (request.EventQuery.Status.HasValue)
            {
                query = query.Where (d => d.Status == (StatusType)request.EventQuery.Status.Value);
            }

            // Tổng số bản ghi thỏa điều kiện
            int totalRecords = await query.CountAsync (cancellationToken);

            // Đảm bảo PageSize có giá trị, nếu chưa thì dùng mặc định
            request.EventQuery.PageSize = request.EventQuery.PageSize > 0
                ? request.EventQuery.PageSize
                : _pagingOptions.DefaultPageSize;

            // Lấy dữ liệu phân trang
            var pagedEntities = await query
                .OrderByDescending (x => x.CreatedAt) // Sắp xếp mới nhất trước
                .Skip ((request.EventQuery.Page - 1) * request.EventQuery.PageSize)
                .Take (request.EventQuery.PageSize)
                .ToListAsync (cancellationToken);

            // Map sang DTO (EventModel)
            var result = pagedEntities
                .Select (d => _mapper.Map<EventModel> (d))
                .ToList ( );

            // Đóng gói vào PagedResult
            var pagedResult = new PagedResult<EventModel>
            {
                Items = result,
                Total = totalRecords,
            };

            return Result<PagedResult<EventModel>>.Success (pagedResult);
        }
    }
}
