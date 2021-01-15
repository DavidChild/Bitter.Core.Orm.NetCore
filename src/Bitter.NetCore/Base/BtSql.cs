using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BT.Manage.Core
{
    /********************************************************************************
    ** auth： Jason
    ** date： 2017/1/19 13:29:22
    ** desc：
    ** Ver.:  V1.0.0
    ** Copyright (C) 2016 备胎 版权所有。
    *********************************************************************************/

    internal class BtSql : IModelOperation
    {
        #region//ModelOpration

        public List<TElement> FindAs<TElement>(Expression<Func<TElement, bool>> lambdawhere)
        {
            return ModelOpretion.ModelListInstance<TElement>(lambdawhere);
        }

        public int Remove<TElement>(Expression<Func<TElement, bool>> lambdawhere)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TElement> Source<TElement>()
        {
            return new BtSqlQuery<TElement>();
        }

        #endregion
    }
}