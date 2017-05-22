using Fangbian.Log;
using Fangbian.Ticket.Server.Trace.Watch;
using Fangbian.Tickets.Trains.Activities;
using MobileValidServer.Properties;
using MyTool.Common;
using PD.Business;
using ResourceBulidTool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobileValidServer
{
    public partial class MobileValidPage : Form
    {

        bool isRunServer = false;

        bool isAddTask = true;

        private Dictionary<UserItem, DataGridViewRow> controls = new Dictionary<UserItem, DataGridViewRow>();

        private int successCount = 0;

        public int SuccessCount
        {
            get { return successCount; }
            set
            {
                successCount = value;
                this.Invoke(new Action(() =>
                {
                    lbSuccess.Text = value.ToString();
                }));
            }
        }


        private int errorCount = 0;

        public int ErrorCount
        {
            get { return errorCount; }
            set
            {
                errorCount = value;
                this.Invoke(new Action(() =>
                {
                    lbError.Text = value.ToString();
                }));
            }
        }

        private string currentPlatfoem;

        public string CurrentPlatfoem
        {
            set
            {
                currentPlatfoem = value;

                this.Text = value + "核验V1.3->" + BoxConnectionManager.ConnectionString;

                this.lbProf.Text = value;
            }
            get { return currentPlatfoem; }
        }

        public int TaskNumber { get; set; }

        public MobileValidPage()
        {
            InitializeComponent(); 
            Load += MobileValidPage_Load;
            //dataGridView1.rowe
        }

        void MobileValidPage_Load(object sender, EventArgs e)
        {
            RunBtnText();
            Run();
            Watch();
        }

        private void Run()
        {
            var taskNumber = TaskNumber;

            Thread th = new Thread(new ThreadStart(() =>
            {
                while (isAddTask)
                {
                    if (ToolCommonMethod.IsProvideServerValid())
                    {
                        var source = UserItemCollection.Current.ToArray().ToList().Where(item => (Convert.ToDateTime(item.LoginTime).AddMinutes(3) < DateTime.Now) && item.State == UserState.正在登陆).ToList();

                        if (source.Count() > 0)
                        {
                            foreach (var user in source)
                            {
                                if (user.Valid != null)
                                {
                                    user.Valid.Stop();
                                }
                                RemoveValidMobile(user);
                            }
                        }

                        if (UserItemCollection.Current.Count < taskNumber)
                        {
                            if (isRunServer)
                            {
                                if (CreateValidPhone()==false)
                                {
                                    Thread.Sleep(3000);
                                }
                            }

                            Thread.Sleep(1000);
                        }
                        else
                        {
                            Thread.Sleep(2000);
                        }
                    }
                    else
                    {
                        Console.WriteLine("12306系统维护阶段");
                        Thread.Sleep(30000);
                    }
                }
            }));
            th.IsBackground = true;
            th.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("暂不可用");
        }

    

        private bool CreateValidPhone()
        {
            if (isRunServer)
            {
                var item = GetPhoneValidServer.Get();

                if (item != null)
                {

                    UserItemCollection.Current.Add(item);
                  
                    BulidValid(item);

                    AddRow(item);

                    item.PropertyChange += UserItemPropertyChange;

                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        void UserItemPropertyChange(object sender, PropertyChangeEventArgs e)
        {
            var item = sender as UserItem;
            if (controls.ContainsKey(item))
            {
                this.Invoke(new Action(() =>
                {
                    switch (e.PropertyName)
                    {
                        case "UserName": controls[item].Cells[2].Value = e.Value; break;
                        case "PassWord": controls[item].Cells[3].Value = e.Value; break;
                        case "CreateTime": controls[item].Cells[0].Value = e.Value; break;
                        case "LoginTime": controls[item].Cells[1].Value = e.Value; break;
                        case "State": controls[item].Cells[4].Value = e.Value; break;
                        case "Message": controls[item].Cells[6].Value = e.Value; break;
                        case "Phone": controls[item].Cells[5].Value = e.Value; break;
                    }
                }));
            }
        }

        private void AddRow(UserItem item)
        {
            this.Invoke(new Action(() =>
            {
                var index = dataGridView1.Rows.Add();

                dataGridView1.Rows[index].Cells[0].Value = item.CreateTime;
                dataGridView1.Rows[index].Cells[1].Value = item.LoginTime;
                dataGridView1.Rows[index].Cells[2].Value = item.UserName;
                dataGridView1.Rows[index].Cells[3].Value = item.PassWord;
                dataGridView1.Rows[index].Cells[4].Value = item.State.ToString();
                dataGridView1.Rows[index].Cells[5].Value = item.Phone;
                dataGridView1.Rows[index].Cells[6].Value = item.Message;

                controls.Add(item, dataGridView1.Rows[index]);
            }));
        }

        private void RemoveValidMobile(UserItem item)
        {
            UserItemCollection.Current.Remove(item);

            this.Invoke(new Action(() =>
            {
                if (controls.ContainsKey(item))
                {
                    dataGridView1.Rows.Remove(controls[item]);
                    controls.Remove(item);
                }
            }));
        }

        private void RefurbishGrid()
        {
            this.Invoke(new Action(() =>
            {
                try
                {
                  
                    //dataGridView1.AutoGenerateColumns = false;
                    //dataGridView1.DataSource = null;
                    //dataGridView1.DataSource = UserItemCollection.Current;
                }
                catch { }
            }));
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("暂不可用");
        }

        private void BulidValid(UserItem data)
        {
            ValidPhoneManager manager = new ValidPhoneManager();

            data.Valid = manager;

            data.Phone = data.Message = string.Empty;

            data.State = UserState.未使用;

            manager.ValidCompleted += PhoneValidCompleted;

            manager.Activation(data);
        }

        void PhoneValidCompleted(object sender, EventArgs e)
        {
            var manager = sender as ValidPhoneManager;

            if (manager.CurrentUser.State == UserState.核验成功)
            {
                string userGuid = manager.CurrentUser.UserGuid;

                var data = DataTransaction.Create();

                var sql = "update t_newaccount set state=10,Phone='" + manager.CurrentUser.Phone + "' where UserGuid='" + userGuid + "'";

                data.ExecuteSql(sql);

                RemoveValidMobile(manager.CurrentUser);
                
                SuccessCount++;
            }
            else if (manager.CurrentUser.Message.Contains("由于您获取验证码短信次数过多"))
            {
                var data = DataTransaction.Create();

                var sql = "update t_newaccount set state=13 where UserGuid='" + manager.CurrentUser.UserGuid + "'";

                data.ExecuteSql(sql);

                RemoveValidMobile(manager.CurrentUser);

                ErrorCount++;
            }
            else if (manager.CurrentUser.Message.Contains("请您先对身份信息进行核验，通过后再办理手机号码核验业务"))
            {
                Logger.Fatal(manager.CurrentUser.UserName + " " + manager.CurrentUser.PassWord + "Message:" + manager.CurrentUser.Message);
                var data = DataTransaction.Create();

                var sql = "update t_newaccount set state=8  where UserGuid='" + manager.CurrentUser.UserGuid + "'";

                data.ExecuteSql(sql);

                RemoveValidMobile(manager.CurrentUser);

                ErrorCount++;
            }
            else if (manager.CurrentUser.State == UserState.登陆失败)
            {
                if (manager.CurrentUser.Message.Contains("登录名不存在")
                      || manager.CurrentUser.Message.Contains("请核实您注册用户信息是否真实")
                      || manager.CurrentUser.Message.Contains("密码输入错误")
                      || manager.CurrentUser.Message.Contains("该用户已被暂停使用") 
                      || manager.CurrentUser.Message.Contains("您的用户信息被他人冒用"))
                {
                    var data = DataTransaction.Create();

                    var sql = "update t_newaccount set state=8  where UserGuid='" + manager.CurrentUser.UserGuid + "'";

                    data.ExecuteSql(sql);

                    RemoveValidMobile(manager.CurrentUser);

                    ErrorCount++;
                }
                else
                {
                    manager.CurrentUser.Valid = null;
                    BulidValid(manager.CurrentUser);
                }
            }
            else if (manager.CurrentUser.State != UserState.登陆失败
                && manager.CurrentUser.State != UserState.拉取验证码失败
                && (manager.CurrentUser.Message.Contains("已被其他用户用于在本网站注册用户") ||
                    manager.CurrentUser.Message.Contains("手机号码格式错误") ||
                    manager.CurrentUser.Message.Contains("String Index Out Of Bounds: null") ||
                    manager.CurrentUser.Message.Contains("未将对象引用设置到对象的实例") ||
                    manager.CurrentUser.State == UserState.获取手机号码失败 ||
                    manager.CurrentUser.State == UserState.手机号码已经被使用))
            {
                manager.CurrentUser.Message = "开始重新获取手机号码在同一个任务";

                Thread.Sleep(3000);
                manager.CurrentUser.Phone = string.Empty;
                manager.ValidPhone();
            }
            else if (manager.CurrentUser.State == UserState.邮件未核验)
            {
                var data = DataTransaction.Create();

                var sql = "update t_newaccount set state=17 where UserGuid='" + manager.CurrentUser.UserGuid + "'";

                data.ExecuteSql(sql);

                RemoveValidMobile(manager.CurrentUser);

                ErrorCount++;
            }
            else if (manager.CurrentUser.State == UserState.身份未验证)
            {
                var data = DataTransaction.Create();

                var sql = "update t_newaccount set state=5 where UserGuid='" + manager.CurrentUser.UserGuid + "'";

                data.ExecuteSql(sql);

                RemoveValidMobile(manager.CurrentUser);

                ErrorCount++;
            }
            else if (manager.CurrentUser.State == UserState.已用)
            {
                var data = DataTransaction.Create();

                var sql = "update t_newaccount set state=6 where UserGuid='" + manager.CurrentUser.UserGuid + "'";

                data.ExecuteSql(sql);

                RemoveValidMobile(manager.CurrentUser);

                ErrorCount++;
            }
            else if (manager.CurrentUser.State == UserState.身份通过邮件未通过)
            {
                var data = DataTransaction.Create();

                var sql = "update t_newaccount set state=14 where UserGuid='" + manager.CurrentUser.UserGuid + "'";

                data.ExecuteSql(sql);

                RemoveValidMobile(manager.CurrentUser);

                ErrorCount++;
            }
            else
            {
                if (manager.CurrentUser.State == UserState.余额不足)
                {
                    isRunServer = false;
                    RunBtnText();
                }

                var data = DataTransaction.Create();

                var sql = "update t_newaccount set state=13 where UserGuid='" + manager.CurrentUser.UserGuid + "'";

                data.ExecuteSql(sql);

                RemoveValidMobile(manager.CurrentUser);

                ErrorCount++;
            }
        }

        public SqlParamterItem AddValidPhoneData(string userGuid)
        {
            var sql = new SqlParamterItem();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into t_customaccount(");
            strSql.Append("userguid,accounttype)");
            strSql.Append(" values (");
            strSql.Append("@userguid,@accounttype)");
            ParameterInfo[] parameters = {
					new ParameterInfo("@userguid", DbType.String ,45),
					new ParameterInfo("@accounttype", DbType.Int32,40) };
            parameters[0].Value = userGuid;
            parameters[1].Value = 2;

            sql.Sql = strSql.ToString();

            sql.ParamterCollection = parameters.ToList();

            return sql;
        }

        private void StartClick(object sender, EventArgs e)
        {
            isRunServer = !isRunServer;
            RunBtnText();
        }

        void RunBtnText()
        {
            Invoke(new Action(() =>
            {
                if (isRunServer)
                {
                    btnValid.Image = Resources.stop;
                    btnValid.Text = "停止核验手机";
                    this.Text = "【运行】"+this.Text ;
                }
                else
                {
                    btnValid.Image = Resources.start;
                    btnValid.Text = "开始核验手机";
                    this.Text = "【停止】" + this.Text;
                }
            }));
         
        }

        private void RefurbishClick(object sender, EventArgs e)
        {
            RefurbishGrid();
        }

        private void MobileValidPage_FormClosing(object sender, FormClosingEventArgs e)
        {
            isAddTask = false; 
        }

        #region 对服务进行监控和接收指令进行下发
        /// <summary>
        /// 上传监视数据
        /// </summary>
        public void Watch()
        {
            return;
            Console.WriteLine("开启监视");

            Thread thread = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    if (ToolCommonMethod.IsProvideServer())
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

            message.Append(" 操作状态:" + (isRunServer ? "启动" : "关闭"));
            message.Append(" 平台:" + CurrentPlatfoem);

            message.Append(" 成功数:" + SuccessCount);
            message.Append(" 失败数:" + ErrorCount);
            message.Append(" 当前任务数:" + UserItemCollection.Current.Count);

            ServerWatch.UpLoadMachineWatch(message.ToString(), "customCalidUser" + ServerWatch.WatchGuid);
        }

        private void GetServerInstruct()
        {
            Console.WriteLine("开始读取控制指令");
        }
        #endregion

        private void button3_Click(object sender, EventArgs e)
        {
            while (true)
            {
                var item = GetPhoneValidServer.Get();
                if (item == null)
                {
                    return;
                }
            }
        }

        private void SetConfigClick(object sender, EventArgs e)
        {
            CX.Config.SetConfig config = new CX.Config.SetConfig();
            config.ShowDialog();
        }

        private void toolStripStatusLabel4_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定清理?", "", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                SuccessCount = ErrorCount = 0;
            }
        }
    }
}
