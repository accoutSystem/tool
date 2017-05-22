using Fangbian.Ticket.Server.AdvanceLogin;
using MyTool.Common;
using MyTool.Properties;
using PD.Business;
using ResourceBulidTool;
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
    public partial class AnalysisPassengerInPC : Form
    {
        public static AnalysisPassengerInPC Current { get; set; }

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

        public AnalysisPassengerInPC()
        {
            InitializeComponent();
            Current = this;
        }

        bool isReadPassenger = false;

        int task = 0;

        public int Task
        {
            get { return task; }
        }

        private void toolStripButton1_Click_2(object sender, EventArgs e)
        {
            GetPassengerActivation_PC login = new GetPassengerActivation_PC() { Data = string.Empty };

            login.ReadPassengerCompleted += login_ReadPassengerCompleted;

            login.OutputMessage += login_OutputMessage;

            login.Read(new Fangbian.Tickets.Trains.WFDataItem.Account12306Item { PassWord = "SX733201", UserName = "SX1994020570" });

        }

        private void btnValid_Click(object sender, EventArgs e)
        {
            isStop = true;

            var db = PD.Business.DataTransaction.Create();

            var source = db.Query(@"select * from t_newaccount where  State=12 and CreateTime<='2016-1-01 23:23:23' limit 0,1").Tables[0];

            string pw=string.Empty;

            string userName = string.Empty;

            if (source.Rows.Count > 0)
            {
                userName = source.Rows[0]["username"] + string.Empty;
                pw = CXDataCipher.DecipheringUserPW(source.Rows[0]["password"] + string.Empty);
            }

            if (string.IsNullOrEmpty(pw))
            {
                return;
            }

            btnValid.Enabled = false;

            GetPassengerActivation_PC login = new GetPassengerActivation_PC() { Data = string.Empty };

            login.ReadPassengerCompleted += login_ReadPassengerCompleted;

            login.OutputMessage += login_OutputMessage;

            login.Data = source.Rows[0]["UserGuid"] + string.Empty;

            login.Read(new Fangbian.Tickets.Trains.WFDataItem.Account12306Item { PassWord = CXDataCipher.DecipheringUserPW(pw), UserName = userName });
           
        }
        void login_ReadPassengerCompleted(object sender, GetPassengerEventArgs e)
        {
            Invoke(new Action(() => { btnValid.Enabled = true; }));
         
            TotalTaskCount++;

            var item = sender as GetPassengerActivation_PC;
        
            if (e.IsLogin)
            {
                if (e.CurrentAccount.CurrentUserPassengers.Count > 1)
                {
                    //存在联系人
                    UpdateAccountState("29",item.CurrentUser.UserName);

                    string path = System.Environment.CurrentDirectory + @"\Data\售出存在联系人账号.txt";

                    Storage(item.CurrentUser.UserName, item.CurrentUser.PassWord, e.CurrentAccount.CurrentUserPassengers.Count + string.Empty, path);
                }
                else 
                {
                    UpdateAccountState("30", item.CurrentUser.UserName);
                }
            }
            if (e.IsBadUser) {
                UpdateAccountState("8", item.CurrentUser.UserName);
            }

            Invoke(new Action(() => {
                if (isStop)
                {
                    btnValid_Click(null, null);
                }
            }));
           
        }
        bool isStop = false;
        private void UpdateAccountState(string state, string userName )
        {
            UpdateAccountState(state, userName, string.Empty);
        }
        private void UpdateAccountState(string state, string userName,string minData)
        {
            try
            {
                var data = DataTransaction.Create();

                data.ExecuteSql("update t_newaccount set   state=" + state + ",LastTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where username='" + userName + "'");

                WriteMessage("修改" + userName + "状态成功");
            }
            catch (Exception ex)
            {
                WriteMessage("修改" + userName + "状态失败" + ex.Message);
            }
        }


        private void Storage(string username,string pw,string count, string path)
        {
            WriteMessage(username + "写入文件");

            try
            {

                FileStream fs = new FileStream(path, FileMode.Append);

                StreamWriter sw = new StreamWriter(fs);

                sw.WriteLine(username + "," + pw + "," + count  );

                sw.Close();

                sw.Dispose();

                WriteMessage(username + "写入文件成功");
            }
            catch (Exception ex)
            {
                WriteMessage(username + "写入文件失败" + ex.Message);
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

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            AddDeletePassenger s = new AddDeletePassenger();
            s.ShowDialog();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            isStop = false;
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {

        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

    
    }
}
