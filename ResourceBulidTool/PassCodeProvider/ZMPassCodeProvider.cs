using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MobileValidServer.PassCodeProvider
{
    /// <summary>
    /// 卓码手机验证平台
    /// </summary>
    public class ZMPassCodeProvider : BasePassCodeProvider
    {

        private string UserName = "lichao7314";

        public string ServerId = "4799";

        private static string Token { get; set; }

        public static ValidPlatformRequest request = null;

        public string BaseUrl = "http://api.zmyzm.com/apiGo.do?action=";

        public ZMPassCodeProvider()
        {
            if (request == null)
            {
                request = new ValidPlatformRequest();
            }
        }

        public override string Login(string user, string password)
        {
            //UserName = user;
            string url = string.Format(BaseUrl + "loginIn&uid={0}&pwd={1}", user, password);

            var source = request.CreateGetRequest(url);

            return source;
        }

        public override string GetMobilePhone()
        {
            string url = string.Format(BaseUrl + "getMobilenum&pid={0}&uid={1}&token={2}&mobile=&size=1", ServerId, UserName, Token);
            var phone = request.CreateGetRequest(url);
            WriteLog("获取手机号"+phone);
            if (phone.Contains("Lack_of_balance"))
            {
                CurrentUser.State = UserState.余额不足;
                return string.Empty;
            }
            if (phone.Contains("|"))
            {
                return phone.Split('|')[0];
            }
            CurrentUser.State = UserState.获取手机号码失败;
            return string.Empty;
        }

        public override string GetValidCode(string phone)
        {
            string url = string.Format(BaseUrl + "getVcodeAndReleaseMobile&uid={0}&token={1}&mobile={2}", UserName, Token, phone);

            int i = 0;

            string value = string.Empty;

            while (true)
            {
                Console.Write("开始获取" + phone + "验证码" + i + "次重试");

                Thread.Sleep(5000);

                var validPassCode = request.CreateGetRequest(url);

                WriteLog("获取验证码"+validPassCode);

                CurrentUser.Message = validPassCode;

                if (phone.Contains("Lack_of_balance"))
                {
                    CurrentUser.State = UserState.余额不足;
                    return string.Empty;
                }
                if (validPassCode.Contains("没有找到手机号") ||
                    validPassCode.Contains("账号ip锁定") ||
                    validPassCode.Contains("账号被锁定") ||
                    validPassCode.Contains("账号被停用"))
                {
                    break;
                }

                if (validPassCode.Contains("|") && validPassCode.Contains("验证码"))
                {
                    return request.ReadValidCode(validPassCode);
                }

                i++;

                if (i > 20)
                {
                    break;
                }
            }

            return value;
        }

        public override void DisposePhone(string phone)
        {
            //string url = string.Format("http://api.f02.cn/http.do?action=cancelSMSRecv&uid={0}&token={1}&mobile={2}", UserName, Token, phone);
            //var source = request.CreateGetRequest(url);
            //WriteLog("释放手机"+source);
        }

        public override void AddBlackPhone(string phone, string reason)
        {
            string url = string.Format(BaseUrl + "addIgnoreList&uid={0}&token={1}&mobiles={2}&pid={3}", UserName, Token, phone, ServerId);
            var source = request.CreateGetRequest(url);
            WriteLog("添加黑名单"+source);
        }

        public override bool LoginSuccess(string result)
        {
            var su = result.Contains(UserName);

            if (su)
            {
                Token = result.Split('|')[1];
            }
            return su;
        }
    }
}
