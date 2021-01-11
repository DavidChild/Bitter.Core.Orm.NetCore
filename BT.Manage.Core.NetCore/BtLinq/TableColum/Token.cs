using System;

namespace BT.Manage.Core
{
    public class Token
    {
        private Token()
        {
        }

        public Column Column { get; private set; }

        public Condition Condition { get; private set; }

        public object Object { get; private set; }

        public TokenType Type { get; private set; }

        public static Token Create(object obj)
        {
            if (obj is Token || obj is Column || obj is Condition)
            {
                throw new Exception();
            }
            return new Token
            {
                Object = obj,
                Type = TokenType.Object
            };
        }

        public static Token Create(Column column)
        {
            return new Token
            {
                Column = column,
                Type = TokenType.Column
            };
        }

        public static Token Create(Condition obj)
        {
            return new Token
            {
                Condition = obj,
                Type = TokenType.Condition
            };
        }

        internal static Token CreateNull()
        {
            return new Token
            {
                Object = null,
                Type = TokenType.Object
            };
        }

        public bool GetBool()
        {
            return (bool) Object;
        }

        public bool IsBool()
        {
            if (Type != TokenType.Object)
            {
                return false;
            }
            return Object is bool || Object is bool?;
        }

        public bool IsNull()
        {
            return (Type == TokenType.Object) && (Object == null);
        }
    }
}