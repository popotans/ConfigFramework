using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZF.Db;

namespace ConfigFramework.ConfigManger.Model
{
    public class Category
    {
        public long Id { get; set; }
        public string CategoryName { get; set; }
        public string Remark { get; set; }
        public DateTime CreateTime { get; set; }

        public static Category CreateModel(DataRow dr)
        {
            Category category = new Category();
            if (dr.Table.Columns.Contains("Id"))
            {
                category.Id = LibConvert.ObjToInt64(dr["Id"]);
            }
            if (dr.Table.Columns.Contains("CategoryName"))
            {
                category.CategoryName = LibConvert.ObjToStr(dr["CategoryName"]);
            }
            if (dr.Table.Columns.Contains("Remark"))
            {
                category.Remark = LibConvert.ObjToStr(dr["Remark"]);
            }
            if (dr.Table.Columns.Contains("CreateTime"))
            {
                category.CreateTime = LibConvert.ObjToDateTime(dr["CreateTime"]);
            }

            return category;
        }
    }
}
