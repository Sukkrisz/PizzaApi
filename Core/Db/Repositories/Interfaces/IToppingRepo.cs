using Data.Db.Models.Pizza;

namespace Data.Db.Repositories.Interfaces
{
    public interface IToppingRepo
    {
        Task DeleteTopping(int id);
        Task<IEnumerable<ToppingModel>> GetAllToppings();
        Task<ToppingModel?> GetToppingAsync(int id);
        Task InsertTopping(ToppingModel topping);
        Task UpdateTopping(ToppingModel topping);
    }
}