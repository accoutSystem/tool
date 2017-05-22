using CX.Common;
using MobileValidServer;
using MobileValidServer.PassCodeProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ResourceBulidTool.PassCodeProvider
{
    /// <summary>
    /// 淘码手机验证码平台
    /// </summary>
    public class TMPassCodeProvider : BasePassCodeProvider
    {
        private string baseUrl = "http://api.taomapt.com/http.do?action=";

        private string UserName = "lichao73141";

        private string PassWord = Md5Helper.Md5("li7314CHAO1989").ToLower();

        public string ServerId = "5988";

        private static string Token { get; set; }

        public static ValidPlatformRequest request = null;

        public TMPassCodeProvider()
        {
            if (request == null)
            {
                request = new ValidPlatformRequest();
            }
        }

        public override string Login(string user, string password)
        {
            return string.Empty;
        }

        public override bool LoginSuccess(string result)
        {
            return true;
        }

        public override string GetMobilePhone()
        {
            string url = string.Format("{0}getPhones&userID={1}&password={2}&projectID={3}&size=1", baseUrl, UserName, PassWord,ServerId);
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
            return string.Empty;
        }

         

        public override string GetValidCode(string phone)
        {
            string url = string.Format("{0}?action=getVcodeAndReleaseMobile&mobile={1}&token={2}&author_uid=lichao73141", baseUrl, Phone, Token);

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
                
                if (phone.Contains("余额不足"))
                {
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
            string url = string.Format("{0}?action=cancelSMSRecv&token={1}&mobile={2}", baseUrl, Token, phone);
            var source = request.CreateGetRequest(url);
            WriteLog("释放手机"+source);
        }

        public override void AddBlackPhone(string phone, string reason)
        {
            string url = string.Format("{0}?action=addIgnoreList&pid={1}&token={2}&mobiles={3}", baseUrl, ServerId, Token, phone );
            var source = request.CreateGetRequest(url);
            WriteLog("添加黑名单"+source);
        }

    }

  
}
