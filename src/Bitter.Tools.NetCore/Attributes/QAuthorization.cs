using System;

namespace Bitter.Tools.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    //[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    //[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
    public class QAuthorization : System.Attribute
    {
        public string functionName;

        public QAuthorization(string functionName)
        {
            this.functionName = functionName;
        }

        internal bool AllowExecute(string httpMethod)
        {
            bool result;
            if (functionName == "Test")
            {
                result = false;
            }
            else
            {
                result = true;
            }
            return result;
        }
    }
}