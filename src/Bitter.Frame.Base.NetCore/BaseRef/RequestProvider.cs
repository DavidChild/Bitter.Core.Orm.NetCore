using Bitter.Tools;
using Bitter.Tools.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using RestSharp;
using Polly;
using Bitter.Base.NetCore.ConfigManage;
using Bitter.Tools.NetCore.NLog;
using System.Linq;

namespace Bitter.Base
{
    /********************************************************************************
    ** auth： davidchild
    ** date： 2017/3/2 13:59:55
    ** desc：
    ** Ver.:  V1.0.0
    ** Copyright (C) 2016 Bitter 版权所有。
    *********************************************************************************/



    public class RequestProvider
    {
        ///// <summary>
        ///// 请求对接，返回通用Result对象Post/Get请求
        ///// </summary>
        ///// <param name="e">传送对象</param>
        ///// <param name="ApiUri">请求路径</param>
        ///// <param name="time">超时时间</param>
        ///// <param name="version">版本号</param>
        ///// <returns>返回最终的Result</returns>
        //public static Result Request(BaseResponseEntity e, string ApiUri, Int32 time = 7000, string version = "1.0")
        //{
        //    Result re = new Result();
        //    try
        //    {

        //        var sentJson = JsonConvert.SerializeObject(e);

        //        LogService.Default.Trace("发送跟踪日志：" + sentJson + "，目标地址：" + ApiUri);//  + ",线程信息：" + BaseThread.GetTraceInfo().ToJsonString());//日志记录
        //        var rjson = ApiUtil.PostHttpRequest(ApiUri, sentJson, time, version);
        //        LogService.Default.Trace("返回跟踪日志：" + rjson);//日志记录
        //        re.@object = rjson;
        //        re.code = 1;
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex is WebException)
        //        {
        //            ParamterCheckExtensions.CheckRequestException(ex).Throw();
        //        }
        //        LogService.Default.Fatal("请求异常，错误"+ex.Message+" 请求地址："+ApiUri,ex);
        //        re.code = 0;
        //        re.message = ex.Message;


        //    }
        //    return re;
        //}
        ///// <summary>
        ///// 请求对接，返回通用Result对象Post/Get请求
        ///// </summary>
        ///// <param name="jsonResult">传送jsonResult</param>
        ///// <param name="ApiUri">请求路径</param>
        ///// <returns>返回最终的Result</returns>
        //public static Result Request(string jsonResult, string ApiUri, Int32 time = 7000, string version = "1.0")
        //{
        //    Result re = new Result();

        //    try
        //    {
        //        var sentJson = jsonResult;
        //        LogService.Default.Trace("发送跟踪日志：" + sentJson + "，目标地址：" + ApiUri);//  + ",线程信息：" + BaseThread.GetTraceInfo().ToJsonString());//日志记录
        //        var rjson = ApiUtil.PostHttpRequest(ApiUri, sentJson, time, version);
        //        LogService.Default.Trace("返回跟踪日志：" + rjson);//日志记录
        //        re.@object = rjson;
        //        re.code = 1;
        //    }
        //    catch (Exception ex)
        //    {
        //        re.code = 0;
        //        re.message = ex.Message;
        //        LogService.Default.Fatal(ex);
        //    }
        //    return re;
        //}


        ///// <summary>
        ///// 请求对接，返回通用Result对象Post/Get请求
        ///// </summary>
        ///// <param name="jsonResult">传送jsonResult</param>
        ///// <param name="ApiUri">请求路径</param>
        ///// <returns>返回最终的Result</returns>
        //public static Result Request(string jsonResult, string ApiUri, Method method, Int32 time = 7000, string version = "1.0")
        //{
        //    Result re = new Result();

        //    try
        //    {
        //        var sentJson = jsonResult;
        //        LogService.Default.Trace("发送跟踪日志：" + sentJson + "，目标地址：" + ApiUri);// + ",线程信息：" + BaseThread.GetTraceInfo().ToJsonString());//日志记录
        //        var rjson = string.Empty;
        //        if (Method.POST == method)
        //        {
        //            rjson = ApiUtil.PostHttpRequest(ApiUri, sentJson, time, version);
        //        }
        //        else
        //        {
        //            rjson = ApiUtil.GetHttpRequest(ApiUri, sentJson, time, version);
        //        }
        //        LogService.Default.Trace("返回跟踪日志：" + rjson);//日志记录



        //        re.@object = rjson;
        //        re.code = 1;
        //    }
        //    catch (Exception ex)
        //    {
        //        re.code = 0;
        //        re.message = ex.Message;
        //        LogService.Default.Fatal(ex);
        //    }
        //    return re;
        //}

        ///// <summary>
        ///// 复杂类型的POST
        ///// </summary>
        ///// <param name="ApiUri">请求资源路径</param>
        ///// <param name="parameters">传送参数集合</param>
        ///// <param name="medieaParameters">传送媒体集合</param>
        ///// <param name="jsonBody">post存放的Json对象</param>
        ///// <param name="time">超时时间</param>
        ///// <param name="version">版本号</param>
        ///// <returns>返回最终的Result</returns>
        //public static Result PostComplexData(string ApiUri,  RestRequest request, BaseResponseEntity jsonBody = null, Int32 time = 7000, string version = "1.0")
        //{
        //    Result re = new Result();
        //    string sentJson = string.Empty;

        //    try
        //    {
        //        if (jsonBody != null)
        //        {
        //            sentJson = JsonConvert.SerializeObject(jsonBody);
        //        }
        //        dynamic dy = new
        //        {

        //            JsonBody = sentJson,
        //            RestSharpParameter = request.Parameters, //parameters,



        //        };
        //        var dyJson = JsonConvert.SerializeObject(dy);
        //        LogService.Default.Trace("发送跟踪日志：" + sentJson + "，目标地址：" + ApiUri);//日志记录
        //        IRestResponse rjson = ApiUtil.PostComplexData(ApiUri, request, sentJson, time, version).Result;




        //        LogService.Default.Trace("返回跟踪日志：" + JsonConvert.SerializeObject("RestSharp_HTTP_StatusCode:" + rjson.StatusCode + "RestSharp_HTTP_Content:" + rjson.Content));//日志记录
        //        re.errorCode = ((int)rjson.StatusCode).ToSafeString();
        //        if ((int)rjson.StatusCode == 200)
        //        {
        //            re.@object = rjson.Content;
        //            re.code = 1;
        //        }
        //        else
        //        {
        //            re.@object = rjson.Content;
        //            re.message = rjson.ErrorMessage ?? "";
        //            re.code = 0;
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        re.code = 0;
        //        re.message = ex.Message;
        //        LogService.Default.Fatal(ex);
        //    }
        //    return re;
        //}
        ///// <summary>
        ///// 复杂类型的POST
        ///// </summary>
        ///// <param name="ApiUri">请求POST数据的地址</param>
        ///// <param name="parameters">参数</param>
        ///// <param name="medieaURI">媒体资源路径</param>
        ///// <param name="medieaName">媒体名称</param>
        ///// <param name="jsonBody">postBody存放的内容</param>
        ///// <param name="time">超时时间</param>
        ///// <param name="version">版本号</param>
        ///// <returns>返回最终的Result</returns>
        //public static Result PostComplexData(string ApiUri, RestRequest request, string medieaURI = "", string medieaName = "", BaseResponseEntity jsonBody = null, Int32 time = 7000, string version = "1.0")
        //{
        //    Result re = new Result();
        //    string sentJson = string.Empty;

        //    try
        //    {
        //        if (jsonBody != null)
        //        {
        //            sentJson = JsonConvert.SerializeObject(jsonBody);
        //        }
        //        dynamic dy = new
        //        {

        //            JsonBody = sentJson,
        //            medieaURI = medieaURI,
        //            medieaName = medieaName,
        //            RestSharpParameter = request.Parameters

        //        };
        //        var dyJson = JsonConvert.SerializeObject(dy);
        //        LogService.Default.Trace("发送跟踪日志：" + sentJson + "，目标地址：" + ApiUri);//日志记录
        //        IRestResponse rjson = ApiUtil.PostComplexData(ApiUri, request,  medieaURI, medieaName, sentJson, time, version).Result;
        //        LogService.Default.Trace("返回跟踪日志：" + JsonConvert.SerializeObject("RestSharp_HTTP_StatusCode:" + rjson.StatusCode + "RestSharp_HTTP_Content:" + rjson.Content));//日志记录
        //        re.errorCode = ((int)rjson.StatusCode).ToSafeString();
        //        if ((int)rjson.StatusCode == 200)
        //        {
        //            re.@object = rjson.Content;
        //            re.code = 1;
        //        }
        //        else
        //        {
        //            re.@object = rjson.Content;
        //            re.message = rjson.ErrorMessage ?? "";
        //            re.code = 0;

        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        re.code = 0;
        //        re.message = ex.Message;
        //        LogService.Default.Fatal(ex);
        //    }
        //    return re;
        //}


        private static void SetIsWriteLog(string uri,out bool writerequestlog, out bool writeresponelog)
        {
             writerequestlog = true; //默认为写入
            writeresponelog = true; 

            try
            {


                var logswtichlist = JsonConfigMange.GetInstance().Settings<List<LogSwitchDto>>("Logswitch");

                if (logswtichlist != null && logswtichlist.Count > 0)
                {
                    var httplogswtichs = logswtichlist.Where(o => o.LogType == enum_logswitchtype.Http请求日志).ToList();
                    if (httplogswtichs != null && httplogswtichs.Count > 0)
                    {
                        var restClient = new RestClient { BaseUrl = new Uri(uri) };
                        foreach (var item in httplogswtichs)
                        {
                            if (restClient.BaseUrl.AbsolutePath.ToLower() == item.MatchRule.ToLower())
                            {
                                writerequestlog = false;
                                writeresponelog = false;
                                break;
                            }

                        }
                    }

                }


                string logfileter = Bitter.Base.NetCore.ConfigManage.JsonConfigMange.GetInstance().AppSettings["logfilter"].ToSafeString("");

                if (!string.IsNullOrEmpty(logfileter))
                {

                    logswtichlist = JsonConvert.DeserializeObject<List<LogSwitchDto>>(logfileter);
                    foreach (var item in logswtichlist)
                    {
                        if (uri.ToLower().Contains(item.MatchRule.ToLower()))
                        {

                            if ( item.request)
                            {
                                writerequestlog = false;
                               
                            }
                            if (item.repones)
                            {
                                writeresponelog = false;
                               
                            }
                            break;
                        }
                       
                    }
                }

            }
            catch (Exception ex)
            {
                LogService.Default.Fatal("find the settig log filter setting error:"+ex.Message);
            }
           
        }

        public static Result SyncRequest(RestBag restBag)
        {
            Result re = new Result();
            string sentJson = string.Empty;
              bool writerequestlog;
            bool writeresponelog;
            SetIsWriteLog(restBag.uri, out writerequestlog,out writeresponelog);
            try
            {

                dynamic dy = new
                {

                    JsonBody = restBag.req.JsonSerializer.ToString(),
                    RestSharpParameter = restBag.req.Parameters

                };
                var dyJson = JsonConvert.SerializeObject(dy);
               
                if (writerequestlog)
                    LogService.Default.Trace("发送跟踪日志：" + dyJson + "，目标地址：" + restBag.uri);//日志记录
                                                                                          // 定义超时重试机制
                                                                                          //var policy = Policy.HandleResult<IRestResponse>(r =>
                                                                                          //{
                                                                                          //    return r.StatusCode == HttpStatusCode.RequestTimeout || r.StatusCode == HttpStatusCode.GatewayTimeout || r.StatusCode == 0;
                                                                                          //}).Or<TimeoutException>().Retry(3);

                //IRestResponse rjson = policy.Execute(() =>
                //{
                //    return ApiUtil.RequestData(restBag).Result;
                //});

                IRestResponse rjson = ApiUtil.RequestData(restBag).Result;
                if (writeresponelog)
                {
                    LogService.Default.Trace("返回跟踪日志：" + JsonConvert.SerializeObject("RestSharp_HTTP_StatusCode:" + rjson.StatusCode + "RestSharp_HTTP_Content:" + rjson.Content + "目标地址：" + restBag.uri));//日志记录););//日志记录
                }
                re.errorCode = ((int)rjson.StatusCode).ToSafeString();
                if ((int)rjson.StatusCode == 200)
                {
                    re.@object = rjson.Content;
                    re.code = 1;
                }
                else
                {
                    re.@object = rjson.Content;
                    re.message = rjson.ErrorMessage ?? "";
                    if (string.IsNullOrWhiteSpace(re.message))
                        re.message = rjson.Content;
                    re.code = 0;
                    LogService.Default.Trace("返回跟踪错误响应日志：result: message" + re.message + "目标地址：" + restBag.uri);//日志记录);
                }


            }
            catch (Exception ex)
            {
                re.code = 0;
                re.message = ex.Message;
                LogService.Default.Fatal(ex);
            }
            return re;
        }

        public static Result AnsyRequest(RestBag restBag)
        {
            Result re = new Result();
            string sentJson = string.Empty;

            try
            {

                dynamic dy = new
                {

                    JsonBody = restBag.req.JsonSerializer.ToString(),
                    RestSharpParameter = restBag.req.Parameters

                };
                var dyJson = JsonConvert.SerializeObject(dy);
                bool writerequestlog;
                bool writeresponelog;
                SetIsWriteLog(restBag.uri, out writerequestlog, out writeresponelog);

                if (writerequestlog)
                {
                    LogService.Default.Trace("发送跟踪日志：" + dyJson + "，目标地址：" + restBag.uri);//日志记录
                }
                
                ApiUtil.RequestData(restBag);
            }
            catch (Exception ex)
            {
                re.code = 0;
                re.message = ex.Message;
                LogService.Default.Fatal(ex);
            }
            return re;
        }


        #region 下载文件
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="requestData">请求数据信息</param>
        /// <returns>返回最终的Result</returns>
        public static Result FileDownLoad(string jsonResult, string ApiUri)
        {
            Result re = new Result();
            try
            {
                var sentJson = jsonResult;
                LogService.Default.Trace("发送跟踪日志：" + sentJson + "，目标地址：" + ApiUri);//日志记录
                Stream str = ApiUtil.FileDownLoad(ApiUri, sentJson);
                LogService.Default.Trace("返回跟踪日志：" + str.ToSafeString());//日志记录
                re.@object = str;
                re.code = 1;
            }
            catch (Exception ex)
            {
                re.code = 0;
                re.message = ex.Message;
            }
            return re;
        }

        #endregion 下载文件
    }
}