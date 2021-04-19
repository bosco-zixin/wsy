using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace WebApi
{
    public class BaseController : ApiController
    {
        protected string connStr = string.Empty;

        public BaseController()
        {
            var header = HttpContext.Current.Request.Headers;
            if (header.AllKeys.Contains("orgId"))
            {
                string orgId = header.GetValues("orgId").FirstOrDefault();
                connStr = BLL.StoreService.GetDBConnStr(orgId);
            }
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        [HttpGet]
        public ResInfo UpLoadFile()
        {
            ResInfo resInfo = new ResInfo();


            return resInfo;
        }
    }
}
