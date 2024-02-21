using Database.Repositories.Interfaces;
using Infrastructure.Mediator;
using MediatR;
using PizzaAPI.Dtos.Pizza;
using PizzaAPI.Mappers;

namespace PizzaAPI.Queries.Pizza
{
    public static class GetAllPizzasQuery
    {
        public struct Request : IRequest<WrapperResult<Response>>
        {
        }

        public struct Response
        {
            public PizzaDto[] Pizzas { get; set; }
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
                var pizzas = await _pizzaRepo.GetAllAsync();
                if (pizzas.Any())
                {
                    return WrapperResult<Response>.Ok(new Response()
                    {
                        Pizzas = pizzas.Select(p => p.ToDto()).ToArray()
                    });
                }
                else
                {
                    return WrapperResult<Response>.Failed("No Pizzas were found");
                }

            }
        }
    }
}
