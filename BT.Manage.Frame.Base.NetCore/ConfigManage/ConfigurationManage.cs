using BT.Manage.Frame.Base.NetCore.Quartz;
using BT.Manage.Tools;
using BT.Manage.Tools.Utils;
using EasyNetQ;
using EasyNetQ.Topology;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BT.Manage.Frame.Base.NetCore.ConfigManage
{
    public   class ConfigurationManage
    {
        private static ConfigOptionsDto _configoptionDto;

        private volatile static IBus bus = null;

        private static readonly object lockHelper = new object();

        private static JsonConfigMange _josnConfigMan = JsonConfigMange.GetInstance();

        /// <summary>
        /// 启用数据中心
        /// </summary>
        public static void UserConfigDataCenter(IBus _bus, string serverName)
        {
            try
            {

                bus = _bus;
                _configoptionDto = new ConfigOptionsDto
                {
                    APPKey = serverName,
                    IsEnableLooppull = SystemJsonConfigManage.GetInstance().AppSettings["IsEnableLooppull"].ToSafeBool(true),
                    ConsulDataCenter = SystemJsonConfigManage.GetInstance().SoaConfigInfo.DataCenterName,
                    ConsulHost = "http://" + SystemJsonConfigManage.GetInstance().SoaConfigInfo.WriteServer,
                    ENVKey = SystemJsonConfigManage.GetInstance().SoaConfigInfo.EnvName,
                    ConsulTimeOut = SystemJsonConfigManage.GetInstance().SoaConfigInfo.ConnectTimeOut,
                    ConsulWaitTime = 30
                };
                RabbitMQRecive();
                Looppull();
            }
            catch (Exception ex)
            {
                LogService.Default.Fatal(ex, "启用配置中心出错" + ex.Message);
            }

        }
        /// <summary>
        /// 配置接收RabbitMQ推送信息
        /// </summary>
        /// <returns></returns>
        private static void RabbitMQRecive()
        {
            var envname = SystemJsonConfigManage.GetInstance().SoaConfigInfo.EnvName; 
            var ex = bus.Advanced.ExchangeDeclare("bt_configDataCenter", ExchangeType.Direct);
            var qu = bus.Advanced.QueueDeclare();
            bus.Advanced.Bind(ex, qu, envname+"_"+ _configoptionDto.APPKey);
            bus.Advanced.Consume(qu, (body, properties, info) => Task.Factory.StartNew(() =>
            {
                try
                {
                    lock (lockHelper)
                    {
                       
                        var message = Encoding.UTF8.GetString(body);
                        //处理消息
                        var value = JsonConvert.DeserializeObject<ConfigValue>(message);
                        var valuelist = new List<ConfigValue>();
                        valuelist.Add(value);
                        //同步配置
                        _josnConfigMan.WiteConfig(valuelist);
                    }

                }
                catch (Exception e)
                {
                    LogService.Default.Fatal(e, "接收配置中心数据出错:" + e.Message);
                    throw e;
                    
                }


            }));
        }
        /// <summary>
        /// 配置循环拉取
        /// </summary>
        /// <returns></returns>
        private static void Looppull()
        {
            QuartzScheduleJobManager JobMange = new QuartzScheduleJobManager();
            if (_configoptionDto.IsEnableLooppull)
            {
                JobMange.ScheduleAsync<LooppullBackgroundJob>(job =>
                {
                    job.WithIdentity("LooppullConfig", "LooppullJob")
                        .UsingJobData("ConPath", "App/EnvConfig/" + _configoptionDto.APPKey + "/" + _configoptionDto.ENVKey + "/")
                       .UsingJobData("ConHost", _configoptionDto.ConsulHost)
                       .UsingJobData("ConDataCenter", _configoptionDto.ConsulDataCenter)
                       .UsingJobData("ConTimeOut", _configoptionDto.ConsulTimeOut)
                       .UsingJobData("ConWaitTime", _configoptionDto.ConsulWaitTime);

                }, trigger =>
                {
                    trigger.WithIdentity("LooppullTri" + Guid.NewGuid().ToString(), "LooppullTrigger")
                    .StartNow()
                    .WithSimpleSchedule(x => x.WithIntervalInMinutes(5).RepeatForever());
                });
            }
            else
            {
                JobMange.ScheduleAsync<LooppullBackgroundJob>(job =>
                {
                    job.WithIdentity("LooppullConfig", "LooppullJob")
                       .UsingJobData("ConPath", "App/EnvConfig/" + _configoptionDto.APPKey + "/" + _configoptionDto.ENVKey + "/")
                       .UsingJobData("ConHost", _configoptionDto.ConsulHost)
                       .UsingJobData("ConDataCenter", _configoptionDto.ConsulDataCenter)
                       .UsingJobData("ConTimeOut", _configoptionDto.ConsulTimeOut)
                       .UsingJobData("ConWaitTime", _configoptionDto.ConsulWaitTime);

                }, trigger =>
                {
                    trigger.WithIdentity("LooppullTri" + Guid.NewGuid().ToString(), "LooppullTrigger")
                    .StartNow();
                });
            }

        }
    }
}
