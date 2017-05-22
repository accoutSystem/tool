using Fangbian.Tickets.Trains.Activities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace AddPassengerServer
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            BoxConnectionManager.ConnectionString = "121.43.110.247:1234:2";

            BoxConnectionManager.Get();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AddPassenger());
        }
    }
}
