using System;

namespace Bitter.Core
{
    /********************************************************************************
    ** auth： davidchild
    ** date： 2017/1/9 14:20:12
    ** desc： Number
    ** Ver.:  V1.0.0
    ** Copyright (C) 2016 Bitter 版权所有。
    *********************************************************************************/

    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
    public class RelationTableAttribute : System.Attribute
    {
        public Type relationTable;

        /// <summary>
        /// 关联表
        /// </summary>
        /// <param name="ForeignTable"></param>
        public RelationTableAttribute(Type relationTable)
        {
            this.relationTable = relationTable;
        }
    }
}