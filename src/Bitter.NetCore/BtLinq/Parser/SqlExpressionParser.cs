using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BT.Manage.Core
{
    internal class SqlExpressionParser : ExpressionVisitor
    {
        private readonly List<Expression> _joinExpressions = new List<Expression>();
        private readonly List<Expression> _whereExpressions = new List<Expression>();
        private Dictionary<string, Expression> _aggregationExpressions;
        private TranslateContext _context;
        private List<KeyValuePair<string, Expression>> _expression = new List<KeyValuePair<string, Expression>>();
        private List<Expression> _nolockExpressions;
        private Expression _selectExpression;
        private int _skip = -1;
        private List<KeyValuePair<string, Expression>> _sortExpressions;
        private int _take = -1;
        private Expression _updateExpression;

        public Dictionary<string, Column> AggregationColumns { get; private set; }


        public List<Column> Columns { get; private set; }

        public IList<Token> Conditions { get; private set; }

        public bool Distinct { get; private set; }

        public Type ElementType { get; set; }

        public bool IsCallAny { get; private set; }


        public bool IsDelete { get; private set; }


        public bool IsUpdate { get; private set; }

        public Dictionary<string, Join> Joins { get; private set; }

        public List<string> NoLockTables { get; private set; }

        public ParseResult Result { get; private set; }

        public int Skip
        {
            get { return _skip; }
        }


        public List<KeyValuePair<string, Column>> SortColumns { get; private set; }

        public int Take
        {
            get { return _take; }
        }


        public Dictionary<string, object> UpdateResult { get; private set; }

        public static string GetTableName(SchemaModel.Table table)
        {
            var str = string.Empty;
            return @"{str}[{table.Name}]";
        }

        public static string GetTableName(Table table)
        {
            var str = string.Empty;
            return @"{str}[{table.Name}]";
        }

        internal void Parse(Expression expression)
        {
            Distinct = false;
            IsCallAny = false;
            IsDelete = false;
            IsUpdate = false;
            UpdateResult = new Dictionary<string, object>();
            _aggregationExpressions = new Dictionary<string, Expression>();
            Conditions = new List<Token>();
            _nolockExpressions = new List<Expression>();
            NoLockTables = new List<string>();
            AggregationColumns = new Dictionary<string, Column>();
            _sortExpressions = new List<KeyValuePair<string, Expression>>();
            SortColumns = new List<KeyValuePair<string, Column>>();
            _context = new TranslateContext();
            _take = -1;
            _skip = -1;
            _context.EntityType = ElementType;
            Visit(expression);
            if (AggregationColumns.Count > 1)
            {
                throw new Exception();
            }
            if ((_skip != -1) && (_take != -1) &&
                !_sortExpressions.Any())
            {
                throw new Exception("分页必须进行排序");
            }
            foreach (MethodCallExpression expression2 in _joinExpressions)
            {
                VisitJoinExpression(expression2);
                break;
            }
            if (_selectExpression != null)
            {
                var operand = (_selectExpression as UnaryExpression).Operand as LambdaExpression;
                VisitSelectExpression(operand.Body);
            }
            else
            {
                VisitSelectExpression(null);
            }
            using (var enumerator = _whereExpressions.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var body =
                        ((((MethodCallExpression) enumerator.Current).Arguments[1] as UnaryExpression).Operand as
                            LambdaExpression).Body;
                    if (body is ConstantExpression)
                    {
                        if (!(bool) (body as ConstantExpression).Value)
                        {
                            Conditions.Add(Token.Create(false));
                        }
                    }
                    else
                    {
                        VisitWhereExpression(body);
                    }
                }
            }
            foreach (var expression5 in _nolockExpressions)
            {
                VisitNoLockExpression(expression5);
            }
            foreach (var pair in _aggregationExpressions)
            {
                VisitAggreationExpression(pair);
            }
            foreach (var pair2 in _sortExpressions)
            {
                VisitSortExpression(pair2);
            }
            VisitUpdateExpression(_updateExpression);
        }

        private void VisitAggreationExpression(KeyValuePair<string, Expression> aggreationExpression)
        {
            if (aggreationExpression.Value != null)
            {
                var visitor = new MemberExpressionVisitor(_context);
                visitor.Visit(aggreationExpression.Value);
                if (visitor.Token.Type != TokenType.Column)
                {
                    throw new Exception("只能针对列进行聚合操作");
                }
                AggregationColumns.Add(aggreationExpression.Key, visitor.Token.Column);
            }
            else
            {
                AggregationColumns.Add(aggreationExpression.Key, null);
            }
        }

        private void VisitJoinExpression(MethodCallExpression node)
        {
            var visitor = new JoinExpressionVisitor(_context);
            visitor.Visit(node);
            Joins = visitor.Joins;
            if (_selectExpression == null)
            {
                _selectExpression = visitor.SelectExpression;
            }
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            Expression expression;
            string str;
            Expression expression2;
            str = node.Method.Name;
            switch (str)
            {
                case "DefaultIfEmpty":
                    expression = node.Arguments[0];
                    if (expression.NodeType == ExpressionType.Call)
                    {
                        node = (MethodCallExpression) expression;
                        return VisitMethodCall(node);
                    }
                    break;
                case "OrderByDescending":
                    _sortExpressions.Add(new KeyValuePair<string, Expression>("DESC", node.Arguments[1]));
                    expression = node.Arguments[0];
                    if (expression.NodeType == ExpressionType.Call)
                    {
                        node = (MethodCallExpression) expression;
                        return VisitMethodCall(node);
                    }
                    break;
                case "Join":
                    break;
                case "OrderBy":
                    _sortExpressions.Add(new KeyValuePair<string, Expression>("ASC", node.Arguments[1]));
                    expression = node.Arguments[0];
                    if (expression.NodeType == ExpressionType.Call)
                    {
                        node = (MethodCallExpression) expression;
                        return VisitMethodCall(node);
                    }
                    break;
                case "SelectMany":
                    break;
                case "ThenBy":
                    _sortExpressions.Add(new KeyValuePair<string, Expression>("ASC", node.Arguments[1]));
                    expression = node.Arguments[0];
                    if (expression.NodeType == ExpressionType.Call)
                    {
                        node = (MethodCallExpression) expression;
                        return VisitMethodCall(node);
                    }
                    break;

                case "Select":
                    _selectExpression = node.Arguments[1];
                    expression = node.Arguments[0];
                    if (expression.NodeType == ExpressionType.Call)
                    {
                        node = (MethodCallExpression) expression;
                        return VisitMethodCall(node);
                    }
                    break;

                case "Sum":
                    expression2 = null;
                    if (node.Arguments.Count >= 2)
                    {
                        if (node.Method.Name == "Count")
                        {
                            _whereExpressions.Add(node);
                        }
                        else
                        {
                            expression2 = node.Arguments[1];
                        }
                    }
                    _aggregationExpressions.Add(node.Method.Name, expression2);
                    expression = node.Arguments[0];
                    if (expression.NodeType == ExpressionType.Call)
                    {
                        node = (MethodCallExpression) expression;
                        return VisitMethodCall(node);
                    }
                    break;
                case "Average":

                    expression2 = null;
                    if (node.Arguments.Count >= 2)
                    {
                        if (node.Method.Name == "Count")
                        {
                            _whereExpressions.Add(node);
                        }
                        else
                        {
                            expression2 = node.Arguments[1];
                        }
                    }
                    _aggregationExpressions.Add(node.Method.Name, expression2);
                    expression = node.Arguments[0];
                    if (expression.NodeType == ExpressionType.Call)
                    {
                        node = (MethodCallExpression) expression;
                        return VisitMethodCall(node);
                    }
                    break;

                case "Delete":

                    IsDelete = true;
                    if (node.Arguments.Count > 1)
                    {
                        _whereExpressions.Add(node);
                    }
                    expression = node.Arguments[0];
                    if (expression.NodeType == ExpressionType.Call)
                    {
                        node = (MethodCallExpression) expression;
                        return VisitMethodCall(node);
                    }
                    break;

                case "FirstOrDefault":

                    if (node.Arguments.Count > 1)
                    {
                        _whereExpressions.Add(node);
                    }
                    expression = node.Arguments[0];
                    if (expression.NodeType == ExpressionType.Call)
                    {
                        node = (MethodCallExpression) expression;
                        return VisitMethodCall(node);
                    }
                    break;

                case "Take":
                    _take = Convert.ToInt32(((ConstantExpression) node.Arguments[1]).Value);
                    expression = node.Arguments[0];
                    if (expression.NodeType == ExpressionType.Call)
                    {
                        node = (MethodCallExpression) expression;
                        return VisitMethodCall(node);
                    }
                    break;

                case "Any":
                    IsCallAny = true;
                    if (node.Arguments.Count > 1)
                    {
                        _whereExpressions.Add(node);
                    }
                    expression = node.Arguments[0];
                    if (expression.NodeType == ExpressionType.Call)
                    {
                        node = (MethodCallExpression) expression;
                        return VisitMethodCall(node);
                    }
                    break;

                case "Where":

                    _whereExpressions.Add(node);
                    expression = node.Arguments[0];
                    if (expression.NodeType == ExpressionType.Call)
                    {
                        node = (MethodCallExpression) expression;
                        return VisitMethodCall(node);
                    }
                    break;

                case "LongCount":
                    expression2 = null;
                    if (node.Arguments.Count >= 2)
                    {
                        if (node.Method.Name == "Count")
                        {
                            _whereExpressions.Add(node);
                        }
                        else
                        {
                            expression2 = node.Arguments[1];
                        }
                    }
                    _aggregationExpressions.Add(node.Method.Name, expression2);
                    expression = node.Arguments[0];
                    if (expression.NodeType == ExpressionType.Call)
                    {
                        node = (MethodCallExpression) expression;
                        return VisitMethodCall(node);
                    }

                    break;

                case "Update":

                    IsUpdate = true;
                    _updateExpression = node.Arguments[1];
                    expression = node.Arguments[0];
                    if (expression.NodeType == ExpressionType.Call)
                    {
                        node = (MethodCallExpression) expression;
                        return VisitMethodCall(node);
                    }
                    break;

                case "ThenByDescending":
                    _sortExpressions.Add(new KeyValuePair<string, Expression>("DESC", node.Arguments[1]));
                    expression = node.Arguments[0];
                    if (expression.NodeType == ExpressionType.Call)
                    {
                        node = (MethodCallExpression) expression;
                        return VisitMethodCall(node);
                    }
                    break;

                case "Skip":
                    _skip = Convert.ToInt32(((ConstantExpression) node.Arguments[1]).Value);
                    expression = node.Arguments[0];
                    if (expression.NodeType == ExpressionType.Call)
                    {
                        node = (MethodCallExpression) expression;
                        return VisitMethodCall(node);
                    }
                    break;

                case "Distinct":
                    Distinct = true;
                    expression = node.Arguments[0];
                    if (expression.NodeType == ExpressionType.Call)
                    {
                        node = (MethodCallExpression) expression;
                        return VisitMethodCall(node);
                    }
                    break;

                case "Count":
                    expression2 = null;
                    if (node.Arguments.Count >= 2)
                    {
                        if (node.Method.Name == "Count")
                        {
                            _whereExpressions.Add(node);
                        }
                        else
                        {
                            expression2 = node.Arguments[1];
                        }
                    }
                    _aggregationExpressions.Add(node.Method.Name, expression2);
                    expression = node.Arguments[0];
                    if (expression.NodeType == ExpressionType.Call)
                    {
                        node = (MethodCallExpression) expression;
                        return VisitMethodCall(node);
                    }
                    break;

                case "NoLock":

                    _nolockExpressions.Add(node);
                    expression = node.Arguments[0];
                    if (expression.NodeType == ExpressionType.Call)
                    {
                        node = (MethodCallExpression) expression;
                        return VisitMethodCall(node);
                    }
                    break;

                case "GroupJoin":
                    break;


                case "First":
                    if (node.Arguments.Count > 1)
                    {
                        _whereExpressions.Add(node);
                    }
                    expression = node.Arguments[0];
                    if (expression.NodeType == ExpressionType.Call)
                    {
                        node = (MethodCallExpression) expression;
                        return VisitMethodCall(node);
                    }
                    break;

                default:
                    throw new NotSupportedException("未支持的方法：" + node.Method.Name);
                    break;
            }
            _joinExpressions.Add(node);
            expression = node.Arguments[0];
            if (expression.NodeType == ExpressionType.Call)
            {
                node = (MethodCallExpression) expression;
                return VisitMethodCall(node);
            }

            return node;
        }


        private void VisitNoLockExpression(Expression node)
        {
            var visitor = new NoLockExpressionVisitor(_context);
            visitor.Visit(node);
            NoLockTables.Add(visitor.ExtraObject.ToString());
        }

        private void VisitSelectExpression(Expression node)
        {
            var visitor = new SelectExpressionVisitor(_context);
            visitor.Visit(node);
            Columns = visitor.Columns;
        }

        private void VisitSortExpression(KeyValuePair<string, Expression> sortExpression)
        {
            var visitor = new MemberExpressionVisitor(_context);
            visitor.Visit(sortExpression.Value);
            if (visitor.Token.Type != TokenType.Column)
            {
                throw new Exception();
            }
            SortColumns.Add(new KeyValuePair<string, Column>(sortExpression.Key, visitor.Token.Column));
        }

        private void VisitUpdateExpression(Expression updateExpression)
        {
            if (updateExpression != null)
            {
                var visitor = new MemberExpressionVisitor(_context);
                visitor.Visit(updateExpression);
                if (visitor.Token.Type != TokenType.Object)
                {
                    throw new NotSupportedException("不支持");
                }
                UpdateResult = (Dictionary<string, object>) visitor.Token.Object;
            }
        }

        private void VisitWhereExpression(Expression binaryExpression)
        {
            var visitor = new WhereExpressionVisitor(_context);
            visitor.Visit(binaryExpression);
            Conditions.Add(visitor.Token);
        }
    }
}