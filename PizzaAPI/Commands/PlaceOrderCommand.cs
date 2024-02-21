using Database.Repositories.Interfaces;
using Infrastructure.Mediator;
using MediatR;
using PizzaAPI.Dtos.Order;
using PizzaAPI.Mappers;

namespace PizzaAPI.Commands
{
    public static class PlaceOrderCommand
    {
        public class Request :IRequest<WrapperResult>
        {
            public OrderDto Order { get; set; }
        }

        public class Handler : IRequestHandler<Request, WrapperResult>
        {
            private readonly IOrderRepo _orderRepo;

            public Handler(IOrderRepo orderRepo)
            {
                _orderRepo = orderRepo;
            }

            public async Task<WrapperResult> Handle(Request request, CancellationToken cancellationToken)
            {
                try
                {
                    var model = request.Order.ToModel();
                    var pizzas = request.Order.Pizzas.Select(p => p.ToModel()).ToArray();
                    await _orderRepo.Create(model, pizzas);

                    return WrapperResult.Ok();
                }
                catch (Exception ex)
                {
                    return WrapperResult.Failed(ex.Message);
                }
            }
        }
    }
}
