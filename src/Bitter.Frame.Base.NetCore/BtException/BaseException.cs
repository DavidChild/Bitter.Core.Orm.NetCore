using Bitter.Tools;
using System;

namespace Bitter.Base
{
    /// <summary>
    /// 功能描述：基础错误处理 创 建 者：徐燕 创建日期：2017/4/5 11:01:54
    /// </summary>
    public static class BaseException
    {
        public static Result CatchBaseException(Exception ex)
        {
            Result re = new Result();
            LogService.Default.Fatal(ex);
            try
            {
                if (!ExceptionHelper.IsSystemException(ex))
                {
                    dynamic dy = ExceptionHelper.GetExceptionType(ex);
                    re = dy.ToResult();
                }
                else
                {
                    LogService.Default.Fatal(ex);
                    re.code = 0;
                    re.message = ex.Message;
                    re.@object = new
                    {
                        ErrorResourceCode = 0,
                        ErrorResourceMsg = ex.Message
                    };
                }
            }
            catch (Exception exception)
            {
                LogService.Default.Fatal(exception);
                re.code = 0;
                re.message = exception.Message;
                re.@object = new
                {
                    ErrorResourceCode = 0,
                    ErrorResourceMsg = exception.Message
                };
            }
            return re;
        }
    }
}