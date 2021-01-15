using System.Linq.Expressions;

namespace BT.Manage.Core
{
    public class NoLockExpressionVisitor : ExpressionVisitorBase
    {
        public NoLockExpressionVisitor(TranslateContext context) : base(context)
        {
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            var table = EntityConfigurationManager.GetTable(node.Type.GetGenericArguments()[0]);
            ExtraObject = table.Name;
            return base.VisitConstant(node);
        }
    }
}