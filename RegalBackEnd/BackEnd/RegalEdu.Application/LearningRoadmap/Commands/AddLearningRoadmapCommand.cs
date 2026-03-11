using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.LearningRoadMap.Commands
{
    public class AddLearningRoadMapCommand : IRequest<Result>
    {
        public required LearningRoadMapModel LearningRoadMapModel { get; set; }
    }

    public class AddLearningRoadMapCommandHandler : IRequestHandler<AddLearningRoadMapCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly AutoMapper.IMapper _Mapper;
        private readonly ILocalizationService _localizer;
        private readonly IFileService _fileService;

        public AddLearningRoadMapCommandHandler(IRegalEducationDbContext context, AutoMapper.IMapper Mapper, ILocalizationService localizer, IFileService fileService)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _Mapper = Mapper ?? throw new ArgumentNullException (nameof (Mapper));
            _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
            _fileService = fileService ?? throw new ArgumentNullException (nameof (fileService));
        }

        public async Task<Result> Handle(AddLearningRoadMapCommand request, CancellationToken cancellationToken)
        {

            // Ensure _context is not null before passing it to the method
            if (_context is not DbContext dbContext)
            {
                throw new InvalidOperationException (_localizer[LocalizationKey.InvalidDbContextInstance]);
            }
            var model = request.LearningRoadMapModel;
            // Xử lý ảnh: chỉ dựa vào ImageUrl (đã upload tạm từ FE)
            var images = new List<Domain.Entities.Image> ( );
            var imgModels = model.Images ?? new List<ImageDto> ( );

            foreach (var m in imgModels.OrderBy (x => x.SortOrder))
            {
                string finalUrl = m.Path ?? string.Empty;

                if (!string.IsNullOrWhiteSpace (finalUrl) && finalUrl.StartsWith ("temp/", StringComparison.OrdinalIgnoreCase))
                {
                    finalUrl = await _fileService.MoveFileAsync (finalUrl, "images");
                }

                if (!string.IsNullOrWhiteSpace (finalUrl))
                {
                    images.Add (new Domain.Entities.Image
                    {
                        LearningRoadMapId = model.Id,
                        Path = finalUrl,
                        Status = model.Status,
                        IsCover = m.IsCover,
                    });
                }
            }
            var learningRoadMap = _Mapper.Map<Domain.Entities.LearningRoadMap> (request.LearningRoadMapModel);


            await _context.LearningRoadMaps.AddAsync (learningRoadMap, cancellationToken);
            var success = await _context.SaveChangesAsync (cancellationToken) > 0;

            if (success)
            {
                return Result.Success (_localizer.Format (
                    LocalizationKey.MSG_CREATE_SUCCESS,
                    EntityName.LearningRoadMap));
            }
            else
            {
                return Result.Failure (_localizer.Format (
                    LocalizationKey.ERR_SAVE_NO_EFFECT,
                    EntityName.LearningRoadMap));
            }


        }
    }
}
