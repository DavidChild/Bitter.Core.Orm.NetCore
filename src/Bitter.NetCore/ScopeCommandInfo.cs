using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Bitter.Core
{
    public class ScopeCommandInfo
    {
        public void Reset()
        {
            SqlCommand = null;
            Parameters = null;
            _parameterJson = null;
        }

        private class xxprames
        {
            public string key { get; set; }
            public object kvalue { get; set; }
        }
        /// <summary>
        /// 执行的语句
        /// </summary>
        public string SqlCommand { get; set; }
        /// <summary>
        /// 参数
        /// </summary>
        public SqlQueryParameterCollection Parameters { get; set; }
        /// <summary>
        /// 
        /// </summary>
        private string _parameterJson { get; set; }

        /// <summary>
        /// 参数Json
        /// </summary>
        public string ParameterJson
        {
            get
            {

                if (Parameters != null)
                {
                    List<xxprames>  pls=new List<xxprames>();
                  
                        
                    for (int i = 0; i < Parameters.Count; i++)
                    {
                        pls.Add(new xxprames() { key = Parameters[i].ParameterName, kvalue = Parameters[i].Value });
                     
                    }

                    return JsonConvert.SerializeObject(pls);
                }
                else
                {
                    return string.Empty;
                }



            }


        }
    }

   
}
