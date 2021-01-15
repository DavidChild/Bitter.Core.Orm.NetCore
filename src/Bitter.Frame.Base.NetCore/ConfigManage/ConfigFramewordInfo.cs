using System;
using System.Collections.Generic;
using System.Text;

namespace Bitter.Base.NetCore.ConfigManage
{
    public class ConfigFramewordInfo
    {
        public string route
        {
            get;set;
        }
        public bool isopen
        {
            get;set;
        }
        /// <summary>
        /// 超时时间
        /// </summary>
      
        public int timeout
        {
            get;set;
        }
        /// <summary>
        /// 转发目标路由
        /// 默认为路由名称
        /// </summary>
        
        public string forwardroute
        {
            get;set;
        }
    }
}
