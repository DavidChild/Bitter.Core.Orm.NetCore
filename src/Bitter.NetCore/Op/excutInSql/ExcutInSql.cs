using Bitter.Tools.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bitter.Core
{
    public class ExcutInSql:BaseQuery
    {
       
        public  ExcutInSql(string commandText, dynamic dynamicParms, string targetdb = null)
        {
            excutParBag = new ExcutParBag_Excut();
            this.SetTargetDb(targetdb.ToSafeString());
            excutParBag.excutEnum = ExcutEnum.ExcutQuery;
            ((ExcutParBag_Excut)excutParBag).commandText = commandText;
            ((ExcutParBag_Excut)excutParBag).dynamicParma = new List<dynamic>();
            if (dynamicParms != null)
            {
                ((ExcutParBag_Excut)excutParBag).dynamicParma.Add(dynamicParms);
            }
        }
           

        public string CommandText
        {
            get
            {
                return ((ExcutParBag_Excut)excutParBag).commandText;
            }
            
        }
        public ExcutInSql AddParms(dynamic dynamicParms)
        {
            if (dynamicParms != null)
            {
                ((ExcutParBag_Excut)excutParBag).dynamicParma.Add(dynamicParms);
                return this;
            }
            return this;
          
        }
      
        
    }
}
