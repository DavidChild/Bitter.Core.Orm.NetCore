using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace BT.Manage.Core
{
    public class PropertyFieldExpressionVisitor : ExpressionVisitorBase
    {
        private readonly Dictionary<string, Column> _columns;
        private readonly Dictionary<string, Join> _joins;
        private readonly Stack<Expression> _memberInfos;
        private MemberInfo _tableMember;

        public PropertyFieldExpressionVisitor(TranslateContext context) : base(context)
        {
            _memberInfos = new Stack<Expression>();
            _joins = context.Joins;
            _columns = context.Columns;
        }

        private Table CreateTable(string alias, string db, string name, Type type)
        {
            return
                new Table
                {
                    Alias = alias,
                    DataBase = db,
                    Name = name,
                    Type = type
                };
        }

        private string GetConverter(string converter)
        {
            if ((_memberInfos.Count > 0) && string.IsNullOrWhiteSpace(converter))
            {
                converter = "{0}";
            }
            while (_memberInfos.Count > 0)
            {
                var expression = _memberInfos.Pop();
                if (expression.NodeType != ExpressionType.MemberAccess)
                {
                    throw new Exception();
                }
                var member = ((MemberExpression) expression).Member;
                if (member.MemberType == MemberTypes.Field)
                {
                    throw new Exception();
                }
                if (member.MemberType != MemberTypes.Property)
                {
                    throw new Exception();
                }
                var info2 = (PropertyInfo) member;
                if (!info2.DeclaringType.IsGenericType ||
                    (info2.DeclaringType.GetGenericTypeDefinition() != typeof (Nullable<>)) || (info2.Name != "Value"))
                {
                    if (((info2.DeclaringType != typeof (DateTime)) && (info2.DeclaringType != typeof (DateTime?))) ||
                        (info2.Name != "Date"))
                    {
                        throw new Exception("除x=>x.Time.Date以外，不支持其他的");
                    }
                    converter = string.Format(converter, "CONVERT(DATE,{0},211)");
                }
            }
            return converter;
        }

        private SchemaModel.Table GetTable()
        {
            MemberExpression expression;
            Label_0000:
            expression = (MemberExpression) _memberInfos.Peek();
            _tableMember = expression.Member;
            Type fieldType = null;
            if (_tableMember.MemberType == MemberTypes.Field)
            {
                fieldType = ((FieldInfo) _tableMember).FieldType;
            }
            else
            {
                fieldType = ((PropertyInfo) _tableMember).PropertyType;
            }
            if (TypeHelper.IsCompilerGenerated(fieldType))
            {
                _memberInfos.Pop();
                goto Label_0000;
            }
            if (EntityConfigurationManager.IsEntity(fieldType))
            {
                _memberInfos.Pop();
                return GetTable(fieldType);
            }
            return null;
        }

        private SchemaModel.Table GetTable(Type tableType)
        {
            if (ParserUtils.IsAnonymousType(tableType))
            {
                _tableMember = ((MemberExpression) _memberInfos.Pop()).Member;
                tableType = ((PropertyInfo) _tableMember).PropertyType;
                return GetTable(tableType);
            }
            return EntityConfigurationManager.GetTable(tableType);
        }

        public object GetValue(MemberExpression memberExpression, object obj)
        {
            if (memberExpression.NodeType != ExpressionType.MemberAccess)
            {
                throw new Exception();
            }
            if (memberExpression.Member.MemberType == MemberTypes.Field)
            {
                return ((FieldInfo) memberExpression.Member).GetValue(obj);
            }
            return ((PropertyInfo) memberExpression.Member).GetValue(obj);
        }

        public override Expression Visit(Expression node)
        {
            var operand = node;
            if (operand.NodeType == ExpressionType.Quote)
            {
                operand = ((UnaryExpression) operand).Operand;
            }
            if (operand.NodeType == ExpressionType.Lambda)
            {
                return base.Visit(((LambdaExpression) operand).Body);
            }
            return base.Visit(node);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            var obj2 = node.Value;
            while (_memberInfos.Count > 0)
            {
                obj2 = GetValue((MemberExpression) _memberInfos.Pop(), obj2);
            }
            Token = Token.Create(obj2);
            return base.VisitConstant(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression == null)
            {
                var obj2 = GetValue(node, null);
                while (_memberInfos.Count > 0)
                {
                    obj2 = GetValue((MemberExpression) _memberInfos.Pop(), obj2);
                }
                Token = Token.Create(obj2);
                return node;
            }
            if (node.Member.DeclaringType == typeof (TimeSpan))
            {
                if (node.Expression.NodeType != ExpressionType.Subtract)
                {
                    throw new Exception();
                }
                var expression1 = (BinaryExpression) node.Expression;
                var left = expression1.Left;
                var right = expression1.Right;
                var visitor = new MemberExpressionVisitor(Context);
                visitor.Visit(left);
                var visitor2 = new MemberExpressionVisitor(Context);
                visitor2.Visit(right);
                if ((visitor.Token.Type == TokenType.Column) && (visitor2.Token.Type == TokenType.Object))
                {
                    var time = (DateTime) visitor2.Token.Object;
                    Token = visitor.Token;
                    var list1 = new List<object>
                    {
                        time
                    };
                    Token.Column.Converters.Push(new ColumnConverter(node.Member, list1, true));
                    return node;
                }
                if ((visitor.Token.Type != TokenType.Object) || (visitor2.Token.Type != TokenType.Column))
                {
                    throw new Exception();
                }
                var time2 = (DateTime) visitor.Token.Object;
                Token = visitor2.Token;
                var parameters = new List<object>
                {
                    time2
                };
                Token.Column.Converters.Push(new ColumnConverter(node.Member, parameters, false));
                return node;
            }
            _memberInfos.Push(node);
            return base.VisitMember(node);
        }

        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            var dictionary = new Dictionary<string, object>();
            foreach (MemberAssignment assignment in node.Bindings)
            {
                var visitor = new MemberExpressionVisitor(Context);
                visitor.Visit(assignment.Expression);
                if (visitor.Token.Type != TokenType.Object)
                {
                    throw new NotSupportedException("不支持");
                }
                dictionary.Add(assignment.Member.Name, visitor.Token.Object);
            }
            Token = Token.Create(dictionary);
            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var visitor = new MethodCallExpressionVisitor(Context);
            visitor.Visit(node);
            var type = visitor.Token.Type;
            if (type != TokenType.Column)
            {
                if (type == TokenType.Condition)
                {
                    throw new Exception();
                }
                var obj2 = visitor.Token.Object;
                while (_memberInfos.Count > 0)
                {
                    obj2 = GetValue((MemberExpression) _memberInfos.Pop(), obj2);
                }
                Token = Token.Create(obj2);
                return node;
            }
            Token = visitor.Token;
            var column = Token.Column;
            while (_memberInfos.Count > 0)
            {
                var expression = _memberInfos.Pop();
                if (expression.NodeType != ExpressionType.MemberAccess)
                {
                    throw new Exception();
                }
                var member = ((MemberExpression) expression).Member;
                column.Converters.Push(new ColumnConverter(member, new List<object>()));
            }
            return node;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            var column = new Column();
            if (!EntityConfigurationManager.IsEntity(node.Type))
            {
                var table2 = GetTable();
                MemberInfo member = null;
                var key = string.Empty;
                Table table3 = null;
                if (table2 != null)
                {
                    key = _tableMember.Name;
                    member = ((MemberExpression) _memberInfos.Pop()).Member;
                    if (_joins != null)
                    {
                        if (_joins.ContainsKey(key))
                        {
                            key = _joins[key].Right.Table.Alias;
                        }
                    }
                    else
                    {
                        key = table2.Name;
                    }
                    table3 = CreateTable(key, table2.DataBase, table2.Name, table2.Type);
                }
                else
                {
                    member = ((MemberExpression) _memberInfos.Pop()).Member;
                    table3 = _columns[member.Name].Table;
                    key = table3.Alias;
                    table2 = GetTable(table3.Type);
                }
                var propertyType = ((PropertyInfo) member).PropertyType;
                var name = string.Empty;
                var column2 = table2.Columns[member.Name];
                if (column2 != null)
                {
                    name = column2.Name;
                }
                else
                {
                    name = Context.Columns[member.Name].Name;
                }
                column = new Column
                {
                    DataType = propertyType,
                    Name = name,
                    Table = table3,
                    MemberInfo = member
                };
                while (_memberInfos.Count > 0)
                {
                    var expression2 = _memberInfos.Pop();
                    if (expression2.NodeType != ExpressionType.MemberAccess)
                    {
                        throw new Exception();
                    }
                    var memberInfo = ((MemberExpression) expression2).Member;
                    column.Converters.Push(new ColumnConverter(memberInfo, new List<object>()));
                }
            }
            else
            {
                var table = GetTable(node.Type);
                var info = ((MemberExpression) _memberInfos.Pop()).Member;
                column.DataType = ((PropertyInfo) info).PropertyType;
                column.Name = table.Columns[info.Name].Name;
                column.MemberInfo = info;
                var alias = node.Name;
                if (_joins != null)
                {
                    if (_joins.ContainsKey(alias))
                    {
                        alias = _joins[alias].Right.Table.Alias;
                    }
                }
                else
                {
                    alias = table.Name;
                }
                column.Table = CreateTable(alias, table.DataBase, table.Name, table.Type);
                while (_memberInfos.Count > 0)
                {
                    var expression = _memberInfos.Pop();
                    if (expression.NodeType != ExpressionType.MemberAccess)
                    {
                        throw new Exception();
                    }
                    var info2 = ((MemberExpression) expression).Member;
                    column.Converters.Push(new ColumnConverter(info2, new List<object>()));
                }
            }
            Token = Token.Create(column);
            return node;
        }
    }
}