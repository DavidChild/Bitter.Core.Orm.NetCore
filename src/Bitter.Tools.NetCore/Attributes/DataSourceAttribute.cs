using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Bitter.Tools.Attributes
{
    public class DataSourceAttribute : Attribute
    {
         public  DataSourceTypes DataSource { get; set; }
    }

 

}
