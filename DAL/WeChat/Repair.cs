using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSY.SQLTools;

namespace DAL.WeChat
{
    public class Repair
    {
        SQLHelper sqlHelper = null;
        string connStr = string.Empty;
        public Repair(string _connStr)
        {
            connStr = _connStr;
            sqlHelper = new SQLHelper(connStr);
        }
        /// <summary>
        /// 添加报修
        /// </summary>
        /// <param name="metId"></param>
        /// <returns></returns>
        public bool AddRepair(Model.WeChat.RepairAddDto repairDto)
        {
            string strSql = "select Sign from dat_fee where MetID=@MetID AND Sign<>5 order by FeeSer desc ";
            SqlParameter[] parameters = {
                    new SqlParameter("@MetID", SqlDbType.VarChar,30)
            };


            int rows = sqlHelper.ExecuteSql(strSql, parameters);

            return rows > 0;
        }

        public DataSet QueryRepairList(Model.WeChat.RepairListDto repairDto)
        {
            string strSql = "";
            DataSet ds = sqlHelper.Query(strSql);
            return ds;
        }

        public DataSet GetRepairDetail(string serviceId)
        {
            string strSql = "";
            DataSet ds = sqlHelper.Query(strSql);
            return ds;
        }
    }
}
