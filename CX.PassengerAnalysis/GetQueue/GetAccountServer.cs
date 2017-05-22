using CaptchaServerCacheClient;
using MyEntiry;
using MyTool.Common;
using PD.Business;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ResourceBulidTool
{
    public class GetAccountServer
    {
        public int i = 0;

        public static T_ValidUser Get()
        {
            string url = "http://121.43.110.247/BaseServer/";

            string auth = "showmethetask";

            var passengerQueue = new QueueClient()
            {
                Url = url,
                Auth = auth,
                Name = "readAddValidAccount"
            };
             
            string data = passengerQueue.Dequeue();

            if (!string.IsNullOrEmpty(data) && data != "HTTPSQS_GET_END")
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T_ValidUser>(data);
            }

            passengerQueue.Name = "Passenger";

            data = passengerQueue.Dequeue();

            if (!string.IsNullOrEmpty(data) && data != "HTTPSQS_GET_END")
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T_ValidUser>(data);
            }

            return null;
            
        }
    }
}
