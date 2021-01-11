using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using NLog.LayoutRenderers;

namespace BT.Manage.Tools
{
    /********************************************************************************
    ** auth： weiyz
    ** date： 2018/2/6
    ** desc：
    ** Ver.:  V1.0.0
    ** Copyright (C) 2018 备胎 版权所有。
    *********************************************************************************/

    [LayoutRenderer("bttraceid")]
    public sealed class TraceIDLayoutRenderer : LayoutRenderer
    {

        public string BTTraceID { get { return ThreadSlot.LogicalGetData().TraceId; } }

        protected override void Append(StringBuilder builder, LogEventInfo ev)
        {

            builder.Append(BTTraceID);

        }
    }

    [LayoutRenderer("bttracesecondid")]
    public sealed class TraceSecondIDLayoutRenderer : LayoutRenderer
    {

        public string BTTraceSecondID { get { return ThreadSlot.LogicalGetData().TraceSecondId; } }

        protected override void Append(StringBuilder builder, LogEventInfo ev)
        {

            builder.Append(BTTraceSecondID);

        }
    }

    [LayoutRenderer("bttracelevel")]
    public sealed class TraceLevelLayoutRenderer : LayoutRenderer
    {

        public string BTTraceLevel { get { return ThreadSlot.LogicalGetData().TraceLayer; } }

        protected override void Append(StringBuilder builder, LogEventInfo ev)
        {
            // 最终添加给指定的StringBuilder 

            builder.Append(BTTraceLevel);

        }
    }

    [LayoutRenderer("bttracefrom")]
    public sealed class TraceFromLayoutRenderer : LayoutRenderer
    {

        public string BTTraceFrom { get { return ThreadSlot.LogicalGetData().FromUrl; } }

        protected override void Append(StringBuilder builder, LogEventInfo ev)
        {
            // 最终添加给指定的StringBuilder 

            builder.Append(BTTraceFrom);

        }
    }

    [LayoutRenderer("bttraceto")]
    public sealed class TraceToLayoutRenderer : LayoutRenderer
    {

        public string BTTraceTo { get { return ThreadSlot.LogicalGetData().ToUrl; } }

        protected override void Append(StringBuilder builder, LogEventInfo ev)
        {
            // 最终添加给指定的StringBuilder 

            builder.Append(BTTraceTo);

        }
    }

    [LayoutRenderer("bttraceinfo")]
    public sealed class TraceInfoLayoutRenderer : LayoutRenderer
    {

        public string BTTraceInfo { get { return ThreadSlot.GetThreadTraceInfo(); } }

        protected override void Append(StringBuilder builder, LogEventInfo ev)
        {
            // 最终添加给指定的StringBuilder 

            builder.Append(BTTraceInfo);

        }
    }
}
