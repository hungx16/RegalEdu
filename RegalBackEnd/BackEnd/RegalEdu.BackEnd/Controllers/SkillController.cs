using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegalEdu.Application.Category.Commands;
using RegalEdu.Application.Skill.Commands;
using RegalEdu.Application.Category.Queries;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Api.Controllers
{
    [Authorize]
    public class SkillController : BaseController
    {

        public SkillController(IMediator mediator) : base(mediator)
        {
        }
        [HttpGet("GetAllSkills")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetAllSkills()
        {
            var categories = await Mediator.Send(new GetAllCategoriesQuery
            {
                CategoryType = CategoryType.Skill
            });
            return categories;
        }

        [HttpGet("GetSkillById")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public async Task<ActionResult> GetSkillById([FromQuery] string id)
        {
            var result = await Mediator.Send(new GetCategoryByIdQuery
            {
                Id = id,
                CategoryType = CategoryType.Skill
            });
            return result;
        }


        //[HttpGet("GetDeletedSkills")]
        //[ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //public async Task<ActionResult> GetDeletedSkills()
        //{
        //    var result = await Mediator.Send(new GetDeletedCategoriesQuery
        //    {
        //        CategoryType = CategoryType.Skill
        //    });
        //    return result;
        //}

        [HttpGet("GetPagedSkills")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetPagedSkills([FromQuery] CategoryQuery query)
        {
            query.CategoryType = (byte)CategoryType.Skill;
            var categories = await Mediator.Send(new GetPagedCategoriesQuery { CategoryQuery = query });
            return categories;
        }

        [HttpPost("AddSkill")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> AddSkill([FromBody] CategoryModel categoryModel)
        {
            var result = await Mediator.Send(new AddSkillCommand
            {
                CategoryModel = categoryModel,                
            });

            return result;
        }


        [HttpDelete("DeleteListSkills")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> DeleteListSkills([FromBody] List<string> arrSkillId)
        {

            var result = await Mediator.Send(new DeleteListCategoriesCommand
            {
                ListIds = arrSkillId,
                CategoryType = CategoryType.Skill// gán CategoryType = 3
            });
            return result;
        }


        //[HttpDelete("RestoreListSkills")]
        //[ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //public async Task<ActionResult> RestoreListSkills([FromBody] List<string> arrCategoriesId)
        //{
        //    var result = await Mediator.Send(new RestoreListCategoriesCommand
        //    {
        //        ListIds = arrCategoriesId,
        //        CategoryType = CategoryType.Skill
        //    });
        //    return result;
        //}

        [HttpPut("UpdateSKill")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> UpdateSkill([FromBody] CategoryModel categoryModel)
        {
            var result = await Mediator.Send(new UpdateSkillCommand { CategoryModel = categoryModel });
            return result;
        }
    }
}
