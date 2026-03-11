using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegalEdu.Application.Item.Commands;
using RegalEdu.Application.Item.Queries;
using RegalEdu.Domain.Models;

namespace RegalEdu.Api.Controllers
{
    [Authorize]
    public class ItemController : BaseController
    {
        private readonly ILogger<ItemController> _logger;
        public ItemController(ILogger<ItemController> logger, IConfiguration configuration, IMediator mediator)
            : base (mediator)
        {
            _logger = logger ?? throw new ArgumentNullException (nameof (logger));
        }

        [HttpPost ("AddItem")]
        public async Task<ActionResult> AddItem([FromBody] ItemModel model)
            => await Mediator.Send (new AddItemCommand { ItemModel = model });

        [HttpPut ("UpdateItem")]
        public async Task<ActionResult> UpdateItem([FromBody] ItemModel model)
            => await Mediator.Send (new UpdateItemCommand { ItemModel = model });

        [HttpDelete ("DeleteListItem")]
        public async Task<ActionResult> DeleteListItem([FromBody] List<string> ids)
            => await Mediator.Send (new DeleteListItemCommand { ListIds = ids });

        [HttpGet ("GetItemById")]
        public async Task<ActionResult> GetItemById([FromBody] string id)
            => await Mediator.Send (new GetItemByIdQuery { Id = id });

        [HttpGet ("GetPagedItems")]
        public async Task<ActionResult> GetPagedItems([FromQuery] ItemQuery query)
            => await Mediator.Send (new GetPagedItemsQuery { Query = query });

        [HttpGet ("GetAllItems")]
        public async Task<ActionResult> GetAllItems( )
            => await Mediator.Send (new GetAllItemsQuery { });

        [HttpGet ("GetDeletedItems")]
        public async Task<ActionResult> GetDeletedItems( )
            => await Mediator.Send (new GetDeletedItemsQuery { });
    }
}
