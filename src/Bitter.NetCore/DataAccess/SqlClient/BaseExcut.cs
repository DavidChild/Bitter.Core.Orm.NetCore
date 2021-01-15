using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Bitter.Core
{
    public class BaseExcut
    {
        public BaseExcut()
        {
        }
        /// <summary>
        /// 执行操作时间
        /// </summary>
        protected const int COMMAND_TIMEOUT_DEFAULT = 300;
        /// <summary>
        /// 最终执行语句
        /// </summary>
        protected string m_CommandText;
        /// <summary>
        /// 超时时间
        /// </summary>
        protected int m_CommandTimeout = 300;
        /// <summary>
        /// 执行类型
        /// </summary>
        protected CommandType m_CommandType = CommandType.Text;
        /// <summary>
        /// SQL 参数
        /// </summary>
        protected SqlQueryParameterCollection m_Parameters = new SqlQueryParameterCollection();
        /// <summary>
        /// 指向哪个操作数据库
        /// </summary>
        private string _targetdb { get; set; }
        private DatabaseProperty _dbpty { get; set; }

        private DatabaseType _dbtype { get; set; }

        public  DatabaseType databaseType
        {

           get
            {
                if (_dbpty == null)
                {

                    _dbpty = dc.conn(_targetdb);
                    _dbtype = _dbpty.Reader.DatabaseType;
                    return _dbtype;

                }
                if (!string.IsNullOrEmpty(_targetdb))
                {
                    return _dbtype;
                }
                return 0;
               
            }
        }
        public DatabaseProperty databaseProperty
        {

            get
            {
                if (_dbpty == null)
                {

                    _dbpty = dc.conn(_targetdb);
                    _dbtype = _dbpty.Reader.DatabaseType;
                    return _dbpty;

                }
                return _dbpty;
            }
        }

        internal virtual   ExcutParBag excutParBag { get;set;}
        public string Targetdb
        {
            get { return _targetdb; }
           
        }


        internal virtual string CommandText
        {
            get
            {
                return this.m_CommandText;
            }
            set
            {
                this.m_CommandText = value;
            }
        }

        public int CommandTimeout
        {
            get
            {
                return this.m_CommandTimeout;
            }
            set
            {
                if (value < 10)
                {
                    this.m_CommandTimeout = 300;
                    return;
                }
                this.m_CommandTimeout = value;
            }
        }

        internal CommandType CommandType
        {
            get
            {
                return this.m_CommandType;
            }
            set
            {
                this.m_CommandType = value;
            }
        }

        public SqlQueryParameterCollection Parameters
        {
            get
            {
                return this.m_Parameters;
            }
            set
            {
                this.m_Parameters = value;
            }
        }

        protected void SetTargetDb(string targetdb)
        {
            this._targetdb = targetdb;
        }

        
    }
}
