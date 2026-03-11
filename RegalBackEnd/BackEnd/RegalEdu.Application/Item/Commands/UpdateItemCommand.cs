using MediatR;
using RegalEdu.Domain.Models;
using RegalEdu.Application.Common.Results;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.Item.Commands
{
    public class UpdateItemCommand : IRequest<Result>
    {
        public required ItemModel ItemModel { get; set; }

        public class Handler : IRequestHandler<UpdateItemCommand, Result>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;
            private readonly ILocalizationService _localizer;

            public Handler(IRegalEducationDbContext context, IMapper mapper, ILocalizationService localizer)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
                _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
            }

            public async Task<Result> Handle(UpdateItemCommand request, CancellationToken cancellationToken)
            {
                var entity = await _context.Items.FirstOrDefaultAsync (x => x.Id == request.ItemModel.Id, cancellationToken);
                if (entity == null)
                    return Result.Failure (_localizer.Format (LocalizationKey.EntityNotFound, "Item"));

                bool dup = await _context.Items.AnyAsync (x =>
                    x.ItemCode == request.ItemModel.ItemCode &&
                    x.Id != request.ItemModel.Id &&
                    !x.IsDeleted, cancellationToken);
                if (dup)
                    return Result.Failure (_localizer.Format ("ItemCodeExists", request.ItemModel.ItemCode));

                _mapper.Map (request.ItemModel, entity);

                var ok = await _context.SaveChangesAsync (cancellationToken) > 0;
                return ok
                    ? Result.Success (_localizer.Format (LocalizationKey.MSG_UPDATE_SUCCESS, _localizer["Item"]))
                    : Result.Failure (_localizer.Format (LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer["Item"]));
            }
        }
    }
}
