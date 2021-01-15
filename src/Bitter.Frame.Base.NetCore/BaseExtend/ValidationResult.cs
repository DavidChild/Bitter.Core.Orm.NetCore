using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitter.Base
{
    /// <summary>
    /// 验证消息结果
    /// </summary>
     public class ValidationResult
    {
         /// <summary>
         /// 字段名称
         /// </summary>
         public string Member { get; set; }
         /// <summary>
         /// 验证错误消息
         /// </summary>
         public string Message { get; set; }
         /// <summary>
         /// 模块名称
         /// </summary>
         public string ModuleName { get; set; }

         /// <summary>
         ///验证结果集 
         /// </summary>
         public ValidationResult()
         {

         }
         /// <summary>
         /// 验证结果集
         /// </summary>
         /// <param name="_member">字段名称</param>
         /// <param name="_message">验证消息</param>
         public ValidationResult(string _member, string _message)
         {
             Member = _member;
             Message = _message;
         }
         /// <summary>
         /// 验证结果集
         /// </summary>
         /// <param name="_member"></param>
         /// <param name="_message"></param>
         /// <param name="_modulename"></param>
         public ValidationResult(string _member, string _message, string _modulename)
         {
             Member = _member;
             Message = _message;
             ModuleName = _modulename;
         }
         /// <summary>
         /// 验证结果集
         /// </summary>
         /// <param name="_message">验证消息</param>
         public ValidationResult(string _message)
         {
             Message = _message;
         }
    }
}
