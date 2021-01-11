using BT.Manage.Frame.Base.NetCore.Common;
using BT.Manage.Tools;
using BT.Manage.Tools.Utils;
using Consul;
using Newtonsoft.Json;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BT.Manage.Frame.Base.NetCore.Consul
{
    public class ConsulNetClient
    {
        private ConsulOption _consulOption;
        public ConsulNetClient(ConsulOption consulOption)
        {
            //EnsureUtil.NotNull(consulOption, "consulOption");
            _consulOption = consulOption;
        }

        private ConsulClient _consulClient
        {
            get
            {
                Action<ConsulClientConfiguration> cfgAction = (cfg) =>
                {
                    cfg.Address = new Uri(_consulOption.Host);
                    cfg.Datacenter = _consulOption.DataCenter;
                    cfg.Token = _consulOption.Token;
                    cfg.WaitTime = _consulOption.WaitTime;
                };
                //Action<HttpClient> cfgHttpAction = (client) => { _consulOption.TimeOut?client.Timeout = _consulOption.TimeOut; };
                return new ConsulClient(cfgAction);
            }
        }

        public void Dispose()
        {
            _consulClient?.Dispose();
        }

        #region 获取kv配置信息
        public async Task<bool> KVAcquireAsync(KVPair p)
        {
            var result = await _consulClient.KV.Acquire(p);
            return result.Response;
        }

        public async Task<bool> KVAcquireAsync(KVPair p, WriteOptions q)
        {
            var result = await _consulClient.KV.Acquire(p, q);
            return result.Response;
        }

        public async Task<bool> KVCasAsync(KVPair p)
        {
            var result = await _consulClient.KV.CAS(p);
            return result.Response;
        }

        public async Task<bool> KVCasAsync(KVPair p, WriteOptions q)
        {
            var result = await _consulClient.KV.CAS(p, q);
            return result.Response;
        }

        public async Task<bool> KVDeleteAsync(string key)
        {
            var result = await _consulClient.KV.Delete(key);
            return result.Response;
        }

        public async Task<bool> KVDeleteAsync(string key, WriteOptions q)
        {
            var result = await _consulClient.KV.Delete(key, q);
            return result.Response;
        }

        public async Task<bool> KVDeleteCasAsync(KVPair p)
        {
            var result = await _consulClient.KV.DeleteCAS(p);
            return result.Response;
        }

        public async Task<bool> KVDeleteCasAsync(KVPair p, WriteOptions q)
        {
            var result = await _consulClient.KV.DeleteCAS(p, q);
            return result.Response;
        }

        public async Task<bool> KVDeleteTreeAsync(string prefix)
        {
            var result = await _consulClient.KV.DeleteTree(prefix);
            return result.Response;
        }

        public async Task<bool> KVDeleteTreeAsync(string prefix, WriteOptions q)
        {
            var result = await _consulClient.KV.DeleteTree(prefix, q);
            return result.Response;
        }

        public async Task<KVPair> KVGetAsync(string key)
        {
            var result = await _consulClient.KV.Get(key);
            return result.Response;
        }

        public async Task<KVPair> KVGetAsync(string key, QueryOptions q)
        {
            var result = await _consulClient.KV.Get(key, q);
            return result.Response;
        }

        public async Task<T> KVGetAsync<T>(string key) where T : class
        {
            var obj = await KVGetValueAsync(key);

            return JsonConvert.DeserializeObject<T>(obj);
        }

        public async Task<T> KVGetAsync<T>(string key, QueryOptions q) where T : class
        {
            var obj = await KVGetValueAsync(key, q);
            return JsonConvert.DeserializeObject<T>(obj);
        }

        public async Task<string> KVGetValueAsync(string key)
        {
            var obj = await KVGetAsync(key);
            return GetString(obj?.Value);
        }

        public async Task<string> KVGetValueAsync(string key, QueryOptions q)
        {
            var obj = await KVGetAsync(key, q);
            return GetString(obj?.Value);
        }

        public async Task<string[]> KVKeysAsync(string prefix)
        {
            var result = await _consulClient.KV.Keys(prefix);
            return result.Response;
        }


        public async Task<string[]> KVKeysAsync(string prefix, string separator)
        {
            var result = await _consulClient.KV.Keys(prefix, separator);
            return result.Response;
        }

        public async Task<string[]> KVKeysAsync(string prefix, string separator, QueryOptions q)
        {
            var result = await _consulClient.KV.Keys(prefix, separator, q);
            return result.Response;
        }

        public async Task<KVPair[]> KVListAsync(string prefix)
        {
            var result = await _consulClient.KV.List(prefix);
            return result.Response;
        }

        public async Task<KVPair[]> KVListAsync(string prefix, QueryOptions q)
        {
            var result = await _consulClient.KV.List(prefix, q);
            return result.Response;
        }

        public async Task<IEnumerable<T>> KVListAsync<T>(string prefix) where T : class
        {
            List<T> list = new List<T>();
            var kvList = await KVListAsync(prefix);
            if (kvList != null)
                foreach (var t in kvList?.ToList())
                {
                    list.Add(JsonConvert.DeserializeObject<T>(GetString(t.Value)));
                }
            return list;
        }

        public Task<bool> KVPutAsync(string key, string value)
        {
            KVPair kv = new KVPair(key)
            {
                Value = GetByte(value)
            };
            return KVPutAsync(kv);
        }

        public Task<bool> KVPutAsync<T>(string key, T value) where T : class
        {
            KVPair kv = new KVPair(key)
            {
                Value = GetByte(JsonConvert.SerializeObject(value))
            };
            return KVPutAsync(kv);
        }

        public async Task<bool> KVPutAsync(KVPair p)
        {
            var result = await _consulClient.KV.Put(p);
            return result.Response;
        }

        public async Task<bool> KVPutAsync(KVPair p, WriteOptions q)
        {
            var result = await _consulClient.KV.Put(p, q);
            return result.Response;
        }

        public async Task<bool> KVReleaseAsync(KVPair p)
        {
            var result = await _consulClient.KV.Release(p);
            return result.Response;
        }

        public async Task<bool> KVReleaseAsync(KVPair p, WriteOptions q)
        {
            var result = await _consulClient.KV.Release(p, q);
            return result.Response;
        }

        private byte[] GetByte(string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }

        private string GetString(byte[] value)
        {
            if (value == null)
                return string.Empty;
            return Encoding.UTF8.GetString(value);
        }

        #endregion

        #region 服务注册和发现
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="serviceEntity"></param>
        public void RegisterConsul(ServiceEntity serviceEntity)
        {
            //注册本地Client节点
            try
            {

                var httpcheck = new AgentServiceCheck()//配置健康检查
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),//服务启动多久后注册
                    Interval = TimeSpan.FromSeconds(10),//健康检查时间间隔，或者称为心跳间隔
                    HTTP = $"http://{serviceEntity.IP}:{serviceEntity.Port}/{serviceEntity.CheckUrl}",//健康检查地址
                    Timeout = TimeSpan.FromSeconds(20),
                };

                var registration = new AgentServiceRegistration()//配置服务注册参数
                {
                    Checks = new[] { httpcheck },
                    ID = serviceEntity.ServiceID(),
                    Name = serviceEntity.ServiceName,
                    Address = serviceEntity.IP,
                    Port = serviceEntity.Port,
                    Tags = serviceEntity.Tags
                    //new[] { $"urlprefix-/{serviceEntity.ServiceName}" }//添加 urlprefix-/servicename 格式的 tag 标签，以便 Fabio 识别
                };


                //定义重试机制 5次重试 延时加载
                var policy = Polly.Policy.HandleResult<List<CatalogService>>(r =>
                {
                    return r.Find(x => x.ServiceID == registration.ID && x.ServiceName == registration.Name) == null;
                }).WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

                List<CatalogService> findService = policy.Execute(() =>
                {
                    _consulClient.Agent.ServiceRegister(registration).Wait();//服务注册
                    var services = _consulClient.Catalog.Service(registration.Name).Result.Response.ToList();
                    return services;
                });


            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    LogService.Default.Fatal("服务注册失败:(当前配置的服务写地址" + $"{_consulOption.Host}" + "):" + ex.InnerException.Message);
                else
                    LogService.Default.Fatal("服务注册失败:(当前配置的服务写地址" + $"{_consulOption.Host}" + "):" + ex.Message);
            }
        }
        /// <summary>
        /// 取消服务注册
        /// </summary>
        /// <param name="serviceEntity"></param>
        public void DisposeRegister(ServiceEntity serviceEntity)
        {
            try
            {
                _consulClient.Agent.ServiceDeregister(serviceEntity.ServiceID()).Wait();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    LogService.Default.Fatal("取消服务注册失败(当前配置的服务写地址" + $"{_consulOption.Host}" + "):" + ex.InnerException.Message);
                }
                else
                    LogService.Default.Fatal("取消服务注册失败;(当前配置的服务写地址" + $"{_consulOption.Host}" + "):" + ex.Message);
            }
        }

      
        public List<string> ServiceDiscovery(string serviceName, bool isGreyTestServer = false, int greyTestFlag = 0)
        {
            try
            {

                ServiceEntity serviceEntity = new ServiceEntity();
                serviceEntity.ServiceName = serviceName;
                List<CatalogService> resultService = new List<CatalogService>();

                var services = _consulClient.Catalog.Service(serviceEntity.ServiceName).Result.Response.ToList();
                if (services == null || (!services.Any()))
                {
                    LogService.Default.Fatal("consul:找不到服务实例");
                    return null;

                }

                //灰度测试中
                if (greyTestFlag == 1)
                {
                    if (isGreyTestServer)
                        resultService = services.Where(x => x.ServiceTags.Contains(SysConstants.GreyTestTagStr)).ToList();
                    else
                        resultService = services.Where(x => !x.ServiceTags.Contains(SysConstants.GreyTestTagStr)).ToList();
                }
                else
                {
                    resultService = services;
                }


                if (resultService == null || (!resultService.Any()))
                {
                    LogService.Default.Fatal($"consul:找不到指定服务实例|{greyTestFlag}");
                    return null;

                }
                return resultService.Select(o => $"{ o.ServiceAddress}:{o.ServicePort}").ToList();
                //return $"{service.Address}:{service.Port}";

            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    LogService.Default.Fatal("服务发现失败(当前配置的服务写地址" + $"{_consulOption.Host}" + "):" + ex.InnerException.Message);
                }
                else
                    LogService.Default.Fatal("服务发现失败;(当前配置的服务写地址" + $"{_consulOption.Host}" + "):" + ex.Message);
                return null;
            }
        }
        #endregion
    }
}
