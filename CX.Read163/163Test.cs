using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace VaildTool
{
    public partial class _163Test : Form
    {

        private string emailName;

        public string EmailName
        {
            get { return emailName; }
            set { emailName = value;

            textBox1.Text = value;
            }
        }
        private string emailPW;

        public string EmailPW
        {
            get { return emailPW; }
            set { emailPW = value;

            textBox2.Text = value;
            
            }
        }

        public bool Valid { get; set; }
        public _163Test()
        {
            InitializeComponent();
            Valid = false;
            load163Bor.Url = new Uri("http://mail.163.com/");
        }

        private void Login163Click(object sender, EventArgs e)
        {
            Load163Email();
        }

        private void Load163Email()
        {
            //Thread th = new Thread(new ThreadStart(() =>
            //{
            //    //Thread.Sleep(1500);
            //    this.Invoke(new Action(() =>
            //    {
            //        var userName = load163Bor.Document.GetElementById("idInput");
            //        var pw = load163Bor.Document.GetElementById("pwdInput");
            //        var login = load163Bor.Document.GetElementById("loginBtn");
            //        if (userName != null && pw != null && login != null)
            //        {
            //            userName.SetAttribute("value", textBox1.Text);
            //            pw.SetAttribute("value", textBox2.Text);
            //            login.InvokeMember("click");
            //        }
            //    }));
            //}));
            //th.Start();
        }

        bool isAutoStop = true;

        private void ValidPageLoad(object sender, EventArgs e)
        {

            return;
            var source = System.Configuration.ConfigurationManager.AppSettings["path"] + "Inner/Email.txt";
            var email = File.ReadAllText(source);
            EmailName = email.Split(',')[0];
            EmailPW = email.Split(',')[1];

            valid12306Bor.DocumentText = "等待12306邮件核验";

            this.Text = EmailName;

            load163ContentBor.DocumentCompleted += ReadTrainValidEmailCompleted;
            load163Bor.DocumentCompleted += MainCompleted;
            valid12306Bor.DocumentCompleted += TrainValidCompleted;
            load163Bor.Url = new Uri("http://mail.163.com/");

            var tick = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["tickCount"]);
            Thread th = new Thread(new ThreadStart(() =>
            {
                for (int i = 0; i < tick; i++)
                {
                    if (isAutoStop)
                    {
                        Thread.Sleep(500);
                    }
                }

                if (isAutoStop)
                {
                    Invoke(new Action(() =>
                    {
                        for (int i = 0; i < 1; i++)
                        {
                            SendKeys.Send("{ENTER}");
                            Thread.Sleep(500);
                        }

                        this.Close();
                    }));
                }
            }));

            //th.Start();
        }


        void TrainValidCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (valid12306Bor.DocumentText.Equals("等待12306邮件核验"))
            {
                return;
            }
            if (Valid == false)
            {
                if (valid12306Bor.DocumentText.Contains("邮箱已成功验证，请您重新登录"))
                { 
                    Valid = true;
                    if (textBox3.Text != "核验成功")
                    {
                        ExitEmail();
                        //写入状态
                    }
                    textBox3.Text = "核验成功";
                }
                else
                {
                    MessageBox.Show("IP已经被封");
                }
            }
        }

        void ReadTrainValidEmailCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var addressSuccess = this.load163ContentBor.Document.Body.OuterHtml.Contains("https://kyfw.12306.cn/otn");

            if (addressSuccess && valid12306Bor.DocumentText.Equals("等待12306邮件核验"))
            {
                var soruce = this.load163ContentBor.Document.GetElementsByTagName("a");
                foreach (HtmlElement item in soruce)
                {
                    if (item.InnerText.Contains("https://kyfw.12306.cn/otn//"))
                    {
                        valid12306Bor.Url = new Uri(item.InnerText);
                        DoScript(System.Configuration.ConfigurationManager.AppSettings["dialogScript"]);
                    }
                }
            }
            else 
            {
                ExitEmail();
            }
        }

        void MainCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url.ToString().Equals("http://mail.163.com/"))
            {
                Load163Email();
            }
            textBox3.Text +=DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+":"+ e.Url.ToString() + "\r\n";
         
            if (e.Url.ToString().Contains("http://mail.163.com/js6/read/readhtml.jsp"))
            { 
                load163ContentBor.Url = e.Url;
            }

            string readStr = System.Configuration.ConfigurationManager.AppSettings["loadEmailStr"];

            if (e.Url.ToString().Contains(readStr) && e.Url.ToString().Contains(EmailName))
            {
               ReadEmailClick(null, null);
            }

            if (e.Url.ToString().Contains("http://mail.163.com/logout.htm"))
            {
                Thread th = new Thread(new ThreadStart(() =>
                {
                    Invoke(new Action(() =>
                    {
                        load163ContentBor.DocumentCompleted -= ReadTrainValidEmailCompleted;
                        load163Bor.DocumentCompleted -= MainCompleted;
                          valid12306Bor.DocumentCompleted -= TrainValidCompleted;
                        this.Close();
                    }));
                }));
                th.Start();
            }
        }

        private void ReadEmailClick(object sender, EventArgs e)
        {
            Thread th = new Thread(new ThreadStart(() =>
            {
                ClickEmail();

                var tick = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["readTick"]);

                Thread.Sleep(tick);

                Read12306Email();
            }));

            th.Start();
        }

        private void Read12306EmailClick(object sender, EventArgs e)
        {
            Read12306Email();
        }
    
        private void ExitEmailClick(object sender, EventArgs e)
        {
            ExitEmail();
        }

        /// <summary>
        /// 退出邮件
        /// </summary>
        private bool ExitEmail() 
        {
            //MessageBox.Show("tuichu");
            try
            {
                var exitEle = load163Bor.Document.GetElementById("_mail_component_36_36");
                
                if (exitEle == null)
                {
                    //MessageBox.Show("sdf");
                    exitEle = load163Bor.Document.GetElementById("_mail_component_37_37");
                }
                 if (exitEle == null)
                {
                    //MessageBox.Show("sdf1");
                    Invoke(new Action(() => {

                        this.Close();
                    }));
                    return false;
                }

                exitEle.InvokeMember("click");
                return true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 读取12306邮件
        /// </summary>
        private void Read12306Email()
        {
            Invoke(new Action(() =>
            {
                var source = System.Configuration.ConfigurationManager.AppSettings["readEmailPoint"];
                //var point = load163Bor.PointToScreen(new Point(336, 230));
                DoScript(source);
            }));
          
     //       return;
     //       bool isSuccess = false;

     //       Invoke(new Action(() =>
     //{
     //    var source = load163Bor.Document.GetElementFromPoint(point);

     //    if (source != null)
     //    {
     //        foreach (HtmlElement panel in source.Children)
     //        {
     //            if (isSuccess)
     //                break;
     //            foreach (HtmlElement current in panel.Children)
     //            {
     //                if (current.InnerText.Contains("12306"))
     //                {
     //                    current.InvokeMember("click");
     //                    isSuccess = true;
     //                    break;
     //                }
     //            }
     //        }
     //    }

     //}));
     //       if (isSuccess == false)
     //       {
     //           DoScript("500," + point.X + "," + point.Y);
     //       }
        }

        /// <summary>
        /// 点击收件箱
        /// </summary>
        private void ClickEmail()
        {
            Invoke(new Action(() =>
            {
                var point = load163Bor.PointToScreen(new Point(438, 74));
                DoScript("0," + point.X + "," + point.Y);
            }));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="validResult"></param>
        private void WriteState(bool validResult)
        {
            var source = System.Configuration.ConfigurationManager.AppSettings["path"] + "Inner/Result.txt";
            File.WriteAllText(source, validResult.ToString() + "," + IsStop.ToString());
        }

        private void ValidFormClosing(object sender, FormClosingEventArgs e)
        {
            isAutoStop = false;
            WriteState(Valid);
        }

        private void ReLoad163Click(object sender, EventArgs e)
        {
            load163Bor.Url = new Uri("http://mail.163.com/");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DoScript("100,100,100|200,200,200|300,300,300");
        }

        /// <summary>
        /// 支持脚本
        /// </summary>
        private void DoScript(string script)
        {
            var groupScript = script.Split('|');

            Thread th=new Thread(new ThreadStart(()=>
            {
                foreach (var scriptItem in groupScript)
                {
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

        bool IsStop = false;

        private void CloseValidClick(object sender, EventArgs e)
        {
            IsStop = true;
        }

        private void DeleteIEProcess() 
        { 
            ///关闭IE进程
            //var ss = System.Diagnostics.Process.GetProcessesByName("iexplore");

            //foreach (System.Diagnostics.Process process in ss)
            //{
            //    process.Kill();
            //}
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
