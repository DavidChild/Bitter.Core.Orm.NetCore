using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BT.Manage.Core
{
    public class BuilderFactory
    {
        private static List<Type> _providerTypes;
        private readonly ProviderBase _provider;

        public BuilderFactory(ProviderBase provider)
        {
            _provider = provider;
        }

        public SqlBuilderBase CreateSqlBuilder()
        {
            if (_providerTypes == null)
            {
                _providerTypes = (from x in Assembly.GetExecutingAssembly().GetTypes()
                    where x.BaseType == typeof (SqlBuilderBase)
                    select x).ToList();
            }
            var typeName = _provider.DatabaseType + ".SqlBuilder";
            var source = from x in _providerTypes
                where x.FullName.EndsWith(typeName)
                select x;
            if (source.Count() > 1)
            {
                throw new NotSupportedException("找到了多个包含" + typeName + "的提供者类");
            }
            var type = source.FirstOrDefault();
            if (type == null)
            {
                throw new NotSupportedException("未找到提供者类：" + typeName);
            }
            object[] parameters = {_provider};
            return (SqlBuilderBase) ExpressionReflector.CreateInstance(type, ObjectPropertyConvertType.Cast, parameters);
        }
    }
}