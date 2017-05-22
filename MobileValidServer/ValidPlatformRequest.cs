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
  public  class ValidPlatformRequest
  {
      static CookieContainer cookie = new CookieContainer();

      public static CookieContainer Cookie
      {
          get { return cookie; }
      }

       
        public static string CreateGetRequest(string requestUri )
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
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    response = e.Response as HttpWebResponse;
                    return ReadResponse(response);
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //ManagerCookie(response, cookieContainer);
            }
        }

        public static string ReadResponse(WebResponse resp)
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

                    using (var streamReader = new StreamReader(streamToRead, Encoding.UTF8))
                    {
                        return streamReader.ReadToEnd();
                    }
                }

                return null;
            }
        } 
    }
}
