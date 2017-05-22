using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Tuniu.RegisterUser
{
    public class WebHttpsRequest
    {
        public string DefaultHost = "kyfw.12306.cn";

        private bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate,
       X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public string CreateRequest(string httpType, string requestUri, [Optional] NameValueCollection queryCollection, [Optional] NameValueCollection bodyCollection, [Optional] NameValueCollection addHeader, out HttpWebResponse response)
        {
            return CreateRequest(httpType, requestUri, null, queryCollection, bodyCollection, addHeader, out response);
        }


        public string CreateRequest(string httpType, string requestUri, CookieCollection cookies, [Optional] NameValueCollection queryCollection, [Optional] NameValueCollection bodyCollection, [Optional] NameValueCollection addHeader, out HttpWebResponse response)
        {
            if (queryCollection != null && queryCollection.Count > 0)
            {
                StringBuilder querys = new StringBuilder("?");

                foreach (string key in queryCollection.Keys)
                {
                    querys.Append(string.Format("{0}={1}&", key, queryCollection.Get(key)));
                }

                querys.Remove(querys.Length - 1, 1);

                requestUri += querys.ToString();
            }

            Console.WriteLine(requestUri);

            var request = (HttpWebRequest)System.Net.WebRequest.Create(requestUri);
            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            request.Proxy = System.Net.WebRequest.GetSystemWebProxy();

            request.Host = DefaultHost;

            ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidationCallback;

            try
            {
                request.KeepAlive = true;

                if (addHeader != null)
                {
                    foreach (var key in addHeader.AllKeys)
                    {
                        if (key.Equals("Content-Type"))
                        {
                            request.ContentType = addHeader[key];
                        }
                        else if (key.Equals("Accept"))
                        {
                            request.Accept = addHeader[key];
                        }
                        else if (key.Equals("Referer"))
                        {
                            request.Referer = addHeader[key];
                        }
                        else if (key.Equals("User-Agent"))
                        {
                            request.UserAgent = addHeader[key];
                        }
                        else
                        {
                            request.Headers.Add(key, addHeader[key]);
                        }
                    }
                }

                request.Method = httpType;


                if (httpType.Equals(System.Net.WebRequestMethods.Http.Post))
                {
                    StringBuilder postData = new StringBuilder("");

                    if (bodyCollection != null && bodyCollection.Count > 0)
                    {
                        foreach (string key in bodyCollection.Keys)
                        {
                            postData.Append(string.Format("{0}={1}&", key, bodyCollection.Get(key)));
                        }

                        postData.Remove(postData.Length - 1, 1);
                    }

                    var postBytes = Encoding.UTF8.GetBytes(postData.ToString());

                    request.ContentLength = postBytes.Length;

                    var stream = request.GetRequestStream();

                    stream.Write(postBytes, 0, postBytes.Length);

                    stream.Close();
                }

                response = request.GetResponse() as HttpWebResponse;

                return ReadResponse(response);
            }
            catch (WebException e)
            {
                response = null;
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
                response = null;
                return null;
            }
            finally
            {
                //ManagerCookie(response, cookieContainer);
            }
        }

        private string ReadResponse(WebResponse resp)
        {
            var response = (HttpWebResponse)resp;
            using (var responseStream = response.GetResponseStream())
            {
                var streamToRead = responseStream;
                if (streamToRead != null)
                {
                    if (response.ContentType.ToLower().Contains("png") ||
                       response.ContentType.ToLower().Contains("jpg") ||
                        response.ContentType.ToLower().Contains("image"))
                    {
                        var ms = new MemoryStream();

                        int k = 1024;

                        var buff = new byte[k];

                        while (k > 0)
                        {
                            k = streamToRead.Read(buff, 0, 1024);

                            ms.Write(buff, 0, k);
                        }
                        ms.Flush();

                        string strbaser64 = Convert.ToBase64String(ms.ToArray());

                        return strbaser64;
                    }
                    else
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
                }

                return null;
            }
        }
    }



}
