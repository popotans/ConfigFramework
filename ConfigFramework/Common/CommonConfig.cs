using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigFramework;

namespace ConfigFramework.Common
{
    public class CommonConfig
    {
        public static string ConfigManagerConnectString { get { return Get("ConfigManagerConnectString", ""); } }
        public static string ProjectName { get { return Get("ProjectName", "未命名项目"); } }
        public static string Get(string key, string defaultvalue = "")
        {
            if (key == "ConfigManagerConnectString" || key == "ProjectName" || ProjectName == "未命名项目" || string.IsNullOrWhiteSpace(ProjectName) || string.IsNullOrWhiteSpace(ConfigManagerConnectString))
            {
                if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains(key))
                {
                    return System.Configuration.ConfigurationManager.AppSettings[key];
                }
                else
                {
                    return defaultvalue;
                }  
            }
            else
            {
                try
                {
                    var value = ConfigMangerHelper.Get<string>(key);
                    if (value != null)
                        return value;
                }
                catch
                {
                    return defaultvalue;
                }

            }
            return defaultvalue;
        }
    }
}
