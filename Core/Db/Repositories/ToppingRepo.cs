using Core.Data.Models;
using Data.Db.DbAccess;
using Data.Db.Repositories;
using Data.Db.Repositories.Interfaces;

namespace Core.Data.Repositories
{
    public class ToppingRepo : BaseRepo, IToppingRepo
    {
        private readonly Dictionary<DataAccessTypes, string> _storedProcs;

        public ToppingRepo(ISqlDataAccess db) : base(db)
        {
            _storedProcs = new Dictionary<DataAccessTypes, string>() 
            {
                { DataAccessTypes.Add, "dbo.spTopping_Insert" },
                { DataAccessTypes.Get, "dbo.spTopping_Get" },
                { DataAccessTypes.GetAll, "dbo.spTopping_GetAll" },
                { DataAccessTypes.Update, "dbo.spTopping_Update" },
                { DataAccessTypes.Delete, "dbo.spTopping_Delete" }
            };
        }

        public Task<IEnumerable<ToppingModel>> GetAllToppings()
        {
            return _db.LoadDataAsync<ToppingModel, dynamic>(_storedProcs[DataAccessTypes.GetAll], new { });
        }

        public async Task<ToppingModel?> GetToppingAsync(int id)
        {
            var results = await _db.LoadDataAsync<ToppingModel, dynamic>(
                                        _storedProcs[DataAccessTypes.Get],
                                        new { Id = id });
            return results.FirstOrDefault();
        }

        public Task InsertTopping(ToppingModel topping)
        {
            return _db.SaveDataAsync(
                                _storedProcs[DataAccessTypes.Add],
                                new { topping.Name, topping.Price });
        }

        public Task UpdateTopping(ToppingModel topping)
        {
            return _db.SaveDataAsync(
                                _storedProcs[DataAccessTypes.Update],
                                topping);
        }

        public Task DeleteTopping(int id)
        {
            return _db.SaveDataAsync(
                                _storedProcs[DataAccessTypes.Delete],
                                new { Id = id });
        }
    }
}
