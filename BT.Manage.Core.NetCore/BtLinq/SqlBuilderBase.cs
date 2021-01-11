using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BT.Manage.Core
{
    public abstract class SqlBuilderBase
    {
        protected BuilderContext _context;
        private ProviderBase _provider;
        protected ParseResult _result;
        protected string tableName;

        public SqlBuilderBase(ProviderBase provider)
        {
            this._provider = provider;
        }

        protected string BuildCondition(Condition condition)
        {
            var left = condition.Left;
            var str = string.Empty;
            if (condition.CompareType == CompareType.Not)
            {
                str = BuildSql(left);
                if ((left.Type == TokenType.Column) &&
                    (TypeHelper.GetUnderlyingType(left.Column.DataType) == ReflectorConsts.BoolType))
                {
                    return str + " = 0";
                }
                return @"NOT ({str})";
            }
            var str2 = BuildSql(left);
            var str3 = BuildSql(condition.Right);
            if (str2 == null)
            {
                return @"({str3} IS NULL)";
            }
            if (str3 == null)
            {
                return @"({str2} IS NULL)";
            }
            return @"({str2} {this.SelectOperation(condition.CompareType)} {str3})";
        }

        private void BuildDeleteSql()
        {
            var str = string.Empty;
            var table = EntityConfigurationManager.GetTable(_context.ElementType);
            this.tableName = table.Name;
            var tableName = GetTableName(table);
            if (_context.Conditions.Any())
            {
                str = BuildWhere(_context.Conditions);
            }
            var format = "DELETE FROM {0} {1}";
            format = string.Format(format, tableName, str);
            _result.CommandText = format;
        }

        protected abstract void BuildSelectSql();

        public ParseResult BuildSql(BuilderContext context)
        {
            _context = context;
            _result = new ParseResult();
            switch (context.SqlType)
            {
                case SqlType.Select:
                    BuildSelectSql();
                    break;

                case SqlType.Update:
                    BuildUpdateSql();
                    break;

                case SqlType.Delete:
                    BuildDeleteSql();
                    break;

                default:
                    throw new Exception();
            }
            return _result;
        }

        protected string BuildSql(Column column)
        {
            var str = @"[{this.GetTableAlias(column)}].[{column.Name}]";
            var str2 = string.Empty;
            if (!column.Converters.Any())
            {
                return str;
            }
            str2 = ParseConverter(column);
            if (string.IsNullOrWhiteSpace(str2))
            {
                return "1=2";
            }
            return string.Format(str2, str);
        }

        protected string BuildSql(Token token)
        {
            switch (token.Type)
            {
                case TokenType.Column:
                    return BuildSql(token.Column);

                case TokenType.Object:
                    if (token.Object != null)
                    {
                        if (token.IsBool())
                        {
                            if (!token.GetBool())
                            {
                                return "0";
                            }
                            return "1";
                        }
                        var key = ParserUtils.GenerateAlias("param");
                        _result.Parameters.Add(key, token.Object);
                        return "@" + key;
                    }
                    return null;

                case TokenType.Condition:
                    return BuildCondition(token.Condition);
            }
            throw new Exception();
        }

        private void BuildUpdateSql()
        {
            var str = string.Empty;
            var table = EntityConfigurationManager.GetTable(_context.ElementType);
            var name = string.Empty;
            var column =
                table.Columns.FirstOrDefault(x => x.Value.IsKey)
                    .Value;
            if (column != null)
            {
                name = column.PropertyInfo.Name;
            }
            this.tableName = table.Name;
            var tableName = GetTableName(table);
            if (_context.Conditions.Any())
            {
                str = BuildWhere(_context.Conditions);
            }
            var values = new List<string>();
            var str4 = string.Empty;
            foreach (var str6 in _context.UpdateResult.Keys)
            {
                if (str6 != name)
                {
                    str4 = ParserUtils.GenerateAlias(str6);
                    var item = @"[{str6}] = @{str4}";
                    _result.Parameters.Add(str4, _context.UpdateResult[str6]);
                    values.Add(item);
                }
            }
            var format = "UPDATE {0} SET {1} {2}";
            format = string.Format(format, tableName, string.Join(",", values), str);
            _result.CommandText = format;
        }

        protected string BuildWhere(IList<Token> conditions)
        {
            var builder = new StringBuilder();
            if (conditions.Any())
            {
                var source = new List<string>();
                foreach (var token in conditions)
                {
                    var str = StartBuildCondition(token);
                    if (!string.IsNullOrWhiteSpace(str))
                    {
                        source.Add(str);
                    }
                }
                if (source.Any())
                {
                    builder.Append("WHERE ");
                    builder.Append(string.Join(" AND ", source));
                }
            }
            return builder.ToString();
        }

        protected string FormatColumn(Column column, bool genColumnAlias = true)
        {
            var tableAlias = GetTableAlias(column);
            var str2 = @"[{tableAlias}].[{column.Name}]";
            var str3 = ParseConverter(column);
            if (!string.IsNullOrWhiteSpace(str3))
            {
                str2 = string.Format(str3, @"[{tableAlias}].[{column.Name}]");
            }
            if (!genColumnAlias)
            {
                return str2;
            }
            return string.Format("{0} [{1}]" + Environment.NewLine, str2, column.Alias ?? column.MemberInfo.Name);
        }

        protected virtual string FormatSelectString(List<Column> columns)
        {
            if (_context.AggregationColumns.Count >= 1)
            {
                _context.SortColumns.Clear();
                var pair =
                    _context.AggregationColumns.FirstOrDefault();
                var str = string.Empty;
                if (pair.Value != null)
                {
                    str = FormatColumn(pair.Value, false);
                }
                var key = pair.Key;
                switch (key)
                {
                    case "Count":
                        if (pair.Value == null)
                        {
                            return "Count(1)";
                        }
                        return @"Count({str})";

                    case "Sum":
                        return @"Sum({str})";
                }
                if (key != "Average")
                {
                    throw new Exception(pair.Key);
                }
                return @"AVG({str})";
            }
            var values = new List<string>();
            foreach (var column in columns)
            {
                var item = FormatColumn(column, true);
                values.Add(item);
            }
            return string.Join(",", values);
        }

        protected string FormatSortColumns()
        {
            var builder = new StringBuilder();
            if (_context.SortColumns.Any())
            {
                builder.Append("ORDER BY ");
                var values = new List<string>();
                foreach (var pair in _context.SortColumns)
                {
                    var tableAlias = GetTableAlias(pair.Value);
                    var str = @"[{tableAlias}].[{pair.Value.Alias ?? pair.Value.Name}]";
                    var str2 = ParseConverter(pair.Value);
                    if (!string.IsNullOrWhiteSpace(str2))
                    {
                        str = string.Format(str2, str);
                    }
                    values.Add(str);
                }
                builder.Append(string.Join(",", values));
            }
            return builder.ToString();
        }

        protected string GenJoinType(JoinType joinType)
        {
            if (joinType == JoinType.Inner)
            {
                return " INNER JOIN ";
            }
            if (joinType != JoinType.Left)
            {
                throw new NotSupportedException("未支持的Join类型：" + joinType);
            }
            return " Left JOIN ";
        }

        protected string GetTableAlias(Column column)
        {
            if (!string.IsNullOrWhiteSpace(tableName))
            {
                return tableName;
            }
            if (!string.IsNullOrWhiteSpace(column.Table.Alias))
            {
                return column.Table.Alias;
            }
            var name = column.Name;
            foreach (var join in _context.Joins.Values)
            {
                Dictionary<string, SchemaModel.Column>.ValueCollection.Enumerator enumerator2;
                if (join.Left.Name == name)
                {
                    return join.Left.Table.Alias;
                }
                if (join.Right.Name == name)
                {
                    return join.Right.Table.Alias;
                }
                var table = EntityConfigurationManager.GetTable(join.Left.Table.Type);
                using (enumerator2 = table.Columns.Values.GetEnumerator())
                {
                    while (enumerator2.MoveNext())
                    {
                        if (enumerator2.Current.Name == name)
                        {
                            return @join.Left.Table.Name == table.Name
                                ? @join.Left.Table.Alias
                                : @join.Right.Table.Alias;
                        }
                    }
                }
                table = EntityConfigurationManager.GetTable(join.Right.Table.Type);
                using (enumerator2 = table.Columns.Values.GetEnumerator())
                {
                    while (enumerator2.MoveNext())
                    {
                        if (enumerator2.Current.Name == name)
                        {
                            return @join.Left.Table.Name == table.Name
                                ? @join.Left.Table.Alias
                                : @join.Right.Table.Alias;
                        }
                    }
                }
            }
            throw new Exception();
        }

        public virtual string GetTableName(string tableName)
        {
            return @"[{tableName}]";
        }

        public abstract string GetTableName(SchemaModel.Table table);
        public abstract string GetTableName(Table leftTable);
       
        public abstract string ParseConverter(Column column);

        protected string SelectOperation(CompareType compareType)
        {
            switch (compareType)
            {
                case CompareType.And:
                    return "AND";

                case CompareType.Or:
                    return "OR";

                case CompareType.Equal:
                    return "=";

                case CompareType.GreaterThan:
                    return ">";

                case CompareType.GreaterThanOrEqual:
                    return ">=";

                case CompareType.LessThan:
                    return "<";

                case CompareType.LessThanOrEqual:
                    return "<=";

                case CompareType.NotEqual:
                    return "<>";

                case CompareType.Add:
                    return "+";

                case CompareType.Substarct:
                    return "-";

                case CompareType.Multiply:
                    return "*";

                case CompareType.Divide:
                    return "/";
            }
            throw new Exception();
        }

        protected string StartBuildCondition(Token token)
        {
            switch (token.Type)
            {
                case TokenType.Column:
                {
                    var str = BuildSql(token.Column);
                    if (TypeHelper.GetUnderlyingType(token.Column.DataType) == ReflectorConsts.BoolType)
                    {
                        str = str + " = 1";
                    }
                    return str;
                }
                case TokenType.Object:
                    if (token.Object != null)
                    {
                        if (token.IsBool())
                        {
                            if (!token.GetBool())
                            {
                                return "1=2";
                            }
                            return null;
                        }
                        var key = ParserUtils.GenerateAlias("param");
                        _result.Parameters.Add(key, token.Object);
                        return "@" + key;
                    }
                    return null;

                case TokenType.Condition:
                    return BuildCondition(token.Condition);
            }
            throw new Exception();
        }
    }
}