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
        BLL.WeChat.Repair bll = null;

        /// <summary>
        /// 
        /// </summary>
        public WeChatController()
        {
            bll = new BLL.WeChat.Repair(connStr);
        }

        /// <summary>
        /// 添加报修
        /// </summary>
        /// <param name="RepairDto">参数</param>      
        [HttpPost]
        public ResInfo AddRepair(Model.WeChat.RepairAddDto RepairDto)
        {
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
            DataTable dt = bll.QueryRepairList(RepairDto,out rows);
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
            DataTable dt = bll.GetRepairDetail(ServiceID);
            ResDataDto resDataDto = new ResDataDto();
            resDataDto.Rows = dt;
            resDataDto.Records = dt.Rows.Count;
            resInfo.ResData = resDataDto;
            return resInfo;
        }


    }
}
