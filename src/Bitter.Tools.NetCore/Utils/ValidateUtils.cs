using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Bitter.Tools.Utils
{
    public static class ValidateUtils
    {
        /// <summary>
        /// 校验身份证合法性
        /// </summary>
        /// <param name="idcard"></param>
        /// <returns></returns>
        public static bool ValidateIdCard(this string idcard)
        {
            bool b = true;
            b = IDCardUtils.CheckIdcard(idcard).Contains("True");
            return b;
        }

        /// <summary>
        /// 手机号码校验
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public static bool ValidateMobile(this string mobile)
        {
            Regex regex = new Regex("^1\\d{10}$");
            return regex.IsMatch(mobile);
        }
    }
}
