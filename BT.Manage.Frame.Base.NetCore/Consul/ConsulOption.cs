using System;
using System.Collections.Generic;
using System.Text;

namespace BT.Manage.Frame.Base.NetCore.Consul
{
    public class ConsulOption
    {
        /// <summary>
        /// 连接
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 数据中心 如未提供 默认为代理数据中心
        /// </summary>
        public string DataCenter { get; set; }

        /// <summary>
        /// ACL Token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 等待时长
        /// </summary>
        public TimeSpan? WaitTime { get; set; }

        /// <summary>
        /// 超时时长
        /// </summary>
        public TimeSpan? TimeOut { get; set; }
    }
}
