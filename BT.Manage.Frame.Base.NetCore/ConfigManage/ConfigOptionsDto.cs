using System;
using System.Collections.Generic;
using System.Text;

namespace BT.Manage.Frame.Base.NetCore.ConfigManage
{
    public class ConfigOptionsDto
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public string APPKey { get; set; }

        public string ENVKey { get; set; }
        /// <summary>
        /// 是否开启循环拉取
        /// </summary>
        public bool IsEnableLooppull { get; set; }
        /// <summary>
        /// Consul地址
        /// </summary>
        public string ConsulHost { get; set; }
        /// <summary>
        /// 对应的数据中心
        /// </summary>
        public string ConsulDataCenter { get; set; }
        /// <summary>
        /// 等待时长
        /// </summary>
        public double ConsulWaitTime { get; set; }
        /// <summary>
        /// 超时时间
        /// </summary>
        public double ConsulTimeOut { get; set; }   
    }
}
