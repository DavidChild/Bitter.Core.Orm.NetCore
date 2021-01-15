using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Bitter.Tools;
using Bitter.Tools.Helper;
using RestSharp;
using Bitter.Tools.Utils;
namespace Bitter.Base
{
    internal class ApiUtil
    {


        /// <summary>
        ///  
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        public static Task<IRestResponse> RequestData(RestBag bag)
        {
            //处理timeout
            
            if (bag.extendParms == null || bag.extendParms.timeOut == 20000)
            {
                bag.req.Timeout = 7000;
            }
            else
            {
                bag.req.Timeout = bag.extendParms.timeOut;
                 
            }
            string str = "";
            //处理version
            if (bag.extendParms == null)
            {
               str="1.0.0.0"; 
            }
            else
            {
                str= bag.extendParms.version.ToSafeString("1.0.0.0");

            }
          
            //bag.req.AddHeader("Content-Type", "application/json; charset=utf-8");
            bag.req.AddHeader("s-version", str);
            
            try
            {
                bag.req.AddHeader("BTProcessInfo", Newtonsoft.Json.JsonConvert.SerializeObject(BTCallContextData.CurrentNlogTraceData));
            }
            catch (Exception ex)
            {
                LogService.Default.Fatal(ex, "post请求添加追踪日志异常：" + ex.Message + ex.StackTrace);
            }

            bag.req.UseDefaultCredentials = true;
            var restClient = new RestClient { BaseUrl = new Uri(bag.uri) };
            var tcs = new TaskCompletionSource<IRestResponse>();
            restClient.ExecuteAsync(bag.req, r => {
                tcs.SetResult(r);
            });
            return tcs.Task;
        }


        /// <summary>
        /// 下载文件接口
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        public static Stream FileDownLoad(string requestUri, string json)
        {
            //json格式请求数据

            string requestData = json;
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(requestUri);
            //utf-8编码
            byte[] buf = System.Text.Encoding.GetEncoding("utf-8").GetBytes(requestData);

            //post请求
            myRequest.Method = "POST";
            myRequest.ContentLength = buf.Length;
            myRequest.MaximumAutomaticRedirections = 1;
            myRequest.Headers.Add("Api-Version", "1.0");
            myRequest.AllowAutoRedirect = true;


            myRequest.ContentType = "application/json; charset=utf-8";
            myRequest.Accept = "application/json";

            Stream newStream = myRequest.GetRequestStream();
            newStream.Write(buf, 0, buf.Length);
            newStream.Close();

            string ReqResult = string.Empty;
            HttpWebResponse myResponse = null;
            try
            {
                myResponse = (HttpWebResponse)myRequest.GetResponse();

            }
            catch (Exception e)
            {
                throw e;
            }

            return myResponse.GetResponseStream();
        }

        

        public static byte[] GetImage(string url)
        {
            try
            {
                WebClient wc = new WebClient();
                byte[] bytes;
                bytes = wc.DownloadData(url);
                return bytes;
            }
            catch (Exception ex)
            {
                LogService.Default.Fatal("下载的图片出错：" + url + ";" + ex.Message);
                throw ex;
            }
        }
    }



}