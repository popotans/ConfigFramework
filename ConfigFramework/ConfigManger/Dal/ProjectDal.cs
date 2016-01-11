using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigFramework.ConfigManger;
using ConfigFramework.ConfigManger.Model;
using System.Data;
using System.Data.SqlClient;

namespace ConfigFramework.ConfigManger.Dal
{
    public class ProjectDal
    {
        public static List<project> GetAll()
        {
            List<project> list = new List<project>();
            string sqlstr = "select * from project";
            DataTable dt = SqlHelper.get(sqlstr);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    project pj = project.CreateModel(dt.Rows[i]);
                    list.Add(pj);
                }
            }
            return list;
        }
    }

    public class SqlHelper
    {
        public static readonly string connstr = ConfigMangerHelper.Get<string>("ProjectName");

        public static DataTable get(string sqlstr)
        {
            SqlConnection conn = new SqlConnection(connstr);
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = sqlstr;
                DataTable dt = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                return dt;
            }
        }
    }
}
