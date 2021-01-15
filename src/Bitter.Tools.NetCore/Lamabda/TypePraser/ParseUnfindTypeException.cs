using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitter.Tools.Lamabda
{
    /// <summary>
    /// Type 异常类
    /// </summary>
    [Serializable]
    [DebuggerStepThrough]
    public class ParseUnfindTypeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParseUnfindTypeException"/> class.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="errorIndex">Index of the error.</param>
        public ParseUnfindTypeException(string typeName, int errorIndex)
            : base(string.Format("{0} in the vicinity of the type \"{1}\" not found", errorIndex, typeName))
        {
        }
    }
}
