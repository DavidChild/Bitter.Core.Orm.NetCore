using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using BT.Manage.Tools.Utils;

namespace BT.Manage.Core
{
    public  class ProxyUtils
    {
        /// <summary>
        /// 获取属性的变更名称,
        /// 此处只检测调用了Set方法的属性,不会检测值是否真的有变
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal static HashSet<string> GetModifiedProperties(object obj)
        {
            FieldInfo fieldInfo = obj.GetType().GetField(ProxyDefineConst.ModifiedPropertyNamesFieldName);
            if (fieldInfo == null) return null;
            object value = fieldInfo.GetValue(obj);
            return value as HashSet<string>;
        }

       
        internal static void SetOrg(object obj,object org)
        {
            FieldInfo fieldInfo = obj.GetType().GetField(ProxyDefineConst.OrgmodelFiledName);
            fieldInfo.SetValue(obj,org);
            
        }

        internal static void ClearHashSet(object obj)
        {
            FieldInfo fieldInfo = obj.GetType().GetField(ProxyDefineConst.ModifiedPropertyNamesFieldName);
            if (fieldInfo == null) return ;
            object value = fieldInfo.GetValue(obj);
            (value as HashSet<string>).Clear();

        }

        /// <summary>
        /// 获取两个对象间的值发生变化的描述
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="obj1">变化前的对象</param>
        /// <param name="obj2">变化后的对象</param>
        /// <param name="isDes">是否过滤掉没有[Description]标记的</param>
        /// <returns>字符串</returns>
        internal static List<ChangedInfo> GetChangeInfos<T>(T obj1, T obj2, HashSet<string> propertyChangedLis) where T : new()
        {
            string res = string.Empty;
            List<ChangedInfo> lis = new List<ChangedInfo>();
            if (obj1 == null || obj2 == null)
            {
                return new List<ChangedInfo>();
            }
            if (propertyChangedLis == null || propertyChangedLis.Count() <= 0)
            {
                return new List<ChangedInfo>();
            }
            var properties =
                from property in (typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public))
                select property;

            //string objVal1 = string.Empty;
            //string objVal2 = string.Empty;

             
            foreach (var propertyName in propertyChangedLis)
            {

             var property = properties.Where(p=>p.Name== propertyName).FirstOrDefault();
               

             var   objVal1 = property.GetValue(obj1, null) == null ? string.Empty : property.GetValue(obj1, null);
               var objVal2 = property.GetValue(obj2, null) == null ? string.Empty : property.GetValue(obj2, null);
                var keyFiledName = BT.Manage.Core.Utils.GetKeyFiledName(typeof(T));
                var tableName = BT.Manage.Core.Utils.GetTableName(typeof(T));
                var keyVlaue = BT.Manage.Core.Utils.GetObjectPropertyValue(obj1,keyFiledName).ToSafeString("");
                string des = string.Empty;
                //DescriptionAttribute descriptionAttribute = ((DescriptionAttribute)Attribute.GetCustomAttribute(property, typeof(DescriptionAttribute)));
                //if (descriptionAttribute != null)
                //{
                //    des = ((DescriptionAttribute)Attribute.GetCustomAttribute(property, typeof(DescriptionAttribute))).Description;// 属性值
                //}
                //if (isDes && descriptionAttribute == null)
                //{
                //    continue;
                //}
                 if (objVal1 == null && objVal2 == null) continue;
                if ((objVal1 == null && objVal2 != null) || (objVal1 != null && objVal2 == null))
                {
                    var dispalyname = "";
                    object[] attrs = property.GetCustomAttributes(true);

                    var f = (from p in attrs where p.GetType().Name == "DisplayAttribute" select p).FirstOrDefault();
                    if (f != null)
                    {
                        DisplayAttribute displayAttr = f as System.ComponentModel.DataAnnotations.DisplayAttribute;
                        dispalyname = displayAttr.Name;
                    }
                    lis.Add(new ChangedInfo() { FFiledName = property.Name, FNewValue = objVal2.ToSafeString(), FOldValue = objVal1.ToSafeString(), FOrgType = property.GetType().ToString(), FFiledDes = dispalyname, FTableName = tableName, FKeyFiledName = keyFiledName, FKeyValue = keyVlaue });
                    continue;
                }
               
                if (!(objVal1.Equals( objVal2)))
                {
                    var dispalyname = "";
                    object[] attrs = property.GetCustomAttributes(true);
                     
                    var f = (from p in attrs where p.GetType().Name == "DisplayAttribute" select p).FirstOrDefault();
                    if (f != null)
                    {
                        DisplayAttribute displayAttr = f as System.ComponentModel.DataAnnotations.DisplayAttribute;
                        dispalyname = displayAttr.Name;
                    }
                    lis.Add(new ChangedInfo() { FFiledName = property.Name, FNewValue = objVal2.ToSafeString() , FOldValue = objVal1.ToSafeString(), FOrgType = property.GetType().ToString(), FFiledDes = dispalyname,FTableName=tableName,FKeyFiledName=keyFiledName, FKeyValue = keyVlaue});
                }
               

            }
            return lis;

        }


    }

   

}
