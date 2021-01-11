using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace BT.Manage.Core
{
    public class EntityMapper<TEntity>
    {
        private static readonly Dictionary<Type, Dictionary<string, Func<TEntity, object>>> _entityGetters;
        private static readonly Dictionary<Type, Dictionary<string, Action<TEntity, object>>> _entitySetters;
        private static Dictionary<Type, Dictionary<string, Func<object, object>>> _objectGetters;
        private static Dictionary<Type, Dictionary<string, Action<object, object>>> _objectSetters;
        private static readonly Type _objectType;
        private readonly Type _entityType;

        static EntityMapper()
        {
            _entitySetters = new Dictionary<Type, Dictionary<string, Action<TEntity, object>>>();
            _entityGetters = new Dictionary<Type, Dictionary<string, Func<TEntity, object>>>();
            _objectGetters = new Dictionary<Type, Dictionary<string, Func<object, object>>>();
            _objectSetters = new Dictionary<Type, Dictionary<string, Action<object, object>>>();
            _objectType = typeof (object);
        }

        public EntityMapper()
        {
            _entityType = typeof (TEntity);
            Properties = GetProperties(_entityType);
        }

        public PropertyInfo[] Properties { get; private set; }

        private Dictionary<string, Func<TEntity, object>> GetGetters()
        {
            if (!_entityGetters.ContainsKey(_entityType))
            {
                var dictionary = new Dictionary<string, Func<TEntity, object>>();
                foreach (var info in Properties)
                {
                    var expression = Expression.Parameter(_entityType);
                    ParameterExpression[] parameters = {expression};
                    var func =
                        Expression.Lambda<Func<TEntity, object>>(
                            Expression.Convert(Expression.Property(expression, info), _objectType), parameters)
                            .Compile();
                    dictionary.Add(info.Name, func);
                }
                _entityGetters.Add(_entityType, dictionary);
            }
            return _entityGetters[_entityType];
        }

        private static PropertyInfo[] GetProperties(Type type)
        {
            return
                type.GetProperties(BindingFlags.SetProperty | BindingFlags.GetProperty | BindingFlags.CreateInstance |
                                   BindingFlags.Public | BindingFlags.Instance);
        }

        public Dictionary<string, object> GetPropertyValues(TEntity entity)
        {
            var dictionary = new Dictionary<string, object>();
            foreach (var pair in GetGetters())
            {
                dictionary.Add(pair.Key, pair.Value(entity));
            }
            return dictionary;
        }

        private Dictionary<string, Action<TEntity, object>> GetSetters()
        {
            if (_entitySetters.ContainsKey(_entityType))
            {
                return _entitySetters[_entityType];
            }
            var dictionary = new Dictionary<string, Action<TEntity, object>>();
            foreach (var info in Properties)
            {
                var instance = Expression.Parameter(_entityType);
                var expression = Expression.Parameter(_objectType);
                var expression3 = Expression.Convert(expression, info.PropertyType);
                var setMethod = info.GetSetMethod();
                if (setMethod != null)
                {
                    Expression[] arguments = {expression3};
                    ParameterExpression[] parameters = {instance, expression};
                    var action =
                        Expression.Lambda<Action<TEntity, object>>(Expression.Call(instance, setMethod, arguments),
                            parameters).Compile();
                    dictionary.Add(info.Name, action);
                }
            }
            var dictionary2 = _entitySetters;
            lock (dictionary2)
            {
                _entitySetters.Add(_entityType, dictionary);
            }
            return dictionary;
        }

        public object GetValue(TEntity entity, string propertyName)
        {
            return GetGetters()[propertyName](entity);
        }


        public TEntity Map(DataRow row)
        {
            var local = Activator.CreateInstance<TEntity>();
            var setters = GetSetters();
            foreach (var str in setters.Keys)
            {
                var obj2 = row[str];
                if (obj2 != DBNull.Value)
                {
                    setters[str](local, obj2);
                }
            }
            return local;
        }

        public IList<TEntity> Map(DataSet ds)
        {
            var list = new List<TEntity>();
            if ((ds.Tables.Count > 0) && (ds.Tables[0].Rows.Count > 0))
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(Map(row));
                }
            }
            return list;
        }

        public Dictionary<TKey, TEntity> Map<TKey>(DataSet ds, Func<TEntity, TKey> keySelector)
        {
            var dictionary = new Dictionary<TKey, TEntity>();
            var setters = GetSetters();
            var table1 = ds.Tables[0];
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                var local = Activator.CreateInstance<TEntity>();
                foreach (var str in setters.Keys)
                {
                    var obj2 = row[str];
                    if (obj2 != DBNull.Value)
                    {
                        setters[str](local, obj2);
                    }
                }
                dictionary.Add(keySelector(local), local);
            }
            return dictionary;
        }

        public void SetValue(TEntity entity, string propertyName, object value)
        {
            GetSetters()[propertyName](entity, value);
        }
    }
}