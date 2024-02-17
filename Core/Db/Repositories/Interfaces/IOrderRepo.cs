using Data.Db.Models.Order;
using Models.Shared.Func;

namespace Data.Db.Repositories.Interfaces
{
    public interface IOrderRepo
    {
        Task Create(OrderModel order, OrderedPizzaModel[] pizzas);

        Task<OrderModel> GetWithPizzasEF(int orderId);

        Task<OrderModel> GetWithPizzas(int orderId);

        Task<IEnumerable<int>> GetOrdersToPhoneNumber(string phoneNumber);

        Task<IEnumerable<PizzaWithMakeTime>> GetMakeTimes(string phoneNumber, DateTime ordereDate);
    }
}