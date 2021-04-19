using log4net;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace WebApi
{
    /// <summary>
    /// 过滤器，用于token验证
    /// </summary>
    public class ActionFilter : ActionFilterAttribute
    {
        // private readonly string Key = "_thisWebApiOnActionMonitorLog_";

        /// <summary>
        /// api执行前处理事件
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            string opcode = string.Empty;
            string controllerName = string.Empty;
            string clientIp = string.Empty;
            string jsonStr = string.Empty;
            ILog log = LogManager.GetLogger(this.GetType());
            if (filterContext.Request.Headers.Contains("opcode"))
            {
                opcode = filterContext.Request.Headers?.GetValues("opcode").FirstOrDefault();
            }
            // var strary = filterContext.Request.RequestUri.AbsolutePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            controllerName = filterContext.Request.RequestUri.AbsolutePath;
            dynamic RemoteEndpoint = null;
            if (filterContext.ActionArguments.Count > 0)
            {
                jsonStr = JsonConvert.SerializeObject(filterContext.ActionArguments);
            }
            bool flag = filterContext.Request.Properties.TryGetValue("System.ServiceModel.Channels.RemoteEndpointMessageProperty", out RemoteEndpoint);
            if (flag)
            {
                clientIp = RemoteEndpoint?.Address;
                if (clientIp == "::1")
                {
                    clientIp = "localhost";
                }
            }
            else
            {
                clientIp = "localhost";
            }
         
                log.Info(string.Format("opcode:{0}.[{1}].[{2}].[{3}]", opcode, controllerName, clientIp, jsonStr));
         
            
            //进行正式token验证  
            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// api执行完处理事件
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuted(HttpActionExecutedContext filterContext)
        {
            string opcode = string.Empty;
            string controllerName = string.Empty;
            string clientIp = string.Empty;
            string jsonStr = string.Empty;
            ILog log = LogManager.GetLogger(this.GetType());

            if (filterContext.Exception != null)
            {
                log.Error(filterContext.Exception);
              
            }
       
            base.OnActionExecuted(filterContext);
        }

    }
}