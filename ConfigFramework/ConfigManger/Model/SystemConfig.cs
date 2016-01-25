using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZF.Db;

namespace ConfigFramework.ConfigManger.Model
{
    public class SystemConfig
    {
        public long Id { get; set; }
        public string ConfigKey { get; set; }
        public string ConfigValue { get; set; }
        public string Remark { get; set; }
        public static SystemConfig CreateModel(DataRow dr)
        {
            SystemConfig o = new SystemConfig();
            if (dr.Table.Columns.Contains("Id"))
            {
                o.Id = LibConvert.ObjToInt64(dr["Id"]);
            }
            if (dr.Table.Columns.Contains("ConfigKey"))
            {
                o.ConfigKey = LibConvert.ObjToStr("ConfigKey");
            }
            if (dr.Table.Columns.Contains("ConfigValue"))
            {
                o.ConfigValue = LibConvert.ObjToStr(dr["ConfigValue"]);
            }
            if (dr.Table.Columns.Contains("Remark"))
            {
                o.Remark = LibConvert.ObjToStr(dr["Remark"]);
            }
            return o;
        }
    }
}
