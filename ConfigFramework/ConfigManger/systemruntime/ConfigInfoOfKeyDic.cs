using ConfigFramework.ConfigManger.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigFramework.ConfigManger.systemruntime
{
    public class ConfigInfoOfKeyDic : Dictionary<string, ConfigModel>   
    {
        /// <summary>
        /// key存在增加，不存在更新
        /// </summary>
        /// <param name="configmodel"></param>
        public void SetConfig(ConfigModel configmodel)
        {
            if (this.ContainsKey(configmodel.config.ConfigKey))
            {
                this[configmodel.config.ConfigKey] = configmodel;
            }
            this.Add(configmodel.config.ConfigKey, configmodel);
        }

        public ConfigModel GetConfig(string key)
        {
            return this[key];
        }
    }
}
