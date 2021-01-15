using Bitter.Tools;
using Bitter.Tools.Helper;
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

    [JsonObject(MemberSerialization.OptOut)]
    public class ResponeResult : BaseResponseEntity
    {

        public ResponeResult()
        { }
        [JsonProperty]
        public object @object { get; set; }

        /// <summary>
        /// 可以覆盖此KEY的方式
        /// </summary>
        public  virtual string  publicApikey { get; set; }
        private  string APIKey {
            get
            {
                if (string.IsNullOrEmpty((publicApikey)))
                {
                    return GlobKey.globKey;
                }
                else
                {
                    return publicApikey;

                }
            }
        }

        [JsonIgnore]
        public List<Byte[]> attachments { get; set; }

       

        /// <summary>
        /// 获取签名
        /// </summary>
        public override string sign
        {
            get
            {
                var orgin = APIKey + this.time.ToString() + JsonConvert.SerializeObject(this.@object);
                LogService.Default.Debug(orgin);
                var orginSign = EntitySign.To32Md5(orgin);
                LogService.Default.Debug(orgin+";MD5值："+orginSign);
                return orginSign;
            }
        }

        /// <summary>
        /// 用户ID 登入人ID
        /// </summary>
        [JsonProperty]
        public Int32 UserID
        {
            get;
            set;
        }

       
    }
}