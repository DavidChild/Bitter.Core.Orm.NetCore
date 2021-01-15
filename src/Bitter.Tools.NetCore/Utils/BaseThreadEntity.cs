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
    public class BaseThreadEntity
    {
        //[Display(Name = @"随路主键ID")]
        public string TraceId { get; set; }

        //[Display(Name = @"顺序ID")]
        public double TorderId { get; set; }

        //[Display(Name = @"初始请求路径")]
        public string InitialUrl { get; set; }

        public DateTime AddTime { get; set; }
    }
}
