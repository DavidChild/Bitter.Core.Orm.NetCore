using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using BT.Manage.Tools.NetCore;
using NLog;
using NLog.Config;
using NLog.Targets;
using BT.Manage.Tools.Utils;
using NLog.LayoutRenderers;

namespace BT.Manage.Tools
{
    public class BTLoggingConfiguration
    {
        private LoggingConfiguration config;


        private static string _serviceName { get; set; }

        /// <summary>
        /// 注册日志服务名称
        /// </summary>
        /// <param name="sname"></param>
        public static void RegiestNLogServiceName(string sname)
        {
            _serviceName = sname;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal static string GetServiceName()
        {
            return _serviceName;
        }

        /// <summary>
        /// Nlog配置
        /// </summary>
        /// <param name="sitename">站点名称（唯一不重复）</param>
        /// <param name="logBaseDir">日志输出根目录(默认D)</param>
        public BTLoggingConfiguration(string sitename,string logBaseDir="D")
        {

            try
            {
               
                config = new LoggingConfiguration();
                var configpath = ConfManage.GetInstance().AppSettings["LogSettingFile"].ToSafeString("");
                //定义变量
                config.Variables.Add("sitename", new NLog.Layouts.SimpleLayout(sitename));
             
                //定义变量
                config.Variables.Add("servicename", new NLog.Layouts.SimpleLayout(sitename));
              
                
                if ((!string.IsNullOrEmpty(configpath)) && File.Exists(configpath))
                {
                    config.LogFactory.LoadConfiguration(configpath);
                    return;
                }

              

                logBaseDir = string.IsNullOrEmpty(logBaseDir) ? "D" : logBaseDir;

              

                //var extassembly = Assembly.LoadFrom("BT.Manage.Tools.NetCore.DLL");
                //ConfigurationItemFactory.Default.RegisterItemsFromAssembly(extassembly);


                // Step 2. Create targets and add them to the configuration 

                //调试日志输出
                var debugTarget = new FileTarget();
                debugTarget.Layout = @"${longdate} [${level}] ${BTTraceInfo}： ${message}";
                if (logBaseDir.Contains("/") || logBaseDir.Contains("\\"))  // 兼容linux自定义
                {
                    debugTarget.FileName = logBaseDir+ "/跟踪[Debug]/${shortdate}/${date:format=yyyy-MM-dd HH}.log";
                }
                else
                {
                    debugTarget.FileName = logBaseDir + ":/Nlog/" + sitename + "/跟踪[Debug]/${shortdate}/${date:format=yyyy-MM-dd HH}.log";
                }
               
                config.AddTarget("Debug", debugTarget);


                //调用异常日志输出
                var traceTarget = new FileTarget();
                traceTarget.Layout = @"${longdate} [${level}] ${BTTraceInfo}： ${message}";
                if (logBaseDir.Contains("/") || logBaseDir.Contains("\\"))  // 兼容linux自定义
                {
                    traceTarget.FileName = logBaseDir+"/跟踪[调用]/${shortdate}/${date:format=yyyy-MM-dd HH}.log";
                }
                else
                {
                    traceTarget.FileName = logBaseDir + ":/Nlog/" + sitename + "/跟踪[调用]/${shortdate}/${date:format=yyyy-MM-dd HH}.log";
                }
                
                   
               
                traceTarget.ArchiveAboveSize = 52428800;
                traceTarget.ArchiveNumbering = ArchiveNumberingMode.Sequence;//"Sequence"
                config.AddTarget("Trace", traceTarget);

                //异常语句输出
                var errTarget = new FileTarget();
               
                traceTarget.Layout = @"${longdate} [${level}] ${BTTraceInfo}： ${message}";
                if (logBaseDir.Contains("/") || logBaseDir.Contains("\\"))  // 兼容linux自定义
                {
                    errTarget.FileName = logBaseDir + "/跟踪[异常]/${shortdate}/${date:format=yyyy-MM-dd HH}.log";
                }
                else
                {
                    errTarget.FileName = logBaseDir + ":/Nlog/" + sitename + "/跟踪[异常]/${shortdate}/${date:format=yyyy-MM-dd HH}.log";
                }
              
                config.AddTarget("err", errTarget);

                //sql语句输出
                var infoTarget = new FileTarget();
                infoTarget.Layout = @"${longdate} [${level}] ${BTTraceInfo}： ${message}";
                if (logBaseDir.Contains("/") || logBaseDir.Contains("\\"))  // 兼容linux自定义
                {
                    infoTarget.FileName = logBaseDir +"/跟踪[SQL]/${shortdate}/${date:format=yyyy-MM-dd HH}.log";
                }
                else
                {
                    infoTarget.FileName = logBaseDir + ":/Nlog/" + sitename + "/跟踪[SQL]/${shortdate}/${date:format=yyyy-MM-dd HH}.log";
                }
                    
                config.AddTarget("Info", infoTarget);


                // Step 4. Define rules
                //定义日志输出规则
                var rule1 = new LoggingRule("*", LogLevel.Debug,LogLevel.Debug, debugTarget);
                config.LoggingRules.Add(rule1);

                var rule2 = new LoggingRule("*", LogLevel.Trace, LogLevel.Trace,traceTarget);
                config.LoggingRules.Add(rule2);

                //输出sql
                var rule3 = new LoggingRule("*", LogLevel.Info,LogLevel.Info, infoTarget);
                config.LoggingRules.Add(rule3);

                var rule4 = new LoggingRule("*", LogLevel.Error, LogLevel.Fatal, errTarget);
                config.LoggingRules.Add(rule4);
            }
            catch (Exception ex)
            {
                //
            }
           
           

            // Step 5. Activate the configuration
            //LogManager.Configuration = config;
        }

        public LoggingConfiguration Config
        {
            get
            {
                return this.config;
            }
        }
    }
}
