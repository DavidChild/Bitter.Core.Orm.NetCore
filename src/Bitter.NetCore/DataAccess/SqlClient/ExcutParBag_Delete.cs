using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
namespace Bitter.Core
{
    internal class ExcutParBag_Delete: ExcutParBag 
    {
        public LambdaExpression condition { get; set; }

    }
}
