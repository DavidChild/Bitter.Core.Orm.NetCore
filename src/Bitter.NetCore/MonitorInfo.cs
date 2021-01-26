using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bitter.Core
{
    public class MonitorInfo
    {
        /// <summary>
        /// SQL 操作语句
        /// </summary>
        public string CommandSqlText { get; set; }
        /// <summary>
        /// SQL 参数
        /// </summary>
        public SqlQueryParameterCollection Parameters { get; set; }
        /// <summary>
        /// SQL 参数序列化字符串
        /// </summary>
        public string ParametersJson
        {
            get
            {
                if (this.Parameters != null&& this.Parameters.Count>0)
                {
                    return JsonConvert.SerializeObject(this.Parameters);
                }
                return null;
            }
        }
    }
}
