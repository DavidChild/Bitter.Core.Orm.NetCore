using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;


namespace BT.Manage.Core
{
  internal   interface IVdb
    {
       bool TransationBulkCopy(DataAccess dataAccess, IDbConnection connection, IDbTransaction dbTransaction, List<DataTable> dtList, bool bl);
    }
}
