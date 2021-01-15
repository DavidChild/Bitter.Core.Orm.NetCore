using System.Collections.Generic;
using BT.Manage.DataAccess.SqlClient;

namespace BT.Manage.Core
{
    //最终执行的对象
    public class ParseResult
    {
        public ParseResult()
        {
            Parameters = new Dictionary<string, object>();
        }

        public string CommandText { get;  set; }

        public string ParameterizedCommandText { get; internal set; }

        public Dictionary<string, object> Parameters { get; internal set; }

        public SqlQueryParameterCollection SqlQueryParameters
        {
            get { return null; }
        }
    }
}