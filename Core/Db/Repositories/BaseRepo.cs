using Core.Data.Models;
using Dapper;
using Data.Db.DbAccess;

namespace Data.Db.Repositories
{
    public abstract class BaseRepo
    {
        protected readonly ISqlDataAccess _db;

        public BaseRepo(ISqlDataAccess db)
        {
            _db = db;
        }
    }
}
