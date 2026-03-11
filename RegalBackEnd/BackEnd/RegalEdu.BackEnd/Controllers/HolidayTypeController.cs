using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegalEdu.Application.Category.Commands;
using RegalEdu.Application.Category.Queries;
using RegalEdu.Application.HolidayType.Commands;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Api.Controllers
{
    [Authorize]
    public class HolidayTypeController : BaseController
    {

        public HolidayTypeController(IMediator mediator) : base (mediator)
        {
        }
        [HttpGet ("GetAllHolidayTypes")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetAllHolidayTypes( )
        {
            var categories = await Mediator.Send (new GetAllCategoriesQuery
            {
                CategoryType = CategoryType.HolidayType
            });
            return categories;
        }

        [HttpGet ("GetHolidayTypeById")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]

        public async Task<ActionResult> GetHolidayTypeById([FromQuery] string id)
        {
            var result = await Mediator.Send (new GetCategoryByIdQuery
            {
                Id = id,
                CategoryType = CategoryType.HolidayType
            });
            return result;
        }


        [HttpGet ("GetDeletedHolidayTypes")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetDeletedHolidayTypes( )
        {
            var result = await Mediator.Send (new GetDeletedCategoriesQuery
            {
                CategoryType = CategoryType.HolidayType
            });
            return result;
        }

        [HttpGet ("GetPagedHolidayTypes")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetPagedHolidayTypes([FromQuery] CategoryQuery query)
        {
            query.CategoryType = (byte)CategoryType.HolidayType;
            var categories = await Mediator.Send (new GetPagedCategoriesQuery { CategoryQuery = query });
            return categories;
        }

        [HttpPost ("AddHolidayType")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]

        public async Task<ActionResult> AddHolidayType([FromBody] CategoryModel categoryModel)
        {
            var result = await Mediator.Send (new AddHolidayTypeCommand
            {
                CategoryModel = categoryModel,
                AutoCodeType = AutoCodeType.HolidayType
            });

            return result;
        }
        [HttpPut ("UpdateHolidayType")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> UpdateHolidayType([FromBody] CategoryModel categoryModel)
        {
            var result = await Mediator.Send (new UpdateHolidayTypeCommand { CategoryModel = categoryModel });
            return result;
        }
        [HttpDelete ("DeleteListHolidayTypes")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> DeleteListHolidayTypes([FromBody] List<string> arrHolidayTypeId)
        {

            var result = await Mediator.Send (new DeleteListCategoriesCommand
            {
                ListIds = arrHolidayTypeId,
                CategoryType = CategoryType.HolidayType // gán CategoryType = 1
            });
            return result;
        }


        [HttpDelete ("RestoreListHolidayTypes")]
        [ProducesResponseType (typeof (ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> RestoreListHolidayTypes([FromBody] List<string> arrCategoriesId)
        {
            var result = await Mediator.Send (new RestoreListCategoriesCommand
            {
                ListIds = arrCategoriesId,
                CategoryType = CategoryType.HolidayType
            });
            return result;
        }

    }
}
