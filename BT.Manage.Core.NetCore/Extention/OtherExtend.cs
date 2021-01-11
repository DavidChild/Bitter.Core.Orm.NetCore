
using BT.Manage.Tools.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Newtonsoft.Json;
using BT.Manage.Tools.Utils;
using BT.Manage.Tools;

namespace BT.Manage.Core
{
    /********************************************************************************
    ** auth： Jason
    ** date： 2017/2/21 17:02:13
    ** desc：
    ** Ver.:  V1.0.0
    ** Copyright (C) 2016 备胎 版权所有。
    *********************************************************************************/

    public static class OtherExtend
    {

        /// <summary>
        /// 取出DataTable的Rows[0][0] 数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static T TryGet<T>(this DataTable dt)
        {
            if (dt == null || dt.Rows.Count <= 0)
            {
                return default(T);
            }
            else
            {
                return TryCast.CastTo<T>(dt.Rows[0][0]);
            }
        }
        public static DataTable GetToDataTable(this List<BulkCopyModel> collection, Type type, string target)
        {
            var props = type.GetProperties();


            string tableName = Utils.GetTableName(type);
            string sqlDataColumns = "SElect TOP 1 * FROM " + tableName + " WHERE 1=0";
            DataTable dtColumns = new DataTable();
            dtColumns = db.FindQuery(sqlDataColumns, null, target).Find();
            if (dtColumns == null) return null;

            dtColumns.TableName = tableName;
            if (collection.Count > 0)
            {


                foreach (var item in collection)
                {
                    DataRow dr = dtColumns.NewRow();
                    foreach (DataColumn column in dtColumns.Columns)
                    {
                        var propx = props.Where(p => p.Name == column.ColumnName).ToList();
                        if (propx != null && propx.Count > 0)
                        {
                            var px = propx.First();
                            object obj = px.GetValue(item.CopyModel, null);
                            if (obj != null)
                                dr[column.ColumnName] = obj;
                            else
                                dr[column.ColumnName] = DBNull.Value;
                        }

                    }
                    dtColumns.Rows.Add(dr);
                }




            }
            return dtColumns;
        }

        internal static DataTable ToDataTable(this List<BulkCopyModel> collection, Type type)
        {
            return GetToDataTable(collection, type, null);
        }

        internal static DataTable ToDataTable(this List<BulkCopyModel> collection, Type type, string target)
        {
            return GetToDataTable(collection, type, target);
        }

        /// <summary>
        /// 将指定的DataTable转化为BListModel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        internal static BList<T> ToBListModel<T>(this DataTable dt) where T : class, new()
        {
            BList<T> lt = new BList<T>();
            if (dt == null || dt.Rows.Count == 0)
            {
                return lt;
            }
            else
            {
                try
                {
                    lt = JsonConvert.DeserializeObject<BList<T>>(JsonConvert.SerializeObject(dt));
                }
                catch (Exception ex)
                {
                    LogService.Default.Fatal("异常:"+ex.Message);
                    throw ex;
                }
                
               
            }
            return lt;

        }

        /// <summary>
        /// 取出默认的数据，如果List为空，则构建T对象实例，并且返回 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static TSourece FirstOrDefault<TSourece>(this BList<TSourece> list) where TSourece : class, new()
        {
            if (list == null || list.Count == 0)
            {
                return new TSourece();
            }
            else
            {

                return list.First();
            }
        }

        /// <summary>
        /// 将指定的DataTable转化为ListModel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> ToListModel<T>(this DataTable dt) where T : class
        {
            List<T> lt = new List<T>();
            if (dt == null || dt.Rows.Count == 0)
            {
                return lt;
            }
            else
            {
                try
                {
                    lt = JsonConvert.DeserializeObject<List<T>>(JsonConvert.SerializeObject(dt));
                }
                catch (Exception ex)
                {
                    LogService.Default.Fatal("异常:" + ex.Message);
                    throw ex;
                }
                
            }
            return lt;

        }

        /// <summary>
        /// 将指定的DataTable转化为ListModel（前端专用）
        /// 自动将DateTime 格式转化为 Long
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> ToLisModelConvetDateTimeLong<T>(this DataTable dt)
        {
            var list = new List<T>();
            var plist = new List<PropertyInfo>(typeof(T).GetProperties());
            foreach (DataRow item in dt.Rows)
            {
                T s = Activator.CreateInstance<T>();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    var dtType = dt.Columns[i].DataType;
                    PropertyInfo info = plist.Find(p => p.Name == dt.Columns[i].ColumnName);
                    if (info != null)
                    {
                        if (!Convert.IsDBNull(item[i]))
                        {
                            if (dtType == typeof(DateTime))
                                info.SetValue(s, ((DateTime)item[i]).ToDateLong(), null);
                            else
                            {
                                if (info.PropertyType == typeof(Boolean))
                                {
                                    info.SetValue(s, item[i].ToSafeBool(), null);
                                }
                                else
                                {
                                    info.SetValue(s, item[i], null);
                                }
                            }

                        }

                    }
                }
                list.Add(s);
            }
            return list;
        }








        ///// <summary>
        ///// DataTable转成List
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="dt"></param>
        ///// <returns></returns>
        //public static List<T> ToList<T>(this DataTable dt)
        //{
        //    var list = new List<T>();
        //    var plist = new List<PropertyInfo>(typeof(T).GetProperties());
        //    foreach (DataRow item in dt.Rows)
        //    {
        //        T s = Activator.CreateInstance<T>();
        //        for (int i = 0; i < dt.Columns.Count; i++)
        //        {
        //            PropertyInfo info = plist.Find(p => p.Name == dt.Columns[i].ColumnName);
        //            if (info != null)
        //            {
        //                if (!Convert.IsDBNull(item[i]))
        //                {
        //                    info.SetValue(s, item[i], null);
        //                }
        //            }
        //        }
        //        list.Add(s);
        //    }
        //    return list;
        //}

        ///// <summary>
        ///// DataTable转成Dto
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="dt"></param>
        ///// <returns></returns>
        //public static T ToDto<T>(this DataTable dt)
        //{
        //    T s = Activator.CreateInstance<T>();
        //    if (dt == null || dt.Rows.Count == 0)
        //    {
        //        return s;
        //    }
        //    var plist = new List<PropertyInfo>(typeof(T).GetProperties());
        //    for (int i = 0; i < dt.Columns.Count; i++)
        //    {
        //        PropertyInfo info = plist.Find(p => p.Name == dt.Columns[i].ColumnName);
        //        if (info != null)
        //        {
        //            if (!Convert.IsDBNull(dt.Rows[0][i]))
        //            {
        //                info.SetValue(s, dt.Rows[0][i], null);
        //            }
        //        }
        //    }
        //    return s;
        //}

    }
}