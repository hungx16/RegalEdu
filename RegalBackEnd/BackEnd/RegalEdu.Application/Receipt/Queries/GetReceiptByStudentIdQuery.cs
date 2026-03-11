using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Receipt.Queries
{
    public class GetReceiptByStudentIdQuery : IRequest<Result<List<ReceiptsModel>>>
    {
        public required string StudentId { get; set; }
    }

    public class GetReceiptByStudentIdQueryHandler : IRequestHandler<GetReceiptByStudentIdQuery, Result<List<ReceiptsModel>>>
    {
        private readonly IRegalEducationDbContext _db;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public GetReceiptByStudentIdQueryHandler(IRegalEducationDbContext db, IMapper mapper, ILocalizationService localizer)
        {
            _db = db; _mapper = mapper; _localizer = localizer;
        }

        public async Task<Result<List<ReceiptsModel>>> Handle(GetReceiptByStudentIdQuery request, CancellationToken ct)
        {
            if (!Guid.TryParse(request.StudentId, out var studentId))
            {
                return Result<List<ReceiptsModel>>.Failure(_localizer["InvalidIdFormat"]);
            }

            var receipts = await _db.Receipts.AsNoTracking()
                .Include(x => x.Student)
                .Include(x => x.Course)
                .Include(x => x.Employee)
                .Include(x => x.RegisterStudy).ThenInclude(x => x.Company)
                .Include(x => x.RegisterStudy).ThenInclude(x => x.Region)
                .Include(x => x.RegisterStudy).ThenInclude(x => x.Student)
                .Where(x => x.StudentId == studentId && !x.IsDeleted)
                .ToListAsync(ct);

            var models = _mapper.Map<List<ReceiptsModel>>(receipts);

            foreach (var model in models)
            {
                model.CompanyName = model.RegisterStudy?.Company?.CompanyName;
                model.StudentName = model.RegisterStudy?.Student?.FullName ?? model.Student?.FullName;
                model.RegionName = model.RegisterStudy?.Region?.RegionName;
            }

            return Result<List<ReceiptsModel>>.Success(models);
        }
    }
}
