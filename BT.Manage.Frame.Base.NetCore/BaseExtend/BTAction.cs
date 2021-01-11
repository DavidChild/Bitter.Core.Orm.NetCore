using System;
using System.Collections.Generic;
using System.Text;
using BT.Manage.Tools;

namespace BT.Manage.Frame.Base
{
    public class BTAction
    {
        public static Result DoTryAction(Func<Result> action, string errMsg = null, string successMsg = null)
        {
            Result r = new Base.Result();
            try
            {
                r = action();
                if (r.code == 1)
                {
                    if (string.IsNullOrEmpty(r.message))
                        r.message = successMsg;
                }
                else
                {
                    if (string.IsNullOrEmpty(r.message))
                        r.message = errMsg;
                }

            }
            catch (Exception ex)
            {
                LogService.Default.Fatal("执行异常：" + errMsg + ex.Message + ex.StackTrace);
                if (string.IsNullOrWhiteSpace(errMsg))
                    errMsg = ex.Message;
                return r.SetZeroResult(ex.Message);
            }
            return r;
        }
    }
}
