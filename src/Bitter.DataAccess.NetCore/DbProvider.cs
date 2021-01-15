using Bitter.Tools;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Web;
using System.Xml;
using System.Data.SqlTypes;
using System.Data.OleDb;

namespace Bitter.DataAccess
{
  internal class DbProvider
    {
        public const  string MSSQLPROVIDER = "System.Data.SqlClient";
        public const string OLEDBPROVIDER = "System.Data.OleDb";
        public const string MYSQLPROVIDER = "MySql.Data.MySqlClient";
        public const string ORACLEPROVIDER = "System.Data.OracleClient";

        public static DatabaseType GetType(string typeInfo)
        {
            DatabaseType dbtype=DatabaseType.MSSQLServer;
            switch (typeInfo)
            {
                case DbProvider.MSSQLPROVIDER:
                    dbtype = DatabaseType.MSSQLServer;
                    break;
                case DbProvider.OLEDBPROVIDER:
                    dbtype = DatabaseType.OleDB;
                    break;
                case DbProvider.MYSQLPROVIDER:
                    dbtype = DatabaseType.MySql;
                    break;
                case DbProvider.ORACLEPROVIDER:
                    dbtype = DatabaseType.Oracle;
                    break;
                default:
                    dbtype = DatabaseType.MSSQLServer;
                    break;
            }
            
            return dbtype;
        }

        //底层操作包括的内容  IDbCommand ,IDbConnection,IDbDataAdapter

        public static IDbCommand GetDbCommand(DatabaseType dbtype)
        {
           
            switch (dbtype)
            {
                case DatabaseType.MSSQLServer:
                    return new SqlCommand();
                    break;
                //case DatabaseType.OleDB:
                //    return new System.Data.   //.OleDbCommand();
                     
                //    break;
                case DatabaseType.MySql:
                    return new MySql.Data.MySqlClient.MySqlCommand();
                    break;
                default:
                    return new SqlCommand();
                    break;
            }
        }

        public static IDbConnection GetDbConnection(string connectionString, DatabaseType dbtype)
        {

            switch (dbtype)
            {
                case DatabaseType.MSSQLServer:
                    return new SqlConnection(connectionString);
                    break;
                //case DatabaseType.OleDB:
                //    return new System.Data.OleDb.OleDbConnection(connectionString);
                //    break;
                case DatabaseType.MySql:
                    return new MySql.Data.MySqlClient.MySqlConnection(connectionString);
                    break;
                default:
                    return new SqlConnection(connectionString);
                    break;
            }
        }
        public static IDbDataAdapter GetDbDataAdapter(DatabaseType dbtype,IDbCommand command)
        {

            switch (dbtype)
            {
                case DatabaseType.MSSQLServer:
                    return new SqlDataAdapter((SqlCommand)command);
                    break;
                //case DatabaseType.OleDB:
                //    return new System.Data.OleDb.OleDbDataAdapter((OleDbCommand)command);
                //    break;
                case DatabaseType.MySql:
                    return new MySql.Data.MySqlClient.MySqlDataAdapter((MySql.Data.MySqlClient.MySqlCommand)command);
                    break;
                default:
                    return new SqlDataAdapter((SqlCommand)command);
                    break;
            }
        }
     }
}
