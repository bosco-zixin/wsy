using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace WeChatApi
{
    /// <summary>
    /// 微信控制器
    /// </summary>
    public class WeChatController : ApiController
    {
        /// <summary>
        /// 添加报修
        /// </summary>
        /// <param name="RepairDto">参数</param>      
        [HttpPost]
        public ResInfo AddRepair(Model.WeChat.RepairAddDto RepairDto)
        {
            ResInfo resInfo = new ResInfo();
    

            return resInfo;
        }


        /// <summary>
        /// 报修列表
        /// </summary>
        /// <param name="RepairDto">参数</param>      
        [HttpPost]
        public ResInfo QueryRepairList(Model.WeChat.RepairListDto RepairDto)
        {
            ResInfo resInfo = new ResInfo();


            return resInfo;
        }

        /// <summary>
        /// 报修详细
        /// </summary>
        /// <param name="ServiceID">参数</param>      
        [HttpGet]
        public ResInfo GetRepairDetail(string ServiceID)
        {
            ResInfo resInfo = new ResInfo();


            return resInfo;
        }
        

    }
}
