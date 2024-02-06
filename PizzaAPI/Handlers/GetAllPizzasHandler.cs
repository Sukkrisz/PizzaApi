using MediatR;
using PizzaAPI.Queries;
using PizzaAPI.Dtos;
using Infrastructure.Mediator;
using PizzaAPI.Mappers;
using Data.Db.Repositories.Interfaces;

namespace PizzaAPI.Handlers
{
    public class GetAllPizzasHandler : IRequestHandler<GetAllPizzasQuery, Result<PizzaDto[]>>
    {
        private readonly IPizzaRepo _repo;

        public GetAllPizzasHandler(IPizzaRepo repo)
        {
            _repo = repo;
        }

        public async Task<Result<PizzaDto[]>> Handle(GetAllPizzasQuery request, CancellationToken cancellationToken)
        {
            var pizzas = await _repo.GetAllAsync();
            var result = pizzas.Select(p => p.ToDto());

            return Result.Ok(result.ToArray());
        }
    }
}