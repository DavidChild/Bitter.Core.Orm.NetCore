using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitter.Tools.Utils
{
    /// <summary>
    /// 创建人： 徐燕 创建时间：2017年11月10日
    /// </summary>
    public static class BaseThread
    {

        [ThreadStatic]
        public static BaseThreadEntity threadInfo = new BaseThreadEntity();

        public static BaseThreadEntity GetTraceInfo()
        {
            return threadInfo;
        }

        public static void SetTraceInfo(BaseThreadEntity tInfo)
        {
            threadInfo = tInfo;
        }
    }
}
