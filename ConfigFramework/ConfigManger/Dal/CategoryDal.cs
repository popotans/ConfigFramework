using ConfigFramework.ConfigManger.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using ZF.Db.SqlHelper;

namespace ConfigFramework.ConfigManger.Dal
{
    public class CategoryDal
    {
        /// <summary>
        /// 读取列表
        /// </summary>
        /// <returns></returns>
        public List<Category> GetList()
        {
            string conn = ConfigMangerHelper.Get<string>("ConfigManager");
            List<Category> list = new List<Category>();
            string sql = "SELECT Id,CategoryName,Remark,CreateTime FROM Category";
            DataTable dt = SqlServerHelper.Get(conn, sql);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Category category = new Category();
                    category = Category.CreateModel(dt.Rows[i]);
                    list.Add(category);
                }
            }
            return list;
        }

        /// <summary>
        /// 根据Id读取一条记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Category GetById(long id)
        {
            string conn = ConfigMangerHelper.Get<string>("ConfigManager");
            Category category = new Category();
            string sql = "SELECT Id,CategoryName,Remark,CreateTime FROM Category WHERE Id=@cid";
            SqlParameter[] paramters = new SqlParameter[] { new SqlParameter("@cid", id) };
            DataTable dt = SqlServerHelper.Get(conn, sql, paramters);
            if (dt.Rows.Count > 0)
            {
                category = Category.CreateModel(dt.Rows[0]);
            }
            return category;
        }

        /// <summary>
        /// 获得cids的分类列表
        /// </summary>
        /// <param name="cids"></param>
        /// <returns></returns>
        public List<Category> GetListByIds(string cids)
        {
            string conn = ConfigMangerHelper.Get<string>("ConfigManager");
            List<Category> list = new List<Category>();
            string sqlwhere = "";
            string[] idsstr = cids.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            SqlParameter[] paramters = new SqlParameter[idsstr.Length];
            for (int i = 0; i < idsstr.Length; i++)
            {
                if (string.IsNullOrEmpty(sqlwhere))
                {
                    sqlwhere += "Id=@cid" + i;
                }
                else
                {
                    sqlwhere += "OR Id=@cid" + i;
                }
                paramters[i] = new SqlParameter("@cid" + i, idsstr[i]);
            }
            string sql = "SELECT Id,CategoryName,Remark,CreateTime FROM Category ";
            if (!string.IsNullOrEmpty(sqlwhere))
            {
                sql += "WHERE " + sqlwhere;
            }
            DataTable dt = SqlServerHelper.Get(conn, sql, paramters);
            if (dt.Rows.Count > 0)
            {
                for (int m = 0; m < dt.Rows.Count; m++)
                {
                    Category cate = new Category();
                    cate = Category.CreateModel(dt.Rows[m]);
                    list.Add(cate);
                }
            }
            return list;
        }
    }
}
