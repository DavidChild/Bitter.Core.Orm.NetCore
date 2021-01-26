using Bitter.Tools.Utils;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Bitter.Core
{
    /********************************************************************************
    ** auth： davidchild
    ** date： 2017/1/9 13:36:31
    ** desc： 对 ExpressionVisitor 进行扩展
    ** Ver.:  V1.0.0
    ** Copyright (C) 2016 Bitter 版权所有。
    *********************************************************************************/

    internal class BtConditionBuilder : BtExpressionVisitor
    {
        private List<object> m_arguments;
        private Stack<string> m_conditionParts;

        public object[] Arguments { get; private set; }
        public string Condition { get; private set; }

        public void Build(Expression expression)
        {
            BtPartialEvaluator evaluator = new BtPartialEvaluator();
            Expression evaluatedExpression = evaluator.Eval(expression);

            this.m_arguments = new List<object>();
            this.m_conditionParts = new Stack<string>();

            this.Visit(evaluatedExpression);

            this.Arguments = this.m_arguments.ToArray();
            this.Condition = this.m_conditionParts.Count > 0 ? this.m_conditionParts.Pop() : null;
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            if (b == null) return b;
            string opr;
            switch (b.NodeType)
            {
                case ExpressionType.Equal:
                    opr = "=";
                    break;

                case ExpressionType.NotEqual:
                    opr = "<>";
                    break;

                case ExpressionType.GreaterThan:
                    opr = ">";
                    break;

                case ExpressionType.GreaterThanOrEqual:
                    opr = ">=";
                    break;

                case ExpressionType.LessThan:
                    opr = "<";
                    break;

                case ExpressionType.LessThanOrEqual:
                    opr = "<=";
                    break;

                case ExpressionType.And:
                    opr = "AND";
                    break;

                case ExpressionType.AndAlso:
                    opr = "AND";
                    break;

                case ExpressionType.Or:
                    opr = "OR";
                    break;

                case ExpressionType.OrElse:
                    opr = "OR";
                    break;

                case ExpressionType.Add:
                    opr = "+";
                    break;

                case ExpressionType.Subtract:
                    opr = "-";
                    break;

                case ExpressionType.Multiply:
                    opr = "*";
                    break;

                case ExpressionType.Divide:
                    opr = "/";
                    break;

                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.SubtractChecked:
                    opr = "-";
                    break;

                case ExpressionType.MultiplyChecked:
                    opr = "*";
                    break;

                case ExpressionType.Modulo:
                    opr = "%";
                    break;

                case ExpressionType.Coalesce:
                    opr = "??";
                    break;

                case ExpressionType.RightShift:
                    opr = ">>";
                    break;

                case ExpressionType.LeftShift:
                    opr = "<<";
                    break;

                case ExpressionType.ExclusiveOr:
                    opr = "^";
                    break;

                default:
                    throw new NotSupportedException(b.NodeType + " 存在不支持的运算符.");
            }
            this.Visit(b.Left);
            this.Visit(b.Right);
            string right = this.m_conditionParts.Pop();
            string left = this.m_conditionParts.Pop();
            string condition = String.Format("({0} {1} {2})", left, opr, right);
            this.m_conditionParts.Push(condition);
            return b;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            if (c == null) return c;

            this.m_arguments.Add(c.Value);
            this.m_conditionParts.Push(String.Format("{{{0}}}", this.m_arguments.Count - 1));

            return c;
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            if (m == null) return m;
            PropertyInfo propertyInfo = m.Member as PropertyInfo;
            if (propertyInfo == null) return m;
            this.m_conditionParts.Push(String.Format("{0}", propertyInfo.Name));
            return m;
        }

        /// <summary>
        /// 监视方法重写
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m == null) return m;
            string right = string.Empty;
            string left = string.Empty;
            string format;
            switch (m.Method.Name)
            {
                case "StartsWith":
                    format = "({0} LIKE {1}+'%')";
                    this.Visit(m.Object);
                    this.Visit(m.Arguments[0]);
                    right = this.m_conditionParts.Pop();
                    left = this.m_conditionParts.Pop();
                    break;

                case "NotLike":
                    format = "({0} NOT LIKE '%'+{1}+'%')";
                    this.Visit(m.Arguments[0]);
                    this.Visit(m.Arguments[1]);
                    right = this.m_conditionParts.Pop();
                    left = this.m_conditionParts.Pop();
                    break;

                case "Like":
                    format = "({0} LIKE '%'+{1}+'%')";
                    this.Visit(m.Arguments[0]);
                    this.Visit(m.Arguments[1]);
                    right = this.m_conditionParts.Pop();
                    left = this.m_conditionParts.Pop();
                    break;

                case "Contains":
                    format = "({0} LIKE '%'+{1}+'%')";
                    this.Visit(m.Object);
                    this.Visit(m.Arguments[0]);
                    right = this.m_conditionParts.Pop();
                    left = this.m_conditionParts.Pop();
                    break;

                case "EndsWith":
                    format = "({0} LIKE '%'+{1})";
                    this.Visit(m.Object);
                    this.Visit(m.Arguments[0]);
                    right = this.m_conditionParts.Pop();
                    left = this.m_conditionParts.Pop();
                    break;

                case "IsNull":
                    format = string.Format("ISNULL({0},'{1}')", ((MemberExpression)m.Arguments[0]).Member.Name, ((ConstantExpression)m.Arguments[1]).Value);
                    break;

                //case "IsNull":
                //    format = string.Format("ISNULL({0},{1})", ((MemberExpression)(((UnaryExpression)m.Arguments[0]).Operand)).Member.Name, ((ConstantExpression)m.Arguments[1]).Value);
                //    break;

                case "In":
                    format = "({0} In(" + ((ConstantExpression)m.Arguments[1]).Value.SQLInString() + "))";
                    this.Visit(m.Arguments[0]);
                    left = this.m_conditionParts.Pop();
                    break;

                case "NotIn":
                    format = "({0} NOT IN(" + ((ConstantExpression)m.Arguments[1]).Value.SQLInString() + "))";
                    this.Visit(m.Arguments[0]);
                    left = this.m_conditionParts.Pop();
                    break;

                default:
                    throw new NotSupportedException(m.NodeType + " is not supported!");
            }
            this.m_conditionParts.Push(String.Format(format, left, right));
            return m;
        }
    }
}