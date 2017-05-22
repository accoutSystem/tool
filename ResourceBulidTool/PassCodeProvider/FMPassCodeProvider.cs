﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MobileValidServer.PassCodeProvider
{
    /// <summary>
    /// 飞码
    /// </summary>
    public class FMPassCodeProvider : BasePassCodeProvider
    {
        public static ValidPlatformRequest request = null;

        private string baseUrl = "http://xapi.83r.com/User/";

        public string ServerId = "1553";

        public  string Token { get; set; }

        public string UserName { get; set; }

        public string PW { get; set; }
        public FMPassCodeProvider()
        {
            if (request == null)
            {
                request = new ValidPlatformRequest();
            }
        }

        public override string Login(string user, string password)
        {
            UserName = user;
            PW = password;
            string url = string.Format("{2}login?uName={0}&pWord={1}&Developer=ZuPdyMJ9cpu%2fKKgPz0wR7w%3d%3d", user, password, baseUrl);
            var source = request.CreateGetRequest(url);
            return source;
        }

        public override bool LoginSuccess(string result)
        {
            if (string.IsNullOrEmpty(result) 
                || result.Contains("用户名")
                ||result.Contains("False"))
                return false;
            Token = result;
            return true;
        }

        public override string GetMobilePhone()
        {
            string url = string.Format("{0}getPhone?ItemId={1}&token={2}", baseUrl, ServerId, Token);

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
                return string.Empty;
            }
            if (phone.Contains("过期")&&phone.Contains("Session")) 
            {
                while (true)
                {
                    var s = Login(UserName, PW);
                    if (LoginSuccess(s))
                        break;
                }
            }

            if (phone.Contains(";"))
            {
                string currentPhone =phone.Split(';')[0];

                if (IsMobilePhone(currentPhone))
                {
                    return currentPhone;
                }
            }
            return string.Empty;
        }

        public string SendSMS(string phone) {

            string url = string.Format("{0}sendMessage?token={1}&Phone={2}&ItemId={3}&Msg={4}", baseUrl, Token,
                phone,ServerId,"999");
           var  validPassCode = request.CreateGetRequest(url);
           return validPassCode;
        }

        public override string GetValidCode(string phone)
        {
            string url = string.Format("{0}getMessage?token={1}", baseUrl, Token);

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

                WriteLog("获取验证码" + validPassCode);

                if (validPassCode.Contains("余额不足"))
                {
                    return string.Empty;
                }

                if (validPassCode.Contains("过期") && validPassCode.Contains("Session"))
                {
                    while (true)
                    {
                        var s = Login(UserName, PW);
                        if (LoginSuccess(s))
                            break;
                    }
                }

                if (validPassCode.Contains("验证码")||validPassCode.Contains("12306"))
                {
                    return request.ReadValidCode(validPassCode);
                }

                i++;

                if (i > 25)
                {
                    break;
                }
            }

            return value;
        }

        public override void DisposePhone(string phone)
        {
            string url = string.Format("{0}releasePhone?token={1}&phoneList={3}-{2};", baseUrl, Token, ServerId, phone);
            var source = request.CreateGetRequest(url);
            WriteLog("释放手机" + source);
        }

        public override void AddBlackPhone(string phone, string reason)
        {
            string url = string.Format("{0}addBlack?token={1}&phoneList={2}-{3};", baseUrl, Token, ServerId, phone);
            var source = request.CreateGetRequest(url);
            WriteLog("添加黑名单" + source);
        }

        internal void Exit()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("退出飞码登录,Token="+Token);
            Console.ForegroundColor = ConsoleColor.White;
            string url = string.Format("{0}exit?token={1}", baseUrl, Token);
            var source = request.CreateGetRequest(url);
        }
    }
}