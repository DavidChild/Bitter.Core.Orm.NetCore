using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace BT.Manage.Core
{
    public class JoinExpressionVisitor : ExpressionVisitorBase
    {
        private static string[] _joinMethodNames = {"Join", "GroupJoin"};

        public JoinExpressionVisitor(TranslateContext context) : base(context)
        {
            Joins = context.Joins;
            ExtraObject = new List<string>();
        }

        public Dictionary<string, Join> Joins { get; private set; }

        public Expression SelectExpression { get; private set; }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var expression = node.Arguments[0];
            if (expression.NodeType == ExpressionType.Call)
            {
                var expression5 = expression as MethodCallExpression;
                if (expression5.Method.Name == "NoLock")
                {
                    expression = expression5.Arguments[0];
                }
            }
            var key = "<>" + Path.GetRandomFileName();
            if (expression.NodeType == ExpressionType.Call)
            {
                var expression1 = (MethodCallExpression) expression;
                var visitor2 = new JoinExpressionVisitor(Context);
                visitor2.Visit(expression);
                if (node.Method.Name == "SelectMany")
                {
                    SelectExpression = node.Arguments[2];
                    key = ((LambdaExpression) ((UnaryExpression) node.Arguments[2]).Operand).Parameters[1].Name;
                    var joins = visitor2.Joins;
                    var pair = joins.LastOrDefault();
                    joins.Remove(pair.Key);
                    Joins.Add(key, pair.Value);
                    return node;
                }
            }
            SelectExpression = node.Arguments[4];
            var expression2 = node.Arguments[1];
            if (expression2.NodeType == ExpressionType.Call)
            {
                var expression6 = (MethodCallExpression) expression2;
                if (expression6.Method.Name != "NoLock")
                {
                    throw new Exception();
                }
                expression2 = expression6.Arguments[0];
            }
            if (expression2.NodeType != ExpressionType.Constant)
            {
                throw new Exception();
            }
            var expression3 = node.Arguments[2];
            var expression4 = node.Arguments[3];
            var visitor =
                new MemberExpressionVisitor(Context);
            visitor.Visit(expression3);
            if (visitor.Token.Type != TokenType.Column)
            {
                throw new Exception();
            }
            var join = new Join
            {
                Left = visitor.Token.Column
            };
            visitor = new MemberExpressionVisitor(Context);
            visitor.Visit(expression4);
            join.Right = visitor.Token.Column;
            var name = node.Method.Name;
            if (name != "Join")
            {
                if (name != "GroupJoin")
                {
                    throw new Exception();
                }
            }
            else
            {
                join.JoinType = JoinType.Inner;
                var expression8 = (LambdaExpression) ((UnaryExpression) node.Arguments[4]).Operand;
                Joins.Add(key, join);
                return node;
            }
            join.JoinType = JoinType.Left;
            var expression7 = (LambdaExpression) ((UnaryExpression) node.Arguments[4]).Operand;
            Joins.Add(key, join);
            return node;
        }
    }
}