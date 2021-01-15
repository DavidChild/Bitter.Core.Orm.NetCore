using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Bitter.Base
{
    /********************************************************************************
    ** auth： davidchild
    ** date： 2017/3/8 10:45:19
    ** desc：
    ** Ver.:  V1.0.0
    ** Copyright (C) 2016 Bitter 版权所有。
    *********************************************************************************/

    public class EntitySign
    {
        /// <summary>
        /// 递归实现签名规则
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <param name="singOrignInfo"></param>
        /// <returns></returns>
        public static StringBuilder GetEntitySign(Object o, StringBuilder singOrignInfo)
        {
            foreach (PropertyInfo property in o.GetType().GetProperties().OrderBy(p => p.Name))
            {
                if (property.Name == "sign") continue;
                else if (property.PropertyType.Name.ToLower() == "string"
                    || property.PropertyType.Name.ToLower() == "int"
                    || property.PropertyType.Name.ToLower() == "int32"
                    || property.PropertyType.Name.ToLower() == "long"
                    || property.PropertyType.Name.ToLower() == "int64"
                    || property.PropertyType.Name.ToLower() == "int?"
                    || property.PropertyType.Name.ToLower() == "decimal"
                    || ((property.PropertyType.IsGenericType) && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    )
                {
                    var filedValue = property.GetValue(o, null);
                    if (filedValue != null)
                    {
                        singOrignInfo.Append(property.GetValue(o, null));
                    }
                }
                else if (property.PropertyType.IsGenericType)
                {
                    foreach (var ko in (ICollection<dynamic>)property.GetValue(o, null))
                    {
                        GetEntitySign(ko, singOrignInfo);
                    }
                }
                else
                {
                    return GetEntitySign(property.GetValue(o, null), singOrignInfo);
                }
            }
            return singOrignInfo;
        }

        /// <summary>
        /// 将字符串转为32位的MD5编码
        /// </summary>
        /// <param name="str">输入的字符串</param>
        /// <returns>32位编码的字符串</returns>
        public static string To32Md5(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            StringBuilder sb = new StringBuilder();

            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            md5.Clear();
            for (int i = 0; i < s.Length; i++)
            {
                sb.Append(s[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}