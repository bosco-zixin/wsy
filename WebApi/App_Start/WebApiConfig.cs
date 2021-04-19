using System.Web.Http;
using System.Web.Http.Cors;

namespace WebApi
{
    /// <summary>
    /// 
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// 全局注册
        /// </summary>
        /// <param name="config"></param>
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务
            //跨域配置
            config.EnableCors(new EnableCorsAttribute("*", "*", "*"));
            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
  
    
            //注册全局Token验证
            config.Filters.Add(new ActionFilter());
            //注册全局Log4日志功能
            log4net.Config.XmlConfigurator.Configure();
            //
           // InitAPI.Init(config);
        }
    }       
}
