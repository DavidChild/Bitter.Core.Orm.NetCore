using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Bitter.Core;
using Bitter.Tools.Attributes;

namespace Bitter.test
{
    
    public class TScoreSearchDto
    {

            /// <summary>
            /// 分数
            /// </summary>
            public  Int32 FScore { get; set; }

            /// <summary>
            /// 学生名称
            /// </summary>
            [Display(Name = @"班级名称")]
            public  String studentName { get; set; }

            /// <summary>
            /// 班级名称
            /// </summary>
            public  String className { get; set; }
            /// <summary>
            /// 年级名称
            /// </summary>
            public String gradeName { get; set; }



    }
}
