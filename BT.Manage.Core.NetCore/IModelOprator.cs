using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BT.Manage.Core
{
   public  interface IModelOprator
    {
        int Delete(Column keyColumn, Table table, params int[] ids);
        int InsertEntities(ArrayList list);
        int UpdateValues(Column keyColumn, Table table, Dictionary<string, object> values);
    }
}
