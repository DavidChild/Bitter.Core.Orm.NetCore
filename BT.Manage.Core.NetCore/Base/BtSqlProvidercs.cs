using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BT.Manage.Core
{
    /********************************************************************************
    ** auth： Jason
    ** date： 2017/1/19 10:50:03
    ** desc：IQueryProvider 的实现
    ** Ver.:  V1.0.0
    ** Copyright (C) 2016 备胎 版权所有。
    *********************************************************************************/

    public class BtSqlProvidercs<TElement> : IQueryProvider
    {
        #region// IQueryProvider

        public List<Expression> lp = new List<Expression>();

        public IQueryable<TElement> AndWhere(Expression<Func<TElement, bool>> expression)
        {
            IQueryable<TElement> btt = (BtSqlQuery<TElement>)(new BtSqlQuery<TElement>(expression, this));
            lp.Add(expression);
            return btt;
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            IQueryable<TElement> btt = (BtSqlQuery<TElement>)(new BtSqlQuery<TElement>(expression, this));
            lp.Add(expression);
            return btt;
            //return query;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }

        public TResult Execute<TResult>(Expression expression)
        {
            Expression<Func<TElement, bool>> result = null;
            foreach (Expression exp in lp)
            {
                MethodCallExpression methodCall = exp as MethodCallExpression;
                while (methodCall != null)
                {
                    Expression method = methodCall.Arguments[0];
                    Expression lambda = methodCall.Arguments[1];
                    LambdaExpression right = (lambda as UnaryExpression).Operand as LambdaExpression;
                    if (result == null)
                    {
                        result = Expression.Lambda<Func<TElement, bool>>(right.Body, right.Parameters);
                    }
                    else
                    {
                        Expression left = (result as LambdaExpression).Body;
                        Expression temp = Expression.And(right.Body, left);
                        result = Expression.Lambda<Func<TElement, bool>>(temp, result.Parameters);
                    }
                    methodCall = method as MethodCallExpression;
                }
            }
            var source = new BtSql().FindAs<TElement>(result);
            dynamic _temp = source;
            TResult t = (TResult)_temp;
            return t;
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}