using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MobileValidServer.PassCodeProvider
{
    /// <summary>
    /// 一片云新版手机验证平台
    /// </summary>
    public class YPYNewPassCodeProvider : BasePassCodeProvider
    {
       public static ValidPlatformRequest request = null;

       private string BaseUrl = "http://api.ypyun.com/http.do?action=";

       private string UserName = string.Empty;

       private string token = string.Empty;

       public YPYNewPassCodeProvider()
       {
           if (request == null)
           {
               request = new ValidPlatformRequest();
           }
       }

       public string ServerId = "3712";

       public override string GetMobilePhone()
        {
            string url =string.Format("{0}getMobilenum&pid={1}&uid={2}&token={3}",BaseUrl,ServerId,UserName,token);

            for (int i = 0; i < 10; i++)
            {
                var phone = request.CreateGetRequest(url);

                if (phone.Contains("no_enough_scor"))
                {
                    return string.Empty;
                }
                
                WriteLog("获取手机号" + phone);

                if (phone.Contains("|") && !phone.Contains("当前无号"))
                {
                    var item = phone.Replace(";","").Split('|');

                    if (item.Length == 2)
                    {
                        var currentPhone = item[0];

                        if (IsMobilePhone(currentPhone))
                        {
                            ExcuteTask(currentPhone);
                            return currentPhone;
                        }
                    }
                }
                Thread.Sleep(1000);
            }

            return string.Empty;
        }

       private void ExcuteTask(string phone) {

           string url = string.Format("{0}executeBs&uid={1}&token={2}&pid={3}&mobile={4}&step=1&Author=lichao7314", BaseUrl, UserName, token, ServerId, phone);
           var result = request.CreateGetRequest(url);
           Console.Write("一片云执行手机号任务" + phone + "结果" + result);
       }

       public override string GetValidCode(string phone)
        {
            string url = string.Format("{0}getExeResult&uid={1}&token={2}&pid={3}&step=1&mobile={4}",BaseUrl,UserName,token,ServerId,phone);

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

                if (validPassCode.Contains("|") || validPassCode.Contains("验证码"))
                {
                    return request.ReadValidCode(validPassCode);
                }

                //if (validPassCode.Contains("短信还未到达") ||
                //    validPassCode.Contains("网络超时") ||
                //     validPassCode.Contains("当前无短信"))
                //{
                    i++;
                //}

                if (i > 15)
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
           // string url = string.Format("http://42.120.60.152/do.aspx?action=releasePhone&type={0}&phone={1}", ServerId, phone);

           //var source= request.CreateGetRequest(url);
           //WriteLog("释放手机" + source);
        }

       public override void AddBlackPhone(string phone, string reason)
        {
            string url = string.Format("{0}addIgnoreCard&uid={1}&token={2}&mobile={3}&pid={4}",BaseUrl,UserName,token,phone,ServerId);

            var blackResult = request.CreateGetRequest(url);
            WriteLog("添加黑名单:" + blackResult);
        }

       public override string Login(string user, string password)
        {
            string url = string.Format("{2}loginIn&uid={0}&pwd={1}", user, password, BaseUrl);
            UserName = user;
            var source = request.CreateGetRequest(url) ;
            return source;
        }

       public override bool LoginSuccess(string result)
        {
           //用户名|token(下面所有方法都要用的令牌)
            if (result.Contains(UserName)&&result.Contains("|")) 
            {
                token = result.Split('|')[1];
                return true;
            }
            return  false;
        }
    }

}
