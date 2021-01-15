using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Web;
using BT.Manage.Tools.Utils;
using Newtonsoft.Json;

namespace BT.Manage.Tools
{
    /********************************************************************************
    ** auth： weiyz
    ** date： 2018/2/6
    ** desc：
    ** Ver.:  V1.0.0
    ** Copyright (C) 2018 备胎 版权所有。
    *********************************************************************************/
    public class ThreadSlot
    {
        //线程安全 线程本地存储
        private static ThreadLocal<ThreadLocalData> _bagLocal = new ThreadLocal<ThreadLocalData>(true);


        /// <summary>
        /// 初始化线程本地数据
        /// </summary>
        private static void InitThreadSlot()
        {
            //lock ("s")
            //{
            //if (_bagLocal.Value == null || string.IsNullOrEmpty(_bagLocal.Value.TraceId))
            //{
            ThreadLocalData data = null;
            //http请求
            if (BtHttpContext.Current != null && BtHttpContext.Current.Request != null && BtHttpContext.Current.Request.Host != null)
            {
                //服务转发过来的请求
                data = string.IsNullOrEmpty(BtHttpContext.Current.Request.Headers["BTProcessInfo"]) ? null : JsonConvert.DeserializeObject<ThreadLocalData>(BtHttpContext.Current.Request.Headers["BTProcessInfo"]);
                if (data != null)
                {
                    data.FromUrl = data.FromUrl; //
                    data.ToUrl = BtHttpContext.RequestUrl;
                    data.TraceSecondId = string.IsNullOrEmpty(ThreadSlot.GetClientID()) ? null : ThreadSlot.GetClientID().Split('|')[0];
                    data.StartTime = data.StartTime == DateTime.MinValue ? DateTime.Now : data.StartTime;
                }
                else
                {
                    //app终端 请求带有requestToken
                    string requestToken = string.IsNullOrEmpty(BtHttpContext.Current.Request.Headers["traceToken"]) ? string.Empty : BtHttpContext.Current.Request.Headers["traceToken"].ToString();
                    //LogService.Default.Debug(HttpContext.Current.Request.Url.AbsoluteUri + "-traceToken-" + (string.IsNullOrEmpty(requestToken) ? "notoken" : requestToken));
                    if (data == null && (!string.IsNullOrEmpty(requestToken)))
                    {
                        data = new ThreadLocalData();
                        data.TraceId = requestToken;//HttpContext.Current.Request.Headers["traceToken"].ToString();
                        data.FromUrl = GetAppOS();
                        data.ToUrl = BtHttpContext.RequestUrl;
                        data.StartTime = DateTime.Now;
                        data.TraceSecondId = string.IsNullOrEmpty(ThreadSlot.GetClientID()) ? null : ThreadSlot.GetClientID().Split('|')[0];
                    }
                }


                if (data != null) CallContext.LogicalSetData("tp", data);
                //ExecutionContext.RestoreFlow();
            }
            else
            {
                data = CallContext.LogicalGetData("tp") == null ? null : CallContext.LogicalGetData("tp") as ThreadLocalData;
                //ThreadPool.QueueUserWorkItem(p => { object s = CallContext.LogicalGetData("tp"); }); 
            }

            _bagLocal.Value = data == null ? new ThreadLocalData() { TraceId = Guid.NewGuid().ToString("N"), StartTime = DateTime.Now, TraceSecondId = string.IsNullOrEmpty(ThreadSlot.GetClientID()) ? null : ThreadSlot.GetClientID().Split('|')[0], FromUrl = GetCurrentWebSiteName() } : data;
            if (CallContext.LogicalGetData("tp") == null) CallContext.LogicalSetData("tp", _bagLocal.Value);
            //LogService.Default.Debug("clientID=" + _bagLocal.Value.TraceSecondId);
            //}
            // }
        }

        /// <summary>
        /// 初始化并保存traceinfo 到逻辑调用上下文
        /// </summary>
        /// <returns></returns>
        public static ThreadLocalData LogicalGetData()
        {
            try
            {

                //if(!_bagLocal.IsValueCreated)
                if (CallContext.LogicalGetData("tp") == null)
                    InitThreadSlot();
            }
            catch (Exception ex)
            {
            }
            return (ThreadLocalData)CallContext.LogicalGetData("tp");//_bagLocal.Value;
        }

        /// <summary>
        /// 日志输出时自定义布局器<traceinfo>获取数据
        /// </summary>
        /// <returns></returns>
        public static string GetThreadTraceInfo()
        {
            string str = null;
            try
            {
                var obj = CallContext.LogicalGetData("tp");
                if (obj != null)
                {
                    str = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                }
            }
            catch (Exception)
            {
                //
                LogService.Default.Debug("获取traceinfo失败");
            }
            return str;
        }

        /// <summary>
        /// 线程处理层级加1
        /// </summary>
        public static void TraceLevelAdd()
        {
            if (_bagLocal.Value != null && _bagLocal.Value.TraceId != null)
            {
                _bagLocal.Value.TraceLayer += ".1";
            }
        }

        /// <summary>
        /// 设置调用结束时间
        /// </summary>
        public static void SetTraceEndTime()
        {
            //if (_bagLocal.Value != null && _bagLocal.Value.TraceId != null)
            //{
            //    _bagLocal.Value.EndTime = DateTime.Now;
            //}
            try
            {
                if (CallContext.LogicalGetData("tp") != null)
                {
                    ThreadLocalData tld = (ThreadLocalData)CallContext.LogicalGetData("tp");
                    tld.EndTime = DateTime.Now;
                    CallContext.LogicalSetData("tp", tld);
                }
            }
            catch (Exception)
            {
                //
            }

        }

        /// <summary>
        /// 清空线程本地数据
        /// </summary>
        public static void LogicalThreadDataClear()
        {
            _bagLocal.Dispose();
            _bagLocal = new ThreadLocal<ThreadLocalData>(true);
            CallContext.LogicalSetData("tp", null);
        }

        /// <summary>
        /// 当前服务id（|后面为终端标识 默认为否）
        /// </summary>
        /// <returns></returns>
        public static string GetClientID()
        {
            string str = string.Empty;
            string clientid = System.Configuration.ConfigurationSettings.AppSettings["ClientID"];
            if (!string.IsNullOrEmpty(clientid))
            {
                if (clientid.IndexOf('|') == -1)
                {
                    clientid += "|0";
                }
                str = clientid;
            }
            return str;
        }

        /// <summary>
        /// 获取请求设备os
        /// </summary>
        /// <returns></returns>
        public static string GetAppOS()
        {
            string str = null;
            try
            {
                //string userAgent = HttpContext.Current.Request.Headers["User-Agent"];
                //if (!string.IsNullOrEmpty(userAgent))
                //{
                //    string[] keywords = { "Android", "iPhone", "iPod", "iPad", "Windows Phone", "MQQBrowser" };
                //    foreach (string item in keywords)
                //    {
                //        if (userAgent.Contains(item))
                //        {
                //            str = item;
                //            break;
                //        }
                //    }
                //}
                //安卓设备号
                string deviceModel = HttpContext.Current.Request.Headers["deviceModel"];
                if (!string.IsNullOrEmpty(deviceModel))
                    str = deviceModel;
            }
            catch (Exception)
            {

                throw;
            }

            return str;
        }

        /// <summary>
        /// 获取当前iis站点名称
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentWebSiteName()
        {
            try
            {
                return System.Web.Hosting.HostingEnvironment.ApplicationHost.GetSiteName();//iis站点
            }
            catch (Exception)
            {
                try
                {
                    return System.AppDomain.CurrentDomain.FriendlyName;//控制台
                }
                catch (Exception)
                {
                    //
                }
            }
            return null;
        }
    }

    public class ThreadLocalData : ICloneable
    {
        /// <summary>
        /// 多线程调用层级
        /// </summary>
        private string pTraceLayer = "1";

        public string TraceLayer { get { return pTraceLayer; } set { this.pTraceLayer = value; } }
        /// <summary>
        /// threadguid
        /// </summary>
        public string TraceId { get; set; }
        /// <summary>
        /// 终端单线程中远程调用id
        /// </summary>
        public string TraceSecondId { get; set; }
        /// <summary>
        /// 请求来源
        /// </summary>
        public string FromUrl { get; set; }
        /// <summary>
        /// 当前url
        /// </summary>
        public string ToUrl { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }


        public object Clone()
        {
            ThreadLocalData cLocalData = new ThreadLocalData();
            cLocalData.TraceLayer = this.TraceLayer + ".1";
            cLocalData.TraceId = this.TraceId;
            cLocalData.FromUrl = this.FromUrl;
            cLocalData.ToUrl = this.ToUrl;
            cLocalData.StartTime = this.StartTime;
            cLocalData.EndTime = this.EndTime;
            return cLocalData;
        }


    }
}
