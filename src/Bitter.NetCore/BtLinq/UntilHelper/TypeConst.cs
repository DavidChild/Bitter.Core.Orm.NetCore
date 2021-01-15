using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using BT.Manage.Tools.Attributes;

namespace BT.Manage.Core
{
    public class ReflectorConsts
    {
        public static readonly Type NonSelectAttributeType = typeof (NonSelectAttribute);
        public static Dictionary<Type, DbType> TypeMapper = new Dictionary<Type, DbType>();

        public static readonly MethodInfo AddDaysMethod;
        public static readonly MethodInfo AddOfList;
        public static readonly Type BoolType = typeof (bool);
        public static readonly Type ByteType = typeof (byte);
        public static readonly Type CompilerGeneratedAttributeType = typeof (CompilerGeneratedAttribute);
        public static readonly MethodInfo ConvertToBoolMethod;
        public static readonly MethodInfo ConvertToDateTimeMethod;
        public static readonly MethodInfo ConvertToInt32Method;
        public static readonly MethodInfo ConvertToStringMethod;
        public static readonly Type ConvertType = typeof (Convert);
        public static readonly PropertyInfo DatePropertyOfDateTime;
        public static readonly Type DateTimeNullableType = typeof (DateTime?);
        public static readonly Type DateTimeType = typeof (DateTime);
        public static readonly Type DecimalType = typeof (decimal);
        public static readonly Type DoubleType = typeof (double);
        public static readonly object[] EmptyObjectArray = new object[0];
        public static readonly Type EnumerableType = typeof (Enumerable);
        public static readonly PropertyInfo FieldCountOfIDataReader;
        public static readonly Type FloatType = typeof (float);
        public static readonly MethodInfo GetBooleanOfIDataReader;
        public static readonly MethodInfo GetByteOfIDataReader;
        public static readonly MethodInfo GetDateTimeOfIDataReader;
        public static readonly MethodInfo GetDecimalOfIDataReader;
        public static readonly MethodInfo GetDoubleOfIDataReader;
        public static readonly MethodInfo GetFloatOfIDataReader;
        public static readonly MethodInfo GetInt16OfIDataReader;
        public static readonly MethodInfo GetInt32OfIDataReader;
        public static readonly MethodInfo GetInt64OfIDataReader;
        public static readonly MethodInfo GetOrdinalOfIDataReader;
        public static readonly MethodInfo GetStringOfIDataReader;
        public static readonly Type IDataReaderType = typeof (IDataReader);
        public static readonly Type IDataRecordType = typeof (IDataRecord);
        public static readonly MethodInfo IEnumerableListContains;
        public static readonly Type IEnumerableType = typeof (IEnumerable<>);
        public static readonly Type IEnumeratorType = typeof (IEnumerator);
        public static readonly Type Int16NullableType = typeof (short?);
        public static readonly Type Int16Type = typeof (short);
        public static readonly Type Int32ArrayType = typeof (int[]);
        public static readonly Type Int32Type = typeof (int);
        public static readonly Type Int64Type = typeof (long);
        public static readonly Type IQueryableType = typeof (IQueryable<>);
        public static readonly MethodInfo IsDBNullfIDataReader;
        public static readonly Type ListIntType = typeof (List<int>);
        public static readonly Type ListObjectType = typeof (List<object>);
        public static readonly Type ListType = typeof (List<>);
        public static readonly Type NullableType = typeof (Nullable<>);
        public static readonly Type ObjectArrayType = typeof (object[]);
        public static readonly Type ObjectType = typeof (object);

        public static readonly Type QueryableType = typeof (Queryable);
        public static readonly MethodInfo ReadOfIDataReader;
        public static readonly Type StringArrayType = typeof (string[]);
        public static readonly MethodInfo StringContains;
        public static readonly Type StringType = typeof (string);

        public static readonly Type TimeSpanType = typeof (TimeSpan);

        static ReflectorConsts()
        {
            TypeMapper.Add(DateTimeType, DbType.DateTime2);
            TypeMapper.Add(Int32Type, DbType.Int32);
            TypeMapper.Add(typeof (short), DbType.Int16);
            TypeMapper.Add(typeof (long), DbType.Int64);
            TypeMapper.Add(typeof (string), DbType.String);
            TypeMapper.Add(typeof (DateTime?), DbType.DateTime2);
            TypeMapper.Add(typeof (int?), DbType.Int32);
            TypeMapper.Add(typeof (short?), DbType.Int16);
            TypeMapper.Add(typeof (long?), DbType.Int64);
            TypeMapper.Add(typeof (decimal), DbType.Decimal);
            TypeMapper.Add(typeof (decimal?), DbType.Decimal);
            TypeMapper.Add(typeof (double), DbType.Double);
            TypeMapper.Add(typeof (double?), DbType.Double);
            TypeMapper.Add(typeof (float), DbType.Single);
            TypeMapper.Add(typeof (float?), DbType.Single);
            TypeMapper.Add(typeof (bool), DbType.Boolean);
            TypeMapper.Add(typeof (byte), DbType.Byte);
            TypeMapper.Add(typeof (byte?), DbType.Byte);
        }
    }
}