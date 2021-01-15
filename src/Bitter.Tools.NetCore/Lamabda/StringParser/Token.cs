using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitter.Tools.Lamabda
{
    /// <summary>
    /// 字符单元
    /// </summary>
    [DebuggerStepThrough]
    [DebuggerDisplay("Text = {Text}, ID = {ID}, Index = {Index}")]
    public struct Token
    {

        #region 字段
        /// <summary>
        /// 空的字符单元
        /// </summary>
        public static readonly Token Empty = new Token();
        private TokenId id;
        private string text;
        private int index;
        private int? hash;
        #endregion


        #region 属性
        /// <summary>
        /// 获取或设置字符类型
        /// </summary>
        public TokenId ID
        {
            get { return id; }
            set
            {
                id = value;
                hash = null;
            }
        }
        /// <summary>
        /// 获取或设置当前字符单元的文本表示
        /// </summary>
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                hash = null;
            }
        }
        /// <summary>
        /// 获取或设置当前字符单元在整体结果中的索引
        /// </summary>
        public int Index
        {
            get { return index; }
            set
            {
                index = value;
                hash = null;
            }
        }
        #endregion


        #region 重写方法
        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null)) return false;
            if (obj is Token)
                return Equals((Token)obj);
            else
                return false;
        }

        /// <summary>
        /// Equalses the specified token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public bool Equals(Token token)
        {
            if (ReferenceEquals(token, null)) return false;
            return ID == token.id && Text == token.Text;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                if (!hash.HasValue)
                {
                    hash = ID.GetHashCode();
                    hash ^= Text.GetHashCode();
                    hash ^= Index.GetHashCode();
                }
                return hash.Value;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return text;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Lenic.DI.Core.Token"/> to <see cref="System.String"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator string(Token value)
        {
            return value.text;
        }
        #endregion

        #region 异常抛出
        // <summary>
        /// 如果当前实例的文本表示与指定的字符串不符, 则抛出异常.
        /// </summary>
        /// <param name="id">待判断的字符串.</param>
        /// <returns>当前实例对象.</returns>
        public Token Throw(TokenId id)
        {
            if (ID != id)
                throw new Exception("当前实例的文本表示与指定的字符串不匹配");

            return this;
        }

        /// <summary>
        /// 如果当前实例的字符类型与指定的字符类型不符, 则抛出异常.
        /// </summary>
        /// <param name="id">待判断的目标类型的字符类型.</param>
        /// <returns>当前实例对象.</returns>
        public Token Throw(string text)
        {
            if (Text != text)
                throw new Exception("当前实例的字符类型与指定的字符类型不匹配");

            return this;
        }
        #endregion
    }
    /// <summary>
    /// 单元类型枚举
    /// </summary>
    public enum TokenId
    {
        /// <summary>
        /// End
        /// </summary>
        End,
        /// <summary>
        /// Identifier
        /// </summary>
        Identifier,
        /// <summary>
        /// String
        /// </summary>
        StringLiteral,
        /// <summary>
        /// Integer Literal
        /// </summary>
        IntegerLiteral,
        /// <summary>
        /// Long Integer Literal
        /// </summary>
        LongIntegerLiteral,
        /// <summary>
        /// Single Real Literal
        /// </summary>
        SingleRealLiteral,
        /// <summary>
        /// Decimal Real Literal
        /// </summary>
        DecimalRealLiteral,
        /// <summary>
        /// Real Literal
        /// </summary>
        RealLiteral,
        /// <summary>
        /// !
        /// </summary>
        Exclamation,
        /// <summary>
        /// %
        /// </summary>
        Percent,
        /// <summary>
        /// &amp;
        /// </summary>
        Amphersand,
        /// <summary>
        /// (
        /// </summary>
        OpenParen,
        /// <summary>
        /// )
        /// </summary>
        CloseParen,
        /// <summary>
        /// *
        /// </summary>
        Asterisk,
        /// <summary>
        /// +
        /// </summary>
        Plus,
        /// <summary>
        /// ,
        /// </summary>
        Comma,
        /// <summary>
        /// -
        /// </summary>
        Minus,
        /// <summary>
        /// .
        /// </summary>
        Dot,
        /// <summary>
        /// /
        /// </summary>
        Slash,
        /// <summary>
        /// :
        /// </summary>
        Colon,
        /// <summary>
        /// &lt;
        /// </summary>
        LessThan,
        /// <summary>
        /// =
        /// </summary>
        Equal,
        /// <summary>
        /// &gt;
        /// </summary>
        GreaterThan,
        /// <summary>
        /// ?
        /// </summary>
        Question,
        /// <summary>
        /// ??
        /// </summary>
        DoubleQuestion,
        /// <summary>
        /// [
        /// </summary>
        OpenBracket,
        /// <summary>
        /// ]
        /// </summary>
        CloseBracket,
        /// <summary>
        /// |
        /// </summary>
        Bar,
        /// <summary>
        /// !=
        /// </summary>
        ExclamationEqual,
        /// <summary>
        /// &amp;&amp;
        /// </summary>
        DoubleAmphersand,
        /// <summary>
        /// &lt;=
        /// </summary>
        LessThanEqual,
        /// <summary>
        /// &lt;&gt; 
        /// </summary>
        LessGreater,
        /// <summary>
        /// ==
        /// </summary>
        DoubleEqual,
        /// <summary>
        /// &gt;=
        /// </summary>
        GreaterThanEqual,
        /// <summary>
        /// ||
        /// </summary>
        DoubleBar,
        /// <summary>
        /// =&gt;
        /// </summary>
        LambdaPrefix,
        /// <summary>
        /// {
        /// </summary>
        OpenBrace,
        /// <summary>
        /// }
        /// </summary>
        CloseBrace,
    }
}
