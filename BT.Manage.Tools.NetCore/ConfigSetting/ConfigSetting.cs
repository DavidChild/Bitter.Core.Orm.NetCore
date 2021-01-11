using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace BT.Manage.Tools
{
    public class ConfigSetting
    {
        /// <summary>
        /// 配置文件目录
        /// </summary>
        private static string jsonPath = "config.json";


        public static T Settings<T>(string key) where T : class,new()
        {
          

            IConfiguration config = new ConfigurationBuilder()
                
                .Add(new JsonConfigurationSource { Path = jsonPath, Optional = false, ReloadOnChange = true })
                .Build();
                var appconfig = new ServiceCollection()
                .AddOptions()
                .Configure<T>(config.GetSection(key))
                .BuildServiceProvider()
                .GetService<IOptions<T>>()
                .Value;
            return appconfig;
        }
        /// <summary>
        /// 配置文件信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string  Settings(string key)
        {
            IConfiguration config = new ConfigurationBuilder()
              .Add(new JsonConfigurationSource { Path = jsonPath, Optional = false, ReloadOnChange = true })
                .Build();
           return  config.GetSection(key).Value;
           
         
        }
        public static IServiceCollection SettingsRetrunIServiceCollection<T>(string key) where T : class, new()
        {


            IConfiguration config = new ConfigurationBuilder()
                .Add(new JsonConfigurationSource { Path = jsonPath, Optional = false, ReloadOnChange = true })
                .Build();
            var appconfig = new ServiceCollection()
                .AddOptions()
                .Configure<T>(config.GetSection(key));
               return appconfig;
        }
        public static void ServiceCollectionSetting<T>(string key, IServiceCollection serviceCollection) where T : class, new()
        {


            IConfiguration config = new ConfigurationBuilder()
                .Add(new JsonConfigurationSource { Path = jsonPath, Optional = false, ReloadOnChange = true })
                .Build();
                 serviceCollection
                .AddOptions()
                .Configure<T>(config.GetSection(key));
           
        }

    }
}
