using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bitter.Tools;

namespace Bitter.Core
{
    public static class CreateOneQuery
    {
        public static BaseQuery ConvertOneQuery(this List<BaseQuery> list)
        {
            StringBuilder builder = new StringBuilder();
            
            SqlQueryParameterCollection lp = new SqlQueryParameterCollection();
            Int32 index = 0;
            try
            {
                list.ForEach(sq =>
                {
                    
                    builder.Append("\n---------SQL语句-------\n");
                    for (Int32 i = 0; i < sq.Parameters.Count; i++)
                    {
                        var parmName = "@sp" + index.ToString();
                        var orParmName = sq.Parameters[i].ParameterName;
                        if (orParmName.IndexOf("@") == -1)
                        {
                            orParmName = "@" + orParmName;
                        }
                        var parm = sq.Parameters[i];
                        sq.CommandText = sq.CommandText.Replace(orParmName, parmName);
                        parm.ParameterName = parmName;
                        lp.Add(parm);
                        index++;
                    }
                    builder.Append(sq.CommandText);
                    builder.Append(";");
                });
                BaseQuery q = new BaseQuery();
                q.CommandText = builder.ToString();
                q.Parameters = lp;
                return q;


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<DataTable> GetConvertBulkTable(this List<BulkCopyModel> list, string target)
        {
            List<DataTable> lt = new List<DataTable>();
            try
            {
                var ls = (from p in list
                          group new { p.CopyModel.GetType().Name } by new { p.CopyModel.GetType().Name }).ToList();
                if (ls == null || ls.Count <= 0)
                {
                    return null;
                }
                foreach (var groupitem in ls)
                {

                    var lk = list.Where(p => p.CopyModel.GetType().Name == groupitem.Key.Name).ToList();
                    if (lk != null && lk.Count > 0)
                    {
                        Type type = lk[0].CopyModel.GetType();
                        DataTable dt = null;
                        dt = lk.ToDataTable(type, target);
                        lt.Add(dt);

                    }
                }

            }
            catch (Exception ex)
            {
                LogService.Default.Fatal("List<BulkCopyModel>---》映射到Table 失败，具体信息：" + ex.Message, ex);
                throw ex;

            }
            return lt;

        }

        internal static List<DataTable> ConvertBulkTable(this List<BulkCopyModel> list)
        {
            return GetConvertBulkTable(list, null);
        }

        internal static List<DataTable> ConvertBulkTable(this List<BulkCopyModel> list, string target)
        {
            return GetConvertBulkTable(list, target);
        }

    }
}
