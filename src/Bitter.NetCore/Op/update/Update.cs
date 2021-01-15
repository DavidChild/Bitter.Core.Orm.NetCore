using Bitter.Tools.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Bitter.Core
{
    public class Update<T> : WhereQuery<T>  where T:BaseModel
    {
        
        public  Update(string targetdb=null) 
        {
            excutParBag = new ExcutParBag_Update();
            this.SetTargetDb(targetdb.ToSafeString());
            ((ExcutParBag_Update)excutParBag).excutEnum = ExcutEnum.Update;
            ((ExcutParBag_Update)excutParBag).SetType(typeof(T));
        }

        public Update<T> Where(Expression<Func<T, bool>> condition)
        {
            base.Where(condition);
            ((ExcutParBag_Update)excutParBag).condition = this._Condition;
            return this;
        }

        public Update<T>  SetColumns(T newdata, Expression<Func<T, object[]>> columns)
        {

            List<string> columnss = new List<string>();

            var selectExpr = ((System.Linq.Expressions.NewArrayExpression)columns.Body);
            selectExpr.Expressions.Cast<object>().ToList().ForEach
                (
                    c =>
                    {
                        var columnName = string.Empty;
                        if (c is MemberExpression)
                        {
                            columnName = ((MemberExpression)c).Member.Name;
                            columnss.Add(columnName);
                        }
                        else if (c is UnaryExpression)
                        {
                            columnName = ((MemberExpression)((UnaryExpression)c).Operand).Member.Name;
                            columnss.Add(columnName);
                        }
                        else if (c is ParameterExpression)
                        {
                            columnName = ((ParameterExpression)c).Type.Name;
                            columnss.Add(columnName);
                        }
                     });
               
            foreach(string c in columnss)
            {

                PropertyInfo[] tmp = newdata.GetType().GetProperties();
                PropertyInfo pp = tmp.Where(p => p.Name == c).FirstOrDefault();
                var value= pp.GetValue(newdata);
                ((ExcutParBag_Update)excutParBag).updatePair.Add(new UpdatePair() { columnName = c, columnValue = value });
            }

            //StringBuilder whereBuilder = new StringBuilder();
            //BtConditionBuilder builder = new BtConditionBuilder();
            //builder.Build(columns.Body);
            ////获取SQL参数数组 (包括查询参数和赋值参数)
            //string sqlCondition = builder.Condition;

            //var updateMemberExpr = (MemberInitExpression)columns.Body;
            //var updateMemberCollection = updateMemberExpr.Bindings.Cast<MemberAssignment>();
            //foreach (MemberAssignment item in updateMemberCollection)
            //{
            //    var name = (item).Member.Name;
            //    object value = null;
            //    if (item.Expression is ConstantExpression)
            //    {
            //         value = ((ConstantExpression)item.Expression).Value;
            //    }
            //    if(item.Expression is MemberExpression)
            //    {
            //        var memberExpression = (MemberExpression)item.Expression;
            //        var @object =((ConstantExpression)(memberExpression.Expression)).Value; //这个是重点
            //        if (memberExpression.Member.MemberType == MemberTypes.Field)
            //        {
            //            value = ((FieldInfo)memberExpression.Member).GetValue(@object);
            //        }
            //        else if (memberExpression.Member.MemberType == MemberTypes.Property)
            //        {
            //            value = ((PropertyInfo)memberExpression.Member).GetValue(@object);
            //        }



            //    }





            //((ExcutParBag_Update)excutParBag).updatePair.Add(new UpdatePair() { columnName = name, columnValue = value });

            //}
            return this;

           
        
        }




    }
}
