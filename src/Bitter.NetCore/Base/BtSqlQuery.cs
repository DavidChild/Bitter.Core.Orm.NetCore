using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BT.Manage.Core
{
    /********************************************************************************
    ** auth： Jason
    ** date： 2017/1/19 10:47:20
    ** desc：IQueryable<T> 的实现
    ** Ver.:  V1.0.0
    ** Copyright (C) 2016 备胎 版权所有。
    *********************************************************************************/

    public class BtSqlQuery<TElement> : IQueryable<TElement>
    {
        public Expression _expression;
        public IQueryProvider _provider;

        public BtSqlQuery()
        {
            _provider = new BtSqlProvidercs<TElement>();
            _expression = Expression.Constant(this);
        }

        public BtSqlQuery(Expression expression, IQueryProvider provider)
        {
            _expression = expression;
            _provider = provider;
        }

        public Type ElementType
        {
            get { return typeof(BtSqlQuery<TElement>); }
        }

        public Expression Expression
        {
            get { return _expression; }
        }

        public IQueryProvider Provider
        {
            get { return _provider; }
        }

        public BtSqlQuery<TElement> AndWhere(Expression<Func<TElement, bool>> expression)
        {
            this.Where((Expression<Func<TElement, bool>>)expression);
            return this;
        }

        public IEnumerator<TElement> GetEnumerator()
        {
            var result = _provider.Execute<List<TElement>>(_expression);
            if (result == null)
                yield break;
            foreach (var item in result)
            {
                yield return item;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}