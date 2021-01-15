using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Bitter.Core
{
    internal class ExcutParBag_Page : ExcutParBag
    {
        /// <summary>
        /// with 子语句
        /// </summary>
        public string _preWith { get; set; }


        public PageMode _pageMode { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public StringBuilder orderBy = new StringBuilder("");

        /// <summary>
        /// 分页Index
        /// </summary>
        public Int32 pageIndex { get; set; }

        /// <summary>
        /// 分页Size
        /// </summary>
        public Int32 pageSize { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string pageColumns { get; set; }
        /// <summary>
        /// 是否已进行分页查询
        /// </summary>
        public bool isPage { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        public string commandText { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string pageTableName { get; set; }


        public List<dynamic> dynamics { get; set; } = new List<dynamic>();
        /// <summary>
        /// setWhere
        /// </summary>
        public StringBuilder whereBuiler = new StringBuilder("1=1");
    }
}
