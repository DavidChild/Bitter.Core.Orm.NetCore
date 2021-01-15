using Bitter.Tools.Helper;
using Bitter.Tools.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Bitter.Core
{
    /********************************************************************************
    ** auth： davidchild
    ** date： 2017/1/9 13:30:21
    ** desc：
    ** Ver.:  V1.0.0
    ** Copyright (C) 2016 Bitter 版权所有。
    *********************************************************************************/

    public class BaseModel
    {
        //public HashSet<string> _ModifiedPropertyNames { get; set; } = new HashSet<string>();
        internal string _targetdb { get; set; }
       // public event PropertyChangedEventHandler PropertyChanged;
    }
}