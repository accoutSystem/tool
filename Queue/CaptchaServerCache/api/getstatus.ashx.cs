#region source code header

// solution:Workstation
// created:2015-02-15
// modify:2015-02-27
// copyright fangbian.com 2015

#endregion

#region

using System.Web;

#endregion

namespace Fangbian.Tickets.Trains.WebInterface
{
    /// <summary>
    ///     getstatus 的摘要说明
    /// </summary>
    public class getstatus : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            var taskId = context.Request["taskid"];

            if (string.IsNullOrEmpty(taskId))
            {
                taskId = context.Request["opt"];
            }

            var type = context.Request["queryModel"];

            if ("All".Equals(type))
            {
                System.Text.StringBuilder key = new System.Text.StringBuilder();
                foreach (var item in HttpRuntime.Cache)
                {
                    var cache = ((System.Collections.DictionaryEntry)(item));

                    string currentKey = cache.Key + string.Empty;
                     
                    context.Response.Write(currentKey+HttpContext.Current.Server.HtmlDecode("<br/>"));
                }
            }
            else if ("like".Equals(type))
            {
                System.Text.StringBuilder key = new System.Text.StringBuilder();
                foreach (var item in HttpRuntime.Cache)
                {
                    string currentKey = ((System.Collections.DictionaryEntry)(item)).Key + string.Empty;
                    if ((currentKey).Contains(taskId))
                    {
                        key.Append(currentKey + ",");
                    }
                }
                context.Response.Write(key.ToString());
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(taskId))
                {
                    context.Response.Write(HttpRuntime.Cache[taskId]);
                }
                else
                {
                    context.Response.Write("taskid is NULL");
                }
            }

            context.Response.End();
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}