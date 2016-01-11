using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigFramework.ConfigManger.Model
{
    public class project
    {
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public string ConnectionString { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime LastUpdateTime { get; set; }
        public static project CreateModel(DataRow dr)
        {
            project pj = new project();
            if (dr.Table.Columns.Contains("Id"))
            {
                pj.Id = int.Parse(dr["Id"].ToString());
            }
            if (dr.Table.Columns.Contains("ProjectName"))
            {
                pj.ProjectName = dr["ProjectName"].ToString();
            }
            if (dr.Table.Columns.Contains("ConnectionString"))
            {
                pj.ConnectionString = dr["ConnectionString"].ToString();
            }
            if (dr.Table.Columns.Contains("CreateTime"))
            {
                pj.CreateTime = DateTime.Parse(dr["CreateTime"].ToString());
            }
            if (dr.Table.Columns.Contains("LastUpdateTime"))
            {
                pj.LastUpdateTime = DateTime.Parse(dr["LastUpdateTime"].ToString());
            }
            
            return pj;
        }
    }
}
