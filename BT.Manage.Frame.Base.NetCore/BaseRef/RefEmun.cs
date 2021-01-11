using System.Collections.Generic;

namespace BT.Manage.Frame.Base
{
    /********************************************************************************
    ** auth： Jason
    ** date： 2017/3/2 10:04:59
    ** desc：
    ** Ver.:  V1.0.0
    ** Copyright (C) 2016 备胎 版权所有。
    *********************************************************************************/

    public enum RefEmun
    {
        /// <summary>
        /// 钱包一起项目
        /// </summary>
        Wallet = 1,
        /// <summary>
        /// 二部主服务地址
        /// </summary>
        SecondDept = 2,
        /// <summary>
        /// 钱包二期项目
        /// </summary>
        BTCWallet = 3,
        /// <summary>
        /// JAVA:一部主服务地址
        /// </summary>
        FistDept = 4,
        /// <summary>
        /// 很好贷系统服务地址
        /// </summary>
        HCRM = 5,
        /// <summary>
        /// 三部服务主地址
        /// </summary>
        ManageAPI = 6,
        /// <summary>
        /// 中怡康服务地址
        /// </summary>
        ZYKAPI = 7,
        /// <summary>
        /// 备胎大数据服务地址
        /// </summary>
        DSJAPI = 8,
        /// <summary>
        /// 信用端登入授权服务地址
        /// </summary>
        XYDAPI = 9,
        /// <summary>
        /// 第三方验证服务地址
        /// </summary>
        FACEAPI = 10,
        /// <summary>
        /// 消费贷业务服务
        /// </summary>
        XLoan = 11,
        /// <summary>
        /// 铜板街
        /// </summary>
        TBJ = 12,
        /// <summary>
        /// 核算服务地址
        /// </summary>
        HSServer = 13,
        /// <summary>
        /// 面签系统服务
        /// </summary>
        MQServer = 14,
        /// <summary>
        /// 途强服务
        /// </summary>
        TQServer = 15,
        /// <summary>
        /// 浦发银行
        /// </summary>
        SPDSerer = 16,
        /// <summary>
        /// 车300服务
        /// </summary>
        Car300Server = 17,
        /// <summary>
        /// 视频面签组件
        /// </summary>
        IntelWebRtc = 18,
        /// <summary>
        /// 山东工行
        /// </summary>
        SDICBCService = 19,

        /// <summary>
        /// 内部文件服务
        /// </summary>
        InternalAttachmentService = 20,

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
        public static readonly string globKey = "KYSYBTJF7852585KJHHGGJUHHIBBVYSY";

        /// <summary>
        /// 跳过签名
        /// </summary>
        public static readonly string globSkipKey = "HHYLKHLKIJUJT45852354YLK585PODXB";
    }

    /// <summary>
    /// 配置业务密钥
    /// </summary>
    public class RefSecretKey
    {
        public static Dictionary<RefEmun, string> RefSecretKeyDic = new Dictionary<RefEmun, string>();

        public RefSecretKey()
        {
            RefSecretKeyDic.Add(RefEmun.Wallet, "123456");
            RefSecretKeyDic.Add(RefEmun.SecondDept, "btjf123456");
            RefSecretKeyDic.Add(RefEmun.FistDept, "123456");
        }
    }
}