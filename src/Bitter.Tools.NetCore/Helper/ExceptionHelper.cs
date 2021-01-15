using System;

namespace Bitter.Tools
{
    /// <summary>
    /// 功能描述：ExceptionHelper 创 建 者：徐燕 创建日期：2017/3/31 15:51:54
    /// </summary>
    public class ExceptionHelper
    {

        /// <summary>
        /// exception转dynamic
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static dynamic GetExceptionType(Exception ex)
        {
            string name = "";
            Type ty = ex.GetType();
            object ob = Convert.ChangeType(ex, ty);
            dynamic dy = (dynamic)ob;
            return dy;
        }

        /// <summary>
        /// 判断错误信息是否包含Bitter.Application.BaseEntity
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static bool IsSystemException(Exception ex)
        {
            Type ty = ex.GetType();
            string fName = ty.FullName;
            if (fName.IndexOf("Bitter.Application.BaseEntity") > -1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //public static Result TranToResult(Exception ex)
        //{
        //}
    }
}