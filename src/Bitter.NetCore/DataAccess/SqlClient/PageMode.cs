using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitter.Core
{
   public enum PageMode
    {
        /// <summary>
        /// 通过使用开窗函数获取条件记录总数
        /// 优势:一个SQL语句中,直接是用开窗行数获取记录总数
        /// </summary>
        CountOver=1,
        
         /// <summary>
         /// 通过普通的方式来获取记录总数
         /// 先SelectCount,然后取记录列
         /// </summary>
        SelectCount=2
         
    }
}
