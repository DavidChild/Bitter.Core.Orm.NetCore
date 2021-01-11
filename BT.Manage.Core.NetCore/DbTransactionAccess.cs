using System.Data;
using BT.Manage.DataAccess.SqlClient;

namespace BT.Manage.Core
{
    public class DbTransactionAccess
    {
        public DbTransactionAccess(IDbTransaction dbTransaction, SqlDataAccess sqlDataAccess)
        {
            this.dbTransaction = dbTransaction;
            this.sqlDataAccess = sqlDataAccess;
        }

        public IDbTransaction dbTransaction { get; set; }

        public SqlDataAccess sqlDataAccess { get; set; }
    }
}