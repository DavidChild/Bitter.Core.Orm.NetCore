using Hangfire;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BT.Manage.Frame.Base.NetCore.Hangfire
{

    /// <summary>
    /// hangfire任务执行管理者
    /// </summary>
    public class BTHangfireBackgroundJobManager
    {
        /// <summary>
        /// 执行后台任务（返回任务Id）
        /// </summary>
        /// <typeparam name="TJob">具体任务类</typeparam>
        /// <typeparam name="TArgs">参数</typeparam>
        /// <param name="args"></param>
        /// <param name="delay">延时时间（默认不填，表示任务立即执行）</param>
        /// <returns></returns>
        public static Task<string> EnqueueBackIDAsync<TJob, TArgs>(TArgs args, TimeSpan? delay = default(TimeSpan?)) where TJob : IBackgroundJob<TArgs>
        {
            string jobid = "";
            if (!delay.HasValue)
                jobid = BackgroundJob.Enqueue<TJob>(job => job.Execute(args));
            else
                jobid = BackgroundJob.Schedule<TJob>(job => job.Execute(args), delay.Value);
            return Task.FromResult(jobid);
        }
        /// <summary>
        /// 执行后台任务（返回任务Id）
        /// </summary>
        /// <typeparam name="TJob">具体任务类</typeparam>
        /// <typeparam name="TArgs">参数</typeparam>
        /// <param name="args"></param>
        /// <param name="delay">延时时间</param>
        /// <returns></returns>
        public static string EnqueueBackID<TJob, TArgs>(TArgs args, TimeSpan? delay = default(TimeSpan?)) where TJob : IBackgroundJob<TArgs>
        {
            string jobid = "";
            if (!delay.HasValue)
                jobid = BackgroundJob.Enqueue<TJob>(job => job.Execute(args));
            else
                jobid =BackgroundJob.Schedule<TJob>(job => job.Execute(args), delay.Value);
            return jobid;
        }
        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="JobId">任务id列表</param>
        /// <returns></returns>
        public static Task<bool> DeleteEnqueueAsync(List<string> JobId)
        {
            foreach (var item in JobId)
            {
                BackgroundJob.Delete(item);
            }
            return Task.FromResult(true);
        }
        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="JobId">任务id列表</param>
        /// <returns></returns>
        public static bool DeleteEnqueue(List<string> JobId)
        {
            foreach (var item in JobId)
            {
                BackgroundJob.Delete(item);
            }
            return true;
        }
        /// <summary>
        /// 执行重复作业
        /// </summary>
        /// <typeparam name="TJob"></typeparam>
        /// <typeparam name="TArgs"></typeparam>
        /// <param name="args">参数</param>
        /// <param name="cron">定时表示式</param>
        /// <param name="recurrId">自定义的作业id(需唯一)</param>
        /// <returns></returns>
        public static Task EnqueueRecurAsync<TJob, TArgs>(TArgs args, string cron, string recurrId = null) where TJob : IBackgroundJob<TArgs>
        {
            if (!string.IsNullOrEmpty(recurrId))
            {
                RecurringJob.RemoveIfExists(recurrId);
                RecurringJob.AddOrUpdate<TJob>(job => job.Execute(args), cron, TimeZoneInfo.Local);
            }
            else
            {
                RecurringJob.AddOrUpdate<TJob>(job => job.Execute(args), cron, TimeZoneInfo.Local);
            }
            return Task.FromResult(0);
        }
        /// <summary>
        /// 执行重复作业
        /// </summary>
        /// <typeparam name="TJob"></typeparam>
        /// <typeparam name="TArgs"></typeparam>
        /// <param name="args">参数</param>
        /// <param name="cron">定时表示式</param>
        /// <param name="recurrId">自定义的作业id(需唯一)</param>
        /// <returns></returns>
        public static void EnqueueRecur<TJob, TArgs>(TArgs args, string cron, string recurrId = null) where TJob : IBackgroundJob<TArgs>
        {
            if (!string.IsNullOrEmpty(recurrId))
            {
                RecurringJob.RemoveIfExists(recurrId);
                RecurringJob.AddOrUpdate<TJob>(job => job.Execute(args), cron, TimeZoneInfo.Local);
            }
            else
            {
                RecurringJob.AddOrUpdate<TJob>(job => job.Execute(args), cron, TimeZoneInfo.Local);
            }

        }
        /// <summary>
        /// 手动触发重复执行的作业
        /// </summary>
        /// <param name="recurrId">作业id</param>
        /// <returns></returns>
        public static Task TriggerRecurringJobAsync(string recurrId)
        {
            RecurringJob.Trigger(recurrId);
            return Task.FromResult(0);
        }

        /// <summary>
        /// 手动触发重复执行的作业
        /// </summary>
        /// <param name="recurrId">作业id</param>
        /// <returns></returns>
        public static void TriggerRecurringJob(string recurrId)
        {
            RecurringJob.Trigger(recurrId);
        }
    }
}
