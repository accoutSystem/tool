#region source code header

// solution:CaptchaServer
// created:2015-04-07
// modify:2015-04-08
// copyright fangbian.com 2015

#endregion

#region

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Mime;
using System.Web;
using System.Web.Caching;

#endregion

namespace CaptchaServerCache.api
{
    /// <summary>
    ///     cache 的摘要说明
    /// </summary>
    public class cache : IHttpHandler
    {
        private const string expirePrefix = "cache_expire_";

        public void ProcessRequest(HttpContext context)
        {
            HttpContext.Current.Application.Lock();
            try
            {

                const string prefix = "cache_";

                context.Response.ContentType = "text/plain";

                var method = (context.Request["opt"] + "").ToLower();

                var auth = context.Request["auth"] + "";

                if (auth != ConfigurationManager.AppSettings["queue_auth"])
                {
                    context.Response.Write("HTTPSQS_AUTH_FAILED");
                    context.Response.End();
                    return;
                }


                var data = context.Request["data"];

                var key = context.Request["key"];

                var expire = context.Request["expire"];

                switch (method)
                {
                    case "get":
                        if (HttpRuntime.Cache[key] != null)
                        {
                            context.Response.Write(HttpRuntime.Cache[key] + string.Empty);
                        }
                        else
                        {
                            context.Response.Write("HTTPSQS_GET_END");
                        }
                        break;
                    case "like":
                        var name = context.Request["key"];

                        System.Text.StringBuilder info = new System.Text.StringBuilder();

                        foreach (System.Collections.DictionaryEntry item in HttpRuntime.Cache)
                        {
                            if ((item.Key + string.Empty).Contains(name))
                            {
                                info.Append(item.Key + string.Empty + " ");
                            }
                        }
                        context.Response.Write(info.ToString());
                        break;
                    case "exist":
                        context.Response.Write(HttpRuntime.Cache[key] != null ? "HTTPSQS_EXIST_OK" : "HTTPSQS_GET_END");
                        break;
                    case "remove":
                        HttpRuntime.Cache.Remove(key);
                        context.Response.Write("HTTPSQS_REMOVE_OK");
                        break;
                    case "reset":
                        context.Response.Write("HTTPSQS_RESET_OK");
                        GC.Collect();
                        break;
                    case "put":
                        try
                        {
                            if (HttpRuntime.Cache[key] != null)
                            {
                                HttpRuntime.Cache.Remove(key);
                            }
                            if (!string.IsNullOrEmpty(expire))
                            {
                                HttpRuntime.Cache.Add(key,data, null,
                                       DateTime.Now.AddSeconds(Convert.ToInt32(expire)),
                                       TimeSpan.Zero, CacheItemPriority.NotRemovable, onRemoveCallback);
                            }
                            else
                            {
                                HttpRuntime.Cache.Insert(key, data);
                            }
                            context.Response.Write("HTTPSQS_PUT_OK");
                        }
                        catch (Exception)
                        {
                            context.Response.Write("HTTPSQS_ERROR");
                        }
                        break;
                }

                context.Response.End();
            }
            finally
            {
                HttpContext.Current.Application.UnLock();
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }

        private void onRemoveCallback(string key, object value, CacheItemRemovedReason reason)
        {
            if (HttpRuntime.Cache[key] != null)
            {
                var cacheKey = (string) HttpRuntime.Cache[key];

                HttpRuntime.Cache.Remove(key);
            }

            var instance = HttpRuntime.Cache[value + ""] as Dictionary<string, string>;
            if (instance != null)
            {
                lock (instance)
                {
                    if (!string.IsNullOrEmpty(key) && key.Length > expirePrefix.Length &&
                        key.IndexOf(expirePrefix, StringComparison.OrdinalIgnoreCase) != -1)
                    {
                        key = key.Substring(expirePrefix.Length);

                        if (instance.ContainsKey(key))
                        {
                            instance.Remove(key);
                        }
                    }
                }
            }
        }

        private void resetStatus(api_status status)
        {
            status.get = 0;
            status.put = 0;
        }

        private static void WriteLine(HttpResponse response, string s)
        {
            response.Write(s + "\r\n");
        }
    }
}