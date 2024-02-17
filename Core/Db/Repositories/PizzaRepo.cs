using Data.Db.DbAccess;
using Data.Db.Models.Pizza;
using Data.Db.Repositories;
using Data.Db.Repositories.Interfaces;

namespace Core.Data.Repositories
{
    public class PizzaRepo : BaseRepo, IPizzaRepo
    {
        public PizzaRepo(ISqlDataAccess db) : base(db)
        {
            _storedProcs = new Dictionary<CommonDataAccessTypes, string>()
            {
                { CommonDataAccessTypes.Add, "dbo.spPizza_Insert" },
                { CommonDataAccessTypes.Get, "dbo.spPizza_Get" },
                { CommonDataAccessTypes.GetAll, "dbo.spPizza_GetAll" },
                { CommonDataAccessTypes.Update, "dbo.spPizza_Update" },
                { CommonDataAccessTypes.Delete, "dbo.spPizza_Delete" }
            };
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
                                                                            _storedProcs[CommonDataAccessTypes.GetAll],
                                                                            new { },
                                                                            nameof(PizzaModel.Toppings));
        }

        public async Task<PizzaModel> GetByIdAsync(int pizzaId)
        {
            var queryRes = await _sqlDataAccess.LoadDataMultiObjectAsync<PizzaModel, ToppingModel, dynamic>(
                                                                            _storedProcs[CommonDataAccessTypes.Get],
                                                                            new { PizzaId = pizzaId },
                                                                            nameof(PizzaModel.Toppings));
            return queryRes?.FirstOrDefault();
        }
    }
}
