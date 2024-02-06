using Data.Db.Repositories.Interfaces;
using Infrastructure.Mediator;
using MediatR;
using PizzaAPI.Dtos;
using PizzaAPI.Mappers;
using PizzaAPI.Queries;

namespace PizzaAPI.Handlers
{
    public class GetAllToppingsHandler : IRequestHandler<GetAllToppingsQuery, Result<ToppingDto[]>>
    {
        private readonly IToppingRepo _repo;

        public GetAllToppingsHandler(IToppingRepo toppingRepo)
        {
            _repo = toppingRepo;
        }

        public async Task<Result<ToppingDto[]>> Handle(GetAllToppingsQuery request, CancellationToken cancellationToken)
        {
            var toppings = await _repo.GetAllToppings();
            var result = toppings.Select(t => t.ToDto());

            return Result.Ok(result.ToArray());
        }
    }
}
