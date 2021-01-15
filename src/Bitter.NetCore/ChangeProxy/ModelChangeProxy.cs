using Bitter.Core;
using Bitter.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bitter.Core
{
    public static class ModelProxys
    {
        /// <summary>
        /// 创建属性变更监控跟踪器并且托管当前model
        /// </summary>
        public static T CreateModelProxy<T>(this T o) where T:class,new()
        {
            var k=   ModelProxy.CreateDynamicProxy<T>();
            T tk = new T();
            o.MapTo(tk);
            o.MapTo(k);
            o = null;
            ProxyUtils.ClearHashSet(k);
            ProxyUtils.SetOrg(k, tk);
            o = k;
            return o;

        }

        /// <summary>
        /// 获取属性的变更名称,
        /// 此处只检测调用了Set方法的属性,不会检测值是否真的有变
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<ChangedInfo> GetChangedInfo<T>(this T o) where T : class, new()
        {
            FieldInfo fieldInfo = o.GetType().GetField(ProxyDefineConst.OrgmodelFiledName);
            T value = (T)fieldInfo.GetValue(o);
            HashSet<string> hs = ProxyUtils.GetModifiedProperties(o);
            return ProxyUtils.GetChangeInfos(value, o, hs);
        }

    }
}
