using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace BT.Manage.Core
{
    public class ExpressionReflector
    {
        private static readonly Dictionary<Type, Func<IDataReader, IList>> _dataReader2ListCahce =
            new Dictionary<Type, Func<IDataReader, IList>>();

        private static readonly Dictionary<Type, Func<object[], object>> _objectConstructors =
            new Dictionary<Type, Func<object[], object>>();

        private static readonly IDictionary<Type, Dictionary<string, Func<object, object>>> _objectGetters =
            new Dictionary<Type, Dictionary<string, Func<object, object>>>();

        private static readonly IDictionary<Type, Dictionary<string, Action<object, object>>> _objectSetters =
            new Dictionary<Type, Dictionary<string, Action<object, object>>>();

        private static IDictionary<Type, Action<object, object>[]> _objectSettersArray =
            new Dictionary<Type, Action<object, object>[]>();

        public static object CreateInstance(Type type, ObjectPropertyConvertType convertType, params object[] parameters)
        {
            return GetConstructor(type, convertType)(parameters);
        }

        public static Func<object[], object> GetConstructor(Type type, ObjectPropertyConvertType convertType)
        {
            Func<object[], object> func = null;
            if (!_objectConstructors.TryGetValue(type, out func))
            {
                var dictionary = _objectConstructors;
                lock (dictionary)
                {
                    Expression expression;
                    if (_objectConstructors.TryGetValue(type, out func))
                    {
                        return func;
                    }
                    var array = Expression.Parameter(ReflectorConsts.ObjectArrayType);
                    if (TypeHelper.IsValueType(type))
                    {
                        expression = Expression.New(type);
                    }
                    else
                    {
                        var constructor = type.GetConstructors()[0];
                        var list = new List<Expression>();
                        var infoArray = constructor.GetParameters();
                        for (var i = 0; i < infoArray.Length; i++)
                        {
                            var info2 = infoArray[i];
                            var expression3 = Expression.ArrayIndex(array, Expression.Constant(i));
                            var flag2 = TypeHelper.IsNullableType(info2.ParameterType);
                            var parameterType = info2.ParameterType;
                            if (flag2)
                            {
                                parameterType = TypeHelper.GetUnderlyingType(parameterType);
                            }
                            Expression expression4 = null;
                            if (convertType != ObjectPropertyConvertType.ConvertTo)
                            {
                                if (convertType == ObjectPropertyConvertType.Cast)
                                {
                                    expression4 = Expression.Convert(expression3, info2.ParameterType);
                                }
                            }
                            else
                            {
                                MethodInfo method = null;
                                if (parameterType == ReflectorConsts.DateTimeType)
                                {
                                    method = ReflectorConsts.ConvertToDateTimeMethod;
                                    Expression[] expressionArray1 = {expression3};
                                    expression4 = Expression.Call(null, ReflectorConsts.ConvertToDateTimeMethod,
                                        expressionArray1);
                                }
                                else if (parameterType == ReflectorConsts.StringType)
                                {
                                    method = ReflectorConsts.ConvertToStringMethod;
                                }
                                else if (parameterType == ReflectorConsts.Int32Type)
                                {
                                    method = ReflectorConsts.ConvertToInt32Method;
                                }
                                else
                                {
                                    if (parameterType != ReflectorConsts.BoolType)
                                    {
                                        throw new Exception("不支持：" + parameterType);
                                    }
                                    method = ReflectorConsts.ConvertToBoolMethod;
                                }
                                Expression[] arguments = {expression3};
                                expression4 = Expression.Call(null, method, arguments);
                                if (flag2)
                                {
                                    expression4 = Expression.Convert(expression4, info2.ParameterType);
                                }
                            }
                       
                            list.Add(expression4);
                        }
                        expression = Expression.New(constructor, list.ToArray());
                    }
                    ParameterExpression[] parameters = {array};
                    func = Expression.Lambda<Func<object[], object>>(expression, parameters).Compile();
                    _objectConstructors.Add(type, func);
                }
            }
            return func;
        }

        public static Func<IDataReader, IList> GetDataReaderMapeer(Type type, IDataReader reader)
        {
            Func<IDataReader, IList> func = null;
            if (!_dataReader2ListCahce.TryGetValue(type, out func))
            {
                var dictionary = _dataReader2ListCahce;
                lock (dictionary)
                {
                    if (_dataReader2ListCahce.TryGetValue(type, out func))
                    {
                        return func;
                    }
                    var right = Expression.Parameter(ReflectorConsts.IDataReaderType, "reader");
                    var properties = GetProperties(type);
                    var fieldCount = reader.FieldCount;
                    var list = new List<Expression>();
                    var left = Expression.Variable(type, "entity");
                    var expression3 = Expression.Variable(ReflectorConsts.Int32Type, "fieldCount");
                    var expression4 = Expression.Variable(ReflectorConsts.IDataReaderType, "readerVar");
                    var array = Expression.Variable(ReflectorConsts.StringArrayType, "pis");
                    var expression6 = Expression.Variable(ReflectorConsts.Int32ArrayType, "indexes");
                    var index = Expression.Variable(ReflectorConsts.Int32Type, "readIndex");
                    var expression8 = Expression.Variable(ReflectorConsts.Int32Type, "index");
                    var @break = Expression.Label("forBreak");
                    var item = Expression.Assign(expression8, Expression.Constant(0));
                    Type[] typeArguments = {type};
                    var type2 = ReflectorConsts.ListType.MakeGenericType(typeArguments);
                    var expression10 = Expression.Variable(type2, "list");
                    list.Add(Expression.Assign(expression10, Expression.New(type2)));
                    list.Add(Expression.Assign(expression4, right));
                    list.Add(item);
                    var expression11 = Expression.Assign(expression3,
                        Expression.MakeMemberAccess(expression4, ReflectorConsts.FieldCountOfIDataReader));
                    list.Add(expression11);
                    Expression[] arguments = {Expression.ArrayIndex(array, expression8)};
                    var expression12 = Expression.Call(expression4, ReflectorConsts.GetOrdinalOfIDataReader, arguments);
                    Expression[] bounds = {Expression.Constant(fieldCount)};
                    var expression13 = Expression.Assign(expression6,
                        Expression.NewArrayBounds(ReflectorConsts.Int32Type, bounds));
                    var initializers = new List<Expression>();
                    for (var i = 0; i < fieldCount; i++)
                    {
                        initializers.Add(Expression.Constant(reader.GetName(i)));
                    }
                    var expression14 = Expression.Assign(array,
                        Expression.NewArrayInit(ReflectorConsts.StringType, initializers));
                    Expression[] indexes = {expression8};
                    var expression15 = Expression.Assign(Expression.ArrayAccess(expression6, indexes), expression12);
                    list.Add(expression14);
                    list.Add(expression13);
                    list.Add(
                        Expression.Loop(
                            Expression.IfThenElse(Expression.LessThan(expression8, expression3),
                                Expression.Block(expression15,
                                    Expression.Assign(expression8, Expression.Add(expression8, Expression.Constant(1)))),
                                Expression.Break(@break)), @break));
                    var target = Expression.Label(type, "return");
                    var list3 = new List<ParameterExpression>();
                    var expressions = new List<Expression>();
                    if (TypeHelper.IsCompilerGenerated(type))
                    {
                        var constructor = type.GetConstructors().FirstOrDefault();
                        if (constructor == null)
                        {
                            throw new ArgumentException("类型" + type.FullName + "未找到构造方法");
                        }
                        var infoArray = constructor.GetParameters();
                        var collection = new List<ParameterExpression>();
                        for (var j = 0; j < fieldCount; j++)
                        {
                            var info2 = infoArray[j];
                            var expression16 = Expression.Variable(info2.ParameterType, info2.Name);
                            var switcher1 =
                                new DataReaderGetMethodSwitcher(TypeHelper.GetUnderlyingType(info2.ParameterType), index,
                                    expression4);
                            switcher1.Process();
                            var result = (Expression) switcher1.Result;
                            if (TypeHelper.IsNullableType(info2.ParameterType))
                            {
                                result = Expression.Convert(result, info2.ParameterType);
                            }
                            Expression[] expressionArray4 = {index};
                            var expression18 =
                                Expression.IfThenElse(
                                    Expression.Call(right, ReflectorConsts.IsDBNullfIDataReader, expressionArray4),
                                    Expression.Assign(expression16, Expression.Default(info2.ParameterType)),
                                    Expression.Assign(expression16, result));
                            new List<Expression>();
                            expressions.Add(Expression.Assign(index,
                                Expression.ArrayIndex(expression6, Expression.Constant(j))));
                            expressions.Add(expression18);
                            collection.Add(expression16);
                        }
                        expressions.Add(Expression.Assign(left, Expression.New(constructor, collection)));
                        list3.AddRange(collection);
                        list3.Add(expression4);
                        list3.Add(expression10);
                    }
                    else
                    {
                        var expression19 = Expression.New(type);
                        expressions.Add(Expression.Assign(left, expression19));
                        for (var k = 0; k < fieldCount; k++)
                        {
                            var name = reader.GetName(k);
                            var property = properties[name];
                            if (property != null)
                            {
                                new List<Expression>();
                                var expression20 = Expression.Property(left, property);
                                Expression expression = null;
                                var switcher2 =
                                    new DataReaderGetMethodSwitcher(
                                        TypeHelper.GetUnderlyingType(property.PropertyType), index, expression4);
                                switcher2.Process();
                                expression = (Expression) switcher2.Result;
                                if (TypeHelper.IsNullableType(property.PropertyType))
                                {
                                    expression = Expression.Convert(expression, property.PropertyType);
                                }
                                expressions.Add(Expression.Assign(index,
                                    Expression.ArrayIndex(expression6, Expression.Constant(k))));
                                Expression[] expressionArray5 = {index};
                                var expression22 =
                                    Expression.IfThen(
                                        Expression.Not(Expression.Call(right, ReflectorConsts.IsDBNullfIDataReader,
                                            expressionArray5)), Expression.Assign(expression20, expression));
                                expressions.Add(expression22);
                            }
                        }
                        list3.Add(expression10);
                        list3.Add(expression4);
                    }
                    list3.Add(expression8);
                    list3.Add(array);
                    list3.Add(expression3);
                    list3.Add(expression6);
                    list3.Add(index);
                    Expression[] expressionArray6 = {left};
                    expressions.Add(Expression.Call(expression10,
                        type2.GetMethods().FirstOrDefault(x => x.Name == "Add"), expressionArray6));
                    var expressionArray7 = new Expression[1];
                    ParameterExpression[] variables = {left};
                    expressionArray7[0] =
                        Expression.IfThenElse(Expression.Call(expression4, ReflectorConsts.ReadOfIDataReader),
                            Expression.Block(variables, expressions), Expression.Break(target, Expression.Default(type)));
                    list.Add(Expression.Loop(Expression.Block(expressionArray7), target));
                    list.Add(expression10);
                    ParameterExpression[] parameters = {right};
                    func =
                        Expression.Lambda<Func<IDataReader, IList>>(Expression.Block(list3, list), parameters).Compile();
                    _dataReader2ListCahce.Add(type, func);
                }
            }
            return func;
        }

        public static Dictionary<string, Func<object, object>> GetGetters(Type entityType)
        {
            Dictionary<string, Func<object, object>> dictionary = null;
            if (!_objectGetters.TryGetValue(entityType, out dictionary))
            {
                var dictionary2 = _objectGetters;
                lock (dictionary2)
                {
                    if (_objectGetters.TryGetValue(entityType, out dictionary))
                    {
                        return dictionary;
                    }
                    dictionary = new Dictionary<string, Func<object, object>>();
                    foreach (var info in ExpressionReflectorCore.GetProperties(entityType).Values)
                    {
                        var expression = Expression.Parameter(ExpressionReflectorCore.ObjectType);
                        ParameterExpression[] parameters = {expression};
                        var func =
                            Expression.Lambda<Func<object, object>>(
                                Expression.Convert(
                                    Expression.Property(Expression.Convert(expression, entityType), info),
                                    ExpressionReflectorCore.ObjectType), parameters).Compile();
                        dictionary.Add(info.Name, func);
                    }
                    _objectGetters.Add(entityType, dictionary);
                }
            }
            return dictionary;
        }

        public static Action<object, object[]> GetMethodDelegate(object proxyObject, string methodName,
            params Type[] argTypes)
        {
            var type = proxyObject.GetType();
            var method = type.GetMethod(methodName, argTypes);
            if (method == null)
            {
                throw new ArgumentException("指定方法未找到");
            }
            var expression = Expression.Parameter(ExpressionReflectorCore.ObjectType);
            Expression.Convert(expression, type);
            var array = Expression.Parameter(typeof (object[]));
            var list = new List<Expression>();
            var parameters = method.GetParameters();
            for (var i = 0; i < parameters.Length; i++)
            {
                var info1 = parameters[i];
                var item = Expression.Convert(Expression.ArrayIndex(array, Expression.Constant(i)),
                    parameters[i].ParameterType);
                list.Add(item);
            }
            ParameterExpression[] expressionArray1 = {expression, array};
            return
                Expression.Lambda<Action<object, object[]>>(
                    Expression.Call(method.IsStatic ? null : Expression.Convert(expression, method.ReflectedType),
                        method, list.ToArray()), expressionArray1).Compile();
        }

        public static Type GetNullableOrSelfType(Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType == null)
            {
                return type;
            }
            return underlyingType;
        }

        public static IDictionary<string, PropertyInfo> GetProperties(Type type)
        {
            return ExpressionReflectorCore.GetProperties(type);
        }


        public static Dictionary<string, object> GetPropertyValues(object entity)
        {
            var dictionary = new Dictionary<string, object>();
            foreach (var pair in GetGetters(entity.GetType()))
            {
                dictionary.Add(pair.Key, pair.Value(entity));
            }
            return dictionary;
        }

        public static Dictionary<string, Action<object, object>> GetSetters(Type entityType)
        {
            Dictionary<string, Action<object, object>> dictionary = null;
            if (!_objectSetters.TryGetValue(entityType, out dictionary))
            {
                var dictionary2 = _objectSetters;
                lock (dictionary2)
                {
                    if (_objectSetters.TryGetValue(entityType, out dictionary))
                    {
                        return dictionary;
                    }
                    dictionary = new Dictionary<string, Action<object, object>>();
                    foreach (var info in ExpressionReflectorCore.GetProperties(entityType).Values)
                    {
                        if (info.GetSetMethod() != null)
                        {
                            var expression = Expression.Parameter(ExpressionReflectorCore.ObjectType);
                            var expression2 = Expression.Parameter(ExpressionReflectorCore.ObjectType);
                            var right = Expression.Convert(expression2, info.PropertyType);
                            ParameterExpression[] parameters = {expression, expression2};
                            var action =
                                Expression.Lambda<Action<object, object>>(
                                    Expression.Assign(
                                        Expression.Property(Expression.Convert(expression, entityType), info), right),
                                    parameters).Compile();
                            dictionary.Add(info.Name, action);
                        }
                    }
                    _objectSetters.Add(entityType, dictionary);
                }
            }
            return dictionary;
        }

        public static object GetValue(object entity, string propertyName)
        {
            Func<object, object> func = null;
            if (!GetGetters(entity.GetType()).TryGetValue(propertyName, out func))
            {
                throw new Exception("Getter未初始化完整");
            }
            return func(entity);
        }

        public static bool IsEntityPropertyType(Type type)
        {
            return ExpressionReflectorCore.EntityPropertyTypes.Contains(type);
        }


        public static void SetValue(object entity, string propertyName, object value)
        {
            Action<object, object> action = null;
            if (!GetSetters(entity.GetType()).TryGetValue(propertyName, out action))
            {
                throw new Exception("Setter未初始化完整");
            }
            action(entity, value);
        }
    }
}