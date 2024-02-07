using Microsoft.AspNetCore.Mvc;
using PizzaAPI.Dtos.Pizza;
using PizzaAPI.Queries;

namespace PizzaAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PizzaController : BaseController
    {
        [HttpGet("GetAll")]
        public async Task<ActionResult<PizzaDto[]>> GetAll()
        {
            var query = new GetAllPizzasQuery();
            var res = await this.Mediator.Send(query);

            return this.FromResult(res);
        }
    }
}
