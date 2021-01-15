namespace BT.Manage.Core
{
    public class Condition
    {
        public CompareType CompareType { get; set; }

        public Token Left { get; set; }

        public Token Right { get; set; }
    }
}