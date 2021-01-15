using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using BT.Manage.Core.BtLinq.MapManger;
using BT.Manage.Tools.Attributes;

namespace BT.Manage.Core
{
    public class EntityConfigurationManager
    {
        private static readonly Type _columnAttrType = typeof (ColumnAttribute);
        private static readonly Type _dataBaseAttrType = typeof (DataBaseAttribute);
        private static Type _dataBaseGeneratedAttrType = typeof (IIDbConfig);
        private static readonly Type _keyAttrType = typeof (KeyAttribute);
        private static readonly Type _tableAttrType = typeof (TableName);

        private static readonly Dictionary<Type, SchemaModel.Table> _tableTypeMap =
            new Dictionary<Type, SchemaModel.Table>();

        public EntityConfiguration<T> Entity<T>()
        {
            return new EntityConfiguration<T>();
        }

        private static void GetColumns(SchemaModel.Table table)
        {
            var flag = false;
            var properties = table.Type.GetProperties();
            for (var i = 0; i < properties.Length; i++)
            {
                var column = ToColumn(properties[i]);
                column.Table = table;
                if (column.IsKey)
                {
                    flag = true;
                    table.Key = column;
                }
                table.Columns.Add(column.PropertyInfo.Name, column);
            }
            if (!flag)
            {
                var column2 = table.Columns["id"]; // table.Columns<string, Column>("Id");
                if (column2 != null)
                {
                    column2.IsKey = true;
                    column2.IsAutoIncreament = true;
                    column2.DataSourceType = (DataSourceTypes) 2;
                    table.Key = column2;
                }
            }
        }

        public static SchemaModel.Table GetTable(Type entityType)
        {
            var table = _tableTypeMap[entityType];
            if (table != null)
            {
                return table;
            }
            var dictionary = _tableTypeMap;
            lock (dictionary)
            {
                table = _tableTypeMap[entityType];
                if (table == null)
                {
                    table = ToTable(entityType);
                    GetColumns(table);
                }
                return table;
            }
        }

        public static bool IsEntity(Type type)
        {
            return dbcontent.IsEntity(type);
        }

        internal static SchemaModel.Column ToColumn(PropertyInfo propertyInfo)
        {
            var customAttributes = propertyInfo.GetCustomAttributes(false);
            var attribute =
                (DatabaseGeneratedAttribute) customAttributes.FirstOrDefault(x => x is DatabaseGeneratedAttribute);
            var attribute2 = (DataSourceAttribute) customAttributes.FirstOrDefault(x => x is DataSourceAttribute);
            if ((attribute != null) && (attribute2 != null))
            {
                throw new Exception(
                    @"实体{propertyInfo.DeclaringType.FullName}的列{propertyInfo.Name}同时标注了DatabaseGenerated与DataSource特性，这两种特性不允许同时使用");
            }
            var attribute3 = (ColumnAttribute) propertyInfo.GetCustomAttributes(_columnAttrType, false).FirstOrDefault();
            var attribute4 = (KeyAttribute) propertyInfo.GetCustomAttributes(_keyAttrType, true).FirstOrDefault();
            var column = new SchemaModel.Column
            {
                PropertyInfo = propertyInfo,
                Name = propertyInfo.Name,
                IsKey = attribute4 != null
            };
            var propertyType = propertyInfo.PropertyType;
            var underlyingType = TypeHelper.GetUnderlyingType(propertyType);
            column.NotNull = underlyingType == propertyType;
            if (underlyingType.IsEnum)
            {
                propertyType = Enum.GetUnderlyingType(propertyType);
            }
            column.DbType = ReflectorConsts.TypeMapper[propertyType];
            if (attribute != null)
            {
                switch (attribute.DatabaseGeneratedOption)
                {
                    case DatabaseGeneratedOption.None:
                        column.IsAutoIncreament = false;
                        column.DataSourceType = (DataSourceTypes) 1;
                        break;

                    case DatabaseGeneratedOption.Identity:
                        column.IsAutoIncreament = true;
                        column.DataSourceType = (DataSourceTypes) 2;
                        break;
                }
                column.IsKey = true;
            }
            else if (attribute2 != null)
            {
                column.DataSourceType = attribute2.DataSource;
                if (attribute2.DataSource == (DataSourceTypes) 2)
                {
                    column.IsAutoIncreament = true;
                }
                column.IsKey = true;
            }
            else if (column.IsKey)
            {
                column.IsAutoIncreament = true;
                column.DataSourceType = (DataSourceTypes) 2;
            }
            if (attribute3 != null)
            {
                column.Name = attribute3.Name;
            }
            return column;
        }

        private static SchemaModel.Table ToTable(Type entityType)
        {
            var table = _tableTypeMap[entityType];
            if (table == null)
            {
                var dictionary = _tableTypeMap;
                lock (dictionary)
                {
                    string name;
                    table = _tableTypeMap[entityType];
                    if (table != null)
                    {
                        return table;
                    }
                    var attribute =
                        (TableAttribute) entityType.GetCustomAttributes(_tableAttrType, true).FirstOrDefault();
                    string str2 = null;
                    table = new SchemaModel.Table();
                    if (attribute != null)
                    {
                        name = attribute.Name;
                    }
                    else
                    {
                        name = BtCoreStringHelper.ToPlural(entityType.Name);
                    }
                    var attribute2 =
                        (DataBaseAttribute)
                            entityType.GetCustomAttributes(_dataBaseAttrType, true).FirstOrDefault();
                    if (attribute2 != null)
                    {
                        str2 = attribute2.Name;
                    }
                    table.DataBase = str2;
                    table.Name = name;
                    table.Type = entityType;
                    _tableTypeMap.Add(entityType, table);
                }
            }
            return table;
        }
    }
}