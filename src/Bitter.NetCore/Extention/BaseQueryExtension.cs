using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Bitter.Core
{
   internal   static class BaseQueryExtension
    {
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        internal static DataTable ReturnDataTable(this BaseQuery o,int tableIndex=0)
        {
            o.Convert(o.Targetdb);
            DataSet Ds = DataAccess.ExecuteDataset(o, o.databaseProperty);
            if (Ds != null && Ds.Tables.Count > 0)
            {
                if (tableIndex > 0)
                {
                    return Ds.Tables[tableIndex];
                }
                else
                {
                    return Ds.Tables[0];
                }
                
            }
            return null;
        }
        internal static bool  Excut(this BaseQuery o)
        {
            o.Convert(o.Targetdb);
             int i= DataAccess.ExecuteNonQuery(o, o.databaseProperty);
            if (i >= 0)
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
