using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BT.Manage.Core
{
    public class WhereExpressionVisitor : ExpressionVisitorBase
    {
        private Dictionary<string, Join> _joins;

        public WhereExpressionVisitor(TranslateContext context) : base(context)
        {
            _joins = context.Joins;
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            var visitor = new BinaryExpressionVisitor(Context);
            visitor.Visit(node);
            Token = visitor.Token;
            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            var visitor = new MemberExpressionVisitor(Context);
            visitor.Visit(node);
            Token = visitor.Token;
            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var visitor = new MethodCallExpressionVisitor(Context);
            visitor.Visit(node);
            Token = visitor.Token;
            return node;
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.NodeType != ExpressionType.Not)
            {
                throw new Exception();
            }
            var operand = node.Operand;
            ExpressionVisitorBase base2 = null;
            if (operand.NodeType == ExpressionType.Call)
            {
                base2 = new MethodCallExpressionVisitor(Context);
            }
            else
            {
                if (operand.NodeType != ExpressionType.MemberAccess)
                {
                    throw new Exception();
                }
                base2 = new MemberExpressionVisitor(Context);
            }
            base2.Visit(node);
            Token = base2.Token;
            if (Token.IsBool())
            {
                Token = Token.Create(!(bool) Token.Object);
                return node;
            }
            if (Token.Type != TokenType.Column)
            {
                throw new Exception();
            }
            var condition1 = new Condition
            {
                CompareType = CompareType.Not,
                Left = Token
            };
            Token = Token.Create(condition1);
            return node;
        }

        private class BinaryExpressionVisitor : ExpressionVisitorBase
        {
            private Dictionary<string, Join> _joins;

            public BinaryExpressionVisitor(TranslateContext context) : base(context)
            {
                _joins = context.Joins;
            }

            private Token ParseBinaryExpressionLeftOrRight(BinaryExpression node)
            {
                Token token = null;
                if (node.Left is MemberExpression)
                {
                    var visitor1 = new MemberExpressionVisitor(Context);
                    visitor1.Visit(node.Left);
                    token = visitor1.Token;
                    if (token.Type != TokenType.Column)
                    {
                        throw new Exception();
                    }
                    if (TypeHelper.GetUnderlyingType(token.Column.DataType) != ReflectorConsts.BoolType)
                    {
                        throw new Exception();
                    }
                    var token2 = token;
                    var token3 = Token.Create(1);
                    var condition1 = new Condition
                    {
                        CompareType = CompareType.Equal,
                        Left = token2,
                        Right = token3
                    };
                    return Token.Create(condition1);
                }
                return VisitBinaryNode(node.Left);
            }

            private Token ParseLogicBinary(BinaryExpression node)
            {
                ExpressionType nodeType;
                var token = VisitBinaryNode(node.Left);
                if (token.IsNull())
                {
                    return token;
                }
                if (token.IsBool())
                {
                    Token = token;
                    if ((bool) token.Object)
                    {
                        if (node.NodeType == ExpressionType.OrElse)
                        {
                            return Token.CreateNull();
                        }
                    }
                    else if (node.NodeType == ExpressionType.AndAlso)
                    {
                        return Token.CreateNull();
                    }
                }
                var token2 = VisitBinaryNode(node.Right);
                if (!token.IsBool())
                {
                    if (token.Type == TokenType.Condition)
                    {
                        var condition = token.Condition;
                        if (token2 != null)
                        {
                            if (!token2.IsBool())
                            {
                                if (token2.Type == TokenType.Condition)
                                {
                                    var condition1 = new Condition
                                    {
                                        Left = Token.Create(condition),
                                        Right = token2,
                                        CompareType = SelectLogicCompareType(node.NodeType)
                                    };
                                    return Token.Create(condition1);
                                }
                                if (token2.Type == TokenType.Column)
                                {
                                    var condition2 = new Condition
                                    {
                                        Left = Token.Create(condition),
                                        Right = token2,
                                        CompareType = SelectLogicCompareType(node.NodeType)
                                    };
                                    return Token.Create(condition2);
                                }
                                var token3 = token;
                                var token4 = Token.Create(token2);
                                var condition3 = new Condition
                                {
                                    Left = token3,
                                    Right = token4,
                                    CompareType = SelectLogicCompareType(node.NodeType)
                                };
                                return Token.Create(condition3);
                            }
                            var flag2 = (bool) token2.Object;
                            nodeType = node.NodeType;
                            if (nodeType != ExpressionType.AndAlso)
                            {
                                if (nodeType != ExpressionType.OrElse)
                                {
                                    throw new Exception();
                                }
                                if (!flag2)
                                {
                                    return Token.Create(condition);
                                }
                                throw new Exception();
                            }
                            if (!flag2)
                            {
                                throw new Exception();
                            }
                        }
                        return Token.Create(condition);
                    }
                    if (token.Type == TokenType.Column)
                    {
                        if (token2 != null)
                        {
                            if (!token2.IsBool())
                            {
                                if (token2.Type == TokenType.Condition)
                                {
                                    var condition4 = new Condition
                                    {
                                        Left = token,
                                        Right = token2,
                                        CompareType = SelectLogicCompareType(node.NodeType)
                                    };
                                    return Token.Create(condition4);
                                }
                                if (token2.Type == TokenType.Column)
                                {
                                    var condition5 = new Condition
                                    {
                                        Left = token,
                                        Right = token2,
                                        CompareType = SelectLogicCompareType(node.NodeType)
                                    };
                                    return Token.Create(condition5);
                                }
                                var token5 = token;
                                var token6 = Token.Create(token2);
                                var condition6 = new Condition
                                {
                                    Left = token5,
                                    Right = token6,
                                    CompareType = SelectLogicCompareType(node.NodeType)
                                };
                                return Token.Create(condition6);
                            }
                            var flag3 = (bool) token2.Object;
                            nodeType = node.NodeType;
                            if (nodeType != ExpressionType.AndAlso)
                            {
                                if (nodeType != ExpressionType.OrElse)
                                {
                                    throw new Exception();
                                }
                                if (!flag3)
                                {
                                    return token;
                                }
                                throw new Exception();
                            }
                            if (!flag3)
                            {
                                throw new Exception();
                            }
                        }
                        return token;
                    }
                }
                else
                {
                    if (token2.IsNull())
                    {
                        return token;
                    }
                    if (token2.IsBool())
                    {
                        nodeType = node.NodeType;
                        if (nodeType != ExpressionType.AndAlso)
                        {
                            if (nodeType != ExpressionType.OrElse)
                            {
                                throw new Exception();
                            }
                            return Token.Create((bool) token2.Object ? 1 : (object) (bool) token.Object);
                        }
                        return Token.Create(!(bool) token2.Object ? 0 : (object) (bool) token.Object);
                    }
                    if ((token2.Type != TokenType.Condition) && (token2.Type != TokenType.Column))
                    {
                        throw new Exception();
                    }
                    var flag = (bool) token.Object;
                    nodeType = node.NodeType;
                    if (nodeType != ExpressionType.AndAlso)
                    {
                        if ((nodeType == ExpressionType.OrElse) && !flag)
                        {
                            return token2;
                        }
                    }
                    else if (flag)
                    {
                        return token2;
                    }
                    return Token.CreateNull();
                }
              
                throw new Exception();
            }

            private Token ParseMathBinary(BinaryExpression node)
            {
                ExpressionVisitorBase base2;
                ExpressionVisitorBase base3;
                if (node.Left is BinaryExpression)
                {
                    base2 = new BinaryExpressionVisitor(Context);
                }
                else
                {
                    base2 = new MemberExpressionVisitor(Context);
                }
                base2.Visit(node.Left);
                var token = base2.Token;
                if (node.Right is BinaryExpression)
                {
                    base3 = new BinaryExpressionVisitor(Context);
                }
                else
                {
                    base3 = new MemberExpressionVisitor(Context);
                }
                base3.Visit(node.Right);
                var token2 = base3.Token;
                if ((token.Type != TokenType.Object) || (token2.Type != TokenType.Object))
                {
                    if ((token.Type == TokenType.Column) && (token2.Type == TokenType.Object))
                    {
                        var condition1 = new Condition
                        {
                            Left = token,
                            CompareType = SelectMathCompareType(node.NodeType),
                            Right = token2
                        };
                        return Token.Create(condition1);
                    }
                    if ((token.Type != TokenType.Condition) || (token2.Type != TokenType.Object))
                    {
                        throw new Exception();
                    }
                    var condition2 = new Condition
                    {
                        Left = token,
                        CompareType = SelectMathCompareType(node.NodeType),
                        Right = token2
                    };
                    return Token.Create(condition2);
                }
                if ((token.Object == null) && (token2.Object == null))
                {
                    return Token.Create(true);
                }
                if ((token == null) || (token2 == null))
                {
                    return Token.Create(false);
                }
                if (token.Type != TokenType.Object)
                {
                    throw new Exception();
                }
                if (token.Object is string || token.Object is bool || token.Object is bool?)
                {
                    return Token.Create(token.Equals(token2));
                }
                if (!(token.Object is DateTime) && !(token.Object is DateTime?))
                {
                    if (!(token.Object is int) && !(token.Object is int?))
                    {
                        if (!(token.Object is short) && !(token.Object is short?))
                        {
                            if (!(token.Object is long) && !(token.Object is long?))
                            {
                                return Token.Create(token.Object == token2.Object);
                            }
                            var num5 = Convert.ToInt64(token.Object);
                            var num6 = Convert.ToInt64(token2.Object);
                            switch (node.NodeType)
                            {
                                case ExpressionType.Equal:
                                    return Token.Create(num5 = num6);

                                case ExpressionType.GreaterThan:
                                    return Token.Create(num5 > num6);

                                case ExpressionType.GreaterThanOrEqual:
                                    return Token.Create(num5 >= num6);

                                case ExpressionType.LessThan:
                                    return Token.Create(num5 < num6);

                                case ExpressionType.LessThanOrEqual:
                                    return Token.Create(num5 <= num6);
                            }
                            throw new Exception();
                        }
                        var num3 = Convert.ToInt16(token.Object);
                        var num4 = Convert.ToInt16(token2.Object);
                        switch (node.NodeType)
                        {
                            case ExpressionType.Equal:
                                return Token.Create(num3 = num4);

                            case ExpressionType.GreaterThan:
                                return Token.Create(num3 > num4);

                            case ExpressionType.GreaterThanOrEqual:
                                return Token.Create(num3 >= num4);

                            case ExpressionType.LessThan:
                                return Token.Create(num3 < num4);

                            case ExpressionType.LessThanOrEqual:
                                return Token.Create(num3 <= num4);
                        }
                        throw new Exception();
                    }
                    var num = Convert.ToInt32(token.Object);
                    var num2 = Convert.ToInt32(token2.Object);
                    switch (node.NodeType)
                    {
                        case ExpressionType.Equal:
                            return Token.Create(num = num2);

                        case ExpressionType.GreaterThan:
                            return Token.Create(num > num2);

                        case ExpressionType.GreaterThanOrEqual:
                            return Token.Create(num >= num2);

                        case ExpressionType.LessThan:
                            return Token.Create(num < num2);

                        case ExpressionType.LessThanOrEqual:
                            return Token.Create(num <= num2);
                    }
                    throw new Exception();
                }
                var time = Convert.ToDateTime(token.Object);
                var time2 = Convert.ToDateTime(token2.Object);
                switch (node.NodeType)
                {
                    case ExpressionType.Equal:
                        return Token.Create(time = time2);

                    case ExpressionType.GreaterThan:
                        return Token.Create(time > time2);

                    case ExpressionType.GreaterThanOrEqual:
                        return Token.Create(time >= time2);

                    case ExpressionType.LessThan:
                        return Token.Create(time < time2);

                    case ExpressionType.LessThanOrEqual:
                        return Token.Create(time <= time2);
                }
                throw new Exception();
            }

            private Token ParseMethodCallExpression(MethodCallExpression methodCallExpression)
            {
                var visitor1 = new MethodCallExpressionVisitor(Context);
                visitor1.Visit(methodCallExpression);
                return visitor1.Token;
            }

            private Token ParseNotExpression(UnaryExpression unaryExpression)
            {
                var operand = unaryExpression.Operand;
                if (operand is MemberExpression)
                {
                    var visitor = new MemberExpressionVisitor(Context);
                    visitor.Visit(operand);
                    var type = visitor.Token.Type;
                    if (type != TokenType.Column)
                    {
                        if (type != TokenType.Object)
                        {
                            throw new Exception();
                        }
                        if (!visitor.Token.IsBool())
                        {
                            throw new Exception("不支持");
                        }
                        return Token.Create(!(bool) visitor.Token.Object);
                    }
                    if ((operand.Type == typeof (bool)) || (operand.Type == typeof (bool?)))
                    {
                        var condition1 = new Condition
                        {
                            CompareType = CompareType.Equal,
                            Left = Token.Create(1),
                            Right = Token.Create(1)
                        };
                        return Token.Create(condition1);
                    }
                    var condition2 = new Condition
                    {
                        Left = visitor.Token,
                        CompareType = CompareType.Not
                    };
                    return Token.Create(condition2);
                }
                if (!(operand is MethodCallExpression))
                {
                    throw new Exception("不支持");
                }
                var visitor1 = new MethodCallExpressionVisitor(Context);
                visitor1.Visit(operand);
                var token = visitor1.Token;
                switch (token.Type)
                {
                    case TokenType.Column:
                    {
                        var condition3 = new Condition
                        {
                            Left = token,
                            CompareType = CompareType.Not
                        };
                        return Token.Create(condition3);
                    }
                    case TokenType.Object:
                        return Token.Create(!(bool) token.Object);

                    case TokenType.Condition:
                    {
                        var condition4 = new Condition
                        {
                            Left = Token.Create(token.Condition),
                            CompareType = CompareType.Not
                        };
                        return Token.Create(condition4);
                    }
                }
                throw new Exception();
            }

            private CompareType SelectLogicCompareType(ExpressionType expressionType)
            {
                if (expressionType != ExpressionType.AndAlso)
                {
                    if (expressionType != ExpressionType.OrElse)
                    {
                        throw new Exception();
                    }
                    return CompareType.Or;
                }
                return CompareType.And;
            }

            private CompareType SelectMathCompareType(ExpressionType expressionType)
            {
                if (expressionType <= ExpressionType.Multiply)
                {
                    if (expressionType <= ExpressionType.AddChecked)
                    {
                        if ((expressionType == ExpressionType.Add) || (expressionType == ExpressionType.AddChecked))
                        {
                            return CompareType.Add;
                        }
                    }
                    else
                    {
                        switch (expressionType)
                        {
                            case ExpressionType.Divide:
                                return CompareType.Divide;

                            case ExpressionType.Equal:
                                return CompareType.Equal;

                            case ExpressionType.GreaterThan:
                                return CompareType.GreaterThan;

                            case ExpressionType.GreaterThanOrEqual:
                                return CompareType.GreaterThanOrEqual;

                            case ExpressionType.LessThan:
                                return CompareType.LessThan;

                            case ExpressionType.LessThanOrEqual:
                                return CompareType.LessThanOrEqual;

                            case ExpressionType.Multiply:
                                return CompareType.Multiply;
                        }
                    }
                }
                else if (expressionType <= ExpressionType.NotEqual)
                {
                    if (expressionType == ExpressionType.MultiplyChecked)
                    {
                        return CompareType.Multiply;
                    }
                    if (expressionType == ExpressionType.NotEqual)
                    {
                        return CompareType.NotEqual;
                    }
                }
                else if ((expressionType == ExpressionType.Subtract) ||
                         (expressionType == ExpressionType.SubtractChecked))
                {
                    return CompareType.Substarct;
                }
                throw new Exception();
               
            }

            public override Expression Visit(Expression node)
            {
                switch (node.NodeType)
                {
                    case ExpressionType.LessThanOrEqual:
                    case ExpressionType.Multiply:
                    case ExpressionType.Add:
                    case ExpressionType.AddChecked:
                    case ExpressionType.Divide:
                    case ExpressionType.Equal:
                    case ExpressionType.GreaterThan:
                    case ExpressionType.GreaterThanOrEqual:
                    case ExpressionType.LessThan:
                    case ExpressionType.NotEqual:
                    case ExpressionType.MultiplyChecked:
                    case ExpressionType.Subtract:
                    case ExpressionType.SubtractChecked:
                        Token = ParseMathBinary((BinaryExpression) node);
                        return node;

                    case ExpressionType.AndAlso:
                    case ExpressionType.OrElse:
                        Token = ParseLogicBinary((BinaryExpression) node);
                        return node;

                    case ExpressionType.Call:
                        Token = ParseMethodCallExpression((MethodCallExpression) node);
                        return node;

                    case ExpressionType.Constant:
                        Token = Token.Create(((ConstantExpression) node).Value);
                        return node;

                    case ExpressionType.Not:
                        Token = ParseNotExpression((UnaryExpression) node);
                        return node;
                }
                throw new Exception(node.NodeType.ToString());
            }

            protected override Expression VisitBinary(BinaryExpression node)
            {
                var visitor = new BinaryExpressionVisitor(Context);
                visitor.Visit(node.Left);
                Token = visitor.Token;
                return node;
            }

            private Token VisitBinaryNode(Expression node)
            {
                if (node.NodeType == ExpressionType.Not)
                {
                    var token = VisitBinaryNode(((UnaryExpression) node).Operand);
                    switch (token.Type)
                    {
                        case TokenType.Column:
                        {
                            var condition1 = new Condition
                            {
                                Left = token,
                                CompareType = CompareType.Not
                            };
                            return Token.Create(condition1);
                        }
                        case TokenType.Object:
                            return Token.Create(!(bool) token.Object);

                        case TokenType.Condition:
                            token.Condition.Right = Token.Create(0);
                            return token;
                    }
                    throw new Exception();
                }
                if (node is MemberExpression)
                {
                    var visitor1 = new MemberExpressionVisitor(Context);
                    visitor1.Visit(node);
                    var token2 = visitor1.Token;
                    var type = token2.Type;
                    if (type != TokenType.Column)
                    {
                        if (type != TokenType.Object)
                        {
                            throw new Exception();
                        }
                        return token2;
                    }
                    if (TypeHelper.GetUnderlyingType(token2.Column.DataType) != ReflectorConsts.BoolType)
                    {
                        throw new Exception();
                    }
                    var token3 = token2;
                    var token4 = Token.Create(1);
                    var condition2 = new Condition
                    {
                        CompareType = CompareType.Equal,
                        Left = token3,
                        Right = token4
                    };
                    return Token.Create(condition2);
                }
                var visitor2 = new BinaryExpressionVisitor(Context);
                visitor2.Visit(node);
                return visitor2.Token;
            }
        }
    }
}