using Infrastructure.Redis;
using Infrastructure.ServiceBus;
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

        [HttpPost("PlaceOrder")]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderCommand.Request order)
        {
            try
            {
                var res = await this.Mediator.Send(order);

                return this.FromWrapperResult(res);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost("SendToKitchen")]
        public async Task<IActionResult> SendToKitchen([FromBody] OrderDto order, IBusMessagePublisher publisher)
        {
            var locations = new string[2] { "Bp", "Pecs" };
            var r = new Random();
            //var locToSend = locations[r.Next(0,2)];
            string locToSend = order.Address.City == "Bp" ? "Bp" : order.Address.City == "Pecs" ? "Pecs" : "test";
            await publisher.SendObjectToTopic(order, "orders", new List<ServiceBusFilter>() { new ServiceBusFilter("location", locToSend) });

            return Ok();
        }
    }
}
