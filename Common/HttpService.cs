using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

namespace WSY.Common
{
    /// <summary>
    /// http连接基础类，负责底层的http通信
    /// </summary>
    public class HttpService
    {
        /// <summary>
        /// 处理http POST请求
        /// </summary>
        /// <param name="xml">请求XML文件内容</param>
        /// <param name="url">请求地址</param>
        /// <param name="isUseCert">是使用证书</param>
        /// <param name="timeout">等待超时时间</param>
        ///<param name="Sslcert_Path">证书路经</param>
        /// <param name="Sslcert_Password">证书密码</param>
        /// <returns></returns>
        public static string Post(HttpPostPara httpPosModel)
        {
            GC.Collect();//垃圾回收，回收没有正常关闭的http连接
            string result = "";//返回结果
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            Stream reqStream = null;
            try
            {
                if (httpPosModel == null)
                {
                    throw new Exception("请求对象不能为空");
                }
                //设置最大连接数
                ///没有设置或者小于零时默认是200
                if (httpPosModel.MaxConnCount <= 0)
                {
                    httpPosModel.MaxConnCount = 200;
                }
                ServicePointManager.DefaultConnectionLimit = httpPosModel.MaxConnCount;
                //设置https验证方式
                if (httpPosModel.Url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                }
                /***************************************************************
                * 下面设置HttpWebRequest的相关属性
                * ************************************************************/
                request = (HttpWebRequest)WebRequest.Create(httpPosModel.Url);

                request.Method = "POST";
                if (httpPosModel.Timeout < 1)
                {
                    httpPosModel.Timeout = 6;
                }
                request.Timeout = httpPosModel.Timeout * 1000;

                //设置代理服务器
                //WebProxy proxy = new WebProxy();                          //定义一个网关对象
                //proxy.Address = new Uri(WxPayConfig.PROXY_URL);              //网关服务器端口:端口
                //request.Proxy = proxy;

                //设置POST的数据类型和长度
                ///没有设置时默认是text/xml
                if (string.IsNullOrEmpty(httpPosModel.ContentType))
                {
                    httpPosModel.ContentType = "text/xml";
                }
                request.ContentType = httpPosModel.ContentType;
                byte[] data = Encoding.UTF8.GetBytes(httpPosModel.Content);
                request.ContentLength = data.Length;

                //是否使用证书
                if (httpPosModel.IsUseCert)
                {
                    string path = HttpContext.Current.Request.PhysicalApplicationPath;
                    X509Certificate2 cert = new X509Certificate2(path + httpPosModel.Sslcert_Path, httpPosModel.Sslcert_Password);
                    request.ClientCertificates.Add(cert);
                }

                //往服务器写入数据
                reqStream = request.GetRequestStream();
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();

                //获取服务端返回
                response = (HttpWebResponse)request.GetResponse();
                //获取服务端返回数据
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                result = sr.ReadToEnd().Trim();
                sr.Close();
            }
            catch (System.Threading.ThreadAbortException e)
            {
                System.Threading.Thread.ResetAbort();
                throw e;
            }
            catch (WebException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                //关闭连接和流
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return result;
        }

        /// <summary>
        /// 处理http POST请求
        /// </summary>
        /// <param name="xml">请求XML文件内容</param>
        /// <param name="url">请求地址</param>
        /// <param name="isUseCert">是使用证书</param>
        /// <param name="timeout">等待超时时间</param>
        ///<param name="Sslcert_Path">证书路经</param>
        /// <param name="Sslcert_Password">证书密码</param>
        /// <param name="heads">请求头</param>
        /// <returns></returns>
        public static string HttpPost(HttpPostPara httpPosModel, Dictionary<string, string> heads)
        {
            GC.Collect();//垃圾回收，回收没有正常关闭的http连接
            string result = "";//返回结果
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            Stream reqStream = null;
            try
            {
                if (httpPosModel == null)
                {
                    throw new Exception("请求对象不能为空");
                }
                //设置最大连接数
                ServicePointManager.DefaultConnectionLimit = httpPosModel.MaxConnCount;
                //设置https验证方式
                if (httpPosModel.Url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                }
                /***************************************************************
                * 下面设置HttpWebRequest的相关属性
                * ************************************************************/
                request = (HttpWebRequest)WebRequest.Create(httpPosModel.Url);
                request.Method = "POST";
                request.Timeout = httpPosModel.Timeout * 1000;
                foreach (var item in heads)
                {
                    request.Headers.Add(item.Key, item.Value);
                }
                //设置代理服务器
                //WebProxy proxy = new WebProxy();                          //定义一个网关对象
                //proxy.Address = new Uri(WxPayConfig.PROXY_URL);              //网关服务器端口:端口
                //request.Proxy = proxy;

                //设置POST的数据类型和长度
                request.ContentType = httpPosModel.ContentType;
                byte[] data = Encoding.UTF8.GetBytes(httpPosModel.Content);
                request.ContentLength = data.Length;

                //是否使用证书
                if (httpPosModel.IsUseCert)
                {
                    string path = HttpContext.Current.Request.PhysicalApplicationPath;
                    X509Certificate2 cert = new X509Certificate2(path + httpPosModel.Sslcert_Path, httpPosModel.Sslcert_Password);
                    request.ClientCertificates.Add(cert);
                }

                //往服务器写入数据
                reqStream = request.GetRequestStream();
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();

                //获取服务端返回
                response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //获取服务端返回数据
                    StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    result = sr.ReadToEnd().Trim();
                    sr.Close();
                }
            }
            catch (System.Threading.ThreadAbortException e)
            {
                System.Threading.Thread.ResetAbort();
                throw e;
            }
            catch (WebException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                //关闭连接和流
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return result;
        }

        /// <summary>
        /// 处理http GET请求，返回数据
        /// </summary>
        /// <param name="url">请求的url地址</param>
        /// <returns>http GET成功后返回的数据，失败抛WebException异常</returns>
        public static string Get(string url)
        {
            GC.Collect();
            string result = "";
            HttpWebRequest request = null;
            HttpWebResponse response = null;

            //请求url以获取数据
            try
            {
                //设置最大连接数
                ServicePointManager.DefaultConnectionLimit = 200;
                //设置https验证方式
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                }

                /***************************************************************
                * 下面设置HttpWebRequest的相关属性
                * ************************************************************/
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";

                //设置代理
                //WebProxy proxy = new WebProxy();
                //proxy.Address = new Uri(WxPayConfig.PROXY_URL);
                //request.Proxy = proxy;

                //获取服务器返回
                response = (HttpWebResponse)request.GetResponse();

                //获取HTTP返回数据
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                result = sr.ReadToEnd().Trim();
                sr.Close();
            }
            catch (System.Threading.ThreadAbortException e)
            {
                System.Threading.Thread.ResetAbort();
                throw e;
            }
            catch (WebException e)
            {
                throw new Exception(e.ToString());
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            finally
            {
                //关闭连接和流
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return result;
        }

        /// <summary>
        /// 处理http GET请求，返回数据
        /// </summary>
        /// <param name="url">请求的url地址</param>
        /// <returns>http GET成功后返回的数据，失败抛WebException异常</returns>
        public static string HttpGet(string url, Dictionary<string, string> heads)
        {
            GC.Collect();
            string result = "";
            HttpWebRequest request = null;
            HttpWebResponse response = null;

            //请求url以获取数据
            try
            {
                //设置最大连接数
                ServicePointManager.DefaultConnectionLimit = 200;
                //设置https验证方式
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                }

                /***************************************************************
                * 下面设置HttpWebRequest的相关属性
                * ************************************************************/
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                foreach (var head in heads)
                {
                    request.Headers.Add(head.Key, head.Value);
                }
                //设置代理
                //WebProxy proxy = new WebProxy();
                //proxy.Address = new Uri(WxPayConfig.PROXY_URL);
                //request.Proxy = proxy;

                //获取服务器返回
                response = (HttpWebResponse)request.GetResponse();
                //获取HTTP返回数据
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                result = sr.ReadToEnd().Trim();
                sr.Close();
            }
            catch (System.Threading.ThreadAbortException e)
            {
                System.Threading.Thread.ResetAbort();
                throw e;
            }
            catch (WebException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                //关闭连接和流
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string HttpClientAsyncGet(string url)
        {
            string respStr = string.Empty;
            Uri address = new Uri(url);

            using (var httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync(address).Result;
                if (response.IsSuccessStatusCode)
                {
                    respStr = response.Content.ReadAsStringAsync().Result;
                }
            }
            return respStr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string HttpClientAsyncPost(string url, HttpContent httpContent)
        {
            string respStr = string.Empty;
            Uri address = new Uri(url);

            using (var httpClient = new HttpClient())
            {
                var response = httpClient.PostAsync(address, httpContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    respStr = response.Content.ReadAsStringAsync().Result;
                }
            }
            return respStr;
        }

        /// <summary>
        /// post方法
        /// </summary>
        /// <param name="p_url"></param>
        /// <param name="p_data"></param>
        /// <returns></returns>
        public static string WebPost(string p_url, string p_data, string token = "")
        {
            try
            {
                WebClient client = new WebClient();
                client.Encoding = Encoding.UTF8;
                byte[] bytearray = Encoding.UTF8.GetBytes(p_data);
                client.Headers.Add("Content-Type", "application/json");
                client.Headers.Add("ContentLength", bytearray.Length.ToString());
                client.Headers.Add("Authorization", "bearer " + token);
                byte[] Ret_Data = client.UploadData(p_url, "POST", bytearray);
                return Encoding.GetEncoding("UTF-8").GetString(Ret_Data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            //直接确认，否则打不开    
            return true;
        }
    }
    /// <summary>
    /// Http的Post请求参数对象
    /// </summary>
    public class HttpPostPara
    {
        /// <summary>
        /// 请求的XML文件内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 请求地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 请求是否使用证书
        /// </summary>
        public bool IsUseCert { get; set; } = false;
        /// <summary>
        /// 等待超时时间（秒）
        /// </summary>
        public int Timeout { get; set; } = 15;
        /// <summary>
        /// 证书路经
        /// </summary>
        public string Sslcert_Path { get; set; }
        /// <summary>
        /// 证书密码
        /// </summary>
        public string Sslcert_Password { get; set; }
        /// <summary>
        /// 最大连接数
        /// </summary>
        public int MaxConnCount { get; set; } = 200;
        /// <summary>
        /// POST的数据类型
        /// </summary>
        public string ContentType { get; set; } = "application/json";
    }
}
