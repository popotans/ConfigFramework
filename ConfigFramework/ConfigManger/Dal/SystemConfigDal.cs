using ConfigFramework.ConfigManger.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZF.Db.SqlHelper;

namespace ConfigFramework.ConfigManger.Dal
{
    public class SystemConfigDal
    {
        public SystemConfig GetRedisServer()
        {
            SystemConfig config = new SystemConfig();
            string conn = ConfigMangerHelper.Get<string>("ConfigManager");
            string sql = "SELECT Id,ConfigKey,ConfigValue,Remark FROM SystemConfig WHERE ConfigKey='RedisServer'";
            DataTable dt = SqlServerHelper.Get(conn, sql);
            if (dt.Rows.Count > 0)
            {
                config = SystemConfig.CreateModel(dt.Rows[0]);
            }
            return config;
        }
    }
}
