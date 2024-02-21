using Database.DbAccess;

namespace Database.Repositories
{
    public abstract class BaseRepo
    {
        protected readonly ISqlDataAccess _sqlDataAccess;

        public BaseRepo(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }
    }
}
