using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace BT.Manage.Core
{
    public class EntityMapper
    {
        public static Dictionary<string, object> GetPropertyValues(object entity)
        {
            var dictionary = new Dictionary<string, object>();
            foreach (var pair in ExpressionReflector.GetGetters(entity.GetType()))
            {
                dictionary.Add(pair.Key, pair.Value(entity));
            }
            return dictionary;
        }

        public static Dictionary<string, object> GetPropertyValues<TEntity>(TEntity entity)
        {
            var dictionary = new Dictionary<string, object>();
            foreach (var pair in ExpressionReflector.GetGetters(typeof (TEntity)))
            {
                dictionary.Add(pair.Key, pair.Value(entity));
            }
            return dictionary;
        }

        public static List<TObject> Map<TObject>(DataSet ds) where TObject : class, new()
        {
            var list = new List<TObject>();
            var entityType = typeof (TObject);
            var setters = ExpressionReflector.GetSetters(entityType);
            var properties = ExpressionReflector.GetProperties(entityType);
            var table1 = ds.Tables[0];
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                var local = Activator.CreateInstance<TObject>();
                foreach (var str in setters.Keys)
                {
                    var obj2 = row[str];
                    if (obj2 != DBNull.Value)
                    {
                        var propertyType = properties[str].PropertyType;
                        var underlyingType = Nullable.GetUnderlyingType(propertyType);
                        if (underlyingType == null)
                        {
                            underlyingType = propertyType;
                        }
                        if (underlyingType.IsEnum)
                        {
                            obj2 = Enum.Parse(underlyingType, Convert.ToString(obj2));
                        }
                        else
                        {
                            obj2 = Convert.ChangeType(obj2, underlyingType);
                        }
                        setters[str](local, obj2);
                    }
                }
                list.Add(local);
            }
            return list;
        }

        public static TTarget Map<TSource, TTarget>(TSource source)
        {
            var target = (TTarget) ExpressionReflector.CreateInstance(typeof (TTarget), ObjectPropertyConvertType.Cast);
            Map(source, target);
            return target;
        }

        public static IList Map(Type type, IDataReader reader)
        {
            return ExpressionReflector.GetDataReaderMapeer(type, reader)(reader);
        }


        public static void Map<TSource, TTarget>(TSource source, TTarget target)
        {
            var entityType = typeof (TTarget);
            var getters = ExpressionReflector.GetGetters(typeof (TSource));
            var setters = ExpressionReflector.GetSetters(entityType);
            foreach (var str in getters.Keys)
            {
                if (setters.ContainsKey(str))
                {
                    setters[str](target, getters[str](source));
                }
            }
        }

        public static IList Map(Type objectType, DataSet ds, ObjectPropertyConvertType convertType)
        {
            var list = (IList) Activator.CreateInstance(ReflectorConsts.ListObjectType);
            var setters = ExpressionReflector.GetSetters(objectType);
            var properties = ExpressionReflector.GetProperties(objectType);
            var table1 = ds.Tables[0];
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                object obj2 = null;
                if (objectType.IsSealed)
                {
                    var list2 = new List<object>();
                    foreach (DataColumn column in ds.Tables[0].Columns)
                    {
                        var item = row[column];
                        if (item == DBNull.Value)
                        {
                            item = null;
                        }
                        list2.Add(item);
                    }
                    if (TypeHelper.IsValueType(objectType))
                    {
                        obj2 = list2[0];
                    }
                    else
                    {
                        obj2 = ExpressionReflector.CreateInstance(objectType, convertType, list2.ToArray());
                    }
                }
                else
                {
                    obj2 = ExpressionReflector.CreateInstance(objectType, convertType);
                    foreach (DataColumn column2 in ds.Tables[0].Columns)
                    {
                        var obj4 = row[column2];
                        if (obj4 != DBNull.Value)
                        {
                            var info = properties[column2.ColumnName];
                            if (info != null)
                            {
                                var propertyType = info.PropertyType;
                                var underlyingType = Nullable.GetUnderlyingType(propertyType);
                                if (underlyingType == null)
                                {
                                    underlyingType = propertyType;
                                }
                                if (underlyingType.IsEnum)
                                {
                                    obj4 = Enum.Parse(underlyingType, Convert.ToString(obj4));
                                }
                                else
                                {
                                    obj4 = Convert.ChangeType(obj4, underlyingType);
                                }
                                setters[column2.ColumnName](obj2, obj4);
                            }
                        }
                    }
                }
                list.Add(obj2);
            }
            return list;
        }


        public static IList Map(Type objectType, DataTable dt, ObjectPropertyConvertType convertType)
        {
            var list = (IList) Activator.CreateInstance(ReflectorConsts.ListObjectType);
            var setters = ExpressionReflector.GetSetters(objectType);
            var properties = ExpressionReflector.GetProperties(objectType);
            var table1 = dt;
            foreach (DataRow row in dt.Rows)
            {
                object obj2 = null;
                if (objectType.IsSealed)
                {
                    var list2 = new List<object>();
                    foreach (DataColumn column in dt.Columns)
                    {
                        var item = row[column];
                        if (item == DBNull.Value)
                        {
                            item = null;
                        }
                        list2.Add(item);
                    }
                    if (TypeHelper.IsValueType(objectType))
                    {
                        obj2 = list2[0];
                    }
                    else
                    {
                        obj2 = ExpressionReflector.CreateInstance(objectType, convertType, list2.ToArray());
                    }
                }
                else
                {
                    obj2 = ExpressionReflector.CreateInstance(objectType, convertType);
                    foreach (DataColumn column2 in dt.Columns)
                    {
                        var obj4 = row[column2];
                        if (obj4 != DBNull.Value)
                        {
                            var info = properties[column2.ColumnName];
                            if (info != null)
                            {
                                var propertyType = info.PropertyType;
                                var underlyingType = Nullable.GetUnderlyingType(propertyType);
                                if (underlyingType == null)
                                {
                                    underlyingType = propertyType;
                                }
                                if (underlyingType.IsEnum)
                                {
                                    obj4 = Enum.Parse(underlyingType, Convert.ToString(obj4));
                                }
                                else
                                {
                                    obj4 = Convert.ChangeType(obj4, underlyingType);
                                }
                                setters[column2.ColumnName](obj2, obj4);
                            }
                        }
                    }
                }
                list.Add(obj2);
            }
            return list;
        }


        public static IList Map(Type objectType, IDataReader dataReader, ObjectPropertyConvertType convertType)
        {
            if (!TypeHelper.IsValueType(objectType))
            {
                return Map(objectType, dataReader);
            }
            Type[] typeArguments = {objectType};
            var list = (IList) Activator.CreateInstance(ReflectorConsts.ListType.MakeGenericType(typeArguments));
            while (dataReader.Read())
            {
                if (!dataReader.IsDBNull(0))
                {
                    list.Add(dataReader.GetValue(0));
                }
            }
            return list;
        }
    }
}