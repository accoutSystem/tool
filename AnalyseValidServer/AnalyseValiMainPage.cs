using Fangbian.Ticket.Server.AdvanceLogin;
using Fangbian.Tickets.Trains.WFDataItem;
using Maticsoft.Model;
using MyTool.FlowManager;
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

namespace AnalyseValidServer
{
    public partial class AnalyseValiMainPage : Form
    {
        List<T_NewAccountEntity> accountCollection = new List<T_NewAccountEntity>();

        private AutoResetEvent orderPayResetEvent = new AutoResetEvent(false);

        int count = 0;

        public bool isValid = false;

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

        private int successCount = 0;

        public int SuccessCount
        {
            get { return successCount; }
            set
            {
                successCount = value; this.Invoke(new Action(() =>
                {
                    this.lbValid.Text = value.ToString();
                }));
            }
        }
        private int errorCount = 0;

        public int ErrorCount
        {
            get { return errorCount; }
            set
            {
                errorCount = value; this.Invoke(new Action(() =>
                {
                    lbNoValid.Text = value.ToString();
                }));
            }
        }

        private int loginErrorCount = 0;

        public int LoginErrorCount
        {
            get { return loginErrorCount; }
            set
            {
                loginErrorCount = value; this.Invoke(new Action(() =>
                {
                    lbLoginError.Text = value.ToString();
                }));
            }
        }

        public AnalyseValiMainPage()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            isValid = true;
            SetCount(0);
            ErrorCount=LoginErrorCount= SuccessCount = 0;
            var maxNumber =Convert.ToInt32( System.Configuration.ConfigurationManager.AppSettings["taskNumber"]);
            Thread th = new Thread(new ThreadStart(() =>
            {
                var db = DataTransaction.Create();

                try
                {
                    DataTable data = null;
                    //if (!checkBox1.Checked)
                    //{
                        var start = dateTimePicker1.Value.ToString("yyyy-MM-dd 00:00:01");

                        var end = dateTimePicker2.Value.ToString("yyyy-MM-dd 23:59:59");

                        data = db.Query("select userguid,username,password,state,PassengerId from t_newaccount where isActive=0 and state in(2,10) and createtime >='" + start + "' and  createtime <='" + end + "' order by createtime").Tables[0];
                    //}
                    //else {
                    //    var start = dateTimePicker3.Value.ToString("yyyy-MM-dd ") + textBox2.Text;
                    //    data = db.Query("select userguid,username,password,state,PassengerId from t_newaccount where isActive=0 and state in(2,10) and lasttime >='" + start + "' order by createtime").Tables[0];
                    //}
                    SetCount(data.Rows.Count);

                    int index = 0;

                    foreach (DataRow row in data.Rows)
                    {
                        if (isValid)
                        {
                            if (ToolCommon.IsProvideServer()==false) 
                            {
                                break;
                            }

                            if (TaskCount >= maxNumber)
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
                    MessageBox.Show("获取数据失败" + ex.Message);
                }
            }));

            th.Start();
        }
        void SetCount(int count)
        {
            this.Invoke(new Action(() =>
            {
                lbCount.Text = count.ToString();
            }));
        }
        void login_OutputMessage(object sender, Fangbian.DataStruct.Business.ConsoleMessageEventArgs e)
        {
            WriteMessage(e.Message);
        }

        void login_AccountCompleted(object sender, EmailValidLoginEventArgs e)
        {
            try
            {
                var user = sender as AccountActivation;

                var userInfo = user.currentUser;

                var data = accountCollection.FirstOrDefault(item => item.UserName.Equals(userInfo.UserName));

                if (data != null)
                {
                    if (e.IsLogin)
                    {
                        if (e.IsValid)
                        {
                            UpdateState(data.State.Value, 1, data);
                            SuccessCount++;
                        }
                        else
                        {
                            if (data.State == 2)
                            {
                                data.State = 14;
                            }
                            if (data.State == 10)
                            {
                                data.State = 15;
                            }
                            UpdateState(data.State.Value, 1, data);
                            ErrorCount++;
                        }
                    }
                    else
                    {

                        if (e.Message.Contains("登录名不存在")
                        || e.Message.Contains("请核实您注册用户信息是否真实")
                        || e.Message.Contains("密码输入错误")
                        || e.Message.Contains("该用户已被暂停使用"))
                        {
                            UpdateState(8, 1, data);
                        }
                        LoginErrorCount++;
                    }
                }
            }
            catch
            {
            }
            finally
            {
                TaskCount--;

                orderPayResetEvent.Set();
            }
        }

        private void UpdateState(int state, int active, T_NewAccountEntity user)
        {
            var sql = new SqlParamterItem();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update t_newaccount set State=" + state + ", isActive=" + active + ", LastTime=@LastTime where userGuid=@userGuid");
            ParameterInfo[] parameters = {
					new ParameterInfo("@UserGuid", DbType.String ,36),
                    new ParameterInfo("@LastTime", DbType.DateTime ,36)};

            parameters[0].Value = user.UserGuid;

            parameters[1].Value = DateTime.Now;

            sql.Sql = strSql.ToString();

            sql.ParamterCollection = parameters.ToList();

            DataTransaction.Create().ExecuteSql(sql);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            isValid = false;
        }

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
    }
}
