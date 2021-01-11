using BT.Manage.Frame.Base.NetCore.ConfigSetting;
using BT.Manage.Tools;
using BT.Manage.Tools.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;

namespace BT.Manage.Frame.Base.NetCore.ConfigManage
{
    public class JsonConfigMange
    {
        // 定义一个标识确保线程同步
        private static readonly object locker = new object();

        private static JsonConfigMange _configManage;

        private IConfigurationRoot configOperat;

        private string jsonFileName="BTCusSetting.json";

        
        private JsonConfigMange()
        {

        }

        public static JsonConfigMange GetInstance()
        {
            lock (locker)//确保线程安全
            {
                if(_configManage==null)
                {

                    _configManage = new JsonConfigMange();
                    _configManage.configOperat= new ConfigurationBuilder()
                    .AddInMemoryCollection()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(_configManage.jsonFileName, optional: true, reloadOnChange: true)
                    .Build();
                }
                return _configManage;
            }
        }
       
        public  T Settings<T>(string key) where T : class, new()
        {

            var appconfig = new ServiceCollection()
            .AddOptions()
            .Configure<T>(_configManage.configOperat.GetSection(key))
            .BuildServiceProvider()
            .GetService<IOptions<T>>()
            .Value;
            return appconfig;
        }
        /// <summary>
        /// AppSettings配置信息
        /// </summary>
        public NameValueCollection AppSettings { get {
                var list = _configManage.configOperat.GetSection("AppSettings").GetChildren().ToList();
                NameValueCollection dc = new NameValueCollection();
                foreach (var item in list)
                {
                    dc.Add(item.Key, item.Value);
                }
                return dc;
            }
        }
       

        /// <summary>
        /// 热修改配置文件信息
        /// </summary>
        /// <param name="configList"></param>
        public void WiteConfig(List<ConfigValue> configList)
        {
            var path= Directory.GetCurrentDirectory() + "/"+ jsonFileName;
            BTConfigBaseInfo baseModel=new BTConfigBaseInfo();
            //读取配置文件信息
            using (StreamReader sr = new StreamReader(path))
            {
                try
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Converters.Add(new JavaScriptDateTimeConverter());
                    serializer.NullValueHandling = NullValueHandling.Ignore;

                    //构建Json.net的读取流  
                    JsonReader reader = new JsonTextReader(sr);
                    //对读取出的Json.net的reader流进行反序列化，并装载到模型中  
                    baseModel = serializer.Deserialize<BTConfigBaseInfo>(reader);
                }
                catch (Exception ex)
                {
                    LogService.Default.Fatal(ex, "json配置读取错误:" + ex.Message);
                    throw ex;
                }
               
            }
            if (baseModel.AppSettings == null)
                baseModel.AppSettings = new Dictionary<string, object>();
            //对比配置信息
            foreach (var conitem in configList)
            {
                if (conitem.ConfigType == 1)//数据库连接配置
                {
                    if (!string.IsNullOrEmpty(conitem.Value))
                    {
                        var newconlist = JsonConvert.DeserializeObject<List<SelfConnectionStringEntity>>(conitem.Value);
                        if (baseModel.connectionString != null && baseModel.connectionString.Count > 0)
                        {
                            //比对新旧配置MD5加密值
                            var olcconMd5 = EncryptUtils.To32Md5(JsonConvert.SerializeObject(baseModel.connectionString));
                            var newconMd5 = EncryptUtils.To32Md5(JsonConvert.SerializeObject(newconlist));
                            if (olcconMd5 != newconMd5)
                                baseModel.connectionString = newconlist;
                        }
                        else
                            baseModel.connectionString = newconlist;
                    }
                    
                }
                else
                {
                    if (!baseModel.AppSettings.ContainsKey(conitem.Key))
                    {
                        baseModel.AppSettings.Add(conitem.Key, conitem.Value);
                    }
                    else if (baseModel.AppSettings[conitem.Key].ToString() != conitem.Value)
                    {
                        baseModel.AppSettings[conitem.Key] = conitem.Value;
                    }
                }
                
            }
            //重新写入到json文件
            using (StreamWriter sw = new StreamWriter(path))
            {
                try
                {
                   
                    JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings() { Formatting= Formatting.Indented});
                    serializer.Converters.Add(new JavaScriptDateTimeConverter());
                    serializer.NullValueHandling = NullValueHandling.Ignore;
                    JsonWriter writer = new JsonTextWriter(sw);
                    serializer.Serialize(writer, baseModel);
                    writer.Close();
                    sw.Close();
                }
                catch (Exception ex)
                {
                    LogService.Default.Fatal(ex, "json配置写入错误:" + ex.Message+" 异常堆 :"+ex.StackTrace);
                    throw ex;
                }
            }
        }
    }
}
