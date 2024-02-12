using Dapper;
using Data.Db.DbAccess;
using Data.Db.Models.Pizza;
using Data.Db.Repositories;
using Data.Db.Repositories.Interfaces;
using System.Data;

namespace Core.Data.Repositories
{
    public class ToppingRepo : BaseRepo, IToppingRepo
    {
        private const string NAME_DB_COLUMN = "Name";
        private const string PRICE_DB_COLUMN = "Price";
        private const string TOPPING_UDT_NAME = "ToppingUDT";
        private const string BULK_IMPORT_SP_NAME = "dbo.spTopping_BulkInsert";

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

        public Task<IEnumerable<ToppingModel>> GetAllAsync()
        {
            return _sqlDataAccess.LoadDataAsync<ToppingModel, dynamic>(_storedProcs[CommonDataAccessTypes.GetAll], new { });
        }

        public async Task<ToppingModel?> GetByIdAsync(int id)
        {
            var results = await _sqlDataAccess.LoadDataAsync<ToppingModel, dynamic>(
                                        _storedProcs[CommonDataAccessTypes.Get],
                                        new { Id = id });
            return results.FirstOrDefault();
        }

        public Task InsertAsync(ToppingModel topping)
        {
            return _sqlDataAccess.SaveDataAsync(
                                _storedProcs[CommonDataAccessTypes.Add],
                                new { topping.Name, topping.Price });
        }

        public async Task<bool> BulkInsertAsync(List<ToppingModel> toppingsToInsert)
        {
            var toppingsConverted = ConvertToInputToppings(toppingsToInsert);
            var toppingInputs = new
            {
                toppings = toppingsConverted.AsTableValuedParameter(TOPPING_UDT_NAME)
            };

            var numberOfItemsInserted = await _sqlDataAccess.SaveWithOutput(BULK_IMPORT_SP_NAME, toppingInputs);

            // Return if all the items were successfully inserted or not
            return numberOfItemsInserted == toppingsToInsert.Count;
        }

        public Task UpdateAsync(ToppingModel topping)
        {
            return _sqlDataAccess.SaveDataAsync(
                                _storedProcs[CommonDataAccessTypes.Update],
                                topping);
        }

        public Task DeleteAsync(int id)
        {
            return _sqlDataAccess.SaveDataAsync(
                                _storedProcs[CommonDataAccessTypes.Delete],
                                new { Id = id });
        }

        private DataTable ConvertToInputToppings(List<ToppingModel> toppings)
        {
            var res = new DataTable();

            res.Columns.Add(NAME_DB_COLUMN, typeof(string));
            res.Columns.Add(PRICE_DB_COLUMN, typeof(int));

            // Luckily foreach is nowdays as fast, or maybe even faster, as for is ;)
            foreach (var topping in toppings)
            {
                res.Rows.Add(topping.Name, topping.Price);
            }

            return res;
        }
    }
}
