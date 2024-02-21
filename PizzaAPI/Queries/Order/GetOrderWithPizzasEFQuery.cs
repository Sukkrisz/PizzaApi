using Database.Repositories.Interfaces;
using Infrastructure.Mediator;
using MediatR;
using PizzaAPI.Dtos.Order;
using PizzaAPI.Dtos.Pizza;
using PizzaAPI.Mappers;

namespace PizzaAPI.Queries.Order
{
    public static class GetOrderWithPizzasEFQuery
    {
        public class Request : IRequest<WrapperResult<ResponseEF>>
        {
            public int OrderId { get; set; }
        }

        public struct ResponseEF
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

        public class Handler : IRequestHandler<Request, WrapperResult<ResponseEF>>
        {
            private readonly IOrderRepo _orderRepo;

            public Handler(IOrderRepo orderRepo)
            {
                _orderRepo = orderRepo;
            }

            public async Task<WrapperResult<ResponseEF>> Handle(Request request, CancellationToken cancellationToken)
            {
                var result = await _orderRepo.GetWithPizzasEF(request.OrderId);
                if (result is not null)
                {
                    return WrapperResult<ResponseEF>.Ok(result.ToEFDto());
                }
                else
                {
                    return WrapperResult<ResponseEF>.Failed("Order was not found.");
                }

            }
        }
    }
}
