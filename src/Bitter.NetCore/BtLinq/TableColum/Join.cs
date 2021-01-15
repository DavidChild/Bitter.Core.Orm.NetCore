namespace BT.Manage.Core
{
    public class Join
    {
        public JoinType JoinType { get; set; }

        public Column Left { get; set; }

        public Column Right { get; set; }

        public override string ToString()
        {
            string[] textArray1 =
            {
                Left.Table.Name, " ", JoinType.ToString(), " join ", Right.Table.Name, " on ",
                Left.Name, " = ", Right.Name
            };
            return string.Concat(textArray1);
        }
    }
}