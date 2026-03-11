using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Receipt.Queries
{
    public class GetAllReceiptsQuery : IRequest<Result<List<ReceiptsModel>>> { }

    public class GetAllReceiptsQueryHandler : IRequestHandler<GetAllReceiptsQuery, Result<List<ReceiptsModel>>>
    {
        private readonly IRegalEducationDbContext _db;
        private readonly IMapper _mapper;

        public GetAllReceiptsQueryHandler(IRegalEducationDbContext db, IMapper mapper)
        {
            _db = db; _mapper = mapper;
        }

        public async Task<Result<List<ReceiptsModel>>> Handle(GetAllReceiptsQuery request, CancellationToken ct)
        {
            var data = await _db.Receipts.AsNoTracking()
                .Include(x => x.Student)
                .Include(x => x.Course)
                .Include(x => x.Employee)
                .Include(x => x.RegisterStudy).ThenInclude(x => x.Company)
                .Include(x => x.RegisterStudy).ThenInclude(x => x.Region)
                .Include(x => x.RegisterStudy).ThenInclude(x => x.Student)
                .Where(x => !x.IsDeleted)
                .ToListAsync(ct);
            List<ReceiptsModel> models = new List<ReceiptsModel>();
            models= _mapper.Map<List<ReceiptsModel>>(data);
            //lấy các giá trì của companyName, StudentName, RegionName

            foreach (var model in models)
            {
                model.CompanyName = model.RegisterStudy?.Company?.CompanyName;
                model.StudentName = model.RegisterStudy?.Student?.FullName;
                model.RegionName = model.RegisterStudy?.Region?.RegionName;
            }
            return Result<List<ReceiptsModel>>.Success(models);
        }
    }
}
