using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.LectureType.Queries
{
    public class GetAllLectureTypesQuery : IRequest<Result<List<LectureTypeModel>>>
    {
        public class GetAllLectureTypesQueryHandler : IRequestHandler<GetAllLectureTypesQuery, Result<List<LectureTypeModel>>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;

            public GetAllLectureTypesQueryHandler(IRegalEducationDbContext context, IMapper mapper)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            }

            public async Task<Result<List<LectureTypeModel>>> Handle(GetAllLectureTypesQuery request, CancellationToken cancellationToken)
            {
                var entities = await _context.LectureTypes.Include (t => t.Attachment).AsNoTracking ( ).ToListAsync (cancellationToken);
                var result = _mapper.Map<List<LectureTypeModel>> (entities);
                return Result<List<LectureTypeModel>>.Success (result);
            }
        }
    }
}
