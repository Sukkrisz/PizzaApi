using Infrastructure.Mediator;
using MediatR;
using PizzaAPI.Dtos.Pizza;

namespace PizzaAPI.Queries
{
    public class GetAllPizzasQuery : IRequest<Result<PizzaDto[]>>
    {
    }
}
