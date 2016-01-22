using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigFramework.ConfigManger.Model
{
    [Serializable]
    public class ConfigModel
    {
        public Config config { get; set; }

        //此类可以扩展 包括config的故障处理
        public string Value()
        {
            return config.ConfigValue;
        }
    }
}
