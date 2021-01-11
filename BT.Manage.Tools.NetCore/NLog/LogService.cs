using NLog;
using NLog.LayoutRenderers;
using System;

namespace BT.Manage.Tools
{
    /********************************************************************************
    ** auth： Jason
    ** date： 2017/2/28 17:22:58
    ** desc：
    ** Ver.:  V1.0.0
    ** Copyright (C) 2016 备胎 版权所有。
    *********************************************************************************/

    public class LogService
    {



        private NLog.Logger logger;
         
        static LogService()
        {
            //注册Nlog布局器(输出参数)
            //LayoutRenderer.Register("bttraceid", typeof(TraceIDLayoutRenderer));
            //LayoutRenderer.Register("bttracelevel", typeof(TraceLevelLayoutRenderer));
            //LayoutRenderer.Register("bttracefrom", typeof(TraceFromLayoutRenderer));
            //LayoutRenderer.Register("bttraceto", typeof(TraceToLayoutRenderer));
            //LayoutRenderer.Register("bttracesecondid", typeof(TraceSecondIDLayoutRenderer));
            //LayoutRenderer.Register("bttraceinfo", typeof(TraceInfoLayoutRenderer));
           // LogManager.Configuration = new BTLoggingConfiguration("qwert").Config;
            Default =   NLog.LogManager.GetCurrentClassLogger(); 
            
        }

        public LogService(string name)
            : this(NLog.LogManager.GetLogger(name))
        {
        }

        private LogService(NLog.Logger logger)
        {
            this.logger = logger;
        }

        public static Logger Default { get; private set; }

        public void Debug(string msg, params object[] args)
        {
            logger.Debug(msg, args);
        }

        public void Debug(string msg, Exception err)
        {
            logger.Debug(msg, err);
        }

        public void Error(string msg, params object[] args)
        {
            logger.Error(msg, args);
        }

        public void Error(string msg, Exception err)
        {
            logger.Error(msg, err);
        }

        /// <summary>
        /// 指定参数记录诊断信息
        /// </summary>
        /// <param name="msg">string</param>
        /// <param name="args">object[]</param>
        public void Fatal(string msg, params object[] args)
        {
            logger.Fatal(msg, args);
        }

        /// <summary>
        /// 程序出错了,记录Exception信息
        /// </summary>
        /// <param name="err">Exception</param>
        public void Fatal(string msg, Exception err)
        {
            logger.Fatal(msg, err);
        }

        /// <summary>
        /// 程序出错了,记录Exception信息
        /// </summary>
        /// <param name="err">Exception</param>
        public void Fatal(Exception err)
        {
            logger.Fatal(err);
        }

        public void Info(string msg, params object[] args)
        {
            logger.Info(msg, args);
        }

        public void Info(string msg, Exception err)
        {
            logger.Info(msg, err);
        }

        /// <summary>
        /// 用于记录框架执行SQL
        /// </summary>
        /// <param name="sql"></param>
        public void LogSql(string sql)
        {
            logger.Info("执行sql:" + sql);
        }

        public void Trace(string msg, params object[] args)
        {
            logger.Trace(msg, args);
        }

        public void Trace(string msg, Exception err)
        {
            logger.Trace(msg, err);
        }

        public void Trace(string msg)
        {
            logger.Trace(msg);
        }
    }
}