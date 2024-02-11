using Microsoft.AspNetCore.Mvc;
using PizzaAPI.Dtos.Pizza;
using PizzaAPI.Queries;

namespace PizzaAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ToppingController : BaseController
    {
        /*[HttpGet("GetAll")]
        public async Task<ActionResult<ToppingDto[]>> GetAll()
        {
            try
            {
                var query = new GetAllToppingsQuery();
                var res = await this.Mediator.Send(query);

                return this.FromResult(res);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
            
        }*/

        // POST: ToppingController/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // POST: ToppingController/Edit/5
        [HttpPut]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ToppingController/Delete/5
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            return View();
        }
    }
}
