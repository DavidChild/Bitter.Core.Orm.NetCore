using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Bitter.Tools.Utils;
namespace Bitter.Tools
{
    public class ReflexWriteByType
    {
        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <returns></returns>
        public static object CreateModelFromRow(Type type, DataRow row)
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
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <returns></returns>
        public static object SetModelFromRow<T>(T data, DataRow row) where T : new()
        {
            
            SetItemFromRow(data, row);
            return data;
        }
        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <returns></returns>
        public static T CreateModelFromRow<T>(T item, DataRow row) where T : new()
        {
            SetItemFromRow(item, row);
            return item;
        }

        public static void SetItemFromRow<T>(T item, DataRow row) where T : new()
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
        /// 根据model获取主键字段
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="data">model</param>
        /// <returns>主键字段</returns>
        public static void SetPropertyValue<T>(T data, string filedName,object value)
        {
           
            PropertyInfo[] properties = data.GetType().GetProperties();
            PropertyInfo zk = data.GetType().GetProperties().Where(k => k.Name == filedName).FirstOrDefault();
            try
            {
                if ((value.ToSafeString("") == "") && (zk.PropertyType).Name == "Nullable`1")
                {
                    zk.SetValue(data, null);
                }
                else if
                    (value.ToSafeString("") == ""
                    &&
                    ((zk.PropertyType).Name.ToLower().IndexOf("int") > -1
                    || (zk.PropertyType).Name.ToLower().IndexOf("decimal") > -1
                    || (zk.PropertyType).Name.ToLower().IndexOf("float") > -1
                    || (zk.PropertyType).Name.ToLower().IndexOf("long") > -1
                    || (zk.PropertyType).Name.ToLower().IndexOf("double") > -1
                    )
                  )
                {

                }
                else zk.SetValue(data, value);
            }
            catch(Exception ex)
            {
                LogService.Default.Fatal("Mpping字段类型映射赋值出错：Model:【" + data.GetType().FullName + "】，字段【" + filedName + "】,值【"+ value==null?"null":value.ToSafeString()+"】");
                throw ex;
            }
        }

    }
}
