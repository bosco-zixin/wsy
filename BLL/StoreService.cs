using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class StoreService
    {

        /// <summary>
        /// 获取配置库数据库链接信息
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public static string GetDBConnStr(string orgId)
        {
            string connStr = string.Empty;
            DAL.StoreService dal = new DAL.StoreService();
            DataTable dt = new DataTable();
            DataSet ds = dal.GetStoreConnInfo(orgId);
            if (ds != null && ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    connStr = dt.Rows[0][""].ToString();
                }
            }
            return connStr;
        }

        /// <summary>
        /// 获取配置库数据库链接信息
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DataTable GetStoreConnInfo(string orgId)
        {
            DAL.StoreService dal = new DAL.StoreService();
            DataTable dt = new DataTable();
            DataSet ds = dal.GetStoreConnInfo(orgId);
            if (ds != null && ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
            }
            return dt;
        }

        /// <summary>
        /// 获取配置库数据库链接信息
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DataTable GetStoreConnInfo()
        {
            DataTable dt = new DataTable();
            DAL.StoreService dal = new DAL.StoreService();
            DataSet ds = dal.GetStoreConnInfo();
            if (ds != null && ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
            }
            return dt;
        }

        /// <summary>
        /// 获取配置库数据库链接信息
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DataTable GetMethodName(string paraId, string custId)
        {
            DataTable dt = new DataTable();
            DAL.StoreService dal = new DAL.StoreService();
            DataSet ds = dal.GetMethodName(paraId, custId);
            if (ds != null && ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
            }
            return dt;
        }

    }
}
