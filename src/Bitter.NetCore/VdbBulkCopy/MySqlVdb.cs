using Bitter.Tools;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitter.Core
{
   internal  class MySqlVdb : IVdb
    {
        public bool TransationBulkCopy(DataAccess dataAccess, IDbConnection connection, IDbTransaction dbTransaction, List<DataTable> dtList, bool bl)
        {
            //处理大批量语句

            if (dtList != null && dtList.Count > 0 && bl)
            {
                foreach (var item in dtList)
                {
                    if (item != null && item.Rows.Count > 0)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("INSERT INTO " + item.TableName + "(");
                        for (int i = 0; i < item.Columns.Count; i++)
                        {
                            sb.Append(item.Columns[i].ColumnName + ",");
                        }
                        sb.Remove(sb.ToString().LastIndexOf(','), 1);
                        sb.Append(") VALUES ");
                        for (int i = 0; i < item.Rows.Count; i++)
                        {
                            sb.Append("(");
                            for (int j = 0; j < item.Columns.Count; j++)
                            {
                                sb.Append("'" + item.Rows[i][j] + "',");
                            }
                            sb.Remove(sb.ToString().LastIndexOf(','), 1);
                            sb.Append("),");
                        }
                        sb.Remove(sb.ToString().LastIndexOf(','), 1);
                        sb.Append(";");
                        BaseQuery sql = new BaseQuery();
                        sql.CommandText = sb.ToString();
                        int res = -1;
                         try
                        {
                            res = dataAccess.ExecuteNonQuery(sql);
                        }
                        catch (Exception ex)
                        {
                            res = -1;
                            // Unknown column 'names' in 'field list' 
                            LogService.Default.Fatal( "操作失败！" + ex.Message.Replace("Unknown column", "未知列").Replace("in 'field list'", "存在字段集合中！"));
                        }
                        if (res >= 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                     }
                }
            }


            return bl;
        }

        
    }
}

                                                                  



    

