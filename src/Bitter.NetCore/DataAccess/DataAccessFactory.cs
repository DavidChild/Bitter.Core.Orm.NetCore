

namespace Bitter.Core
{
    public sealed class DataAccessFactory
    {
        public static DataAccess CreateSqlDataAccess(string connString)
        {
            return new DataAccess(connString);
        }

        public static DataAccess CreateSqlDataAccess(DatabaseProperty dp)
        {
            return new DataAccess(dp.Writer.ConnectionString);
        }

        public static DataAccess CreateSqlDataAccessReader(DatabaseProperty dp)
        {
            return new DataAccess(dp.Reader.ConnectionString);
        }

        public static DataAccess CreateSqlDataAccessWriter(DatabaseProperty dp)
        {
            return new DataAccess(dp.Writer.ConnectionString);
        }
    }
}