using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BT.Manage.Core
{
    public class DataQuery<T> : IQueryable<T>, IEnumerable<T>, IEnumerable, IQueryable, IOrderedQueryable<T>,
        IOrderedQueryable
    {
        private readonly QueryProvider provider;

        public DataQuery(QueryProvider provider)
        {
            this.provider = provider;
            Expression = Expression.Constant(this);
            ElementType = typeof (T);
        }

        public DataQuery(QueryProvider provider, Expression expression)
        {
            this.provider = provider;
            Expression = expression;
            ElementType = typeof (T);
        }

        public string CommandText
        {
            get { return ((QueryProvider) Provider).GetCommandText(Expression); }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return provider.Execute<IEnumerable<T>>(Expression).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        public Type ElementType { get; private set; }


        public Expression Expression { get; private set; }


        public IQueryProvider Provider
        {
            get { return provider; }
        }
    }
}