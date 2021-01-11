using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using BT.Manage.Tools;
using BT.Manage.Tools.Utils;
namespace BT.Manage.Core

{
    public class FindQuery<T> : WhereQuery<T>  where T:BaseModel,new()
    { 
       
        public FindQuery(string targetdb = null)
        {
           
            excutParBag= new ExcutParBag_Select();
            this.SetTargetDb(targetdb.ToSafeString());
            ((ExcutParBag_Select)excutParBag).excutEnum = ExcutEnum.Select;
            
            ((ExcutParBag_Select)excutParBag).selectColumns = new List<string>();
            ((ExcutParBag_Select)excutParBag).orders = new List<OrderPair>();
            ((ExcutParBag_Select)excutParBag).SetType(typeof(T));
        }
        public FindQuery<T> ThenDesc(Expression<Func<T, object>> orderFiled)
        {
            var ordername = MppingUntil.GetPropertyName<T>(orderFiled);
            ((ExcutParBag_Select)excutParBag).orders.Add(new OrderPair() {  orderBy=OrderBy.DESC, orderName= ordername });
            return this; 
        }
        public FindQuery<T> ThenAsc(Expression<Func<T, object>> orderFiled)
        {
            var ordername = MppingUntil.GetPropertyName<T>(orderFiled);
            ((ExcutParBag_Select)excutParBag).orders.Add(new OrderPair() { orderBy = OrderBy.ASC, orderName = ordername });
            return this;
        }
        public   FindQuery<T> Where(Expression<Func<T, bool>> condition)
        {
            base.Where(condition);
            ((ExcutParBag_Select)excutParBag).condition = this._Condition;
            return this;
        }

        public FindQuery<T> SetSize(int? i=1)
        {
            ((ExcutParBag_Select)excutParBag).topSize = i.ToSafeInt32(1);
            return this;
        }

       

        public FindQuery<T> Select(Expression<Func<T, List<object>>> columns)
        {
           
           
            List<string> selectColumns = new List<string>();
            //获取Update的赋值语句
            var selectMemberExpr = ((System.Linq.Expressions.ListInitExpression)columns.Body);
              selectMemberExpr.Initializers.Cast<ElementInit>().ToList().ForEach
                (
                    c =>
                    {
                        (c.Arguments).ToList().ForEach(p =>
                        {
                            var selectName = string.Empty;
                            if (p is MemberExpression)
                            {
                                selectName = ((MemberExpression)p).Member.Name;
                            }
                            else if (p is UnaryExpression)
                            {
                                selectName = ((MemberExpression)((UnaryExpression)p).Operand).Member.Name;
                            }
                            else if (p is ParameterExpression)
                            {
                                selectName = ((ParameterExpression)p).Type.Name;
                            }
                            selectColumns.Add(selectName);
                          
                        });
                    });
           
            return this;
        }
        public FindQuery<T> Select(Expression<Func<T, object>> column)
        {
            var columName = MppingUntil.GetPropertyName<T>(column);
            ((ExcutParBag_Select)excutParBag).selectColumns.Add(columName);
            return this;
        }

        public FindQuery<T> Select(Expression<Func<T, object[]>>  columns)
        {
            var selectExpr = ((System.Linq.Expressions.NewArrayExpression)columns.Body);
            selectExpr.Expressions.Cast<object>().ToList().ForEach
                (
                    c =>
                    {
                        var selectName = string.Empty;
                        if (c is MemberExpression)
                        {
                            selectName = ((MemberExpression)c).Member.Name;
                        }
                        else if (c is UnaryExpression)
                        {
                            selectName = ((MemberExpression)((UnaryExpression)c).Operand).Member.Name;
                        }
                        else if (c is ParameterExpression)
                        {
                            selectName = ((ParameterExpression)c).Type.Name;
                        }
                        ((ExcutParBag_Select)excutParBag).selectColumns.Add(selectName);

                    });
            return this;
        }


        public int FindCount()
        {
            var condition = ((ExcutParBag_Select)excutParBag).condition;
            excutParBag = this.excutParBag.MapTo<ExcutParBag_Count>();
            ((ExcutParBag_Count)(excutParBag)).condition = condition;
            ((ExcutParBag_Count)(excutParBag)).excutEnum = ExcutEnum.Count;
            ((ExcutParBag_Count)(excutParBag)).SetType(typeof(T));
            DataTable dt = this.ReturnDataTable();
            if (dt == null)
            {
                return 0;
            }
            else
            {
                return dt.Rows[0][0].ToSafeInt32(0);

            }
           
        }


        /// <summary>
        ///  获取数据
        /// </summary>
        /// <typeparam name="TOut">转换类型</typeparam>
        /// <param name="targetdb">执行的数据库</param>
        /// <returns>持久层如果没数据，那么就返回默认的default(IEnumerable<TOut>)</returns>
        public BList<T> Find()
        {
            DataTable dt = this.ReturnDataTable();
            if (dt == null||dt.Rows.Count<=0)
            {
                return new BList<T>();
            }
            return dt.ToBListModel<T>(); 
        }

        


        public T  QueryById(object id)
        {
            Where((Expression<Func<T, bool>>)BT.Manage.Core.Utils.IdConvertToLamda(typeof(T), id));
            var list= Find();
            if (list == null || list.Count() <= 0)
            {
                return  new T();
            }
            else
            {
                return list.First();
            }
          
        }


    }
}
