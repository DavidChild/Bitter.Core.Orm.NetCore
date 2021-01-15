using Newtonsoft.Json;
using System.Text;

namespace Bitter.Base
{
    /********************************************************************************
    ** auth： davidchild
    ** date： 2017/3/2 12:59:27
    ** desc：
    ** Ver.:  V1.0.0
    ** Copyright (C) 2016 Bitter 版权所有。
    *********************************************************************************/

    [JsonObject(MemberSerialization.OptIn)]
    public class BaseRequestEntity : IEntity
    {
        protected RefEmun _refEmun;
        private string _secretKey;

        /// <summary>
        /// 初始化BaseEntity
        /// </summary>
        /// <param name="refEnum"></param>
        public BaseRequestEntity(RefEmun refEnum)
        {
            _refEmun = refEnum;
        }

        public BaseRequestEntity()
        {
        }

        public virtual string checkedSign
        {
            get
            {
                var orignSign = new StringBuilder();
                //orignSign = EntitySign.GetEntitySign(this, orignSign);
                var orignSignInfo = _secretKey + orignSign.ToString();
                var md5Sign = EntitySign.To32Md5(orignSignInfo);
                return "签.名.忽.略";
            }
        }

        [JsonProperty]
        public string sign { get; set; }

        [JsonProperty]
        public virtual long? time { get; set; }

        /// <summary>
        /// 获取密钥
        /// </summary>
        private string SecretKey
        {
            get
            {
                RefSecretKey.RefSecretKeyDic.TryGetValue(_refEmun, out _secretKey);
                return _secretKey;
            }
        }
    }
}