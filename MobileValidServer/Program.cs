using Fangbian.Data.Client;
using Fangbian.Log;
using Fangbian.Ticket.Server.Trace.Watch;
using Fangbian.Tickets.Trains.Activities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobileValidServer
{
    static class Program
    {

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
           
            SystemConfigClient.GetAgent = "http://121.43.110.247/analog.ashx";
            ServerWatch.WatchGuid = System.Environment.TickCount.ToString();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Login());
        }
    }
}
