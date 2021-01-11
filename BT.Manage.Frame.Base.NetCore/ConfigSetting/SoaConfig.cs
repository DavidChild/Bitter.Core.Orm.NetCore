using System;
using System.Collections.Generic;
using System.Text;

namespace BT.Manage.Frame.Base.NetCore.ConfigSetting
{
    public class SoaConfig
    {
        /// <summary>
        /// 会话超时时间
        /// </summary>

        public int SessionTimeOut { get; set; }

        /// <summary>
        /// 连接超时时间
        /// </summary>
        public int ConnectTimeOut { get; set; }

        /// <summary>
        /// 当前服务地址
        /// </summary>
        public string LocalServer { get; set; }

        /// <summary>
        /// 服务读取地址
        /// </summary>
        public string ReadServer { get; set; }
        /// <summary>
        /// 是否记录日志,该设置仅用于排查运行时出现的问题,如工作正常,请关闭该项
        /// </summary>
        public bool RecordeLog { get; set; }
        /// <summary>
        /// 是否启用缓存机制
        /// </summary>
        public bool EnabledCache { get; set; }

        /// <summary>
        /// 排除提供者
        /// </summary>
        public string ExcludeServers { get; set; }
        /// <summary>
        /// 调试用的Debug
        /// </summary>
        public string DebugServer { get; set; }
        /// <summary>
        /// 可写的连接地址
        /// </summary>
        public string WriteServer { get; set; }

        /// <summary>
        /// 关闭SOA   寻址
        /// </summary>
        public bool ClosedSoaProviders { get; set; }
        /// <summary>
        /// 是否开启服务注册
        /// </summary>
        public bool IsRegister { get; set; }

        /// <summary>
        /// 数据中心名称：默认是dc1
        /// </summary>
        public string DataCenterName { get; set; }

        /// <summary>
        /// 环境名称
        /// </summary>
        public string EnvName { get; set; }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get; set; }


        /// <summary>
        ///  增加了是否部署在docker中(是否发布到docker环境中)
        /// </summary>
        public bool IsDeployInDocker { get; set; }


        /// <summary>
        ///  如果 此服务为docker部署,请设置宿主机存放的IP目录
        /// </summary>
        public string DockerHostIPFile { get; set; }

        /// <summary>
        ///  定义服务端口
        /// </summary>
        public string ServicePort { get; set; }


        /// <summary>
        /// 是否灰度测试服务
        /// </summary>
        public bool IsGreyTestServer { get; set; }

        ///// <summary>
        ///// 灰度测试状态 1灰度测试中 0非灰度测试
        ///// </summary>
        //public int? GreyTestFlag { get; set; }
    }
}
