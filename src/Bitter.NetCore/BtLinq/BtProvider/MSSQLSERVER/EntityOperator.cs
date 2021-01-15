using System.Collections;
using System.Collections.Generic;
using BT.Manage.Core;
using BT.Manage.DataAccess;

namespace BT.Manage.Core.Provider.MSSQLServer
{
    internal class ModelOperator : IModelOprator
    {
        private ProviderBase _provider = ProviderFactory.CreateProvider(dc.dbconn(string.Empty).Reader.DatabaseType);
        private SqlBuilderBase _sqlBuilder;


        public int Delete(Column keyColumn, Table table, params int[] ids)
        {
            return 1;
        }

        public int InsertEntities(ArrayList list)
        {
            return 1;
        }

        public int UpdateValues(Column keyColumn, Table table, Dictionary<string, object> values)
        {
            return 1;
        }
    }
}