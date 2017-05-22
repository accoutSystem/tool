using Fangbian.Ticket.Server.AdvanceLogin;
using Fangbian.Tickets.Trains.WFDataItem;
using MyTool.Common;
using PD.Business;
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
    public partial class SendEmailValidPage : Form
    {
        private AutoResetEvent orderPayResetEvent = new AutoResetEvent(false);

        private List<AccountBind> source = new List<AccountBind>();

        private int currentValidCount = 0;

        public bool IsRun = false;

        public int CurrentValidCount
        {
            get { return currentValidCount; }
            set
            {
                currentValidCount = value;
                this.Invoke(new Action(() =>
                {
                    lbTask.Text = value.ToString();
                }));
            }
        }

        public SendEmailValidPage()
        {
            InitializeComponent();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (IsRun)
            {
                IsRun = false;
                btnValid.Text = "开始";
                return;
            }
            else
            {
                btnValid.Text = "停止";
                IsRun = true;
            }
            lbFile.Text = openFileDialog1.FileName;
            var type = tbType.Text;
            Thread th = new Thread(new ThreadStart(() =>
            {
                var db = PD.Business.DataTransaction.Create();
                string sql = string.Empty;
                if (type.Equals("1"))
                { 
                    sql = " select * from  t_newaccount  where createTime>'2015-09-28 01:26:22' and state=0 ";
                }

                if (type.Equals("2"))
                {

                    sql = " select * from  t_newaccount  where   state=14 ";
                }

                if (type.Equals("3"))
                {

                    sql = " select * from  t_newaccount  where  state=15 ";
                }
                if (type.Equals("4"))
                {

                    sql = " select * from  t_newaccount  where  state=16 ";
                }

                var dbsource = db.Query(sql);

                foreach (DataRow row in dbsource.Tables[0].Rows)
                {
                    source.Add(new AccountBind
                    {
                        State = AccountState.NoExcute,
                        PassWord = row["UserName"] + string.Empty,
                        UserName = row["PassWord"] + string.Empty,
                        UserId = row["UserGuid"] + string.Empty,
                        Passenger = new Maticsoft.Model.T_NewAccountEntity { PassengerId = row["PassengerId"] + string.Empty },
                        Account = new Account12306Item
                        {
                            UserName = row["UserName"] + string.Empty,
                            PassWord = CXDataCipher.DecipheringUserPW(row["PassWord"] + string.Empty)
                        }
                    });
                }

                BindNoExcuteTask();

                int number = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["validTaskNumber"]);
                source.ForEach(account =>
                {
                    if (IsRun)
                    {
                        if (CurrentValidCount >= number)
                            orderPayResetEvent.WaitOne();

                        CurrentValidCount++;
                        if (type.Equals("1"))
                        {
                            ChangeState(account, 18);
                        }
                        SendEmailActivation login = new SendEmailActivation() { Passenger = account.Passenger };
                        login.Data = account;
                        login.SendEmailCompleted += login_AccountCompleted;
                        login.OutputMessage += login_OutputMessage;
                        account.Account.PassWord = CXDataCipher.DecipheringUserPW(account.Account.PassWord);
                        login.Activation(account.Account);
                    }
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

        public void ChangeState(AccountBind bind,int  state) 
        {
            var data = DataTransaction.Create();

            var sql = "update t_newaccount set state=" + state + " ,LastTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'  where UserGuid='" + bind.UserId + "'";

            data.ExecuteSql(sql);
        }

        void login_OutputMessage(object sender, Fangbian.DataStruct.Business.ConsoleMessageEventArgs e)
        {
            //WriteMessage(e.Message);
        }

        void login_AccountCompleted(object sender, SendEmailEventArgs e)
        {
            CurrentValidCount--;

            var user = sender as SendEmailActivation;

            if (e.Send)
            {
                (user.Data as AccountBind).State = AccountState.ValidSuccess;

                if (tbType.Text.Equals("1"))
                {
                    ChangeState((user.Data as AccountBind), 19);
                }
                else
                {
                    ChangeState((user.Data as AccountBind), 0);
                }
            }
            else
            {
                if (e.Message.Contains("登录名不存在"))
                {
                    (user.Data as AccountBind).Message = "登录名不存在,已变废账号";
                    (user.Data as AccountBind).State = AccountState.ValidSuccess;
                    ChangeState((user.Data as AccountBind), 8);
                }
                else if (e.Message.Contains("邮件已经核验"))
                {
                    (user.Data as AccountBind).Message = "邮件已经核验";
                    (user.Data as AccountBind).State = AccountState.ValidSuccess;
                    ChangeState((user.Data as AccountBind), 1);
                }
                else if (e.Message.Contains("邮件手机均核验"))
                {
                    (user.Data as AccountBind).Message = "邮件手机均核验";
                    (user.Data as AccountBind).State = AccountState.ValidSuccess;
                    ChangeState((user.Data as AccountBind), 10);
                }
                else if (e.Message.Contains("请核实您注册用户信息是否真实")
                   || e.Message.Contains("密码输入错误")
                   || e.Message.Contains("该用户已被暂停使用")
                   || e.Message.Contains("您的用户信息被他人冒用"))
                {
                    (user.Data as AccountBind).State = AccountState.ValidSuccess;
                    ChangeState((user.Data as AccountBind), 9);
                }
                else
                {
                    (user.Data as AccountBind).State = AccountState.ValidError;
                    (user.Data as AccountBind).Message = e.Message;
                }
            }
         
            BindNoExcuteTask();

            orderPayResetEvent.Set();
        }

        private void BindNoExcuteTask()
        {
            this.Invoke(new Action(() =>
            {
                dgNoExcute.AutoGenerateColumns = false;
                dgValid.AutoGenerateColumns = false;
                dgNoValid.AutoGenerateColumns = false;

                var excute = source.Where(item => item.State == AccountState.NoExcute);
                var noValid = source.Where(item => item.State == AccountState.ValidError);
                var valid = source.Where(item => item.State == AccountState.ValidSuccess);
                dgNoExcute.DataSource = excute.ToList();
                dgNoValid.DataSource = noValid.ToList();
                dgValid.DataSource = valid.ToList();
                toolStripStatusLabel1.Text = string.Format("信息显示:待执行{0} 登陆成功{1} 登陆失败{2}", excute.Count(),
                    valid.Count(), noValid.Count());
            }));
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            var selectSource = dgNoValid.SelectedRows;

            if (selectSource.Count <= 0) 
            {
                MessageBox.Show("请选择登陆失败的账号");
                return;
            }
            List<AccountBind> BadSource = new List<AccountBind>();
            for (int i = 0; i < selectSource.Count; i++)
            {
                var item = dgNoValid.SelectedRows[i].DataBoundItem as AccountBind;
                BadSource.Add(item);
            }
            BindNoExcuteTask();

            for (int i = 0; i < BadSource.Count; i++)
            {
                if (CurrentValidCount <= 10)
                {
                    CurrentValidCount++;
                    var item = BadSource[i];
                    SendEmailActivation login = new SendEmailActivation();
                    login.Data = item;
                    login.SendEmailCompleted += login_AccountCompleted;
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
