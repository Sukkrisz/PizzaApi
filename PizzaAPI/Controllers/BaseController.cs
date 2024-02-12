using Infrastructure.Mediator;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace PizzaAPI.Controllers
{
    public abstract class BaseController : Controller
    {
        private IMediator _mediator;

        protected IMediator Mediator
        {
            get
            {
                if(_mediator != null)
                {
                    return _mediator;
                }
                else
                {
                    _mediator = HttpContext?.RequestServices.GetService<IMediator>();
                    return _mediator;
                }
            }
        }

        protected ActionResult FromWrapperResult(WrapperResult<Unit> res)
        {
            if (res.IsSuccess)
            {
                return this.Ok();
            }
            else
            {
                return this.BadRequest(res.ErrorMessage);
            }
        }

        protected ActionResult<T> FromWrapperResult<T>(WrapperResult<T> res)
        {
            if (res.IsSuccess)
            {
                return this.Ok(res.Value);
            }
            else
            {
                return this.BadRequest(res.ErrorMessage);
            }
        }
    }
}
