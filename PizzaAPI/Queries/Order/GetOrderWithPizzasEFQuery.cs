using Data.Db.Repositories.Interfaces;
using Infrastructure.Mediator;
using MediatR;
using PizzaAPI.Dtos.Order;
using PizzaAPI.Dtos.Pizza;
using PizzaAPI.Mappers;

namespace PizzaAPI.Queries.Order
{
    public static class GetOrderWithPizzasEFQuery
    {
        public class Request : IRequest<Result<Response>>
        {
            public int OrderId { get; set; }
        }

        public struct Response
        {
            public string PhoneNumber { get; set; }

            public DateTime OrderDate { get; set; }

            public string? Comment { get; set; }

            public AddressDto Address { get; set; }

            public PizzaDto[] Pizzas { get; set; }

            public int Price
            {
                get { return Pizzas.Sum(p => p.BasePrice + p.Toppings.Sum(t => t.Price)); }
            }
        }

        public class Handler : IRequestHandler<Request, Result<Response>>
        {
            private readonly IOrderRepo _orderRepo;

            public Handler(IOrderRepo orderRepo)
            {
                _orderRepo = orderRepo;
            }

            public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
            {
                var result = await _orderRepo.GetWithPizzas(request.OrderId);
                return Result.Ok(result.ToDto());
            }
        }
    }
}
