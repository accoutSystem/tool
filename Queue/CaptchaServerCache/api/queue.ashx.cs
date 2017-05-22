#region source code header

// solution:CaptchaServer
// created:2015-04-23
// modify:2015-04-30
// copyright fangbian.com 2015

#endregion

#region

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;

#endregion

namespace CaptchaServerCache.api
{
    /// <summary>
    ///     queue 的摘要说明
    /// </summary>
    public class queue : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            HttpContext.Current.Application.Lock();

            try
            {
                const string prefix = "queue_";

                context.Response.ContentType = "text/plain";

                var method = (context.Request["opt"] + "").ToLower();

                var cacheKey = prefix + (context.Request["name"] ?? "task");
                var statusKey = cacheKey + "_status";

                var auth = context.Request["auth"] + "";

                if (auth != ConfigurationManager.AppSettings["queue_auth"])
                {
                    context.Response.Write("HTTPSQS_AUTH_FAILED");
                    context.Response.End();
                    return;
                }

                var instance = HttpRuntime.Cache[cacheKey] as Queue<string>;
                if (instance == null)
                {
                    instance = new Queue<string>();
                    HttpRuntime.Cache[cacheKey] = instance;

                    HttpRuntime.Cache[statusKey] = new api_status
                    {
                        get = 0,
                        name = (context.Request["name"] ?? "task"),
                        put = 0,
                        unread = 0,
                        version = "Queue server 1.0 by fangbian.com#suifei"
                    };
                }
                var status = (api_status) HttpRuntime.Cache[statusKey];

                switch (method)
                {
                    case "get_status":
                        context.Response.Write(instance.Count);
                        break;
                    case "get":
                        if (instance.Count > 0)
                        {
                            if (status.get + 1 >= int.MaxValue)
                                resetStatus(status);
                            status.get++;
                            status.unread = instance.Count;
                            context.Response.Write(instance.Dequeue());
                        }
                        else
                        {
                            status.unread = instance.Count;
                            context.Response.Write("HTTPSQS_GET_END");
                        }
                        break;
                    case "view":
                        context.Response.Write(instance.Peek());
                        break;
                    case "status":
                        WriteLine(context.Response, status.version);
                        WriteLine(context.Response, "------------------------------");
                        WriteLine(context.Response, "Queue Name: " + (context.Request["name"] ?? "task"));
                        WriteLine(context.Response, "Maximum number of queues: " + int.MaxValue);
                        WriteLine(context.Response, "Put position of queue (1st lap):" + status.put);
                        WriteLine(context.Response, "Get position of queue (1st lap):" + status.get);
                        WriteLine(context.Response, "Number of unread queue:" + status.unread);
                        break;
                    case "reset":
                        instance.Clear();
                        status.unread = 0;
                        status.get = 0;
                        status.put = 0;
                        context.Response.Write("HTTPSQS_RESET_OK");
                        GC.Collect();
                        break;
                    case "put":
                        try
                        {
                            var data = context.Request["data"];
                            instance.Enqueue(data);

                            if (status.put + 1 >= int.MaxValue)
                                resetStatus(status);

                            status.put++;
                            status.unread = instance.Count;
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