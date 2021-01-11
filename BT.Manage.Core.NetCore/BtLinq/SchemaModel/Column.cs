using System;
using System.Data;
using System.Reflection;
using BT.Manage.Tools.Attributes;

namespace BT.Manage.Core.SchemaModel
{
    public class Column
    {
        public DataSourceTypes DataSourceType { get; set; }

        public DbType DbType { get; set; }

        [Obsolete("请使用DataSourceType")]
        public bool IsAutoIncreament { get; set; }

        public bool IsKey { get; set; }

        public int MaxLength { get; set; }

        public string Name { get; set; }

        public bool NotNull { get; set; }

        public int Precision { get; set; }

        public PropertyInfo PropertyInfo { get; set; }

        public int Scale { get; set; }

        public Table Table { get; set; }
    }
}