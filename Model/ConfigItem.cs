using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ConfigItem
    {
        /// <summary>
        /// 中间库连接字符串
        /// </summary>
        public static string StoreConnString = System.Configuration.ConfigurationManager.AppSettings["StoreConnString"];
        /// <summary>
        /// 三方库连接字符串
        /// </summary>
        public static string TrdConneString = System.Configuration.ConfigurationManager.AppSettings["TrdConneString"];
        /// <summary>
        /// 微信数据库链接字符串
        /// </summary>
        public static string WeChatMainString = System.Configuration.ConfigurationManager.AppSettings["WeChatMainString"];
        /// <summary>
        /// 集抄系统接口地址
        /// </summary>
        public static string ReadingUrl = System.Configuration.ConfigurationManager.AppSettings["ReadingPlatformUrl"].ToString();
        /// <summary>
        /// 文件服务器地址
        /// </summary>
        public static string FileUrl = System.Configuration.ConfigurationManager.AppSettings["FileServiceAddr"].ToString();
        /// <summary>
        /// Base平台
        /// </summary>
        private static string BaseUrl = System.Configuration.ConfigurationManager.AppSettings["BaseAddr"].ToString();
        /// <summary>
        /// 三方接口地址
        /// </summary>
        private static string TrdUrl = System.Configuration.ConfigurationManager.AppSettings["TrdUrl"].ToString();

        /// <summary>
        /// 
        /// </summary>
        public static string ResponerMethod = BaseUrl + "/base/nologin/org/users";
        /// <summary>
        /// 
        /// </summary>
        public static string DepatrMethod = BaseUrl + "/base/department/org/departments";
    }
}
