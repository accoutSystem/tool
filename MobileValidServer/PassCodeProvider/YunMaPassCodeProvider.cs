using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MobileValidServer.PassCodeProvider
{
    /// <summary>
    /// 云码手机验证平台
    /// </summary>
    public class YunMaPassCodeProvider : BasePassCodeProvider
    {
        public static ValidPlatformRequest request = null;

        private string baseUrl = "http://api.vim6.com/DevApi/";

        public string ServerId = "1000";

        public static string Token = string.Empty;
        public static string Uid = string.Empty;


        public YunMaPassCodeProvider()
        {
            if (request == null)
            {
                request = new ValidPlatformRequest();
            }
        }

        public override string Login(string user, string password)
        {
            string url = string.Format("{2}loginIn?uid={0}&pwd={1}", user, password, baseUrl);
            var source = request.CreateGetRequest(url);
            return source;

        }

        public override void AddBlackPhone(string phone, string reason)
        {
            string url = string.Format("{0}addIgnoreList?pid={1}&mobiles={2}&uid={3}&token={4}", baseUrl, ServerId, phone, Uid,Token);
            var source = request.CreateGetRequest(url);
            WriteLog("添加黑名单" + source);
        }

        public override string GetMobilePhone()
        {
            string url = string.Format("{0}getMobilenum?uid={1}&pid={2}&token={3}&size=1", baseUrl, Uid, ServerId, Token );
            var phone = request.CreateGetRequest(url);

            WriteLog("获取手机号" + phone);
            if (phone.Contains("余额不足"))
            {
                CurrentUser.State = UserState.余额不足;
                return string.Empty;
            }
            if (phone.Length == 11) {
                return phone;
            }
            CurrentUser.State = UserState.获取手机号码失败;

            return string.Empty;
           
        }
        public override void DisposePhone(string phone)
        {
            string url = string.Format("{0}cancelSMSRecv?mobile={1}&uid={2}&token={3}", baseUrl, phone, Uid, Token);
            var source = request.CreateGetRequest(url);
            WriteLog("取消接收短信" + source); 
        }

        public override string GetValidCode(string phone)
        {
            string url = string.Format("{0}getVcodeAndReleaseMobile?mobile={1}&uid={2}&token={3}&author_uid={4}&pid={5}", baseUrl, phone, Uid,
                Token,"lichao7314",ServerId);

            int i = 0;

            string value = string.Empty;

            while (true)
            {
                Console.Write("开始获取" + phone + "验证码" + i + "次重试");

                Thread.Sleep(3000);

                var validPassCode = request.CreateGetRequest(url);

                WriteLog("获取验证码" + validPassCode);

                if (phone.Contains("余额不足"))
                {
                    CurrentUser.State = UserState.余额不足;
                    return string.Empty;
                }

                if (validPassCode.Contains("|") || validPassCode.Contains("验证码"))
                {
                    var ss = validPassCode.Split('|');

                    if (ss[0].Equals("1"))
                    {
                        return request.ReadValidCode(validPassCode);
                    }
                }

                i++;

                if (i > 30)
                {
                    break;
                }
            }

            return value;
        }

        public override bool LoginSuccess(string result)
        {
            try
            {
                JObject s = JObject.Parse(result);
                Uid = s["Uid"] + string.Empty;
                Token = s["Token"] + string.Empty;
                return true;
            }
            catch {
                return false;
            }
            //{"Uid":46116,"Token":"3Rl0IhkTWBvwdPj9wkaBoIf8w0FNp9RUOnRCGEgeq33JL57tYyB2Me3jUtc0auMFFyPNTOHyLnIpmcYPdkRVqQ==","InCome":0.000,"Balance":0.000,"UsedMax":20}
        }
    }
}
