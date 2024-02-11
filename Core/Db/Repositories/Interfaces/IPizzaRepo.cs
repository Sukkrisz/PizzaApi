using Data.Db.Models.Pizza;

namespace Data.Db.Repositories.Interfaces
{
    public interface IPizzaRepo
    {
        public Task<PizzaModel> GetByIdAsync(int orderId);

        public Task<IEnumerable<PizzaModel>> GetAllAsync();

        /*public Task AddAsync(PizzaModel pizza);

        public Task GenerateDemoPizzasAsync(int numberOfPizzasToCreate);*/
    }
}
