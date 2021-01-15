using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bitter.Tools;

namespace Bitter.Core
{

    internal class DistributedTransaction:IDisposable
    {
        private  List<BaseQuery>  _list=new List<BaseQuery>();


        private ScopeCommandInfo _comaCommandInfo=new ScopeCommandInfo();


        public ScopeCommandInfo ComamInfo {
            get { return _comaCommandInfo; }
        }

        public string _targetdb { get; set; }

        /// <summary>
        /// 实例化分布事务的事务提交实例
        /// </summary>
        /// <param name="?"></param>
        public DistributedTransaction()
        {
            
        }
        /// <summary>
        /// 实例化分布事务的事务提交实例
        /// </summary>
        /// <param name="?"></param>
        public DistributedTransaction(string targetdb)
        {
            _targetdb = targetdb;
        }

        public List<BaseQuery> SetListQuery
        {
            set
            {
                if (value != null) _list = value;
                InitCommadInfo();
            }
        }
        /// <summary>
        ///  大批量操作的执行语句
        /// </summary>
        private List<BulkCopyModel> bulkList = new List<BulkCopyModel>();


        public List<BulkCopyModel> SetbulkList
        {

            set
            {

                if (value != null) bulkList = value;
                InitCommadInfo();
            }
        }

        /// <summary>
        ///  大批量操作的DataTable
        /// </summary>
        public List<DataTable> BulkTablies = new List<DataTable>();

        /// <summary>
        /// 最终执行的SQL
        /// </summary>
        private BaseQuery finalSqlQuery=new BaseQuery();


        /// <summary>
        /// 事务最终是否成功,默认为false
        /// </summary>
        public bool IsScopeSucessed = false;


        /// <summary>
        /// 创建一个数据库线程连接
        /// </summary>
        private DataAccess da;


          public delegate void EventDistributedTransaction(object sender);
        /// <summary>
        ///  发生异常事件
        /// </summary>
       public event EventDistributedTransaction DistributedEventException;


       private Exception _ex { get; set; }

        private Exception DistributedEx 
        {
            set
            {
                 _ex = value;
                if (DistributedEventException != null)
                {
                    DistributedEventException(_ex);
                }
            }
        }

       


        /// <summary>
        /// 实例化数据
        /// </summary>
        void  InitCommadInfo()
        {
            bool bl = true;
            if (_list == null || _list.Count == 0) bl = true;
            else
            {
                    try
                    {
                        BaseQuery q=  _list.ConvertOneQuery();
                        finalSqlQuery = q;
                        _comaCommandInfo.Parameters = q.Parameters;
                        _comaCommandInfo.SqlCommand = q.CommandText;

                    }
                    catch (Exception ex)
                    {
                       DistributedEx = ex;
                        LogService.Default.Fatal("处理事务语句时出现异常：" + ex.Message);
                    }
              }
            
        }


        /// <summary>
        /// 实例化数据
        /// </summary>
        void InitBulkTablies()
        {
            bool bl = true;
            if (bulkList == null || bulkList.Count == 0) bl = true;
            else
            {
                try
                {
                     this.BulkTablies = bulkList.ConvertBulkTable();
                  

                }
                catch (Exception ex)
                {
                    DistributedEx = ex;
                    LogService.Default.Fatal("处理事务语句时出现异常：" + ex.Message);
                }
            }

        }



        /// <summary>
        /// 本地事务第一次提交是否成功,默认为true
        /// </summary>
        private bool insideSucessed = true;
        /// <summary>
        /// 本地事务第一次提交是否成功,默认为false
        /// </summary>
        public bool InsideSucessed 
        {
              get
              {
                  return insideSucessed;
              }
              set
              {
                  if (!value)
                  {
                      IsScopeSucessed = false;
                      insideSucessed = false;
                      if ((idbTransaction != null)&&(idbTransaction.Connection!=null))
                      {
                          idbTransaction.Rollback();
                         
                      }
                      ClosedConn();
                     
                  }
                  else
                  {
                      insideSucessed = true;
                  }
              }
        }

        /// <summary>
        /// 事务回滚
        /// </summary>
        public void Rollback()
        {
            if ((idbTransaction != null) && (idbTransaction.Connection != null))
            {
                idbTransaction.Rollback();

            }
            ClosedConn();
        }


        /// <summary>
        /// 分布式事务提交,二次提交依赖外部条件,二次提交时一定要注意外部事务提交或者应用执行的时间
        /// 如果外部事务执行时间过长，会导致本事务的连接线程占用过长,会出现数据库连接泄露的情况出现。
        /// 默认情况为true,具体情况请根据当时业务场景处理
        /// </summary>
        public bool ExternalSucessed = true;

        /// <summary>
        /// 事务
        /// </summary>
        private IDbTransaction idbTransaction;

        internal bool BeginTransaction()
        {
            List<BaseQuery> sqlQueries=new List<BaseQuery>();
            List<DataTable> cBulkTablies = new List<DataTable>();
            if (BulkTablies != null && BulkTablies.Count > 0)
            {
                cBulkTablies = BulkTablies;
            }
            if (finalSqlQuery == null)
            {
              sqlQueries=new List<BaseQuery>();
            }
            else
            {
                sqlQueries=new List<BaseQuery>(){finalSqlQuery};
            }
            idbTransaction = GetBeginTransaction(sqlQueries, cBulkTablies, _targetdb);
            if (idbTransaction == null || (!insideSucessed)) return false;
            else return true;

        }


        /// <summary>
        /// 关闭连接
        /// </summary>
        private void ClosedConn()
        {
            if (da != null)
            {
                if (!da.IsClosed)
                {
                    da.Close();
                }
            }
        }


        /// <summary>
        /// 事务进行第二次提交
        /// </summary>
        internal void CommitTransaction()
        {
            if ((!InsideSucessed)||(_ex != null || idbTransaction.Connection == null))
            {
                ClosedConn();
                return;
                
            }
          

            if (InsideSucessed && ExternalSucessed)
            {
                if (idbTransaction != null)
                {
                     idbTransaction.Commit();
                     IsScopeSucessed = true;
                }
              
            }
            else
            {
                if (idbTransaction != null)
                {
                    idbTransaction.Rollback();
                    IsScopeSucessed = false;
                }
            }
            if (idbTransaction != null)
            {
                 idbTransaction.Dispose();
            }
            ClosedConn();
        }


       


        /// <summary>
        /// 执行数据库事物操作
        /// </summary>
        /// <param name="lsSqlCommandQuery">需要执行的SqlQuery集合</param>
        /// <returns>事务执行成功与否</returns>
        internal IDbTransaction GetBeginTransaction(List<BaseQuery> lisqQueries,List<DataTable> dtList =null,string targetdb=null)
        {
            if ((lisqQueries == null || lisqQueries.Count == 0) && (dtList==null||dtList.Count==0))
            {
                IsScopeSucessed = true;//如果是零条,也是成功
                return null;
            }
            var dbp = dc.conn(targetdb);
            da = DataAccessFactory.CreateSqlDataAccessWriter(dbp);
            
            //记录事务
            bool bl = true;

            int index = 0;
           
            //打开事务
            da.Open();
            idbTransaction = da.BeginTransaction();
            {
                try
                {
                    if (_list != null && _list.Count > 0)
                    {
                        foreach (BaseQuery q in _list)
                        {
                            if (da.ExecuteNonQuery(q) < 0)
                            {
                                bl = false;

                                break;
                            }
                            else continue;
                        }
                    }
                  //处理BULKCOPY
                   bl= DbProvider.GetVdb(dbp.Writer.DatabaseType).TransationBulkCopy(da, idbTransaction.Connection, idbTransaction,dtList,bl);
                   InsideSucessed = bl;
                }
                catch (Exception ex)
                {
                  
                  DistributedEx = ex;
                  LogService.Default.Fatal("数据库事务执行失败：失败原因" + ex.Message, ex);
                  InsideSucessed = false;
                }
            }
            return idbTransaction;
        }

        /// <summary>
        /// 主动销毁数据库连接资源
        /// </summary>
        public void Dispose()
        {
            if (idbTransaction != null)
            {
                idbTransaction.Dispose();
            }

            ClosedConn();
        }
    }
}
