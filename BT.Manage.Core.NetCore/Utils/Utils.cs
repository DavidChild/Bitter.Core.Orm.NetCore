using BT.Manage.Tools.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;


namespace BT.Manage.Core
{
    public class Utils
    {
        //判断是否是自增长模型
        public static bool CheckedIsIdentityModel(Type type)
        {
            bool isIdentityKey = false;
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                string FieldName = propertyInfo.Name;
                if (FieldName.Length > 0 && FieldName.IndexOf('_') == 0) continue; //continue
                object[] attrs = propertyInfo.GetCustomAttributes(true);
                for (int i = 0; i < attrs.Count(); i++)
                {
                    string memberInfoName = attrs.GetValue(i).GetType().Name;
                    if (memberInfoName == "IdentityAttribute")
                    {
                        Tools.Attributes.IdentityAttribute keyAttr = attrs[i] as Tools.Attributes.IdentityAttribute;
                        if (keyAttr != null)
                        {
                            isIdentityKey = true;
                            break;
                        }
                    }
                }
                if (isIdentityKey) break;
            }
            return isIdentityKey;
        }
        /// <summary>
        /// 根据model获取主键字段
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="data">model</param>
        /// <returns>主键字段</returns>
        public static string GetDisplayName<T>(T data, string filedName)
        {
            //string KeyFiled = string.Empty;    //主键字段
            //var sd = data.GetType().GetCustomAttributes(true);
            PropertyInfo[] properties = data.GetType().GetProperties();
            PropertyInfo zk = data.GetType().GetProperties().Where(k => k.Name == filedName).FirstOrDefault();
            var f = zk.GetCustomAttributes(false).Where(p => p.GetType().Name == "DisplayAttribute").FirstOrDefault();
            if (f == null)
            {
                return string.Empty;
            }
            else
            {
                return ((System.ComponentModel.DataAnnotations.DisplayAttribute)f).Name.Trim().Split(new char[] { ' ' }).FirstOrDefault();
            }

            //foreach (PropertyInfo propertyInfo in properties)
            //{
            //    Type propertyType = propertyInfo.PropertyType;
            //    var attrs = propertyInfo.GetCustomAttributes(true).Where(p => p.GetType().Name == "KeyAttribute");
            //    if (attrs.Count() > 0)
            //    {
            //        KeyFiled = propertyInfo.Name;
            //        break;
            //    }
            //    else
            //        continue;

            //}
            //return f.ToString() ;
        }
        public static string[] GetDtoEnumTypeValueAndParentName(object data, string FileName)
        {
            Type type = data.GetType();
            string value = GetObjectPropertyValue(data, FileName.Remove(0, 3));
            string[] ks = new string[2];
            var properties = from k in type.GetProperties() where k.Name == FileName.Remove(0, 1) select k;
            if (properties == null || properties.Count() == 0)
            {
                return ks;
            }
            PropertyInfo propertyInfo = properties.ToArray()[0];
            var px =
                     from t in propertyInfo.GetCustomAttributes(true)
                     where (t.GetType().Name == "EnumTypeAttribute")
                     select t;
            if (px.Count() != 0)
            {
                BT.Manage.Core.EnumTypeAttribute k = px.AsEnumerable().FirstOrDefault() as BT.Manage.Core.EnumTypeAttribute;
                ks[0] = k.EnumTypeName;
                ks[1] = value;
            }

            return ks;
        }

        public static string[] GetEnumTypeValueAndParentName(object data, string FileName)
        {
            Type type = data.GetType();
            string value = GetObjectPropertyValue(data, FileName.Remove(0, 1));
            string[] ks = new string[2];
            var properties = from k in type.GetProperties() where k.Name == FileName.Remove(0, 1) select k;
            if (properties == null || properties.Count() == 0)
            {
                return ks;
            }
            PropertyInfo propertyInfo = properties.ToArray()[0];
            var px =
                     from t in propertyInfo.GetCustomAttributes(true)
                     where (t.GetType().Name == "EnumTypeAttribute")
                     select t;
            if (px.Count() != 0)
            {
                BT.Manage.Core.EnumTypeAttribute k = px.AsEnumerable().FirstOrDefault() as BT.Manage.Core.EnumTypeAttribute;
                ks[0] = k.EnumTypeName;
                ks[1] = value;
            }

            return ks;
        }
        /// <summary>
        /// 根据model获取主键字段
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="data">model</param>
        /// <returns>主键字段</returns>
        public static string GetKeyFiledName<T>(T data)
        {
            string KeyFiled = string.Empty;    //主键字段
            var sd = data.GetType().GetCustomAttributes(true);
            PropertyInfo[] properties = data.GetType().GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                Type propertyType = propertyInfo.PropertyType;
                var attrs = propertyInfo.GetCustomAttributes(true).Where(p => p.GetType().Name == "KeyAttribute");
                if (attrs.Count() > 0)
                {
                    KeyFiled = propertyInfo.Name;
                    break;
                }
                else
                    continue;
            }
            return KeyFiled;
        }
        /// <summary>
        /// 根据Type获取主键字段
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="data">model</param>
        /// <returns>主键字段</returns>
        public static string GetKeyFiledName(Type type)
        {
            string KeyFiled = string.Empty;    //主键字段
            var sd = type.GetCustomAttributes(true);
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                Type propertyType = propertyInfo.PropertyType;
                var attrs = propertyInfo.GetCustomAttributes(true).Where(p => p.GetType().Name == "KeyAttribute");
                if (attrs.Count() > 0)
                {
                    KeyFiled = propertyInfo.Name;
                    break;
                }
                else
                    continue;
            }
            return KeyFiled;
        }
        /// <summary>
        /// 根据Type获取主键字段
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="data">model</param>
        /// <returns>主键字段</returns>
        public static bool CheckedIsIdentity(Type type, string filedName)
        {
            bool bl = false;
            string KeyFiled = string.Empty;    //主键字段
            var sd = type.GetCustomAttributes(true);
            List<PropertyInfo> properties = (from p in type.GetProperties() where p.Name == filedName select p).ToList();
            if (properties == null || properties.Count <= 0) return false;
            foreach (PropertyInfo propertyInfo in properties)
            {


                Type propertyType = propertyInfo.PropertyType;
                var attrs = propertyInfo.GetCustomAttributes(true).Where(p => p.GetType().Name == "IdentityAttribute");
                if (attrs.Count() > 0)
                {
                    KeyFiled = propertyInfo.Name;
                    bl = true;
                    break;
                }
                else
                    continue;
            }
            return bl;
        }
        /// <summary>
        /// 反射属性值
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="data">model</param>
        /// <returns>表名</returns>
        public static string GetObjectPropertyValue<T>(T data, string fileName)
        {
            PropertyInfo p = data.GetType().GetProperty(fileName);
            if (p == null) return string.Empty;
            object o = p.GetValue(data, null);
            if (o == null) return string.Empty;
            return o.ToString();



        }

        /// <summary>
        /// 根据model获取主键字段
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="data">model</param>
        /// <returns>主键字段</returns>
        public static Type GetPropertyType<T>(T data, string filedName)
        {
            //string KeyFiled = string.Empty;    //主键字段
            //var sd = data.GetType().GetCustomAttributes(true);
            PropertyInfo[] properties = data.GetType().GetProperties();
            PropertyInfo zk = data.GetType().GetProperties().Where(k => k.Name == filedName).FirstOrDefault();
            return zk.PropertyType;

        }

        public static string[] GetRelationChildrenTableFileNameAndValue(object data, Type type)
        {
            string sql = "SELECT * FROM {0} WHERE  {1}='{2}'";
            string childtablename = GetTableName(type);
            string typetablename = GetTableName(data);
            string fid = GetKeyFiledName(data);
            string vfid = GetObjectPropertyValue(data, fid);
            string[] ks = new string[3];
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                var px =
                      from t in propertyInfo.GetCustomAttributes(true)
                      where (t.GetType().Name == "RelationTableAttribute")
                      select t;

                if (px.Count() != 0)
                {
                    BT.Manage.Core.RelationTableAttribute k = px.AsEnumerable().FirstOrDefault() as BT.Manage.Core.RelationTableAttribute;
                    if (GetTableName(k.relationTable) == typetablename)
                    {
                        ks[0] = vfid;
                        ks[1] = GetObjectPropertyValue(data, propertyInfo.Name);
                        sql = string.Format(sql, childtablename, propertyInfo.Name, vfid);
                        ks[2] = sql;
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            return ks;
        }

        public static string[] GetRelationTableFileNameAndValue(object data, Type type)
        {
            string sql = "SELECT * FROM {0} WHERE  {1}='{2}'";

            string typetablename = GetTableName(type);
            string fid = GetKeyFiledName(type);
            string[] ks = new string[3];
            PropertyInfo[] properties = data.GetType().GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                var px =
                      from t in propertyInfo.GetCustomAttributes(true)
                      where (t.GetType().Name == "RelationTableAttribute")
                      select t;

                if (px.Count() != 0)
                {
                    BT.Manage.Core.RelationTableAttribute k = px.AsEnumerable().FirstOrDefault() as BT.Manage.Core.RelationTableAttribute;
                    if (GetTableName(k.relationTable) == typetablename)
                    {
                        ks[0] = fid;
                        ks[1] = GetObjectPropertyValue(data, propertyInfo.Name);
                        sql = string.Format(sql, typetablename, fid, ks[1]);
                        ks[2] = sql;
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            return ks;
        }

        /// <summary>
        /// 根据model获取表名（只获取表名）
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="data">model</param>
        /// <returns>表名</returns>
        public static string GetTableName<T>(T data)
        {
            string tableName = string.Empty;    //数据库名
            var sd = data.GetType().GetCustomAttributes(true);
            for (int i = 0; i < sd.Count(); i++)
            {
                if (sd.GetValue(i).GetType().Name == "TableName")
                {
                    Tools.Attributes.TableName tableNameTmp = sd[i] as Tools.Attributes.TableName;
                    if (tableNameTmp != null)
                    {
                        tableName = tableNameTmp.name;
                    }
                }
            }
            return tableName;
        }

        /// <summary>
        /// 根据Type获取表名（只获取表名）
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="data">model</param>
        /// <returns>表名</returns>
        public static string GetTableName(Type type)
        {
            string tableName = string.Empty;    //数据库名
            var sd = type.GetCustomAttributes(true);
            for (int i = 0; i < sd.Count(); i++)
            {
                if (sd.GetValue(i).GetType().Name == "TableName")
                {
                    Tools.Attributes.TableName tableNameTmp = sd[i] as Tools.Attributes.TableName;
                    if (tableNameTmp != null)
                    {
                        tableName = tableNameTmp.name;
                    }
                }
            }
            return tableName;
        }
        public static SqlDbType SqlTypeString2SqlType(string sqlTypeString)
        {
            SqlDbType dbType = SqlDbType.Variant;//默认为Object

            switch (sqlTypeString)
            {
                case "int16":
                case "int32":
                
                    dbType = SqlDbType.Int;
                    break;

                case "string":
                case "varchar":
                    dbType = SqlDbType.NVarChar;
                    break;

                case "boolean":
                case "bool":
                case "bit":
                    dbType = SqlDbType.Bit;
                    break;

                case "datetime":
                    dbType = SqlDbType.DateTime;
                    break;

                case "decimal":
                    dbType = SqlDbType.Decimal;
                    break;

                case "float":
                    dbType = SqlDbType.Float;
                    break;

                case "image":
                    dbType = SqlDbType.Image;
                    break;

                case "money":
                    dbType = SqlDbType.Money;
                    break;

                case "ntext":
                    dbType = SqlDbType.NText;
                    break;

                case "nvarchar":
                    dbType = SqlDbType.NVarChar;
                    break;

                case "smalldatetime":
                    dbType = SqlDbType.SmallDateTime;
                    break;

                case "smallint":
                    dbType = SqlDbType.SmallInt;
                    break;

                case "text":
                    dbType = SqlDbType.Text;
                    break;

                case "int64":
                case "bigint":
                    dbType = SqlDbType.BigInt;
                    break;

                case "binary":
                    dbType = SqlDbType.Binary;
                    break;

                case "char":
                    dbType = SqlDbType.Char;
                    break;

                case "nchar":
                    dbType = SqlDbType.NChar;
                    break;

                case "numeric":
                    dbType = SqlDbType.Decimal;
                    break;

                case "real":
                    dbType = SqlDbType.Real;
                    break;

                case "smallmoney":
                    dbType = SqlDbType.SmallMoney;
                    break;

                case "sql_variant":
                    dbType = SqlDbType.Variant;
                    break;

                case "timestamp":
                    dbType = SqlDbType.Timestamp;
                    break;

                case "tinyint":
                    dbType = SqlDbType.TinyInt;
                    break;

                case "uniqueidentifier":
                    dbType = SqlDbType.UniqueIdentifier;
                    break;

                case "varbinary":
                    dbType = SqlDbType.VarBinary;
                    break;

                case "xml":
                    dbType = SqlDbType.Xml;
                    break;
            }
            return dbType;
        }
        public static DbType SqlTypeToDbType(SqlDbType SqlDbType)
        {
            DbType dbType = DbType.Object;//默认为Object

            switch (SqlDbType)
            {

                case SqlDbType.Int:
                    dbType = DbType.Int32;
                    break;
                case SqlDbType.NVarChar:
                    dbType = DbType.String;
                    break;
                case SqlDbType.Bit:
                    dbType = DbType.Binary;
                    break;
                case SqlDbType.DateTime:
                    dbType = DbType.DateTime;
                    break;
                case SqlDbType.Decimal:
                    dbType = DbType.Decimal;
                    break;
                case SqlDbType.Float:
                    dbType = DbType.Double;
                    break;
                case SqlDbType.Image:
                    dbType = DbType.Binary;
                    break;
                //case SqlDbType.Money:
                //    dbType = DbType
                //    break;
                case SqlDbType.NText:
                    dbType = DbType.String;
                    break;

                case SqlDbType.SmallDateTime:
                    dbType = DbType.DateTime2;
                    break;
                case SqlDbType.SmallInt:
                    dbType = DbType.Int32;
                    break;
                case SqlDbType.Text:
                    dbType = DbType.String;
                    break;
                case SqlDbType.BigInt:
                    dbType = DbType.Int64;
                    break;
                case SqlDbType.Binary:
                    dbType = DbType.Binary;
                    break;
                case SqlDbType.Char:
                    dbType = DbType.String;
                    break;
                case SqlDbType.NChar:
                    dbType = DbType.String;
                    break;

                case SqlDbType.Real:
                    dbType = DbType.Boolean;
                    break;
                case SqlDbType.SmallMoney:
                    dbType = DbType.Decimal;
                    break;
                //case SqlDbType.Variant:
                //    dbType = DbType.Decimal;
                //    break;

                case SqlDbType.Timestamp:
                    dbType = DbType.DateTimeOffset;
                    break;

                case SqlDbType.TinyInt:
                    dbType = DbType.Int32;
                    break;
                case SqlDbType.UniqueIdentifier:
                    dbType = DbType.Int32;
                    break;
                case SqlDbType.VarBinary:
                    dbType = DbType.Binary;
                    break;
                case SqlDbType.Xml:
                    dbType = DbType.Xml;
                    break;
                    
            }
            return dbType;
        }



        internal static Expression<Func<T, bool>> IdConvertToLamda<T>(object value)
        {
            var propertyName = Utils.GetKeyFiledName(typeof(T));
            ParameterExpression parameter = Expression.Parameter(typeof(T), "p");//创建参数p
            MemberExpression member = Expression.PropertyOrField(parameter, propertyName);
            ConstantExpression constant = Expression.Constant(value);//创建常数
            return Expression.Lambda<Func<T, bool>>(Expression.Equal(member, constant), parameter);

        }

        internal static LambdaExpression IdConvertToLamda(Type type, object value)
        {
            var propertyName = Utils.GetKeyFiledName(type);
            ParameterExpression parameter = Expression.Parameter(type, "p");//创建参数p
            MemberExpression member = Expression.PropertyOrField(parameter, propertyName);
            ConstantExpression constant = Expression.Constant(value);//创建常数
            return Expression.Lambda(Expression.Equal(member, constant), parameter);

        }



    }



    }
