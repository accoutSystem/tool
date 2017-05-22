using Fangbian.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MobileValidServer
{
    /// <summary>
    /// 验证平台请求数据包
    /// </summary>
    public class ValidPlatformRequest
    {
        CookieContainer cookie = new CookieContainer();

        public CookieContainer Cookie
        {
            get { return cookie; }
        }

        public string CreateGetRequest(string requestUri)
        {
            Console.WriteLine(requestUri);

            HttpWebResponse response = null;

            var request = (HttpWebRequest)WebRequest.Create(requestUri);

            request.Proxy = WebRequest.GetSystemWebProxy();

            try
            {
                request.KeepAlive = true;

                request.Accept = "text/javascript, text/html, application/xml, text/xml, */*";

                request.Headers.Add("X-Requested-With", @"XMLHttpRequest");

                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "zh_CN");

                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322)";

                request.Method = WebRequestMethods.Http.Get;

                request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip,deflate");

                request.CookieContainer = cookie;

                response = request.GetResponse() as HttpWebResponse;
                return ReadResponse(response);
            }
            catch (WebException e)
            {
                Logger.Fatal("请求"+requestUri+"错误:"+e.Message);
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    response = e.Response as HttpWebResponse;
                    return ReadResponse(response);
                }
                else
                    return string.Empty;
            }
            catch(Exception ex) 
            {
                Logger.Fatal("请求" + requestUri + "错误:" + ex.Message);
                return string.Empty;
            }
            finally
            {
                //ManagerCookie(response, cookieContainer);
            }
        }

        public string ReadResponse(WebResponse resp)
        {
            try
            {
                var response = (HttpWebResponse)resp;

                using (var responseStream = response.GetResponseStream())
                {
                    var streamToRead = responseStream;
                    if (streamToRead != null)
                    {
                        if (response.ContentEncoding.ToLower().Contains("gzip"))
                        {
                            streamToRead = new GZipStream(streamToRead, CompressionMode.Decompress);
                        } 
                        else if (response.ContentEncoding.ToLower().Contains("deflate"))
                        {
                            streamToRead = new DeflateStream(streamToRead, CompressionMode.Decompress);
                        }

                        if (response.CharacterSet.ToLower().Contains("iso") && response.CharacterSet.ToLower().Contains("8859"))
                        {
                            using (var streamReader = new StreamReader(streamToRead, Encoding.GetEncoding("gb2312")))
                            {
                                return streamReader.ReadToEnd();
                            }
                        }
                        else
                        {
                            using (var streamReader = new StreamReader(streamToRead, Encoding.UTF8))
                            {
                                return streamReader.ReadToEnd();
                            }
                        }
                    }

                    return string.Empty;
                }
            }
            catch(Exception ex)
            {
                Logger.Fatal("请求" + resp.ResponseUri.AbsoluteUri + "错误:" + ex.Message);
                return string.Empty;
            }
        }


        /// <summary>
        /// 读取验证码
        /// </summary>
        /// <param name="validCode">验证码</param>
        /// <returns></returns>
        public string ReadValidCode(string validCode)
        {
            //10||2015-08-02 22:34:57: COM 62口(13530740156)从12306接收了一条短信  【铁路客服】验证码：241641，切勿转发。12306网站用户正在申请核验尾号为0156的手机。如非本人操作，请忽略本短信。

            var valid = validCode;

            var index = valid.IndexOf("验证码：");

            valid = valid.Substring(index, valid.Length - index);

            valid = valid.Replace("验证码：", "");

            valid = valid.Substring(0, valid.IndexOf("，"));

            valid = valid.Replace("，", "");

            return valid;
        }
    }
}
