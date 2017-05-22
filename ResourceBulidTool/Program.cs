using AccountRegister;
using Fangbian.Log;
using Fangbian.Ticket.Server.AdvanceLogin;
using Fangbian.Ticket.Server.Trace.Watch;
using Fangbian.Tickets.Trains.Activities;
using MobileValidServer;
using MobileValidServer.PassCodeProvider;
using MyTool.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ResourceBulidTool
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //var userName = System.Configuration.ConfigurationManager.AppSettings["lhQZ"]; 
            //var between = System.Configuration.ConfigurationManager.AppSettings["lhBetween"].Split(',');
            //var min = Convert.ToInt32(between[0]);
            //var max = Convert.ToInt32(between[1]);
            //Random r = new Random();
            //var value = r.Next(min, max).ToString();
            //var maxValue = max.ToString();
            //var addZero = maxValue.Length - value.Length;
            //for (int i = 0; i < addZero; i++)
            //{
            //    userName += "0";
            //}
            //userName += value;
            //Console.WriteLine(userName);
            //var source = CXDataCipher.EncryptionUserPW("CQY244167");

            //var data = CXDataCipher.DecipheringUserPW(source);
            ServerWatch.WatchGuid = "Register_" + System.Windows.Forms.SystemInformation.ComputerName + "_" + System.Environment.TickCount;

            Application.EnableVisualStyles();

            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new Login());
        }

        public static string ReadValidCode(string validCode)
        {
            //USER&23.184&30&30&30&0.98&0.058[End]MSG&1553&13629674041&【铁路客服】12306用户注册或既有用户手机核验专用验证码：772700。如非本人直接访问12306，请停止操作，切勿将验证码提供给第三方。[End]STATE&1553&13629674041&失败,发送超时[End]

            Logger.Info(validCode);
            var valid = validCode;

            var index = valid.IndexOf("验证码：");

            valid = valid.Substring(index, valid.Length - index);

            valid = valid.Replace("验证码：", "");

            valid = valid.Substring(0, valid.IndexOf("。"));

            valid = valid.Replace("。", "");

            return valid;
        }
    }
}
