using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using WebApi;

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
          
       
            return resInfo;
        }


        /// <summary>
        /// 报修列表
        /// </summary>
        /// <param name="RepairDto">参数</param>      
        [HttpPost]
        public ResInfo QueryRepairList(Model.WeChat.RepairListDto RepairDto)
        {
         
         

            return resInfo;
        }

        /// <summary>
        /// 报修详细
        /// </summary>
        /// <param name="ServiceID">参数</param>      
        [HttpGet]
        public ResInfo GetRepairDetail(string ServiceID)
        {
        
       

            return resInfo;
        }
        

    }
}
