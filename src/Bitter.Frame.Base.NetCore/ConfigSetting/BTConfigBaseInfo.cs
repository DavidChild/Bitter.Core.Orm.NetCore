using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Bitter.Base.NetCore.ConfigSetting
{
    public class BTConfigBaseInfo
    {
        /// <summary>
        /// 数据库相关配置信息
        /// </summary>
        public List<SelfConnectionStringEntity> connectionString { get; set; }
        /// <summary>
        /// KV配置信息
        /// </summary>
        public Dictionary<string,object> AppSettings { get; set; }
    }
}
