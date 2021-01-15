using System.Linq.Expressions;
using BT.Manage.DataAccess;

namespace BT.Manage.Core
{
    public class Parser : ParserBase
    {
        private Expression _expression;
        private readonly SqlExpressionParser parser = new SqlExpressionParser();

        public override void Parse(Expression expression)
        {
            _expression = expression;
            parser.ElementType = ElementType;
            parser.Parse(expression);
            var base2 =
                ProviderFactory.CreateProvider(dc.dbconn(string.Empty).Reader.DatabaseType)
                    .CreateSqlBuilderFactory()
                    .CreateSqlBuilder();
            var context = new BuilderContext();
            var select = SqlType.Select;
            if (parser.IsCallAny)
            {
                context.Take = 1;
            }
            if (parser.IsDelete)
            {
                select = SqlType.Delete;
            }
            else if (parser.IsUpdate)
            {
                select = SqlType.Update;
            }
            context.SqlType = select;
            context.Pager = (parser.Skip != -1) && (parser.Take != -1);
            context.SortColumns = parser.SortColumns;
            context.Joins = parser.Joins;
            context.UpdateResult = parser.UpdateResult;
            context.Skip = parser.Skip;
            context.Take = parser.Take;
            context.AggregationColumns = parser.AggregationColumns;
            context.Columns = parser.Columns;
            context.Distinct = parser.Distinct;
            context.NoLockTables = parser.NoLockTables;
            context.Conditions = parser.Conditions;
            context.ElementType = parser.ElementType;
            Result = base2.BuildSql(context);
        }
    }
}