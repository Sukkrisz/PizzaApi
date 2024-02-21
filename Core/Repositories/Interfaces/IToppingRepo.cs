using Database.Models.Pizza;

namespace Database.Repositories.Interfaces
{
    public interface IToppingRepo
    {
        Task DeleteAsync(int id);
        Task<IEnumerable<ToppingModel>> GetAllAsync();
        Task<ToppingModel?> GetByIdAsync(int id);
        Task InsertAsync(ToppingModel topping);
        Task<bool> BulkInsertAsync(List<ToppingModel> toppingsToInsert);
        Task UpdateAsync(ToppingModel topping);
    }
}