using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using ZF.Db.SqlHelper;
using ConfigFramework.ConfigManger.Model;

namespace ConfigFramework.ConfigManger.Dal
{
    public class ProjectDal
    {
        /// <summary>
        /// 读取列表
        /// </summary>
        /// <returns></returns>
        public List<Project> GetList(string conn)
        {
            List<Project> list = new List<Project>();
            string sql = "SELECT Id,ProjectName,CategoryIds,Remark,CreateTime FROM Project";
            DataTable dt = SqlServerHelper.Get(conn, sql);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Project project = new Project();
                    project = Project.CreateModel(dt.Rows[i]);
                    list.Add(project);
                }
            }
            return list;
        }

        /// <summary>
        /// 根据name读取一条信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Project GetByName(string conn,string name)
        {
            Project project = new Project();
            string sql = "SELECT Id,ProjectName,CategoryIds,Remark,CreateTime FROM Project WHERE ProjectName=@pname";
            SqlParameter[] paramters = new SqlParameter[] { new SqlParameter("@pname", name) };
            DataTable dt = SqlServerHelper.Get(conn, sql, paramters);
            if (dt.Rows.Count > 0)
            {
                project = Project.CreateModel(dt.Rows[0]);
            }
            return project;
        }
    }
}
