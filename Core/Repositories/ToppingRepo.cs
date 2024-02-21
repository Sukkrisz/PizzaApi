using Dapper;
using Database.Configs;
using Database.DbAccess;
using Database.Models.Pizza;
using Database.Repositories.Interfaces;
using System.Data;

namespace Database.Repositories
{
    public sealed class ToppingRepo : BaseRepo, IToppingRepo
    {
        public ToppingRepo(ISqlDataAccess db) : base(db)
        {
        }

        public Task<IEnumerable<ToppingModel>> GetAllAsync()
        {
            return _sqlDataAccess.LoadDataAsync<ToppingModel, dynamic>(ToppingRepoConstants.Sp_GetAll, new { });
        }

        public async Task<ToppingModel?> GetByIdAsync(int id)
        {
            var results = await _sqlDataAccess.LoadDataAsync<ToppingModel, dynamic>(
                                        ToppingRepoConstants.Sp_GetbyId,
                                        new { Id = id });
            return results.FirstOrDefault();
        }

        public Task InsertAsync(ToppingModel topping)
        {
            return _sqlDataAccess.SaveDataAsync(
                                ToppingRepoConstants.Sp_Insert,
                                new { topping.Name, topping.PrepareTime });
        }

        public async Task<bool> BulkInsertAsync(List<ToppingModel> toppingsToInsert)
        {
            // Create a table UDT and fill it with data. Thisway they can inserted in bulk to spare the line by line inserts
            var toppingsConverted = ConvertToInputToppings(toppingsToInsert);
            var toppingInputs = new
            {
                toppings = toppingsConverted.AsTableValuedParameter(ToppingRepoConstants.ToppingUdtName)
            };

            var numberOfItemsInserted = await _sqlDataAccess.SaveWithOutput(ToppingRepoConstants.Sp_BulkImport, toppingInputs);

            // Return if all the items were successfully inserted or not
            return numberOfItemsInserted == toppingsToInsert.Count;
        }

        public Task UpdateAsync(ToppingModel topping)
        {
            return _sqlDataAccess.SaveDataAsync(
                                ToppingRepoConstants.Sp_Update,
                                topping);
        }

        public Task DeleteAsync(int id)
        {
            return _sqlDataAccess.SaveDataAsync(
                                ToppingRepoConstants.Sp_Delete,
                                new { Id = id });
        }

        // Udt used for bulk imports
        private DataTable ConvertToInputToppings(List<ToppingModel> toppings)
        {
            var res = new DataTable();

            res.Columns.Add(ToppingRepoConstants.NameDbColumn, typeof(string));
            res.Columns.Add(ToppingRepoConstants.PriceDbColumn, typeof(int));

            // Luckily foreach is nowdays as fast, or maybe even faster, as for is ;)
            foreach (var topping in toppings)
            {
                res.Rows.Add(topping.Name, topping.PrepareTime);
            }

            return res;
        }
    }
}
