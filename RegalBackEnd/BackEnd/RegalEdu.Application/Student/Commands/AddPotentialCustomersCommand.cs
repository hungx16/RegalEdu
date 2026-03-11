using AutoMapper;
using MediatR;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Student.Commands
{
    public class AddPotentialCustomersCommand : IRequest<Result>
    {
        public required StudentModel StudentModel { get; set; }
    }

    public class AddPotentialCustomersCommandHandler : IRequestHandler<AddPotentialCustomersCommand, Result>
    {
        private readonly IRegalEducationDbContext _db;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public AddPotentialCustomersCommandHandler(IRegalEducationDbContext db, IMapper mapper, ILocalizationService localizer)
        {
            _db = db; _mapper = mapper; _localizer = localizer;
        }

        public async Task<Result> Handle(AddPotentialCustomersCommand request, CancellationToken ct)
        {
            var m = request.StudentModel;
            var entity = _mapper.Map<Domain.Entities.Student> (m);
            if (m.Contacts != null)
                entity.Contacts = _mapper.Map<List<Domain.Entities.Contact>> (m.Contacts);
            await _db.Students.AddAsync (entity, ct);
            var ok = await _db.SaveChangesAsync (ct) > 0;

            return ok
                ? Result.Success (_localizer.Format (LocalizationKey.MSG_CREATE_SUCCESS, EntityName.Student))
                : Result.Failure (_localizer.Format (LocalizationKey.ERR_SAVE_NO_EFFECT, EntityName.Student));
        }
    }
}
