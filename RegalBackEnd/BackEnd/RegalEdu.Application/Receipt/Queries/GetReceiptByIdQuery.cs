using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Receipt.Queries
{
    public class GetReceiptByIdQuery : IRequest<Result<ReceiptsModel>>
    {
        public required string Id { get; set; }
    }

    public class GetReceiptByIdQueryHandler : IRequestHandler<GetReceiptByIdQuery, Result<ReceiptsModel>>
    {
        private readonly IRegalEducationDbContext _db;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public GetReceiptByIdQueryHandler(IRegalEducationDbContext db, IMapper mapper, ILocalizationService localizer)
        {
            _db = db; _mapper = mapper; _localizer = localizer;
        }

        public async Task<Result<ReceiptsModel>> Handle(GetReceiptByIdQuery request, CancellationToken ct)
        {
            var e = await _db.Receipts.AsNoTracking()
                .Include(x => x.Student)
                .Include(x => x.Course)
                .Include(x => x.Employee)
                .Include(x => x.RegisterStudy)
                .FirstOrDefaultAsync(x => x.Id.ToString() == request.Id && !x.IsDeleted, ct);

            if (e == null)
                return Result<ReceiptsModel>.Failure(_localizer.Format(LocalizationKey.EntityWithIdNotFound, EntityName.Receipt, request.Id));

            return Result<ReceiptsModel>.Success(_mapper.Map<ReceiptsModel>(e));
        }
    }
}