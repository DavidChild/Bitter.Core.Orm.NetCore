using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Bitter.Core
{
    internal class ExcutParBag_Select: ExcutParBag
    {
        public LambdaExpression condition { get; set; }
        public List<OrderPair> orders { get; set; }

        public List<string> selectColumns { get; set; }
    
        public int? topSize { get; set; }
         
    }
}
