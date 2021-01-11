using System;

namespace BT.Manage.Tools.Utils
{
    /********************************************************************************
    ** auth： Jason
    ** date： 2017/2/22 12:40:57
    ** desc：
    ** Ver.:  V1.0.0
    ** Copyright (C) 2016 备胎 版权所有。
    *********************************************************************************/

    public class GuidExtends
    {

        /// <summary>
        /// 获取16的系统生成的guid
        /// </summary>
        /// <returns></returns>
        public static string ShortGuid()
        {
            return GenerateGuid16String(Guid.NewGuid());
        }

        /// Guid to string(16)
        private static string GenerateGuid16String(Guid guid)
        {
            long i = 1;
            foreach (byte b in guid.ToByteArray())
                i *= ((int)b + 1);
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }
    }
}