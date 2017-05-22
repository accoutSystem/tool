using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MobileValidServer.PassCodeProvider
{
    /// <summary>
    /// 壹码手机验证平台
    /// </summary>
    public class YMPassCodeProvider : BasePassCodeProvider
    {
        public static ValidPlatformRequest request = null;

        private string baseUrl = "http://www.yzm1.com/api/do.php?";

        public string ServerId = "5995";

        public static string Token { get; set; }

        public YMPassCodeProvider()
        {
            if (request == null)
            {
                request = new ValidPlatformRequest();
            }
        }

        public override string Login(string user, string password)
        {
            string url = string.Format("{2}action=loginIn&name={0}&password={1}", user, password, baseUrl);
            var source = request.CreateGetRequest(url);
            return source;
        }

        public override bool LoginSuccess(string result)
        {
            var success = result.Contains("|");
            if (success)
            {
                Token = result.Split('|')[1];
            }
            return success;
        }

        public override string GetMobilePhone()
        {
            string url = string.Format("{0}action=getPhone&sid={1}&token={2}", baseUrl, ServerId, Token);

            string phone = string.Empty;
            try
            {
                phone = request.CreateGetRequest(url);
            }
            catch (Exception ex)
            {
               WriteLog("开始手机号码错误" + ex.Message);
            }

            WriteLog("获取手机号" + phone);
            if (phone.Contains("余额不足"))
            {
                CurrentUser.State = UserState.余额不足;
                return string.Empty;
            }
            if (phone.Contains("|"))
            {
                var o = phone.Split('|');

                if (o[0].Equals("1"))
                {
                     string currentPhone= phone.Split('|')[1];
                     if (IsMobilePhone(currentPhone))
                         return currentPhone;
                }
            }

            if (phone.Contains("超出频率"))
            {
                Thread.Sleep(2000);
            }

            CurrentUser.State = UserState.获取手机号码失败;

            return string.Empty;
        }

        public override string GetValidCode(string phone)
        {
            string url = string.Format("{0}action=getMessage&sid={1}&phone={2}&token={3}&author=lichao7314", baseUrl, ServerId, phone, Token);

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

                CurrentUser.Message = validPassCode;

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

                if (i > 10)
                {
                    break;
                }
            }

            return value;
        }

        public override void DisposePhone(string phone)
        {
            string url = string.Format("{0}action=cancelRecv&sid={1}&phone={2}&token={3}", baseUrl, ServerId, phone, Token);
            var source = request.CreateGetRequest(url);
            WriteLog("释放手机" + source);
        }

        public override void AddBlackPhone(string phone, string reason)
        {
            string url = string.Format("{0}action=addBlacklist&sid={1}&phone={2}&token={3}", baseUrl, ServerId, phone, Token);
            var source = request.CreateGetRequest(url);
            WriteLog("添加黑名单" + source);
        }
    }
}
