using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BT.Manage.Frame.Base.NetCore.Quartz
{
    public abstract class JobBase: IJob
    {
        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="context"></param>
       public  abstract Task Execute(IJobExecutionContext context);
    }
}
