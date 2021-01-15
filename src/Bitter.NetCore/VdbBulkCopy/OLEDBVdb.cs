using Bitter.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitter.Core
{
    internal class OLEDBVdb : IVdb
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
                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy((SqlConnection)connection,
                                SqlBulkCopyOptions.Default, (SqlTransaction)dbTransaction))
                        {
                            try
                            {
                                bulkCopy.BatchSize = item.Rows.Count;
                                bulkCopy.DestinationTableName = item.TableName;
                                bulkCopy.WriteToServer(item);
                            }
                            catch (Exception ex)
                            {
                                bl = false;
                                LogService.Default.Fatal("SqlBulkCopy 执行失败：失败原因" + ex.Message, ex);
                                throw ex;
                            }
                            finally
                            {
                                bulkCopy.Close();
                            }


                        }
                    }
                }
            }

            return bl;
        }
    }
}
