using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bitter.Tools
{
    public class ReflexHelper
    {
        /// <summary>
        /// 根据model获取表名（只获取表名）
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="data">model</param>
        /// <returns>表名</returns>
        public static string GetTableName<T>(T data)
        {
            string tableName = string.Empty;    //数据库名
            var sd = data.GetType().GetCustomAttributes(true);
            for (int i = 0; i < sd.Count(); i++)
            {
                if (sd.GetValue(i).GetType().Name == "TableName")
                {
                    Tools.Attributes.TableName tableNameTmp = sd[i] as Tools.Attributes.TableName;
                    if (tableNameTmp != null)
                    {
                        tableName = tableNameTmp.name;
                    }
                }
            }
            return tableName;
        }
        

        /// <summary>
        /// 根据Type获取表名（只获取表名）
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="data">model</param>
        /// <returns>表名</returns>
        public static string GetTableName(Type type)
        {
            string tableName = string.Empty;    //数据库名
            var sd = type.GetCustomAttributes(true);
            for (int i = 0; i < sd.Count(); i++)
            {
                if (sd.GetValue(i).GetType().Name == "TableName")
                {
                    Tools.Attributes.TableName tableNameTmp = sd[i] as Tools.Attributes.TableName;
                    if (tableNameTmp != null)
                    {
                        tableName = tableNameTmp.name;
                    }
                }
            }
            return tableName;
        }

        /// <summary>
        /// 根据model获取主键字段
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="data">model</param>
        /// <returns>主键字段</returns>
        public static Type GetPropertyType<T>(T data, string filedName)
        {
            //string KeyFiled = string.Empty;    //主键字段
            //var sd = data.GetType().GetCustomAttributes(true);
            PropertyInfo[] properties = data.GetType().GetProperties();
            PropertyInfo zk = data.GetType().GetProperties().Where(k => k.Name == filedName).FirstOrDefault();
            return zk.PropertyType;
        }
        /// <summary>
        /// 根据Type获取主键字段
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="data">model</param>
        /// <returns>主键字段</returns>
        public static string GetKeyFiledName(Type type)
        {
            string KeyFiled = string.Empty;    //主键字段
            var sd = type.GetCustomAttributes(true);
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                Type propertyType = propertyInfo.PropertyType;
                var attrs = propertyInfo.GetCustomAttributes(true).Where(p => p.GetType().Name == "KeyAttribute");
                if (attrs.Count() > 0)
                {
                    KeyFiled = propertyInfo.Name;
                    break;
                }
                else
                    continue;
            }
            return KeyFiled;
        }
        /// <summary>
        /// 根据model获取主键字段
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="data">model</param>
        /// <returns>主键字段</returns>
        public static string GetDisplayName<T>(T data, string filedName)
        {
            //string KeyFiled = string.Empty;    //主键字段
            //var sd = data.GetType().GetCustomAttributes(true);
            PropertyInfo[] properties = data.GetType().GetProperties();
            PropertyInfo zk = data.GetType().GetProperties().Where(k => k.Name == filedName).FirstOrDefault();
            var f = zk.GetCustomAttributes(false).Where(p => p.GetType().Name == "DisplayAttribute").FirstOrDefault();
            if (f == null)
            {
                return string.Empty;
            }
            else
            {
                return ((System.ComponentModel.DataAnnotations.DisplayAttribute)f).Name.Trim().Split(new char[] { ' ' }).FirstOrDefault();
            }
        }

        /// <summary>
        /// 根据model获取主键字段
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="data">model</param>
        /// <returns>主键字段</returns>
        public static string GetDisplayName(Type type, string filedName)
        {
            //string KeyFiled = string.Empty;    //主键字段
            //var sd = data.GetType().GetCustomAttributes(true);
            PropertyInfo[] properties = type.GetProperties();
            PropertyInfo zk = type.GetProperties().Where(k => k.Name == filedName).FirstOrDefault();
            var f = zk.GetCustomAttributes(false).Where(p => p.GetType().Name == "DisplayAttribute").FirstOrDefault();
            if (f == null)
            {
                return string.Empty;
            }
            else
            {
                return ((System.ComponentModel.DataAnnotations.DisplayAttribute)f).Name.Trim().Split(new char[] { ' ' }).FirstOrDefault();
            }
        }

        /// <summary>
        /// 根据反射获取属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="propertyname"></param>
        /// <returns></returns>
        public static object GetPropertyValue<T>(T t, string propertyname)
        {
            Type type = t.GetType();

            PropertyInfo property = type.GetProperty(propertyname);

            if (property == null) return string.Empty;

            object o = property.GetValue(t, null);

            if (o == null) return string.Empty;

            return o;
        }

     

        //判断是否是自增长模型
        public static bool CheckedIsIdentityModel(Type type)
        {
            bool isIdentityKey = false;
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                string FieldName = propertyInfo.Name;
                if (FieldName.Length > 0 && FieldName.IndexOf('_') == 0) continue; //continue
                object[] attrs = propertyInfo.GetCustomAttributes(true);
                for (int i = 0; i < attrs.Count(); i++)
                {
                    string memberInfoName = attrs.GetValue(i).GetType().Name;
                    if (memberInfoName == "IdentityAttribute")
                    {
                        Tools.Attributes.IdentityAttribute keyAttr = attrs[i] as Tools.Attributes.IdentityAttribute;
                        if (keyAttr != null)
                        {
                            isIdentityKey = true;
                            break;
                        }
                    }
                }
                if (isIdentityKey) break;
            }
            return isIdentityKey;
        }
        public  static string GetPropertyName<T>(Expression<Func<T, object>> expr)
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
    }
}
