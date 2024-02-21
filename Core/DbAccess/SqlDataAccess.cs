using Dapper;
using Database.Models;
using Infrastructure.Settings;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;
using System.Reflection;

namespace Database.DbAccess
{
    public sealed class SqlDataAccess : ISqlDataAccess
    {
        private readonly IOptions<ConnectionStringSettings> _settings;

        public SqlDataAccess(IOptions<ConnectionStringSettings> settings)
        {
            _settings = settings;
        }

        public async Task<IEnumerable<T>> LoadDataAsync<T, P>(
            string spName,
            P parameters)
        {
            using (IDbConnection conn = new SqlConnection(_settings.Value.PizzaDb))
            {
                return await conn.QueryAsync<T>(spName, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        // box and unbox for usability with structs as well
        // https://stackoverflow.com/questions/9694404/propertyinfo-setvalue-not-working-but-no-errors
        public async Task<IEnumerable<T>> LoadDataMultiObjectAsync<T, C, P>(
                string spName,
                P parameters,
                string childPropertyName,
                IDbConnection conn = null)
            where T : IModel
            where C : IModel
        {
            // Sometimes the caller fucntion already has a connection open, which can be used (sparing some perf)
            if (conn != null)
            {
                return await DoLoadDataMultiObjectAsync<T, C, P>(spName, parameters, childPropertyName, conn);
            }
            else
            {
                using (conn = new SqlConnection(_settings.Value.PizzaDb))
                {
                    return await DoLoadDataMultiObjectAsync<T, C, P>(spName, parameters, childPropertyName, conn);
                }
            }
        }

        public async Task<int> SaveWithOutput<T>(string spName, T parameters)
        {
            using (IDbConnection conn = new SqlConnection(_settings.Value.PizzaDb))
            {
                return await conn.QuerySingleAsync<int>(spName, param: parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task SaveDataAsync<T>(string spName, T parameters)
        {
            using (IDbConnection conn = new SqlConnection(_settings.Value.PizzaDb))
            {
                await conn.ExecuteAsync(spName, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        private async Task<IEnumerable<TParent>> DoLoadDataMultiObjectAsync<TParent, TChild, TParam>(
                string spName,
                TParam parameters,
                string childPropertyName,
                IDbConnection conn)
            where TParent : IModel
            where TChild : IModel
        {
            var result = await conn.QueryAsync(
                spName,
                param: parameters,
                map: GetMapper<TParent, TChild>(childPropertyName));

            return result;
        }

        private Func<TParent, TChild, TParent> GetMapper<TParent, TChild>(string childPropertyName)
            where TParent : IModel
            where TChild : IModel
        {
            // Used in the process of merging multiple parent typed rows into one
            var foundParents = new Dictionary<int, TParent>();

            // Property of the parent type, which will contain a list of child items
            var propToSet = GetProperty<TParent>(typeof(TParent), childPropertyName);

            return (parent, child) =>
            {
                /* If a new parent object is found, set it aside.
                 * Later, if more rows of the same parent are found, their child objects will be added to
                 * the "first set aside" representation of the parent object */
                if (!foundParents.TryGetValue(parent.Id, out var parentEntry))
                {
                    foundParents.Add(parent.Id, parent);
                    parentEntry = parent;
                }

                /* If there is a child object found (and is not only an empty object),
                 * we should append it to the "set aside" parent */
                if (child.Id != 0)
                {
                    var childListProp = (List<TChild>)propToSet.GetValue(parentEntry);
                    if (childListProp is null)
                    {

                        // The property value is only set to a list, if there's at least one child item
                        // (otherwise it will stay null)
                        propToSet.SetValue(parentEntry, new List<TChild>());
                        childListProp = (List<TChild>)propToSet.GetValue(parentEntry);
                    }

                    childListProp.Add(child);
                }

                return parentEntry;
            };
        }

        // Retrieves a given property's propertyInfo from a type.
        // This will be used to be able to genericly assign values to a property, which's name we know
        private PropertyInfo GetProperty<T>(Type type, string propertyToGet)
        {
            var foundProp = type.GetProperty(propertyToGet);
            if (foundProp is null)
            {
                throw new KeyNotFoundException("The type doesn't have the given property.");
            }

            return foundProp;
        }
    }
}
