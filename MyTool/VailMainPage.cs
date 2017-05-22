using Fangbian.Ticket.Server.AdvanceLogin;
using Fangbian.Tickets.Trains.WFDataItem;
using Maticsoft.Model;
using MyTool.Statistics;
using PD.Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MyTool
{
    public partial class VailMainPage : Form
    {
        List<T_NewAccountEntity> accountCollection = new List<T_NewAccountEntity>();

        List<T_NewAccountEntity> validAccountCollection = new List<T_NewAccountEntity>();

        private int taskCount = 0;

        public int TaskCount
        {
            get { return taskCount; }
            set
            {
                taskCount = value;
                this.Invoke(new Action(() =>
                {
                    lbTask.Text = value.ToString();
                }));
            }
        }
    
        private AutoResetEvent orderPayResetEvent = new AutoResetEvent(false);

        public VailMainPage()
        {
            InitializeComponent();
        }

        private void StartVailClick(object sender, EventArgs e)
        {
            isValid = true;

            if ((accountCollection.Count - validAccountCollection.Count) > 0) 
            {
                MessageBox.Show("任务正在运行，请等待...");
            }

            StringBuilder values = new StringBuilder();

            if (cbNew.Checked)
            {
                values.Append("0,");
            }

            if (cbNoValid.Checked)
            {
                values.Append("5,");
            }

            if (cbGo.Checked)
            {
                values.Append("3,");
            }

            if (cbEmailValid.Checked)
            {
                values.Append("1,");
            }

            if (cbGoNoValid.Checked)
            {
                values.Append("4,");
            }

            if (values.Length <= 0)
            {
                MessageBox.Show("请勾选资源类型!");
                return;
            }
            values.Remove(values.Length - 1, 1);

            accountCollection.Clear();

            validAccountCollection.Clear();

            toolStripStatusLabel6.Text = "0";

            Thread th = new Thread(new ThreadStart(() => 
            {
                var db = DataTransaction.Create();
               
                try
                {
                    var start = dateTimePicker1.Value.ToString("yyyy-MM-dd 00:00:01");

                    var end = dateTimePicker2.Value.ToString("yyyy-MM-dd 23:59:59");

                    var data = db.Query("select userguid,username,password,state,PassengerId from t_newaccount where state in(" + values + ")  and createtime >='" + start + "' and  createtime <='" + end + "' order by createtime").Tables[0];

                    SetCount(data.Rows.Count);

                    int index = 0;

                    foreach (DataRow row in data.Rows) 
                    {
                        if (isValid)
                        {
                            if (TaskCount > 10)
                            {
                                orderPayResetEvent.WaitOne();
                            }
                           
                            TaskCount++;

                            var item = new T_NewAccountEntity
                            {
                                UserGuid = row["userguid"] + string.Empty,
                                UserName = row["username"] + string.Empty,
                                PassWord = row["password"] + string.Empty,
                                PassengerId = row["PassengerId"] + string.Empty,
                                State = Convert.ToInt32(row["state"] + string.Empty),
                            };

                            index++;

                            this.Invoke(new Action(() =>
                            {
                                toolStripStatusLabel6.Text = index.ToString();
                            }));

                            accountCollection.Add(item);

                            AccountActivation login = new AccountActivation() { Passenger = item };

                            login.AccountCompleted += login_AccountCompleted;

                            login.OutputMessage += login_OutputMessage;

                            login.Activation(new Account12306Item { UserName = item.UserName, PassWord = item.PassWord });
                        }
                    }
                   
                }
                catch (Exception ex)
                {
                    MessageBox.Show("获取数据失败"+ex.Message);
                }
            }));

            th.Start();
        }

        void login_OutputMessage(object sender, Fangbian.DataStruct.Business.ConsoleMessageEventArgs e)
        {
            WriteMessage(e.Message);
        }

        void login_AccountCompleted(object sender, AccountLoginEventArgs e)
        {
            var user = sender as AccountActivation;

            var userInfo = user.currentUser;

            var data = accountCollection.FirstOrDefault(item => item.UserName.Equals(userInfo.UserName));

            if (data != null)
            {
               

                validAccountCollection.Add(data);

                try
                {
                    WriteMessage(data.UserName + (e.IsLogin ? "通过" : "未通过") + "核验");
                    //0 新注册 1通过邮箱核验 2 已通过验证 3已出 4已出未核验 5未核验 6已出且验证
                    if (data.State == 1)
                    {
                        UpdateState(e.IsLogin ? 2 : 5, data);
                    }
                    if (data.State == 3) 
                    {
                        UpdateState(e.IsLogin ? 6 : 4, data);
                    }
                    if (data.State == 0&&e.IsLogin) 
                    {
                        UpdateState(2, data);
                    }
                    if (data.State == 5&&e.IsLogin)
                    {
                        UpdateState(2, data);
                    }
                    WriteMessage(data.UserName + "修改账号状态成功");
                    WriteMessage("");
                }
                catch (Exception ex)
                {
                    WriteMessage(data.UserName + "修改账号状态失败" + ex.Message);
                  
                }

                SetValidCount();
            }

            TaskCount--;

            orderPayResetEvent.Set();
        }

        private void UpdateState(int newState, T_NewAccountEntity user)
        {
            var sql = new SqlParamterItem();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update t_newaccount set state=" + newState + " where userGuid=@userGuid");
            ParameterInfo[] parameters = {
					new ParameterInfo("@UserGuid", DbType.String ,36) };
            parameters[0].Value = user.UserGuid;

            sql.Sql = strSql.ToString();

            sql.ParamterCollection = parameters.ToList();

            DataTransaction.Create().ExecuteSql(sql);
        }

        void SetCount(int count)
        {
            this.Invoke(new Action(() =>
            {
                lbCount.Text = count.ToString();
            }));
        }

        void SetValidCount()
        {
            this.Invoke(new Action(() =>
            {
                lbValid.Text = validAccountCollection.Count.ToString();

                lbNoValid.Text = (Convert.ToInt32(lbCount.Text) - validAccountCollection.Count).ToString();
            }));
        }

        int count = 0;

        public bool isValid = false;

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

                textBox1.Text += "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":" + message;

                textBox1.SelectionStart = this.textBox1.Text.Length;

                textBox1.ScrollToCaret();
            }));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            isValid = false;
        }

        private void toolStripStatusLabel5_Click(object sender, EventArgs e)
        {
            ResourceStatisticsPage page = new ResourceStatisticsPage();
            page.Show();
        }
    }

    //0 新注册 1通过邮箱核验 2 已通过验证 3已出 4已出未核验 5未核验
}
