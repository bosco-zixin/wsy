using Newtonsoft.Json;
using System;
using Flag.Model;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Flag.Common;
using System.Web.Mvc;

namespace WebApi
{
    /// <summary>
    /// 
    /// </summary>
    public class TokenController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="opCode"></param>
        /// <returns></returns>
        //[Obsolete("此方法暂时不启用")]

        //public HttpResponseMessage GetToken(string opCode)
        //{
        //    ResInfo resultMsg = null;

        //    //判断参数是否合法
        //    if (string.IsNullOrEmpty(opCode))
        //    {
        //        resultMsg = new ResInfo();
        //        resultMsg.ResCode = -1;
        //        resultMsg.ResMsg = "参数不合法";
        //        return Utils.JsonResponse(JsonConvert.SerializeObject(resultMsg));
        //    }

        //    //插入缓存
        //    //TokenModel token = (TokenModel)HttpRuntime.Cache.Get(opCode);
        //    //if (HttpRuntime.Cache.Get(opCode) == null)
        //    //{
        //    //    token = new TokenModel();
        //    //    token.StaffId = opCode;
        //    //    token.SignToken = Guid.NewGuid();
        //    //    token.ExpireTime = DateTime.Now.AddDays(1);
        //    //    HttpRuntime.Cache.Insert(token.StaffId, token, null, token.ExpireTime, TimeSpan.Zero);
        //    //}

        //    //返回token信息
        //    resultMsg = new ResInfo();
        //    resultMsg.ResCode = 1;
        //    resultMsg.ResMsg = "";

        //    return Utils.JsonResponse(JsonConvert.SerializeObject(resultMsg));
        //}

        public HttpResponseMessage GetTimeStamp(string opCode)
        {
            ResInfo resultMsg = new ResInfo();

            //判断参数是否合法
            if (string.IsNullOrEmpty(opCode))
            {
                resultMsg = new ResInfo();
                resultMsg.ResCode = -1;
                resultMsg.ResMsg = "参数不合法";
                return Utils.JsonResponse(JsonConvert.SerializeObject(resultMsg));
            }

            resultMsg.ResCode = 1;
            resultMsg.ResMsg = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds;

            return Utils.JsonResponse(JsonConvert.SerializeObject(resultMsg));
        }

        public HttpResponseMessage GetSignature(string opCode)
        {
            ResInfo resultMsg = new ResInfo();

            //判断参数是否合法
            if (string.IsNullOrEmpty(opCode))
            {
                resultMsg = new ResInfo();
                resultMsg.ResCode = -1;
                resultMsg.ResMsg = "参数不合法";
                return Utils.JsonResponse(JsonConvert.SerializeObject(resultMsg));
            }

            resultMsg.ResCode = 1;
            resultMsg.ResMsg = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds;

            return Utils.JsonResponse(JsonConvert.SerializeObject(resultMsg));
        }
    }
}