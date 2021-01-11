using System.Collections.Generic;
using System.Reflection;

namespace BT.Manage.Core
{
    public class ColumnConverter
    {
        public ColumnConverter(MemberInfo memberInfo, List<object> parameters) : this(memberInfo, parameters, false)
        {
        }

        public ColumnConverter(MemberInfo memberInfo, List<object> parameters, bool isLeftColumn)
        {
            MemberInfo = memberInfo;
            Parameters = parameters;
            IsInstanceColumn = isLeftColumn;
        }

        public bool IsInstanceColumn { get; set; }

        public MemberInfo MemberInfo { get; set; }

        public List<object> Parameters { get; set; }
    }
}