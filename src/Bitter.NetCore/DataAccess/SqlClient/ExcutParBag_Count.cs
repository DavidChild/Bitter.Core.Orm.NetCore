using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Bitter.Core
{ 
    internal class ExcutParBag_Count : ExcutParBag
    {
        public LambdaExpression condition { get; set; }
         
    }
}
