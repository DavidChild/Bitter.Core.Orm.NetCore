using System;
using System.Data;

namespace Bitter.Tools.Utils
{
    public class DataTypeUtils
    {
        /// <summary>
        /// 将DbType类型字符串表达形式对应映射到DbType类型
        /// </summary>
        /// <param name="dbType">DbType类型字符串表达类型</param>
        /// <returns></returns>
        public static DbType GetDbType(string typeName)
        {
            switch (typeName.ToLower())
            {
                case "char":
                case "ansistring":
                    return DbType.AnsiString;

                case "ansistringfixedlength":
                    return DbType.AnsiStringFixedLength;

                case "varbinary":
                case "binary":
                case "image":
                case "timestamp":
                case "byte[]":
                    return DbType.Binary;

                case "bit":
                case "boolean":
                    return DbType.Boolean;

                case "tinyint":
                case "byte":
                    return DbType.Byte;

                case "smallmoney":
                case "currency":
                    return DbType.Currency;

                case "date":
                    return DbType.Date;

                case "smalldatetime":
                case "datetime":
                    return DbType.DateTime;

                case "decimal":
                    return DbType.Decimal;

                case "money":
                case "double":
                    return DbType.Double;

                case "uniqueidentifier":
                case "guid":
                    return DbType.Guid;

                case "smallint":
                case "int16":
                case "uint16":
                    return DbType.Int16;

                case "int":
                case "int32":
                case "uint32":
                    return DbType.Int32;

                case "bigint":
                case "int64":
                case "uint64":
                    return DbType.Int64;

                case "variant":
                case "object":
                    return DbType.Object;

                case "sbyte":
                    return DbType.SByte;

                case "float":
                case "single":
                    return DbType.Single;

                case "text":
                case "string":
                case "varchar":
                case "nvarchar":
                    return DbType.String;

                case "nchar":
                case "stringfixedlength":
                    return DbType.StringFixedLength;

                case "time":
                    return DbType.Time;

                case "varnumeric":
                    return DbType.VarNumeric;

                case "xml":
                    return DbType.Xml;
            }

            return DbType.Object;
        }

        /// <summary>
        /// 将SqlDbType类型对应映射到DbType类型
        /// </summary>
        /// <param name="type">SqlDbType类型</param>
        /// <returns></returns>
        public static DbType GetDbType(Type type)
        {
            return GetDbType(type.Name.ToString());
        }

        /// <summary>
        /// 将DbType类型对应映射到SqlDbType类型
        /// </summary>
        /// <param name="type">DbType类型</param>
        /// <returns></returns>
        public static SqlDbType GetSqlType(Type type)
        {
            return GetSqlType(type.Name.ToString());
        }

        /// <summary>
        /// 将DbType类型字符串表达方式对应映射到SqlDbType类型
        /// </summary>
        /// <param name="dbType">DbType类型字符串表达类型</param>
        /// <returns></returns>
        public static SqlDbType GetSqlType(string typeName)
        {
            switch (typeName.ToLower())
            {
                case "char":
                case "ansistring":
                    return SqlDbType.Char;

                case "varchar":
                case "nvarchar":
                case "nvarchar2":
                case "ansistringfixedlength":
                case "stringfixedlength":
                case "string":
                    return SqlDbType.NVarChar;

                case "binary":
                case "image":
                case "varbinary":
                case "timestamp":
                    return SqlDbType.Binary;

                case "bit":
                case "boolean":
                    return SqlDbType.Bit;

                case "tinyint":
                case "byte":
                case "sbyte":
                    return SqlDbType.TinyInt;

                case "money":
                case "currency":
                    return SqlDbType.Money;

                case "datetime":
                case "date":
                    return SqlDbType.DateTime;

                case "decimal":
                    return SqlDbType.Decimal;

                case "real":
                case "double":
                    return SqlDbType.Real;

                case "uniqueidentifier":
                case "guid":
                    return SqlDbType.UniqueIdentifier;

                case "smallint":
                case "int16":
                case "uint16":
                    return SqlDbType.SmallInt;

                case "int":
                case "int32":
                case "uint32":
                case "number":
                    return SqlDbType.Int;

                case "bigint":
                case "int64":
                case "uint64":
                case "varnumeric":
                    return SqlDbType.BigInt;

                case "variant":
                case "object":
                    return SqlDbType.Variant;

                case "float":
                case "single":
                    return SqlDbType.Float;

                case "smalldatetime":
                case "time":
                    return SqlDbType.SmallDateTime;

                case "xml":
                    return SqlDbType.Xml;

                case "ntext":
                    return SqlDbType.NText;

                case "text":
                    return SqlDbType.Text;
            }

            return SqlDbType.NVarChar;
        }

        /// <summary>
        /// 将数据类型转换为Type
        /// </summary>
        /// <param name="sqlType">sqlType</param>
        /// <returns></returns>
        public static Type GetType(SqlDbType sqlType)
        {
            switch (sqlType)
            {
                case SqlDbType.BigInt:
                    return typeof(Int64);

                case SqlDbType.Binary:
                case SqlDbType.Image:
                case SqlDbType.Timestamp:
                case SqlDbType.VarBinary:
                    return typeof(byte[]);

                case SqlDbType.Bit:
                    return typeof(Boolean);

                case SqlDbType.Text:
                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                case SqlDbType.VarChar:
                    return typeof(String);

                case SqlDbType.SmallDateTime:
                case SqlDbType.DateTimeOffset:
                case SqlDbType.DateTime2:
                case SqlDbType.DateTime:
                case SqlDbType.Date:
                    return typeof(DateTime);

                case SqlDbType.Money:
                case SqlDbType.Decimal:
                case SqlDbType.SmallMoney:
                    return typeof(Decimal);

                case SqlDbType.Float:
                    return typeof(double);

                case SqlDbType.Int:
                    return typeof(int);

                case SqlDbType.Real:
                    return typeof(Single);

                case SqlDbType.TinyInt:
                    return typeof(Byte);

                case SqlDbType.SmallInt:
                    return typeof(Int16);

                case SqlDbType.UniqueIdentifier:
                    return typeof(Guid);

                default:
                    return typeof(object);
            }
        }

        /// <summary>
        /// 根据数据库字段类 型返回程序修饰符
        /// </summary>
        /// <param name="type">数据库字段类型</param>
        /// <returns>返回程序修饰符</returns>
        public static string ParseType(string type)
        {
            switch (type)
            {
                case "String":
                case "char":
                case "nchar":
                case "ntext":
                case "text":
                case "nvarchar":
                case "varchar":
                case "xml":
                case "set":
                case "longtext":
                case "enum":
                case "blob":
                case "longblob":
                case "mediumtext":
                case "CHAR":
                case "CLOB":
                case "LONG":
                case "NCHAR":
                case "NCLOB":
                case "NVARCHAR2":
                case "ROWID":
                case "VARCHAR2":
                    return "string";

                case "smallint":
                    return "short";

                case "int":
                case "Int32":
                case "INTERVAL YEAR TO MONTH":
                    return "int";

                case "bigint":
                    return "long";

                case "binary":
                case "image":
                case "varbinary":
                case "timestamp":
                case "BFILE":
                case "BLOB":
                case "LONG RAW":
                case "RAW":
                    return "byte[]";

                case "tinyint":
                    return "SByte";

                case "bit":
                case "Boolean":
                    return "bool";

                case "float":
                    return "double";

                case "real":
                    return "Guid";

                case "uniqueidentifier":
                    return "Guid";

                case "sql_variant":
                    return "object";

                case "decimal":
                case "Decimal":
                case "numeric":
                case "money":
                case "FLOAT":
                case "INTEGER":
                case "NUMBER":
                case "smallmoney":
                case "UNSIGNED INTEGER":
                    return "decimal";

                case "INTERVAL DAY TO SECOND":
                    return "TimeSpan";

                case "datetime":
                case "smalldatetime":
                case "date":
                case "time":
                case "DATE":
                case "TIMESTAMP":
                case "TIMESTAMP WITH LOCAL TIME ZONE":
                case "TIMESTAMP WITH TIME ZONE":
                    return "DateTime";
            }
            return type;
        }
    }
}