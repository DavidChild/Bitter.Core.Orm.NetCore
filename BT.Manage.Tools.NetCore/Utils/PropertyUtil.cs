using BT.Manage.Tools.NetCore.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BT.Manage.Tools.NetCore.Utils
{
    public static class PropertyUtil
    {
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, List<PropertyInfo>> _propertyCache = new ConcurrentDictionary<RuntimeTypeHandle, List<PropertyInfo>>();

        private static readonly ConcurrentDictionary<RuntimeTypeHandle, List<PropertyInfo>> _propertyWithIgnoreAttributeCache = new ConcurrentDictionary<RuntimeTypeHandle, List<PropertyInfo>>();

        #region 获取实例的属性列表

        /// <summary>
        /// 获取实例的属性列表
        /// </summary>
        /// <param name="obj">实例信息</param>
        /// <param name="ignoreProperties">忽略的属性名字</param>
        /// <returns>实例的属性列表</returns>
        public static List<PropertyInfo> GetPropertyInfos(object obj, string[] ignoreProperties)
        {
            List<PropertyInfo> proList = GetPropertyInfos(obj)
                                                    .Where(m =>
                                                    {
                                                        if (ignoreProperties == null) return true;
                                                        if (ignoreProperties.Contains(m.Name)) return false;
                                                        return true;
                                                    }).ToList();
            return proList;
        }

        /// <summary>
        /// 获取实例的属性列表
        /// </summary>
        /// <param name="obj">实例信息</param>
        /// <param name="ignoreProperties">忽略的属性名字</param>
        /// <param name="ignoreAttributes">需要忽略的标记列表</param>
        /// <returns>实例的属性列表</returns>
        public static List<PropertyInfo> GetPropertyInfos(object obj, string[] ignoreProperties, Type[] ignoreAttributes)
        {
            List<PropertyInfo> proList = GetPropertyInfos(obj, ignoreAttributes)
                                                    .Where(m =>
                                                    {
                                                        if (ignoreProperties == null) return true;
                                                        if (ignoreProperties.Contains(m.Name)) return false;
                                                        return true;
                                                    }).ToList();
            return proList;
        }

        /// <summary>
        /// 获取实例的属性列表
        /// </summary>
        /// <param name="obj">实例信息</param>
        /// <returns>实例的属性列表</returns>
        public static List<PropertyInfo> GetPropertyInfos(object obj)
        {
            if (obj == null)
            {
                return new List<PropertyInfo>();
            }
            return GetTypeProperties(obj.GetType());
        }

        /// <summary>
        /// 获取实例的属性列表
        /// </summary>
        /// <param name="obj">实例信息</param>
        /// <param name="ignoreAttributes">需要忽略的标记列表</param>
        /// <returns>实例的属性列表</returns>
        public static List<PropertyInfo> GetPropertyInfos(object obj, Type[] ignoreAttributes)
        {
            if (obj == null)
            {
                return new List<PropertyInfo>();
            }
            return GetTypeProperties(obj.GetType(), ignoreAttributes);
        }

        /// <summary>
        /// 根据类型获取类型的属性信息
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="ignoreProperties">要忽略的属性</param>
        /// <returns>类型的属性信息</returns>
        public static List<PropertyInfo> GetTypeProperties(Type type, string[] ignoreProperties)
        {
            return GetTypeProperties(type).Where(m =>
            {
                if (ignoreProperties == null) return true;
                if (ignoreProperties.Contains(m.Name)) return false;
                return true;
            }).ToList();
        }

        /// <summary>
        /// 根据类型获取类型的属性信息
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="ignoreProperties">要忽略的属性</param>
        /// <param name="ignoreAttributes">需要忽略的标记列表</param>
        /// <returns>类型的属性信息</returns>
        public static List<PropertyInfo> GetTypeProperties(Type type, string[] ignoreProperties, Type[] ignoreAttributes)
        {
            return GetTypeProperties(type, ignoreAttributes).Where(m =>
            {
                if (ignoreProperties == null) return true;
                if (ignoreProperties.Contains(m.Name)) return false;
                return true;
            }).ToList();
        }

        /// <summary>
        /// 根据类型获取类型的属性信息
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>类型的属性信息</returns>
        public static List<PropertyInfo> GetTypeProperties(Type type)
        {
            if (type == null) return new List<PropertyInfo>();
            var typeHandle = type.TypeHandle;
            return _propertyCache.GetValue(typeHandle, () =>
            {
                return type.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
            });
        }

        /// <summary>
        /// 根据类型获取类型的属性信息
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="ignoreAttributes">需要忽略的标记列表</param>
        /// <returns>类型的属性信息</returns>
        public static List<PropertyInfo> GetTypeProperties(Type type, Type[] ignoreAttributes = null)
        {
            if (type == null) return new List<PropertyInfo>();
            var typeHandle = type.TypeHandle;
            return _propertyWithIgnoreAttributeCache.GetValue(typeHandle, () =>
            {
                var allPropertyList = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
                var propertyList = new List<PropertyInfo>();
                bool isCanAdd = true;
                if (ignoreAttributes != null && ignoreAttributes.Any())
                {
                    foreach (var propertyInfo in allPropertyList)
                    {
                        foreach (var ignoreAttribute in ignoreAttributes)
                        {
                            if (isCanAdd)
                            {
                                var ignore = propertyInfo.GetCustomAttribute(ignoreAttribute, true);
                                if (ignore != null)
                                {
                                    isCanAdd = false;
                                }
                            }
                        }
                        if (isCanAdd)
                        {
                            propertyList.Add(propertyInfo);
                        }
                        isCanAdd = true;
                    }
                }
                else
                {
                    return allPropertyList;
                }
                return propertyList;
            });
        }

        #endregion 获取实例的属性列表

        /// <summary>
        /// 获取sqlparametr参数值
        /// </summary>
        /// <param name="value">实际值</param>
        /// <param name="typeName">类型名字</param>
        /// <param name="isNullable">是否为可空类型</param>
        /// <returns>sqlparametr参数值</returns>
        private static object GetParameterValue(object value, string typeName, bool isNullable)
        {
            object parameterValue = null;
            if (value == null)
            {
                if (isNullable || string.Equals("string", typeName, StringComparison.OrdinalIgnoreCase))
                {
                    parameterValue = DBNull.Value;
                }
            }
            if (parameterValue == null)
            {
                parameterValue = value;
            }
            return parameterValue;
        }
    }
}

