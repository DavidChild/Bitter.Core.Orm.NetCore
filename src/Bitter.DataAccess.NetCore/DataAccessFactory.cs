using Bitter.DataAccess.SqlClient;

namespace Bitter.DataAccess
{
    public sealed class DataAccessFactory
    {
        public static SqlDataAccess CreateSqlDataAccess(string connString)
        {
            return new SqlDataAccess(connString);
        }

        public static SqlDataAccess CreateSqlDataAccess(DatabaseProperty dp)
        {
            return new SqlDataAccess(dp.Writer.ConnectionString);
        }

        public static SqlDataAccess CreateSqlDataAccessReader(DatabaseProperty dp)
        {
            return new SqlDataAccess(dp.Reader.ConnectionString);
        }

        public static SqlDataAccess CreateSqlDataAccessWriter(DatabaseProperty dp)
        {
            return new SqlDataAccess(dp.Writer.ConnectionString);
        }
    }
}