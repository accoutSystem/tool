using MyTool.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace BulidTaskServer
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {

            //var item = "1CD";
            ////如果是生僻身份证的话且开始字符是数字就需要替换为字符串
            //string str = string.Empty;
            //foreach (var c in item)
            //{
            //    if (Convert.ToInt32(c) >= 49 && Convert.ToInt32(c) <= 57)
            //    {
            //        str += "A";
            //    }
            //    else
            //    {
            //        str += c;
            //    }
            //}
            //var birthday = Convert.ToDateTime(ToolCommonMethod.GetBirthdayIDNoTo("430121198911207314"));

            //var ss = (DateTime.Now - birthday);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new BulidTaskPage());
        }
    }
}
