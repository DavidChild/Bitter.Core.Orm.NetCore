﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
namespace BT.Manage.Core
{
    internal class ExcutParBag_Delete: ExcutParBag 
    {
        public LambdaExpression condition { get; set; }

    }
}
