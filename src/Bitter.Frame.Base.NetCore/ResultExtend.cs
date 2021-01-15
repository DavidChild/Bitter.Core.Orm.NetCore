using Bitter.Tools;
using Bitter.Tools.Helper;
using Bitter.Tools.Utils;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;


namespace Bitter.Base
{
    /********************************************************************************
    ** auth： davidchild
    ** date： 2016/12/9 15:24:25
    ** desc：
    ** Ver.:  V1.0.0
    ** Copyright (C) 2016 Bitter 版权所有。
    *********************************************************************************/

    public static class DynamicHepler
    {
        /// <summary>
        /// 将Result中的object转换成DataTable
        /// </summary>
        /// <param name="dt"></param>
        public static DataTable ToDataTable(this Result o)
        {
            if (o == null)
            {
                return null;
            }
            if (o.code == 0)
            {
                return new DataTable();
            }
            if (o.@object == null)
            {
                return new DataTable();
            }
            string jsonstr = JsonConvert.SerializeObject(o.@object);
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(jsonstr);
            return dt;
        }

        /// <summary>
        /// 将DataTabel中指定的字段转换成指定的C#时间格式
        /// </summary>
        /// <param name="dt">指定</param>
        public static DataTable ToDataTableIntoSysTime(this DataTable dt, string fileds)
        {
            if (dt == null) return null;
            if (dt.Rows.Count == 0) return dt;
            DataTable dtmpe = dt.Copy();
            foreach (string columnnName in fileds.Split(new char[] { ',' }))
            {
                dtmpe.Columns.Remove(columnnName);
                dtmpe.Columns.Add(columnnName, typeof(string));
            }
            foreach (string columnnName in fileds.Split(new char[] { ',' }))
            {
                for (Int32 i = 0; i < dt.Rows.Count; i++)
                {
                    //if (dt.Rows[i][columnnName] == null || string.IsNullOrEmpty(dt.Rows[i][columnnName].ToString()))
                    //{
                    //    dtmpe.Rows[i][columnnName] = defalutValue;
                    //}
                    //else
                    //{
                    //    dtmpe.Rows[i][columnnName] = TryCast.CastTo<long?>(dt.Rows[i][columnnName]).ToSafeLongDataTime();
                    //}
                    dtmpe.Rows[i][columnnName] = TryCast.CastTo<long?>(dt.Rows[i][columnnName]).ToSafeLongDataTime();
                }
            }
            return dtmpe;
        }

        /// <summary>
        /// 将DataTabel中指定的字段转换成时间戳格式
        /// </summary>
        /// <param name="dt">指定的DatTable</param>
        public static DataTable ToDataTableIntoTimeStap(this DataTable dt, string fileds)
        {
            if (dt == null) return null;
            if (dt.Rows.Count == 0) return dt;
            DataTable dtmpe = dt.Copy();
            foreach (string columnnName in fileds.Split(new char[] { ',' }))
            {
                dtmpe.Columns.Remove(columnnName);
                dtmpe.Columns.Add(columnnName, typeof(long));
            }
            foreach (string columnnName in fileds.Split(new char[] { ',' }))
            {
                for (Int32 i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i][columnnName] == null || string.IsNullOrEmpty(dt.Rows[i][columnnName].ToString()))
                    {
                        dtmpe.Rows[i][columnnName] = string.Empty;
                    }
                    else
                    {
                        dtmpe.Rows[i][columnnName] = dt.Rows[i][columnnName].ToString().ToSafeDateTime().ToSafeDataLong();
                    }
                    //dt.Rows[i][columnnName] = dt.Rows[i][columnnName].ToString()(null).ToLongDateString();
                }
            }
            return dtmpe;
        }

       

        /// <summary>
        /// 将当前的Result中的@object转换dynamic对象,使用新的Json.Net(版本：6.0.0.5)取值需要Value
        /// </summary>
        /// <param name="o">当前Result对象</param>
        /// <returns>异常则返回null</returns>
        public static dynamic ToJsonNetDynamic(this Result o)
        {
            try
            {
                if (o == null)
                {
                    return null;
                }
                if (o.@object == null)
                {
                    return null;
                }
                dynamic dy = JsonConvert.DeserializeObject<dynamic>(o.@object);
                return dy;
            }
            catch (Exception ex)
            {
                LogService.Default.Fatal(ex);
                return null;
            }
        }

        /// <summary>
        /// 将当前字符串转换成dynamic对象,使用新的Json.Net(版本：6.0.0.5)取值需要Value
        /// </summary>
        /// <param name="o">当前字符串</param>
        /// <returns>异常则为返回null</returns>
        public static dynamic ToJsonNetDynamic(this string o)
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<dynamic>(o);
                return dy;
            }
            catch (Exception ex)
            {
                LogService.Default.Fatal(ex);
                return null;
            }
        }

        /// <summary>
        /// 将对象转换成Json字符串
        /// </summary>
        /// <param name="o">对象</param>
        /// <returns></returns>
        public static string ToJsonString(this Object o)
        {
            try
            {
                var sentJson = JsonConvert.SerializeObject(o); 
                return sentJson;
            }
            catch (Exception ex)
            {
                LogService.Default.Fatal(ex);
            }

            return string.Empty;
        }

        /// <summary>
        /// 取出返回数据中的Result中的@object,并且将@object转换成Result对象
        /// </summary>
        /// <param name="o">返回的Result对象</param>
        /// <returns>异常:则返回空Result对象:{code=0,message=string.Empty,object=null}</returns>
        public static Result ToResult(this Result o)
        {
            try
            {
                if (o.code == 0)
                {
                    return o;
                }
                if (o.@object == null)
                {
                    LogService.Default.Warn(JsonConvert.SerializeObject(o) + ": 被调用方放回的Result为NULL：/n");
                    return new Result() { message = "被调用方放回的Result为NULL！" };
                }

                return GetObjectByDynamic<Result>(o);
            }
            catch (Exception ex)
            {
                LogService.Default.Fatal(JsonConvert.SerializeObject(o) + ": 转换成系统Result对象失败！异常信息：/n", ex);
                return new Result() { message = ex.Message };
            }
        }

        /// <summary>
        /// 将result中的object 中的集合中的字段转换成时间戳的格式
        /// </summary>
        /// <param name="o"></param>
        /// <param name="fileds"></param>
        /// <returns></returns>
        public static Result ToResultIntoTimeStap(this Result o, string fileds)
        {
            DataTable Dt = ToDataTable(o).ToDataTableIntoTimeStap(fileds);
            o.@object = Dt;
            return o;
        }

        private static T GetObjectByDynamic<T>(this Result R)
        {
            if (R.code == 0)
            {
                return default(T);
            }
            else
            {
                var obj = JsonConvert.DeserializeObject<T>(R.@object);
                return obj;
            }
        }
    }
}
