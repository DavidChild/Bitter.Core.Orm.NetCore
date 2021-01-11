using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BT.Manage.DataAccess;

namespace BT.Manage.Core
{
    public class ProviderFactory
    {
        private static Dictionary<string, Type> _providerTypes;
        private static Type _type;

        internal static ProviderBase CreateProvider()
        {
            return CreateProvider(dc.dbconn(string.Empty).Reader.DatabaseType);
        }

        public static ProviderBase CreateProvider(DatabaseType databaseType)
        {
            if (_type == null)
            {
                if (_providerTypes == null)
                {
                    _providerTypes = (from x in Assembly.GetExecutingAssembly().GetTypes()
                        where x.BaseType == typeof (ProviderBase)
                        select x).ToDictionary(x => x.Name);
                }
                // BT.Manage.Core.Provider.MSSQLServer.MSSQLServerProvider
                var typeName = string.Format("Provider.{0}.{0}Provider", databaseType);
                var source = from x in _providerTypes.Values
                    where x.FullName.EndsWith(typeName)
                    select x;
                if (source.Count() > 1)
                {
                    throw new NotSupportedException("找到了多个包含" + typeName + "的提供者类");
                }
                if (source.Count() > 0)
                {
                    _type = source.FirstOrDefault();
                }
               
                if (_type == null)
                {
                    throw new NotSupportedException("未找到提供者类：" + typeName);
                }
            }
            return (ProviderBase) ObjectCacheLocked.GetObjectFromCallContext(_type);
        }
    }
}