using BT.Manage.Tools;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BT.Manage.Frame.Base.NetCore.Quartz
{
    /// <summary>
    /// 后台作业管理类
    /// </summary>
    public class QuartzScheduleJobManager: IQuartzScheduleJobManager
    {
        public BTQuartzJobFactory btfactory = BTQuartzJobFactory.GetInstance();

        private static object locker = new object();
        /// <summary>
        /// 后台作业调度实现
        /// </summary>
        /// <typeparam name="TJob"></typeparam>
        /// <param name="configureJob"></param>
        /// <param name="configureTrigger"></param>
        /// <returns></returns>
        public  Task ScheduleAsync<TJob>(Action<JobBuilder> configureJob, Action<TriggerBuilder> configureTrigger) where TJob : IJob
        {
            lock (locker)
            {
                //创建任务
                var jobToBuild = JobBuilder.Create<TJob>();
                configureJob(jobToBuild);
                var job = jobToBuild.Build();
                //创建触发器
                var triggerToBuild = TriggerBuilder.Create();
                configureTrigger(triggerToBuild);
                var trigger = triggerToBuild.Build();
                //判断是否已存在
                var curentjob = btfactory.scheduler.GetJobDetail(job.Key).GetAwaiter().GetResult();
                var curenttrigger = btfactory.scheduler.GetTrigger(trigger.Key).GetAwaiter().GetResult();
                if (curentjob != null)
                {
                    var newtrigger = triggerToBuild.ForJob(curentjob).Build();
                    btfactory.scheduler.ScheduleJob(newtrigger);
                }
                else
                    btfactory.scheduler.ScheduleJob(job, trigger);
                ////开始运行
                btfactory.scheduler.Start();
                LogService.Default.Info("启动后台作业管理....");
                //try
                //{
                //    btfactory.scheduler.Shutdown(true);
                //    LogService.Default.Info("后台作业管理关闭....");
                //}
                //catch (Exception ex)
                //{
                //    LogService.Default.Error(ex,ex.Message);
                //}
                return Task.FromResult(0);
            }


        }
    }
}
