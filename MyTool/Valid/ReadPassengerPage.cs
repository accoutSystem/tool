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
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MyTool.Valid
{
    public partial class ReadPassengerPage : Form
    {
        public static ReadPassengerPage Current { get; set; }

        private AutoResetEvent orderPayResetEvent = new AutoResetEvent(false);

        private int taskCount = 0;

        private int totalTaskCount = 0;

        public double ShowOP { get {
            return Convert.ToDouble(Current.tbOpr.Text);
        } }

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
        List<GetPassengerActivation_PC> getPassengers = new List<GetPassengerActivation_PC>();
        public ReadPassengerPage()
        {
            InitializeComponent();
            Current = this;

            task = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["readPassengerNumber"]);

            toolStripTextBox1.Text = task.ToString();
        }
        bool isReadPassenger = false;
        int task = 0;

        public int Task
        {
            get { return task; }
        }
        private void btnValid_Click(object sender, EventArgs e)
        {
            if (isReadPassenger)
            {
                btnValid.Image = Resources.start;
                btnValid.Text = "开始分析乘车人";
                isReadPassenger = false;
                return;
            }
            else
            {
                isReadPassenger = true;
                btnValid.Image = Resources.stop;
                btnValid.Text = "停止分析乘车人";
            }
            task = Convert.ToInt32(toolStripTextBox1.Text);

            Thread th = new Thread(new ThreadStart(() =>
            {
                while (isReadPassenger)
                {
                    if (ToolCommonMethod.IsProvideServer())
                    {
                        var data = GetPassengerServer.Get();

                        if (data == null)
                        {
                            WriteMessage("账号队列为空");
                            Thread.Sleep(1000);
                            continue;
                        }
                        if (TaskCount >= task)
                        {
                            orderPayResetEvent.WaitOne();
                        }

                        TaskCount++;

                        GetPassengerActivation_PC login = new GetPassengerActivation_PC();

                        getPassengers.Add(login);

                        login.ReadPassengerCompleted += login_ReadPassengerCompleted;

                        login.OutputMessage += login_OutputMessage;

                        login.Data = data.UserGuid;

                        login.Read(new Fangbian.Tickets.Trains.WFDataItem.Account12306Item { PassWord = CXDataCipher.DecipheringUserPW(data.PassWord), UserName = data.UserName });
                    }
                    else
                    {
                        Invoke(new Action(() =>
                        {
                            MessageBox.Show("12点了");
                            btnValid.Image = Resources.start;
                            btnValid.Text = "开始分析乘车人";
                            isReadPassenger = false;
                        }));
                        return;
                    }
                }
            }));

            th.Start();
        }

        void login_ReadPassengerCompleted(object sender, GetPassengerEventArgs e)
        {
            var item = sender as GetPassengerActivation_PC;
            getPassengers.Remove(item);
            TaskCount--;
            TotalTaskCount++;
            orderPayResetEvent.Set();
            if (e.IsSystemError)
            {
                WriteMessage("系统错误回滚状态");

                UpdateReadPassengerState(item.Data + string.Empty, "0", item.CurrentUser.UserName);
            }
            else if (e.IsFormatError)
            {
                WriteMessage("系统解析联系人错误");
                UpdateReadPassengerState(item.Data + string.Empty, "3", item.CurrentUser.UserName);
            }
            else
            {
                if (e.IsLogin)
                {
                    List<string> sqls = new List<string>();

                    var state = "2";

                    var data = DataTransaction.Create();

                    StringBuilder ids = new StringBuilder();

                    foreach (var passenger in e.CurrentAccount.CurrentUserPassengers)
                    {
                        if (passenger.Status == 0 && passenger.IdType == "1")
                        {
                            ids.Append("'" + passenger.IdNo + "',");
                        }
                    }

                    if (ids.Length > 0)
                    {
                        ids.Remove(ids.Length - 1, 1);

                        string sql = string.Format(@"select idNo from  t_passenger where idNo in({0})
                                                      union all
                                                      select idNo from  t_userpassenger where idNo in({0})", ids.ToString());

                        var idSource = data.DoGetDataTable(sql);

                        foreach (var passenger in e.CurrentAccount.CurrentUserPassengers)
                        {
                            if (passenger.Status == 0 && passenger.IdType == "1")
                            {
                                if (idSource.Select("idNo='" + passenger.IdNo + "'").Length <= 0)
                                {
                                    sqls.Add("insert into  t_passenger(passengerId,name,idNo,state) values('" + Guid.NewGuid().ToString() + "','" + passenger.UserName + "','" + passenger.IdNo + "',7)");
                                }
                            }
                            else
                            {
                                state = "3";
                            }
                        }
                    }


                    WriteMessage("分析" + item.CurrentUser.UserName + "中乘车人【" + e.CurrentAccount.CurrentUserPassengers.Count + "】个，有效乘车人【" + sqls.Count() + "】个");

                    if (sqls.Count > 0)
                    {
                        try
                        {
                            data.ExecuteMultiSql(DataUpdateBehavior.Transactional, sqls.ToArray());

                            SuccessPassengerCount += sqls.Count;
                        }
                        catch (Exception x)
                        {
                            ErrorPassengerCount += sqls.Count;
                            WriteMessage("存储" + item.CurrentUser.UserName + "的乘车人失败" + x.Message);
                        }
                    }
                    var passengerCount = e.CurrentAccount.CurrentUserPassengers.Count;
                    if (passengerCount >= 15)
                    {
                        UpdateReadPassengerState(item.Data + string.Empty, state, item.CurrentUser.UserName,passengerCount);
                    }
                    else
                    {
                        UpdateReadPassengerState(item.Data + string.Empty, "5", item.CurrentUser.UserName, passengerCount);
                    }

                    WriteMessage("分析" + item.CurrentUser.UserName + "的乘车人结束");
                }
                else
                {
                    if (e.IsBadUser)
                    {
                        UpdateReadPassengerState(item.Data + string.Empty, "4", item.CurrentUser.UserName);
                    }
                }
            }
          
        }

        private void UpdateReadPassengerState(string userGuid, string state, string userName)
        {
            try
            {
                var data = DataTransaction.Create();

                data.ExecuteSql("update t_newaccount set readPassengerState=" + state + ",LastTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where UserGuid='" + userGuid + "'");

                data.ExecuteSql("update t_hisnewaccount set readPassengerState=" + state + ",LastTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where UserGuid='" + userGuid + "'");

                WriteMessage("修改" + userName + "状态成功");
            }
            catch (Exception ex)
            {
                WriteMessage("修改" + userName + "状态失败" + ex.Message);
            }
        }

        private void UpdateReadPassengerState(string userGuid, string state, string userName,int passengerCount)
        {
            try
            {
                var data = DataTransaction.Create();

                data.ExecuteSql("update t_newaccount set readPassengerState=" + state + ",LastTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where UserGuid='" + userGuid + "'");

                data.ExecuteSql("update t_hisnewaccount set  passengerNumber=" + passengerCount + ",readPassengerState=" + state + ",LastTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where UserGuid='" + userGuid + "'");

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

                if (message.Contains("name")) {
                    Fangbian.Log.Logger.Error(message);
                }
            }));
        }

        private void lbTask_Click(object sender, EventArgs e)
        {
            StringBuilder s = new StringBuilder();
            foreach (var item in getPassengers)
            {
                s.Append(item.CurrentUser.UserName + " " + item.CurrentUser.PassWord + " " + item.CurrentTime.ToString("yyyy-MM-dd HH:mm:ss") + " " + item.CurrentMessage + "\r\n");
            }
            WriteMessage(s.ToString());
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
            task = Convert.ToInt32(toolStripTextBox1.Text);
        }
    }
}
