namespace Bitter.Tools.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class ReadDbName : System.Attribute
    {
        public string name;

        public ReadDbName(string ReadDbConnectName)
        {
            this.name = ReadDbConnectName;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class WriteDbName : System.Attribute
    {
        public string name;

        public WriteDbName(string WriteDbConnectName)
        {
            this.name = WriteDbConnectName;
        }
    }
}