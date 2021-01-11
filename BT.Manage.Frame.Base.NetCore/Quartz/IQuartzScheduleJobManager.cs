using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BT.Manage.Frame.Base.NetCore.Quartz
{
    public interface IQuartzScheduleJobManager
    {

        /// <summary>
        /// 执行作业
        /// </summary>
        /// <typeparam name="TJob">后台作业</typeparam>
        /// <param name="configureJob">作业创建</param>
        /// <param name="configureTrigger">触发器创建</param>
        /// <returns></returns>
        Task ScheduleAsync<TJob>(Action<JobBuilder> configureJob, Action<TriggerBuilder> configureTrigger) where TJob : IJob;
    }
}
