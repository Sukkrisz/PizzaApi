
using Data.Db.Models;

namespace Data.Db.DbAccess
{
    public interface ISqlDataAccess
    {
        Task<IEnumerable<T>> LoadDataAsync<T, U>(string spName, U parameters);

        Task<IEnumerable<T>> LoadDataMultiObjectAsync<T, C, P>(string spName, P parameters, string childPropertyName)
            where T : IModel
            where C : IModel;

        Task SaveDataAsync<T>(string spName, T parameters);
    }
}