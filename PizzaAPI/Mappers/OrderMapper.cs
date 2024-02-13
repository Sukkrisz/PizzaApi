using Data.Db.Models.Order;
using PizzaAPI.Dtos.Order;
using PizzaAPI.Queries.Order;

namespace PizzaAPI.Mappers
{
    public static class OrderMapper
    {
        public static OrderModel ToModel(this OrderDto dto)
        {
            var address = dto.Address.ToModel();
            return new OrderModel
            {
                PhoneNumber = dto.PhoneNumber,
                OrderDate = dto.OrderDate,
                Comment = dto.Comment,
                Address = address
            };
        }

        public static GetOrderWithPizzasEFQuery.ResponseEF ToEFDto(this OrderModel model)
        {
            var pizzas = model.Pizzas!.Select(p => p.ToDto()).ToArray();
            var address = model.Address.ToDto();
            return new GetOrderWithPizzasEFQuery.ResponseEF()
            {
                PhoneNumber = model.PhoneNumber,
                OrderDate = model.OrderDate,
                Comment = model.Comment,
                Address = address,
                Pizzas = pizzas,
            };
        }

        public static GetOrderWithPizzasQuery.Response ToDto(this OrderModel model)
        {
            var pizzas = model.Pizzas!.Select(p => p.ToDto()).ToArray();
            var address = model.Address.ToDto();
            return new GetOrderWithPizzasQuery.Response()
            {
                PhoneNumber = model.PhoneNumber,
                OrderDate = model.OrderDate,
                Comment = model.Comment,
                Address = address,
                Pizzas = pizzas,
            };
        }
    }
}
