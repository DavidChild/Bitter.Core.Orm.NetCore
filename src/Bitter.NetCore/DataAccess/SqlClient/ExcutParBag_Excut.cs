using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Bitter.Core
{
    internal class ExcutParBag_Excut : ExcutParBag
    {
        public string commandText { get; set; }
        public List<dynamic> dynamicParma { get; set; }

        public object defaultValue { get; set; } = null;
         
    }
}
