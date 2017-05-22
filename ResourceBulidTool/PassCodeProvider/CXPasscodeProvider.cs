using Fangbian.Log;
using FangBian.Common;
using MobileValidServer.PassCodeProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ResourceBulidTool.PassCodeProvider
{
   public class CXPasscodeProvider : BasePassCodeProvider
    {
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
           string url = string.Format("http://{0}/phone.ashx?type=getPhone", System.Configuration.ConfigurationManager.AppSettings["baseUrl"]);

           var source = Get(url, "businessId=1");

           if (source.Code == CodeResult.Success)
           {
               return source.Result;
           }
           return string.Empty;  
       }

       public  string SendMessage(string phone)
       {
           string url = string.Format("http://{0}/phone.ashx?type=sendMessage", System.Configuration.ConfigurationManager.AppSettings["baseUrl"]);

           var source = Get(url, "phone="+phone+"&businessId=1&sendToPhone=12306&sendMsg=999");

           if (source.Code == CodeResult.Success) 
           {
               return "OK";
           }
             
           return source.Message;
 
       }

       public override void DisposePhone(string phone)
       {
           string url = string.Format("http://{0}/phone.ashx?type=disposePhone", System.Configuration.ConfigurationManager.AppSettings["baseUrl"]);

           var source = Get(url, "phone=" + phone + "&businessId=1");
       }

       public override void AddBlackPhone(string phone, string reason)
       {
           string url = string.Format("http://{0}/phone.ashx?type=addBlackPhone", System.Configuration.ConfigurationManager.AppSettings["baseUrl"]);

           var source = Get(url, "phone=" + phone+"&businessId=1");

           Logger.Info(phone + "加黑成功" + source.Code+" "+source.Result+" "+source.Message);
       }

       public override string GetValidCode(string phone)
       {
           string url = string.Format("http://{0}/phone.ashx?type=getMsgPhone", System.Configuration.ConfigurationManager.AppSettings["baseUrl"]);

           int i = 0;

           string value = string.Empty;

           var retryCount = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["retryCount"]);

           while (true)
           {
               Console.Write("开始获取" + phone + "验证码" + i + "次重试");

               Thread.Sleep(5000);

               var validPassCode = Get(url, "phone=" + phone + "&businessId=1");

               //WriteLog("获取验证码" + validPassCode);

               if (validPassCode.Code == CodeResult.Success)
               {
                   Logger.Info(phone + "获取验证码成功:" + validPassCode.Result);
                   value = ReadValidCode(validPassCode.Result);
                   break;
               }
               i++;

               if (i > retryCount)
               {
                   break;
               }
           }

           return value;
       }

       public void SuccessPhone(string phone)
       {
           string url = string.Format("http://{0}/phone.ashx?type=successPhone", System.Configuration.ConfigurationManager.AppSettings["baseUrl"]);

           var source = Get(url, "phone=" + phone + "&businessId=1");

           Logger.Info(phone + "计费成功"+Newtonsoft.Json.JsonConvert.SerializeObject( source));
       }

       public DataResult Get(string url, string data)
       {
           var result = HttpHelper.Post(url, data);
           try
           {
               return Newtonsoft.Json.JsonConvert.DeserializeObject<DataResult>(result);
           }
           catch
           {
               return new DataResult { Code = CodeResult.Exception };
           }
       }

       public string ReadValidCode(string validCode)
       {
           //10||2015-08-02 22:34:57: COM 62口(13530740156)从12306接收了一条短信  【铁路客服】验证码：241641，切勿转发。12306网站用户正在申请核验尾号为0156的手机。如非本人操作，请忽略本短信。

           var valid = validCode;

           var index = valid.IndexOf("验证码：");

           valid = valid.Substring(index, valid.Length - index);

           valid = valid.Replace("验证码：", "");

           valid = valid.Substring(0, valid.IndexOf("。"));

           valid = valid.Replace("。", "");

           return valid;
       }

       /// <summary>
       /// 可手工重试
       /// </summary>
       /// <param name="phone"></param>
       public void RetryPhone(string phone)
       {
           string url = string.Format("http://{0}/phone.ashx?type=setPhoneState", System.Configuration.ConfigurationManager.AppSettings["baseUrl"]);

           var source = Get(url, "phone=" + phone + "&state=9&businessId=1"  );
       }
    }

   public class DataResult
   {
       public string Code { get; set; }

       public string Result { get; set; }
       public string Message { get; set; }
   }

   public class CodeResult
   {
       /// <summary>
       /// 成功
       /// </summary>
       public const string Success = "101";
       /// <summary>
       /// 参数错误
       /// </summary>
       public const string ParamError = "102";
       /// <summary>
       /// 执行异常
       /// </summary>
       public const string Exception = "103";
       /// <summary>
       /// 没有手机后啊
       /// </summary>
       public const string NoPhone = "104";
       /// <summary>
       /// 空结果
       /// </summary>
       public const string ResultNull = "105";
       /// <summary>
       /// 手机号码被使用
       /// </summary>
       public const string PhoneUse = "106";
   }
}
