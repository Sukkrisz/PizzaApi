using Data.Db.Models.Order;
using ModelLibrary.Shared.Func;

namespace Data.Db.Repositories.Interfaces
{
    public interface IOrderRepo
    {
        Task<OrderModel> GetWithPizzas(int orderId);

        Task<OrderModel> GetWithPizzasEF(int orderId);

        Task<IEnumerable<int>> GetOrdersToPhoneNumber(string phoneNumber);

        Task<IEnumerable<PizzaWithMakeTime>> GetMakeTimes(string phoneNumber, DateTime ordereDate);

        Task Create(OrderModel order, OrderedPizzaModel[] pizzas);

        Task CompleteOrder(string phoneNumber, DateTime orderDate);
    }
}