using System;
using Bitter.Core;
using Bitter.Tools.Utils;
namespace Bitter.test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            MonitorInfo monitor_end = db.FindQuery<TStudentInfo>().Where(p => p.FName.EndsWith("h")).ToMonitorInfo();

            MonitorInfo monitor_start = db.FindQuery<TStudentInfo>().Where(p => p.FName.StartsWith("j")).ToMonitorInfo();

            MonitorInfo monitor_contains = db.FindQuery<TStudentInfo>().Where(p => p.FName.Contains("h")).ToMonitorInfo();

            MonitorInfo monitor_in = db.FindQuery<TStudentInfo>().Where(p => p.FAage.In(new int?[] { 1,2,3,4})).ToMonitorInfo();

            MonitorInfo monitor_notin = db.FindQuery<TStudentInfo>().Where(p => p.FAage.NotIn(new int?[] { 1, 2, 3, 4 })).ToMonitorInfo();

            MonitorInfo monitor_isnull_str = db.FindQuery<TStudentInfo>().Where(p => p.FName.IsNull("")).ToMonitorInfo();

            MonitorInfo monitor_isnull_int = db.FindQuery<TStudentInfo>().Where(p => p.FAage.IsNull(-1)).ToMonitorInfo();

            MonitorInfo monitor_like = db.FindQuery<TStudentInfo>().Where(p => p.FName.Like("h")).ToMonitorInfo();

            MonitorInfo monitor_Notlike = db.FindQuery<TStudentInfo>().Where(p => p.FName.NotLike("h")).ToMonitorInfo();

            MonitorInfo monitor_ThanEquel = db.FindQuery<TStudentInfo>().Where(p => p.FAage>=10).ToMonitorInfo();

            MonitorInfo monitor_LessThanEquel = db.FindQuery<TStudentInfo>().Where(p => p.FAage <=10).ToMonitorInfo();

            MonitorInfo monitor_And = db.FindQuery<TStudentInfo>().Where(p => p.FAage <= 10 && p.FAage<=20).ToMonitorInfo();

            MonitorInfo monitor_And_And = db.FindQuery<TStudentInfo>().Where(p =>p.FName.Like("h")&&(p.FAage <= 10 && p.FAage <= 20)).ToMonitorInfo();

            MonitorInfo monitor_And_And_And = db.FindQuery<TStudentInfo>().Where(p => p.FName.Like("h") && (p.FAage <= 10 && p.FAage <= 20)).Where(p=>p.FAddTime>"2020-10-11".ToSafeDateTime()).ToMonitorInfo();

            var k= Console.ReadKey();
        }
    }
}
