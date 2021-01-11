using System;
using System.Data;
using System.Linq;

namespace BT.Manage.Core.BtLinq.MapManger
{
    public class PropertyConfiguration<T>
    {
        private readonly string _propertyName;
        private Type _propertyType;
        private readonly SchemaModel.Table _table;

        public PropertyConfiguration(SchemaModel.Table table, string propertyName)
        {
            _propertyType = typeof (T);
            _table = table;
            _propertyName = propertyName;
        }

        private SchemaModel.Column GetColumn()
        {
            var local1 = _table.Columns.FirstOrDefault(x => x.Value.PropertyInfo.Name == _propertyName).Value;
            if (local1 == null)
            {
                throw new Exception("未找到" + _propertyName + "属性");
            }
            return local1;
        }

        public PropertyConfiguration<T> MaxLength(int length)
        {
            GetColumn().MaxLength = length;
            return this;
        }

        public PropertyConfiguration<T> Name(string name)
        {
            GetColumn().Name = name;
            return this;
        }

        public PropertyConfiguration<T> Precision(int precision, int scale)
        {
            var column = GetColumn();
            column.Precision = precision;
            column.Scale = scale;
            return this;
        }

        public PropertyConfiguration<T> Type(DbType dbType)
        {
            GetColumn().DbType = dbType;
            return this;
        }
    }
}