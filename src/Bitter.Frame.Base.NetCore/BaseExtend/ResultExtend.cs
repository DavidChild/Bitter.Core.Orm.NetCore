using Bitter.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitter.Base
{
    public static class ResultExtend
    {
        public static Result SetZeroResult(this Result o, Exception ex)
        {
            o.code = 0;
            o.message = string.Format("error:" + ex.Message + "|statckInfo:" + ex.Message);
            LogService.Default.Fatal("error:" + ex.Message + "|statckInfo:" + ex.Message);
            return o;
        }

        /// <summary>
        /// 设置code0
        /// </summary>
        /// <param name="o"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static Result SetZeroResult(this Result o, string errorMessage)
        {
            o.code = 0;
            o.message = errorMessage;
            //LogService.Default.Fatal("error:" + errorMessage);
            return o;
        }

       /// <summary>
       /// 设置成功返回
       /// </summary>
       /// <param name="o"></param>
       /// <param name="obj"></param>
       /// <param name="message"></param>
       /// <returns></returns>
        public static Result SetSucessResult(this Result o,  object obj,  string message = null)
        {
            o.code = 1;
            if (!string.IsNullOrEmpty(message))
            {
                o.message = message;
            }
            o.@object = obj;
            return o;
        }

        /// <summary>
        /// 设置成功返回（带分页）
        /// </summary>
        /// <param name="o"></param>
        /// <param name="obj"></param>
        /// <param name="page"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Result SetSucessResult(this Result o, object obj, page page, string message = null)
        {
            o.code = 1;
            if (!string.IsNullOrEmpty(message))
            {
                o.message = message;
            }
            if (page == null)
            {
                page = new page();
            }
            o.@object = obj;
            o.page = page;
            return o;
        }
    }
}
