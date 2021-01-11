using System;
using System.Collections.Generic;
using System.Text;
using BT.Manage.Tools.Utils;
using BT.Manage.Tools;
using System.Data;

namespace BT.Manage.Core
{
    public static class SubmitExtension
    {
        #region //Submit--指定多库操作

        private static Int32 GetSubmit(BaseQuery o, string targetdb,out Exception outex)
        {
            //先判断是否是新增,如果不是新增就修改

            outex = null;
            o.Convert(targetdb);  
            Int32 affectedCount = -1;
            try
            {
                if (o.excutParBag.excutEnum == ExcutEnum.Insert && ((ExcutParBag_Insert)o.excutParBag).isOutIdentity)
                {
                    affectedCount = DataAccess.ExecuteScalarToWriterOnlyForInsertRentrunIdentity(o, o.databaseProperty).ToSafeInt32(-1);
                }
                else
                {
                    affectedCount = DataAccess.ExecuteNonQuery(o, o.databaseProperty).ToSafeInt32(-1);
                }

                
            }
            catch (Exception ex)
            {
                outex = ex;
                LogService.Default.Fatal("执行失败：" + ex.Message, ex);
                affectedCount = -1;

            }
            finally
            {

            }
            return affectedCount;

        }

        /// <summary>
        /// 提交到数据库执行,成功：返回受影响的行数,如果执行不成功,返回-1
        /// </summary>
        /// <param name="o">执行的SqlQuery:成功：返回受影响的行数,如果执行不成功,返回-1</param>
        /// <returns>返回受影响的行数</returns>
        public static Int32 Submit(this BaseQuery o, string targetdb)
        {
            Exception ex = null;
            return GetSubmit(o, targetdb, out ex);
        }

        /// <summary>
        /// 提交到数据库执行,成功：返回受影响的行数,如果执行不成功,返回-1
        /// </summary>
        /// <param name="o">执行的SqlQuery:成功：返回受影响的行数,如果执行不成功,返回-1</param>
        /// <returns>返回受影响的行数</returns>
        public static Int32 Submit(this BaseQuery o)
        {
            Exception ex = null;
            return GetSubmit(o, null,out ex);
        }


        /// <summary>
        /// 提交到数据库执行,成功：返回受影响的行数,如果执行不成功,返回-1
        /// </summary>
        /// <param name="o">执行的SqlQuery:成功：返回受影响的行数,如果执行不成功,返回-1</param>
        /// <returns>返回受影响的行数</returns>
        internal static Int32 Submit(this BaseQuery o,out Exception exception)
        {
            return GetSubmit(o, null, out exception);
        }
        /// <summary>
        /// 提交到数据库执行,成功：返回受影响的行数,如果执行不成功,返回-1
        /// </summary>
        /// <param name="o">执行的SqlQuery:成功：返回受影响的行数,如果执行不成功,返回-1</param>
        /// <returns>返回受影响的行数</returns>
        public static Int32 Submit(this BaseQuery o, string targetdb, out Exception exception)
        {
            
            return GetSubmit(o, targetdb, out exception);
        }
        #endregion

        #region //Submit重载--指定多库操作

        internal static bool GetSubmit(List<BaseQuery> list, ScopeCommandInfo scopecommandInfo, out Exception Ex,
            List<BulkCopyModel> bulkModels, List<DataTable> bulkTablies, string targetdb)
        {
            bool bl = true;
            Ex = null;
            if ((list == null || list.Count == 0) && (bulkModels == null || bulkModels.Count == 0)) bl = true;
            else
            {
                try
                {
                     var pty = dc.conn(targetdb);
                      var index = 0;
                      list.ForEach(p =>
                      { 
                          if (p.excutParBag == null)
                          {
                             string error = "错误:core-excutParBag为null.事务语句索引:请检查业务执行语句." + index.ToSafeString();
                             LogService.Default.Fatal(error);
                             throw new  Exception(error);
                          }
                          p.Convert(targetdb);
                          index++;
                       });
                   

                    if (bulkModels != null && bulkModels.Count > 0)
                    {

                        bulkTablies = bulkModels.ConvertBulkTable(targetdb);
                    }
                   
                    if (scopecommandInfo == null)
                    {
                        scopecommandInfo = default(ScopeCommandInfo);
                    }
                 
                    BaseQuery q = null;
                    if (list != null && list.Count > 0)
                    {

                        q = list.ConvertOneQuery();
                        scopecommandInfo.Parameters = q.Parameters;

                        scopecommandInfo.SqlCommand = q.CommandText;
                    }
                    

                    List<BaseQuery> ql = null;
                    if (q != null)
                    {
                        ql = new List<BaseQuery>() { q };
                    }
         
                    bl = ModelOpretion.ExecuteBacheNonQuery(ql, out Ex, bulkTablies, targetdb);
                }
                catch (Exception ex)
                {

                    bl = false;
                    Ex = ex;
                    LogService.Default.Fatal("处理事务语句时出现异常：" + ex.Message);
                }
            }
            return bl;
        }

        internal static bool Submit(this List<BaseQuery> list, ScopeCommandInfo scopecommandInfo, out Exception Ex,
            List<BulkCopyModel> bulkModels = null, List<DataTable> bulkTablies = null)
        {
            Ex = null;
            return GetSubmit(list, scopecommandInfo, out Ex, bulkModels, bulkTablies, null);
        }

        internal static bool Submit(this List<BaseQuery> list, ScopeCommandInfo scopecommandInfo, out Exception Ex,
            List<BulkCopyModel> bulkModels, List<DataTable> bulkTablies, string targetdb)
        {
            Ex = null;
            return GetSubmit(list, scopecommandInfo, out Ex, bulkModels, bulkTablies, targetdb);
        }

        #endregion
    }
}
