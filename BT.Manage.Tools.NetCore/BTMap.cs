using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using System.Dynamic;
using System.Reflection;

namespace BT.Manage.Tools
{
    public class BTMap
    {
        #region 动态类型映射
        public static void DynamicMapToUpdate(dynamic source, object destination)
        {
            var config = new MapperConfiguration(cfg => { });
            config.AssertConfigurationIsValid();
            var map = config.CreateMapper();
            map.Map(source, destination);
            Type t = source.GetType();

            Type dt = destination.GetType();
            if (t == typeof(Dictionary<string, object>))
            {
                var dicsource = (Dictionary<string, object>)source;
                foreach (var item in dicsource)
                {
                    if (item.Value == null || string.IsNullOrEmpty(item.Value.ToString()))
                    {
                        //获取目标对象的对应属性
                        var dpro = dt.GetProperties(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(o => o.Name == item.Key);
                        if (dpro != null)
                        {
                            if (dpro.PropertyType.IsGenericType && dpro.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                            {
                                //更新值
                                dpro.SetValue(destination, null);
                            }
                            else if (dpro.PropertyType.IsClass)
                            {
                                //更新值
                                dpro.SetValue(destination, null);
                            }
                        }
                    }
                }
            }
            if (t == typeof(ExpandoObject))
            {
                foreach (var item in (IDictionary<string, object>)source)
                {
                    if (item.Value == null || string.IsNullOrEmpty(item.Value.ToString()))
                    {
                        //获取目标对象的对应属性
                        var dpro = dt.GetProperties(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(o => o.Name == item.Key);
                        if (dpro != null)
                        {
                            if (dpro.PropertyType.IsGenericType && dpro.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                            {
                                //更新值
                                dpro.SetValue(destination, null);
                            }
                            else if (dpro.PropertyType.IsClass)
                            {
                                //更新值
                                dpro.SetValue(destination, null);
                            }
                        }
                    }
                }
            }
        }
        public static void DynamicMapToUpdate(dynamic source, object destination, Action<IMapperConfigurationExpression> actionMapperConfigurationExpression)
        {

            var config = new MapperConfiguration(cfg =>
            {
                actionMapperConfigurationExpression(cfg);

            });
            config.AssertConfigurationIsValid();
            var map = config.CreateMapper();

            map.Map(source, destination);

            Type t = source.GetType();

            Type dt = destination.GetType();
            if (t == typeof(Dictionary<string, object>))
            {
                var dicsource = (Dictionary<string, object>)source;
                foreach (var item in dicsource)
                {
                    if (item.Value == null || string.IsNullOrEmpty(item.Value.ToString()))
                    {
                        //获取目标对象的对应属性
                        var dpro = dt.GetProperties(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(o => o.Name == item.Key);
                        if (dpro != null)
                        {
                            if (dpro.PropertyType.IsGenericType && dpro.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                            {
                                //更新值
                                dpro.SetValue(destination, null);
                            }
                            else if (dpro.PropertyType.IsClass)
                            {
                                //更新值
                                dpro.SetValue(destination, null);
                            }
                        }
                    }
                }
            }
            if (t == typeof(ExpandoObject))
            {
                foreach (var item in (IDictionary<string, object>)source)
                {
                    if (item.Value == null||string.IsNullOrEmpty(item.Value.ToString()))
                    {
                        //获取目标对象的对应属性
                        var dpro = dt.GetProperties(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(o => o.Name == item.Key);
                        if (dpro != null)
                        {
                            if (dpro.PropertyType.IsGenericType && dpro.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                            {
                                //更新值
                                dpro.SetValue(destination, null);
                            }
                            else if (dpro.PropertyType.IsClass)
                            {
                                //更新值
                                dpro.SetValue(destination, null);
                            }
                        }
                    }
                }
            }

        }
        public static TDestination DynamicMapToNew<TDestination>(dynamic source)
            where TDestination : class
        {
            var config = new MapperConfiguration(cfg =>
            {

            });
            var map = config.CreateMapper();
            return map.Map<TDestination>(source);
        }
        public static TDestination DynamicMapToNew<TDestination>(dynamic source, Action<IMapperConfigurationExpression> actionMapperConfigurationExpression)
            where TDestination : class
        {
            var config = new MapperConfiguration(cfg =>
            {
                actionMapperConfigurationExpression(cfg);
            });
            config.AssertConfigurationIsValid();
            var map = config.CreateMapper();
            return map.Map<TDestination>(source);
        }
        #endregion


        /// <summary>
        /// 使用默认配置规则
        /// </summary>
        /// <returns></returns>
        internal static IMapper InstanceMap(Type sourceType, Type destinationType)
        {
            BTMap bt = new BTMap();
            return bt.GetMap(sourceType, destinationType);
        }


        /// <summary>
        ///  绑定规则
        /// </summary>
        /// <param name="actionMapperConfigurationExpression"></param>
        /// <returns></returns>
        internal static IMapper InstanceMap(Type sourceType, Type destinationType, Action<IMapperConfigurationExpression> actionMapperConfigurationExpression,int type=1)
        {
            BTMap bt = new BTMap();
            return bt.GetMap(sourceType, destinationType, actionMapperConfigurationExpression,type);
        }

        private IMapper GetMap(Type sourceType, Type destinationType, Action<IMapperConfigurationExpression> ActionMapperConfigurationExpression = null,int type=1)
        {

            MapperConfiguration cgf = MapperConfiguration(sourceType, destinationType, ActionMapperConfigurationExpression,type);

            return cgf.CreateMapper();
        }



        /// <summary>
        /// 获取一个MapperConfiguration 
        /// </summary>
        /// <param name="ActionMapperConfigurationExpression"></param>
        /// <returns></returns>
        private static MapperConfiguration MapperConfiguration(Type sourceType, Type destinationType, Action<IMapperConfigurationExpression> ActionMapperConfigurationExpression = null,int type=1)
        {
            MapperConfiguration config = null;
            if (type == 1)
            {
                if (ActionMapperConfigurationExpression != null)
                {
                    config = new MapperConfiguration(cfg =>
                    {
                        ActionMapperConfigurationExpression(cfg);
                        cfg.CreateMap(sourceType, destinationType);

                    });
                }
                else
                {
                    config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap(sourceType, destinationType);
                    });
                }
            }
            else
            {
                config = new MapperConfiguration(cfg =>
                {
                    ActionMapperConfigurationExpression(cfg);
                });
            }
            return config;

        }

    }
}
