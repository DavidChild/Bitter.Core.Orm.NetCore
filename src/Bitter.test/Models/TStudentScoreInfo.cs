using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Bitter.Core;
using Bitter.Tools.Attributes;

namespace Bitter.test
{
    [TableName("t_StudentScore")]
    public class TStudentScoreInfo : BaseModel
    {
     
     
            /// <summary>
            /// 主键
            /// </summary>
            [Key]
            [Identity]
            [Display(Name = @"主键")]
            public virtual Int32 FID { get; set; }

            /// <summary>
            /// 
            /// </summary>
            [Display(Name = @"学生Id")]
            public virtual Int32? FStudentId { get; set; }



            /// <summary>
            /// 
            /// </summary>
            [Display(Name = @"学分")]
            public virtual Int32? FScore { get; set; }

            /// <summary>
            /// 插入时间
            /// </summary>
            [Display(Name = @"插入时间")]
            public virtual DateTime? FAddTime { get; set; }

        }
    }

