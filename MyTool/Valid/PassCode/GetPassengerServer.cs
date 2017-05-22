using CaptchaServerCacheClient;
using MyEntiry;
using PD.Business;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ResourceBulidTool
{
    public class GetPassengerServer
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
                Name = "readPassenger"
            };
            string data = passengerQueue.Dequeue();

            if (data == "HTTPSQS_GET_END" || string.IsNullOrEmpty(data))
            {
                return null;
            }
            else
            {
                return  Newtonsoft.Json.JsonConvert.DeserializeObject<T_ValidUser>(data);
            }
        }
    }
}
