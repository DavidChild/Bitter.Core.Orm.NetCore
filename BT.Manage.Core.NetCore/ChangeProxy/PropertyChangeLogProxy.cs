using System;
using System.Collections.Generic;
using System.Text;

namespace BT.Manage.Core.NetCore.ChangeProxy.changeV2
{
    ///// <summary>
    ///// 类属性变更日志记录器
    ///// </summary>
    //class PropertyChangeLogProxy<T> : ChannelAdam.DispatchProxies.RetryEnabledObjectDisposableDispatchProxy
    //    where T : BaseModel
    //{

    //    private T _target;
    //    public PropertyChangeLogProxy(T target)
    //        : base(typeof(T))
    //    {
    //        this._target = target;
    //    }

    //    public override System.Runtime.Remoting.Messaging.IMessage Invoke(System.Runtime.Remoting.Messaging.IMessage msg)
    //    {

    //        //PreProceede(msg);
    //        IMethodCallMessage callMessage = (IMethodCallMessage)msg;

    //        //如果已开启属性变更日志记录,才进行记录,也就是在执行属性变更前把这个属性打开.保存完,再关闭.
    //        if (_target.LogPropertyChange && callMessage.MethodName.StartsWith("set_"))
    //        {
    //            //set_是设置对象的属性时候会被调用的方法.每个属性都有对应的Set_函数.例如属性 Age .net会自动生成set_Age函数..
    //            //记录日志.
    //            DataChangeLog log = new DataChangeLog();

    //            log.PropertyName = callMessage.MethodName.Replace("set_", "");
    //            PropertyInfo property = typeof(T).GetProperty(log.PropertyName);
    //            log.TypeName = callMessage.TypeName;
    //            log.OldValue = property.GetValue(_target);
    //            log.NewValue = callMessage.Args[0];

    //            log.UserId = _target.DataChangeUserId;
    //            log.UserRealName = _target.DataChangeUserRealName;
    //            //LogToHttpServer.SaveOP(log); do log
    //        }

    //        object returnValue = callMessage.MethodBase.Invoke(this._target, callMessage.Args);
    //        //PostProceede(msg);
    //        return new ReturnMessage(returnValue, new object[0], 0, null, callMessage);

    //    }

    }
