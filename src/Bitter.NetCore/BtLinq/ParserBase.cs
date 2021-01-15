using System;
using System.Linq.Expressions;

namespace BT.Manage.Core
{
    public abstract class ParserBase
    {
        public Type ElementType { get; set; }

        public ParseResult Result { get; protected set; }

        public abstract void Parse(Expression expression);
    }
}