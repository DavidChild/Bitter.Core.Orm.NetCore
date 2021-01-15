using System;
using System.Collections.Generic;
using System.Data;

namespace BT.Manage.Core
{
    public class Mapper
    {
        public static TResult Map<TSource, TResult>(TSource source)
        {
            return EntityMapper.Map<TSource, TResult>(source);
        }


        public static IDictionary<TKey, TEntity> Map<TKey, TEntity>(DataSet ds, Func<TEntity, TKey> keySelector)
        {
            return new EntityMapper<TEntity>().Map(ds, keySelector);
        }


        public static IList<T> MapList<T>(DataSet ds)
        {
            return new EntityMapper<T>().Map(ds);
        }


        public static T MapSingle<T>(DataSet ds)
        {
            if ((ds.Tables.Count > 0) && (ds.Tables[0].Rows.Count > 0))
            {
                return new EntityMapper<T>().Map(ds.Tables[0].Rows[0]);
            }
            return default(T);
        }
    }
}