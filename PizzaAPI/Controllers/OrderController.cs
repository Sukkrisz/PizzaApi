using Microsoft.AspNetCore.Mvc;
using PizzaAPI.Commands;
using PizzaAPI.Dtos.Order;
using PizzaAPI.Queries.Order;

namespace PizzaAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : BaseController
    {
        [HttpGet("GetWithPizzasEF")]
        public async Task<ActionResult<GetOrderWithPizzasEFQuery.Response>> GetWithPizzasEF(int orderId)
        {
            var query = new GetOrderWithPizzasEFQuery.Request() { OrderId = orderId };
            var res = await this.Mediator.Send(query);

            return this.FromResult(res);
        }

        [HttpPost()]
        public async Task<IActionResult> PlaceOrder([FromBody] OrderCommandDto order)
        {
            var command = new PlaceOrderCommand.Request() { Order = order };
            var res = await this.Mediator.Send(command);

            return this.FromResult(res);
        }
    }
}
