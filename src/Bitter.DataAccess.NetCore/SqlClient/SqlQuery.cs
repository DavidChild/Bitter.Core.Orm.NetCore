using Bitter.Tools.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Bitter.DataAccess.SqlClient
{
    public class SqlQuery : IQuery
    {
        protected const int COMMAND_TIMEOUT_DEFAULT = 300;
        protected string m_CommandText;
        protected int m_CommandTimeout = 300;
        protected CommandType m_CommandType = CommandType.Text;
        protected SqlQueryParameterCollection m_Parameters = new SqlQueryParameterCollection();
        /// <summary>
        /// 指向哪个操作数据库
        /// </summary>
        public string _targetdb { get; set; }

        public string Targetdb
        {
            get { return _targetdb; }
            set { _targetdb = value; }
        }


        public virtual string CommandText
        {
            get
            {
                return this.m_CommandText;
            }
            set
            {
                this.m_CommandText = value;
            }
        }

        public int CommandTimeout
        {
            get
            {
                return this.m_CommandTimeout;
            }
            set
            {
                if (value < 10)
                {
                    this.m_CommandTimeout = 300;
                    return;
                }
                this.m_CommandTimeout = value;
            }
        }

        public CommandType CommandType
        {
            get
            {
                return this.m_CommandType;
            }
            set
            {
                this.m_CommandType = value;
            }
        }

        public SqlQueryParameterCollection Parameters
        {
            get
            {
                return this.m_Parameters;
            }
            set
            {
                this.m_Parameters = value;
            }
        }

        #region 返回添加sql语句及参数

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
                Bitter.Core.EnumTypeAttribute k = px.AsEnumerable().FirstOrDefault() as Bitter.Core.EnumTypeAttribute;
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
                Bitter.Core.EnumTypeAttribute k = px.AsEnumerable().FirstOrDefault() as Bitter.Core.EnumTypeAttribute;
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
        public static bool CheckedIsIdentity(Type type,string filedName)
        {
            bool bl = false;
            string KeyFiled = string.Empty;    //主键字段
            var sd = type.GetCustomAttributes(true);
            List<PropertyInfo> properties = (from p in type.GetProperties() where p.Name == filedName select p).ToList();
            if (properties==null||properties.Count <= 0) return false;
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
                    Bitter.Core.RelationTableAttribute k = px.AsEnumerable().FirstOrDefault() as Bitter.Core.RelationTableAttribute;
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
                    Bitter.Core.RelationTableAttribute k = px.AsEnumerable().FirstOrDefault() as Bitter.Core.RelationTableAttribute;
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

        /// <summary>
        /// 返回添加sql语句及参数
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="data">类型的实例化对象</param>
        /// <returns>返回添加sql语句及参数</returns>
        public bool ModelSqlAddCommandText<T>(T data)
        {
            string tableName = string.Empty;    //数据库名
            //string commandText = string.Empty;    //sql语句
            //SqlQueryParameterCollection parameters = new SqlQueryParameterCollection(); //sql参数
            string fields = string.Empty;   //字段
            string values = string.Empty;   //值
            bool IsIdentity = false;
            var sd = data.GetType().GetCustomAttributes(true);
            foreach (object attr in sd)
            {
                Bitter.Tools.Attributes.TableName tableNameTmp = attr as Tools.Attributes.TableName;
                if (tableNameTmp != null)
                {
                    tableName = tableNameTmp.name;
                }
            }

            PropertyInfo[] properties = data.GetType().GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                Type propertyType = propertyInfo.PropertyType;

                string FieldName = propertyInfo.Name;
                string displayName = string.Empty;
                string typeName = string.Empty;
                bool isKey = false;
                bool isNull = false;
                bool isIdentity = false;
                if (FieldName.Length > 0 && FieldName.IndexOf('_') == 0) continue; //continue
                object[] attrs = propertyInfo.GetCustomAttributes(true);
                for (int i = 0; i < attrs.Count(); i++)
                {
                    string memberInfoName = attrs.GetValue(i).GetType().Name;
                    if (memberInfoName == "DisplayAttribute")
                    {
                        DisplayAttribute displayAttr = attrs[i] as DisplayAttribute;
                        if (displayAttr != null)
                        {
                            displayName = displayAttr.Name;
                        }
                    }
                    else if (memberInfoName == "KeyAttribute")
                    {
                        KeyAttribute keyAttr = attrs[i] as KeyAttribute;
                        if (keyAttr != null)
                        {
                            isKey = true;
                        }
                    }
                    else if (memberInfoName == "IdentityAttribute")
                    {
                        Bitter.Tools.Attributes.IdentityAttribute keyAttr = attrs[i] as Bitter.Tools.Attributes.IdentityAttribute;
                        if (keyAttr != null)
                        {
                            isIdentity = true;
                            IsIdentity = true;
                        }
                    }
                }
                if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    typeName = propertyType.GetGenericArguments()[0].Name;
                    isNull = true;
                }
                else
                {
                    typeName = propertyType.Name;
                }
                string propertyValue = string.Empty;
                if ((isKey && !isIdentity) || (!isKey))
                {
                    var shortguid = "_" + Bitter.Tools.Utils.GuidExtends.ShortGuid();
                    fields += FieldName + ',';

                    values += "@" + FieldName + shortguid + ',';

                    var val = propertyInfo.GetValue(data, null);

                    this.Parameters.Add("@" + FieldName + shortguid, ((val == null && isNull) || (typeName.ToLower() == "string" && string.IsNullOrWhiteSpace(val == null ? string.Empty : val.ToString()))) ? DBNull.Value : val, SqlTypeString2SqlType(typeName.ToLower()));
                }

                //fields += FieldName + ',';
                //values += "@" + FieldName + ',';

                ////var val =(typeName.ToLower()=="string" && propertyInfo.GetValue(data, null)==null) ? propertyInfo.GetValue(data, null);
                //var val = propertyInfo.GetValue(data, null);

                //this.Parameters.Add("@" + FieldName, ((val == null && isNull) || (typeName.ToLower() == "string" && string.IsNullOrWhiteSpace(val.ToString()))) ? DBNull.Value : val, SqlTypeString2SqlType(typeName.ToLower()));
            }
            var selectParimaryKey = "SELECT @@IDENTITY AS FKey;";
            if (!IsIdentity)
            {
                var FID = SqlQuery.GetKeyFiledName<T>(data);
                var VFID = SqlQuery.GetObjectPropertyValue<T>(data, FID);
                selectParimaryKey = "SELECT '" + VFID + "' AS FKey;";
            }
            this.CommandText += string.Format("INSERT INTO {0} ({1}) VALUES ({2}){3}", tableName, fields.Substring(0, fields.Length - 1), values.Substring(0, values.Length - 1), selectParimaryKey);
            return true;
        }

        /// <summary>
        /// 返回添加sql语句及参数
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="data">类型的实例化对象</param>
        /// <param name="IsIdentity">是否自增长</param>
        /// <returns>返回添加sql语句及参数</returns>
        public bool ModelSqlAddCommandText<T>(T data, out bool IsIdentity)
        {
            string tableName = string.Empty;    //数据库名
            string fields = string.Empty;   //字段
            string values = string.Empty;   //值

            IsIdentity = false;
            var sd = data.GetType().GetCustomAttributes(true);
            for (int i = 0; i < sd.Count(); i++)
            {
                if (sd.GetValue(i).GetType().Name == "TableName")
                {
                    Bitter.Tools.Attributes.TableName tableNameTmp = sd[i] as Tools.Attributes.TableName;
                    if (tableNameTmp != null)
                    {
                        tableName = tableNameTmp.name;
                    }
                }
            }

            PropertyInfo[] properties = data.GetType().GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                Type propertyType = propertyInfo.PropertyType;

                string FieldName = propertyInfo.Name;
                string displayName = string.Empty;
                string typeName = string.Empty;
                bool isKey = false;
                bool isNull = false;
                bool isIdentity = false;
                if (FieldName.Length > 0 && FieldName.IndexOf('_') == 0) continue; //continue
                object[] attrs = propertyInfo.GetCustomAttributes(true);
                for (
                    int i = 0; i < attrs.Count(); i++)
                {
                    string memberInfoName = attrs.GetValue(i).GetType().Name;
                    if (memberInfoName == "DisplayAttribute")
                    {
                        DisplayAttribute displayAttr = attrs[i] as DisplayAttribute;
                        if (displayAttr != null)
                        {
                            displayName = displayAttr.Name;
                        }
                    }
                    else if (memberInfoName == "KeyAttribute")
                    {
                        KeyAttribute keyAttr = attrs[i] as KeyAttribute;
                        if (keyAttr != null)
                        {
                            isKey = true;
                        }
                    }
                    else if (memberInfoName == "IdentityAttribute")
                    {
                        Bitter.Tools.Attributes.IdentityAttribute keyAttr = attrs[i] as Bitter.Tools.Attributes.IdentityAttribute;
                        if (keyAttr != null)
                        {
                            isIdentity = true;
                            IsIdentity = true;
                        }
                    }
                }
                if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    typeName = propertyType.GetGenericArguments()[0].Name;
                    isNull = true;
                }
                else
                {
                    typeName = propertyType.Name;
                }
                string propertyValue = string.Empty;
                if ((isKey && !isIdentity) || (!isKey))
                {
                    var shortguid = "_" + Bitter.Tools.Utils.GuidExtends.ShortGuid();
                    fields += FieldName + ',';
                    values += "@" + FieldName + shortguid + ',';

                    var val = propertyInfo.GetValue(data, null);

                    this.Parameters.Add("@" + FieldName + shortguid, ((val == null && isNull) || (typeName.ToLower() == "string" && string.IsNullOrWhiteSpace(val == null ? string.Empty : val.ToString()))) ? DBNull.Value : val, SqlTypeString2SqlType(typeName.ToLower()));
                }
            }
            var selectParimaryKey = "SELECT @@IDENTITY AS FKey;";
            if (!IsIdentity)
            {
                var FID = SqlQuery.GetKeyFiledName<T>(data);
                var VFID = SqlQuery.GetObjectPropertyValue<T>(data, FID);
                selectParimaryKey = "SELECT '" + VFID + "' AS FKey;";
            }
            this.CommandText = string.Format("INSERT INTO {0} ({1}) VALUES ({2}){3}", tableName, fields.Substring(0, fields.Length - 1), values.Substring(0, values.Length - 1), selectParimaryKey);
            return true;
        }

        /// <summary>
        /// 返回添加sql语句及参数
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="data">类型的实例化对象</param>
        /// <param name="IsIdentity">是否自增长</param>
        /// <returns>返回添加sql语句及参数</returns>
        public bool ModelSqlAddCommandTextForScope<T>(T data)
        {
            string tableName = string.Empty;    //数据库名
            string fields = string.Empty;   //字段
            string values = string.Empty;   //值
            bool IsIdentity = false;
            Dictionary<string, string> dc = new Dictionary<string, string>();
            var sd = data.GetType().GetCustomAttributes(true);
            for (int i = 0; i < sd.Count(); i++)
            {
                if (sd.GetValue(i).GetType().Name == "TableName")
                {
                    Bitter.Tools.Attributes.TableName tableNameTmp = sd[i] as Tools.Attributes.TableName;
                    if (tableNameTmp != null)
                    {
                        tableName = tableNameTmp.name;
                    }
                }
            }

            PropertyInfo[] properties = data.GetType().GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                Type propertyType = propertyInfo.PropertyType;
                string FieldName = propertyInfo.Name;
                string displayName = string.Empty;
                string typeName = string.Empty;
                string relationtable = string.Empty;
                bool isrelationtable = false;
                bool isKey = false;
                bool isNull = false;
                bool isIdentity = false;
                if (FieldName.Length > 0 && FieldName.IndexOf('_') == 0) continue; //continue
                object[] attrs = propertyInfo.GetCustomAttributes(true);
                for (
                    int i = 0; i < attrs.Count(); i++)
                {
                    string memberInfoName = attrs.GetValue(i).GetType().Name;
                    if (memberInfoName == "DisplayAttribute")
                    {
                        DisplayAttribute displayAttr = attrs[i] as DisplayAttribute;
                        if (displayAttr != null)
                        {
                            displayName = displayAttr.Name;
                        }
                    }
                    else if (memberInfoName == "KeyAttribute")
                    {
                        KeyAttribute keyAttr = attrs[i] as KeyAttribute;
                        if (keyAttr != null)
                        {
                            isKey = true;
                        }
                    }
                    else if (memberInfoName == "IdentityAttribute")
                    {
                        Bitter.Tools.Attributes.IdentityAttribute keyAttr = attrs[i] as Bitter.Tools.Attributes.IdentityAttribute;
                        if (keyAttr != null)
                        {
                            isIdentity = true;
                            IsIdentity = true;
                        }
                    }
                    else if (memberInfoName == "RelationTableAttribute")
                    {
                        Bitter.Core.RelationTableAttribute k = attrs[i] as Bitter.Core.RelationTableAttribute;
                        if (k.relationTable != null)
                        {
                            relationtable = GetTableName(k.relationTable);
                            isrelationtable = true;
                        }
                    }
                }
                if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    typeName = propertyType.GetGenericArguments()[0].Name;
                    isNull = true;
                }
                else
                {
                    typeName = propertyType.Name;
                }
                string propertyValue = string.Empty;
                if (isrelationtable)
                {
                    var val = propertyInfo.GetValue(data, null);
                    if ((!string.IsNullOrEmpty(val.ToSafeString())) || val.ToSafeInt32(0) != 0)
                    {
                        var shortguid = "_" + Bitter.Tools.Utils.GuidExtends.ShortGuid();

                        fields += FieldName + ',';
                        values += "@" + FieldName + shortguid + ',';
                        this.Parameters.Add("@" + FieldName + shortguid, ((val == null && isNull) || (typeName.ToLower() == "string" && string.IsNullOrWhiteSpace(val == null ? string.Empty : val.ToString()))) ? DBNull.Value : val, SqlTypeString2SqlType(typeName.ToLower()));
                    }
                    else
                    {
                        fields += FieldName + ',';
                        values += string.Format("(SELECT  TOP 1 SCOPE_IDENTITY() FROM {0})", relationtable) + ',';
                    }
                }
                else if (((isKey && !isIdentity) || (!isKey)) && (!isrelationtable))
                {
                    var shortguid = "_" + Bitter.Tools.Utils.GuidExtends.ShortGuid();
                    fields += FieldName + ',';
                    values += "@" + FieldName + shortguid + ',';

                    var val = propertyInfo.GetValue(data, null);

                    this.Parameters.Add("@" + FieldName + shortguid, ((val == null && isNull) || (typeName.ToLower() == "string" && string.IsNullOrWhiteSpace(val == null ? string.Empty : val.ToString()))) ? DBNull.Value : val, SqlTypeString2SqlType(typeName.ToLower()));
                }
                else if ((isKey && (!isIdentity)) || ((!isKey) && isrelationtable))
                {
                    var val = propertyInfo.GetValue(data, null);
                    if (string.IsNullOrEmpty(val.ToSafeString()) || val.ToSafeInt32(0) != 0)
                    {
                        var shortguid = "_" + Bitter.Tools.Utils.GuidExtends.ShortGuid();

                        fields += FieldName + ',';
                        values += "@" + FieldName + shortguid + ',';
                        this.Parameters.Add("@" + FieldName + shortguid, ((val == null && isNull) || (typeName.ToLower() == "string" && string.IsNullOrWhiteSpace(val == null ? string.Empty : val.ToString()))) ? DBNull.Value : val, SqlTypeString2SqlType(typeName.ToLower()));
                    }
                    else
                    {
                        fields += FieldName + ',';
                        values += string.Format("(SELECT  TOP 1 SCOPE_IDENTITY() FROM {0})", relationtable) + ',';
                    }
                }
            }
            var selectParimaryKey = "SELECT @@IDENTITY AS FKey;";
            if (!IsIdentity)
            {
                var FID = SqlQuery.GetKeyFiledName<T>(data);
                var VFID = SqlQuery.GetObjectPropertyValue<T>(data, FID);
                selectParimaryKey = "SELECT '" + VFID + "' AS FKey;";
            }
            this.CommandText = string.Format("INSERT INTO {0} ({1}) VALUES ({2}){3}", tableName, fields.Substring(0, fields.Length - 1), values.Substring(0, values.Length - 1), selectParimaryKey);
            return true;
        }

        #endregion 返回添加sql语句及参数
        #region 返回修改sql语句及参数

        /// <summary>
        /// 返回删除sql语句及参数
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="data">类型的实例化对象</param>
        /// <returns>返回删除sql语句及参数</returns>
        public bool ModelSqlDeleteCommandText<T>(T data)
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
            StringBuilder deleteSQL = new StringBuilder();  //delete语句
            deleteSQL.Append("DELETE FROM ").Append(tableName);

            StringBuilder whereSQL = new StringBuilder();  //delete条件
            whereSQL.Append(" WHERE ");

            PropertyInfo[] properties = data.GetType().GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                Type propertyType = propertyInfo.PropertyType;

                string FieldName = propertyInfo.Name;
                string displayName = string.Empty;
                string typeName = string.Empty;
                bool isKey = false;
                bool isNull = false;
                if (FieldName.Length > 0 && FieldName.IndexOf('_') == 0) continue; //continue
                object[] attrs = propertyInfo.GetCustomAttributes(true);
                for (int i = 0; i < attrs.Count(); i++)
                {
                    string memberInfoName = attrs.GetValue(i).GetType().Name;
                    if (memberInfoName == "DisplayAttribute")
                    {
                        DisplayAttribute displayAttr = attrs[i] as DisplayAttribute;
                        if (displayAttr != null)
                        {
                            displayName = displayAttr.Name;
                        }
                    }
                    else if (memberInfoName == "KeyAttribute")
                    {
                        KeyAttribute keyAttr = attrs[i] as KeyAttribute;
                        if (keyAttr != null)
                        {
                            isKey = true;
                        }
                    }
                }
                if (isKey)
                {
                    if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        typeName = propertyType.GetGenericArguments()[0].Name;
                        isNull = true;
                    }
                    else
                    {
                        typeName = propertyType.Name;
                    }
                    var shortguid = "_" + Bitter.Tools.Utils.GuidExtends.ShortGuid();
                    var val = propertyInfo.GetValue(data, null);
                    this.Parameters.Add("@" + FieldName + shortguid, ((val == null && isNull) || (typeName.ToLower() == "string" && string.IsNullOrWhiteSpace(val == null ? string.Empty : val.ToString()))) ? DBNull.Value : val, SqlTypeString2SqlType(typeName.ToLower()));
                    whereSQL.AppendFormat("{0}=@{1}", FieldName, FieldName + shortguid);
                    break;
                }
            }
            this.CommandText = string.Format("{0}{1};", deleteSQL.ToString(), whereSQL.ToString());
            return true;
        }

        /// <summary>
        /// 返回修改sql语句及参数
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="data">类型的实例化对象</param>
        /// <param name="updateProperty">不需要修改的字段集合(主键不能传)</param>
        /// <returns>返回修改sql语句及参数</returns>
        public bool ModelSqlModifyCommandText<T>(T data, string[] noModifyField, out bool IsIdentity)
        {
            string tableName = string.Empty;    //数据库名
            IsIdentity = false;
            var sd = data.GetType().GetCustomAttributes(true);
            foreach (object attr in sd)
            {
                Tools.Attributes.TableName tableNameTmp = attr as Tools.Attributes.TableName;
                if (tableNameTmp != null)
                {
                    tableName = tableNameTmp.name;
                }
            }

            StringBuilder updateSQL = new StringBuilder();  //update语句
            updateSQL.Append("UPDATE ").Append(tableName).Append("  SET  ");

            StringBuilder whereSQL = new StringBuilder();  //update语句
            whereSQL.Append(" WHERE ");

            PropertyInfo[] properties = data.GetType().GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                Type propertyType = propertyInfo.PropertyType;

                string FieldName = propertyInfo.Name;
                string displayName = string.Empty;
                string typeName = string.Empty;
                bool isKey = false;
                bool isNull = false;
                if (FieldName.Length > 0 && FieldName.IndexOf('_') == 0) continue; //continue
                if (noModifyField == null || !noModifyField.Contains(FieldName))
                {
                    object[] attrs = propertyInfo.GetCustomAttributes(true);
                    for (int i = 0; i < attrs.Count(); i++)
                    {
                        string memberInfoName = attrs.GetValue(i).GetType().Name;
                        if (memberInfoName == "DisplayAttribute")
                        {
                            DisplayAttribute displayAttr = attrs[i] as DisplayAttribute;
                            if (displayAttr != null)
                            {
                                displayName = displayAttr.Name;
                            }
                        }
                        else if (memberInfoName == "KeyAttribute")
                        {
                            KeyAttribute keyAttr = attrs[i] as KeyAttribute;
                            if (keyAttr != null)
                            {
                                isKey = true;
                            }
                        }
                        else if (memberInfoName == "IdentityAttribute")
                        {
                            Tools.Attributes.IdentityAttribute keyAttr = attrs[i] as Tools.Attributes.IdentityAttribute;
                            if (keyAttr != null)
                            {
                                IsIdentity = true;
                            }
                        }
                    }
                    if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        typeName = propertyType.GetGenericArguments()[0].Name;
                        isNull = true;
                    }
                    else
                    {
                        typeName = propertyType.Name;
                    }
                    var shortguid = "_" + Bitter.Tools.Utils.GuidExtends.ShortGuid();
                    var val = propertyInfo.GetValue(data, null);
                    this.Parameters.Add("@" + FieldName + shortguid, ((val == null && isNull) || (typeName.ToLower() == "string" && string.IsNullOrWhiteSpace(val == null ? string.Empty : val.ToString()))) ? DBNull.Value : val, SqlTypeString2SqlType(typeName.ToLower()));

                    if (isKey)
                    {
                        whereSQL.AppendFormat("{0}=@{1}", FieldName, FieldName + shortguid);
                    }
                    else
                    {
                        updateSQL.AppendFormat("{0}=@{1},", FieldName, FieldName + shortguid);
                    }
                }
            }

            this.CommandText += string.Format("{0}{1};", updateSQL.ToString().Substring(0, updateSQL.ToString().Length - 1), whereSQL.ToString());
            return true;
        }

        public bool ModelSqlModifyCommandText<T>(T data, out bool IsIdentity)
        {
            return ModelSqlModifyCommandText<T>(data, null, out IsIdentity);
        }

        public void ModelSqlModifyCommandText<T>(T data, string[] updateField, string[] whereField)
        {
            var strTableName = data.GetType().GetCustomAttributes(true).SingleOrDefault(p => p.GetType().Name == "TableName");
            if (strTableName != null)
            {
                Tools.Attributes.TableName tableNameTmp = strTableName as Tools.Attributes.TableName;
                if (tableNameTmp != null)
                {
                    strTableName = tableNameTmp.name;
                }

                StringBuilder updateSQL = new StringBuilder();  //update语句
                StringBuilder whereSQL = new StringBuilder();  //update条件语句
                updateSQL.Append("UPDATE ").Append(strTableName).Append("  SET  ");
                whereSQL.Append(" WHERE ");
                PropertyInfo[] properties = data.GetType().GetProperties();
                foreach (PropertyInfo propertyInfo in properties)
                {
                    bool isIdentityKey = false;
                    string IdentityKey = "FID";
                    bool isKey = false;
                    string FieldName = propertyInfo.Name;
                    if (FieldName.Length > 0 && FieldName.IndexOf('_') == 0) continue; //continue
                    #region
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
                                IdentityKey = propertyInfo.Name;
                            }
                        }
                        else if (memberInfoName == "KeyAttribute")
                        {
                            KeyAttribute keyAttr = attrs[i] as KeyAttribute;
                            if (keyAttr != null)
                            {
                                isKey = true;
                            }
                        }
                    }
                    #endregion 返回修改sql语句及参数

                    if (updateField.Contains(FieldName))
                    {
                        var shortguid = "_" + Bitter.Tools.Utils.GuidExtends.ShortGuid();
                        updateSQL.AppendFormat("{0}=@{1},", FieldName, FieldName + shortguid);
                        var val = propertyInfo.GetValue(data);
                        this.Parameters.Add("@" + FieldName + shortguid, val);
                    }
                    if (whereField != null && whereField.Contains(FieldName))
                    {
                        var shortguid = "_" + Bitter.Tools.Utils.GuidExtends.ShortGuid();
                        whereSQL.AppendFormat("{0}=@{1} and ", FieldName, FieldName + shortguid);
                        var val = propertyInfo.GetValue(data);
                        this.Parameters.Add("@" + FieldName + shortguid, val);
                    }
                    else
                    {
                        if (isKey)
                        {
                            var shortguid = "_" + Bitter.Tools.Utils.GuidExtends.ShortGuid();
                            whereSQL.AppendFormat(IdentityKey + "=@{0} and ", FieldName + shortguid);
                            var val = propertyInfo.GetValue(data);
                            this.Parameters.Add("@" + FieldName + shortguid, val);
                        }
                    }
                }
                this.CommandText = string.Format("{0}{1};", updateSQL.ToString().Substring(0, updateSQL.ToString().Length - 1), whereSQL.ToString().Substring(0, whereSQL.ToString().Length - 4));
            }
        }

        /// <summary>
        /// 返回修改sql语句及参数
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="data">类型的实例化对象</param>
        /// <param name="updateProperty">需要修改的字段集合</param>
        /// <returns>返回修改sql语句及参数</returns>
        public bool ModelSqlModifyContainCommandText<T>(T data, string[] updateProperty, out bool IsIdentity)
        {
            string tableName = string.Empty;    //数据库名
            IsIdentity = false;
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
            StringBuilder updateSQL = new StringBuilder();  //update语句
            updateSQL.Append("UPDATE ").Append(tableName).Append("  SET  ");

            StringBuilder whereSQL = new StringBuilder();  //update条件语句

            PropertyInfo[] properties = data.GetType().GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                Type propertyType = propertyInfo.PropertyType;

                string FieldName = propertyInfo.Name;
                string displayName = string.Empty;
                string typeName = string.Empty;
                bool isKey = false;
                bool isNull = false;

                if (FieldName.Length > 0 && FieldName.IndexOf('_') == 0) continue; //continue
                object[] attrs = propertyInfo.GetCustomAttributes(true);
                for (int i = 0; i < attrs.Count(); i++)
                {
                    string memberInfoName = attrs.GetValue(i).GetType().Name;
                    if (memberInfoName == "DisplayAttribute")
                    {
                        DisplayAttribute displayAttr = attrs[i] as DisplayAttribute;
                        if (displayAttr != null)
                        {
                            displayName = displayAttr.Name;
                        }
                    }
                    else if (memberInfoName == "KeyAttribute")
                    {
                        KeyAttribute keyAttr = attrs[i] as KeyAttribute;
                        if (keyAttr != null)
                        {
                            isKey = true;
                        }
                    }
                    else if (memberInfoName == "IdentityAttribute")
                    {
                        Tools.Attributes.IdentityAttribute keyAttr = attrs[i] as Tools.Attributes.IdentityAttribute;
                        if (keyAttr != null)
                        {
                            IsIdentity = true;
                        }
                    }
                }
                if ((updateProperty.Contains(FieldName) && !isKey) || (isKey && whereSQL.Length <= 0))
                {//如果在修改的字段集合内并且不是主键的 或者满足 是主键并且修改条件的长度为0的
                    if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        typeName = propertyType.GetGenericArguments()[0].Name;
                        isNull = true;
                    }
                    else
                    {
                        typeName = propertyType.Name;
                    }
                    var shortguid = "_" + Bitter.Tools.Utils.GuidExtends.ShortGuid();
                    var val = propertyInfo.GetValue(data, null);
                    this.Parameters.Add("@" + FieldName + shortguid, ((val == null && isNull) || (typeName.ToLower() == "string" && string.IsNullOrWhiteSpace(val == null ? string.Empty : val.ToString()))) ? DBNull.Value : val, SqlTypeString2SqlType(typeName.ToLower()));
                    if (isKey)
                    {
                        whereSQL.Append(" WHERE ").AppendFormat("{0}=@{1}", FieldName, FieldName + shortguid);
                    }
                    else
                    {
                        updateSQL.AppendFormat("{0}=@{1},", FieldName, FieldName + shortguid);
                    }
                }
            }
            this.CommandText = string.Format("{0}{1};", updateSQL.ToString().Substring(0, updateSQL.ToString().Length - 1), whereSQL.ToString());
            return true;
        }

        #endregion
        #region 返回删除sql语句及参数
        #endregion
        #region 返回根据主键查询单个实体的sql语句及参数

        /// <summary>
        /// 返回获取分页列表sql语句及参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">实体模型</param>
        /// <param name="selectProperty">需要查询的字段（如果为空则默认查询全部）</param>
        /// <param name="where">查询条件</param>
        /// <param name="order">排序字段</param>
        /// <param name="pageIndex">当前页面</param>
        /// <param name="pageSize">页长</param>
        /// <returns>返回获取分页列表sql语句</returns>
        public bool ModelSqlSelectListCommandText<T>(T data, string selectProperty, string where, string order, int pageIndex, int pageSize, params SqlParameter[] cmdParms)
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
            string sql = string.Empty;//select语句
            if (pageIndex == 1)
            {
                sql = string.Format(@"select top(@num) {0} FROM {1} with(nolock) {2} order by {3}", string.IsNullOrWhiteSpace(selectProperty) ? "*" : selectProperty, tableName, string.IsNullOrWhiteSpace(where) ? string.Empty : string.Format(" where {0} ", where), order);
                this.Parameters.Add("@num", pageSize, SqlDbType.Int, 4);
            }
            else
            {
                sql = string.Format(@"SELECT * FROM ( SELECT {0},row_number() over(order by {3}) as [num] FROM {1} with(nolock) {2} ) as [tab] where [num] between @Start and @End;", string.IsNullOrWhiteSpace(selectProperty) ? "*" : selectProperty, tableName, string.IsNullOrWhiteSpace(where) ? "" : string.Format(" where {0} ", where), order);
                this.Parameters.Add("@Start", ((pageIndex - 1) * pageSize + 1), SqlDbType.Int, 4);
                this.Parameters.Add("@End", (pageIndex * pageSize), SqlDbType.Int, 4);
            }
            if (cmdParms != null)
            {
                foreach (SqlParameter parameter in cmdParms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    this.Parameters.Add(parameter);
                }
            }
            this.CommandText = sql;
            return true;
        }

        /// <summary>
        /// 返回获取获取总数sql语句及参数
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="data">类型的实例化对象</param>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public bool ModelSqlSelectListCountCommandText<T>(T data, string where, params SqlParameter[] cmdParms)
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
            StringBuilder selectSQL = new StringBuilder();  //select语句
            selectSQL.Append(string.Format("SELECT COUNT(0) FROM {0} with(nolock)", tableName));
            if (!string.IsNullOrWhiteSpace(where)) selectSQL.Append(string.Format(" WHERE {0}", where));
            this.CommandText = string.Format("{0};", selectSQL.ToString());
            if (cmdParms != null)
            {
                foreach (SqlParameter parameter in cmdParms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    this.Parameters.Add(parameter);
                }
            }
            return true;
        }

        /// <summary>
        /// 根据条件返回查询实体集合
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="data">实体对象</param>
        /// <param name="where">要查询的字段</param>
        /// <param name="cmdParms">字段参数</param>
        /// <returns></returns>
        public bool ModelSqlSelectModelCommandText<T>(T data, string where, params SqlParameter[] cmdParms)
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
            StringBuilder selectSQL = new StringBuilder();  //select语句
            selectSQL.Append(string.Format("SELECT * FROM {0}", tableName));
            if (!string.IsNullOrWhiteSpace(where)) selectSQL.Append(string.Format(" WHERE {0}", where));
            this.CommandText = string.Format("{0};", selectSQL.ToString());
            if (cmdParms != null)
            {
                foreach (SqlParameter parameter in cmdParms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    this.Parameters.Add(parameter);
                }
            }
            return true;
        }

        /// <summary>
        /// 返回根据主键查询单个实体的sql语句及参数
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="data">类型的实例化对象</param>
        /// <param name="KeyId">要查询的主键值</param>
        /// <returns></returns>
        public bool ModelSqlSelectSingleCommandText<T>(T data, int KeyId)
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
            StringBuilder seleteSQL = new StringBuilder();  //selete语句
            seleteSQL.Append("SELECT * FROM ").Append(tableName);

            StringBuilder whereSQL = new StringBuilder();  //selete条件
            whereSQL.Append(" WHERE ");

            PropertyInfo[] properties = data.GetType().GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                Type propertyType = propertyInfo.PropertyType;

                string FieldName = propertyInfo.Name;
                string displayName = string.Empty;
                string typeName = string.Empty;
                bool isKey = false;
                bool isNull = false;
                if (FieldName.Length > 0 && FieldName.IndexOf('_') == 0) continue; //continue
                object[] attrs = propertyInfo.GetCustomAttributes(true);
                for (int i = 0; i < attrs.Count(); i++)
                {
                    string memberInfoName = attrs.GetValue(i).GetType().Name;
                    if (memberInfoName == "DisplayAttribute")
                    {
                        DisplayAttribute displayAttr = attrs[i] as DisplayAttribute;
                        if (displayAttr != null)
                        {
                            displayName = displayAttr.Name;
                        }
                    }
                    else if (memberInfoName == "KeyAttribute")
                    {
                        KeyAttribute keyAttr = attrs[i] as KeyAttribute;
                        if (keyAttr != null)
                        {
                            isKey = true;
                        }
                    }
                }
                if (isKey)
                {
                    if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        typeName = propertyType.GetGenericArguments()[0].Name;
                        isNull = true;
                    }
                    else
                    {
                        typeName = propertyType.Name;
                    }
                    this.Parameters.Add("@" + FieldName, KeyId, SqlTypeString2SqlType(typeName.ToLower()));
                    whereSQL.AppendFormat("{0}=@{0}", FieldName);
                    break;
                }
            }
            this.CommandText = string.Format("{0}{1};", seleteSQL.ToString(), whereSQL.ToString());
            return true;
        }

        #endregion
        #region 返回获取获取总数sql语句及参数
        #endregion
        #region 返回获取分页列表sql语句及参数
        #endregion
        #region 根据model获取表名（只获取表名）
        #endregion
        #region 根据Type获取表名（只获取表名）
        #endregion
        #region
        #endregion
        #region 反射属性值
        #endregion
        #region 根据model获取主键字段
        #endregion
        #region 根据model获取显示名称
        #endregion
    }
}