using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using BT.Manage.Core;

namespace BT.Manage.Core.Provider.MSSQLServer
{
    public class SqlBuilder : SqlBuilderBase
    {
        public SqlBuilder(ProviderBase provider) : base(provider)
        {
        }

        private string BuildJoinSql()
        {
            var joins = _context.Joins;
            var join = joins.Values.FirstOrDefault();
            var left = join.Left;
            var leftTable = left.Table;
            var builder = new StringBuilder();
            builder.Append(GetTableName(leftTable));
            builder.AppendFormat(" [{0}]", leftTable.Alias);
            builder.Append(BuildNoLockSql(leftTable.Name));
            builder.Append(GenJoinType(join.JoinType));
            var right = join.Right;
            var table = right.Table;
            builder.Append(GetTableName(table));
            builder.AppendFormat(" [{0}]", table.Alias);
            builder.Append(BuildNoLockSql(table.Name));
            builder.AppendLine();
            builder.Append(" ON ");
            builder.AppendFormat("[{0}].[{1}]", leftTable.Alias, left.Name);
            builder.AppendFormat(" = ");
            builder.AppendFormat("[{0}].[{1}]" + Environment.NewLine, table.Alias, right.Name);
            builder.AppendLine();
            foreach (var join2 in joins.Values.Skip(1))
            {
                left = join2.Left;
                leftTable = left.Table;
                builder.Append(GenJoinType(join2.JoinType));
                right = join2.Right;
                table = right.Table;
                builder.Append(GetTableName(table));
                builder.AppendFormat(" [{0}]", table.Alias);
                builder.Append(BuildNoLockSql(table.Name));
                builder.AppendLine();
                builder.Append(" ON ");
                builder.AppendFormat("[{0}].[{1}]", leftTable.Alias, left.Name);
                builder.AppendFormat(" = ");
                builder.AppendFormat("[{0}].[{1}]" + Environment.NewLine, table.Alias, right.Name);
                builder.AppendLine();
            }
            return builder.ToString();
        }

        private string BuildNoLockSql(string tableName)
        {
            if (_context.NoLockTables.Contains(tableName))
            {
                return " WITH (NOLOCK) ";
            }
            return string.Empty;
        }

        private void BuildPageSql()
        {
            var columns = _context.Columns;
            var conditions = _context.Conditions;
            var joins = _context.Joins;
            var builder = new StringBuilder("FROM ");
            var builder2 = new StringBuilder("SELECT ");
            if (_context.Distinct)
            {
                builder2.Append("DISTINCT ");
            }
            var builder3 = new StringBuilder();
            var builder4 = new StringBuilder();
            var builder5 = new StringBuilder();
            if ((joins != null) && (joins.Count > 0))
            {
                builder.Append(BuildJoinSql());
                var str2 = @"{str2},{" + @"ROW_NUMBER() OVER({base.FormatSortColumns()}) #index" + "}";

                builder2.AppendLine(str2);
                if (conditions.Any())
                {
                    builder3.Append(BuildWhere(conditions));
                }
                builder5.Clear();
                object[] args = {builder2.ToString(), builder.ToString(), builder3.ToString(), builder4.ToString()};
                builder5.AppendFormat("{0} {1} {2} {3}", args);
            }
            else
            {
                builder = new StringBuilder("FROM ");
                var leftTable = columns.FirstOrDefault().Table;
                builder.Append(GetTableName(leftTable));
                tableName = ParserUtils.GenerateAlias(leftTable.Name);
                builder.AppendFormat(" [{0}]", tableName);
                if (_context.NoLockTables.Contains(leftTable.Name))
                {
                    builder.Append(" WITH (NOLOCK) ");
                }
                var str3 = FormatSelectString(columns);
                if (!_context.Distinct)
                {
                    str3 = @"{str3},{" + @"ROW_NUMBER() OVER({base.FormatSortColumns()}) #index" + @"}";

                }
                builder2.Append(str3);
                if (conditions.Any())
                {
                    builder3.Append(BuildWhere(conditions));
                }
                builder5.Clear();
                object[] objArray2 = {builder2.ToString(), builder.ToString(), builder3.ToString(), builder4.ToString()};
                builder5.AppendFormat("{0} {1} {2} {3}", objArray2);
            }
            var str = builder5.ToString();
            builder5.Clear();
            builder2.Clear();
            builder3.Clear();
            builder.Clear();
            if (_context.Distinct)
            {
                tableName = ParserUtils.GenerateAlias("table");
                builder.AppendFormat("FROM ({0}) {1}", str, tableName);
                builder2.Append("SELECT ");
                var values = new List<string>();
                foreach (var column in columns)
                {
                    values.Add(@"[{base.tableName}].[{column.Alias ?? column.MemberInfo.Name}]");
                }
                values.Add(@"ROW_NUMBER() OVER({base.FormatSortColumns()}) #index");
                builder2.Append(string.Join(",", values));
                builder5.AppendFormat("{0} {1}", builder2, builder);
                str = builder5.ToString();
                builder5.Clear();
                builder.Clear();
            }
            if (_context.Pager)
            {
                builder.AppendFormat("FROM ({0}) [_indexTable]", str);
                var list4 = new List<string>();
                foreach (var column2 in _context.Columns)
                {
                    list4.Add(@"[_indexTable].[{column2.Alias ?? column2.MemberInfo.Name}]");
                }
                builder5.Append("SELECT ");
                object[] objArray3 = {string.Join(",", list4), str, _context.Skip, _context.Take};
                builder5.AppendFormat("{0} FROM ({1}) _indexTable where [_indexTable].[#index] BETWEEN {2} AND {3}",
                    objArray3);
                str = builder5.ToString();
            }
            _result.CommandText = str;
        }

        protected override void BuildSelectSql()
        {
            if (_context.Pager)
            {
                BuildPageSql();
            }
            else
            {
                var columns = _context.Columns;
                var conditions = _context.Conditions;
                var joins = _context.Joins;
                var builder = new StringBuilder("FROM ");
                builder.AppendLine();
                var builder2 = new StringBuilder("SELECT ");
                builder2.AppendLine();
                if (_context.Distinct)
                {
                    builder2.Append(" DISTINCT ");
                }
                else if (_context.Take > 0)
                {
                    builder2.Append(" TOP " + _context.Take + " ");
                }
                var builder3 = new StringBuilder();
                var builder4 = new StringBuilder();
                var builder5 = new StringBuilder();
                if ((joins != null) && (joins.Count > 0))
                {
                    builder.Append(BuildJoinSql());
                    builder2.Append(FormatSelectString(columns));
                    if (conditions.Any())
                    {
                        builder3.Append(BuildWhere(conditions));
                    }
                    if ((_context.Skip == -1) || (_context.Take == -1))
                    {
                        builder4.Append(FormatSortColumns());
                    }
                    builder5.Clear();
                    object[] args = {builder2.ToString(), builder.ToString(), builder3.ToString(), builder4.ToString()};
                    builder5.AppendFormat("{0} {1} {2} {3}", args);
                }
                else
                {
                    builder = new StringBuilder("FROM ");
                    var leftTable = columns.FirstOrDefault().Table;
                    builder.Append(GetTableName(leftTable));
                    tableName = ParserUtils.GenerateAlias(leftTable.Name);
                    builder.AppendFormat(" [{0}]", tableName);
                    if (_context.NoLockTables.Contains(leftTable.Name))
                    {
                        builder.Append(" WITH (NOLOCK) ");
                    }
                    builder2.Append(FormatSelectString(columns));
                    if (conditions.Any())
                    {
                        builder3.Append(BuildWhere(conditions));
                    }
                    if ((_context.Take == -1) || (_context.Skip == -1))
                    {
                        builder4.Append(FormatSortColumns());
                    }
                    builder5.Clear();
                    object[] objArray2 =
                    {
                        builder2.ToString(), builder.ToString(), builder3.ToString(),
                        builder4.ToString()
                    };
                    builder5.AppendFormat("{0} {1} {2} {3}", objArray2);
                }
                var str = builder5.ToString();
                _result.CommandText = str;
            }
        }

        private string FormatConverter(bool isColumnCaller, string rawConverter, string converter, string param)
        {
            if (isColumnCaller)
            {
                converter = string.Format(rawConverter, string.Format(converter, param, "{0}"));
                return converter;
            }
            converter = string.Format(rawConverter, string.Format(converter, "{0}", param));
            return converter;
        }


        public override string GetTableName(Table table)
        {
            var str = string.Empty;
            if (!string.IsNullOrWhiteSpace(table.DataBase))
            {
                str = @"[{table.DataBase}].DBO.";
            }
            return @"{str}{this.GetTableName(table.Name)}";
        }

        public override string ParseConverter(Column column)
        {
            var format = string.Empty;
            if (column.Converters.Any())
            {
                format = "{0}";
            }
            while (column.Converters.Count > 0)
            {
                string name;
                var converter = column.Converters.Pop();
                var memberInfo = converter.MemberInfo;
                var parameters = converter.Parameters;
                var param = "@" + ParserUtils.GenerateAlias("param");
                var memberType = memberInfo.MemberType;
                if (memberType == MemberTypes.Method)
                {
                    goto Label_01E3;
                }
                if (memberType != MemberTypes.Property)
                {
                    throw new Exception();
                }
                if (TypeHelper.IsNullableType(memberInfo.DeclaringType) && (memberInfo.Name == "Value"))
                {
                    continue;
                }
                if ((memberInfo.DeclaringType == ReflectorConsts.DateTimeNullableType) ||
                    (memberInfo.DeclaringType == ReflectorConsts.DateTimeType))
                {
                    name = memberInfo.Name;
                    if (name != "Date")
                    {
                        if (name != "Value")
                        {
                            throw new Exception("不支持");
                        }
                    }
                    else
                    {
                        format = string.Format(format, "CONVERT(DATE,{0},211)");
                    }
                    continue;
                }
                if (!(memberInfo.DeclaringType == ReflectorConsts.TimeSpanType))
                {
                    throw new Exception("不支持");
                }
                var str4 = string.Empty;
                name = memberInfo.Name;
                if (name != "TotalDays")
                {
                    if (name != "TotalHours")
                    {
                        if (name == "TotalMilliseconds")
                        {
                            goto Label_0173;
                        }
                        if (name == "TotalMinutes")
                        {
                            goto Label_017C;
                        }
                        if (name != "TotalSeconds")
                        {
                            throw new Exception("不支持");
                        }
                        goto Label_0185;
                    }
                }
                else
                {
                    str4 = "DAY";
                    format = FormatConverter(converter.IsInstanceColumn, format, "DATEDIFF(" + str4 + ",{1},{0})", param);
                    _result.Parameters.Add(param, parameters[0]);
                    continue;
                }
                str4 = "HOUR";
                format = FormatConverter(converter.IsInstanceColumn, format, "DATEDIFF(" + str4 + ",{1},{0})", param);
                _result.Parameters.Add(param, parameters[0]);
                continue;

                Label_0173:
                str4 = "MILLISECOND";
                format = FormatConverter(converter.IsInstanceColumn, format, "DATEDIFF(" + str4 + ",{1},{0})", param);
                _result.Parameters.Add(param, parameters[0]);
                continue;

                Label_017C:
                str4 = "MINUTE";
                format = FormatConverter(converter.IsInstanceColumn, format, "DATEDIFF(" + str4 + ",{1},{0})", param);
                _result.Parameters.Add(param, parameters[0]);
                continue;
                Label_0185:
                str4 = "SECOND";
                format = FormatConverter(converter.IsInstanceColumn, format, "DATEDIFF(" + str4 + ",{1},{0})", param);
                _result.Parameters.Add(param, parameters[0]);
                continue;
                Label_01E3:
                if (!(memberInfo.DeclaringType == ReflectorConsts.StringType))
                {
                    goto Label_039F;
                }
                name = memberInfo.Name;
                if (name != "Contains")
                {
                    if (name != "StartsWith")
                    {
                        if (name != "Substring")
                        {
                            throw new Exception("不支持");
                        }
                        goto Label_0295;
                    }
                }
                else
                {
                    format = FormatConverter(converter.IsInstanceColumn, format, "CHARINDEX({0},{1})>0", param);
                    _result.Parameters.Add(param, parameters[0]);
                    continue;
                }
                format = FormatConverter(converter.IsInstanceColumn, format, "CHARINDEX({0},{1})=1", param);
                _result.Parameters.Add(param, parameters[0]);
                continue;
                Label_0295:
                if (converter.Parameters.Count == 1)
                {
                    if (!converter.IsInstanceColumn)
                    {
                        throw new Exception("不支持");
                    }
                    format = string.Format(format,
                        string.Concat("SUBSTRING({0},", Convert.ToInt32(converter.Parameters[0]) + 1, ",LEN({0})+1-",
                            converter.Parameters[0], ")"));
                    continue;
                }
                if (converter.Parameters.Count != 2)
                {
                    throw new Exception("不支持");
                }
                if (converter.IsInstanceColumn)
                {
                    format = string.Format(format,
                        string.Concat("SUBSTRING({0},", Convert.ToInt32(converter.Parameters[0]) + 1, ",",
                            converter.Parameters[1], ")"));
                    continue;
                }
                throw new Exception("不支持");
                Label_039F:
                if (!(memberInfo.DeclaringType == ReflectorConsts.DateTimeType) &&
                    !(memberInfo.DeclaringType == ReflectorConsts.DateTimeNullableType))
                {
                    goto Label_064A;
                }
                name = memberInfo.Name;
                switch (name)
                {
                    case "AddHours":

                        format = FormatConverter(converter.IsInstanceColumn, format, "DATEADD(HOUR,{0},{1})", param);
                        _result.Parameters.Add(param, parameters[0]);
                        continue;
                        break;


                    case "AddDays":
                        throw new Exception("不支持");
                        break;

                    case "AddMonths":
                        format = FormatConverter(converter.IsInstanceColumn, format, "DATEADD(MONTH,{0},{1})", param);
                        _result.Parameters.Add(param, parameters[0]);
                        continue;
                        break;

                    case "AddMinutes":
                        format = FormatConverter(converter.IsInstanceColumn, format, "DATEADD(MINUTE,{0},{1})", param);
                        _result.Parameters.Add(param, parameters[0]);
                        continue;
                        break;
                    case "AddYears":
                        format = FormatConverter(converter.IsInstanceColumn, format, "DATEADD(YEAR,{0},{1})", param);
                        _result.Parameters.Add(param, parameters[0]);
                        continue;
                        break;

                    case "AddMilliseconds":
                        format = FormatConverter(converter.IsInstanceColumn, format, "DATEADD(MILLISECOND,{0},{1})",
                            param);
                        _result.Parameters.Add(param, parameters[0]);
                        continue;
                        break;

                    case "AddSeconds":

                        format = FormatConverter(converter.IsInstanceColumn, format, "DATEADD(SECOND,{0},{1})", param);
                        _result.Parameters.Add(param, parameters[0]);
                        continue;
                        break;


                    default:
                        throw new Exception("不支持");
                }
                format = FormatConverter(converter.IsInstanceColumn, format, "DATEADD(DAY,{0},{1})", param);
                _result.Parameters.Add(param, parameters[0]);
                continue;
                format = FormatConverter(converter.IsInstanceColumn, format, "DATEADD(HOUR,{0},{1})", param);
                _result.Parameters.Add(param, parameters[0]);
                continue;
                format = FormatConverter(converter.IsInstanceColumn, format, "DATEADD(YEAR,{0},{1})", param);
                _result.Parameters.Add(param, parameters[0]);
                continue;
                format = FormatConverter(converter.IsInstanceColumn, format, "DATEADD(MONTH,{0},{1})", param);
                _result.Parameters.Add(param, parameters[0]);
                continue;
                format = FormatConverter(converter.IsInstanceColumn, format, "DATEADD(SECOND,{0},{1})", param);
                _result.Parameters.Add(param, parameters[0]);
                continue;
                format = FormatConverter(converter.IsInstanceColumn, format, "DATEADD(MILLISECOND,{0},{1})", param);
                _result.Parameters.Add(param, parameters[0]);
                continue;
                format = FormatConverter(converter.IsInstanceColumn, format, "DATEADD(MINUTE,{0},{1})", param);
                _result.Parameters.Add(param, parameters[0]);
                continue;
                throw new Exception("不支持");
                Label_064A:
                if (memberInfo.DeclaringType.IsGenericType)
                {
                    if (memberInfo.Name != "Contains")
                    {
                        throw new Exception("不支持");
                    }
                    if (converter.IsInstanceColumn)
                    {
                        throw new Exception("不支持");
                    }
                    var obj2 = parameters[0];
                    var declaringType = memberInfo.DeclaringType;
                    if (!declaringType.IsGenericType)
                    {
                        throw new Exception();
                    }
                    var collectionObject = (IEnumerable) obj2;
                    var processor1 =
                        new EnumerableContainsMethodProcessor(declaringType.GetGenericArguments().Last(),
                            collectionObject, format);
                    processor1.Process();
                    format = processor1.Result.ToString();
                }
                else
                {
                    if (memberInfo.DeclaringType == ReflectorConsts.EnumerableType)
                    {
                        if (memberInfo.Name != "Contains")
                        {
                            throw new Exception("不支持");
                        }
                        if (converter.IsInstanceColumn)
                        {
                            throw new Exception("不支持");
                        }
                        var obj3 = parameters[0];
                        var type = obj3.GetType();
                        Type elementType = null;
                        var enumerable2 = (IEnumerable) obj3;
                        if (type.IsArray)
                        {
                            var array1 = (Array) obj3;
                            elementType = type.GetElementType();
                        }
                        else
                        {
                            if (!type.IsGenericType)
                            {
                                throw new Exception();
                            }
                            elementType = type.GetGenericArguments().Last();
                        }
                        var processor2 = new EnumerableContainsMethodProcessor(
                            elementType, enumerable2, format);
                        processor2.Process();
                        format = processor2.Result.ToString();
                    }
                    else
                    {
                        if (!(memberInfo.DeclaringType == ReflectorConsts.QueryableType))
                        {
                            throw new Exception("不支持");
                        }
                        if (memberInfo.Name != "Contains")
                        {
                            throw new Exception("暂时对延迟加载支持不好");
                        }
                        if (converter.IsInstanceColumn)
                        {
                            throw new Exception("不支持");
                        }
                        var list = (IQueryable) parameters[0];
                        var type4 = list.GetType();
                        if (!type4.IsGenericType)
                        {
                            throw new Exception();
                        }
                        var type5 = type4.GetGenericArguments().Last();
                        var processor3 = new QueryableContainsMethodProcessor(list, type5,
                            format);
                        processor3.Process();
                        format = processor3.Result.ToString();
                    }
                }
            }
            return format;
        }

        public override string GetTableName(Manage.Core.SchemaModel.Table table)
        {
            string str = string.Empty;
            if (!string.IsNullOrWhiteSpace(table.DataBase))
            {
                str = @"[{table.DataBase}].DBO.";
            }
            return @"{str}{this.GetTableName(table.Name)}";
        }

       
    }
}