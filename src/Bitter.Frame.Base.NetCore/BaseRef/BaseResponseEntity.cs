using Bitter.Tools.Utils;
using System;
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

    public class BaseResponseEntity : IEntity
    {
        public BaseResponseEntity()
        { }
        private RefEmun _refEmun;
        private string _secretKey;

        /// <summary>
        /// 初始化BaseEntity
        /// </summary>
        /// <param name="refEnum"></param>
        public BaseResponseEntity(RefEmun refEnum)
        {
            _refEmun = refEnum;
        }

        public virtual string sign
        {
            get
            {
                var orignSign = new StringBuilder();
                //orignSign = EntitySign. GetEntitySign(this, orignSign);
                var orignSignInfo = _secretKey + orignSign.ToString();
                var md5Sign = EntitySign.To32Md5(orignSignInfo);
                return "跳过签名";
            }
        }

        private long? _time { get; set; }

        public virtual long? time
        {
            get
            {
                if (_time.HasValue)
                {
                    return _time.Value;
                }
                else
                {
                  return _time= DateTime.Now.ToString().ToSafeDateTime().ToSafeDataLong();
                }
             
            }
        }

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