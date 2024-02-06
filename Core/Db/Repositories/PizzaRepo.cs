using Core.Data.Models;
using Data.Db.DbAccess;
using Data.Db.Repositories;
using Data.Db.Repositories.Interfaces;

namespace Core.Data.Repositories
{
    public class PizzaRepo : BaseRepo, IPizzaRepo
    {
        private readonly Dictionary<DataAccessTypes, string> _storedProcs;

        public PizzaRepo(ISqlDataAccess db) : base(db)
        {
            _storedProcs = new Dictionary<DataAccessTypes, string>()
            {
                { DataAccessTypes.Add, "dbo.spTopping_Insert" },
                { DataAccessTypes.Get, "dbo.spTopping_Get" },
                { DataAccessTypes.GetAll, "dbo.spPizza_GetAll" },
                { DataAccessTypes.Update, "dbo.spTopping_Update" },
                { DataAccessTypes.Delete, "dbo.spTopping_Delete" }
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
            return _db.LoadDataMultiObjectAsync<PizzaModel, ToppingModel, dynamic>(
                                                                            _storedProcs[DataAccessTypes.GetAll],
                                                                            new { },
                                                                            nameof(PizzaModel.Toppings));
        }

        /*public async Task GenerateDemoPizzas(int numberOfPizzasToCreate)
        {
            var spName = "spCreate_DemoPizzas";
            var p = new DynamicParameters();
            p.Add("@lastCreatedId", 0, DbType.Int32, direction: ParameterDirection.Output);
            p.Add("@@ammountOfPizzasToCreate", numberOfPizzasToCreate);
            p.Add("@output", DbType.Int32, direction: ParameterDirection.ReturnValue);

            await this.ExecuteSpAsync(spName, p);
        }*/
    }
}
