using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using BT.Manage.DataAccess;

namespace BT.Manage.Core
{
    public class QueryProvider : IQueryProvider
    {
        private readonly Type _elementType;
        private readonly dbcontent dbset;
        private Expression _expression;
        public QueryProvider(dbcontent dbset, Type elementType)
        {
            _elementType = elementType;
            dbset = dbset;
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            this._expression = expression;
            return new DataQuery<TElement>(this, expression);
        }

        public IQueryable CreateQuery(Expression expression)
        {
            IQueryable queryable;
            Type type = expression.Type.GetGenericArguments()[0];
            try
            {
                Type[] typeArguments = new Type[] { type };
                object[] args = new object[] { this, expression };
                queryable = (IQueryable)Activator.CreateInstance(typeof(DataQuery<>).MakeGenericType(typeArguments), args);
            }
            catch (TargetInvocationException exception1)
            {
                throw exception1.InnerException;
            }
            return queryable;
        }


        public object Execute(Expression expression)
        {
            Type type = expression.Type;
            if (type.IsGenericType && (type.GetGenericTypeDefinition() == ReflectorConsts.IQueryableType))
            {
                type = ReflectorConsts.IEnumerableType.MakeGenericType(type.GetGenericArguments());
            }
            Type[] typeArguments = new Type[] { type };
            object[] parameters = new object[] { expression };
            return base.GetType().GetMethods().FirstOrDefault<MethodInfo>(x => ((x.Name == "Execute") && x.IsGenericMethodDefinition)).MakeGenericMethod(typeArguments).Invoke(this, parameters);
        }


        public TResult Execute<TResult>(Expression expression)
        {
            object obj2;
            var base2 = ProviderFactory.CreateProvider(dc.dbconn(string.Empty).Reader.DatabaseType);
            var result = ParseExpression(expression);
            var type = typeof (TResult);
            //SqlExecutorBase base3 = base2.CreateSqlExecutor();
            if ((expression.NodeType != ExpressionType.Call) || !type.IsValueType)
            {
                var flag = false;
                if (type.IsGenericType)
                {
                    var reader = ModelOpretion.SearchDataRetunDataTable(result.CommandText,
                        result.SqlQueryParameters, null); //base3.ExecuteReader(result.CommandText, result.Parameters);
                    if (type.GetGenericTypeDefinition() == ReflectorConsts.IEnumerableType)
                    {
                        var objectType = type.GetGenericArguments()[0];
                        IList list = null;
                        try
                        {
                            list = EntityMapper.Map(objectType, reader, base2.ObjectPropertyConvertType);
                        }
                        catch
                        {
                            throw new Exception("转化成TResult失败");
                        }


                        return (TResult) list;
                    }
                    if (typeof (Nullable<>).IsAssignableFrom(type))
                    {
                        flag = true;
                    }
                    if (!flag)
                    {
                        throw new Exception();
                    }
                }
                if (type.IsValueType | flag)
                {
                    var obj = ModelOpretion.ScalarBache(result.CommandText, result.SqlQueryParameters, null);
                    if (obj == null)
                    {
                        return default(TResult);
                    }


                    if (obj == DBNull.Value)
                    {
                        return default(TResult);
                    }
                    return (TResult) Convert.ChangeType(obj, type);
                }
                if (!EntityConfigurationManager.IsEntity(type))
                {
                    throw new Exception();
                }
                var dataReader = ModelOpretion.SearchDataRetunDataTable(result.CommandText,
                    result.SqlQueryParameters, null);
                IList list2 = null;
                try
                {
                    list2 = EntityMapper.Map(type, dataReader, base2.ObjectPropertyConvertType);
                }
                catch (Exception ex)
                {
                    throw new Exception("TResult:转化成实体失败.");
                }
                finally
                {
                }
                if (list2.Count <= 0)
                {
                    return default(TResult);
                }
                var entity = list2[0];

                return (TResult) entity;
            }
            //var name = ((MethodCallExpression) expression).Method.Name;
            //if (name != "Any")
            //{
            //    if ((name != "Delete") && (name != "Update"))
            //    {
            //        if ((name != "Average") && (name != "Sum") && (name != "Count"))
            //        {
            //            throw new Exception();
            //        }
            //        obj2 = base3.ExecuteScalar(result.CommandText, result.Parameters);
            //        if (obj2 == DBNull.Value)
            //        {
            //            return default(TResult);
            //        }
            //        var converter1 = new BaseTypeConverter(obj2, type);
            //        converter1.Process();
            //        return (TResult) converter1.Result;
            //    }
            //}
            //else
            //{
            //    DataSet set = base3.ExecuteDataSet(result.CommandText, result.Parameters);
            //    obj2 = set.Tables.Count <= 0 ? 0 : (object) (set.Tables[0].Rows.Count > 0);
            //    return (TResult) obj2;
            //}

            obj2 = ModelOpretion.ScalarBache(result.CommandText,
                result.SqlQueryParameters, null);
            return (TResult) obj2;
        }

        public string GetCommandText(Expression expression)
        {
            return ParseExpression(expression).CommandText;
        }

        internal ParseResult ParseExpression(Expression expression)
        {
            var base1 = ProviderFactory.CreateProvider(dc.dbconn(string.Empty).Reader.DatabaseType).CreateParser();
            base1.ElementType = _elementType;
            base1.Parse(expression);
            return base1.Result;
        }
    }
}