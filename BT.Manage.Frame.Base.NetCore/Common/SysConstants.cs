using System;
using System.Collections.Generic;
using System.Text;

namespace BT.Manage.Frame.Base.NetCore.Common
{
    public class SysConstants
    {
        /// <summary>
        /// 灰度测试Tag
        /// </summary>
        public const string GreyTestTagStr = "GreyTestTag";

        /// <summary>
        /// Soa寻址配置字段名称 
        /// 值0 分布式寻址 1配置寻址
        /// </summary>
        public const string ApiCloseSoaProviderFlag = "CloseSoaProviderFlag";

        /// <summary>
        /// 灰度测试状态
        ///  1灰度测试中 0非灰度测试
        /// </summary>
        public const string GreyTestFlagStr = "GreyTestFlag";
    }
}
