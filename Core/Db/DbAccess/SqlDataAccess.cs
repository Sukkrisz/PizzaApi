using Dapper;
using Data.Db.Models;
using Data.Db.Network;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;
using System.Reflection;

namespace Data.Db.DbAccess
{
    public class SqlDataAccess : ISqlDataAccess
    {
        private readonly IOptions<DbSettings> _settings;

        public SqlDataAccess(IOptions<DbSettings> settings)
        {
            _settings = settings;
        }

        public async Task<IEnumerable<T>> LoadDataAsync<T, P>(
            string spName,
            P parameters)
        {
            using (IDbConnection conn = new SqlConnection(_settings.Value.ConnectionStrings.PizzaDb))
            {
                return await conn.QueryAsync<T>(spName, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        // box and unbox
        // https://stackoverflow.com/questions/9694404/propertyinfo-setvalue-not-working-but-no-errors
        public async Task<IEnumerable<T>> LoadDataMultiObjectAsync<T, C, P>(
                string spName,
                P parameters,
                string childPropertyName
            )
            where T : IModel
            where C : IModel
        {
            // Used in the process of merging multiple parent typed rows into one
            var doesParentExistDict = new Dictionary<int, T>();

            using (IDbConnection conn = new SqlConnection(_settings.Value.ConnectionStrings.PizzaDb))
            {
                // Property of the parent type, which will contain a list of child items
                var propToSet = GetProperty<T>(typeof(T), childPropertyName);

                var result = await conn.QueryAsync<T, C, T>(
                    spName,
                    param: parameters,
                    map: (parent, child) =>
                    {
                        /* If a new parent object is found, set it aside.
                         * Later, if more rows of the same parent are found, their child objects will be added to
                         * the "first set aside" representation of the parent object */
                        if (!doesParentExistDict.TryGetValue(parent.Id, out var parentEntry))
                        {
                            doesParentExistDict.Add(parent.Id, parent);
                            parentEntry = parent;
                        }

                        /* If there is a child object found (and is not only an empty object),
                         * we should append it to the "set aside" parent */
                        if (child.Id != 0)
                        {
                            var childListProp = (List<C>)propToSet.GetValue(parentEntry);
                            if (childListProp is null)
                            {

                                // The property value is only set to a list, if there's at least one child item
                                // (otherwise it will stay null)
                                propToSet.SetValue((object)parentEntry, new List<C>());
                                childListProp = (List<C>)propToSet.GetValue(parentEntry);
                            }

                            childListProp.Add(child);
                        }

                        return parentEntry;
                    });

                return result;
            }
        }

        public async Task SaveDataAsync<T>(string spName, T parameters)
        {
            using (IDbConnection conn = new SqlConnection(_settings.Value.ConnectionStrings.PizzaDb))
            {
                await conn.ExecuteAsync(spName, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public PropertyInfo GetProperty<T>(Type type, string propertyToGet)
        {
            var foundProp = type.GetProperty(propertyToGet);
            if(foundProp is null)
            {
                throw new KeyNotFoundException("The type doesn't have the given property.");
            }

            return foundProp;
        }
    }
}
