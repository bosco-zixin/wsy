using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebApi
{
    public class BaseController: ApiController
    {       

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
