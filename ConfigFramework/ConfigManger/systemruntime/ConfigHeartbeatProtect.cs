using ConfigFramework.ConfigManger.Dal;
using ConfigFramework.ConfigManger.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConfigFramework.ConfigManger.systemruntime
{
    /// <summary>
    /// 配置心跳守护
    /// </summary>
    public class ConfigHeartbeatProtect:IDisposable
    {
        private CancellationTokenSource cancelSource;  //Task任务is取消
        private static ConfigHeartbeatProtect _configheartbeatprotect;  //单例
        private static object _instancelock = new object();//单例实例锁
        private static object _contextupdatelock = new object();//上下文更新锁 
        //private RedisNetCommandListener redislistener = null;
        public ConfigHeartbeatProtect()
        {
            cancelSource = new CancellationTokenSource();

            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                HeatbeatRun();//注册心跳
            }, cancelSource.Token);
        }

        /// <summary>
        /// 单例
        /// </summary>
        /// <returns></returns>
        public static ConfigHeartbeatProtect Instance()
        {
            if (_configheartbeatprotect != null)
            {
                return _configheartbeatprotect;
            }
            else
            {
                lock (_instancelock)
                {
                    _configheartbeatprotect = new ConfigHeartbeatProtect();
                    return _configheartbeatprotect;
                }
            }
        }

        /// <summary>
        /// 注册心跳
        /// </summary>
        private void HeatbeatRun()
        {
            while (!cancelSource.IsCancellationRequested)
            {
                System.Threading.Thread.Sleep(10 * 1000);
                try
                {
                    LoadConfig(true);
                }
                catch (Exception exp)
                {
                    //LogHelper.Error(-1, "配置心跳循环错误", exp);
                }
                //LogHelper.Debug("配置心跳循环一次");
            }
        }

        /// <summary>
        /// 加载配置信息
        /// </summary>
        /// <param name="isupdata">是否刷新配置信息</param>
        public void LoadConfig(bool isupdata)
        {
            lock (_contextupdatelock)
            {
                string localcachexmlpath = AppDomain.CurrentDomain.BaseDirectory.Trim('\\') + "\\" + "temp\\" + "config.localcache.json";
                bool islocalcache = true;//是否需要序列化到本地缓存
                try
                {
                    DateTime? updatetime = null;
                        
                        if (isupdata == true)  //刷新
                        {
                            ConfigParams configparams;
                            //加载系统配置
                            AppDomainContext.Context.ConfigParams.RedisServer = 
                            redislistener.RedisServerIp = AppDomainContext.Context.ConfigParams.RedisServer;
                            List<project> configinfos = ProjectDal.GetAll();
                            foreach (var config in configinfos)
                            {
                                configparams = new ConfigParams() { ProjectName = config.ProjectName, ConfigManagerConnectString = config.ConnectionString };
                            }
                            AppDomainContext.Context.ConfigParams = configparams;
                            //LogHelper.Debug("重新加载配置成功");
                            //LogHelper.Log(-1, "重新加载配置成功");
                        }
                        else    //不刷新从
                        {
                            //刷新系统配置
                            redislistener.RedisServerIp = AppDomainContext.Context.ConfigParams.RedisServer;

                            //更新项目心跳
                            Dal.tb_project_dal projectdal = new Dal.tb_project_dal();
                            projectdal.UpdateLastheartbeattime(c, AppDomainContext.Context.ProjectModel.id);
                            //更新配置信息
                            Dal.tb_config_dal configdal = new Dal.tb_config_dal();
                            var updatedconfigs = configdal.GetListByCategoryIDs(c, CommonHelper.GetCategoryIDs(AppDomainContext.Context.CategoryModels), AppDomainContext.Context.LastUpdateTime);
                            var configinfos = ToConfigInfoModels(updatedconfigs);
                            var categoryids = CommonHelper.GetProjectCategoryIDs(AppDomainContext.Context.ProjectModel.categoryids);
                            foreach (var config in configinfos)
                            {
                                AppDomainContext.Context.ConfigInfoOfKeyDic.SetConfig(config, categoryids);
                            }
                            if (configinfos.Count == 0)
                                islocalcache = false;//本次没有更新配置
                            //LogHelper.Debug("更新当前配置成功");
                        }
                    
                    if (islocalcache)
                    {
                        //本地磁盘缓存
                        //Common.IOHelper.CreateDirectory(localcachexmlpath);
                        var xml = JsonConvert.SerializeObject(AppDomainContext.Context); //use json.net,未来加密处理
                        System.IO.File.WriteAllText(localcachexmlpath, xml);
                        //LogHelper.Debug("当前上下文序列化到磁盘成功");
                    }
                    if (updatetime != null)
                        AppDomainContext.Context.LastUpdateTime = updatetime.Value;

                }
                catch (Exception exp)
                {
                    //LogHelper.Error(-1, "统一配置中心获取配置失败,本地磁盘缓存恢复上一次可用配置", exp);
                    //本地磁盘缓存恢复上一次可用配置
                    if (System.IO.File.Exists(localcachexmlpath))
                    {
                        var xml = System.IO.File.ReadAllText(localcachexmlpath);
                        AppDomainContext.Context = JsonConvert.DeserializeObject<ConfigContext>(xml);//use json.net,未来解密处理
                    }

                    //LogHelper.Debug("统一配置中心获取配置失败,本地磁盘缓存恢复上一次可用配置");
                    //throw exp;
                }
            }
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
