using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bitter.Base
{
   public class MessageResultRequest: BaseRequestEntity
    {
        [JsonProperty]
        public object @object { get; set; }

        /// <summary>
        /// 业务处理
        /// </summary>
        [JsonProperty]
        public int dealType { get; set; }


        [JsonProperty]
        public string  requestToken { get; set; }
        /// <summary>
        /// 可以覆盖此KEY的方式
        /// </summary>
        public virtual string publicApikey { get; set; }
        private string APIKey
        {
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


        /// <summary>
        /// 开放平台所使用的分配给客户的OPENID
        /// </summary>
        [JsonProperty]
        public string openId
        {

            get;

            set;
        }
        /// <summary>
        /// 获取签名
        /// </summary>
        public override string checkedSign
        {
            get
            {
                var orgin = this.time.ToString()+"_"+ EntitySign.To32Md5(APIKey)+"_"+   APIKey +"_"  + JsonConvert.SerializeObject(@object);
                return EntitySign.To32Md5(orgin);
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

        [JsonIgnore]
        public List<Byte[]> attachments { get; set; }


        /// <summary>
        /// 签名验证
        /// </summary>
        /// <returns></returns>
        public Result CheckedSign()
        {
            Result r = new Result();
            if (sign == GlobKey.globSkipKey)
            {
                r.code = 1;
                return r;
            }
            if (this.sign == checkedSign)
            {
                r.code = 1;
                return r;
            }
            else
            {
                r.code = 0;
                r.message = "签名验证失败！";
            }
            return r;
        }
    }
}
