using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Bitter.Base
{
    public class  Result<T>
    {

        [JsonIgnore]
        public List<Byte[]> attachments { get; set; }

        [JsonProperty]
        public long[] attachmentLengthArry
        {
            get
            {
                if (attachments == null || attachments.Count == 0)
                {
                    return null;
                }
                else
                {
                    List<long> arry = new List<long>();
                    attachments.ForEach(p =>
                    {
                        arry.Add(p.Length);
                    });
                    return arry.ToArray();
                }
            }

        }
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
        /// 返回成功或失败的显示值
        /// </summary>
        [JsonProperty]
        public string message { get; set; }
        /// <summary>
        /// 拆箱后的对象
        /// </summary>
        [JsonProperty]
        public T @object { get; set; }
        /// <summary>
        /// page对象
        /// </summary>
        public page @page { get; set; }
        /// <summary>
        /// 开放平台所使用的openid
        /// </summary>
        [JsonProperty]
        public string openId { get; set; }
    }
}
