using AutoMapper;
using System;

namespace Bitter.Base
{
    /********************************************************************************
    ** auth： davidchild
    ** date： 2017/3/24 15:59:20
    ** desc：
    ** Ver.:  V1.0.0
    ** Copyright (C) 2016 Bitter 版权所有。
    *********************************************************************************/

    public class ResultArgumentException : ArgumentNullException
    {
        /// <summary>
        /// 返回冗余的装箱对象
        /// </summary>
        public dynamic @object { get; set; }

        /// <summary>
        /// 成功
        /// </summary>
        public Int32 code
        {
            get { return 0; }
        }

        /// <summary>
        /// 错误状态码
        /// </summary>
        public ErrorCode errorcode { get; set; }

        /// <summary>
        /// 返回成功或失败的显示值
        /// </summary>
        public string message
        {
            get
            {
                return this.Message;
            }
        }

        /// <summary>
        /// 对ResultException 装
        /// </summary>
        /// <returns></returns>
        public Result ToResult()
        {
            var result = Mapper.Map<ResultArgumentException, Result>(this);
            return result;
        }
    }
}