using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BT.Manage.Core
{
    /********************************************************************************
    ** auth： Jason
    ** date： 2017/1/9 13:32:14
    ** desc： Model 能直接执行的操作办法
    ** Ver.:  V1.0.0
    ** Copyright (C) 2016 备胎 版权所有。
    *********************************************************************************/

    public interface IModelOperation
    {
        List<T> FindAs<T>(Expression<Func<T, bool>> lambdaCondition);

        int Remove<T>(Expression<Func<T, bool>> lambdaCondition);

        IQueryable<T> Source<T>();
    }
}