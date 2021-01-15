using System;
using System.Collections.Generic;
using System.Data;

namespace Bitter.Core
{
    /********************************************************************************
    ** auth： davidchild
    ** date： 2017/2/20 10:35:38
    ** desc：
    ** Ver.:  V1.0.0
    ** Copyright (C) 2016 Bitter 版权所有。
    *********************************************************************************/

    public interface IUnionPageAccess
    {
        Int32 Count();// { get; }
        /// <summary>
        /// 对with as 子句的支持
        /// </summary>
        /// <param name="withSql"></param>
        /// <returns></returns>
        IUnionPageAccess AddPreWith(string withSql);
        /// <summary>
        /// 对with as 子句的支持
        /// </summary>
        /// <param name="withSql"></param>
        /// <param name="parmaters"></param>
        /// <returns></returns>
        IUnionPageAccess AddPreWith(string withSql, dynamic parmaters);

        IUnionPageAccess Skip(Int32 page, bool isApp = false);

        IUnionPageAccess Take(Int32 pageSize);

        IUnionPageAccess ThenASC(string filedName);

        IUnionPageAccess ThenDESC(string filedName);

        IEnumerable<DataRow> ToDataTable();

        IUnionPageAccess Union(IPageAccess page);

        IUnionPageAccess Union(List<IPageAccess> unionPage);

        IUnionPageAccess Where(string setwhere, dynamic parmaters);

        IUnionPageAccess Where(string setwhere);
        /// <summary>
        /// Or; 注意：此时的Or, 跟前置条件条件 永远是并列关系
        /// 例如：1：（(x.y=="1") or (x.z=3)） or (x.n=4)
        /// 例如：2：（(x.y=="1") and (x.z=3)） or (x.n=4)
        /// 一定要注意：并没有这样的写法：(x.y=="1") and (x.z=3) or (x.n=4)，杜绝这种错误的写法
        /// 在使用Or时,自动将你之前的所有的条件会放到一个() 中跟你现有的Or 形成并条件 例如： (之前写的条件) or (现有的条件) ， 永远是这种关系
        /// </summary>
        /// <param name="setOr"></param>
        IUnionPageAccess Or(string setOr, dynamic parmaters);
        /// <summary>
        /// Or; 注意：此时的Or, 跟前置条件条件 永远是并列关系
        /// 例如：1：（(x.y=="1") or (x.z=3)） or (x.n=4)
        /// 例如：2：（(x.y=="1") and (x.z=3)） or (x.n=4)
        /// 一定要注意：并没有这样的写法：(x.y=="1") and (x.z=3) or (x.n=4)，杜绝这种错误的写法
        /// 在使用Or时,自动将你之前的所有的条件会放到一个() 中跟你现有的Or 形成并条件 例如： (之前写的条件) or (现有的条件) ， 永远是这种关系
        /// </summary>
        /// <param name="setOr"></param>
        IUnionPageAccess Or(string setOr);
        IUnionPageAccess SetPageMode(PageMode pageMode);
    }
}