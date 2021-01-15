using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace BT.Manage.Core
{
    public class ParserUtils
    {
        private static readonly Type _compilerGeneratedAttribute = typeof (CompilerGeneratedAttribute);
        private static readonly object _tableLocker = new object();
        private static int _tableNum;

        public static string GenerateAlias(string name)
        {
            var obj2 = _tableLocker;
            lock (obj2)
            {
                _tableNum++;
                return name + _tableNum;
            }
        }

        public static bool IsAnonymousType(Type type)
        {
            return (type.GetCustomAttributes(_compilerGeneratedAttribute, false).Count() > 0) &
                   type.FullName.Contains("AnonymousType");
        }
    }
}