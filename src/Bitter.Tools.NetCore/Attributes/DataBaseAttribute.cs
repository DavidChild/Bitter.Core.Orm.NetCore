using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Bitter.Tools.Attributes
{
    public class DataBaseAttribute : Attribute
    {
        // Methods
        public DataBaseAttribute(string dataBaseName)
        {
            this.Name = dataBaseName;
        }

        // Properties
        public string Name { get;  private set; }
    }

 

}
