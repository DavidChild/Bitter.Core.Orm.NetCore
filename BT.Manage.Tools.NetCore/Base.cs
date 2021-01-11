using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

namespace BT.Manage.Tools
{
    /*----------------------------------------------------------------
    // Copyright (C) 2016 备胎 版权所有。
    //
    // 文件名：Base.cs 文件功能描述：【保险订单】
    //
    // 创建标识：老前 2016年1月09日13:48:34 修改： 陈乔 2016年1月15日17:59:30 添加方法GetCurrentPageAbsolutePath
     * 修改： 陈乔 2016年1月21日14:45:30 修改方法IsMobile, IsIDCard正则验证
    //----------------------------------------------------------------*/

    public class Base
    {
      
        #region "过滤特殊符号"

        /// <summary>
        /// 过滤特殊特号(完全过滤)
        /// </summary>
        /// <param name="Str"></param>
        /// <returns></returns>
        public static string FilterSql(string Str)
        {
            string[] aryReg = { "'", "\"", "\r", "\n", "<", ">", "%", "?", ",", "=", "-", "_", ";", "|", "[", "]", "&", "/" };
            if (!string.IsNullOrEmpty(Str))
            {
                foreach (string str in aryReg)
                {
                    Str = Str.Replace(str, string.Empty);
                }
                return Str;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Json特符字符过滤，参见http://www.json.org/
        /// </summary>
        /// <param name="sourceStr">要过滤的源字符串</param>
        /// <returns>返回过滤的字符串</returns>
        public static string JsonCharFilter(string sourceStr)
        {
            sourceStr = sourceStr.Replace("\\", "\\\\");
            sourceStr = sourceStr.Replace("\b", "\\\b");
            sourceStr = sourceStr.Replace("\t", "\\\t");
            sourceStr = sourceStr.Replace("\n", "\\\n");
            sourceStr = sourceStr.Replace("\n", "\\\n");
            sourceStr = sourceStr.Replace("\f", "\\\f");
            sourceStr = sourceStr.Replace("\r", "\\\r");
            return sourceStr.Replace("\"", "\\\"");
        }

        /// <summary>
        /// 过滤掉"<>"符号内除">"符号的内容
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RemoveHtml(string content)
        {
            string pattern = "<[^>]*>";
            return Regex.Replace(content, pattern, string.Empty, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 过滤HTML中的不安全标签
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RemoveUnsafeHtml(string content)
        {
            content = Regex.Replace(content, @"(\<|\s+)o([a-z]+\s?=)", "$1$2", RegexOptions.IgnoreCase);
            content = Regex.Replace(content, @"(script|frame|form|meta|behavior|style)([\s|:|>])+", "$1.$2", RegexOptions.IgnoreCase);
            return content;
        }

        #endregion "过滤特殊符号"

        #region "字符格式检查"

        /// <summary>
        /// 邮件地址格式判断
        /// </summary>
        /// <param name="Email">邮件地址</param>
        /// <returns></returns>
        public static bool IsEmail(string Email)
        {
            return Regex.IsMatch(Email, @"\\w{1,}@\\w{1,}\\.\\w{1,}");
        }

        /// <summary>
        /// 身份证号码判断
        /// </summary>
        /// <param name="IDCard">身份证号码</param>
        /// <returns></returns>
        public static bool IsIDCard(string IDCard)
        {
            //(^\d{15}$)|(^\d{17}([0-9]|X)$)
            return Regex.IsMatch(IDCard, @"(^\d{15}$)|(^\d{17}([0-9]|X|x)$)");
        }

        /// <summary>
        /// 手机号码格式判断
        /// </summary>
        /// <param name="Mobile">手机号码</param>
        /// <returns></returns>
        public static bool IsMobile(string Mobile)
        {
            //@"^1[3|5|8]\d{9}$"
            return Regex.IsMatch(Mobile, @"^1[3|5|8]\d{9}$");
        }

        /// <summary>
        /// 数字格式判断
        /// </summary>
        /// <param name="Number">数字</param>
        /// <returns></returns>
        public static bool IsNumber(string Number)
        {
            return Regex.IsMatch(Number, @"^[0-9]*$");
        }

        /// <summary>
        /// 邮政编码格式判断
        /// </summary>
        /// <param name="PostCode">邮政编码</param>
        /// <returns></returns>
        public static bool IsPostCode(string PostCode)
        {
            return Regex.IsMatch(PostCode, @"^\d{6}$");
        }

        /// <summary>
        /// 固定电话格式判断
        /// </summary>
        /// <param name="TelPhone">固定电话</param>
        /// <returns></returns>
        public static bool IsTelphone(string TelPhone)
        {
            return Regex.IsMatch(TelPhone, @"^(\d{3,4}-)?\d{6,8}$");
        }

        #endregion "字符格式检查"

        #region "合并数据表格"

        /// <summary>
        /// 合并DataTable
        /// </summary>
        /// <param name="DataTable1">表1</param>
        /// <param name="DataTable2">表2</param>
        /// <param name="DTName">合并后新表的名称</param>
        /// <returns>合并后的新表</returns>
        public static DataTable MergerDt(DataTable DataTable1, DataTable DataTable2, string DTName)
        {
            //克隆DataTable1的结构
            DataTable newDataTable = DataTable1.Clone();
            for (int i = 0; i < DataTable2.Columns.Count; i++)
            {
                //再向新表中加入DataTable2的列结构
                newDataTable.Columns.Add(DataTable2.Columns[i].ColumnName);
            }
            object[] obj = new object[newDataTable.Columns.Count];
            //添加DataTable1的数据
            for (int i = 0; i < DataTable1.Rows.Count; i++)
            {
                DataTable1.Rows[i].ItemArray.CopyTo(obj, 0);
                newDataTable.Rows.Add(obj);
            }
            if (DataTable1.Rows.Count >= DataTable2.Rows.Count)
            {
                for (int i = 0; i < DataTable2.Rows.Count; i++)
                {
                    for (int j = 0; j < DataTable2.Columns.Count; j++)
                    {
                        newDataTable.Rows[i][j + DataTable1.Columns.Count] = DataTable2.Rows[i][j].ToString();
                    }
                }
            }
            else
            {
                DataRow dr3;
                //向新表中添加多出的几行
                for (int i = 0; i < DataTable2.Rows.Count - DataTable1.Rows.Count; i++)
                {
                    dr3 = newDataTable.NewRow();
                    newDataTable.Rows.Add(dr3);
                }
                for (int i = 0; i < DataTable2.Rows.Count; i++)
                {
                    for (int j = 0; j < DataTable2.Columns.Count; j++)
                    {
                        newDataTable.Rows[i][j + DataTable1.Columns.Count] = DataTable2.Rows[i][j].ToString();
                    }
                }
            }
            newDataTable.TableName = DTName; //设置DT的名字
            return newDataTable;
        }

        #endregion "合并数据表格"

        #region "根据类型转换"

        /// <summary>
        /// 根据类型转换值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static object GetDefaultValue(object obj, Type type)
        {
            try
            {
                if (obj == null || obj == DBNull.Value)
                {
                    obj = default(object);
                }
                else
                {
                    if (type == typeof(String))
                        obj = obj.ToString().Trim();
                    obj = Convert.ChangeType(obj, Nullable.GetUnderlyingType(type) ?? type);
                }
                return obj;
            }
            catch
            {
                return null;
            }
        }

        #endregion "根据类型转换"

        #region "日期时间格式"

        /// <summary>
        /// 返回标准日期(年-月-日)
        /// </summary>
        public static string GetDate()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// 返回标准日期(年-月-日)
        /// </summary>
        /// <param name="dataTime">日期时间</param>
        /// <returns></returns>
        public static string GetDate(string dataTime)
        {
            string Str = dataTime;
            if (!string.IsNullOrEmpty(dataTime))
            {
                try
                {
                    Str = Convert.ToDateTime(dataTime).ToString("yyyy-MM-dd");
                }
                catch
                {
                }
            }
            return Str;
        }

        /// <summary>
        /// 返回标准时间(年-月-日 时-分-秒)
        /// </summary>
        public static string GetDateTime()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 返回标准时间(年-月-日 时-分-秒)
        /// </summary>
        /// <param name="dataTime">日期时间</param>
        /// <returns></returns>
        public static string GetDateTime(string dataTime)
        {
            string Str = dataTime;
            if (!string.IsNullOrEmpty(dataTime))
            {
                try
                {
                    Str = Convert.ToDateTime(dataTime).ToString("yyyy-MM-dd HH:mm:ss");
                }
                catch
                {
                }
            }
            return Str;
        }

        /// <summary>
        /// 返回标准时间(年-月-日 时-分-秒)
        /// </summary>
        /// <param name="Days">相对天数</param>
        /// <returns></returns>
        public static string GetDateTime(int Days)
        {
            return DateTime.Now.AddDays(Days).ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 返回标准时间(年-月-日 时-分-秒-毫秒)
        /// </summary>
        public static string GetDateTimeF()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fffffff");
        }

        /// <summary>
        /// 返回标准时间(两位日)
        /// </summary>
        /// <param name="dataTime"></param>
        /// <returns></returns>
        public static string GetDay(string dataTime)
        {
            string Str = dataTime;
            if (!string.IsNullOrEmpty(dataTime))
            {
                try
                {
                    Str = Convert.ToDateTime(dataTime).ToString("dd");
                }
                catch
                {
                }
            }
            return Str;
        }

        /// <summary>
        /// 返回标准时间(两位时)
        /// </summary>
        /// <param name="dataTime"></param>
        /// <returns></returns>
        public static string GetHour(string dataTime)
        {
            string Str = dataTime;
            if (!string.IsNullOrEmpty(dataTime))
            {
                try
                {
                    Str = Convert.ToDateTime(dataTime).ToString("HH");
                }
                catch
                {
                }
            }
            return Str;
        }

        /// <summary>
        /// 返回标准时间(两位分)
        /// </summary>
        /// <param name="dataTime"></param>
        /// <returns></returns>
        public static string GetMinute(string dataTime)
        {
            string Str = dataTime;
            if (!string.IsNullOrEmpty(dataTime))
            {
                try
                {
                    Str = Convert.ToDateTime(dataTime).ToString("mm");
                }
                catch
                {
                }
            }
            return Str;
        }

        /// <summary>
        /// 返回标准时间(两位月)
        /// </summary>
        /// <param name="dataTime"></param>
        /// <returns></returns>
        public static string GetMonth(string dataTime)
        {
            string Str = dataTime;
            if (!string.IsNullOrEmpty(dataTime))
            {
                try
                {
                    Str = Convert.ToDateTime(dataTime).ToString("MM");
                }
                catch
                {
                }
            }
            return Str;
        }

        /// <summary>
        /// 返回月份间隔
        /// </summary>
        /// <param name="dataTime1">起始时间</param>
        /// <param name="dataTime2">结束时间</param>
        /// <returns></returns>
        public static string GetMonth(string dataTime1, string dataTime2)
        {
            int Month = 0;
            DateTime dtbegin = Convert.ToDateTime(dataTime1);
            DateTime dtend = Convert.ToDateTime(dataTime2);
            Month = (dtend.Year - dtbegin.Year) * 12 + (dtend.Month - dtbegin.Month);
            return Month.ToString();
        }

        /// <summary>
        /// 返回标准时间(两位秒)
        /// </summary>
        /// <param name="dataTime"></param>
        /// <returns></returns>
        public static string GetSecond(string dataTime)
        {
            string Str = dataTime;
            if (!string.IsNullOrEmpty(dataTime))
            {
                try
                {
                    Str = Convert.ToDateTime(dataTime).ToString("ss");
                }
                catch
                {
                }
            }
            return Str;
        }

        /// <summary> 返回标准时间 yyyy-MM-dd </sumary>
        public static string GetStandardDate(string fDate)
        {
            return GetStandardDateTime(fDate, "yyyy-MM-dd");
        }

        /// <summary>
        /// 返回相对时间
        /// </summary>
        /// <param name="fDateTime">相对时间</param>
        /// <param name="formatStr">日期时间格式</param>
        /// <returns></returns>
        public static string GetStandardDateTime(string fDateTime, string formatStr)
        {
            if (fDateTime == "0000-0-0 0:00:00")
                return fDateTime;
            DateTime time = new DateTime(1900, 1, 1, 0, 0, 0, 0);
            if (DateTime.TryParse(fDateTime, out time))
                return time.ToString(formatStr);
            else
                return "N/A";
        }

        /// <summary> 返回标准时间 yyyy-MM-dd HH:mm:ss </sumary>
        public static string GetStandardDateTime(string fDateTime)
        {
            return GetStandardDateTime(fDateTime, "yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 返回标准时间(时-分-秒)
        /// </summary>
        public static string GetTime()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }

        /// <summary>
        /// 返回标准时间(四位年)
        /// </summary>
        /// <param name="dataTime"></param>
        /// <returns></returns>
        public static string GetYear(string dataTime)
        {
            string Str = dataTime;
            if (!string.IsNullOrEmpty(dataTime))
            {
                try
                {
                    Str = Convert.ToDateTime(dataTime).ToString("yyyy");
                }
                catch
                {
                }
            }
            return Str;
        }

        #endregion "日期时间格式"

        #region "转成汉字数字"

        /// <summary>
        /// 转成汉字数字
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static string ConvertToChinese(double x)
        {
            string s = x.ToString("#L#E#D#C#K#E#D#C#J#E#D#C#I#E#D#C#H#E#D#C#G#E#D#C#F#E#D#C#.0B0A"); string d = Regex.Replace(s, @"((?<=-|^)[^1-9]*)|((?'z'0)[0A-E]*((?=[1-9])|(?'-z'(?=[F-L\.]|$))))|((?'b'[F-L])(?'z'0)[0A-L]*((?=[1-9])|(?'-z'(?=[\.]|$))))", "${b}${z}"); return Regex.Replace(d, ".", delegate(Match m) { return "负元空零壹贰叁肆伍陆柒捌玖空空空空空空空分角拾佰仟万亿兆京垓秭穰"[m.Value[0] - '-'].ToString(); });
        }

        /// <summary>
        /// 转成汉字数字
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static string ConvertToChinese(int x)
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            dic.Add(0, "零");
            dic.Add(1, "一");
            dic.Add(2, "二");
            dic.Add(3, "三");
            dic.Add(4, "四");
            dic.Add(5, "五");
            dic.Add(6, "六");
            dic.Add(7, "七");
            dic.Add(8, "八");
            dic.Add(9, "九");

            string svalue = string.Empty;
            foreach (int key in dic.Keys)
            {
                if (key == x)
                {
                    svalue = dic[key];
                    break;
                }
            }
            return svalue;
        }

        #endregion "转成汉字数字"

        /// <summary>
        /// 隐藏身份证号中间 @Qian
        /// </summary>
        /// <param name="str">身份证号</param>
        /// <returns></returns>
        public static string IdCardHide(string str)
        {
            string temp = str;
            if (str.Length == 18)
            {
                temp = str.Substring(0, 4) + "**********" + str.Substring(14, 4);
            }
            if (str.Length == 15)
            {
                temp = str.Substring(0, 4) + "********" + str.Substring(12, 3);
            }
            return temp;
            //if (!string.IsNullOrWhiteSpace(str))
            //{
            //    return str.Substring(0, s) + "**********" + str.Substring(str.Length - e, e);
            //}
            //return str;
        }

        /// <summary>
        /// 隐藏手机号 @Qian
        /// </summary>
        /// <param name="str">手机号</param>
        /// <returns></returns>
        public static string MobileHide(string str)
        {
            string temp = str;
            if (str.Length == 11)
            {
                temp = str.Substring(0, 3) + "*****" + str.Substring(8, 3);
            }
            return temp;
        }
    }
}