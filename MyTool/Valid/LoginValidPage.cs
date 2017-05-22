using Fangbian.Ticket.Server.AdvanceLogin;
using Fangbian.Tickets.Trains.WFDataItem;
using MyTool.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MyTool.Valid
{
    public partial class LoginValidPage : Form
    {
        private AutoResetEvent orderPayResetEvent = new AutoResetEvent(false);

        private List<AccountBind> source = new List<AccountBind>();

        private int currentValidCount = 0;

        public int CurrentValidCount
        {
            get { return currentValidCount; }
            set { currentValidCount = value;
            this.Invoke(new Action(() => {
                lbTask.Text = value.ToString();
            }));
            }
        }

        public LoginValidPage()
        {
            InitializeComponent();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                source.Clear();
                this.Invoke(new Action(() => 
                {

                    btnValid.Enabled = false;

                    btnValidBad.Enabled = false;
                }));

                lbFile.Text = openFileDialog1.FileName;

                Thread th = new Thread(new ThreadStart(() =>
                {
                    File.ReadAllLines(openFileDialog1.FileName).ToList().ForEach(item =>
                    {
                        var user =item.TrimEnd(',').Split(',');

                        string userName = user.Length > 2 ? user[1] : user[0];

                        string passWord = user.Length > 2 ? user[2] : user[1];

                        source.Add(new AccountBind
                        {
                            State = AccountState.NoExcute,
                            PassWord = CXDataCipher.DecipheringUserPW(passWord),
                            UserName = userName,
                            Passenger = new Maticsoft.Model.T_NewAccountEntity { PassengerId = user.Length > 2 ? user[4] : string.Empty },
                            Account = new Account12306Item { UserName = userName, PassWord = CXDataCipher.DecipheringUserPW(passWord) }
                        });
                    });

                    BindNoExcuteTask();

                    source.ForEach(account =>
                    {
                        if (CurrentValidCount > 5)
                            orderPayResetEvent.WaitOne();

                        CurrentValidCount++;
                        AccountActivation login = new AccountActivation() {  Passenger=null   };
                        login.Data = account;
                        login.AccountCompleted += login_AccountCompleted;
                        login.OutputMessage += login_OutputMessage;
                        login.Activation(account.Account);
                    });

                    BindNoExcuteTask();
                    this.Invoke(new Action(() =>
                    {
                        btnValid.Enabled = true;
                        btnValidBad.Enabled = true;
                    }));
                }));

                th.Start();
            }
        }

        void login_OutputMessage(object sender, Fangbian.DataStruct.Business.ConsoleMessageEventArgs e)
        {
            //WriteMessage(e.Message);
        }

        void login_AccountCompleted(object sender, AccountLoginEventArgs e)
        {
            CurrentValidCount--;

            var user = sender as AccountActivation;

            var userInfo=(user.Data as AccountBind).Account;

            if (e.IsLogin == false)
            { 
                if (e.Message.Contains("您的用户信息被他人冒用"))
                {
                    (user.Data as AccountBind).State = AccountState.FindError;

                    (user.Data as AccountBind).Message = e.Message;

                    WriteBusinessBad(userInfo.UserName, userInfo.PassWord, 0, 1);
                } 
                else
                {
                    (user.Data as AccountBind).State = AccountState.ValidError;

                    (user.Data as AccountBind).Message = e.Message;
                }
            }
            else
            {
                if (e.IsValid && cbRegister.Checked && e.PassengerCount <=14)
                {
                    var path = System.Environment.CurrentDirectory + @"\\Data\User.txt";
                    StreamWriter sw = File.AppendText(path);
                    string w = userInfo.UserName + "," + userInfo.PassWord;
                    sw.WriteLine(w);
                    sw.Close();
                }

                if (e.IsValid == false)
                {
                    WriteBusinessBad(userInfo.UserName, userInfo.PassWord, e.PassengerCount, 2);
                }
                else
                {
                    if (e.IsReceive == false)
                    {
                        WriteBusinessBad(userInfo.UserName, userInfo.PassWord, e.PassengerCount, 3);
                    }
                    else
                    {
                        WriteBusinessBad(userInfo.UserName, userInfo.PassWord, e.PassengerCount, 4);
                    }
                }
                 
                if (cbValid.Checked)
                {
                    if (e.IsActive && e.IsReceive)
                    {
                        (user.Data as AccountBind).State = AccountState.ValidSuccess; 
                    }
                    else
                    {
                        (user.Data as AccountBind).State = AccountState.FindError;
                        (user.Data as AccountBind).Message = (e.IsActive ? "邮件通过" : "邮件未通过") + (e.IsReceive ? "手机通过" : "手机未通过");
                    }
                }
                else 
                {
                    if (e.IsValid)
                    {
                        (user.Data as AccountBind).State = AccountState.ValidSuccess;
                        (user.Data as AccountBind).Message = e.Message;
                    }
                    else
                    {
                        (user.Data as AccountBind).State = AccountState.FindError;
                        (user.Data as AccountBind).Message = "身份未通过"; 
                    }
                }
            }
         
            BindNoExcuteTask();
            orderPayResetEvent.Set();
        }

        private void WriteBusinessBad(string userName, string pw, int passengerCount, int state)
        {
            if (checkBox1.Checked)
            {
                var path = System.Environment.CurrentDirectory + @"\\Data\分析结果.txt";
                StreamWriter sw = File.AppendText(path);
                //1 身份信息重复 需赔偿
                //2 身份未通过
                //3 手机未通过
                //4 其他状态
                string message = string.Empty;
                string pc = string.Empty;
                if (passengerCount <= 1)
                {
                    switch (state)
                    {
                        case 1:
                            message = "身份信息重复";
                            pc = "赔偿";
                            break;
                        case 2:
                            message = "身份未通过";
                            pc = "赔偿";
                            break;
                        case 3:
                            message = "手机未通过";
                            pc = "赔偿";
                            break;
                        case 4:
                            pc = "待定";
                            message = "登录成功";
                            break;
                    }
                }
                else 
                {
                    pc = "待定";
                }
                string w = userName + "," + pw + "," + passengerCount + "," + message + "," + pc;
                sw.WriteLine(w);
                sw.Close();
            }
        }

        private void BindNoExcuteTask()
        {
            this.Invoke(new Action(() =>
            {
                dgNoExcute.AutoGenerateColumns = false;
                dgValid.AutoGenerateColumns = false;
                dgNoValid.AutoGenerateColumns = false;
                dgFind.AutoGenerateColumns = false;

                var excute = source.Where(item => item.State == AccountState.NoExcute);

                var noValid = source.Where(item => item.State == AccountState.ValidError);

                var valid = source.Where(item => item.State == AccountState.ValidSuccess);

                var findError = source.Where(item => item.State == AccountState.FindError);

                dgNoExcute.DataSource = excute.ToList();
                dgNoValid.DataSource = noValid.ToList();
                dgValid.DataSource = valid.ToList();
                dgFind.DataSource = findError.ToList();
                toolStripStatusLabel1.Text = string.Format("信息显示:待执行{0} 登陆成功{1} 登陆失败{2} 被找回{3}", excute.Count(),
                    valid.Count(), noValid.Count(), findError.Count());
            }));
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            var selectSource = dgNoValid.SelectedRows;
            List<AccountBind> source = new List<AccountBind>();
            for (int i = 0; i < selectSource.Count; i++)
            {
                source.Add(dgNoValid.SelectedRows[i].DataBoundItem as AccountBind);
            } 
            if (selectSource.Count <= 0)
            {
                MessageBox.Show("请选择登陆失败的账号");
                return;
            }

            BindNoExcuteTask();

            for (int i = 0; i < source.Count; i++)
            {
                if (CurrentValidCount <= 10)
                {
                    CurrentValidCount++;
                    var item = source[i];
                    AccountActivation login = new AccountActivation();
                    login.Data = item;
                    login.AccountCompleted += login_AccountCompleted;
                    login.OutputMessage += login_OutputMessage;
                    login.Activation(item.Account);
                }
                else
                {
                    orderPayResetEvent.WaitOne();
                }
            }
                
        }
    }

    public class AccountBind
    {
        public string Data { get; set; }
        public Account12306Item Account { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public AccountState State { get; set; }
        public string Message { get; set; }

        public string UserId{get;set;}

        public Maticsoft.Model.T_NewAccountEntity Passenger { get; set; }

    }

    public enum AccountState
    {
        NoExcute = 0,
        ValidSuccess = 1,
        ValidError = 2,
        FindError = 3
    }
}
