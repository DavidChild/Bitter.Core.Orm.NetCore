using System.Collections.Generic;

namespace Bitter.Base
{
    /********************************************************************************
    ** auth： davidchild
    ** date： 2017/3/2 10:04:59
    ** desc：
    ** Ver.:  V1.0.0
    ** Copyright (C) 2016 Bitter 版权所有。
    *********************************************************************************/

    public enum RefEmun
    {
       
        Wallet = 1,
      

    }

    public enum RequestType
    {
        Get = 1,
        Post = 2
    }

    public class GlobKey
    {
        /// <summary>
        /// 签名Key
        /// </summary>
        public static readonly string globKey = "8859F446-8463-4C80-A17E-B8E9B92C9657";

        /// <summary>
        /// 跳过签名
        /// </summary>
        public static readonly string globSkipKey = "4BDE017B-4284-4BB7-922A-BE00C554E7A6";
    }

    /// <summary>
    /// 配置业务密钥
    /// </summary>
    public class RefSecretKey
    {
        public static Dictionary<RefEmun, string> RefSecretKeyDic = new Dictionary<RefEmun, string>();

        public RefSecretKey()
        {
            RefSecretKeyDic.Add(RefEmun.Wallet, "4F161626-F281-487A-A168-CA689A5C0B36"); 
         
        }
    }
}