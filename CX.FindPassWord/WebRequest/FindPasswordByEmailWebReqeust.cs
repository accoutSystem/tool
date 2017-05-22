using ChangePassWord.Base;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using Tuniu.RegisterUser;

namespace ChangePassWord.WebRequest
{
    /// <summary>
    /// 找回邮件
    /// </summary>
   public class FindPasswordByEmailWebReqeust:IWebRequest
    {
        public string Email { get; set; }
        public string IdNo { get; set; }
        public string RandCode { get; set; }

        public string Request(Entiry.ChangePassWordSession session)
        {
            NameValueCollection queryCollection = new NameValueCollection();

          

            NameValueCollection headerCollection = new NameValueCollection();

            headerCollection.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");

            headerCollection.Add("Accept-Encoding", "gzip,deflate");

            headerCollection.Add("Accept-Language", "zh-CN,zh;q=0.8");

            headerCollection.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/33.0.1750.146 BIDUBrowser/6.x Safari/537.36");

            headerCollection.Add("Referer", "https://kyfw.12306.cn/otn/forgetPassword/initforgetMyPassword");

            headerCollection.Add("Origin", "https://kyfw.12306.cn");

            headerCollection.Add("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");


            NameValueCollection bodyCollection = new NameValueCollection();

            bodyCollection.Add("userDTO.email", Email);

            bodyCollection.Add("userDTO.loginUserDTO.id_type_code", "1");

            bodyCollection.Add("userDTO.loginUserDTO.id_no", IdNo);

            bodyCollection.Add("randCode", RandCode);

            bodyCollection.Add("randCode_validate", "");

            WebHttpsRequest webRequest = new WebHttpsRequest();

            HttpWebResponse webResponse = null;

            string url = "https://kyfw.12306.cn/otn/forgetPassword/findPasswordByEmail";

            var source = webRequest.CreateRequest(System.Net.WebRequestMethods.Http.Post, url, session.Cookies, queryCollection, bodyCollection, headerCollection, out webResponse);

            return source;
        }
    }
}
