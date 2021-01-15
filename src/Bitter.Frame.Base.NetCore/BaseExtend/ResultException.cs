using AutoMapper;
using Bitter.Tools.Helper;
using System;

namespace Bitter.Base
{
    /// <summary>
    /// 功能描述：ResultException 创 建 者：徐燕 创建日期：2017/4/6 11:53:58
    /// </summary>
    public class ResultException : Exception
    {
        private string _message;

        public ResultException(string meg)
        {
            
            
            string[] lst = StringHelper.GetSplitException(meg);
            this.errorCode = lst[1];
            this._message = lst[0];
             
        }

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

        ///// <summary>
        ///// 错误状态码
        ///// </summary>
        public string errorCode { get; set; }

        /// <summary>
        /// 返回成功或失败的显示值
        /// </summary>
        public string message
        {
            get
            {
                if (string.IsNullOrEmpty(_message))
                {
                    return this.Message;
                }
                else
                {
                    return _message;
                }
            }
        }

        /// <summary>
        /// 对ResultException 装
        /// </summary>
        /// <returns></returns>
        public virtual Result ToResult()
        {
            var result = Mapper.Map<ResultException, Result>(this);
            return result;
        }
    }
}