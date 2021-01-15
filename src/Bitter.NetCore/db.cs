using System;
using System.Collections.Generic;
using System.Text;

namespace Bitter.Core
{
    public class db
    {


        public static dbscope dbscope(string targetdb = null)
        {
            var d = new dbscope();
            d.targetdb(targetdb);
            return d;

        }

        public static FindQuery<T> FindQuery<T>(string targetdb = null) where T : BaseModel, new()
        {
            var d = new FindQuery<T>(targetdb);
            return d;

        }


        public static Delete<T> Delete<T>(string targetdb = null) where T : BaseModel, new()
        {
            return new Delete<T>(targetdb);
        }
        public static Update<T> Update<T>(string targetdb = null) where T : BaseModel, new()
        {
            var d = new Update<T>(targetdb);
            return d;
        }

        public static FindQuery FindQuery(string commandText, dynamic dynamicParms, string targetdb = null)
        {
            return new FindQuery(commandText, dynamicParms, targetdb);
        }


        public static ExcutInSql Excut(string commandText, dynamic dynamicParms, string targetdb = null)
        {
            return new ExcutInSql(commandText, dynamicParms, targetdb);
        }

        public static PageQuery PageQuery(string selectCommandText, string targetdb = null)
        {
            return new PageQuery(selectCommandText, targetdb);
        }

    }
}
