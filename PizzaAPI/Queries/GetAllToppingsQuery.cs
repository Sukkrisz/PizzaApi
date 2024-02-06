using Infrastructure.Mediator;
using MediatR;
using PizzaAPI.Dtos;

namespace PizzaAPI.Queries
{
    public class GetAllToppingsQuery : IRequest<Result<ToppingDto[]>>
    {
    }
}
