using Bitter.Tools;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Xml;
using System.Data.Odbc;
namespace Bitter.Core
{
    public sealed class DataAccess
    {
        private const int DEF_CMD_TIMEOUT = 300;
        private int m_CommandTimeOut = 300;
        private  IDbConnection m_Connection;
        private IDbTransaction m_Trans;
        private DatabaseType dbtype;
        public DataAccess(string connectionString)
            : this()
        {
            this.m_Connection = new SqlConnection(connectionString);
       

            this.Open();
        }

        public DataAccess(string connectionString,DatabaseType databaseType)
          : this()
        {
            this.dbtype = databaseType;
            this.m_Connection =  DbProvider.GetDbConnection(connectionString,databaseType);
            
            this.Open();
        }

        private DataAccess()
        {
        }

        public int CommandTimeOut
        {
            get
            {
                return this.m_CommandTimeOut;
            }
            set
            {
                this.m_CommandTimeOut = value;
            }
        }

        public IDbConnection Connection
        {
            get
            {
                return this.m_Connection;
            }
        }

        public bool IsClosed
        {
            get
            {
                return this.Connection.State.Equals(ConnectionState.Closed);
            }
        }

        public static DataSet ExecuteDataset(BaseQuery q, DatabaseProperty dp)
        {
            string message = "SQL语句:" + q.CommandText + ";Parameters:{";
            if (q.Parameters != null)
            {
                for (int i = 0; i < q.Parameters.Count; i++)
                {
                    message += "\"" + q.Parameters[i].ParameterName + "\"" + ":";
                    message += "\"" + q.Parameters[i].Value + "\"" + ";";
                }
            }
            message += "}";
            DataSet result = new DataSet();
            try
            {
                LogService.Default.Info(message);
                DataAccess sqlDataAccess = new DataAccess(dp.Reader.ConnectionString, dp.Reader.DatabaseType);
                sqlDataAccess.Open();
                result = sqlDataAccess.ExecuteDataset(q);
                sqlDataAccess.Close();
                return result;
            }
            catch (Exception e)
            {
                LogService.Default.Fatal(e);
                LogService.Default.Fatal((message + "\n" + Assembly.GetExecutingAssembly().Location.ToLower()));
                throw e;
            }
        }


       


        public static int ExecuteNonQuery(BaseQuery q, DatabaseProperty dp)
        {
            string message = "SQL语句:" + q.CommandText + ";Parameters:{";
            if (q.Parameters != null)
            {
                for (int i = 0; i < q.Parameters.Count; i++)
                {
                    message += "\"" + q.Parameters[i].ParameterName + "\"" + ":";
                    message += "\"" + q.Parameters[i].Value + "\"" + ";";
                }
            }
            message += "}";

            try
            {
                LogService.Default.Info(message);
                DataAccess sqlDataAccess = new DataAccess(dp.Writer.ConnectionString, dp.Writer.DatabaseType);
                sqlDataAccess.Open();
                int result = sqlDataAccess.ExecuteNonQuery(q);
                sqlDataAccess.Close();
                return result;
            }
            catch (Exception e)
            {
                LogService.Default.Fatal(e);
                LogService.Default.Fatal((message + "\n" + Assembly.GetExecutingAssembly().Location.ToLower()));
                throw e;
            }
        }

        public static IDataReader ExecuteReader(BaseQuery q, DatabaseProperty dp)
        {
            DataAccess da = new DataAccess(dp.Reader.ConnectionString, dp.Reader.DatabaseType);
            return DataAccess.ExecuteReader(q, da);
        }

        public static object ExecuteScalar(BaseQuery q, DatabaseProperty dp, bool isWritable)
        {
            DataAccess da;
            if (isWritable)
            {
                da = new DataAccess(dp.Writer.ConnectionString, dp.Writer.DatabaseType);
            }
            else
            {
                da = new DataAccess(dp.Reader.ConnectionString, dp.Reader.DatabaseType);
            }
            return DataAccess.ExecuteScalar(q, da);
        }

        public static object ExecuteScalar(BaseQuery q, DatabaseProperty dp)
        {
            DataAccess da = new DataAccess(dp.Reader.ConnectionString, dp.Reader.DatabaseType);
            return DataAccess.ExecuteScalar(q, da);
        }
        /// <summary>
        /// 供新增的时候使用,新增并且取出Identity
        /// </summary>
        /// <param name="q"></param>
        /// <param name="dp"></param>
        /// <returns></returns>
        internal static object ExecuteScalarToWriterOnlyForInsertRentrunIdentity(BaseQuery q, DatabaseProperty dp)
        {
            DataAccess da = new DataAccess(dp.Writer.ConnectionString, dp.Writer.DatabaseType);
            return DataAccess.ExecuteScalar(q, da);
        }

        public static XmlDocument ExecuteXmlDoc(BaseQuery q, DatabaseProperty dp)
        {
            DataAccess sqlDataAccess = new DataAccess(dp.Reader.ConnectionString, dp.Reader.DatabaseType);
            sqlDataAccess.Open();
            XmlDocument result = sqlDataAccess.ExecuteXmlDoc(q);
            sqlDataAccess.Close();
            return result;
        }

        public static XmlReader ExecuteXmlReader(BaseQuery q, DatabaseProperty dp)
        {
            DataAccess sqlDataAccess = new DataAccess(dp.Reader.ConnectionString, dp.Reader.DatabaseType);
            sqlDataAccess.Open();
            return sqlDataAccess.ExecuteXmlReader(q);
        }

        public IDbTransaction BeginTransaction()
        {
            this.m_Trans = this.m_Connection.BeginTransaction();
            return this.m_Trans;
        }

        public void Close()
        {
            this.Connection.Close();
        }

        public DataSet ExecuteDataset(BaseQuery q)
        {
            string message = "SQL语句:" + q.CommandText + ";Parameters:{";
            if (q.Parameters != null)
            {
                for (int i = 0; i < q.Parameters.Count; i++)
                {
                    message += "\"" + q.Parameters[i].ParameterName + "\"" + ":";
                    message += "\"" + q.Parameters[i].Value + "\"" + ";";
                }
            }
            message += "}";

            try
            {
                LogService.Default.Info(message);
                DataSet dataSet = new DataSet();
                IDbCommand sqlCommand = DbProvider.GetDbCommand(this.dbtype);
                this.PrepareCommand(sqlCommand, q);
                IDbDataAdapter sqlDataAdapter =  DbProvider.GetDbDataAdapter(dbtype,sqlCommand);
                
                sqlDataAdapter.Fill(dataSet);
                this.SyncParameter(sqlCommand, q);
                sqlCommand.Parameters.Clear();
                return dataSet;
            }
            catch (Exception e)
            {
                 LogService.Default.Fatal(message + "\n" + Assembly.GetExecutingAssembly().Location.ToLower());
                 LogService.Default.Fatal(e);
                throw e;
            }
        }

        public int ExecuteNonQuery(BaseQuery q)
        {
            string message = "SQL语句:" + q.CommandText + ";Parameters:{";
            if (q.Parameters != null)
            {
                for (int i = 0; i < q.Parameters.Count; i++)
                {
                    message += "\"" + q.Parameters[i].ParameterName + "\"" + ":";
                    message += "\"" + q.Parameters[i].Value + "\"" + ";";
                }
            }
            message += "}";

            try
            {
                LogService.Default.Info(message);
                IDbCommand sqlCommand = DbProvider.GetDbCommand(this.dbtype);
                this.PrepareCommand(sqlCommand, q);
                int result = sqlCommand.ExecuteNonQuery();
                this.SyncParameter(sqlCommand, q);
                sqlCommand.Parameters.Clear();
                return result;
            }
            catch (Exception e)
            {
                LogService.Default.Fatal(e);
                LogService.Default.Fatal((message + "\n" + Assembly.GetExecutingAssembly().Location.ToLower()));
                throw e;
            }
        }

        public IDataReader ExecuteReader(BaseQuery q)
        {
            string message = "SQL语句:" + q.CommandText + ";Parameters:{";
            if (q.Parameters != null)
            {
                for (int i = 0; i < q.Parameters.Count; i++)
                {
                    message += "\"" + q.Parameters[i].ParameterName + "\"" + ":";
                    message += "\"" + q.Parameters[i].Value + "\"" + ";";
                }
            }
            message += "}";

            try
            {
                LogService.Default.Info(message);
                IDbCommand sqlCommand = DbProvider.GetDbCommand(this.dbtype);
                this.PrepareCommand(sqlCommand, q);
                IDataReader result = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
                this.SyncParameter(sqlCommand, q);
                sqlCommand.Parameters.Clear();
                return result;
            }
            catch (Exception e)
            {
                LogService.Default.Fatal(e);
                LogService.Default.Fatal((message + "\n" + Assembly.GetExecutingAssembly().Location.ToLower()));
                return null;
            }
        }

        public object ExecuteScalar(BaseQuery q)
        {
            string message = "SQL语句:" + q.CommandText + ";Parameters:{";
            if (q.Parameters != null)
            {
                for (int i = 0; i < q.Parameters.Count; i++)
                {
                    message += "\"" + q.Parameters[i].ParameterName + "\"" + ":";
                    message += "\"" + q.Parameters[i].Value + "\"" + ";";
                }
            }
            message += "}";
            try
            {
                LogService.Default.Info(message);
                IDbCommand sqlCommand = DbProvider.GetDbCommand(this.dbtype);
                this.PrepareCommand(sqlCommand, q);
                object result = sqlCommand.ExecuteScalar();
                this.SyncParameter(sqlCommand, q);
                sqlCommand.Parameters.Clear();
                return result;
            }
            catch (Exception e)
            {
                LogService.Default.Fatal(e);
                LogService.Default.Fatal((message + "\n" + Assembly.GetExecutingAssembly().Location.ToLower()));
                throw e;
            }
        }

        public XmlDocument ExecuteXmlDoc(BaseQuery q)
        {
            throw new NotImplementedException();
        }

        public XmlReader ExecuteXmlReader(BaseQuery q)
        {
            SqlCommand sqlCommand = (SqlCommand)DbProvider.GetDbCommand(this.dbtype);
            this.PrepareCommand(sqlCommand, q);
            XmlReader result = sqlCommand.ExecuteXmlReader();
            this.SyncParameter(sqlCommand, q);
            sqlCommand.Parameters.Clear();
            return result;
        }

        public void Open()
        {
            if (this.IsClosed)
            {
                this.Connection.Open();
            }
        }

        private static IDataReader ExecuteReader(BaseQuery q, DataAccess da)
        {
            da.Open();
            return da.ExecuteReader(q);
        }

        private static object ExecuteScalar(BaseQuery q, DataAccess da)
        {
            da.Open();
            object result = da.ExecuteScalar(q);
            da.Close();
            return result;
        }

        private void PrepareCommand(IDbCommand cmd, BaseQuery q)
        {
            cmd.CommandType = q.CommandType;
            cmd.CommandText = q.CommandText;
            cmd.Connection = this.m_Connection;
            cmd.CommandTimeout = q.CommandTimeout;
            cmd.Transaction = this.m_Trans;
         
            if (q.CommandType == CommandType.Text)
            {

                //if (HttpContext.Current != null && HttpContext.Current.Request != null && HttpContext.Current.Request.Url != null)
                //{
                //    string text = HttpContext.Current.Request.Url.ToString();
                //    if (text.Contains("?"))
                //    {
                //        text = text.Remove(text.IndexOf("?"));
                //    }
                //    cmd.CommandText = cmd.CommandText + "/* URL:" + text + " */";
                //}
                //else
                //{
                //    cmd.CommandText = cmd.CommandText + "/* Location:" + Assembly.GetExecutingAssembly().Location.ToLower() + " */";
                //}
                cmd.CommandText = cmd.CommandText + "/* Location:" + Assembly.GetExecutingAssembly().Location.ToLower() + " */";
            }
            if (q.Parameters.Count > 0)
            {
                int count = q.Parameters.Count;
                for (int i = 0; i < count; i++)
                {
                   var par= cmd.CreateParameter();
                    par.ParameterName = q.Parameters[i].ParameterName;
                    par.Size = q.Parameters[i].Size;
                    par.Value = q.Parameters[i].Value;
                    par.Direction = q.Parameters[i].Direction;
                    par.DbType = q.Parameters[i].DbType;//Utils.SqlTypeToDbType( q.Parameters[i].SqlDbType);
                    cmd.Parameters.Add(par);
                }
            }
        }

        private void SyncParameter(IDbCommand cmd, BaseQuery q)
        {

        }
    }
}