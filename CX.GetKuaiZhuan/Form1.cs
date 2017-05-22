using MyTool.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace CX.GetKuaiZhuan
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            webBrowser1.Url = new Uri("http://bbs.51kwz.com/reg.php?uid=2490");
        }

        private void button2_Click(object sender, EventArgs e)
        {
         tbPhone.Text = ToolCommonMethod.GetPhoneSegment() + ToolCommonMethod.GetRandom(8);
         tbPW.Text = ToolCommonMethod.GetRandom(6);
        
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var userName = webBrowser1.Document.GetElementById("username");
            var pw = webBrowser1.Document.GetElementById("password");
            userName.SetAttribute("value", tbPhone.Text);
            pw.SetAttribute("value", tbPW.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Point point = new Point(Convert.ToInt32(476), Convert.ToInt32(311));
            MouseFlag.SetCursorPos(point.X, point.Y);
            MouseFlag.MouseLefDownEvent(point.X, point.Y, 0);
            MouseFlag.MouseLeftUPEvent(point.X, point.Y, 0);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var source = PD.Business.DataTransaction.Create();
            source.ExecuteSql("insert into klz_user(userName,password,createtime) values('" + tbPhone.Text + "','" + tbPW.Text + "','"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+"')");
            ADSLConnection.Connection();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ADSLConnection.Connection();
        }
    }

    public class ADSLConnection
    {

        public static void Connection()
        {
            ExcuteCmd();
        }

        private static void ExcuteCmd()
        {
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = System.Environment.CurrentDirectory + @"\connection.bat";
            proc.StartInfo.CreateNoWindow = false;
            proc.Start();
            proc.WaitForExit();
        }
    }
    public class MouseFlag
    {

        [DllImport("user32.dll")]
        public static extern int SetCursorPos(int x, int y);

        [DllImport("user32.dll")]

        static extern void mouse_event(MouseEventFlag flags, int dx, int dy, uint data, UIntPtr extraInfo);

        [Flags]
        enum MouseEventFlag : uint
        {
            Move = 0x0001,
            LeftDown = 0x0002,
            LeftUp = 0x0004,
            RightDown = 0x0008,
            RightUp = 0x0010,
            MiddleDown = 0x0020,
            MiddleUp = 0x0040,
            XDown = 0x0080,
            XUp = 0x0100,
            Wheel = 0x0800,
            VirtualDesk = 0x4000,
            Absolute = 0x8000
        }
        public static void MouseLefDownEvent(int dx, int dy, uint data)
        {
            mouse_event(MouseEventFlag.LeftDown, dx, dy, data, UIntPtr.Zero);
        }
        public static void MouseLeftUPEvent(int dx, int dy, uint data)
        {
            mouse_event(MouseEventFlag.LeftUp, dx, dy, data, UIntPtr.Zero);
        }

        public static void MouseMove(int dx, int dy)
        {
            SetCursorPos(dx, dy);
        }
    }  
}
