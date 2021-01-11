using BT.Manage.Core.NetCore.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace BT.Manage.Core.NetCore.Extention
{
    public static partial class Extension
    {
        /// <summary>
        /// 获取枚举值的描述
        /// </summary>
        /// <param name="enumValue">枚举值</param>
        /// <returns>枚举值的描述</returns>
        public static string Desc(this Enum enumValue)
        {
            if (enumValue == null) return string.Empty;
            return EnumUtil.GetDesc(enumValue);
        }

        /// <summary>
        /// 获取枚举值对应的描述
        /// </summary>
        /// <typeparam name="EnumType">枚举类型</typeparam>
        /// <param name="obj">枚举值</param>
        /// <returns>枚举值对应的描述</returns>
        public static string Desc<EnumType>(this object obj)
        {
            if (obj == null) return string.Empty;
            if (string.IsNullOrWhiteSpace(obj.ToString()))
            {
                return string.Empty;
            }
            return EnumUtil.GetDesc<EnumType>(obj.ToString());
        }
    }
}