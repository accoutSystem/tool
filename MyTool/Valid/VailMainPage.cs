using Fangbian.Ticket.Server.AdvanceLogin;
using Fangbian.Tickets.Trains.WFDataItem;
using Maticsoft.Model;
using MyTool.Common;
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
        public bool IsNewRegister { get; set; }
        public string  CurrentDate{ get; set; }

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

        private int runTaskNumber;

        public int RunTaskNumber
        {
            get { return runTaskNumber; }
            set {
                runTaskNumber = value;
                 this.Invoke(new Action(() =>
                {
                    textBox2.Text = value.ToString();
                }));
            
            }
        }
    
        private AutoResetEvent orderPayResetEvent = new AutoResetEvent(false);

        public VailMainPage()
        {
            InitializeComponent();
            Load += VailMainPage_Load;
        }

        void VailMainPage_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(CurrentDate))
            {
                dateTimePicker1.Value = dateTimePicker2.Value = Convert.ToDateTime(CurrentDate);
            }
            RunTaskNumber = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["validTaskNumber"]);
            //if (IsNewRegister)
            //{
            //    MessageBox.Show("验证新注册的账号将自动分析如果出现登陆名不存在密码错误则状态变为废账号!");
            //    cbNew.Checked = true;
            //    cbNoValid.Checked = cbEmailValid.Checked = cbGo.Checked = cbGoNoValid.Checked = false;
            //    cbNoValid.Enabled = cbEmailValid.Enabled = cbGo.Enabled = cbGoNoValid.Enabled = false;
            //}
            //else
            //{
            //    cbNew.Enabled = false;
            //}
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
            if (checkBox1.Checked)
            {
                values.Append("16,");
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

            if (cbUser.Checked) {
                values.Append("2,");
            } if (cbBad.Checked)
            {
                values.Append("8,");
            } if (checkBox2.Checked)
            {
                values.Append("15,");
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

                    var data = db.Query("select phone, userguid,username,password,state,PassengerId,PassengerName from t_newaccount where state in(" + values + ")  and createtime >='" + start + "' and  createtime <='" + end + "' and  passengername !='外来资源'").Tables[0];
                   
                    SetCount(data.Rows.Count);

                    if (data.Rows.Count <= 0)
                    {
                        MessageBox.Show("没有数据");
                        return;
                    }
                    int index = 0;
                 
                    foreach (DataRow row in data.Rows) 
                    {
                        if (isValid)
                        {
                               var item = new T_NewAccountEntity
                            {
                                UserGuid = row["userguid"] + string.Empty,
                                UserName = row["username"] + string.Empty,
                                PassWord = row["password"] + string.Empty,
                                Phone = row["phone"] + string.Empty,
                                PassengerId = row["PassengerId"] + string.Empty,
                                State = Convert.ToInt32(row["state"] + string.Empty),
                            };
                            if (row["PassengerName"].Equals("外来资源"))
                            {
                                WriteMessage( row["username"] +"为外来资源不予处理");
                                accountCollection.Add(item);
                                validAccountCollection.Add(item);
                                SetValidCount();
                                continue;
                            }

                            if (TaskCount >= RunTaskNumber)
                            {
                                orderPayResetEvent.WaitOne();
                            }
                           
                            TaskCount++;

                            index++;

                            this.Invoke(new Action(() =>
                            {
                                toolStripStatusLabel6.Text = index.ToString();
                            }));

                            accountCollection.Add(item);

                            AccountActivation login = new AccountActivation() { Passenger = item };

                            login.AccountCompleted += login_AccountCompleted;

                            login.OutputMessage += login_OutputMessage;

                            login.Activation(new Account12306Item { UserName = item.UserName, PassWord =CXDataCipher.DecipheringUserPW( item.PassWord) });
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
            try
            {
                var user = sender as AccountActivation;

                var userInfo = user.currentUser;

                var data = accountCollection.FirstOrDefault(item => item.UserName.Equals(userInfo.UserName));

                if (data != null)
                {
                    validAccountCollection.Add(data);

                    try
                    {
                        StringBuilder msg = new StringBuilder();
                        msg.Append("【登录" + data.UserName + (e.IsLogin ? "成功" : "失败") + "】");
                        msg.Append("【"+ (e.IsValid ? "通过" : "未通过") + "身份核验】");
                        if (cbNoValidEmail.Checked) 
                        {
                            msg.Append("【无需邮件核验】");
                        }
                        else
                        {
                            msg.Append("【" + (e.IsActive ? "通过" : "未通过") + "邮箱核验】");
                        }
                        msg.Append("【" + (e.IsReceive ? "通过" : "未通过") + "手机核验】");
                        msg.Append("【手机号" + data.Phone + "】");
                       
                        WriteMessage(msg.ToString());
                        //0 新注册 1通过邮箱核验 2 已通过验证 3已出 4已出未核验 5未核验 6已出且验证 7入队列 8坏账好

                        if ((e.Message + "").Contains("登录名不存在")||
                            (e.Message + "").Contains("您的用户信息被他人冒用"))
                        {
                            UpdateState(8, data);
                            return;
                        }
                        if (e.IsLogin == false)
                            return;

                        var emailValie = cbNoValidEmail.Checked ? true : e.IsActive;

                        if (e.IsValid && emailValie && e.IsReceive)
                        {
                            UpdateState(10, data);
                            return;
                        }

                        if (e.IsValid == false) 
                        {
                            UpdateState(5, data);
                            return;
                        }
                       
                            if (data.State == 1 || data.State == 16)
                            {
                                if (e.IsValid)
                                {
                                    if (e.IsActive)
                                    {
                                        if (e.IsReceive)
                                        {
                                            UpdateState(10, data, 1);//优质资源
                                        }
                                        else
                                        {
                                            UpdateState(2, data, 1);//可用状态
                                        }
                                    }
                                    else
                                    {
                                        if (e.IsReceive)
                                        {
                                            UpdateState(15, data, 1);
                                        }
                                        else
                                        {
                                            UpdateState(14, data, 1);
                                        }
                                    }
                                }
                                else
                                {
                                    UpdateState(5, data);
                                }
                            }
                            
                            if (data.State == 3)
                            {
                                UpdateState(e.IsValid ? 6 : 4, data);
                            }
                            if (data.State == 2)
                            {
                                if (e.IsValid && e.IsActive && e.IsReceive)
                                {
                                    UpdateState(10, data);
                                }
                            }
                            if ((data.State == 5 || data.State == 0) && e.IsValid)
                            {
                                if (e.IsActive)
                                {
                                    UpdateState(2, data, 1);
                                }
                                else
                                {
                                    UpdateState(14, data, 1);
                                }
                            }
                      
                        WriteMessage(data.UserName + "修改账号状态成功");
                        WriteMessage("");
                    }
                    catch (Exception ex)
                    {
                        WriteMessage(data.UserName + "修改账号状态失败" + ex.Message);
                    }

                   
                }
            }
            catch 
            {
            }
            finally
            {
                SetValidCount();

                TaskCount--;

                orderPayResetEvent.Set();
            }
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

        private void UpdateState(int newState, T_NewAccountEntity user,int active)
        {
            var sql = new SqlParamterItem();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update t_newaccount set State=" + newState + ", isActive=" + active + ", LastTime=@LastTime where userGuid=@userGuid");
            ParameterInfo[] parameters = {
					new ParameterInfo("@UserGuid", DbType.String ,36),
                    new ParameterInfo("@LastTime", DbType.DateTime ,36)};

            parameters[0].Value = user.UserGuid;

            parameters[1].Value = DateTime.Now;

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
                if (message.Length > 200) {
                    message = message.Substring(0, 200) + "...";
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

        private void btnUpdateTaskNumber_Click(object sender, EventArgs e)
        {
            RunTaskNumber = Convert.ToInt32(textBox2.Text);
        }

    }

    //0 新注册 1通过邮箱核验 2 已通过验证 3已出 4已出未核验 5未核验 6已出未核验 7 坏账号
}
