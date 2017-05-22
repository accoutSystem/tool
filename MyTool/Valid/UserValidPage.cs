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
    public partial class UserValidPage : Form
    {
        private AutoResetEvent orderPayResetEvent = new AutoResetEvent(false);

        private List<AccountBind> source = new List<AccountBind>();

        private int fullAccount=0;
        public int FullAccount
        {
            get
            {
                return fullAccount;
            }
            set
            {
                fullAccount = value;
                Invoke(new Action(() => {
                    toolStripStatusLabel4.Text = value + string.Empty;
                }));
            }
        }

        private int noFullAcciont = 0;

        public int NoFullAccount
        {
            get
            {
                return noFullAcciont;
            }
            set
            {
                noFullAcciont = value;
                Invoke(new Action(() =>
                {
                    toolStripStatusLabel6.Text = value + string.Empty;
                }));
            }
        }

        private int bl = 0;
        public int BL
        {
            get
            {
                return bl;
            }
            set
            {
                bl = value;
                Invoke(new Action(() =>
                {
                    toolStripStatusLabel8.Text = value + "%";
                }));
            }
        }

        private int ysgs = 0;
        public int YSGS
        {
            get
            {
                return ysgs;
            }
            set
            {
                ysgs = value;
                Invoke(new Action(() =>
                {
                    toolStripStatusLabel10.Text = value + "";
                }));
            }
        }

        public int TotalCount = 0;

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

        public UserValidPage()
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
                TotalCount = 0;
                int count = Convert.ToInt32(toolStripTextBox1.Text)-2;
                Random ran=new Random();
                Thread th = new Thread(new ThreadStart(() =>
                {
                    openFileDialog1.FileNames.ToList().ForEach(file =>
                    {
                        var fileItems = File.ReadAllLines(file).ToList();
                        TotalCount += fileItems.Count;
                        source.Add(CreateAccountData(fileItems[0], Path.GetFileName(file)));

                        for (int i = 0; i < count; i++)
                        {
                            var index = ran.Next(1, fileItems.Count - 2);

                            source.Add(CreateAccountData(fileItems[index], Path.GetFileName(file)));
                        }

                        source.Add(CreateAccountData(fileItems[fileItems.Count - 1], Path.GetFileName(file)));
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

        private AccountBind CreateAccountData(string item,string data)
        {
            var user = item.TrimEnd(',').Split(',');

            string userName = user.Length > 2 ? user[1] : user[0];

            string passWord = user.Length > 2 ? user[2] : user[1];

            return new AccountBind
             {
                 State = AccountState.NoExcute, Data=data,
                 PassWord = CXDataCipher.DecipheringUserPW(passWord),
                 UserName = userName,
                 Passenger = new Maticsoft.Model.T_NewAccountEntity { PassengerId = user.Length > 2 ? user[4] : string.Empty },
                 Account = new Account12306Item { UserName = userName, PassWord = CXDataCipher.DecipheringUserPW(passWord) }
             };
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

                if (e.IsValid)
                {
                    (user.Data as AccountBind).State = AccountState.ValidSuccess;
                    (user.Data as AccountBind).Message = e.Message;
                    if (e.PassengerCount >= 14)
                    {
                        FullAccount++;
                        BL = Convert.ToInt32(((decimal)FullAccount / (decimal)source.Count) * 100);
                        YSGS = Convert.ToInt32((decimal)BL * (decimal)TotalCount / 100);
                    }
                    else
                    {
                        NoFullAccount++;
                    }
                }
                else
                {
                    (user.Data as AccountBind).State = AccountState.FindError;
                    (user.Data as AccountBind).Message = "身份未通过";
                }
            }
         
            BindNoExcuteTask();
            orderPayResetEvent.Set();
        }

        private void WriteBusinessBad(string userName, string pw, int passengerCount, int state)
        {
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


  
}
