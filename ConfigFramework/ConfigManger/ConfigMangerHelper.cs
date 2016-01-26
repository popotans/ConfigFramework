using ConfigFramework.Common;
using ConfigFramework.ConfigManger.Dal;
using ConfigFramework.ConfigManger.systemruntime;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZF.Log;

namespace ConfigFramework
{
    public class ConfigMangerHelper
    {
        private static object _singletonlock = new object(); //锁
        private static ConfigMangerHelper _sigleconfig;

        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            try
            {
                if (_singletonlock != null)
                {
                    ConfigHeartbeatProtect.Instance().Dispose();
                    //LogHelper.Log(-1, string.Format("当前域进程退出时释放完毕,服务器ip地址:{0}", CommonHelper.GetDefaultIP()));
                }
            }
            catch (Exception exp)
            {
                //LogHelper.Error(-1, string.Format("当前域进程退出时释放出错,服务器ip地址:{0}", CommonHelper.GetDefaultIP()), exp);
                throw exp;
            }
        }
        static void CurrentDomain_DomainUnload(object sender, EventArgs e)
        {
            try
            {
                if (_singletonlock != null)
                {
                    ConfigHeartbeatProtect.Instance().Dispose();
                    //LogHelper.Log(-1, string.Format("当前域域卸载时释放完毕,服务器ip地址:{0}", CommonHelper.GetDefaultIP()));
                }
            }
            catch (Exception exp)
            {
                //LogHelper.Error(-1, string.Format("当前域域卸载释放出错,服务器ip地址:{0}", CommonHelper.GetDefaultIP()), exp);
                throw exp;
            }
        }
        /// <summary>
        /// 获得配置中心唯一实例
        /// </summary>
        /// <returns></returns>
        public static ConfigMangerHelper GetInstance(string configkey)
        {
            if (_sigleconfig == null)
            {
                lock (_singletonlock)
                {
                    if (_sigleconfig == null)
                    {
                        if (string.IsNullOrEmpty(CommonConfig.ProjectName) || CommonConfig.ProjectName=="未命名项目")
                        {
                            LogHelper.WriteError("请选择请在web.config或AppSettings.config中配置ProjectName");
                            throw new Exception("请选择请在web.config或AppSettings.config中配置ProjectName");
                        }
                        if (string.IsNullOrEmpty(CommonConfig.ConfigManagerConnectString)) 
                        {
                            LogHelper.WriteError("请在web.config或AppSettings.config中配置ConfigRedisConnectString");
                            throw new Exception("请在web.config或AppSettings.config中配置ConfigRedisConnectString");
                        }
                        SystemConfigDal redisdal = new SystemConfigDal();
                        string redisserver = "";
                        redisserver = redisdal.GetRedisServer(CommonConfig.ConfigManagerConnectString).ConfigValue;
                        AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
                        AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
                        ConfigContext context = new ConfigContext(); 
                        context.ConfigParams = new ConfigParams() 
                        {
                            ProjectName = CommonConfig.ProjectName,
                            ConfigManagerConnectString = CommonConfig.ConfigManagerConnectString,
                            RedisServer = redisserver 
                        };
                        AppDomainContext.Context = context;
                        ConfigHeartbeatProtect.Instance().LoadConfig(true);
                        _sigleconfig = new ConfigMangerHelper();
                    }
                }
            }
            return _sigleconfig;
        }

        /// <summary>
        /// 获得config
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configkey"></param>
        /// <returns></returns>
        public static T Get<T>(string configkey)
        {
            string value = null;
            if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains(configkey))
            {
                value = System.Configuration.ConfigurationManager.AppSettings[configkey];
            }
            else
            {
                if (GetInstance(configkey) != null)
                {
                    if (AppDomainContext.Context == null)
                        throw new Exception("当前项目未在配置中心获取配置列表");
                    var config = AppDomainContext.Context.ConfigInfoOfKeyDic.GetConfig(configkey);
                    if (config == null)
                        throw new Exception(string.Format("未找到当前配置项,请联系管理员在配置中心中添加.key:{0}", configkey));
                    value = config.Value();
                }
            }
            return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
        }
    }
}
