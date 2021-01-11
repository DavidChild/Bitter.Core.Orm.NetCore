using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using BT.Manage.Tools.Attributes;

namespace BT.Manage.Core.BtLinq.MapManger
{
    public class EntityConfiguration<T>
    {
        public string propertyName { get; set; }

        public EntityConfiguration<T> Key<TKey>(Expression<Func<T, TKey>> selector, DataSourceTypes dataSourceType)
        {
            var column = Parse(selector);
            column.DataSourceType = dataSourceType;
            column.Table.Key = column;
            column.IsKey = true;
            return this;
        }

        private SchemaModel.Column Parse<TKey>(Expression<Func<T, TKey>> selector)
        {
            var body = (MemberExpression) selector.Body;
            var member = (PropertyInfo) body.Member;
            var propertyName = member.Name;
            return
                EntityConfigurationManager.GetTable(((ParameterExpression) body.Expression).Type)
                    .Columns.FirstOrDefault(x => x.Value.PropertyInfo.Name == propertyName)
                    .Value;
        }

        public PropertyConfiguration<TProperty> Property<TProperty>(Expression<Func<T, TProperty>> selector)
        {
            return new PropertyConfiguration<TProperty>(EntityConfigurationManager.GetTable(typeof (T)),
                ((MemberExpression) selector.Body).Member.Name);
        }

        public EntityConfiguration<T> TableName(string tableName)
        {
            EntityConfigurationManager.GetTable(typeof (T)).Name = tableName;
            return this;
        }
    }
}