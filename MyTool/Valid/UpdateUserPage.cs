using Fangbian.Ticket.Server.AdvanceLogin;
using Fangbian.Tickets.Trains.WFDataItem;
using MyTool.FlowManager;
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
    public partial class UpdateUserPage : Form
    {
        private AutoResetEvent orderPayResetEvent = new AutoResetEvent(false);

        private List<AccountBind> source = new List<AccountBind>();

        private int currentValidCount = 0;

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

        public UpdateUserPage()
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
                    File.ReadAllLines(openFileDialog1.FileName, System.Text.Encoding.Default).ToList().ForEach(item =>
                    {
                        var user = item.Split(',');
                        source.Add(new AccountBind
                        {
                            State = AccountState.NoExcute,
                            PassWord = user[2],
                            UserName = user[1],
                            Passenger = new Maticsoft.Model.T_NewAccountEntity { PassengerId = user[4] },
                            Account = new Account12306Item { UserName = user[1], PassWord = user[2] }
                        });
                    });

                    BindNoExcuteTask();

                    source.ForEach(account =>
                    {
                        if (CurrentValidCount > 10)
                            orderPayResetEvent.WaitOne();

                        CurrentValidCount++;
                        UpdateUserActivation login = new UpdateUserActivation() { };
                        login.Data = account;
                        login.UpdateUserCompleted += UpdateUserCompleted;
                        login.OutputMessage += login_OutputMessage;
                        login.StartUpdateUser(account.Account);
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
            WriteMessage(e.Message);
        }

        int count = 0;


        private void WriteMessage(string message)
        {
            this.Invoke(new Action(() =>
            {
                count++;

                if (count == 100)
                {
                    count = 0;
                    textBox1.Text = string.Empty;
                } 

                if (message.Length > 200)
                {
                    message = message.Substring(0, 200) + "...";
                }

                textBox1.Text += "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":" + message;

                textBox1.SelectionStart = this.textBox1.Text.Length;

                textBox1.ScrollToCaret();
            }));
        }

        void UpdateUserCompleted(object sender, UpdateUserEventArgs e)
        {
            CurrentValidCount--;

            var user = sender as UpdateUserActivation;

            if (e.UpdateUserSeccess == false)
            {
                (user.Data as AccountBind).State = AccountState.ValidError;
                (user.Data as AccountBind).Message = e.Message;
            }
            else
            {
                (user.Data as AccountBind).State = AccountState.ValidSuccess;
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
                toolStripStatusLabel1.Text = string.Format("信息显示:待执行{0} 修改成功{1} 修改失败{2}", excute.Count(),
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

            BindNoExcuteTask();

            for (int i = 0; i < selectSource.Count; i++)
            {
                if (CurrentValidCount <= 10)
                {
                    CurrentValidCount++;
                    var item = dgNoValid.SelectedRows[i].DataBoundItem as AccountBind;
                    UpdateUserActivation login = new UpdateUserActivation() { };
                    login.Data = item;
                    login.UpdateUserCompleted += UpdateUserCompleted;
                    login.OutputMessage += login_OutputMessage;
                    login.StartUpdateUser(item.Account);
                }
                else
                {
                    orderPayResetEvent.WaitOne();
                }
            }
        }
    }
}
