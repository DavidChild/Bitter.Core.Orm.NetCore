using BT.Manage.Frame.Base.NetCore.Consul;
using BT.Manage.Frame.Base.NetCore.Quartz;
using BT.Manage.Tools;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BT.Manage.Frame.Base.NetCore.ConfigManage
{
    public class LooppullBackgroundJob: JobBase
    {

        /// <summary>
        /// 获取路径
        /// </summary>
        public string ConPath { get; set; }
        /// <summary>
        /// 连接
        /// </summary>
        public string ConHost { get; set; }
        /// <summary>
        /// 数据中心
        /// </summary>
        public string ConDataCenter { get; set; }
        /// <summary>
        /// 超时时长
        /// </summary>
        public double ConTimeOut { get; set; }
        /// <summary>
        /// 等待时长
        /// </summary>
        public double ConWaitTime { get; set; }

        public override async Task Execute(IJobExecutionContext context)
        {
            try
            {
                
                ConsulNetClient consulClient = new ConsulNetClient(new ConsulOption()
                {
                    Host = ConHost,
                    DataCenter = ConDataCenter,
                    WaitTime = TimeSpan.FromSeconds(ConWaitTime),
                    TimeOut = TimeSpan.FromSeconds(ConTimeOut)
                });
               // LogService.Default.Debug("xxxx-xxx-1");
                //获取配置信息
                var conList = await consulClient.KVListAsync<ConfigValue>(ConPath);
               // LogService.Default.Debug("xxxx-xxx-2");

                var valueList = conList.Where(o => o.IsEnable).ToList();
              //  LogService.Default.Debug("xxxx-xxx-3");
           
                //同步配置信息
                JsonConfigMange.GetInstance().WiteConfig(valueList);

                //释放Concul连接
                consulClient.Dispose();
            }
            catch (Exception ex)
            {
                LogService.Default.Fatal(ex, "执行后台任务拉取配置信息出错" + ex.Message);
            }

        }
    }
}
