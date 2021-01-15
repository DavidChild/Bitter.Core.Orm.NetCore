using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Bitter.Core
{
    public class dbHelper
    {
        /// <summary>
        /// 生成billno
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static string GetBillNo(string tableName)
        {
            string str = "";
            try
            {
                DataTable dt = db.FindQuery("exec Prc_GetBillNo @FTableName,@FDate", new { FTableName = tableName, FDate = DateTime.Now }).Find();
                if (dt != null && dt.Rows.Count > 0)
                {
                    str = dt.Rows[0][0].ToString();
                }
            }
            catch (Exception)
            {
                //
            }
            return str;

        }
    }
}
