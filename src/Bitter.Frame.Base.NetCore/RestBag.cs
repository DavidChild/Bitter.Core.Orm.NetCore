using Bitter.Base;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using Bitter.Tools.Utils;
namespace Bitter.Base
{
    [Serializable]
    public class RestBag
    {
        public RestRequest req { get; set; }

        public ExtendParams extendParms { get; set; }

        public string uri { get; set; }

        /// <summary>
        /// 没有值的话，代表不尝试通知,如果有值的话代表产生通知
        /// </summary>
        public int retryCount
        {
            get
            {
                if (extendParms != null)
                {
                    return extendParms.retryCount;
                }
                else
                {
                    return 0;
                }

            }
        }

        public int retryspanTime
        {
            get
            {
                if (extendParms != null)
                {
                    return extendParms.retrySpanTime;
                }
                else
                {
                    return 10000;
                }

            }
        }
        //当前已重试次数
        public int currentRetryCount { get; set; } = 0;

        /// <summary>
        /// 路由重定向ip,如果设定了此值,那么就按此ip进行请求接口，如果没有，那么就寻找配置文件或者服务提供者寻址
        /// </summary>
        public string RouteRedirectionUri {
            get
            {
                if (extendParms != null)
                {
                    return extendParms.routeRedirectionUri;
                }
                else
                {
                    return "";
                }
            }
        }
     


        

    }
}
