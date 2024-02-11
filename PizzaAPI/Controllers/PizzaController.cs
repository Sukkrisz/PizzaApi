using Infrastructure.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Options;
using PizzaAPI.Queries.Pizza;

namespace PizzaAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PizzaController : BaseController
    {
        private readonly IOptions<ConnectionStringSettings> _settings;

        public PizzaController(IOptions<ConnectionStringSettings> settings)
        {
            _settings = settings;
        }

        [HttpGet("Ok")]
        public async Task<ActionResult<string>> GetOk()
        {
            var res = $"setting: {(_settings == null).ToString()}, value: {(_settings?.Value == null).ToString()}, pdb: {(_settings?.Value?.PizzaDb == null)}, pdbV: {_settings?.Value?.PizzaDb}";
            //var res = 

            return Ok(res);
        }

        // In memory cached by pizzaId for 1 minute (default)
        [OutputCache(VaryByQueryKeys = ["pizzaId"])]
        [HttpGet]
        public async Task<ActionResult<GetPizzaByIdQuery.Response>> GetById(int pizzaId)
        {
            var query = new GetPizzaByIdQuery.Request() { PizzaId = pizzaId };
            var res = await this.Mediator.Send(query);

            return this.FromWrapperResult(res);
        }

        // In memory cache for 1 hour (or is evicted on new pizza creation)
        [OutputCache(Duration = 3600, Tags = ["Pizzas"])]
        [HttpGet("GetAll")]
        public async Task<ActionResult<GetAllPizzasQuery.Response>> GetAll()
        {
            try
            {
                var query = new GetAllPizzasQuery.Request();
                var res = await this.Mediator.Send(query);

                return this.FromWrapperResult(res);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Create(IOutputCacheStore cacheStore, CancellationToken cancellationToken)
        {
            cacheStore.EvictByTagAsync("Pizzas", cancellationToken);
            return Ok();
        }
    }
}
