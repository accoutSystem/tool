using CaptchaServerCacheClient;
using MobileValidServer;
using MyEntiry;
using PD.Business;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ResourceBulidTool
{
    public class GetPhoneValidServer
    {
        public int i = 0;

        public static UserItem Get()
        {
            string url = "http://121.43.110.247/BaseServer/";

            string auth = "showmethetask";

            var passengerQueue = new QueueClient()
            {
                Url = url,
                Auth = auth,
                Name = "validPhone"
            };
            string data = passengerQueue.Dequeue();

            if (data == "HTTPSQS_GET_END" || string.IsNullOrEmpty(data))
            {
                return null;
            }
            else
            {
                var item = Newtonsoft.Json.JsonConvert.DeserializeObject<T_ValidUser>(data);
                return new UserItem { UserGuid = item.UserGuid, UserName = item.UserName, PassWord = item.PassWord };
            }
        }
    }
}
