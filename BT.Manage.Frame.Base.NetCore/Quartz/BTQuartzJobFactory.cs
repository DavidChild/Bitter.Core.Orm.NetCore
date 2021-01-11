using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Text;

namespace BT.Manage.Frame.Base.NetCore.Quartz
{
    public class BTQuartzJobFactory
    {
        // 定义一个静态变量来保存类的实例
        public static BTQuartzJobFactory uniqueInstance;

        //Quartz 作业工厂
        public StdSchedulerFactory factory { get; private set; }
        //Quartz 调度操作类
        public IScheduler scheduler { get; private set; }
        // 定义一个标识确保线程同步
        private static readonly object locker = new object();

        private BTQuartzJobFactory()
        {
        }
        /// <summary>
        /// 创建实例
        /// </summary>
        /// <returns></returns>
        public static BTQuartzJobFactory GetInstance()
        {
            //保证线程安全
            lock (locker)
            {
                // 如果类的实例不存在则创建，否则直接返回
                if (uniqueInstance == null)
                {
                    uniqueInstance = new BTQuartzJobFactory();
                    uniqueInstance.factory = new StdSchedulerFactory();
                    uniqueInstance.scheduler = uniqueInstance.factory.GetScheduler().GetAwaiter().GetResult();
                    //添加任务监听
                    uniqueInstance.scheduler.ListenerManager.AddJobListener(new BTQuartzJobListener());


                }
            }

            return uniqueInstance;
        }
    }
}
