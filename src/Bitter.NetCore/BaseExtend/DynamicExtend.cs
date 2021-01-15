using BT.Manage.Tools.Utils;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.CompilerServices;

namespace BT.Manage.Core
{
    /********************************************************************************
    ** auth： Jason
    ** date： 2017/2/23 10:06:22
    ** desc：
    ** Ver.:  V1.0.0
    ** Copyright (C) 2016 备胎 版权所有。
    *********************************************************************************/

    public class DelegateObj
    {
        private PramDynamicDelegate _delegate;

        private DelegateObj(PramDynamicDelegate D)
        {
            _delegate = D;
        }

        public delegate object PramDynamicDelegate(dynamic Sender, params object[] PMs);

        public PramDynamicDelegate CallMethod
        {
            get { return _delegate; }
        }

        /// <summary>
        /// 构造委托对象，让它看起来有点javascript定义的味道.
        /// </summary>
        /// <param name="D"></param>
        /// <returns></returns>
        public static DelegateObj Function(PramDynamicDelegate D)
        {
            return new DelegateObj(D);
        }
    }

    /// <summary>
    /// 自定义动态类型
    /// </summary>
    public class PramDynamic : DynamicObject
    {
        //保存对象动态定义的属性值
        private Dictionary<string, object> _values;

        public PramDynamic()
        {
            _values = new Dictionary<string, object>();
        }

        /// <summary>
        /// 定义签名
        /// </summary>
        public string sign
        {
            get { return ""; }
        }

        /// <summary>
        /// 定义时间戳
        /// </summary>
        public long? time
        {
            get { return DateTime.Now.ToString().ToSafeDateTime().ToSafeDataLong(); }
        }

        public Dictionary<string, object> GetDynamic()
        {
            return _values;
        }

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public object GetPropertyValue(string propertyName)
        {
            if (_values.ContainsKey(propertyName) == true)
            {
                return _values[propertyName];
            }
            return null;
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public void SetPropertyValue(string propertyName, object value)
        {
            if (_values.ContainsKey(propertyName) == true)
            {
                _values[propertyName] = value;
            }
            else
            {
                _values.Add(propertyName, value);
            }
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public void SetPropertyValueForDto(object value, [CallerMemberName]  string propertyName = null)
        {
            propertyName = "Dto" + propertyName.Remove(0, 1);
            if (_values.ContainsKey(propertyName) == true)
            {
                _values[propertyName] = value;
            }
            else
            {
                _values.Add(propertyName, value);
            }
        }

        /// <summary>
        /// 实现动态对象属性成员访问的方法，得到返回指定属性的值
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = GetPropertyValue(binder.Name);
            return result == null ? false : true;
        }

        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            return base.TryInvoke(binder, args, out result);
        }

        /// <summary>
        /// 动态对象动态方法调用时执行的实际代码
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="args"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            var theDelegateObj = GetPropertyValue(binder.Name) as DelegateObj;
            if (theDelegateObj == null || theDelegateObj.CallMethod == null)
            {
                result = null;
                return false;
            }
            result = theDelegateObj.CallMethod(this, args);
            return true;
        }

        /// <summary>
        /// 实现动态对象属性值设置的方法。
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            SetPropertyValue(binder.Name, value);
            return true;
        }
    }
}