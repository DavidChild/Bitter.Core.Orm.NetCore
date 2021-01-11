using System;
using System.Collections.Generic;
using System.Reflection;

namespace BT.Manage.Core
{
    public class Column
    {
        public Column()
        {
            Converters = new Stack<ColumnConverter>();
            ConverterParameters = new List<object>();
        }

        public string Alias { get; set; }

        public string Converter { get; set; }

        public List<object> ConverterParameters { get; private set; }

        public Stack<ColumnConverter> Converters { get; private set; }

        public Type DataType { get; set; }

        public MemberInfo MemberInfo { get; set; }

        public string Name { get; set; }

        public Table Table { get; set; }

        public override string ToString()
        {
            return @"{this.Table.Name}.{this.Name} AS {this.Alias}";
        }
    }
}