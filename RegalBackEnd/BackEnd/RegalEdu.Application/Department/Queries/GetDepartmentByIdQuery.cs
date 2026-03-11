using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Department.Queries
{
    public class GetDepartmentByIdQuery : IRequest<Result<DepartmentModel>>
    {
        public required string Id { get; set; }

    }

    public class GetDepartmentByIdQueryHandler : IRequestHandler<GetDepartmentByIdQuery, Result<DepartmentModel>>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public GetDepartmentByIdQueryHandler(IRegalEducationDbContext context, IMapper mapper, ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper)); ;
            _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
        }

        public async Task<Result<DepartmentModel>> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
        {
            // Chỉ lấy bản ghi chưa bị xoá mềm (IsDeleted == false)
            var Department = await _context.Departments
                .AsNoTracking ( )
                .FirstOrDefaultAsync (x => x.Id.ToString ( ) == request.Id && !x.IsDeleted, cancellationToken);

            if (Department == null)
            {
                var msg = _localizer.Format (LocalizationKey.EntityWithIdNotFound, _localizer[EntityName.Department], request.Id);
                return Result<DepartmentModel>.Failure (msg);
            }

            var result = _mapper.Map<DepartmentModel> (Department);
            return Result<DepartmentModel>.Success (result);
        }
    }
}