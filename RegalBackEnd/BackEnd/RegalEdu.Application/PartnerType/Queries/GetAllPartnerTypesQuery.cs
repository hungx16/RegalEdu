using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.PartnerType.Queries
{
    public class GetAllPartnerTypesQuery : IRequest<Result<List<PartnerTypeModel>>> { }

    public class Handler_GetAll : IRequestHandler<GetAllPartnerTypesQuery, Result<List<PartnerTypeModel>>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;

        public Handler_GetAll(IRegalEducationDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Result<List<PartnerTypeModel>>> Handle(GetAllPartnerTypesQuery request, CancellationToken cancellationToken)
        {
            var list = await _context.PartnerTypes.AsNoTracking().ToListAsync(cancellationToken);
            return Result<List<PartnerTypeModel>>.Success(_mapper.Map<List<PartnerTypeModel>>(list));
        }
    }
}