using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace BT.Manage.Core
{
    public class ExpressionReflector<TEntity>
    {
        private static readonly Dictionary<Type, Dictionary<string, Func<TEntity, object>>> _entityGetters;
        private static readonly Dictionary<Type, Dictionary<string, Action<TEntity, object>>> _entitySetters;
        private static Dictionary<Type, Dictionary<string, Func<object, object>>> _objectGetters;
        private static Dictionary<Type, Dictionary<string, Action<object, object>>> _objectSetters;
        private static readonly Type _objectType;
        private static Dictionary<Type, Func<IDataReader, object>> _reader2Objects;
        private readonly Type _entityType;

        static ExpressionReflector()
        {
            _reader2Objects = new Dictionary<Type, Func<IDataReader, object>>();
            _entitySetters = new Dictionary<Type, Dictionary<string, Action<TEntity, object>>>();
            _entityGetters = new Dictionary<Type, Dictionary<string, Func<TEntity, object>>>();
            _objectGetters = new Dictionary<Type, Dictionary<string, Func<object, object>>>();
            _objectSetters = new Dictionary<Type, Dictionary<string, Action<object, object>>>();
            _objectType = typeof (object);
        }

        public ExpressionReflector()
        {
            _entityType = typeof (TEntity);
            Properties = GetProperties(_entityType);
        }

        public PropertyInfo[] Properties { get; private set; }

        private Dictionary<string, Func<TEntity, object>> GetGetters()
        {
            Dictionary<string, Func<TEntity, object>> dictionary = null;
            if (!_entityGetters.TryGetValue(_entityType, out dictionary))
            {
                var dictionary2 = _entityGetters;
                lock (dictionary2)
                {
                    if (_entityGetters.TryGetValue(_entityType, out dictionary))
                    {
                        return dictionary;
                    }
                    dictionary = new Dictionary<string, Func<TEntity, object>>();
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
            }
            return dictionary;
        }

        private static PropertyInfo[] GetProperties(Type type)
        {
            return
                type.GetProperties(BindingFlags.SetProperty | BindingFlags.GetProperty | BindingFlags.CreateInstance |
                                   BindingFlags.Public | BindingFlags.Instance);
        }


        public Dictionary<string, Func<TEntity, object>> GetPropertyGetters()
        {
            return GetGetters();
        }


        public Dictionary<string, Action<TEntity, object>> GetPropertySetters()
        {
            return GetSetters();
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
            Dictionary<string, Action<TEntity, object>> dictionary = null;
            if (!_entitySetters.TryGetValue(_entityType, out dictionary))
            {
                var dictionary2 = _entitySetters;
                lock (dictionary2)
                {
                    if (_entitySetters.TryGetValue(_entityType, out dictionary))
                    {
                        return dictionary;
                    }
                    dictionary = new Dictionary<string, Action<TEntity, object>>();
                    foreach (var info in Properties)
                    {
                        var setMethod = info.GetSetMethod();
                        if (setMethod != null)
                        {
                            var instance = Expression.Parameter(_entityType);
                            var expression = Expression.Parameter(_objectType);
                            var expression3 = Expression.Convert(expression, info.PropertyType);
                            Expression[] arguments = {expression3};
                            ParameterExpression[] parameters = {instance, expression};
                            var action =
                                Expression.Lambda<Action<TEntity, object>>(
                                    Expression.Call(instance, setMethod, arguments), parameters).Compile();
                            dictionary.Add(info.Name, action);
                        }
                    }
                    _entitySetters.Add(_entityType, dictionary);
                }
            }
            return dictionary;
        }

        public object GetValue(TEntity entity, string propertyName)
        {
            Func<TEntity, object> func = null;
            if (!GetGetters().TryGetValue(propertyName, out func))
            {
                throw new Exception("Getter未初始化完整");
            }
            return func(entity);
        }

        public void SetValue(TEntity entity, string propertyName, object value)
        {
            Action<TEntity, object> action = null;
            if (!GetSetters().TryGetValue(propertyName, out action))
            {
                throw new Exception("Setter未初始化完整");
            }
            action(entity, value);
        }
    }
}