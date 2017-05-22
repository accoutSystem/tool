using AccountRegister;
using Fangbian.Log;
using Fangbian.Ticket.Server.AdvanceLogin;
using Fangbian.Ticket.Server.Trace.Watch;
using Fangbian.Tickets.Trains.Activities;
using MobileValidServer;
using MyTool.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            var ss = SS("梁怡");
            ServerWatch.WatchGuid = "AnalyPassenger_" + System.Windows.Forms.SystemInformation.ComputerName + "_" + System.Environment.TickCount;

            Application.EnableVisualStyles();
          
            Application.SetCompatibleTextRenderingDefault(false);
          
            Application.Run(new Login());
        }

        private static string SS(string ss)
        {
            var cc = ss.ToArray().ToList();
            string fd = string.Empty;
            foreach (var sss in cc)
            {

                fd += getSpell(sss.ToString());
            }
            return fd;
        }
        private static string getSpell(string cnChar)
        {
            byte[] arrCN = Encoding.Default.GetBytes(cnChar);
            if (arrCN.Length > 1)
            {
                int area = (short)arrCN[0];
                int pos = (short)arrCN[1];
                int code = (area << 8) + pos;
                int[] areacode = { 45217, 45253, 45761, 46318, 46826, 47010, 47297, 47614, 48119, 48119, 49062, 49324, 49896, 50371, 50614, 50622, 50906, 51387, 51446, 52218, 52698, 52698, 52698, 52980, 53689, 54481 };

                for (int i = 0; i < 26; i++)
                {
                    int max = 55290;
                    if (i != 25)
                    {
                        max = areacode[i + 1];
                    }
                    if (areacode[i] <= code && code < max)
                    {
                        return Encoding.Default.GetString(new byte[] { (byte)(97 + i) });
                    }
                }
                return "*";
            }
            else return cnChar;
        }  
    }
}
