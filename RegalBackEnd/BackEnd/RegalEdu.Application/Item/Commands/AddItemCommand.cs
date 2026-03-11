using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.Item.Commands
{
    public class AddItemCommand : IRequest<Result>
    {
        public required ItemModel ItemModel { get; set; }

        public class Handler : IRequestHandler<AddItemCommand, Result>
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

            public async Task<Result> Handle(AddItemCommand request, CancellationToken cancellationToken)
            {
                //bool dup = await _context.Items.AnyAsync (x =>
                //    x.ItemCode == request.ItemModel.ItemCode && !x.IsDeleted, cancellationToken);
                //if (dup)
                //    return Result.Failure (_localizer.Format ("ItemCodeExists", request.ItemModel.ItemCode));

                //var entity = _mapper.Map<RegalEdu.Domain.Entities.Item> (request.ItemModel);
                //await _context.Items.AddAsync (entity, cancellationToken);

                //var ok = await _context.SaveChangesAsync (cancellationToken) > 0;
                //return ok
                //    ? Result.Success (_localizer.Format (LocalizationKey.MSG_CREATE_SUCCESS, _localizer["Item"]))
                //    : Result.Failure (_localizer.Format (LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer["Item"]));

                //var info = AutoCodeConfig.Get (AutoCodeType.Division);
                //// Ensure _context is not null before passing it to the method
                //if (_context is not DbContext dbContext)
                //{
                //    throw new InvalidOperationException (_localizer[LocalizationKey.InvalidDbContextInstance]);
                //}

                var info = AutoCodeConfig.Get (AutoCodeType.Item);
                // Ensure _context is not null before passing it to the method
                if (_context is not DbContext dbContext)
                {
                    throw new InvalidOperationException (_localizer[LocalizationKey.InvalidDbContextInstance]);
                }

                var result = await AutoCodeHelper.CreateWithAutoCodeRetryAsync (
                    info,
                    async (code) =>
                    {
                        var item = _mapper.Map<Domain.Entities.Item> (request.ItemModel);
                        item.ItemCode = code;

                        await _context.Items.AddAsync (item, cancellationToken);
                        var success = await _context.SaveChangesAsync (cancellationToken) > 0;

                        if (success)
                        {
                            return Result.Success (_localizer.Format (LocalizationKey.MSG_CREATE_SUCCESS, EntityName.Item));
                        }
                        else
                        {
                            return Result.Failure (_localizer.Format (LocalizationKey.ERR_SAVE_NO_EFFECT, EntityName.Item));
                        }
                    },
                    dbContext // Pass the validated DbContext instance
                );
                return result;
            }
        }
    }
}
