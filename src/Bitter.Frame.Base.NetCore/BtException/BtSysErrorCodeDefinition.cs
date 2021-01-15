using System;
using System.Collections.Generic;
using System.Text;

namespace Bitter.Base
{
    public static class SysErrorCodeDefinition
    {

        public static readonly string AnyRadixConvert_CharacterIsNotValid = "参数中的字符{0}不是 {1} 进制数的有效字符。★1001";
        public static readonly string AnyRadixConvert_Overflow = "0★1002";
        public static readonly string CacheEntryNotFound = "未找到指定的缓存项。★10001";
        public static readonly string ConnectFailure = "未能在传输级联系到远程服务点。★10002";
        public static readonly string ConnectionClosed = "连接被过早关闭。★10003";//  10003
        public static readonly string Filter_GroupOperateError = "查询条件组中的操作类型错误，只能为And或者Or。★1003";//  1003
        public static readonly string Filter_RuleFieldInTypeNotFound = "指定的属性{0}在类型{1}中不存在。★1004";//  1004
        public static readonly string KeepAliveFailure = "指定Keep-alive 标头的请求连接被意外关闭。★10004";//  10004
        public static readonly string Logging_CreateLogInstanceReturnNull = "创建名称为{0}的日志实例时{1}返回空实例。★1005";//  1005
        public static readonly string Mef_HttpContextItems_NotFoundRequestContainer = "当前Http上下文中不存在Request有效范围的Mef部件容器。★1006";//  1006
        public static readonly string MessageLengthLimitExceeded = "当发送请求或从服务器接收响应时，会接收到超出指定限制的消息。★10005";//  10005
        public static readonly string NameResolutionFailure = "名称解析服务未能解析主机名。★10006";//  10006
        public static readonly string ObjectExtensions_PropertyNameNotExistsInType = "指定对象中不存在名称为{0}的属性。★1007";//  1007
        public static readonly string ObjectExtensions_PropertyNameNotFixedType = "指定名称{0}的属性类型不是{1}。★1008";//  1008
        public static readonly string ParameterCheck_Between = "参数{0}的值必须在{1}与{2}之间。★1009";//  1009
        public static readonly string ParameterCheck_BetweenNotEqual = "参数{0}的值必须在{1}与{2}之间，且不能等于{3}参数{0}的值必须在{1}与{2}之间，且不能等于{3}。★1010";//  1010
        public static readonly string ParameterCheck_DirectoryNotExists = "指定的目录路径{0}不存在。★1011";//  1011
        public static readonly string ParameterCheck_FileNotExists = "指定的文件路径{0}不存在。★1012";// 1012
        public static readonly string ParameterCheck_NotEmpty_Guid = "参数{0}的值不能为Guid.Empty★1013";//  1013
        public static readonly string ParameterCheck_NotGreaterThan = "参数{0}的值必须大于{1}。★1014";//  1014
        public static readonly string ParameterCheck_NotGreaterThanOrEqual = "参数{0}的值必须大于或等于{1}。★1015";//  1015
        public static readonly string ParameterCheck_NotLessThan = "参数{0}的值必须小于{1}。★1016";//  1016
        public static readonly string ParameterCheck_NotLessThanOrEqual = "参数{0}的值必须小于或等于{1}。★1017";//  1017
        public static readonly string ParameterCheck_NotNull = "参数{0}不能为空引用。★1018";//  1018
        public static readonly string ParameterCheck_NotNullAndNotIsZero = "参数{0}不能为空引用或者值必须大于零★1023";//  1023
        public static readonly string ParameterCheck_NotNullOrEmpty_Collection = "参数{0}不能为空引用或空集合。★1019";//  1019
        public static readonly string ParameterCheck_NotNullOrEmpty_String = "参数{0}不能为空引用或空字符串。★1020";//  1020
        public static readonly string ParameterCheck_PhpneNumber = "参数{0}不是有效的电话号码。★1025";//  1025
        public static readonly string ParameterCheck_Regular = "参数{0}未通过正则表达式{1}的验证。★1026";//  1026
        public static readonly string ParameterCheck_StringLength = "参数{0}长度必须在{1}与{2}之间。★1024";//  1024
        public static readonly string Pending = "内部异步请求挂起。★10007";//  10007
        public static readonly string PipelineFailure = "该请求是管线请求，并且连接未接收到响应即被关闭。★10008";//  10008
        public static readonly string ProtocolError = "从服务器接收到的响应完成了，但它指示了一个协议级错误。例如，HTTP协议错误（如401访 问被拒绝）使用此状态。★10009";//  10009
        public static readonly string ProxyNameResolutionFailure = "名称解析服务未能解析代理主机名。★10010";//  10010
        public static readonly string ReceiveFailure = "没有从远程服务器接收到完整响应。★10011";//  10011
        public static readonly string RequestCanceled = "请求被取消，WebRequest.Abort方法被调用，或者发生了不可分类的错误。这是Status的默认值。★10012";//  10012
        public static readonly string RequestProhibitedByCachePolicy = "缓存策略不允许该请求。一般而言，当请求不可缓存或有效策略禁止向服务器发送请求时会发生这种情况。如果请求方法暗示请求正文存在，请求方法需要与服务器直接交互，或者请求包含条件标头，则您可能会收到此状态。★10013";//  10013
        public static readonly string RequestProhibitedByProxy = "代理不允许此请求。★10014";//   10014
        public static readonly string SecureChannelFailure = "使用SSL建立连接时发生错误。★10015";//   10015
        public static readonly string Security_DES_KeyLenght = "参数key的长度必须为8或24，当前为{0}。★1021";//   1021
        public static readonly string Security_RSA_Sign_HashType = "参数hashType必须为MD5或SHA1★1022";//   1022
        public static readonly string SendFailure = "未能将完整请求发送到远程服务器。★10016";//   10016
        public static readonly string ServerProtocolViolation = "此服务器响应不是有效的HTTP响应。Success未遇到任何错误。★10017";//   10017
        public static readonly string Timeout = "在请求的超时期限内未收到任何响应。★10018";//   10018
        public static readonly string TrustFailure = "未能验证服务器证书。★10019";//   10019
        public static readonly string UnknownError = "发生未知类型的异常。★10020";//   10020


    }
}
