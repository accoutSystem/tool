using CaptchaServerCacheClient;
using MyEntiry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VaildTool
{
    public class ReadEmailQueue
    {
        public static T_ValidUser GetEmail() {
            string url = "http://121.43.110.247/BaseServer/";

            string auth = "showmethetask";

            var validQueue = new QueueClient()
            {
                Url = url,
                Auth = auth,
                Name = "valid"
            };

            var queueData = validQueue.Dequeue(); 
            
            if (queueData == "HTTPSQS_GET_END" || string.IsNullOrEmpty(queueData))
            {
                return null;    
            }

            return     Newtonsoft.Json.JsonConvert.DeserializeObject<T_ValidUser>(queueData);
        }
    }
}
