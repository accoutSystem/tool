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

        private string baseUrl = "http://api.83r.com:20153/User/";

        public string ServerId = "1553";

        public static string Token { get; set; }

        public FMPassCodeProvider()
        {
            if (request == null)
            {
                request = new ValidPlatformRequest();
            }
        }

        public override string Login(string user, string password)
        {
            string url = string.Format("{2}login?uName={0}&pWord={1}&Developer=ZuPdyMJ9cpu%2fKKgPz0wR7w%3d%3d", user, password, baseUrl);
            var source = request.CreateGetRequest(url);
            return source;
        }

        public override bool LoginSuccess(string result)
        {
            if (string.IsNullOrEmpty(result) || result.Contains("用户名"))
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
                CurrentUser.State = UserState.余额不足;
                return string.Empty;
            }
            if (phone.Contains(";"))
            {
                return phone.Split(';')[0];
            }

            CurrentUser.State = UserState.获取手机号码失败;

            return string.Empty;
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
                CurrentUser.Message = validPassCode;

                WriteLog("获取验证码" + validPassCode);

                if (phone.Contains("余额不足"))
                {
                    CurrentUser.State = UserState.余额不足;
                    return string.Empty;
                }

                if (validPassCode.Contains("验证码")||validPassCode.Contains("12306"))
                {
                    return request.ReadValidCode(validPassCode);
                }

                i++;

                if (i > 6)
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
    }
}