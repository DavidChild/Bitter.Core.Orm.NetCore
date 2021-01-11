using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace BT.Manage.Tools.Utils
{
    /*----------------------------------------------------------------

    // Copyright (C) 2015 备胎 版权所有。
    //
    // 文件名：ObjectsUtils.cs
    // 文件功能描述：
    //
    // 2016年1月16日10:36:13 添加ToSafeDateTime方法 添加人: yjq
    //----------------------------------------------------------------*/
    public static class ObjectsUtils
    {
        /// <summary>
        /// 判断对象是否为空
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool IsEmpty(this object o)
        {
            return ((o == null) || ((o == DBNull.Value) || Convert.IsDBNull(o)));
        }

        /// <summary>
        /// 对象插入','分隔符
        /// </summary>
        /// <param name="o">目标对象</param>
        /// <returns>返回插入分割符后字符串（为null则返回-1000001）</returns>
        public static string SQLInString(this object o)
        {
            string k = "-1000001";
            if ((o == null))
            {
                return "-1000001";
            }
            else
            {
                dynamic ls = (dynamic)o;
                if (ls.Length == 0)
                {
                    k = string.Empty;
                }
                else
                {
                    k = "'" + string.Join("','", ls) + "'";
                }
            }
            return k;
        }

        /// <summary>
        /// 比较对象和字符"true","false"是否相同
        /// </summary>
        /// <param name="o"></param>
        /// <param name="defValue">默认值</param>
        /// <returns>返回对象和字符"true","false"的比较值（若对象o为null，则返回defValue）</returns>
        public static bool ToSafeBool(this object o, bool defValue)
        {
            if (o != null)
            {
                if (string.Compare(o.ToString().Trim().ToLower(), "true", true) == 0)
                {
                    return true;
                }
                if (string.Compare(o.ToString().Trim().ToLower(), "false", true) == 0)
                {
                    return false;
                }
            }
            return defValue;
        }

        /// <summary>
        /// 比较对象和字符"true","false"是否相同
        /// </summary>
        /// <param name="o"></param>
        /// <returns>返回对象和字符"true","false"的比较值（若对象o为null，则返回null）</returns>
        public static bool? ToSafeBool(this object o)
        {
            if (o != null)
            {
                if (string.Compare(o.ToString().Trim().ToLower(), "true", true) == 0 || o.ToString().Trim() == "1")
                {
                    return true;
                }
                if (string.Compare(o.ToString().Trim().ToLower(), "false", true) == 0 || o.ToString().Trim() == "0")
                {
                    return false;
                }
            }
            return null;
        }

        /// <summary>
        /// 将当前时间转换成时间戳
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static long? ToSafeDataLong(this DateTime? o)
        {
            if (!o.HasValue)
            {
                return null;
            }
            else
            {
                System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                return (long)((o.Value - startTime).TotalMilliseconds);
            }
        }
        /// <summary>
        /// 将当前时间转换成时间戳
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static int? ToSafeDataSecondInt(this DateTime? o)
        {
            if (!o.HasValue)
            {
                return null;
            }
            else
            {
                System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                return (int)((o.Value - startTime).TotalMinutes*60);
            }
        }

        /// <summary>
        /// 对象转时间类型
        /// </summary>
        /// <param name="o"></param>
        /// <param name="defValue">时间默认值</param>
        /// <returns>返回对象o（若o对象为null或为空，返回defValue值）</returns>
        public static DateTime ToSafeDateTime(this object o, DateTime defValue)
        {
            DateTime result = defValue;

            if (o == null || string.IsNullOrWhiteSpace(o.ToString()))
            {
                return result;
            }
            DateTime.TryParse(o.ToString(), out result);

            return result;
        }

        /// <summary>
        /// 对象转时间类型
        /// </summary>
        /// <param name="o"></param>
        /// <returns>返回datetime类型（若不能转时间格式或参数为null为空，则返回null）</returns>
       
        public static DateTime? ToSafeDateTime(this object o,int exactBit=1)
        {
            if (o != null && !string.IsNullOrWhiteSpace(o.ToString()))
            {
                DateTime result;
                if (DateTime.TryParse(o.ToString(), out result))
                    return result;
                else return null;
            }
            return null;
        }

        /// <summary>
        /// 对象转时间类型
        /// </summary>
        /// <param name="o"></param>
        /// <returns>返回datetime类型（若不能转时间格式或参数为null为空，则返回null）</returns>
        public static DateTime? ToSafeDateTime(this object o)
        {
            if (o != null && !string.IsNullOrWhiteSpace(o.ToString()))
            {
                DateTime result;
                if (DateTime.TryParse(o.ToString(), out result))
                    return result;
                else return null;
            }
            return null;
        }

        /// <summary>
        /// 对象转decimal
        /// </summary>
        /// <param name="o"></param>
        /// <param name="defValue">默认返回值</param>
        /// <returns>返回decimal（若对象为null或超过30长度，则返回默认defvalue）</returns>
        public static decimal ToSafeDecimal(this object o, decimal defValue)
        {
            if ((o == null) || (o.ToString().Length > 30))
            {
                return defValue;
            }
            decimal result = defValue;
            if ((o != null) && Regex.IsMatch(o.ToString(), @"^([-]|[0-9])[0-9]*(\.\w*)?$"))
            {
                 decimal.TryParse(o.ToString(), out result);
            }
            return result;
        }

        public static decimal? ToSafeDecimal(this object o)
        {
            if (o != null && Regex.IsMatch(o.ToString(), @"^([-]|[0-9])[0-9]*(\.\w*)?$"))
            {
                decimal result;
                decimal.TryParse(o.ToString().Trim(), out result);
                return result;
            }
            return null;
        }

        /// <summary>
        /// 对象转float
        /// </summary>
        /// <param name="o"></param>
        /// <param name="defValue">默认返回值</param>
        /// <returns>返回float（若对象为null或超过10长度，则返回默认defvalue）</returns>
        public static float ToSafeFloat(this object o, float defValue)
        {
            if ((o == null) || (o.ToString().Length > 10))
            {
                return defValue;
            }
            float result = defValue;
            if ((o != null) && Regex.IsMatch(o.ToString(), @"^([-]|[0-9])[0-9]*(\.\w*)?$"))
            {
                float.TryParse(o.ToString(), out result);
            }
            return result;
        }

        public static float? ToSafeFloat(this object o)
        {
            if (o != null && Regex.IsMatch(o.ToString(), @"^([-]|[0-9])[0-9]*(\.\w*)?$"))
            {
                float result;
                float.TryParse(o.ToString().Trim(), out result);
                return result;
            }
            return null;
        }

        /// <summary>
        /// 对象转int
        /// </summary>
        /// <param name="o"></param>
        /// <param name="defaultValue">默认返回值</param>
        /// <returns>返回int（若对象o为null，或对象无法转int，则返回defaultValue，字符"true"返回1，字符"false"返回0）</returns>
        public static int ToSafeInt32(this object o, int defaultValue)
        {
            if ((o != null) && !string.IsNullOrEmpty(o.ToString()) && Regex.IsMatch(o.ToString().Trim(), @"^([-]|[0-9])[0-9]*(\.\w*)?$"))
            {
                int num;
                string s = o.ToString().Trim().ToLower();
                switch (s)
                {
                    case "true":
                        return 1;

                    case "false":
                        return 0;
                }
                if (int.TryParse(s, out num))
                {
                    return num;
                }
            }
            return defaultValue;
        }

        public static int? ToSafeInt32(this object o)
        {
            if ((o != null) && !string.IsNullOrEmpty(o.ToString()) && Regex.IsMatch(o.ToString().Trim(), @"^([-]|[0-9])[0-9]*(\.\w*)?$"))
            {
                int num;
                string s = o.ToString().Trim().ToLower();
                switch (s)
                {
                    case "true":
                        return 1;

                    case "false":
                        return 0;
                }
                if (int.TryParse(s, out num))
                {
                    return num;
                }
            }
            return null;
        }

        /// <summary>
        /// 对象转Int64（long类型）
        /// </summary>
        /// <param name="o"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static long ToSafeInt64(this object o, int defaultValue)
        {
            if ((o != null) && !string.IsNullOrEmpty(o.ToString()) && Regex.IsMatch(o.ToString().Trim(), @"^([-]|[0-9])[0-9]*(\.\w*)?$"))
            {
                long num;
                string s = o.ToString().Trim().ToLower();
                switch (s)
                {
                    case "true":
                        return 1;

                    case "false":
                        return 0;
                }
                if (long.TryParse(s, out num))
                {
                    return num;
                }
            }
            return defaultValue;
        }

        public static long? ToSafeInt64(this object o)
        {
            if ((o != null) && !string.IsNullOrEmpty(o.ToString()) && Regex.IsMatch(o.ToString().Trim(), @"^([-]|[0-9])[0-9]*(\.\w*)?$"))
            {
                long num;
                string s = o.ToString().Trim().ToLower();
                switch (s)
                {
                    case "true":
                        return 1;

                    case "false":
                        return 0;
                }
                if (long.TryParse(s, out num))
                {
                    return num;
                }
            }
            return null;
        }

        /// <summary>
        /// 将当前时间转换成时间戳
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static DateTime? ToSafeLongDataTime(this long? o)
        {
            if (!o.HasValue)
            {
                return null;
            }
            else if (o.Value == 0)
            {
                return null;
            }
            else
            {
                System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                return (startTime.AddMilliseconds(o.Value));
            }
        }

        /// <summary>
        /// 将当前时间转换成时间戳
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static DateTime? ToSafeIntDataTime(this int? o)
        {
            if (!o.HasValue)
            {
                return null;
            }
            else if (o.Value == 0)
            {
                return null;
            }
            else
            {
                System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                return (startTime.AddMinutes(o.Value/60));
            }
        }

        /// <summary>
        /// 对象转string
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string ToSafeString(this object o)
        {
            return o.ToSafeString(string.Empty);
        }

        public static string ToSafeString(this object o, string defaultString)
        {
            if (o.IsEmpty())
            {
                return defaultString;
            }
            return Convert.ToString(o);
        }

        /// <summary>
        /// 将字符串转换成全角
        /// </summary>
        /// <param name="o">当前字符串</param>
        /// <returns></returns>
        public static string ToSafeSBC(this string o)
        {
            if (string.IsNullOrEmpty(o))
            {
                return o;
            }
            else
            {
                char[] cc = o.ToCharArray();
                for (int i = 0; i < cc.Length; i++)
                {
                    if (cc[i] == 32)
                    {
                        // 表示空格   
                        cc[i] = (char)12288;
                        continue;
                    }
                    if (cc[i] < 127 && cc[i] > 32)
                    {
                        cc[i] = (char)(cc[i] + 65248);
                    }
                }
                return new string(cc);
            }
        }

        /// <summary>
        /// 将字符串转换成半角
        /// </summary>
        /// <param name="o">当前字符串</param>
        /// <returns></returns>
        public static string ToSafeDBC(this string o)
        {
            if (string.IsNullOrEmpty(o))
            {
                return o;
            }
            else
            {
                char[] cc = o.ToCharArray();
                for (int i = 0; i < cc.Length; i++)
                {
                    if (cc[i] == 12288)
                    {
                        // 表示空格   
                        cc[i] = (char)32;
                        continue;
                    }
                    if (cc[i] > 65280 && cc[i] < 65375)
                    {
                        cc[i] = (char)(cc[i] - 65248);
                    }

                }
                return new string(cc);
            }
        }
      

    }
}