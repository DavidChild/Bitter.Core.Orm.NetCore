using System;
using System.Collections.Generic;
using System.Text;

namespace Bitter.Core
{
    public class FiledProperty
    {
        public string filedName { get; set; }
        public Type type { get; set; }

        public bool isIdentity { get; set; } = false;

        public string displayName { get; set; }=string.Empty;

        public bool isKey { get; set; } =false;
        
        public bool isNull { get; set; } = false;

        public string typeName { get; set; }

        public object value { get; set; }
    }
}
