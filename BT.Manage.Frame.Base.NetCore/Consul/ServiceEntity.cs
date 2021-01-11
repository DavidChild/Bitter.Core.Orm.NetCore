using BT.Manage.Frame.Base.NetCore.Common;
using BT.Manage.Frame.Base.NetCore.ConfigManage;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BT.Manage.Frame.Base.NetCore.Consul
{
    public class ServiceEntity
    {
        public string IP { get; set; }
        public int Port { get; set; }
        private string serviceName { get; set; }
        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName
        {
            get
            {
                return SystemJsonConfigManage.GetInstance().SoaConfigInfo.EnvName + ":" + serviceName;
            }
            set
            {
                serviceName = value;
            }
        }

        /// <summary>
        /// 服务ID
        /// </summary>
        public string ServiceID()
        {
            return "【NETCORE】:" + this.ServiceName + "_" + IP + ":" + Port;
        }
        /// <summary>
        /// 健康检查地址
        /// </summary>
        public string CheckUrl { get; set; } = "ConsulCheckHealth/Healthy";
        /// <summary>
        /// 服务Id
        /// </summary>
        private static string LocalIpV4()
        {
            var ipV4 = string.Empty;
            return ipV4;
        }

        private bool _greyTestServer { get; set; }
        /// <summary>
        /// 是否灰度测试服务
        /// </summary>
        public bool GreyTestServer { get { return _greyTestServer; } set { _greyTestServer = value; } }

        /// <summary>
        /// Tag
        /// </summary>
        public string[] Tags
        {
            get
            {
                var tags = new List<string>();
                //添加 urlprefix-/servicename 格式的 tag 标签，以便 Fabio 识别
                tags.Add($"urlprefix-/{ ServiceName}");
                //添加灰度Tag
                if (GreyTestServer)
                {
                    tags.Add(SysConstants.GreyTestTagStr);
                }
                return tags.ToArray();
            }
        }
    }
}
