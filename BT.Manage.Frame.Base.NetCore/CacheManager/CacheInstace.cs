using CacheManager.Core;
using System;
using System.Collections.Generic;
using System.Text;
using BT.Manage.Tools.Utils;
using BT.Manage.Frame.Base.NetCore.ConfigManage;
using System.Net;

namespace BT.Manage.Frame.Base
{
    public class CacheInstace<T>
    {

        private static ICacheManager<T> memerySingleton { get; set; }
        private static ICacheManager<T> redisCacheSingleton { get; set; }


        private static readonly object lockobjk = string.Empty;
        /// <summary>
        /// 获取Redis缓存实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public static CacheManager.Core.ICacheManager<T> GetRedisInstace()
        {
            var rediscon = SystemJsonConfigManage.GetInstance().AppSettings["RedisCon"].ToSafeString();
            rediscon.CheckNotNullOrEmpty("错误: appsettings.json->appsetting下未配置节点:RedisCon").ThrowUserFriendlyException();
            if (redisCacheSingleton == null)
            {
                lock (lockobjk)
                {
                    if (redisCacheSingleton == null)
                    {
                        redisCacheSingleton = (CacheManager.Core.CacheFactory.Build<T>(settings =>
                        {

                            settings
                                .WithRedisConfiguration("redis", config =>//Redis缓存配置
                    {
                                    config.WithAllowAdmin()
                                        .WithDatabase(0)
                                        .WithEndpoint((rediscon.Split(':')[0]).ToSafeString(""), (rediscon.Split(':')[1]).ToSafeInt32(0));

                                })
                                .WithMaxRetries(1000)//尝试次数
                                .WithRetryTimeout(100)//尝试超时时间
                                .WithJsonSerializer()
                                .WithRedisCacheHandle("redis", true);//redis缓存handle

                        }));
                        return redisCacheSingleton;

                    }
                    else
                    {
                        return redisCacheSingleton;
                    }
                }
            }
            else
            {
                return redisCacheSingleton;
            }





            
        }


        /// <summary>
        /// 获取内存缓存单例实例
        /// <typeparam name="T"></typeparam>
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static ICacheManager<T> GetMemeryInstace()
        {

            if (memerySingleton == null)
            {
                lock (lockobjk)
                {
                    if (memerySingleton == null)
                    {
                        memerySingleton = CacheManager.Core.CacheFactory.Build<T>("StartedMemoryCacheBlock",
                        settings => {
                            settings.WithMicrosoftMemoryCacheHandle("inMyProcessCache");
                        });
                        return memerySingleton;
                    }
                    else
                    {
                        return memerySingleton;
                    }
                }
            }
            else
            {
                return memerySingleton;
            }

        }


    


        /// <summary>
        /// 获取二级缓存实例,一级缓存本地内存，二级缓存Redis。 本地内存的依赖蓝本数据为Redis为准。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public static CacheManager.Core.ICacheManager<T> GetMultilevelInstace()
        {
 
            var rediscon = SystemJsonConfigManage.GetInstance().AppSettings["RedisCon"].ToSafeString();
            rediscon.CheckNotNullOrEmpty("错误: appsettings.json->appsetting下未配置节点:RedisCon").ThrowUserFriendlyException();
            
            return (CacheManager.Core.CacheFactory.Build<T>(settings => 
            {

                settings.WithMicrosoftMemoryCacheHandle("inProcessCache")//内存缓存Handle
                    .And
                    .WithRedisConfiguration("redis", config =>//Redis缓存配置
                    {
                        config.WithAllowAdmin()
                            .WithDatabase(0)
                           .WithEndpoint((rediscon.Split(':')[0]).ToSafeString(""), (rediscon.Split(':')[1]).ToSafeInt32(0));
                    })
                    .WithMaxRetries(1000)//尝试次数
                    .WithRetryTimeout(100)//尝试超时时间
                    .WithRedisBackplane("redis") //redis使用Back Plate
                    .WithJsonSerializer()
                    .WithRedisCacheHandle("redis", true);//redis缓存handle
            }));
        }


        /// <summary>
        /// 检查是否配置了redis
        /// </summary>
        /// <returns></returns>
        public static bool CheckedRedisConsetting()
        {
            var rediscon = SystemJsonConfigManage.GetInstance().AppSettings["RedisCon"].ToSafeString();
            return string.IsNullOrEmpty(rediscon) ? false : true;
        }


        /// <summary>
        /// 获取缓存实例，优先redis,默认为内存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ICacheManager<T> GetRedisInstanceDefaultMemery()
        {
            if (CheckedRedisConsetting())
            {
                return GetRedisInstace();
            } else
            {
                return GetMemeryInstace();
            }
        }
    }
}
