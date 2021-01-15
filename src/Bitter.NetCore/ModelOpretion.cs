using Bitter.Tools;
using Bitter.Tools.Helper;
using Bitter.Tools.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Bitter.Core
{
    /********************************************************************************
    ** auth： davidchild
    ** date： 2017/1/9 13:36:31
    ** desc： DataMppingoOpretion
    ** Ver.:  V1.0.0
    ** Copyright (C) 2016 Bitter 版权所有。
    *********************************************************************************/

    internal class ModelOpretion
    {
       
        
     
        
        #region  //ExecuteBacheNonQuery--指定多库操作

        private static bool GetExecuteBacheNonQueryDo(List<BaseQuery> lsSqlCommandQuery, out Exception Ex,
            List<DataTable> dtList, string targetdb)
        {

            DataAccess da = DataAccessFactory.CreateSqlDataAccessWriter(dc.conn(targetdb));
            Ex = null;
            //定义事物执行成功与否
            bool isSuccess = true;
            //记录事务
            bool bl = true;
            //提交事务记录
            bool commit = false;
            //打开事务
            da.Open();

            using (IDbTransaction t = da.BeginTransaction())
            {
                try
                {
                    if (lsSqlCommandQuery != null && lsSqlCommandQuery.Count > 0)
                    {
                        foreach (BaseQuery q in lsSqlCommandQuery)
                        {
                            if (da.ExecuteNonQuery(q) < 0)
                            {
                                bl = false;
                                break;
                            }
                            else continue;
                        }
                    }

                    if (dtList != null && dtList.Count > 0 && bl)
                    {
                        IVdb dbculkcopy = DbProvider.GetVdb(dc.conn(targetdb).Writer.DatabaseType);
                        dbculkcopy.TransationBulkCopy(da, da.Connection, t, dtList, bl);
                    }


                    if (bl) commit = true;
                    else commit = false;
                    if (commit == true)
                    {
                        isSuccess = true;
                        t.Commit();
                    }
                    else
                    {
                        isSuccess = false;
                        t.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    t.Rollback();
                    Ex = ex;
                    LogService.Default.Fatal("数据库事务执行失败：失败原因" + ex.Message, ex);
                    isSuccess = false;


                }
            }
            da.Close();
            return isSuccess;
        }

        private static bool ExecuteBacheNonQueryDo(List<BaseQuery> lsSqlCommandQuery, out Exception Ex,
            List<DataTable> dtList = null)
        {
            Ex = null;
            return GetExecuteBacheNonQueryDo(lsSqlCommandQuery, out Ex, dtList, null);
        }

        private static bool ExecuteBacheNonQueryDo(List<BaseQuery> lsSqlCommandQuery, out Exception Ex,
            List<DataTable> dtList, string targertdb)
        {
            Ex = null;
            return GetExecuteBacheNonQueryDo(lsSqlCommandQuery, out Ex, dtList, targertdb);
        }

        /// <summary>
        /// 执行数据库事物操作
        /// </summary>
        /// <param name="lsSqlCommandQuery">需要执行的SqlQuery集合</param>
        /// <returns>事务执行成功与否</returns>
        public static bool ExecuteBacheNonQuery(List<BaseQuery> lsSqlCommandQuery, List<DataTable> dtList = null
            )
        {

            Exception Ex = new Exception();
            return ExecuteBacheNonQueryDo(lsSqlCommandQuery, out Ex, dtList, null);

        }

        /// <summary>
        /// 执行数据库事物操作
        /// </summary>
        /// <param name="lsSqlCommandQuery">需要执行的SqlQuery集合</param>
        /// <returns>事务执行成功与否</returns>
        public static bool ExecuteBacheNonQuery(List<BaseQuery> lsSqlCommandQuery, List<DataTable> dtList,
            string targetdb
            )
        {

            Exception Ex = new Exception();
            return ExecuteBacheNonQueryDo(lsSqlCommandQuery, out Ex, dtList, targetdb);

        }

        /// <summary>
        /// 执行数据库事物操作
        /// </summary>
        /// <param name="lsSqlCommandQuery">需要执行的SqlQuery集合</param>
        /// <returns>事务执行成功与否</returns>
        public static bool ExecuteBacheNonQuery(List<BaseQuery> lsSqlCommandQuery, out Exception Ex,
            List<DataTable> dtList = null)
        {
            Ex = null;
            return ExecuteBacheNonQueryDo(lsSqlCommandQuery, out Ex, dtList, null);


        }

        /// <summary>
        /// 执行数据库事物操作
        /// </summary>
        /// <param name="lsSqlCommandQuery">需要执行的SqlQuery集合</param>
        /// <returns>事务执行成功与否</returns>
        public static bool ExecuteBacheNonQuery(List<BaseQuery> lsSqlCommandQuery, out Exception Ex,
            List<DataTable> dtList, string targetdb)
        {
            Ex = null;
            return ExecuteBacheNonQueryDo(lsSqlCommandQuery, out Ex, dtList, targetdb);


        }

        #endregion

        
     
    }
}