using Data.Db.Repositories.Interfaces;
using Infrastructure.Mediator;
using MediatR;
using PizzaAPI.Dtos.Order;
using PizzaAPI.Mappers;

namespace PizzaAPI.Commands
{
    public static class PlaceOrderCommand
    {
        public class Request :IRequest<MyResult>
        {
            public OrderCommandDto Order { get; set; }
        }

        public class Handler : IRequestHandler<Request, MyResult>
        {
            private readonly IOrderRepo _orderRepo;

            public Handler(IOrderRepo orderRepo)
            {
                _orderRepo = orderRepo;
            }

            public async Task<MyResult> Handle(Request request, CancellationToken cancellationToken)
            {
                try
                {
                    var model = request.Order.ToModel();
                    await _orderRepo.Create(model, request.Order.PizzaIds);

                    return MyResult.Ok();
                }
                catch (Exception ex)
                {
                    return MyResult.Failed(ex.Message);
                }
            }
        }
    }
}
