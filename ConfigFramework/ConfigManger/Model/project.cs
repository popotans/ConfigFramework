using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZF.Db;

namespace ConfigFramework.ConfigManger.Model
{
    public class Project
    {
        public long Id { get; set; }
        public string ProjectName { get; set; }
        public string CategoryIds { get; set; }
        public string Remark { get; set; }
        public DateTime CreateTime { get; set; }

        public static Project CreateModel(DataRow dr)
        {
            Project project = new Project();
            if (dr.Table.Columns.Contains("Id"))
            {
                project.Id = LibConvert.ObjToInt64(dr["Id"]);
            }
            if (dr.Table.Columns.Contains("ProjectName"))
            {
                project.ProjectName = LibConvert.ObjToStr(dr["ProjectName"]);
            }
            if (dr.Table.Columns.Contains("CategoryIds"))
            {
                project.CategoryIds = LibConvert.ObjToStr(dr["CategoryIds"]);
            }
            if (dr.Table.Columns.Contains("Remark"))
            {
                project.Remark = LibConvert.ObjToStr(dr["Remark"]);
            }
            if (dr.Table.Columns.Contains("CreateTime"))
            {
                project.CreateTime = LibConvert.ObjToDateTime(dr["CreateTime"]);
            }
            return project;
        }
    }
}
