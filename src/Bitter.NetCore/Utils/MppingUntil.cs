using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Bitter.Core
{
    /********************************************************************************
    ** auth： davidchild
    ** date： 2017/1/16 13:20:55
    ** desc： MppingUntil
    ** Ver.:  V1.0.0
    ** Copyright (C) 2016 Bitter 版权所有。
    *********************************************************************************/

    public class MppingUntil
    {
        #region //private:Mapping工具

        public static void SetItemFromRow<T>(T item, DataRow row)
        {
            foreach (DataColumn c in row.Table.Columns)
            {
                PropertyInfo p = item.GetType().GetProperty(c.ColumnName);
                if (p != null && row[c] != DBNull.Value)
                {
                    p.SetValue(item, row[c], null);
                }
            }
        }

        /// <summary>
        /// 匿名类的转换方式
        /// </summary>
        /// <param name="GenericType"></param>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        protected internal static IList FromTable(Type GenericType, DataTable dataTable)
        {
            Type typeMaster = typeof(List<>);
            Type listType = typeMaster.MakeGenericType(GenericType);
            IList list = Activator.CreateInstance(listType) as IList;
            if (dataTable == null || dataTable.Rows.Count == 0)
                return list;
            var constructor = GenericType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                           .OrderBy(c => c.GetParameters().Length).First();
            var parameters = constructor.GetParameters();
            var values = new object[parameters.Length];
            foreach (DataRow dr in dataTable.Rows)
            {
                int index = 0;
                foreach (System.Reflection.ParameterInfo item in parameters)
                {
                    object itemValue = null;
                    if (dr[item.Name] != null && dr[item.Name] != DBNull.Value)
                    {
                        itemValue = Convert.ChangeType(dr[item.Name], item.ParameterType);
                    }
                    values[index++] = itemValue;
                }
                list.Add(constructor.Invoke(values));
            }
            return list;
        }

        /// <summary>
        /// 匿名类的转换方式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        protected internal static List<T> FromTable<T>(DataTable dataTable)
        {
            List<T> list = new List<T>();
            if (dataTable == null || dataTable.Rows.Count == 0)
                return list;
            //取当前匿名类的构造函数
            var constructor = typeof(T).GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                           .OrderBy(c => c.GetParameters().Length).First();
            //取当前构造函数的参数
            var parameters = constructor.GetParameters();
            var values = new object[parameters.Length];
            foreach (DataRow dr in dataTable.Rows)
            {
                int index = 0;
                foreach (System.Reflection.ParameterInfo item in parameters)
                {
                    object itemValue = null;
                    if (dr[item.Name] != null)
                    {
                        itemValue = Convert.ChangeType(dr[item.Name], item.ParameterType);
                    }
                    values[index++] = itemValue;
                }
                T entity = (T)constructor.Invoke(values);
                list.Add(entity);
            }
            return list;
        }

        protected internal static string GetPropertyName<T>(Expression<Func<T, object>> expr)
        {
            var rtn = "";

            if (expr.Body is UnaryExpression)
            {
                rtn = ((MemberExpression)((UnaryExpression)expr.Body).Operand).Member.Name;
            }
            else if (expr.Body is MemberExpression)
            {
                rtn = ((MemberExpression)expr.Body).Member.Name;
            }
            else if (expr.Body is ParameterExpression)
            {
                rtn = ((ParameterExpression)expr.Body).Type.Name;
            }
            return rtn;
        }

        #endregion //private:Mapping工具
    }
}