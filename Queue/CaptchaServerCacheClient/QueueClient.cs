#region source code header

// solution:CaptchaServer
// created:2015-04-07
// modify:2015-04-08
// copyright fangbian.com 2015

#endregion

#region

#endregion

namespace CaptchaServerCacheClient
{
    public class QueueClient : AbstractBaseClient
    {
        public QueueClient()
        {
            ClientType = ClientType.Queue;
        }
        public string Dequeue()
        {
            return GetStringData("get");
        }


        public int GetCount()
        {
            var value = GetStringData("get_status");

            int outValue = 0;

            int.TryParse(value, out outValue);

            return outValue;
        }

        public void Enqueue(string data)
        {
            PutStringData("put", data);
        }
    }
}