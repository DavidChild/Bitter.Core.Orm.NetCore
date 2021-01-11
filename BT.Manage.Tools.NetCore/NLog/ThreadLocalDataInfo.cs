using System;
using System.Collections.Generic;
using System.Text;

namespace BT.Manage.Tools
{
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
