using System;
using System.Data;

namespace Bitter.Core
{
    /********************************************************************************
    ** auth： davidchild
    ** date： 2017/2/14 12:52:44
    ** desc： 执行者
    ** Ver.:  V1.0.0
    ** Copyright (C) 2016 Bitter 版权所有。
    *********************************************************************************/

    public interface IPageAccess
    {
        Int32 Count();
        /// <summary>
        /// 对with as 子句的支持
        /// </summary>
        /// <param name="withSql"></param>
        /// <returns></returns>
        IPageAccess AddPreWith(string withSql);
        /// <summary>
        /// 对with as 子句的支持
        /// </summary>
        /// <param name="withSql"></param>
        /// <param name="parmaters"></param>
        /// <returns></returns>
        IPageAccess AddPreWith(string withSql, dynamic parmaters);
        IPageAccess All();

        IPageAccess OrderBy(string fullOrder);

        IPageAccess Select(dynamic selectColums);

        /// <summary>
        /// 当前页
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        IPageAccess Skip(Int32 page, bool isApp = false);

        /// <summary>
        /// 当前分页
        /// </summary>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IPageAccess Take(Int32 pageSize);

        IPageAccess ThenASC(string filedName);

        IPageAccess ThenDESC(string filedName);

        DataTable ToDataTable();

        IUnionPageAccess ToUnionPage();

        IPageAccess Where(string setwhere, dynamic parmaters);
        /// <summary>
        /// Or; 注意：此时的Or, 跟前置条件条件 永远是并列关系
        /// 例如：1：（(x.y=="1") or (x.z=3)） or (x.n=4)
        /// 例如：2：（(x.y=="1") and (x.z=3)） or (x.n=4)
        /// 一定要注意：并没有这样的写法：(x.y=="1") and (x.z=3) or (x.n=4)，杜绝这种错误的写法
        /// 在使用Or时,自动将你之前的所有的条件会放到一个() 中跟你现有的Or 形成并条件 例如： (之前写的条件) or (现有的条件) ， 永远是这种关系
        /// </summary>
        /// <param name="setOr"></param>
        IPageAccess Or(string setOr, dynamic parmaters);
        /// <summary>
        /// Or; 注意：此时的Or, 跟前置条件条件 永远是并列关系
        /// 例如：1：（(x.y=="1") or (x.z=3)） or (x.n=4)
        /// 例如：2：（(x.y=="1") and (x.z=3)） or (x.n=4)
        /// 一定要注意：并没有这样的写法：(x.y=="1") and (x.z=3) or (x.n=4)，杜绝这种错误的写法
        /// 在使用Or时,自动将你之前的所有的条件会放到一个() 中跟你现有的Or 形成并条件 例如： (之前写的条件) or (现有的条件) ， 永远是这种关系
        /// </summary>
        /// <param name="setOr"></param>
        IPageAccess Or(string setOr);

        IPageAccess Where(string setwhere);
        IPageAccess SetPageMode(PageMode pageMode);
    }
}