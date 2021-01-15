using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bitter.Base
{
    public class MessageRequest
    {

        private string _secretKey;

        public virtual string checkedSign
        {
            get
            {
                var orignSign = new StringBuilder();
                //orignSign = EntitySign.GetEntitySign(this, orignSign);
                var orignSignInfo = "" + orignSign.ToString();
                var md5Sign = EntitySign.To32Md5(orignSignInfo);
                return "跳过签名";
            }
        }

        [JsonProperty]
        public string sign { get; set; }

        [JsonProperty]
        public virtual long? time { get; set; }


        /// <summary>
        /// 业务包装对象
        /// </summary>
        [JsonProperty]
        public object @object { get; set; }


        /// <summary>
        /// 业务处理
        /// </summary>
        [JsonProperty]
        public int dealType { get; set; }

    
        [JsonProperty]
        public string requestToken { get; set; }

        /// <summary>
        /// 获取密钥
        /// </summary>
        private string SecretKey
        {
            get
            {
                _secretKey = "YYYYXXYYYXYT779868778X";
                return _secretKey;
            }
        }
    }
}
