using System.Collections.Generic;
using System.Linq.Expressions;

namespace BT.Manage.Core
{
    public class MemberExpressionVisitor : ExpressionVisitorBase
    {
        private Dictionary<string, Join> _joins;

        public MemberExpressionVisitor(TranslateContext context) : base(context)
        {
            _joins = context.Joins;
        }

        public override Expression Visit(Expression node)
        {
            ExpressionVisitorBase base2 = null;
            if (node.NodeType == ExpressionType.Quote)
            {
                node = ((UnaryExpression) node).Operand;
            }
            if (node.NodeType == ExpressionType.Lambda)
            {
                node = ((LambdaExpression) node).Body;
            }
            if (node.NodeType == ExpressionType.Call)
            {
                base2 = new MethodCallExpressionVisitor(Context);
            }
            else
            {
                base2 = new PropertyFieldExpressionVisitor(Context);
            }
            base2.Visit(node);
            Token = base2.Token;
            return node;
        }
    }
}