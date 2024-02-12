using Data.Db.Models.Pizza;
using Data.Db.Repositories.Interfaces;
using Infrastructure.Mediator;
using MediatR;
using PizzaAPI.Dtos.Pizza;
using PizzaAPI.Mappers;

namespace PizzaAPI.Queries.Pizza
{
    public class GetPizzaByIdQuery
    {
        public struct Request : IRequest<WrapperResult<Response>>
        {
            public int PizzaId { get; set; }
        }

        public struct Response
        {
            public PizzaDto Pizza { get; set; }
        }

        public class Handler : IRequestHandler<Request, WrapperResult<Response>>
        {
            private readonly IPizzaRepo _pizzaRepo;

            public Handler(IPizzaRepo pizzaRepo)
            {
                _pizzaRepo = pizzaRepo;
            }

            public async Task<WrapperResult<Response>> Handle(Request request, CancellationToken cancellationToken)
            {
                var res = await _pizzaRepo.GetByIdAsync(request.PizzaId);
                if (res is not null)
                {
                    return WrapperResult<Response>.Ok(new Response()
                    {
                        Pizza = res.ToDto()
                    });
                }
                else
                {
                    return WrapperResult<Response>.Failed($"No pizza with Id:{request.PizzaId} could be found.");
                }
            }
        }
    }
}
