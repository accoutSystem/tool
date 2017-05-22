using Fangbian.Ticket.Server.AdvanceLogin;
using Fangbian.Tickets.Trains.WFDataItem;
using Maticsoft.Model;
using MyTool.Common;
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

namespace MyTool.Valid
{
    public partial class DeletePassenger : Form
    {
        List<T_NewAccountEntity> accountCollection = new List<T_NewAccountEntity>();

        List<T_NewAccountEntity> validAccountCollection = new List<T_NewAccountEntity>();
        public bool IsNewRegister { get; set; }
        public string CurrentDate { get; set; }

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

        public DeletePassenger()
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
           
            accountCollection.Clear();

            validAccountCollection.Clear();

            toolStripStatusLabel6.Text = "0";

            Thread th = new Thread(new ThreadStart(() => 
            {
                var db = DataTransaction.Create();
               
                try
                {
                    var data = db.Query("select userguid,username,password,state,PassengerId from t_newaccount where state =22").Tables[0];

                    SetCount(data.Rows.Count);
                    if (data.Rows.Count <= 0)
                    {
                        MessageBox.Show("没有数据");
                        return;
                    }
                    int index = 0;

                    var taskNumber =Convert.ToInt32( System.Configuration.ConfigurationManager.AppSettings["validTaskNumber"]);

                    foreach (DataRow row in data.Rows) 
                    {
                        if (isValid)
                        {
                            if (TaskCount >= taskNumber)
                            {
                                orderPayResetEvent.WaitOne();
                            }
                           
                            TaskCount++;

                            var item = new T_NewAccountEntity
                            {
                                UserGuid = row["userguid"] + string.Empty,
                                UserName = row["username"] + string.Empty,
                                PassWord = CXDataCipher.DecipheringUserPW(row["password"] + string.Empty),
                                PassengerId = row["PassengerId"] + string.Empty,
                                State = Convert.ToInt32(row["state"] + string.Empty),
                            };

                            index++;

                            this.Invoke(new Action(() =>
                            {
                                toolStripStatusLabel6.Text = index.ToString();
                            }));

                            accountCollection.Add(item);

                            DeletePassengerActivation login = new DeletePassengerActivation() { Passenger = item };
                            login.DeleteSuccessCount += login_DeleteSuccessCount;
                            login.DeleteCompleted += login_AccountCompleted;

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

        void login_DeleteSuccessCount(object sender, EventArgs e)
        {
            Invoke(new Action(() => {

                lbClearCount.Text =( Convert.ToInt32(lbClearCount.Text) + 1).ToString();
            }));
        }

        void login_OutputMessage(object sender, Fangbian.DataStruct.Business.ConsoleMessageEventArgs e)
        {
            WriteMessage(e.Message);
        }

        void login_AccountCompleted(object sender, DeletePassengerEventArgs e)
        {
            var item = sender as DeletePassengerActivation;

            try
            {
                if (e.IsLogin && e.IsFull == false)
                {
                    UpdateState(20, item.Passenger);
                }
                else if (e.IsLogin && e.IsFull)
                {
                    UpdateState(23, item.Passenger);
                }
                else if (e.IsBad)
                {
                    UpdateState(8, item.Passenger);
                }
                else if (e.IsLogin && e.IsValid == false)
                {
                    UpdateState(5, item.Passenger);
                }
            }
            catch
            {
            }
            finally
            {
                validAccountCollection.Add(item.Passenger);
                SetValidCount();
                TaskCount--;

                orderPayResetEvent.Set();
            }
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
                if (message.Length > 200)
                {
                    message = message.Substring(0, 200) + "...";
                }
                textBox1.Text += "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":" + message;

                textBox1.SelectionStart = this.textBox1.Text.Length;

                textBox1.ScrollToCaret();
            }));
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

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            isValid = false;
        }

        private void toolStripStatusLabel5_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {

        }
    }
}
