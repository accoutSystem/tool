using ChangePassWord.PassCode;
using Common.Common;
using Fangbian.Data.Client;
using Fangbian.Log;
using Fangbian.Tickets.Trains.Activities;
using FangBian.Common;
using Maticsoft.Model;
using MyTool.Common;
using MyTool.Valid;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using YDMCSDemo;

namespace MyTool
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //初始化云打码

            Login();
 
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ToolMain());
        }

        public static void Login()
        {
            YDMWrapper.YDM_SetAppInfo(2706, "4f635fb9a5dd14fb13b3e65d22b1473b");

            var ret = YDMWrapper.YDM_Login("lichao7314", "lichao");

            if (ret > 0)
            {
                Console.WriteLine("登录成功");
            }
            else
            {
                MessageBox.Show("登陆失败，错误代码：" + ret.ToString(), "登陆失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
