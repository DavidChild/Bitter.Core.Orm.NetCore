using System;
using System.Collections.Generic;

namespace BT.Manage.Core.SchemaModel
{
    public class Table
    {
        public Table()
        {
            Columns = new Dictionary<string, Column>();
        }

        public Dictionary<string, Column> Columns { get; private set; }

        public string DataBase { get; set; }

        public Column Key { get; set; }

        public string Name { get; set; }

        public Type Type { get; set; }
    }
}