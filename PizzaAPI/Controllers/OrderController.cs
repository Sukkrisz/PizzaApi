using Infrastructure.Redis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using PizzaAPI.Commands;
using PizzaAPI.Dtos.Order;
using PizzaAPI.Queries.Order;
using StackExchange.Redis;

namespace PizzaAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : BaseController
    {
        private const string CONTROLLER_CACHE_KEY = "Order";
        private readonly IDistributedCache _redisCache;

        public OrderController(IDistributedCache redisCache)
        {
            _redisCache = redisCache;
        }

        [HttpGet("GetWithPizzas")]
        public async Task<ActionResult<GetOrderWithPizzasQuery.Response>> GetWithPizzas(int orderId)
        {
            var cacheKey = CONTROLLER_CACHE_KEY + $"_{orderId}";
            var cachedRes = await _redisCache.GetRecordAsync<GetOrderWithPizzasQuery.Response?>(cacheKey);

            if (cachedRes is not null)
            {
                return Ok(cachedRes);
            }
            else
            {
                var query = new GetOrderWithPizzasQuery.Request() { OrderId = orderId };
                var res = await this.Mediator.Send(query);

                if (res.IsSuccess)
                {
                    await _redisCache.SetRecordAsync(cacheKey, res.Value);
                }

                return this.FromWrapperResult(res);
            }
        }

        [HttpGet("GetWithPizzasEF")]
        public async Task<ActionResult<GetOrderWithPizzasEFQuery.ResponseEF>> GetWithPizzasEF(int orderId)
        {
            var query = new GetOrderWithPizzasEFQuery.Request() { OrderId = orderId };
            var res = await this.Mediator.Send(query);

            return this.FromWrapperResult(res);
        }

        [HttpPut]
        public void Edit(int orderId)
        {
            var cacheKey = CONTROLLER_CACHE_KEY + $"_{orderId}";
            _redisCache.Remove(cacheKey);

        }

        [HttpPost()]
        public async Task<IActionResult> PlaceOrder([FromBody] OrderCommandDto order)
        {
            try
            {
                var command = new PlaceOrderCommand.Request() { Order = order };
                var res = await this.Mediator.Send(command);

                return this.FromWrapperResult(res);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}
