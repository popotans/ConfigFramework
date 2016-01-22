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
    public class ConfigHeartbeatProtect : IDisposable
    {
        private static ConfigHeartbeatProtect configHeartbeat;
        private static CancellationTokenSource cancelSource;
        private object _lockheart = new object();
        private object _lockconfig = new object();

        public ConfigHeartbeatProtect()
        {
            
        }

        public static ConfigHeartbeatProtect Instance()
        {
            if (configHeartbeat == null)
            {
                return new ConfigHeartbeatProtect();
            }
            else
            {
                return configHeartbeat;
            }
        }

        public void LoadConfig(bool updatejson)
        {
            lock (_lockconfig)
            { 
            
            }
        }

        public void Dispose()
        {
            if (cancelSource != null)
                cancelSource.Cancel();
            //if (redislistener != null)
            //    redislistener.Dispose();
        }
    }
}
