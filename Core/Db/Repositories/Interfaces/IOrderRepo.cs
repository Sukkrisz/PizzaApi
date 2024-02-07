using Data.Db.Models.Order;

namespace Data.Db.Repositories.Interfaces
{
    public interface IOrderRepo
    {
        Task Create(OrderModel order, int[]PizzaIds);

        Task<OrderModel> GetWithPizzas(int orderId);
    }
}