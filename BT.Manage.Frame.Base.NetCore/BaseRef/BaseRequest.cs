using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using BT.Manage.Tools;
using BT.Manage.Tools.Utils;
using Newtonsoft.Json;
using RestSharp;
using System.Configuration;
using BT.Manage.Base;
using BT.Manage.Frame.Base.NetCore.ConfigManage;

namespace BT.Manage.Frame.Base
{
    /********************************************************************************
    ** auth： Jason
    ** date： 2017/3/2 10:40:57
    ** desc：
    ** Ver.:  V1.0.0
    ** Copyright (C) 2016 备胎 版权所有。
    *********************************************************************************/

    public class BaseRequest
    {
        private string _uri { get; set; }

        /// <summary>
        /// 整个系统定义的交互服务枚举
        /// </summary>
        public RefEmun _refEnum;
       
        private string _secretKey;

       
        /// <summary>
        /// 实例化请求服务
        /// </summary>
        /// <param name="refEnum"></param>
        public BaseRequest(RefEmun refEnum)
        {
            this.refEnum = refEnum;
        }
        /// <summary>
        /// 实例化请求服务
        /// </summary>
        /// <param name="refEnum"></param>
        public BaseRequest()
        {
            
        }
        
        
        public RefEmun refEnum
        {
            set { _refEnum = value; }
        }

      

        /// <summary>
        /// 获取密钥
        /// </summary>
        public string Secretkey
        {
            get
            {
                RefSecretKey.RefSecretKeyDic.TryGetValue(_refEnum, out _secretKey);
                return _secretKey;
            }
        }

        private string RouteName { get; set; }



        /// <summary>
        /// 获取请求地址
        /// </summary>
        /// <param name="uriName"></param>
        /// <returns></returns>
        public string ApiUr
        {
            get
            {

                string apiUrl = JsonConfigMange.GetInstance().AppSettings[_refEnum.ToString()].ToSafeString("");
                if (!string.IsNullOrEmpty(_uri))
                {
                    apiUrl += _uri;//System.Configuration.ConfigurationSettings.AppSettings[_refEnum.ToString()] + RouteName;
                }
                 LogService.Default.Debug("这是请求的地址："+apiUrl);
                return apiUrl;
            }
        }

        /// <summary>
        /// 获取回调请求地址
        /// </summary>
        /// <param name="uriName"></param>
        /// <returns>返回notifyURI</returns>
        public string CallBackApiUri(string uriName)
        {
            var CallBackName = _refEnum.ToString() + "CallBack";
            return "";//System.Configuration.ConfigurationSettings.AppSettings[CallBackName] + uriName;
        }

        /// <summary>
        /// 请求下载的数据
        /// </summary>
        /// <returns>返回最终的Result</returns>
        public Result FileDownLoad(string jsonResult, string uriPath)
        {
            RouteName = uriPath;
            return RequestProvider.FileDownLoad(jsonResult, this.ApiUr);
        }


        /// <summary>
        /// 请求服务数据POST/GET
        /// </summary>
        /// <param name="e">传送的参数对象</param>
        /// <param name="uriPath">请求资源地址</param>
        /// <param name="time">超时时间</param>
        /// <param name="version">版本</param>
        /// <returns>返回最终的Result</returns>
        private Result SyncRequest(RestRequest rest, ExtendParams restExtendParams,string fullpath)
        {
            try
            {
                
                RestBag rrest = new RestBag();
                rrest.extendParms = restExtendParams;
                rrest.req = rest;
                rrest.uri = fullpath;
                Result r = RequestProvider.SyncRequest(rrest);
                return r;
            }
            finally
            {
                
            }
            
        }
        private Result AnsyRequest(RestRequest rest, ExtendParams restExtendParams, string fullpath)
        {
            try
            {

                RestBag rrest = new RestBag();
                rrest.extendParms = restExtendParams;
                rrest.req = rest;
                rrest.uri = fullpath;
                Result r = RequestProvider.AnsyRequest(rrest);
                return r;
            }
            finally
            {

            }

        }



        public Result SyncRequestByResourcePath(RestRequest rest, ExtendParams restExtendParams, string resourcePath)
        {
            try
            {

                this._uri = resourcePath;
                Result r = SyncRequest(rest, restExtendParams,ApiUr);
                return r;
            }
            finally
            {

            }

        }
        public Result AsyncRequestByResourcePath(RestRequest rest, ExtendParams restExtendParams, string resourcePath)
        {
            try
            {

                this._uri = resourcePath;
                Result r = AnsyRequest(rest, restExtendParams, ApiUr);
                return r;
            }
            finally
            {

            }

        }

        public Result SyncRequestByResourceFullPath(RestRequest rest, ExtendParams restExtendParams, string fullpath)
        {
            try
            {
                 Result r = SyncRequest(rest, restExtendParams, fullpath);
                return r;
            }
            finally
            {

            }

        }
        public Result AsyncRequestByResourceFullPath(RestRequest rest, ExtendParams restExtendParams, string fullpath)
        {
            try
            {
                Result r = AnsyRequest(rest, restExtendParams, fullpath);
                return r;
            }
            finally
            {

            }

        }





        public Result<T> SyncRequestByResourcePath<T>(RestRequest rest, ExtendParams restExtendParams, string resourcePath)
        {
            Result<T> t = new Result<T>();
            try
            {

                this._uri = resourcePath;
                Result r = SyncRequest(rest, restExtendParams, ApiUr);
                t.code = r.code;
                t.message = r.message;
                t.errorCode = r.errorCode;
                if (r.code == 0) return t;
                if (r.code == 1)
                {

                    try
                    {

                        string resultInfo = TryCast.CastTo<string>(r.@object);
                        t = JsonConvert.DeserializeObject<Result<T>>(resultInfo);

                    }
                    catch (Exception ex)
                    {
                        LogService.Default.Fatal("转化TResult 中的T对象出错：对象【" + typeof(T).FullName + "】,转化对象：JSON:" +
                                               JsonConvert.SerializeObject(t.@object));
                        return t;
                    }

                }
            }
            catch (Exception ex)
            {
                t.code = 0;
                t.message = ex.Message;
                return t;
            }
            finally
            {

            }
            return t;

        }
     

        public Result<T> SyncRequestByResourceFullPath<T>(RestRequest rest, ExtendParams restExtendParams, string fullpath)
        {
            Result<T> t = new Result<T>();
            try
            {
                
                Result r = SyncRequest(rest, restExtendParams, fullpath);
                t.code = r.code;
                t.message = r.message;
                t.errorCode = r.errorCode;
                if (r.code == 0) return t;
                if (r.code == 1)
                {
                    try
                    {

                        string resultInfo = TryCast.CastTo<string>(r.@object);
                        t = JsonConvert.DeserializeObject<Result<T>>(resultInfo);

                    }
                    catch (Exception ex)
                    {
                        LogService.Default.Fatal("转化TResult 中的T对象出错：对象【" + typeof(T).FullName + "】,转化对象：JSON:" +
                                               JsonConvert.SerializeObject(t.@object));
                        return t;
                    }
                }


            }
            catch (Exception ex)
            {
                t.code = 0;
                t.message = ex.Message;
                return t;
            }
            finally
            {

            }
            return t;
        }
    


       
          
        
      
       
       
    }
}