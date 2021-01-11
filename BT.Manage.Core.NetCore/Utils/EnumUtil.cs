using BT.Manage.Tools.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace BT.Manage.Core.NetCore.Utils
{
    public static class EnumUtil
    {
        private static ConcurrentDictionary<RuntimeTypeHandle, Dictionary<string, string>> _enumDescCache = new ConcurrentDictionary<RuntimeTypeHandle, Dictionary<string, string>>();

        #region 根据枚举值数组获取对应的枚举列表

        /// <summary>
        /// 根据枚举值数组获取对应的枚举列表
        /// </summary>
        /// <typeparam name="EnumType">枚举类型</typeparam>
        /// <param name="enumValues">枚举值</param>
        /// <returns>枚举列表</returns>
        public static EnumType[] GetEnumType<EnumType>(string[] enumValues)
        {
            if (enumValues.IsEmpty()) return null;
            Type enumType = typeof(EnumType);
            List<EnumType> enumList = new List<EnumType>();

            foreach (var item in enumValues)
            {
                int value = item.ToSafeInt32(-100);
                if (Enum.IsDefined(enumType, value))
                {
                    enumList.Add((EnumType)Enum.Parse(enumType, item));
                }
            }

            return enumList.ToArray();
        }

        #endregion 根据枚举值数组获取对应的枚举列表

        #region 获取枚举类型的说明信息

        /// <summary>
        /// 根据枚举值获取枚举描述
        /// </summary>
        /// <param name="enumValue">枚举值</param>
        /// <returns>枚举值对应的描述</returns>
        public static string GetDesc(Enum enumValue)
        {
            var dic = GetDesc(enumValue.GetType());
            string key = Convert.ToInt32(enumValue).ToSafeString();
            if (dic != null && dic.ContainsKey(key))
            {
                return dic[key];
            }
            return string.Empty;
        }

        /// <summary>
        /// 根据枚举值获取枚举描述
        /// </summary>
        /// <typeparam name="EnumType">枚举类型</typeparam>
        /// <param name="value">枚举值（数字）</param>
        /// <returns>枚举值对应的描述</returns>
        public static string GetDesc<EnumType>(string value)
        {
            var dic = GetDesc<EnumType>();
            if (dic != null && dic.ContainsKey(value))
            {
                return dic[value];
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取枚举类型的说明信息
        /// </summary>
        /// <typeparam name="EnumType">枚举类型</typeparam>
        /// <returns>枚举类型的说明信息</returns>
        public static Dictionary<string, string> GetDesc<EnumType>()
        {
            return GetDesc(typeof(EnumType));
        }

        /// <summary>
        /// 获取枚举类型的说明信息
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <returns>枚举类型的说明信息</returns>
        public static Dictionary<string, string> GetDesc(Type enumType)
        {
            if (enumType == null) return new Dictionary<string, string>();
            var typeHandle = enumType.TypeHandle;
            return _enumDescCache.GetValue(typeHandle, () =>
            {
                return GetDescriptions(enumType);
            });
        }

        #endregion 获取枚举类型的说明信息

        #region 根据枚举类型获取枚举说明

        /// <summary>
        /// 根据枚举类型获取枚举说明
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <returns>枚举说明</returns>
        private static Dictionary<string, string> GetDescriptions(Type enumType)
        {
            Dictionary<string, string> enumDic = new Dictionary<string, string>();
            Array enumList = Enum.GetValues(enumType);
            foreach (int item in enumList)
            {
                FieldInfo fieldInfo = enumType.GetField(Enum.GetName(enumType, item));
                if (fieldInfo != null)
                {
                    DescriptionAttribute[] customAttributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
                    if ((customAttributes != null) && (customAttributes.Length == 1))
                    {
                        enumDic.Add(item.ToString(), customAttributes[0].Description);
                    }
                }
            }
            return enumDic;
        }

        #endregion 根据枚举类型获取枚举说明
    }
}