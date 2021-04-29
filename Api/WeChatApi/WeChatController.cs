using Model;
using WebApi;
using System.Web.Http;
using System.Data;

namespace WeChatApi
{
    /// <summary>
    /// 微信控制器
    /// </summary>
    public class WeChatController : BaseController
    {
        ResInfo resInfo = new ResInfo();

        /// <summary>
        /// 添加报修
        /// </summary>
        /// <param name="RepairDto">参数</param>      
        [HttpPost]
        public ResInfo AddRepair(Model.WeChat.RepairAddDto RepairDto)
        {
            BLL.WeChat.Repair bll = new BLL.WeChat.Repair(connStr);
            bll.AddRepair(RepairDto);
            return resInfo;
        }

        /// <summary>
        /// 报修列表
        /// </summary>
        /// <param name="RepairDto">参数</param>      
        [HttpPost]
        public ResInfo QueryRepairList(Model.WeChat.RepairListDto RepairDto)
        {
            int rows = 0;
            BLL.WeChat.Repair bll = new BLL.WeChat.Repair(connStr);
            DataTable dt = bll.QueryRepairList(RepairDto, out rows);
            ResDataDto resDataDto = new ResDataDto();
            resDataDto.Rows = dt;
            resDataDto.Records = rows;
            resInfo.ResData = resDataDto;
            return resInfo;
        }

        /// <summary>
        /// 报修详细
        /// </summary>
        /// <param name="ServiceID">参数</param>      
        [HttpGet]
        public ResInfo GetRepairDetail(string ServiceID)
        {
            BLL.WeChat.Repair bll = new BLL.WeChat.Repair(connStr);
            DataTable dt = bll.GetRepairDetail(ServiceID);
            ResDataDto resDataDto = new ResDataDto();
            resDataDto.Rows = dt;
            resDataDto.Records = dt.Rows.Count;
            resInfo.ResData = resDataDto;
            return resInfo;
        }

        /// <summary>
        /// 录量
        /// </summary>
        /// <param name="MetID">参数</param>      
        /// <param name="Num">参数</param>      
        /// <param name="DedNum">参数</param>      
        [HttpGet]
        public ResInfo InputMeterDosage(long MetID, decimal Num, decimal DedNum)
        {
            BLL.WeChat.Charge bll = new BLL.WeChat.Charge(connStr);
            long iDosageID = bll.InputMeterDosage(MetID, Num, DedNum, 0);
            resInfo.ResCode = 1;
            resInfo.ResData = iDosageID;

            return resInfo;
        }

        /// <summary>
        /// 算费
        /// </summary>
        /// <param name="NodeID">参数</param>      
        /// <param name="MetID">参数</param>      
        /// <param name="DosageID">参数</param>      
        [HttpGet]
        public ResInfo CalcFee(long NodeID, long MetID, long DosageID)
        {
            BLL.WeChat.Charge bll = new BLL.WeChat.Charge(connStr);
            DataTable dt = bll.CalcFee(NodeID, MetID, DosageID);
            resInfo.ResCode = 1;
            resInfo.ResData = dt;

            return resInfo;
        }

        /// <summary>
        /// 缴费
        /// </summary>
        /// <param name="ChargeDto">参数</param>      
        [HttpPost]
        public ResInfo PayCharge(Model.WeChat.Charge ChargeDto)
        {
            string errMsg = "";
            Model.WeChat.Card card = new Model.WeChat.Card();
            BLL.WeChat.Charge bll = new BLL.WeChat.Charge(connStr);
            bool flag = bll.PayCharge(ChargeDto, out errMsg, out card);
            if (flag)
            {
                resInfo.ResCode = 1;
                resInfo.ResMsg = errMsg;
                resInfo.ResData = card;
            }
            else
            {
                resInfo.ResCode = -1;
                resInfo.ResMsg = errMsg;
            }
            return resInfo;
        }
    }
}
