using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZF.Db;

namespace ConfigFramework.Model
{
    public class Config
    {
        public long Id { get; set; }
        public long CategoryId { get; set; }
        public string ConfigKey { get; set; }
        public string ConfigValue { get; set; }
        public string Remark { get; set; }
        public DateTime CreateTime { get; set; }

        public static Config CreateModel(DataRow dr)
        {
            Config config = new Config();
            if (dr.Table.Columns.Contains("Id"))
            {
                config.Id = LibConvert.ObjToInt64(dr["Id"]);
            }
            if (dr.Table.Columns.Contains("CategoryId"))
            {
                config.CategoryId = LibConvert.ObjToInt64(dr["CategoryId"]);
            }
            if (dr.Table.Columns.Contains("ConfigKey"))
            {
                config.ConfigKey = LibConvert.ObjToStr(dr["ConfigKey"]);
            }
            if (dr.Table.Columns.Contains("ConfigValue"))
            {
                config.ConfigValue = LibConvert.ObjToStr(dr["ConfigValue"]);
            }
            if (dr.Table.Columns.Contains("Remark"))
            {
                config.Remark = LibConvert.ObjToStr(dr["Remark"]);
            }
            if (dr.Table.Columns.Contains("CreateTime"))
            {
                config.CreateTime = LibConvert.ObjToDateTime(dr["CreateTime"]);
            }
            return config;
        }
    }
}
