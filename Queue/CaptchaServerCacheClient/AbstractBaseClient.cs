#region source code header

// solution:CaptchaServer
// created:2015-04-08
// modify:2015-04-08
// copyright fangbian.com 2015

#endregion

#region

using System;
using System.Diagnostics;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Web;
using Newtonsoft.Json;

#endregion

namespace CaptchaServerCacheClient
{
    public   class AbstractBaseClient :IDisposable
    {
        public string Auth { get; set; }
        protected ClientType ClientType { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        //api/cache.ashx?auth=showmethetask&name=testtask&opt=status
        protected string GetCommand(string opt)
        {
            Url = (Url + "").TrimEnd("/\\".ToCharArray());

            return Url + "/api/" + GetInvoke(ClientType) + "?name=" + Name + "&opt=" + opt + "&auth=" + Auth;
        }

        protected string GetJsonData(string opt)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                var response = TryWebGet(GetCommand(opt), 1);

                if (response == "HTTPSQS_GET_END")
                    return "";
                if (string.IsNullOrEmpty(response))
                    return "";
                if (response.IndexOf("{", StringComparison.Ordinal) != -1 &&
                    response.LastIndexOf("}", StringComparison.Ordinal) != -1)
                {
                    response = response.Substring(response.IndexOf("{", StringComparison.Ordinal));
                    response = response.Substring(0, response.LastIndexOf("}", StringComparison.Ordinal) + 1);
                    //json
                    return response;
                }
                return response;
            }
            finally
            {
                stopwatch.Stop();
                Debug.Write("[GetJsonData]" + GetCommand(opt) + ":" + stopwatch.ElapsedMilliseconds);
            }
        }

        protected string GetStringData(string opt)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                var response = TryWebGet(GetCommand(opt), 1);

                if (response == "HTTPSQS_GET_END")
                    return "";
                if (string.IsNullOrEmpty(response))
                    return "";
                return response;
            }
            finally
            {
                stopwatch.Stop();
                Debug.Write("[GetStringData]" + GetCommand(opt) + ":" + stopwatch.ElapsedMilliseconds);
            }
        }

        protected string GetStringKeyData(string opt, string key)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                var response = TryWebGet(GetCommand(opt) + "&key=" + key, 1);

                if (response == "HTTPSQS_GET_END")
                    return "";
                if (string.IsNullOrEmpty(response))
                    return "";
                return response;
            }
            finally
            {
                stopwatch.Stop();
                Debug.Write("[GetStringKeyData]" + GetCommand(opt) + ":" + stopwatch.ElapsedMilliseconds);
            }
        }

        protected T GetJsonObject<T>(string opt)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                var response = GetJsonData(opt);
                if (response.IndexOf("{", StringComparison.Ordinal) != -1 &&
                    response.LastIndexOf("}", StringComparison.Ordinal) != -1)
                {
                    response = response.Substring(response.IndexOf("{", StringComparison.Ordinal));
                    response = response.Substring(0, response.LastIndexOf("}", StringComparison.Ordinal) + 1);
                    //json
                    return JsonConvert.DeserializeObject<T>(response);
                }
                return default(T);
            }
            finally
            {
                stopwatch.Stop();
                Debug.Write("[GetJsonObject]" + GetCommand(opt) + ":" + stopwatch.ElapsedMilliseconds);
            }
        }

        protected bool PutJsonObject(string opt, object data)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                return
                    TryWebPost(GetCommand(opt), "data=" + HttpUtility.UrlEncode(JsonConvert.SerializeObject(data)),
                        1) == "HTTPSQS_PUT_OK";
            }
            finally
            {
                stopwatch.Stop();
                Debug.Write("[PutJsonObject]" + GetCommand(opt) + ":" + stopwatch.ElapsedMilliseconds);
            }
        }

        protected bool PutStringKeyData(string opt, string key, string data, int? expire)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                var addExpire = "";
                if (expire != null)
                {
                    addExpire = expire.ToString();
                }
                return
                    TryWebPost(GetCommand(opt),
                        "data=" + HttpUtility.UrlEncode(data) + "&key=" + key + "&expire=" + addExpire,
                        1) == "HTTPSQS_PUT_OK";
            }
            finally
            {
                stopwatch.Stop();
                Debug.Write("[PutStringKeyData]" + GetCommand(opt) + ":" + stopwatch.ElapsedMilliseconds);
            }
        }

        protected bool PutStringData(string opt, string data)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                return
                    TryWebPost(GetCommand(opt), "data=" + HttpUtility.UrlEncode(data),
                        1) == "HTTPSQS_PUT_OK";
            }
            finally
            {
                stopwatch.Stop();
                Debug.Write("[PutStringData]" + GetCommand(opt) + ":" + stopwatch.ElapsedMilliseconds);
            }
        }

        private string GetInvoke(ClientType clientType)
        {
            switch (clientType)
            {
                case ClientType.Cache:
                    return "cache.ashx";
                case ClientType.Queue:
                    return "queue.ashx";
                //case ClientType.Stack:
                default:
                    return "stack.ashx";
            }
        }

        #region static http helper

        protected static string WebPost(string url, string data)
        {
            string r = null;
            var web = new WebClient();
            var postData = Encoding.Default.GetBytes(data);
            try
            {
                web.Headers.Add("User-Agent",
                    "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/28.0.1500.95 Safari/537.36 SE 2.X MetaSr 1.0");
                web.Headers.Add("Accept", "*/*");

                web.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                web.Headers.Add("ContentLength", postData.Length.ToString());
                var bytes = web.UploadData(url, "POST", postData);
                r = Encoding.UTF8.GetString(bytes);
                Debug.WriteLine(r, "WebPost");
            }
            catch
            {
            }

            return r;
        }

        public static string TryWebPost(string url, string data)
        {
            return TryWebPost(url, data, 3);
        }

        public static string TryWebPost(string url, string data, int retry)
        {
            string r = null;
            var i = 0;
            do
            {
                ++i;
                if (i > 1)
                    Thread.Sleep(100);
                r = WebPost(url, data);
                Debug.WriteLine(r, "TryWebPost" + i);
            } while (r == null && i < retry);

            return r;
        }

        public static string WebGet(string url)
        {
            string r = null;

            var bytes = GetWebBytes(url);

            r = Encoding.UTF8.GetString(bytes);

            //Debug.WriteLine(r);

            return r;
        }

        public static byte[] GetWebBytes(string url)
        {
            //ServicePointManager.DefaultConnectionLimit = 1000;
            //ServicePointManager.ServerCertificateValidationCallback =
            //    CheckValidationResult;

            var web = new WebClient();
            try
            {
                web.Headers.Add("User-Agent",
                    "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/28.0.1500.95 Safari/537.36 SE 2.X MetaSr 1.0");
                web.Headers.Add("Accept", "*/*");
                //Debug.WriteLine(url);
                var bytes = web.DownloadData(url);
                return bytes;
            }
            catch
            {
                //670 352
            }
            return new byte[] {};
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain,
            SslPolicyErrors sslpolicyerrors)
        {
            return true;
        }

        public static string TryWebGet(string url)
        {
            return TryWebGet(url, 3);
        }

        public static string TryWebGet(string url, int retry)
        {
            string r = null;
            var i = 0;
            do
            {
                ++i;
                if (i > 1)
                    Thread.Sleep(100);
                r = WebGet(url);
               
            } while (r == null && i < retry);

            return r;
        }

        #endregion

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            GC.Collect();
        }
    }
}