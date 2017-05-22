using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MobileValidServer.PassCodeProvider
{
    /// <summary>
    /// 爱码手机验证平台
    /// </summary>
    public class AMPassCodeProvider : BasePassCodeProvider
    {

        private string UserName = "lichao7314";

        public string ServerId = "4036";

        private static string Token { get; set; }

        public static ValidPlatformRequest request = null;

        public AMPassCodeProvider()
        {
            if (request == null)
            {
                request = new ValidPlatformRequest();
            }
        }

        public override string Login(string user, string password)
        {
            //UserName = user;
            string url = string.Format("http://api.f02.cn/http.do?action=loginIn&uid={0}&pwd={1}", user, password);

            var source = request.CreateGetRequest(url);

            return source;
        }

        public override string GetMobilePhone()
        {
            string url = string.Format("http://api.f02.cn/http.do?action=getMobilenum&pid={0}&uid={1}&token={2}&mobile=&size=1", ServerId, UserName, Token);
            string phone = string.Empty;
            try
            {
                phone = request.CreateGetRequest(url);
            }
            catch (Exception ex)
            {
                WriteLog("开始手机号码错误" + ex.Message);
            }
            WriteLog("获取手机号"+phone);
            if (phone.Contains("余额不足"))
            {
                CurrentUser.State = UserState.余额不足;
                return string.Empty;
            }
            if (phone.Contains("message"))
            {
                CurrentUser.State = UserState.获取手机号码失败;
                return string.Empty;
            }
            if (phone.Contains("|"))
            {
                string currentPhone = phone.Split('|')[0];

                if (IsMobilePhone(currentPhone))
                {
                    return currentPhone;
                }
            }

            if (phone.Contains("速度过快"))
            {
                Thread.Sleep(2000);
            }

            CurrentUser.State = UserState.获取手机号码失败;

            return string.Empty;
        }

        public override string GetValidCode(string phone)
        {
            string url = string.Format("http://api.f02.cn/http.do?action=getVcodeAndReleaseMobile&uid={0}&token={1}&mobile={2}&author_uid=lichao7314", UserName, Token, phone);

            int i = 0;

            string value = string.Empty;

            while (true)
            {
                Console.Write("开始获取" + phone + "验证码" + i + "次重试");

                Thread.Sleep(4000);
                string validPassCode = string.Empty;
                try
                {
                    validPassCode = request.CreateGetRequest(url);
                }
                catch (Exception ex)
                {
                    WriteLog("开始获取验证码错误" + ex.Message);
                }

                WriteLog("获取验证码"+validPassCode);

                CurrentUser.Message = validPassCode;
                
                if (phone.Contains("余额不足"))
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

                if (i > 10)
                {
                    break;
                }
            }

            return value;
        }

        public override void DisposePhone(string phone)
        {
            string url = string.Format("http://api.f02.cn/http.do?action=cancelSMSRecv&uid={0}&token={1}&mobile={2}", UserName, Token, phone);
            var source = request.CreateGetRequest(url);
            WriteLog("释放手机"+source);
        }

        public override void AddBlackPhone(string phone, string reason)
        {
            string url = string.Format("http://api.f02.cn/http.do?action=addIgnoreList&uid={0}&token={1}&mobiles={2}&pid={3}", UserName, Token, phone, ServerId);
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
