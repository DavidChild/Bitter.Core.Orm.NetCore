using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BT.Manage.Core
{
   public class TranslateParse:TranslateBase
    {
        private Expression _expression;
        private SqlExpressionParser parser = new SqlExpressionParser();

        public override void Parse(Expression expression)
        {
            this._expression = expression;
            this.parser.ElementType = base.ElementType;
            this.parser.Parse(expression);
            SqlBuilderBase base2 = ProviderFactory.CreateProvider(ConfigManager.DataBaseType).CreateSqlBuilderFactory().CreateSqlBuilder();
            BuilderContext context = new BuilderContext();
            SqlType select = SqlType.Select;
            if (this.parser.IsCallAny)
            {
                context.Take = 1;
            }
            if (this.parser.IsDelete)
            {
                select = SqlType.Delete;
            }
            else if (this.parser.IsUpdate)
            {
                select = SqlType.Update;
            }
            context.SqlType = select;
            context.Pager = (this.parser.Skip != -1) && (this.parser.Take != -1);
            context.SortColumns = this.parser.SortColumns;
            context.Joins = this.parser.Joins;
            context.UpdateResult = this.parser.UpdateResult;
            context.Skip = this.parser.Skip;
            context.Take = this.parser.Take;
            context.AggregationColumns = this.parser.AggregationColumns;
            context.Columns = this.parser.Columns;
            context.Distinct = this.parser.Distinct;
            context.NoLockTables = this.parser.NoLockTables;
            context.Conditions = this.parser.Conditions;
            context.ElementType = this.parser.ElementType;
            base.Result = base2.BuildSql(context);
        }
        
    }
}
