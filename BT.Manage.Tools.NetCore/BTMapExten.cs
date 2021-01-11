using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BT.Manage.Tools
{
    public static class BTMapExten
    {

        #region 类型映射扩展方法

        public static void MapTo<TSource>(this TSource value, object desion)
            where TSource : class
        {
            var dtype = desion.GetType();
            var stype = value.GetType();
            IMapper map;
            if(stype.IsGenericType && stype.Name == "List`1")
                map = BTMap.InstanceMap(stype.GetGenericArguments()[0],dtype.GetGenericArguments()[0]);
            else
                map= BTMap.InstanceMap(stype, dtype);
            map.Map(value,desion);
        }
        

        public static TDesion MapTo<TDesion>(this object value)
          where TDesion : class
        {
            var dtype = typeof(TDesion);
            var stype = value.GetType();
            IMapper map;
            if (stype.IsGenericType && stype.Name == "List`1")
                map = BTMap.InstanceMap(stype.GetGenericArguments()[0], dtype.GetGenericArguments()[0]);
            else
                map = BTMap.InstanceMap(stype, dtype);
            return map.Map<TDesion>(value);
        }
        /// <summary>
        /// 将 DataTable 转为实体对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static T GetEntity<T>(this DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
                return  default(T);
            var dr = dt.CreateDataReader();
            return Mapper.Map<T>(dr);
           
        }

        public static TDesion MapTo<TSource, TDesion>(this TSource value)
           where TSource : class
           where TDesion : class
        {
            var dtype = typeof(TDesion);
            var stype = value.GetType();
            IMapper map;
            if (stype.IsGenericType&&stype.Name == "List`1")
                map = BTMap.InstanceMap(stype.GetGenericArguments()[0], dtype.GetGenericArguments()[0]);
            else
                map = BTMap.InstanceMap(stype, dtype);
            return map.Map<TDesion>(value);
        }
        public static void MapTo<TSource>(this TSource value, object desion, Action<IMapperConfigurationExpression> actionMapperConfigurationExpression,int type=1)
            where TSource : class
        {
            var dtype = desion.GetType();
            var stype = value.GetType();
            IMapper map;
            if (stype.IsGenericType&&stype.Name== "List`1")
                map = BTMap.InstanceMap(stype.GetGenericArguments()[0], dtype.GetGenericArguments()[0], actionMapperConfigurationExpression, type);
            else
                map = BTMap.InstanceMap(stype, dtype, actionMapperConfigurationExpression, type);
            map.Map(value, desion);
        }

        public static TDesion MapTo<TSource, TDesion>(this TSource value, Action<IMapperConfigurationExpression> actionMapperConfigurationExpression,int type=1)
           where TDesion : class
           where TSource : class
        {
            var dtype = typeof(TDesion);
            var stype = value.GetType();
            IMapper map;
            if (stype.IsGenericType && stype.Name == "List`1")
                map = BTMap.InstanceMap(stype.GetGenericArguments()[0], dtype.GetGenericArguments()[0], actionMapperConfigurationExpression, type);
            else
                map = BTMap.InstanceMap(stype, dtype, actionMapperConfigurationExpression, type);
            //var map = BTMap.InstanceMap(stype, dtype,actionMapperConfigurationExpression,type);
            return map.Map<TDesion>(value);
        }


        public static TDesion MapTo<TDesion>(this object value, Action<IMapperConfigurationExpression> actionMapperConfigurationExpression, int type = 1)
         where TDesion : class
        {
            var dtype = typeof(TDesion);
            var stype = value.GetType();
            IMapper map;
            if (stype.IsGenericType && stype.Name == "List`1")
                map = BTMap.InstanceMap(stype.GetGenericArguments()[0], dtype.GetGenericArguments()[0], actionMapperConfigurationExpression, type);
            else
                map = BTMap.InstanceMap(stype, dtype, actionMapperConfigurationExpression, type);
            //var map = BTMap.InstanceMap(stype, dtype, actionMapperConfigurationExpression,type);
            return map.Map<TDesion>(value);
        }
        #endregion

        
    }
}
