using System;
using System.Collections.Generic;

namespace BT.Manage.Core
{
    public class BuilderContext
    {
        public Dictionary<string, Column> AggregationColumns { get; set; }

        public List<Column> Columns { get; set; }

        public IList<Token> Conditions { get; set; }

        public bool Distinct { get; set; }

        public Type ElementType { get; set; }

        public Dictionary<string, Join> Joins { get; set; }

        public List<string> NoLockTables { get; set; }

        public bool Pager { get; set; }

        public int Skip { get; set; }

        public List<KeyValuePair<string, Column>> SortColumns { get; set; }

        public SqlType SqlType { get; set; }

        public int Take { get; set; }

        public Dictionary<string, object> UpdateResult { get; set; }
    }
}