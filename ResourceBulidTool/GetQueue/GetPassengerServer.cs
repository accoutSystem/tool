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
    public class GetPassengerServer
    {
        public int i = 0;

        public static T_Passenger Get()
        {
            string url = "http://121.43.110.247/BaseServer/";

            string auth = "showmethetask";

            var passengerQueue = new QueueClient()
            {
                Url = url,
                Auth = auth,
                Name = "goodPassenger"
            };
             
            string data = passengerQueue.Dequeue();

            if (!string.IsNullOrEmpty(data) && data != "HTTPSQS_GET_END")
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T_Passenger>(data);
            }

            passengerQueue.Name = "Passenger";

            data = passengerQueue.Dequeue();

            if (!string.IsNullOrEmpty(data) && data != "HTTPSQS_GET_END")
            {
                var source = Newtonsoft.Json.JsonConvert.DeserializeObject<T_Passenger>(data);

                if (ToolCommonMethod.IsIdNo(source.IdNo))
                {
                    return source;
                }
                else
                {
                    WritePassengerState(1, source.PassengerId);
                    return null;
                }
            }

            return null;
            
        }
        private static void WritePassengerState(int state, string id)
        {
            try
            {
                var data = DataTransaction.Create();

                data.ExecuteSql("update t_passenger set state=" + state + " where passengerid='" + id + "' ");
            }
            catch { }
        }
    }
}
