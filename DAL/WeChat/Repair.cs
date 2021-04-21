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
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into hou_service_info(");
            strSql.Append("AreaID,NodeID,ClassID,ServiceSer,ContactPerson,ContactTel,Address,TodayDate,ServiceNote,Picture1,Picture2,Picture3,Memo,Sign,ISFeedback");
            strSql.Append(") values (");
            strSql.Append("@AreaID,@NodeID,@ClassID,dbo.F_GetClassSer(1),@ContactPerson,@ContactTel,@Address,getdate(),@ServiceNote,@Picture1,@Picture2,@Picture3,@Memo,0,@ISFeedback");
            strSql.Append(") ");

            SqlParameter[] parameters = {
                        new SqlParameter("@AreaID", SqlDbType.VarChar,10) ,
                        new SqlParameter("@NodeID", SqlDbType.VarChar,10) ,
                        new SqlParameter("@ClassID", SqlDbType.Int,4) ,
                        new SqlParameter("@ContactPerson", SqlDbType.VarChar,100) ,
                        new SqlParameter("@ContactTel", SqlDbType.VarChar,50) ,
                        new SqlParameter("@Address", SqlDbType.VarChar,200) ,
                        new SqlParameter("@ServiceNote", SqlDbType.VarChar,3000) ,
                        new SqlParameter("@Picture1", SqlDbType.VarChar,300) ,
                        new SqlParameter("@Picture2", SqlDbType.VarChar,300) ,
                        new SqlParameter("@Picture3", SqlDbType.VarChar,300) ,
                        new SqlParameter("@Memo", SqlDbType.VarChar,100) ,
                        new SqlParameter("@ISFeedback", SqlDbType.SmallInt,2)
            };

            parameters[0].Value = repairDto.AreaID;
            parameters[1].Value = repairDto.NodeID;
            parameters[2].Value = 1;
            parameters[3].Value = repairDto.ContactPerson;
            parameters[4].Value = repairDto.ContactTel;
            parameters[5].Value = repairDto.Address;
            parameters[6].Value = repairDto.ServiceNote;
            parameters[7].Value = repairDto.Picture1;
            parameters[8].Value = repairDto.Picture2;
            parameters[9].Value = repairDto.Picture3;
            parameters[10].Value = repairDto.Memo;
            parameters[11].Value = repairDto.ISFeedback;

            int rows = sqlHelper.ExecuteSql(strSql.ToString(), parameters);
            if (rows > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="doCount"></param>
        public DataSet QueryRepairList(Model.WeChat.RepairListDto dto, out int doCount)
        {
            doCount = 0;
            string strSql = string.Format(@"select a.ServiceSer,b.ClassName,a.ContactPerson,a.ContactTel,a.Address,a.ServiceNote,a.Picture1,a.Picture2,a.Picture3,a.Memo,
case a.Sign when -1 then '禁用' when 0 then '未受理' when 1 then '已派工' when 3 then '已关闭' end as SignName,a.ServiceID
,a.TodayDate,
case a.ISFeedback when 1 then '电话反馈' else '不反馈' end as ISFeedbackName,a.ServiceFee
from hou_service_info a
inner join sys_class_para b on b.ClassID=a.ClassID and a.AreaID=b.AreaID
where a.AreaID='{0}' ", dto.AreaID);

            if (!string.IsNullOrEmpty(dto.ServiceSer))
            {
                strSql += " and a.ServiceSer Like '%" + dto.ServiceSer + "%'";
            }
            if (!string.IsNullOrEmpty(dto.CreateTimeS))
            {
                strSql += " and a.TodayDate >= '" + dto.CreateTimeS + "'";
            }
            if (!string.IsNullOrEmpty(dto.CreateTimeE))
            {
                strSql += " and a.TodayDate <= '" + dto.CreateTimeE + "'";
            }
            if (!string.IsNullOrEmpty(dto.ServiceNote))
            {
                strSql += " and a.ServiceNote like '" + dto.ServiceNote + "%' ";
            }
            if (dto.Sign != null)
            {
                strSql += " and a.Sign='" + dto.Sign + "' ";
            }

            SqlParameter[] sqlParameter = {
                         new SqlParameter("@sSQL", SqlDbType.VarChar, 2000),
                         new SqlParameter("@PageSize", SqlDbType.Int),
                         new SqlParameter("@PageIndex", SqlDbType.Int),
                         new SqlParameter("@doCount", SqlDbType.Int),
                         new SqlParameter("@sOrder", SqlDbType.VarChar, 300),
                    };
            sqlParameter[0].Value = strSql;
            sqlParameter[1].Value = dto.PageSize;
            sqlParameter[2].Value = dto.PageIndex;
            sqlParameter[3].Direction = ParameterDirection.Output;
            sqlParameter[4].Value = "a.ServiceID";

            DataSet ds = sqlHelper.RunProcedure("P_SqlPagerAll", sqlParameter, "QueryHouServiceInfo");
            if (!string.IsNullOrEmpty(sqlParameter[3].Value.ToString()))
            {
                doCount = Convert.ToInt32(sqlParameter[3].Value.ToString());
            }

            return ds;
        }

        public DataSet GetRepairDetail(string serviceId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ProcessID,ServiceID,ProcessOrd,UserName,UserTel,Memo,Convert(Varchar(19),TodayDate,120) TodayDate,OPCode,Sign ");
            strSql.Append(" ,case sign when 1 then '派工' else '关闭' end SignName ,ServiceFee");
            strSql.Append("  from hou_service_process ");
            strSql.Append(" where serviceID=@serviceID");
            SqlParameter[] parameters = {
                    new SqlParameter("@serviceID", SqlDbType.BigInt)
            };
            parameters[0].Value = serviceId;

            return sqlHelper.Query(strSql.ToString(), parameters);
        }
    }
}
