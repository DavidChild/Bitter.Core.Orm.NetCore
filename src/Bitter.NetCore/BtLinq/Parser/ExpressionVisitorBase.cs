using System.Linq.Expressions;

namespace BT.Manage.Core
{
    public abstract class ExpressionVisitorBase : ExpressionVisitor
    {
        public ExpressionVisitorBase(TranslateContext context)
        {
            Context = context;
        }

        protected TranslateContext Context { get; private set; }


        public object ExtraObject { get; protected set; }

        public Token Token { get; protected set; }
    }
}