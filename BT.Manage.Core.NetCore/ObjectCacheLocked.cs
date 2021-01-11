using System;
using System.Runtime.Remoting.Messaging;

namespace BT.Manage.Core
{
    public class ObjectCacheLocked
    {
        // Methods
        public static object GetObjectFromCallContext(Type type)
        {
            return GetObjectFromCallContext(type, x => Activator.CreateInstance(x));
        }

        public static object GetObjectFromCallContext(Type type, Func<Type, object> createInstance)
        {
            var data = CallContext.GetData(type.FullName);
            if (data == null)
            {
                data = createInstance(type);
                CallContext.SetData(type.FullName, data);
            }
            return data;
        }
    }
}