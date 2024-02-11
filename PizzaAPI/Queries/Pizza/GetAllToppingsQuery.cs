using Infrastructure.Mediator;
using MediatR;
using PizzaAPI.Dtos.Pizza;

namespace PizzaAPI.Queries.Pizza
{
    public class GetAllToppingsQuery : IRequest<Result<ToppingDto[]>>
    {
    }
}
