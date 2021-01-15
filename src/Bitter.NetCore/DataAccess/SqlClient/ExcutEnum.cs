using System;
using System.Collections.Generic;
using System.Text;

namespace Bitter.Core
{
   internal enum ExcutEnum
    {
        /// <summary>
        /// 分页操作
        /// </summary>
        PageQuery = 0,
        /// <summary>
        ///  执行操作   
        /// </summary>
        ExcutQuery = 1,
        /// <summary>
        /// 删除操作
        /// </summary>
        Delete = 2,
        /// <summary>
        /// 检索数据但并非分页
        /// </summary>
        Select = 3,
      
        /// <summary>
        /// 更新操作
        /// </summary>
        Update = 4,
        /// <summary>
        /// 插入操作
        /// </summary>
        Insert = 5,
        /// <summary>
        /// 检索总数量
        /// </summary>
        Count = 6
    }
}
