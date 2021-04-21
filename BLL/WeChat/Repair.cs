using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.WeChat
{
    public class Repair
    {
        DAL.WeChat.Repair dal = null;
        string connStr = string.Empty;

        public Repair(string _connStr)
        {
            connStr = _connStr;
            dal = new DAL.WeChat.Repair(connStr);           
        }
        /// <summary>
        /// 添加报修
        /// </summary>
        /// <param name="metId"></param>
        /// <returns></returns>
        public bool AddRepair(Model.WeChat.RepairAddDto repairDto)
        {
            return dal.AddRepair(repairDto);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repairDto"></param>
        /// <param name="doCount"></param>
        public DataTable QueryRepairList(Model.WeChat.RepairListDto repairDto, out int doCount)
        {
            DataTable dt = new DataTable();
            DataSet ds = dal.QueryRepairList(repairDto, out doCount);
            if (ds != null && ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
            }
            return dt;
        }

        public DataTable GetRepairDetail(string serviceId)
        {
            DataTable dt = new DataTable();
            DataSet ds = dal.GetRepairDetail(serviceId);
            if (ds != null && ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
            }
            return dt;
        }
    }
}
