using System;

namespace Bitter.Tools.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
    public class TableName : System.Attribute
    {
        public string name;


        public Type dbName;

        public TableName(string name)
        {
            this.name = name;
        }
        //支持跨库
        public TableName(string name,  Type dbName)
        {
            this.name = name;
            this.dbName = dbName;
        }
    }
}  