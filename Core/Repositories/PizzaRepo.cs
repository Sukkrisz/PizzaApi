using Dapper;
using Database.Configs;
using Database.DbAccess;
using Database.Models.Pizza;
using Database.Repositories.Interfaces;
using Infrastructure.Settings;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;
using System.Transactions;

namespace Database.Repositories
{
    public sealed class PizzaRepo : BaseRepo, IPizzaRepo
    {
        private readonly IOptions<ConnectionStringSettings> _settings;

        public PizzaRepo(ISqlDataAccess db, IOptions<ConnectionStringSettings> settings) : base(db)
        {
            _settings = settings;
        }

        public async Task GenerateDemoPizzasAsync(int numberOfPizzasToCreate)
        {
            using (IDbConnection conn = new SqlConnection(_settings.Value.PizzaDb))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        var tasks = new Task[2];
                        tasks[0] = conn.ExecuteAsync(InitConstans.Sp_InitPizzas, new { AmmountOfPizzasToCreate = 1 }, trans);
                        tasks[1] = conn.ExecuteAsync(InitConstans.Sp_InitSizes, transaction: trans);

                        Task.WaitAll(tasks);
                        trans.Commit();
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine($"Error in the db: {ex.Message}");
                        trans.Rollback();
                    }
                }
            }
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
