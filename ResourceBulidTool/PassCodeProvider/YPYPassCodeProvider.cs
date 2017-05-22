using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MobileValidServer.PassCodeProvider
{
    /// <summary>
    /// 一片云手机验证平台
    /// </summary>
    public class YPYPassCodeProvider : BasePassCodeProvider
    {
       public static ValidPlatformRequest request = null;

       public YPYPassCodeProvider()
       {
           if (request == null)
           {
               request = new ValidPlatformRequest();
           }
       }

       public string ServerId = "4291";

       public override string GetMobilePhone()
        {
            string url = "http://42.120.60.152/do.aspx?action=getMobilenum&type=" + ServerId;

            for (int i = 0; i < 10; i++)
            {
                var phone = request.CreateGetRequest(url);

                if (phone.Contains("余额不足"))
                {
                    return string.Empty;
                }
                
                WriteLog("获取手机号" + phone);

                if (phone.Contains("|") && !phone.Contains("当前无号"))
                {
                    var item = phone.Split('|');

                    if (item.Length == 2)
                    {
                        var currentPhone = item[1];
                        if (IsMobilePhone(currentPhone))
                        {
                            return currentPhone;
                        }
                    }
                }
                Thread.Sleep(1000);
            }

            return string.Empty;
        }

       public override string GetValidCode(string phone)
        {
            string url = "http://42.120.60.152/do.aspx?action=getSMS&authorid=116361&phone=" + phone;

            int i = 0;

            string value = string.Empty;

            while (true)
            {
                Thread.Sleep(3000);

                var validPassCode = request.CreateGetRequest(url);
                WriteLog("获取验证码" + validPassCode);

                if (validPassCode.Contains("余额不足"))
                {
                    break;
                }

                if (validPassCode.Contains("|")
                    || validPassCode.Contains("验证码"))
                {

                    return request.ReadValidCode(validPassCode);
                }

                if (validPassCode.Contains("短信还未到达") ||
                    validPassCode.Contains("网络超时") ||
                     validPassCode.Contains("当前无短信"))
                {
                    i++;
                }

                if (i > 30)
                {
                    break;
                }

                if (validPassCode.Contains("号码已经被释放") ||
                    validPassCode.Contains("COOK失效"))
                {
                    break;
                }
            }

            return value;
        }

       public override void DisposePhone(string phone)
        {
            string url = string.Format("http://42.120.60.152/do.aspx?action=releasePhone&type={0}&phone={1}", ServerId, phone);

           var source= request.CreateGetRequest(url);
           WriteLog("释放手机" + source);
        }

       public override void AddBlackPhone(string phone, string reason)
        {
            if (string.IsNullOrEmpty(reason))
            {
                reason = "0";
            }
            string url = string.Format("http://42.120.60.152/do.aspx?action=BlackList&phone={0}&type=9&reason={1}", phone, reason);

            var blackResult = request.CreateGetRequest(url);
            WriteLog("添加黑名单" + blackResult);
        }

       public override string Login(string user, string password)
        {
            string url = string.Format("http://42.120.60.152/do.aspx?action=loginIn&uid={0}&pwd={1}", user, password);

            var source = request.CreateGetRequest(url) ;
            return source;
        }


       public override bool LoginSuccess(string result)
        {
            return result.Contains("Login success");
        }


    }

}
