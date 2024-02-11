
using Data.Db.Models;
using System.Data;

namespace Data.Db.DbAccess
{
    public interface ISqlDataAccess
    {
        Task<IEnumerable<T>> LoadDataAsync<T, U>(string spName, U parameters);

        Task<IEnumerable<T>> LoadDataMultiObjectAsync<T, C, P>(
                string spName,
                P parameters,
                string childPropertyName,
                IDbConnection connection = null)
            where T : IModel
            where C : IModel;

        Task SaveDataAsync<T>(string spName, T parameters);

        Task<int> SaveWithOutput<T>(string spName, T parameters);
    }
}