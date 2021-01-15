using System;
using System.Linq.Expressions;

namespace BT.Manage.Core
{
    public abstract class TranslateBase
    {
        public Type ElementType { get; set; }

        public ParseResult Result { get; protected set; }

        public abstract void Translate(Expression expression);
    }
}