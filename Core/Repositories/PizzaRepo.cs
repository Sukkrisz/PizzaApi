using Database.Configs;
using Database.DbAccess;
using Database.Models.Pizza;
using Database.Repositories.Interfaces;

namespace Database.Repositories
{
    public sealed class PizzaRepo : BaseRepo, IPizzaRepo
    {
        public PizzaRepo(ISqlDataAccess db) : base(db)
        {
        }

        public Task AddAsync(PizzaModel pizza)
        {
            throw new NotImplementedException();
        }

        public Task GenerateDemoPizzasAsync(int numberOfPizzasToCreate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PizzaModel>> GetAllAsync()
        {
            return _sqlDataAccess.LoadDataMultiObjectAsync<PizzaModel, ToppingModel, dynamic>(
                                                                            PizzaRepoConstants.Sp_PizzaGetAll,
                                                                            new { },
                                                                            nameof(PizzaModel.Toppings));
        }

        public async Task<PizzaModel> GetByIdAsync(int pizzaId)
        {
            var queryRes = await _sqlDataAccess.LoadDataMultiObjectAsync<PizzaModel, ToppingModel, dynamic>(
                                                                            PizzaRepoConstants.Sp_PizzaGet,
                                                                            new { PizzaId = pizzaId },
                                                                            nameof(PizzaModel.Toppings));
            return queryRes?.FirstOrDefault();
        }
    }
}
