using System;
using System.Collections.Generic;

namespace BT.Manage.Core
{
    public class TranslateContext
    {
        public TranslateContext()
        {
            Joins = new Dictionary<string, Join>();
            Columns = new Dictionary<string, Column>();
        }

        public Dictionary<string, Column> Columns { get; private set; }

        public Type EntityType { get; set; }

        public Dictionary<string, Join> Joins { get; private set; }
    }
}