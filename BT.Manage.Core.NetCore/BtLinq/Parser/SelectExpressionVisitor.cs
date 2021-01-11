using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace BT.Manage.Core
{
    public class SelectExpressionVisitor : ExpressionVisitorBase
    {
        private readonly Type _elementType;
        private readonly Stack<Expression> _memberExpressions;
        private Dictionary<string, Join> _Joins;

        public SelectExpressionVisitor(TranslateContext context) : base(context)
        {
            _memberExpressions = new Stack<Expression>();
            _elementType = context.EntityType;
            _Joins = context.Joins;
            Columns = new List<Column>();
        }

        public List<Column> Columns { get; private set; }

        private void ParseEntityType(Type type)
        {
            var table = EntityConfigurationManager.GetTable(type);
            var table2 = new Table
            {
                DataBase = table.DataBase,
                Name = table.Name,
                Type = table.Type
            };
            foreach (var column in table.Columns.Values)
            {
                var item = new Column
                {
                    Name = column.Name,
                    DataType = column.PropertyInfo.PropertyType,
                    MemberInfo = column.PropertyInfo,
                    Table = table2
                };
                Columns.Add(item);
                Context.Columns.Add(column.Name, item);
            }
        }

        public override Expression Visit(Expression node)
        {
            if (node == null)
            {
                ParseEntityType(_elementType);
                return node;
            }
            _memberExpressions.Push(node);
            return base.Visit(node);
        }

        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            foreach (MemberAssignment assignment in node.Bindings)
            {
                var visitor = new MemberExpressionVisitor(Context);
                visitor.Visit(assignment.Expression);
                if ((visitor.SelectedColumn != null) &&
                    (assignment.Member.GetCustomAttribute(ReflectorConsts.NonSelectAttributeType, false) ==
                     null))
                {
                    visitor.SelectedColumn.Alias = assignment.Member.Name;
                    Columns.Add(visitor.SelectedColumn);
                    Context.Columns.Add(assignment.Member.Name, visitor.SelectedColumn);
                }
            }
            return node;
        }

        protected override Expression VisitNew(NewExpression node)
        {
            for (var i = 0; i < node.Arguments.Count; i++)
            {
                var visitor = new MemberExpressionVisitor(Context);
                var expression = node.Arguments[i];
                var info = node.Members[i];
                visitor.Visit(expression);
                visitor.SelectedColumn.Alias = info.Name;
                Columns.Add(visitor.SelectedColumn);
                Context.Columns.Add(info.Name, visitor.SelectedColumn);
            }
            return node;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            var type = node.Type;
            var expression = _memberExpressions.LastOrDefault();
            if (!TypeHelper.IsValueType(expression.Type))
            {
                while (!EntityConfigurationManager.IsEntity(type))
                {
                    type = _memberExpressions.Pop().Type;
                }
                ParseEntityType(type);
                return node;
            }
            var visitor = new MemberExpressionVisitor(Context);
            visitor.Visit(expression);
            Columns.Add(visitor.SelectedColumn);
            return node;
        }

        private class MemberExpressionVisitor : ExpressionVisitorBase
        {
            private readonly Dictionary<string, Join> _Joins;
            private readonly Stack<MemberInfo> _memberInfoStack;
            private MemberInfo _tableMember;

            public MemberExpressionVisitor(TranslateContext context) : base(context)
            {
                _memberInfoStack = new Stack<MemberInfo>();
                _Joins = context.Joins;
                SelectedColumn = new Column();
            }

            public Column SelectedColumn { get; private set; }

            public object Value { get; set; }

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

            private string GetConverter()
            {
                if (_memberInfoStack.Count <= 0)
                {
                    return null;
                }
                var info = (PropertyInfo) _memberInfoStack.Pop();
                if (info.DeclaringType.FullName.StartsWith("System.Nullable") &&
                    info.DeclaringType.Assembly.GlobalAssemblyCache)
                {
                    if (_memberInfoStack.Count <= 0)
                    {
                        return null;
                    }
                    info = (PropertyInfo) _memberInfoStack.Pop();
                }
                if ((info.DeclaringType != typeof (DateTime)) || (info.Name != "Date"))
                {
                    throw new Exception("除x=>x.Time.Date以外，不支持其他的");
                }
                return "CONVERT(DATE,{0},211)";
            }

            private SchemaModel.Table GetTable()
            {
                _tableMember = _memberInfoStack.Pop();
                var propertyType = ((PropertyInfo) _tableMember).PropertyType;
                return GetTable(propertyType);
            }

            private SchemaModel.Table GetTable(Type tableType)
            {
                if (ParserUtils.IsAnonymousType(tableType))
                {
                    _tableMember = _memberInfoStack.Pop();
                    tableType = ((PropertyInfo) _tableMember).PropertyType;
                    return GetTable(tableType);
                }
                return EntityConfigurationManager.GetTable(tableType);
            }

            public object GetValue(MemberInfo memberInfo, object obj)
            {
                if (memberInfo.MemberType == MemberTypes.Field)
                {
                    return ((FieldInfo) memberInfo).GetValue(obj);
                }
                return ((PropertyInfo) memberInfo).GetValue(obj);
            }

            protected override Expression VisitConstant(ConstantExpression node)
            {
                throw new NotSupportedException("Select子句中不允许直接写常量");
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                _memberInfoStack.Push(node.Member);
                if (node.Expression == null)
                {
                    throw new NotSupportedException("Select子句中不允许调用方法");
                }
                if (node.Expression.NodeType == ExpressionType.Constant)
                {
                    return node;
                }
                return base.VisitMember(node);
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (EntityConfigurationManager.IsEntity(node.Type))
                {
                    var table = GetTable(node.Type);
                    var info = _memberInfoStack.Pop();
                    if (table.Columns[info.Name] == null)
                    {
                        SelectedColumn = null;
                        return node;
                    }
                    SelectedColumn.DataType = ((PropertyInfo) info).PropertyType;
                    SelectedColumn.Name = table.Columns[info.Name].Name;
                    SelectedColumn.MemberInfo = info;
                    var key = node.Name;
                    if (_Joins != null)
                    {
                        if (_Joins.ContainsKey(key))
                        {
                            key = _Joins[key].Right.Table.Alias;
                        }
                    }
                    else
                    {
                        key = table.Name;
                    }
                    SelectedColumn.Table = CreateTable(key, table.DataBase, table.Name, table.Type);
                    while (_memberInfoStack.Count > 0)
                    {
                        var memberInfo = _memberInfoStack.Pop();
                        SelectedColumn.Converters.Push(new ColumnConverter(memberInfo, new List<object>()));
                    }
                    return node;
                }
                var table2 = GetTable();
                var info3 = _memberInfoStack.Pop();
                if (table2.Columns[info3.Name] == null)
                {
                    SelectedColumn = null;
                    return node;
                }
                var propertyType = ((PropertyInfo) info3).PropertyType;
                var name = _tableMember.Name;
                if (_Joins != null)
                {
                    if (_Joins.ContainsKey(name))
                    {
                        name = _Joins[name].Right.Table.Alias;
                    }
                }
                else
                {
                    name = table2.Name;
                }
                var table3 = CreateTable(name, table2.DataBase, table2.Name, table2.Type);
                var column1 = new Column
                {
                    DataType = propertyType,
                    Name = table2.Columns[info3.Name].Name,
                    Table = table3,
                    MemberInfo = info3
                };
                SelectedColumn = column1;
                while (_memberInfoStack.Count > 0)
                {
                    var info4 = _memberInfoStack.Pop();
                    SelectedColumn.Converters.Push(new ColumnConverter(info4, new List<object>()));
                }
                return node;
            }
        }
    }
}