using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitter.Base
{
    /// <summary>
    /// 
    /// </summary>
    public class UserFriendlyException : Exception
    {

        public string message { private get; set; }

        public override string Message
        {
            get
            {
                return this.message;
            }
        }

        public UserFriendlyException(string _message)
        {
            message = _message;
        }
    }
}
