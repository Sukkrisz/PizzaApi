using Data.Db.DbAccess;
using Data.Db.Models.Pizza;
using Data.Db.Repositories;
using Data.Db.Repositories.Interfaces;

namespace Core.Data.Repositories
{
    public class ToppingRepo : BaseRepo, IToppingRepo
    {
        private readonly Dictionary<CommonDataAccessTypes, string> _storedProcs;

        public ToppingRepo(ISqlDataAccess db) : base(db)
        {
            _storedProcs = new Dictionary<CommonDataAccessTypes, string>() 
            {
                { CommonDataAccessTypes.Add, "dbo.spTopping_Insert" },
                { CommonDataAccessTypes.Get, "dbo.spTopping_Get" },
                { CommonDataAccessTypes.GetAll, "dbo.spTopping_GetAll" },
                { CommonDataAccessTypes.Update, "dbo.spTopping_Update" },
                { CommonDataAccessTypes.Delete, "dbo.spTopping_Delete" }
            };
        }

        public Task<IEnumerable<ToppingModel>> GetAllToppings()
        {
            return _sqlDataAccess.LoadDataAsync<ToppingModel, dynamic>(_storedProcs[CommonDataAccessTypes.GetAll], new { });
        }

        public async Task<ToppingModel?> GetToppingAsync(int id)
        {
            var results = await _sqlDataAccess.LoadDataAsync<ToppingModel, dynamic>(
                                        _storedProcs[CommonDataAccessTypes.Get],
                                        new { Id = id });
            return results.FirstOrDefault();
        }

        public Task InsertTopping(ToppingModel topping)
        {
            return _sqlDataAccess.SaveDataAsync(
                                _storedProcs[CommonDataAccessTypes.Add],
                                new { topping.Name, topping.Price });
        }

        public Task UpdateTopping(ToppingModel topping)
        {
            return _sqlDataAccess.SaveDataAsync(
                                _storedProcs[CommonDataAccessTypes.Update],
                                topping);
        }

        public Task DeleteTopping(int id)
        {
            return _sqlDataAccess.SaveDataAsync(
                                _storedProcs[CommonDataAccessTypes.Delete],
                                new { Id = id });
        }
    }
}
