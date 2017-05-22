#region source code header

// solution:CaptchaServer
// created:2015-04-08
// modify:2015-04-08
// copyright fangbian.com 2015

#endregion

namespace CaptchaServerCacheClient
{
    public class StackClient : AbstractBaseClient
    {
        public StackClient()
        {
            ClientType= ClientType.Stack;
        }
        public string Pop()
        {
            return GetStringData("get");
        }

        public void Push(string data)
        {
            PutStringData("put", data);
        }
    }
}