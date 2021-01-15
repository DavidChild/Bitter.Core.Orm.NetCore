using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Bitter.Base
{
    /********************************************************************************
    ** auth： davidchild
    ** date： 2016/7/14 15:11:19
    ** desc：
    ** Ver.:  V1.0.0
    ** Copyright (C) 2016 Bitter 版权所有。
    *********************************************************************************/

    [JsonObject(MemberSerialization.OptIn)]
    public class Result
    {


        [JsonIgnore]
        public List<Byte[]> attachments { get; set; }



       
            /// <summary>
            /// 返回冗余的装箱对象
            /// </summary>
            [JsonProperty]
            public dynamic @object { get; set; }

            /// <summary>
            /// 1：成功 0：失败
            /// </summary>
            [JsonProperty]
            public Int32 code { get; set; }

            /// <summary>
            /// HTTP错误状态码
            /// </summary>
            public string errorCode { get; set; }

            /// <summary>
            /// 扩展参数
            /// </summary>
            [JsonProperty]
            public dynamic extendObject { get; set; }

            /// <summary>
            /// APP 版本参数
            /// </summary>
            [JsonProperty]
            public object handelEntity { get; set; }

            /// <summary>
            /// 获取的IsReOperational
            /// </summary>
            public bool IsActionExceted { get; set; }

            /// <summary>
            /// 返回成功或失败的显示值
            /// </summary>
            [JsonProperty]
            public string message { get; set; }


            /// <summary>
            /// 开放平台所使用的openid
            /// </summary>
            [JsonProperty]
            public string openId { get; set; }

            //private page _page { get; set; }
            /// <summary>
            /// page对象
            /// </summary>
            [JsonProperty]
            public page @page
            {
                get;
                set;
                //get 
                //{
                //    if (_page == null) return new page() { totalPages = 0, totalRecords = 0 };
                //    return _page;
                //}

                //set
                //{
                //    _page = value;
                //}


            }
        }
    
        
}