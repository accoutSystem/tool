using FangBian.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using VaildTool;

namespace EmailReadTool
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //ADSLConnection.Connection();
          
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new EmailRead());
        }

    
    }
}
