using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Diagnostics;
namespace VaildTool
{
    public class ADSLConnection
    {

        public static void Connection()
        {
            ExcuteCmd();
        }

        private static void ExcuteCmd()
        {

            System.Diagnostics.Process proc = new Process();
            proc.StartInfo.FileName = System.Environment.CurrentDirectory + @"\connection.bat";
            proc.StartInfo.CreateNoWindow = false;
            proc.Start();
            proc.WaitForExit();
        }
    }
}
