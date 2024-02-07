using Data.Db.DbAccess;

namespace Data.Db.Repositories
{
    public abstract class BaseRepo
    {
        protected Dictionary<DataAccessTypes, string> _storedProcs;

        protected readonly ISqlDataAccess _sqlDataAccess;

        public BaseRepo(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }
    }
}
