#region source code header

// solution:CaptchaServer
// created:2015-04-08
// modify:2015-04-08
// copyright fangbian.com 2015

#endregion
using System.Linq;
using System.Collections.Generic;
namespace CaptchaServerCacheClient
{
    public class CacheClient : AbstractBaseClient
    {
        public CacheClient()
        {
            ClientType= ClientType.Cache;
            Auth = "showmethetask";
        }
        public string Get(string key)
        {
            return GetStringKeyData("get", key);
        }

        public void Add(string key, string data, int? expire)
        {
            PutStringKeyData("put", key, data, expire);
        }

       
        public void Add(string key, string data)
        {
            PutStringKeyData("put", key, data, null);
        }

        public bool Exist(string key)
        {
            return GetStringKeyData("exist", key) == "HTTPSQS_EXIST_OK";
        }

        public List<string> Like(string name) {

            var source= GetStringKeyData("like", name);

            return source.TrimEnd(' ').Split(' ').ToList();
        }

        public bool Remove(string key)
        {
            return GetStringKeyData("remove", key) == "HTTPSQS_REMOVE_OK";
        }
    }
}