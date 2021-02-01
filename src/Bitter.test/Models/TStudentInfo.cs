using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Bitter.Core;
using Bitter.Tools.Attributes;

namespace Bitter.test
{
    [TableName("t_Student")]
    public class TStudentInfo : BaseModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        [Identity]
        [Display(Name = @"主键")]
        public virtual Int32 FID { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Display(Name = @"姓名")]
        public virtual String FName { get; set; }



        /// <summary>
        /// 姓名
        /// </summary>
        [Display(Name = @"年龄")]
        public virtual Int32? FAage { get; set; }

        /// <summary>
        /// 班级
        /// </summary>
        [Display(Name = @"班级")]
        public virtual Int32? FClassId { get; set; }

        /// <summary>
        /// 插入时间
        /// </summary>
        [Display(Name = @"插入时间")]
        public virtual DateTime? FAddTime { get; set; }

    }
}
