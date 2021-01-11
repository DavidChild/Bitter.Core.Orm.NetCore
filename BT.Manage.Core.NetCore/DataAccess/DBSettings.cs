
using System.Collections.Generic;
using System.Configuration;
using BT.Manage.Frame.Base.NetCore.ConfigManage;

namespace BT.Manage.Core
{
    public sealed class DBSettings
    {
        private const string dbPath = "connectionString";

        public static string  GetConnectionInfo(string name)
        {

            /*** 对各版本数据库的支持
             *
             * web.config连接字符串中加入providerName特性
             * Aceess数据库--->providerName="System.Data.OleDb"
             * Oracle 数据库--->providerName="System.Data.OracleClient"或者providerName="Oracle.DataAccess.Client"
             * SQLite数据库--->providerName="System.Data.SQLite"
             * sql     数据库--->providerName="System.Data.SqlClient"
             * MySQL数据库--->providerName="MySql.Data.MySqlClient"
             * * **/
            List<connectionStringEntity> list = JsonConfigMange.GetInstance().Settings<List<connectionStringEntity>>(dbPath);

            connectionStringEntity readerconnetionInfo = list.Find(item => item.name == (name + ".Reader"));
            connectionStringEntity writerconnetionInfo = list.Find(item => item.name == (name + ".Writer"));  
            if (readerconnetionInfo == null)
            {
                return string.Empty;
            }
            else
            {
               return writerconnetionInfo.value;
                
            }
        }
 

        public static DatabaseProperty GetDatabaseProperty(string name)
        {
            DatabaseConnection reader = default(DatabaseConnection);
            List<connectionStringEntity> list = JsonConfigMange.GetInstance().Settings<List<connectionStringEntity>>(dbPath);

            connectionStringEntity readerconnetionInfo = list.Find(item => item.name == (name + ".Reader"));
            connectionStringEntity writerconnetionInfo = list.Find(item => item.name == (name + ".Writer"));
            reader.DatabaseType = DatabaseType.MSSQLServer;

            if (readerconnetionInfo == null)
            {
                reader.ConnectionString = string.Empty;
            }
            else
            {
                reader.ConnectionString = readerconnetionInfo.value;
                reader.DatabaseType = DbProvider.GetType(readerconnetionInfo.dbType);
              
            }
            DatabaseConnection writer = default(DatabaseConnection);
            writer.DatabaseType = DatabaseType.MSSQLServer;
           
            if (writerconnetionInfo == null)
            {
                writer.ConnectionString = string.Empty;
            }
            else
            {
                writer.ConnectionString = writerconnetionInfo.value ;
                writer.DatabaseType = DbProvider.GetType(writerconnetionInfo.dbType);
               
            }
            return new DatabaseProperty(reader, writer);
        }
    }
}