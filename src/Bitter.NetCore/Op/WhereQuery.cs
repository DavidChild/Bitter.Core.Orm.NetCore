using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Bitter.Core
{
    public  class WhereQuery<T> : BaseQuery where T:BaseModel
    {
        protected  Expression<Func<T, bool>> _Condition { get; set; }
        //
        public virtual WhereQuery<T> Where(Expression<Func<T, bool>> condition)
        {
            if (_Condition == null)
            {
                _Condition = condition;
            }
            else
            {
                //将当前条件下的 condition 添加到 当前的condion 中.
                var invokedExpr = Expression.Invoke(condition, _Condition.Parameters.Cast<Expression>());
                _Condition = Expression.Lambda<Func<T, bool>>(Expression.And(_Condition.Body, invokedExpr), _Condition.Parameters);

            }
           
            return this;
        }
     
    }
}
