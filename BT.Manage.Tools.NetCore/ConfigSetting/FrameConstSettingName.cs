using System;
using System.Collections.Generic;
using System.Text;

namespace BT.Manage.Tools
{
  public  static class FrameConstSettingName
    {
        /// <summary>
        /// 数据库配置节点(目录）
        /// </summary>
        public static readonly string Bt_DataBase_Setting = "connectionStrings";
        /// <summary>
        /// 系统Rpc框架配置节点（目录）
        /// </summary>
        private static readonly string Bt_RpcSetting = "RpcSetting";
        /// <summary>
        /// 系统Rpc框架Client配置节点（节点）
        /// </summary>
        public static readonly string Bt_RpcSetting_Client= Bt_RpcSetting + ":" + "Client";
        /// <summary>
        /// 系统Rpc框架Server配置节点（节点）
        /// </summary>
        public static readonly string Bt_RpcSetting_Server = Bt_RpcSetting + ":" + "Server";
        /// <summary>
        /// 系统Rpc框架中使用Zk分布式协调环境配置节点（节点）
        /// </summary>
        public static readonly string Bt_RpcSetting_Zookeeper = Bt_RpcSetting + ":" + "Zookeeper";

    }
}
