using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToolWebManager
{
    /// <summary>
    /// analog 的摘要说明
    /// </summary>
    public class analog : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            JObject source = new JObject();
            source.Add("code", "200");
            JObject data = new JObject();
            data.Add("user_agent", "Mozilla/5.0 (Linux; U; Android 4.2.2; zh-cn; HM NOTE 1W Build/JDQ39) AppleWebKit/534.30 (KHTML, like Gecko) Version/4.0 Mobile Safari/534.30/Worklight/6.0.0");
            data.Add("wap_profile", "http://218.249.47.94/Xianghe/MTK_Phone_JB_UAprofile.xml");
            data.Add("rdevice_no", "");
            source.Add("data", data);
            context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(source));
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}