#region source code header

// solution:Workstation
// created:2015-02-15
// modify:2015-02-27
// copyright fangbian.com 2015

#endregion

#region

using System;
using System.Web;

#endregion

namespace Fangbian.Tickets.Trains.WebInterface
{
    /// <summary>
    ///     writestatus 的摘要说明
    /// </summary>
    public class writestatus : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            
            var taskId = context.Request["taskid"];

            var taskVal = context.Request["val"];

            if (!string.IsNullOrWhiteSpace(taskId))
            {
                if (("remove").Equals(context.Request["type"]))
                {
                    HttpRuntime.Cache.Remove(taskId);
                }
                else
                {
                    var timeOut = context.Request["timeout"];

                    int outValue = 0;

                    int.TryParse(timeOut, out outValue);

                    if (outValue>0)
                    {
                        if (HttpRuntime.Cache.Get(taskId) != null)
                        {
                            HttpRuntime.Cache.Remove(taskId);
                        }

                        HttpRuntime.Cache.Insert(taskId, taskVal, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, outValue, 0));
                    }
                    else 
                    {
                        HttpRuntime.Cache[taskId] = taskVal;
                    }
                }
                context.Response.Write("OK");
            }
            else
            {
                context.Response.Write("taskid is NULL");
            }

            context.Response.End();
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}