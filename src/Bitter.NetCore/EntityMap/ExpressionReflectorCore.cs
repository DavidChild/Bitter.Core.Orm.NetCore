using System;
using System.Collections.Generic;
using System.Reflection;

namespace BT.Manage.Core
{
    internal class ExpressionReflectorCore
    {
        public static readonly Type ObjectType = typeof (object);

        static ExpressionReflectorCore()
        {
            _propertyInfos = new Dictionary<Type, IDictionary<string, PropertyInfo>>();
            EntityPropertyTypes = new HashSet<Type>();
            EntityPropertyTypes.Add(typeof (string));
            EntityPropertyTypes.Add(typeof (DateTime));
            EntityPropertyTypes.Add(typeof (DateTime?));
            EntityPropertyTypes.Add(typeof (int));
            EntityPropertyTypes.Add(typeof (short));
            EntityPropertyTypes.Add(typeof (long));
            EntityPropertyTypes.Add(typeof (int?));
            EntityPropertyTypes.Add(typeof (short?));
            EntityPropertyTypes.Add(typeof (long?));
            EntityPropertyTypes.Add(typeof (bool));
            EntityPropertyTypes.Add(typeof (bool?));
            EntityPropertyTypes.Add(typeof (decimal));
            EntityPropertyTypes.Add(typeof (decimal?));
            EntityPropertyTypes.Add(typeof (float?));
            EntityPropertyTypes.Add(typeof (float));
            EntityPropertyTypes.Add(typeof (double?));
            EntityPropertyTypes.Add(typeof (double));
            EntityPropertyTypes.Add(typeof (byte));
            EntityPropertyTypes.Add(typeof (byte?));
        }

        private static IDictionary<Type, IDictionary<string, PropertyInfo>> _propertyInfos { get; set; }

        public static HashSet<Type> EntityPropertyTypes { get; private set; }

        public static IDictionary<string, PropertyInfo> GetProperties(Type entityType)
        {
            IDictionary<string, PropertyInfo> dictionary = null;
            if (!_propertyInfos.TryGetValue(entityType, out dictionary))
            {
                var dictionary2 = _propertyInfos;
                lock (dictionary2)
                {
                    if (_propertyInfos.TryGetValue(entityType, out dictionary))
                    {
                        return dictionary;
                    }
                    dictionary = new Dictionary<string, PropertyInfo>();
                    foreach (
                        var info in
                            entityType.GetProperties(BindingFlags.SetProperty | BindingFlags.GetProperty |
                                                     BindingFlags.Public | BindingFlags.Instance |
                                                     BindingFlags.DeclaredOnly))
                    {
                        var propertyType = info.PropertyType;
                        if (EntityPropertyTypes.Contains(propertyType) || propertyType.IsEnum)
                        {
                            dictionary.Add(info.Name, info);
                        }
                    }
                    _propertyInfos.Add(entityType, dictionary);
                }
            }
            return dictionary;
        }
    }
}