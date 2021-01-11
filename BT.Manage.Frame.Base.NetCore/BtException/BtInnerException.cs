using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BT.Manage.Frame.Base
{
   
       [Serializable]
       public class BtInnerException : Exception
       {
           public BtInnerException()
               : this("An exception occurred in the persistence layer.")
           {
           }

           public BtInnerException(string message)
               : base(message)
           {
           }

           public BtInnerException(Exception innerException)
               : base(innerException.Message, innerException)
           {
           }

           public BtInnerException(string message, Exception innerException)
               : base(message, innerException)
           {
           }
       }
    
}
