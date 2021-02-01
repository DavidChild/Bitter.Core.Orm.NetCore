using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Bitter.Core;
using Bitter.Tools.Attributes;


namespace Bitter.test
{
    [TableName("t_Grade")]
    public class TGRADEInfo : BaseModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        [Identity]
        [Display(Name = @"主键")]
        public virtual Int32 FID { get; set; }

        /// <summary>
        /// 年级名称
        /// </summary>
        [Display(Name = @"年级名称")]
        public virtual String FName { get; set; }

        /// <summary>
        /// 插入时间
        /// </summary>
        [Display(Name = @"插入时间")]
        public virtual DateTime? FAddTime { get; set; }

    }
}
