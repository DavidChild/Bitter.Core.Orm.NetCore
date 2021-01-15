using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitter.Core
{
    public class DbDefaultConfig:IDbConfig
    {
        public string DbConfig
        {
            get { return "AkData"; }
           
        }
    }
}
