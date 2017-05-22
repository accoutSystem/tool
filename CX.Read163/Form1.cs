using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using VaildTool;

namespace CX.Read163
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left; //最左坐标
            public int Top; //最上坐标
            public int Right; //最右坐标
            public int Bottom; //最下坐标
        }
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);  

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
         //|1000,1147,305|1000,1147,305|1000,1147,305|1000,1467,305

            IntPtr awin = FindWindow(null, "屏幕键盘");
            if (awin == null) {
                Process.Start(@"C:\Windows\System32\osk.exe");
                Thread.Sleep(1000);
            }
            awin = FindWindow(null, "屏幕键盘");
            RECT rc = new RECT();
            var ss=  GetWindowRect(awin, ref rc);
            int width = rc.Right - rc.Left; //窗口的宽度
            int height = rc.Bottom - rc.Top; //窗口的高度
            int x = rc.Left;
            int y = rc.Top;


            string v1 = "10,520,620|500," + (x + 130) + "," + (y + 88) + "|";
            string v2 = "10," + (x + 448) + "," + (y + 88) + "|";
            string v3 = "10," + (x + 407) + "," + (y + 88) + "|";
            string v4 = "10," + (x + 448) + "," + (y + 88) + "|";
            string v5 = "10," + (x + 375) + "," + (y + 88) + "|";
            string v6 = "10," + (x + 221) + "," + (y + 88)  ;
            DoScript(v1 + v2 + v3 + v4 + v5 + v5);
        }

         
        

        /// <summary>
        /// 支持脚本
        /// </summary>
        private void DoScript(string script)
        {
            var groupScript = script.Split('|');

            Thread th = new Thread(new ThreadStart(() =>
            {
                foreach (var scriptItem in groupScript)
                {
                    if (string.IsNullOrEmpty(scriptItem))
                        continue;
                    var excute = scriptItem.Split(',');
                    Thread.Sleep(Convert.ToInt32(excute[0]));
                    Invoke(new Action(() =>
                    {
                        Point point = new Point(Convert.ToInt32(excute[1]), Convert.ToInt32(excute[2]));
                        MouseFlag.SetCursorPos(point.X, point.Y);
                        MouseFlag.MouseLefDownEvent(point.X, point.Y, 0);
                        MouseFlag.MouseLeftUPEvent(point.X, point.Y, 0);
                    }));
                }
            }));
            th.Start();

        }
    }
}
