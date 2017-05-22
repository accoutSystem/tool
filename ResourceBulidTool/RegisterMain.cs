using Fangbian.Common;
using Fangbian.Data.Client;
using Fangbian.Ticket.Server.Trace.Watch;
using Fangbian.Tickets.Trains;
using Fangbian.Tickets.Trains.DataTransferObject.Request;
using Fangbian.Tickets.Trains.DataTransferObject.Response.Login;
using Fangbian.Tickets.Trains.WFDataItem;
using FangBian.Common;
using MyEntiry;
using Newtonsoft.Json.Linq;
using ResourceBulidTool;
using ResourceBulidTool.Properties;
using System;
using System.Activities;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace AccountRegister
{
    public partial class RegisterMain : Form
    {
        public static string Path = Environment.CurrentDirectory + @"\Flow\"; // @"C:\Users\lichao\Desktop\工作流\Test\";

        private AutoResetEvent orderResetEvent = new AutoResetEvent(false);

        /// <summary>
        /// 开始注册用户
        /// </summary>
        bool isStartUser = false;

        public RegisterMain()
        {
            InitializeComponent();
            Load += Form1_Load;
        }

        void Form1_Load(object sender, EventArgs e)
        {
            var s = System.Configuration.ConfigurationManager.AppSettings["taskNumber"];
            Watch();
            this.Text = "Resource Bulid->" + ServerWatch.WatchGuid;
        }


        #region 注册用户
        private void RegisterUserClick(object sender, EventArgs e)
        {
            if (isStartUser)
            {
                isStartUser = false;
                btnRegisterUser.Image = Resources.start;
                btnRegisterUser.Text = "开始注册";
                return;
            }
            else
            {
                isStartUser = true;
                btnRegisterUser.Image = Resources.stop;
                btnRegisterUser.Text = "停止注册";
            }
            WriteMessage("开始初始化Register");
            var maxTaskNumber = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["taskNumber"]);

            Thread th = new Thread(new ThreadStart(() =>
            {
                while (isStartUser)
                {
                    if (CommonMethod.IsProvideServer())
                    {
                        if (RegisterUserCollection.Current.Count < maxTaskNumber)
                        {
                            T_Email emailItem = null;
                            while (emailItem == null)
                            {
                                emailItem = GetEmailServer.Get();
                                if (emailItem == null)
                                {
                                    WriteMessage("邮件为空");
                                    Thread.Sleep(2000);
                                }
                            } 
                            T_Passenger passengerItem = null;
                            while (passengerItem == null)
                            {
                                passengerItem = GetPassengerServer.Get();

                                if (passengerItem == null)
                                {
                                    WriteMessage("联系人为空");
                                    Thread.Sleep(2000);
                                }
                            }
                             

                            if (emailItem == null || passengerItem == null)
                            {
                                WriteMessage("注册用户->邮箱和乘车人数据为空已经");

                                Thread.Sleep(2000);

                                continue;
                            }
                            try
                            {
                                WriteMessage("开始Register"+passengerItem.Name+" "+emailItem.Email);

                                RegisterUser user = new RegisterUser();

                                user.Register(passengerItem, emailItem);

                                user.OutputMessage += user_OutputMessage;

                                user.RegisterUserCompleted += user_RegisterUserCompleted;

                                RegisterUserCollection.Current.Add(user);

                                UpdateTaskNumber();
                            }
                            catch
                            {

                            }
                        }
                    }
                    else
                    {
                        WriteMessage("12306系统维护阶段");
                        Thread.Sleep(30000);
                    }
                    Thread.Sleep(1000);


                }
            }));
            th.IsBackground = false;
            th.Start();
        }

        void user_RegisterUserCompleted(object sender, RegisterUserEventArgs e)
        {
            RegisterUserCollection.Current.Remove(sender as RegisterUser);

            if (e.Success)
                runInfo.SuccessCount++;
            else
                runInfo.ErrorCount++;
            UpdateRunInfo();
            UpdateTaskNumber();

        }

        void user_OutputMessage(object sender, Fangbian.DataStruct.Business.ConsoleMessageEventArgs e)
        {
            WriteMessage("Register User:" + e.Message);
        }
        #endregion


        int count = 0;
        private void WriteMessage(string message)
        {
            this.Invoke(new Action(() =>
            {
                count++;

                if (count == 200)
                {
                    count = 0;
                    textBox1.Text = string.Empty;
                }
                if (message.Length > 200)
                {
                    message = message.Substring(0, 200);
                }
                textBox1.Text += "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":" + message;
                textBox1.SelectionStart = this.textBox1.Text.Length;
                textBox1.ScrollToCaret();
            }));
        }

        private void UpdateTaskNumber()
        {
            this.Invoke(new Action(() =>
            {
                this.toolStripStatusLabel13.Text = "任务数:" + RegisterUserCollection.Current.Count + string.Empty;
            }));
        }

        private void UpdateRunInfo()
        {

            this.Invoke(new Action(() =>
            {

                this.lbRunInfo.Text = "注册成功:" + runInfo.SuccessCount + " 注册失败:" + runInfo.ErrorCount + " 总数" + (runInfo.ErrorCount + runInfo.SuccessCount);
            }));
        }

        private void toolStripStatusLabel13_Click(object sender, EventArgs e)
        {
            string message = "正在注册:\r\n";
            RegisterUserCollection.Current.ForEach(item =>
            {
                message += item.currentPassengerItem.Name + " " + item.currentPassengerItem.IdNo + " 创建时间:" + item.BulidTime.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n";
            });
            MessageBox.Show(message);
        }

        #region 对服务进行监控和接收指令进行下发
        /// <summary>
        /// 上传监视数据
        /// </summary>
        public void Watch()
        {
            Console.WriteLine("开启监视");

            Thread thread = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    if (CommonMethod.IsProvideServer())
                    {
                        try
                        {
                            GetServerInstruct();

                            UpLoadWatch();

                            Thread.Sleep(5000);
                        }
                        catch
                        {
                            Thread.Sleep(5000);
                            // SystemException.AddException("上传监控数据失败:" + ex.StackTrace);
                        }
                    }
                    else {
                        Thread.Sleep(5000);
                    }
                }
            }));

            thread.IsBackground = true;

            thread.Start();
        }

        private void UpLoadWatch()
        {
            Console.WriteLine("上传机器运行数据中....");

            StringBuilder message = new StringBuilder();

            message.Append(ServerWatch.WatchGuid + "机器:刷新时间:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            message.Append(" 操作状态:" + (isStartUser ? "启动" : "关闭"));

            message.Append(" 任务数当前:" + RegisterUserCollection.Current.Count.ToString());

            ServerWatch.UpLoadMachineWatch(message.ToString(), "lcregisterUser" + ServerWatch.WatchGuid);
        }

        private void GetServerInstruct()
        {
            Console.WriteLine("开始读取控制指令");
        }
        #endregion

        RunInfo runInfo = new RunInfo();

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            this.ShowInTaskbar = false;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            this.ShowInTaskbar = true;

        }
    }

    public class RunInfo
    {
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
    }
}
