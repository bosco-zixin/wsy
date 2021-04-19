using System;
using System.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using WebApi.Areas.HelpPage.ModelDescriptions;
using WebApi.Areas.HelpPage.Models;

namespace WebApi.Areas.HelpPage.Controllers
{
    /// <summary>
    /// The controller that will handle requests for the help page.
    /// </summary>
    public class HelpController : Controller
    {
        private const string ErrorViewName = "Error";
        /// <summary>
        /// 
        /// </summary>
        public HelpController()
            : this(GlobalConfiguration.Configuration)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public HelpController(HttpConfiguration config)
        {
            Configuration = config;
        }
        /// <summary>
        /// 
        /// </summary>
        public HttpConfiguration Configuration { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.Version = String.Format("({0})", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            ViewBag.DocumentationProvider = Configuration.Services.GetDocumentationProvider();
            return View(Configuration.Services.GetApiExplorer().ApiDescriptions);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiId"></param>
        /// <returns></returns>
        public ActionResult Api(string apiId)
        {
            if (!String.IsNullOrEmpty(apiId))
            {
                ViewBag.AccessToken = ConfigurationManager.AppSettings["AccessToken"];
                HelpPageApiModel apiModel = Configuration.GetHelpPageApiModel(apiId);
                if (apiModel != null)
                {
                    return View(apiModel);
                }
            }

            return View(ErrorViewName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public ActionResult ResourceModel(string modelName)
        {
            if (!String.IsNullOrEmpty(modelName))
            {
                ModelDescriptionGenerator modelDescriptionGenerator = Configuration.GetModelDescriptionGenerator();
                ModelDescription modelDescription;
                if (modelDescriptionGenerator.GeneratedModels.TryGetValue(modelName, out modelDescription))
                {
                    return View(modelDescription);
                }
            }

            return View(ErrorViewName);
        }
    }
}