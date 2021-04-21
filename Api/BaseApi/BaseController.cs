using Model;
using System;
using System.IO;
using System.Linq;
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
            string sourcePath = string.Empty;
            ResInfo resInfo = new ResInfo();
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
            HttpRequestBase request = context.Request;         
            string relativePath = "~/UpLoad/" + DateTime.Now.Year.ToString() + "/" + DateTime.Now.Month.ToString();
            string tempFileName = DateTime.Now.ToString("yyyyMMddHHmmssffff");
            string filePath = System.Web.Hosting.HostingEnvironment.MapPath(relativePath);
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            string extension = System.IO.Path.GetExtension(request.Files[0].FileName);
            sourcePath = filePath + tempFileName+ extension;           
            request.Files[0].SaveAs(sourcePath);

            resInfo.ResMsg = relativePath + tempFileName+ extension;
            return resInfo;

        }
    }
}
