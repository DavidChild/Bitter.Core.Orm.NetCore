using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Bitter.Tools.Utils;
namespace Bitter.Core
{
    public class UpdateIns<T>:BaseQuery where T : BaseModel
    {
      
        public UpdateIns(T data,string targetdb=null)
        {
            excutParBag = new ExcutParBag_Update();
            this.SetTargetDb(targetdb.ToSafeString());
            ((ExcutParBag_Update)excutParBag).excutEnum = ExcutEnum.Update;
            ((ExcutParBag_Update)excutParBag).data = data;
        }
        public UpdateIns()
        {
            excutParBag = new ExcutParBag_Update();
          
        }

        /// <summary>
        /// 设置表名
        /// </summary>
        /// <param name="tableName"></param>
        public UpdateIns<T> SetTableName(string tableName)
        {
            excutParBag.SetTableName(tableName);
            return this;
        }

        public UpdateIns<T> SetColumns(Expression<Func<T, object[]>> columns)
        {
           
            var selectExpr = ((System.Linq.Expressions.NewArrayExpression)columns.Body);
            selectExpr.Expressions.Cast<object>().ToList().ForEach
                (
                    c =>
                    {
                        var columnName = string.Empty;
                        if (c is MemberExpression)
                        {
                            columnName = ((MemberExpression)c).Member.Name;
                        }
                        else if (c is UnaryExpression)
                        {
                            columnName = ((MemberExpression)((UnaryExpression)c).Operand).Member.Name;
                        }
                        else if (c is ParameterExpression)
                        {
                            columnName = ((ParameterExpression)c).Type.Name;
                        }
                        if (((ExcutParBag_Update)excutParBag).data != null)
                        {
                            ((ExcutParBag_Update)excutParBag).SetUpdatePair(new UpdatePair() { columnName = columnName });
                        }
                        else
                        {
                            ((ExcutParBag_Update)excutParBag).SetUpdatePair(new UpdatePair() { columnName = columnName, columnValue = "" });
                        }


                    });
            return this;
        }
    }
}
