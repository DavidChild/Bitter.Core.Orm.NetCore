using BT.Manage.Tools.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;


namespace BT.Manage.Tools.Helper
{
    public sealed class ModelHelper<T> where T : class, new()
    {
        /// <summary>
        /// 返回实体列表,自动关闭datareader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static List<T> ConvertManyModel(IDataReader reader)
        {
            return ConvertManyModel(reader, true);
        }

        /// <summary>
        /// 返回实体列表
        /// </summary>
        /// <param name="reader">IDataReader接口</param>
        /// <param name="autoClose">手动控制是否关闭dataReader</param>
        /// <returns></returns>
        public static List<T> ConvertManyModel(IDataReader reader, bool autoClose)
        {
            List<T> list = null;
            if (!reader.IsClosed)
            {
                list = new List<T>();
                try
                {
                    while (reader.Read())
                    {
                        T obj = new T();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string fieldName = reader.GetName(i);
                            PropertyInfo p = obj.GetType().GetProperty(fieldName);
                            if (p == null || !p.CanWrite) continue;
                            p.SetValue(obj, Base.GetDefaultValue(reader[i], p.PropertyType), null);
                        }
                        list.Add(obj);
                    }
                }
                catch
                {
                    reader.Dispose();
                    reader.Close();
                }
                finally
                {
                    if (autoClose)
                    {
                        reader.Dispose();
                        reader.Close();
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 返回单个实体
        /// </summary>
        /// <param name="reader">IDataReader接口</param>
        /// <returns></returns>
        public static T ConvertSingleModel(IDataReader reader)
        {
            return ConvertSingleModel(reader, true);
        }

        /// <summary>
        /// 返回单个实体
        /// </summary>
        /// <param name="reader">IDataReader接口</param>
        /// <returns></returns>
        public static T ConvertSingleModel(IDataReader reader, bool autoClose)
        {
            T t = null;
            if (reader.Read())
            {
                try
                {
                    t = new T();
                    Type modelType = t.GetType();
                    int len = reader.FieldCount;
                    for (int i = 0; i < len; i++)
                    {
                        string filedName = reader.GetName(i);
                        PropertyInfo p = modelType.GetProperty(filedName);
                        if (p == null || !p.CanWrite) continue;
                        p.SetValue(t, Base.GetDefaultValue(reader[p.Name], p.PropertyType), null);
                    }
                }
                catch
                {
                    reader.Dispose();
                    reader.Close();
                }
                finally
                {
                    if (autoClose)
                    {
                        reader.Dispose();
                        reader.Close();
                    }
                }
            }
            return t;
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <returns></returns>
        public static Tt CreateModelFromRow<Tt>(DataRow row) where Tt : new()
        {
            Tt item = new Tt();
            SetItemFromRow(item, row);
            return item;
        }


        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <returns></returns>
        public static  object CreateModelFromRow(Type type,  DataRow row) 
        {
            //查询条件表达式转换成SQL的条件语句
            //获取类的初始化参数信息
            ConstructorInfo ct1 = type.GetConstructor(System.Type.EmptyTypes);
            //调用不带参数的构造器
            var data = ct1.Invoke(null);
            SetItemFromRow(data, row);
            return data;
        }

        /// <summary>
        /// 将datarow赋值到泛型类上
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <returns></returns>
        public static Tt CreateModelFromRow<Tt>(Tt item, DataRow row) where Tt : new()
        {
            SetItemFromRow(item, row);
            return item;
        }

        public static void SetItemFromRow<Tt>(Tt item, DataRow row) where Tt : new()
        {
            foreach (DataColumn c in row.Table.Columns)
            {
                PropertyInfo p = item.GetType().GetProperty(c.ColumnName);
                if (p != null && row[c] != DBNull.Value)
                {
                    try
                    {
                        p.SetValue(item, row[c], null);
                    }
                    catch(Exception ex)
                    {
                        LogService.Default.Fatal("Mpping字段类型映射赋值出错：Model:【"+item.GetType().FullName+"】，字段【" + c.ColumnName+"】");
                        throw ex;
                    }
                }
            }
        }
    }
}