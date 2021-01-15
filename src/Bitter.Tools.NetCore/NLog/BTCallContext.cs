using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;

namespace BT.Manage.Tools
{
    public static class BTCallContext
    {
        static ConcurrentDictionary<string, AsyncLocal<object>> state = new ConcurrentDictionary<string, AsyncLocal<object>>();

        public static void SetData(string name, object data) =>
            state.GetOrAdd(name, _ => new AsyncLocal<object>()).Value = data;

        public static object GetData(string name) =>
            state.TryGetValue(name, out AsyncLocal<object> data) ? data.Value : null;

        public static bool Remove(string name)
        {
            return state.TryRemove(name,out AsyncLocal<object> data);
        }

        public static int Count()
        {
            return state.Keys.Count;
        }
    }
}
