using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitter.Base
{
    public class ExtendParams
    {
        private int? _timeOut { get; set; }

        private string _version { get; set; }

        /// <summary>
        /// 请求超时时间
        /// </summary>
        public int timeOut
        {
            get
            {
                if (_timeOut.HasValue)
                {
                    return _timeOut.Value;
                }
                else
                {
                    return 7000;
                }

            }
            set
            {
                _timeOut = value;

            }
        }

        /// <summary>
        /// 尝试重试次数：-1:永久 0: 不重试，>0:重试次数
        /// </summary>
        public int retryCount { get; set; } = 0;
        /// <summary>
        /// 重试时间间隔，默认为10000毫秒一次，当前仅单 retryCount=-1 或者 retryCount >0时 有效
        /// </summary>
        public int retrySpanTime { get; set; } = 10000;

        /// <summary>
        /// 版本号
        /// </summary>
        public string version
        {
            get
            {
                if (!string.IsNullOrEmpty(_version))
                {
                    return _version;
                }
                else
                {
                    return "1.0";
                }

            }
            set
            {
                _version = value;

            }
        }
        /// <summary>
        /// 路由重定向ip,如果设定了此值,那么就按此ip进行请求接口，如果没有，那么就寻找配置文件或者服务提供者寻址
        /// </summary>
        public string routeRedirectionUri { get; set; }

        /// <summary>
        /// 开放接口Api校验开关
        /// 默认关闭
        /// </summary>
       // public bool isOpenApiAuth { get; set; } = false;
    }
}
