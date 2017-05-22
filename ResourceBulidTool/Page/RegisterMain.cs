using CaptchaServerCacheClient;
using Fangbian.Common;
using Fangbian.Data.Client;
using Fangbian.Ticket.Server.AdvanceLogin;
using Fangbian.Ticket.Server.Trace.Watch;
using Fangbian.Tickets.Trains;
using Fangbian.Tickets.Trains.DataTransferObject.Request;
using Fangbian.Tickets.Trains.DataTransferObject.Response.Login;
using Fangbian.Tickets.Trains.WFDataItem;
using FangBian.Common;
using MobileValidServer;
using MobileValidServer.PassCodeProvider;
using MyEntiry;
using MyTool.Common;
using Newtonsoft.Json.Linq;
using ResourceBulidTool;
using ResourceBulidTool.Entity;
using ResourceBulidTool.LoginPool;
using ResourceBulidTool.Page;
using ResourceBulidTool.Properties;
using ResourceBulidTool.Register;
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
        public static RegisterMain Current { get; set; }

        public static string Path = Environment.CurrentDirectory + @"\Flow\"; // @"C:\Users\lichao\Desktop\工作流\Test\";

        private AutoResetEvent orderResetEvent = new AutoResetEvent(false);

        Dictionary<BaseRegisterUser, DataGridViewRow> controls = new Dictionary<BaseRegisterUser, DataGridViewRow>();

        public static BulidType BulidType { get; set; }

        /// <summary>
        /// 开始注册用户
        /// </summary>
        bool isStartUser = false;

        UserPoolManager pool = new UserPoolManager();

        public int TaskNumber { get; set; }

        /// <summary>
        /// 是否开启添加账号联系人的检查
        /// </summary>
        public bool IsCheckIdNo { get; set; }

        public RegisterMain()
        {
            InitializeComponent();

            AccountActivationPool.Current.ItemChange += Current_ItemChange;

            Current = this;
        }

        private void RegisterMain_Load(object sender, EventArgs e)
        {
            StartWatch();

            BulidType = ResourceBulidTool.BulidType.Default;

            tbTaskNumber.Text = TaskNumber.ToString();

            Text = "资源构建->" + ServerWatch.WatchGuid + " 短信平台:" + PassCodeProviderFactory.GetName() + " 任务数:" + TaskNumber + " 通道:" + CurrentSystemType.Type;

            if (IsCheckIdNo == false)
            {
                toolStripButton6.Visible = false;
            }
        }

        void Current_ItemChange(object sender, EventArgs e)
        {
            Invoke(new Action(() =>
            {
                lbCheck.Text = AccountActivationPool.Current.Count + string.Empty;
            }));
        }

        #region 注册用户
        private void RegisterUserClick(object sender, EventArgs e)
        {
            if (isStartUser)
            {
                isStartUser = false;
                btnRegisterUser.Image = Resources.start;
                btnRegisterUser.Text = "开始";
                foreach (var item in RegisterUserCollection.Current)
                {
                    item.IsActivity = false;
                }
                return;
            }
            else
            {

                isStartUser = true;
                btnRegisterUser.Image = Resources.stop;
                btnRegisterUser.Text = "停止";
            }
            WriteMessage("开始初始化Register");

            SelectBulidType select = new SelectBulidType();

            select.ShowDialog();

            BulidType = select.BulidType;

            switch (BulidType)
            {
                case ResourceBulidTool.BulidType.Continuity: this.Text += "-连号"; break;
                case ResourceBulidTool.BulidType.AppointUser: this.Text += "-指定号"; break;
                case ResourceBulidTool.BulidType.Default: this.Text += "-默认"; break;
                case ResourceBulidTool.BulidType.PW: this.Text += "-密码一致账号"; break;
            }


            if (BulidType == ResourceBulidTool.BulidType.AppointUser)
            {
                btnRegisterUser.Enabled = false;
                toolStripButton3.Visible = true;
            }

            Thread th = new Thread(new ThreadStart(() =>
            {
                while (isStartUser)
                {
                    if (ToolCommonMethod.IsProvideServer())
                    {
                        if (RegisterUserCollection.Current.Count < TaskNumber)
                        {
                            T_Email emailItem = GetEmail();

                            T_Passenger passengerItem = GetPassenger();

                            CustomUserItem user = null;

                            if (BulidType == ResourceBulidTool.BulidType.AppointUser  )
                            {
                                user = CustomUserCollection.Current.Get();

                                if (user == null)
                                {
                                    if (CustomUserCollection.Current.Completed())
                                    {
                                        this.Invoke(new Action(() =>
                                        {
                                            RegisterUserClick(null, EventArgs.Empty);
                                            btnRegisterUser.Enabled = true;
                                            toolStripButton3.Visible = false;
                                        }));

                                        break;
                                    }
                                    continue;
                                } 
                                user.State = 2;
                            }
                            Thread.Sleep(1800);
                            BulidUser(emailItem, passengerItem, user != null ? user.UserName : string.Empty, user != null ? user.PassWord : string.Empty);
                        }
                    }
                    else
                    {
                        RegisterUserCollection.Current.ForEach(item =>
                        {
                            if (item.IsActivity)
                            {
                                item.IsActivity = false;
                            }
                        });

                        if (RegisterUserCollection.Current.Count == 0 && AccountActivationPool.Current.Count > 0)
                        {
                            AccountActivationPool.Current.ClearAccount();
                        }

                        WriteMessage("12306系统维护阶段");

                        Thread.Sleep(30000);
                    }
                    Thread.Sleep(1000);
                }
            }));

            th.IsBackground = true;

            th.Start();
        }

        private void BulidUser(T_Email emailItem, T_Passenger passengerItem, string userName, string passWord)
        {
            try
            {
                WriteMessage("开始Register" + passengerItem.Name + " " + emailItem.Email);

                BaseRegisterUser user = null;

                if (CurrentSystemType.Type == SystemRegisterType.手机通道)
                {
                    user = new MobileRegisterUser();
                }
                if (CurrentSystemType.Type == SystemRegisterType.PC通道)
                {
                    user = new PCRegisterUser();
                }

                user.IsCheckIdNo = IsCheckIdNo;

                user.UserName = userName;

                user.PassWord = passWord;

                user.Register(passengerItem, emailItem);

                user.OutputMessage += BulidUserOutputMessage;

                user.RegisterUserCompleted += BulidUserCompleted;

                RegisterUserCollection.Current.Add(user);

                AddRow(user);

                user.PropertyChange += user_PropertyChange;

                //user

                UpdateTaskNumber();
            }
            catch
            {

            }
        }

        void user_PropertyChange(object sender, PropertyChangeEventArgs e)
        {
            var item = sender as BaseRegisterUser;
            if (controls.ContainsKey(item))
            {
                this.Invoke(new Action(() =>
                {
                    switch (e.PropertyName)
                    {
                        case "BulidTime": controls[item].Cells[2].Value = e.Value; break;
                        case "LastOperationTime": controls[item].Cells[3].Value = e.Value; break;
                        case "IsPlatformPhone":
                            if (item.IsPlatformPhone)
                            {
                                controls[item].Cells[1].Value = item.RegisterInfo.MobileNo;
                            }
                            else
                            {
                                controls[item].Cells[1].Value = string.Empty;
                            }
                            break;
                        case "LinkUser": controls[item].Cells[9].Value = e.Value; break;
                        case "UserName":
                            var user = (e.Value as RegisterUserInfo);
                            if (user != null)
                            {
                                controls[item].Cells[0].Value = user.UserName;
                            }
                            else
                            {
                                controls[item].Cells[0].Value = string.Empty;
                                controls[item].Cells[0].Value = string.Empty;
                            }
                            break;
                        case "CurrentRegisterPassenger":
                            controls[item].Cells[4].Value = (e.Value as T_Passenger).Name;
                            controls[item].Cells[5].Value = (e.Value as T_Passenger).IdNo; break;
                        case "CurrentRegisterEmail":
                            controls[item].Cells[6].Value = (e.Value as T_Email).Email; break;
                        case "Message":
                            controls[item].Cells[7].Value = e.Value;
                            break; 
                        case "TaskRegisterCount":
                            controls[item].Cells[8].Value = e.Value;
                            break;


                    }
                }));
            }
        }

        private void AddRow(BaseRegisterUser user)
        {
            Invoke(new Action(() =>
            { 
                var index = dataGridView1.Rows.Add();

                dataGridView1.Rows[index].Cells[0].Value = user.UserName;
                dataGridView1.Rows[index].Cells[1].Value = string.Empty;
                dataGridView1.Rows[index].Cells[2].Value = user.BulidTime.ToString("yyyy-MM-dd HH:mm:ss");
                dataGridView1.Rows[index].Cells[3].Value = user.LastOperationTime.ToString("yyyy-MM-dd HH:mm:ss");
                dataGridView1.Rows[index].Cells[4].Value = user.CurrentRegisterPassenger.Name;
                dataGridView1.Rows[index].Cells[5].Value = user.CurrentRegisterPassenger.IdNo;
                dataGridView1.Rows[index].Cells[6].Value = user.CurrentRegisterEmail.Email;
                dataGridView1.Rows[index].Cells[7].Value = user.Message;
                dataGridView1.Rows[index].Cells[8].Value = user.TaskRegisterCount;
                dataGridView1.Rows[index].Cells[9].Value = user.LinkUser;

                controls.Add(user, dataGridView1.Rows[index]);
            }));
        }

        public T_Email GetEmail()
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
            return emailItem;
        }

        public T_Passenger GetPassenger()
        {
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
            return passengerItem;
        }

        private void BulidUserCompleted(object sender, RegisterUserEventArgs e)
        {
            var bulidUser = sender as BaseRegisterUser;

            RegisterUserCollection.Current.Remove(bulidUser);

            Invoke(new Action(() =>
            {
                if (controls.ContainsKey(bulidUser))
                {
                    dataGridView1.Rows.Remove(controls[bulidUser]);
                    controls.Remove(bulidUser);
                }
            }));

            var currentItem = CustomUserCollection.Current.FirstOrDefault(item => item.UserName.Equals(bulidUser.UserName));

            if (e.Success)
            {
                runInfo.SuccessCount++;
                 //|| BulidType == ResourceBulidTool.BulidType.Continuity
                if (BulidType == ResourceBulidTool.BulidType.AppointUser)
                {
                    currentItem.State = 1;
                }
            }
            else
            {
                Fangbian.Log.Logger.Error("提交验证码环节失败:" + e.Message);
                 //|| BulidType == ResourceBulidTool.BulidType.Continuity
                if (BulidType == ResourceBulidTool.BulidType.AppointUser)
                {
                    if ((e.Message + string.Empty).Contains("用户名已被注册"))
                    {
                        currentItem.State = 4;
                    }
                    else
                    {
                        currentItem.State = 0;
                    }
                }
                runInfo.ErrorCount++;
            }

            UpdateRunInfo();

            UpdateTaskNumber();
        }

        private void BulidUserOutputMessage(object sender, Fangbian.DataStruct.Business.ConsoleMessageEventArgs e)
        {
            WriteMessage("Register User:" + e.Message);
        }
        #endregion

        int count = 0;

        public void WriteMessage(string message)
        {
            if (isShowLog)
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
                if (BulidType == ResourceBulidTool.BulidType.AppointUser || BulidType == ResourceBulidTool.BulidType.Continuity)
                {
                    int sys = CustomUserCollection.Current.Count(item => item.State == 0);
                    lbRunInfo.Text += "==自定义总数:" + CustomUserCollection.Current.Count;
                    lbRunInfo.Text += " 剩余:" + sys;
                    lbRunInfo.Text += " 成功:" + CustomUserCollection.Current.Count(item => item.State == 1);
                    lbRunInfo.Text += " 正在使用:" + CustomUserCollection.Current.Count(item => item.State == 2);
                    lbRunInfo.Text += " 失败:" + CustomUserCollection.Current.Count(item => item.State == 3);
                    lbRunInfo.Text += " 已被注册:" + CustomUserCollection.Current.Count(item => item.State == 4);
                }
            }));
        }

        private void toolStripStatusLabel13_Click(object sender, EventArgs e)
        {
            string message = "正在注册:\r\n";
            RegisterUserCollection.Current.ForEach(item =>
            {
                message += item.CurrentRegisterPassenger.Name 
                    + " " + item.CurrentRegisterPassenger.IdNo 
                    + " 创建时间:" + item.BulidTime.ToString("yyyy-MM-dd HH:mm:ss")
                    + " 最后操作时间:" + item.LastOperationTime.ToString("yyyy-MM-dd HH:mm:ss")
                      + " 当前消息:" + item.Message + "\r\n";
            });
            ShowMessage ss = new ShowMessage();
            ss.Text = message;
            ss.ShowDialog();
            //MessageBox.Show(message);
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
                    else
                    {
                        Thread.Sleep(30000);
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

        private void RunInfoClick(object sender, EventArgs e)
        {
            runInfo.SuccessCount = runInfo.ErrorCount = 0;
            UpdateRunInfo();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            T_Email emailItem = GetEmail();

            T_Passenger passengerItem = GetPassenger();

            BulidUser(emailItem, passengerItem, string.Empty, string.Empty);
        }

        bool isShowLog = true;
        private void LogClick(object sender, EventArgs e)
        {
            if (isShowLog)
            {
                isShowLog = false;
                toolStripButton5.Text = "启用日志";
                toolStripButton5.Image = Resources.start;
            }
            else
            {
                isShowLog = true;
                toolStripButton5.Text = "禁用日志";
                toolStripButton5.Image = Resources.stop;
            }
        }

        private void lbCheck_Click(object sender, EventArgs e)
        {
            ValidUserPage page = new ValidUserPage();
            page.ShowDialog();
        }

        private void StartFillClick(object sender, EventArgs e)
        {
            if (pool.Run)
            {
                pool.Stop();
                toolStripButton6.Text = "开始填充";
                toolStripButton6.Image = Resources.start;
                return;
            }
            else
            {
                toolStripButton6.Text = "停止填充";
                toolStripButton6.Image = Resources.stop;
                pool.Start(TaskNumber);
            }
        }

        private void UpdateConnectionClick(object sender, EventArgs e)
        {
            CX.Config.SetConfig config = new CX.Config.SetConfig();
            config.ShowDialog();
        }

        private void toolStripStatusLabel2_Click(object sender, EventArgs e)
        {
            runInfo.SuccessCount = runInfo.ErrorCount = 0;
        }

        private void UpdateTaskNumberClick(object sender, EventArgs e)
        {
            pool.MaxTaskNumber = TaskNumber = Convert.ToInt32(tbTaskNumber.Text);
        }

        private bool isWatch = false;

        private void StartWatch()
        {
            if (isWatch)
                return;
            isWatch = true;

            CacheClient client = new CacheClient() { };

            client.Url = System.Configuration.ConfigurationManager.AppSettings["cacheServer"];

            Thread th = new Thread(new ThreadStart(() =>
            {
                while (isWatch)
                {
                    JObject watch = new JObject();
                    watch.Add("注册线程数", TaskNumber);
                    watch.Add("乘车人账号填充情况", pool.Run ? "填充中" : "未填充");
                    watch.Add("是否注册", isStartUser ? "注册中" : "未注册");
                    watch.Add("注册成功数", runInfo.SuccessCount);
                    watch.Add("注册失败数", runInfo.ErrorCount);
                    watch.Add("注册总数", runInfo.ErrorCount + runInfo.SuccessCount);
                    watch.Add("正在执行任务数", RegisterUserCollection.Current.Count);
                    watch.Add("所用平台", PassCodeProviderFactory.GetName());

                    client.Add(ServerWatch.WatchGuid, Newtonsoft.Json.JsonConvert.SerializeObject(watch), 60);

                    Console.WriteLine("上传监视数据成功");

                    for (int i = 0; i < 5; i++)
                    {
                        Thread.Sleep(1000);
                    }

                }
            }));
            th.Start();
        }

        private void RegisterMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            isWatch = false;
            isStartUser = false;
        }

    }

    public class RunInfo
    {
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
    }
}
