using CaptchaServerCacheClient;
using Fangbian.Ticket.Server.AdvanceLogin;
using Maticsoft.Model;
using MyTool.Common;
using MyTool.Properties;
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
using System.Windows.Forms;

namespace MyTool.Valid
{
    public partial class DeletePassengerInPC : Form
    {
        public static DeletePassengerInPC Current { get; set; }

        private AutoResetEvent orderPayResetEvent = new AutoResetEvent(false);

        private int taskCount = 0;

        private int totalTaskCount = 0;

        public int TotalTaskCount
        {
            get { return totalTaskCount; }
            set
            {
                totalTaskCount = value;
                this.Invoke(new Action(() =>
                {
                    lbCountTask.Text = value.ToString();
                }));
            }
        }

        private int successPassengerCount = 0;

        public int SuccessPassengerCount
        {
            get { return successPassengerCount; }
            set
            {
                successPassengerCount = value; this.Invoke(new Action(() =>
                {
                    lbPassengerCount.Text = value.ToString();
                }));
            }
        }

        private int errorPassengerCount = 0;

        public int ErrorPassengerCount
        {
            get { return errorPassengerCount; }
            set
            {
                errorPassengerCount = value; this.Invoke(new Action(() =>
                {
                    lberrorPassenger.Text = value.ToString();
                }));
            }
        }

        public int TaskCount
        {
            get
            {
                return taskCount;
            }
            set
            {
                taskCount = value;
                this.Invoke(new Action(() =>
                {
                    lbTask.Text = value.ToString();
                }));
            }
        }

        public DeletePassengerInPC()
        {
            InitializeComponent();
            Current = this;
        }

        bool isDeletePassenger = false;

        public int Task
        {
            get;
            set;
        }

        public   QueueAccount GetInQueue()
        {
            string url = "http://121.43.110.247/BaseServer/";

            string auth = "showmethetask";

            var passengerQueue = new QueueClient()
            {
                Url = url,
                Auth = auth,
                Name = "clearAccount"
            };
            string data = passengerQueue.Dequeue();

            if (data == "HTTPSQS_GET_END" || string.IsNullOrEmpty(data))
            {
                return null;
            }
            else
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<QueueAccount>(data);
            }
        }
        private void btnValid_Click(object sender, EventArgs e)
        {
            if (isDeletePassenger)
            {
                btnValid.Image = Resources.start;
                btnValid.Text = "开始删除乘车人";
                isDeletePassenger = false;
                return;
            }
            else
            {
                isDeletePassenger = true;
                btnValid.Image = Resources.stop;
                btnValid.Text = "停止删除乘车人";
            }
            btnTaskNumber_Click(null, null);
            Thread th = new Thread(new ThreadStart(() =>
            {
                while (isDeletePassenger)
                {
                    if (ToolCommonMethod.IsProvideServer())
                    {
                        var data = GetInQueue();

                        if (data == null)
                        {
                            WriteMessage("删除联系人队列为空");
                            Thread.Sleep(1000);
                            continue;
                        }
                        if (TaskCount >= Task)
                        {
                            orderPayResetEvent.WaitOne();
                        }

                        TaskCount++;

                        DeletePassengerActivation_PC login = new DeletePassengerActivation_PC() {  Data=string.Empty };
                        
                        login.DeletePassengerCompleted += DeletePassengerCompleted;
                        
                        login.OutputMessage += login_OutputMessage;

                        login.Delete(new Fangbian.Tickets.Trains.WFDataItem.Account12306Item { PassWord = CXDataCipher.DecipheringUserPW(data.Password), UserName = data.UserName });
                    }
                    else
                    {
                        Invoke(new Action(() =>
                        {
                            MessageBox.Show("12点了");
                            btnValid.Image = Resources.start;
                            btnValid.Text = "开始删除乘车人";
                            isDeletePassenger = false;
                        }));
                        return;
                    }
                }
            }));

            th.Start();
        }

        void DeletePassengerCompleted(object sender, GetPassengerEventArgs e)
        {
            Invoke(new Action(() => { btnValid.Enabled = true; }));
         
            TotalTaskCount++;

            var item = sender as DeletePassengerActivation_PC;
        
            if (e.IsLogin)
            {
                if (e.IsDelete)
                {
                    UpdateAccountState("10", item.CurrentUser.UserName);
                }
                if (e.IsTicket) 
                {
                    UpdateAccountState("28", item.CurrentUser.UserName, "存在票");
                }
                if (e.IsYQ)
                {
                    UpdateAccountState("25", item.CurrentUser.UserName,e.Message);
                }
                if (e.IsSystemError) {
                    UpdateAccountState("24", item.CurrentUser.UserName,""); 
                }
            }
            else
            {
                if (e.IsBadUser)
                {
                    UpdateAccountState("8", item.CurrentUser.UserName);
                }
            }
            TaskCount--;
            orderPayResetEvent.Set();
        }
        private void UpdateAccountState(string state, string userName )
        {
            UpdateAccountState(state, userName, string.Empty);
        }
        private void UpdateAccountState(string state, string userName,string minData)
        {
            try
            {
                var data = DataTransaction.Create();

                //data.ExecuteSql("update t_newaccount set readPassengerState=" + state + ",LastTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where UserGuid='" + userGuid + "'");

                data.ExecuteSql("update t_hisnewaccount set deleteMinDate='"+minData+"', state=" + state + ",LastTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where username='" + userName + "'");

                WriteMessage("修改" + userName + "状态成功");
            }
            catch (Exception ex)
            {
                WriteMessage("修改" + userName + "状态失败" + ex.Message);
            }
        }

        void login_OutputMessage(object sender, Fangbian.DataStruct.Business.ConsoleMessageEventArgs e)
        {
            WriteMessage(e.Message);
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

                if (message.Contains("name"))
                {
                    Fangbian.Log.Logger.Error(message);
                }
            }));
        }

        private void lbTask_Click(object sender, EventArgs e)
        {
          
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定?", "确定?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Cancel)
                return;
            Thread th = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    var data = GetPassengerServer.Get();

                    if (data == null)
                    {
                        break;
                    }
                    else
                    {
                        WriteMessage("获取数据:" + data.UserName);
                    }
                }
            }));

            th.Start();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            AddDeletePassenger s = new AddDeletePassenger();
            s.ShowDialog();
        }
         

        private void btnTaskNumber_Click(object sender, EventArgs e)
        {
            Task = Convert.ToInt32(tbTaskNumber.Text);
        }
    }
}
