using Bitter.Base;
using System;
using System.Text;
using Bitter.Tools.Helper;
using Bitter.Tools;

namespace Bitter.Base
{
    /********************************************************************************
    ** auth： davidchild
    ** date： 2017/3/24 15:06:22
    ** desc： 异常操作扩展
    ** Ver.:  V1.0.0
    ** Copyright (C) 2016 Bitter 版权所有。
    *********************************************************************************/

    public static class ExceptionExtensions
    {


        /// <summary>
        /// 格式化异常消息
        /// </summary>
        /// <param name="e">异常对象</param>
        /// <param name="isHideStackTrace">是否隐藏异常规模信息</param>
        /// <returns>格式化后的异常信息字符串</returns>
        public static ResultException ToResultException<TException>(this TException e, bool isHideStackTrace = false)where TException:Exception
        {
            if (e == null) return null;
                ResultException result = new ResultException(e.Message);
                return result;
            
        }

        /// <summary>
        /// 格式化友好异常
        /// </summary>
        /// <param name="e">异常对象</param>
        /// <param name="isHideStackTrace">是否隐藏异常规模信息</param>
        /// <returns>格式化后的异常信息字符串</returns>
        public static UserFriendlyException ToUserFriendlyException<TException>(this TException e, bool isHideStackTrace = false) where TException : Exception
        {
            if (e == null) return null;
            var message = string.Empty;
            if (e.Data != null && e.Data.Keys.Count != 0)
            {
                try
                {
                    message = (e.Data["paramName"] != null) ? e.Data["paramName"].ToString() : e.Message.Replace("★", "异常编码:");
                }
                catch(Exception ex)
                {
                    LogService.Default.Error("ToUserFriendlyException:框架异常消息转化发生异常.");
                    message = e.Message.Replace("★", "异常编码:");
                }
              
            }
            else
            {
              message =e.Message.Replace("★", "异常编码:");
            }
            UserFriendlyException result = new UserFriendlyException(message);
            return result;

        }

        /// <summary>
        /// 格式化异常信息
        /// </summary>
        /// <param name="e">异常对象</param>
        /// <param name="isHideStackTrace">是否隐藏异常规模信息</param>
        /// <returns>格式化后的异常信息字符串</returns>
        public static ValidationResult ToValidationResult<TException>(this TException e, bool isHideStackTrace = false) where TException : Exception
        {
            if (e == null) return null;
            ValidationResult result = new ValidationResult();
            return result;

        }
        /// <summary>
        /// 格式化友好异常
        /// </summary>
        /// <param name="e">异常对象</param>
        /// <param name="isHideStackTrace">是否隐藏异常规模信息</param>
        /// <returns>格式化后的异常信息字符串</returns>
        public static Exception ToException(this ValidationResult e, Func<ValidationResult,string> eFun,  bool isHideStackTrace = false)
        {
            if (e != null) return null;
            string message = eFun(e);
            Exception exception = (Exception)Activator.CreateInstance(typeof(Exception), message);
            return exception;


        }

        /// <summary>
        /// 格式化友好异常
        /// </summary>
        /// <param name="e">异常对象</param>
        /// <param name="isHideStackTrace">是否隐藏异常规模信息</param>
        /// <returns>格式化后的异常信息字符串</returns>
        public static void ThrowUserFriendlyException<TException>(this TException e, bool isHideStackTrace = false) where TException : Exception
        {
            if (e == null) return ;
                   e.ToUserFriendlyException().Throw();
           
        }


        /// <summary>
        /// 由上层决定异常是否抛出
        /// </summary>
        /// <param name="e">异常对象</param>
        /// <param name="isHideStackTrace">是否隐藏异常规模信息</param>
        /// <returns>格式化后的异常信息字符串</returns>
        public static void Throw<TException>(this TException e, bool isHideStackTrace = false) where TException : Exception
        {

             if(e!=null)
             {
                LogService.Default.Error(e.FormatMessage(isHideStackTrace));
                throw e;

            }
        }

        /// <summary>
        /// 格式化异常消息
        /// </summary>
        /// <param name="e">异常对象</param>
        /// <param name="isHideStackTrace">是否隐藏异常规模信息</param>
        /// <returns>格式化后的异常信息字符串</returns>
        public static string FormatMessage(this Exception e, bool isHideStackTrace = false)
        {
            StringBuilder sb = new StringBuilder();
            int count = 0;
            string appString = string.Empty;
            while (e != null)
            {
                if (count > 0)
                {
                    appString += "  ";
                }
                sb.AppendLine(string.Format("{0}异常消息：{1}", appString, e.Message));
                sb.AppendLine(string.Format("{0}异常类型：{1}", appString, e.GetType().FullName));
                sb.AppendLine(string.Format("{0}异常方法：{1}", appString, (e.TargetSite == null ? null : e.TargetSite.Name)));
                sb.AppendLine(string.Format("{0}异常源：{1}", appString, e.Source));
                if (!isHideStackTrace && e.StackTrace != null)
                {
                    sb.AppendLine(string.Format("{0}异常堆栈：{1}", appString, e.StackTrace));
                }
                if (e.InnerException != null)
                {
                    sb.AppendLine(string.Format("{0}内部异常：", appString));
                    count++;
                }
                e = e.InnerException;
            }
              return sb.ToString();
        }
    }
}