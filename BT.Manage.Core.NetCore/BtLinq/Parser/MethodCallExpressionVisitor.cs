using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BT.Manage.Core
{
    public class MethodCallExpressionVisitor : ExpressionVisitorBase
    {
        private bool _isColumn;
        private Dictionary<string, Join> _joins;

        public MethodCallExpressionVisitor(TranslateContext context) : base(context)
        {
            _joins = context.Joins;
        }

        private Token ParseArgument(Expression argExp)
        {
            while ((argExp.NodeType == ExpressionType.Convert) || (argExp.NodeType == ExpressionType.ConvertChecked))
            {
                argExp = ((UnaryExpression) argExp).Operand;
            }
            if ((argExp.NodeType == ExpressionType.MemberAccess) || (argExp.NodeType == ExpressionType.Call))
            {
                var visitor1 = new MemberExpressionVisitor(Context);
                visitor1.Visit(argExp);
                if (visitor1.Token.Type == TokenType.Column)
                {
                    _isColumn = true;
                }
                return visitor1.Token;
            }
            if (argExp.NodeType != ExpressionType.Constant)
            {
                throw new Exception();
            }
            return Token.Create(((ConstantExpression) argExp).Value);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            ColumnConverter converter;
            var source = new List<Token>();
            foreach (var expression in node.Arguments)
            {
                source.Add(ParseArgument(expression));
            }
            if (node.Object != null)
            {
                var visitor = new MemberExpressionVisitor(Context);
                visitor.Visit(node.Object);
                if ((visitor.Token.Type == TokenType.Column) && !_isColumn)
                {
                    Token = visitor.Token;
                    var memberInfo = node.Method;
                    var enumerable = from x in source select x.Object;
                    Token.Column.Converters.Push(new ColumnConverter(memberInfo, enumerable.ToList(), true));
                    return node;
                }
                if ((visitor.Token.Type == TokenType.Object) && !_isColumn)
                {
                    Token =
                        Token.Create(node.Method.Invoke(visitor.Token.Object,
                            (from x in source select x.Object).ToArray()));
                    return node;
                }
                if ((visitor.Token.Type != TokenType.Object) || !_isColumn)
                {
                    throw new Exception();
                }
                Token = source[0];
                var list4 = new List<object>
                {
                    visitor.Token.Object
                };
                list4.AddRange(source.Skip(1));
                Token.Column.Converters.Push(new ColumnConverter(node.Method, list4, false));
                return node;
            }
            var method = node.Method;
            if (!_isColumn)
            {
                var list3 = new List<object>();
                var infoArray = node.Method.GetParameters().ToArray();
                BaseTypeConverter converter2 = null;
                for (var i = 0; i < infoArray.Length; i++)
                {
                    var info2 = infoArray[i];
                    var obj2 = source[i].Object;
                    if (TypeHelper.IsValueType(info2.ParameterType))
                    {
                        converter2 = new BaseTypeConverter(obj2, info2.ParameterType);
                        converter2.Process();
                        list3.Add(converter2.Result);
                    }
                    else
                    {
                        list3.Add(obj2);
                    }
                }
                Token = Token.Create(node.Method.Invoke(null, list3.ToArray()));
                return node;
            }
            var parameters = new List<object>();
            var name = method.Name;
            if (name != "ToDateTime")
            {
                if (name != "Contains")
                {
                    throw new Exception();
                }
            }
            else
            {
                Token = source[0];
                parameters.AddRange(source.Skip(1));
                converter = new ColumnConverter(method, parameters);
                Token.Column.Converters.Push(converter);
                return node;
            }
            Token = source[1];
            parameters.Add(source[0].Object);
            parameters.AddRange(from x in source.Skip(2)
                where x.Type == TokenType.Object
                select x.Object);
            converter = new ColumnConverter(method, parameters);
            Token.Column.Converters.Push(converter);
            return node;
        }
    }
}