using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigFramework.ConfigManger.systemruntime
{
    /// <summary>
    /// 配置文件上下文
    /// </summary>
    [Serializable]
    public class ConfigContext
    {
        /// <summary>
        /// 配置参数信息
        /// </summary>
        public ConfigParams ConfigParams { get; set; }
        /// <summary>
        /// 项目信息
        /// </summary>
        //public tb_project_model ProjectModel { get; set; }
        /// <summary>
        /// 分类信息
        /// </summary>
        //public List<tb_category_model> CategoryModels { get; set; }
        /// <summary>
        /// 配置信息字典
        /// </summary>
        //public ConfigInfoOfKeyDic ConfigInfoOfKeyDic { get; set; }
        /// <summary>
        /// 配置最后更新时间（以配置数据库服务器时间为标准时间）
        /// </summary>
        public DateTime LastUpdateTime { get; set; }
    }

    /// <summary>
    /// 当前应用域上下文
    /// </summary>
    public class AppDomainContext
    {
        public static ConfigContext Context;
    }

    [Serializable]
    public class ConfigParams
    {
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConfigManagerConnectString { get; set; }
        /// <summary>
        /// redisip
        /// </summary>
        public string RedisServer { get; set; }
    }
}
