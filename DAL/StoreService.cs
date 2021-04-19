using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSY.SQLTools;

namespace DAL
{
    public class StoreService
    {
        /// <summary>
        /// 获取配置库数据库链接信息
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DataSet GetStoreConnInfo(string orgid)
        {
            SQLHelper SQLHelper = new SQLHelper(Model.ConfigItem.StoreConnString);
            string strSql= "select top 1 * FROM DATA_CONN where OrgID='" + orgid + "'";
            return SQLHelper.Query(strSql.ToString());
        }
        /// <summary>
        /// 获取配置库数据库链接信息
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DataSet GetStoreConnInfo()
        {
            SQLHelper SQLHelper = new SQLHelper(Model.ConfigItem.StoreConnString);
            string strSql="select OrgName AS AreaName,OrgID  FROM DATA_CONN where BluetoothAPP=1";
            return SQLHelper.Query(strSql);
        }

  
        public DataSet GetMethodName(string paraId, string custId)
        {
            SQLHelper SQLHelper = new SQLHelper(Model.ConfigItem.TrdConneString);
            List<SqlParameter> paraList = new List<SqlParameter>();
            string strSql = "SELECT CustID,ParaID,Para1,Para2,Para3,Memo,InterfaceDir,MethodName FROM Cust_Para_Info where Sign=1";
            if (!string.IsNullOrEmpty(custId))
            {
                strSql += " and CustID=@CustID";
                paraList.Add(new SqlParameter("@CustID", SqlDbType.VarChar,50));
                paraList[paraList.Count - 1].Value = custId;
            }
            if (!string.IsNullOrEmpty(paraId))
            {
                strSql += " and ParaID=@ParaID ";
                paraList.Add(new SqlParameter("@ParaID", SqlDbType.VarChar, 50));
                paraList[paraList.Count - 1].Value = paraId;
            }
            SqlParameter[] parameters = paraList.ToArray();

            return SQLHelper.Query(strSql, parameters);
        }

    }
}
