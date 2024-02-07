using Data.Db.Repositories.Interfaces;
using Infrastructure.Mediator;
using MediatR;
using PizzaAPI.Dtos.Order;
using PizzaAPI.Mappers;

namespace PizzaAPI.Commands
{
    public static class PlaceOrderCommand
    {
        public class Request :IRequest<Result>
        {
            public OrderCommandDto Order { get; set; }
        }

        public class Handler : IRequestHandler<Request, Result>
        {
            private readonly IOrderRepo _orderRepo;

            public Handler(IOrderRepo orderRepo)
            {
                _orderRepo = orderRepo;
            }

            public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
            {
                var model = request.Order.ToModel();
                await _orderRepo.Create(model, request.Order.PizzaIds);

                return Result.Ok();
            }
        }
    }
}
