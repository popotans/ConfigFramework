using ConfigFramework.ConfigManger.Dal;
using ConfigFramework.ConfigManger.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZF.IOHelper;
using ZF.Log;

namespace ConfigFramework.ConfigManger.systemruntime
{
    /// <summary>
    /// 配置心跳守护
    /// </summary>
    public class ConfigHeartbeatProtect : IDisposable
    {
        private static ConfigHeartbeatProtect configHeartbeat = null;
        private static CancellationTokenSource cancelSource;
        private static object _lockheart = new object();  //单例锁
        private static object _lockconfig = new object(); //更新锁

        private static RedisCommandListener redislistener = null;

        private ConfigHeartbeatProtect()
        {
            try
            {
                cancelSource = new CancellationTokenSource();

                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    HeartRun();
                }, cancelSource.Token);

                redislistener = new RedisCommandListener(AppDomainContext.Context.ConfigParams.RedisServer);
                redislistener.Register((channle, msg) => 
                {
                    RedisCommandListen(channle, msg);
                }, cancelSource, RedisCommandConfig.Redis_channle);
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

        }

        public void HeartRun()
        {
            while (!cancelSource.IsCancellationRequested)
            {
                System.Threading.Thread.Sleep(10 * 1000);
                try
                {
                    LoadConfig(false);
                    LogHelper.WriteInfo("心跳循环配置一次");
                }
                catch (Exception ex)
                {
                    LogHelper.WriteError(ex);
                }
            }
        }

        public void RedisCommandListen(string channle, string msg)
        {
            if (!string.IsNullOrEmpty(msg))
            {
                LoadConfig(true);
                LogHelper.WriteInfo("RedisCommandListen执行一次，msg:" + msg);
            }
        }

        /// <summary>
        /// 单例
        /// </summary>
        /// <returns></returns>
        public static ConfigHeartbeatProtect Instance()
        {
            if (configHeartbeat == null)
            {
                lock (_lockheart)
                {
                    configHeartbeat = new ConfigHeartbeatProtect();
                }
            }
            return configHeartbeat;
        }

        /// <summary>
        /// 是否重新下载项目信息
        /// </summary>
        /// <param name="updatejson">是否重新下载配置分类</param>
        public void LoadConfig(bool isload)
        {
            lock (_lockconfig)
            {
                string jasonpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "json", "json.txt");
                try
                {
                    bool isupdatelocal = false;
                    DateTime updatetime = DateTime.Now;
                    
                    if (isload)
                    {
                        SystemConfigDal scdal = new SystemConfigDal();
                        SystemConfig sc = scdal.GetRedisServer(AppDomainContext.Context.ConfigParams.ConfigManagerConnectString);
                        if (sc != null)
                        {
                            AppDomainContext.Context.ConfigParams.RedisServer = sc.ConfigValue;
                        }

                        //更新项目信息
                        ProjectDal prodal = new ProjectDal();
                        AppDomainContext.Context.ProjectModel = prodal.GetByName(AppDomainContext.Context.ConfigParams.ConfigManagerConnectString, AppDomainContext.Context.ConfigParams.ProjectName);

                        //更新分类信息
                        CategoryDal catedal=new CategoryDal();
                        AppDomainContext.Context.CategoryModels = catedal.GetListByIds(AppDomainContext.Context.ConfigParams.ConfigManagerConnectString, AppDomainContext.Context.ProjectModel.CategoryIds);
                        long[] cids = AppDomainContext.Context.CategoryModels.Select(p => p.Id).ToArray();
                        //更新配置信息
                        ConfigDal configdal = new ConfigDal();
                        List<Config> configs = configdal.GetListByCategoryIds(AppDomainContext.Context.ConfigParams.ConfigManagerConnectString, cids, updatetime);
                        List<ConfigModel> configmodels = ToConfigModel(configs);

                        ConfigInfoOfKeyDic dic = new ConfigInfoOfKeyDic();
                        foreach (var item in configmodels)
                        {
                            dic.SetConfig(item);
                        }
                        AppDomainContext.Context.ConfigInfoOfKeyDic = dic;

                        if (configs.Count > 0)
                        {
                            configdal.SetUpdatetime(AppDomainContext.Context.ConfigParams.ConfigManagerConnectString, cids, updatetime);
                            isupdatelocal = true;
                        }
                        
                    }
                    else
                    {
                        ConfigDal configdal = new ConfigDal();
                        long[] cids = AppDomainContext.Context.CategoryModels.Select(p => p.Id).ToArray();

                        List<Config> configs = configdal.GetListByCategoryIds(AppDomainContext.Context.ConfigParams.ConfigManagerConnectString, cids, updatetime);
                        List<ConfigModel> configmodels = ToConfigModel(configs);
                        foreach (var item in configmodels)
                        {
                            AppDomainContext.Context.ConfigInfoOfKeyDic.SetConfig(item);
                        }
                        if (configs.Count > 0)  //本次有配置更新
                        {
                            configdal.SetUpdatetime(AppDomainContext.Context.ConfigParams.ConfigManagerConnectString, cids, updatetime);
                            isupdatelocal = true;
                        }
                    }

                    if (isupdatelocal)
                    {
                        string json1 = JsonConvert.SerializeObject(AppDomainContext.Context);
                        IOHelper.Write(jasonpath, json1);  //写入磁盘
                        LogHelper.WriteInfo("写入磁盘成功");
                    }
                }
                catch (Exception)
                {
                    string json2 = IOHelper.Read(jasonpath);
                    JsonConvert.DeserializeObject<ConfigContext>(json2);   //从磁盘获得上次正确的配置
                    LogHelper.WriteInfo("从磁盘读取配置");
                }
            }
        }

        //Config信息转换到ConfigModel   通过ConfigModel可配置故障转移配置轮询等
        public List<ConfigModel> ToConfigModel(List<Config> configs)
        {
            List<ConfigModel> modellist = new List<ConfigModel>();
            foreach (var item in configs)
            {
                ConfigModel model = new ConfigModel();
                model.config = item;
                modellist.Add(model);
            }
            return modellist;
        }

        

        public void Dispose()
        {
            if (cancelSource != null)
                cancelSource.Cancel();
            if (redislistener != null)
                redislistener.Dispose();
        }
    }
}
