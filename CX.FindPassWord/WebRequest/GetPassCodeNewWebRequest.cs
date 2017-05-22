using ChangePassWord.Base;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using Tuniu.RegisterUser;
using Tuniu.WebRequest;

namespace ChangePassWord.WebRequest
{
    /// <summary>
    /// 获取验证码
    /// </summary>
    public class GetPassCodeNewWebRequest : IWebRequest
    {
        public string Request(Entiry.ChangePassWordSession session)
        {
            NameValueCollection queryCollection = new NameValueCollection();

            queryCollection.Add("module", "findpassword");

            queryCollection.Add("rand", "sjrand");

            NameValueCollection headerCollection = new NameValueCollection();

            headerCollection.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");

            headerCollection.Add("Accept-Encoding", "gzip,deflate");

            headerCollection.Add("Accept-Language", "zh-CN,zh;q=0.8");

            headerCollection.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/33.0.1750.146 BIDUBrowser/6.x Safari/537.36");

            NameValueCollection bodyCollection = new NameValueCollection();

            WebHttpsRequest webRequest = new WebHttpsRequest();

            HttpWebResponse webResponse = null;

            string url = "https://kyfw.12306.cn/otn/passcodeNew/getPassCodeNew";

            var source = webRequest.CreateRequest(System.Net.WebRequestMethods.Http.Get, url, session.Cookies, queryCollection, bodyCollection, headerCollection, out webResponse);

            return source;
        }
    }
}
