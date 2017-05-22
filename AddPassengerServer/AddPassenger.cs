using MyEntiry;
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

namespace AddPassengerServer
{
    public partial class AddPassenger : Form
    {
        public AddPassenger()
        {
            InitializeComponent();
        }

        public string IdNoType { get; set; }

        private AutoResetEvent orderResetEvent = new AutoResetEvent(false);

        List<AddPassengerUser> users = new List<AddPassengerUser>();

        private int successCount = 0;

        public int SuccessCount
        {
            get { return successCount; }
            set
            {
                successCount = value; Invoke(new Action(() =>
                {
                  lbSuccess.Text = "成功数:" + value;
                }));
            }
        }

        private int errorCount = 0;

        public int ErrorCount
        {
            get { return errorCount; }
            set
            {
                errorCount = value;

                Invoke(new Action(() =>
                {
                    lbError.Text = "失败数:" + value;
                }));
            }
        }


        int messageCount = 0;

        /// <summary>
        /// 添加消息到UI中
        /// </summary>
        /// <param name="str"></param>
        private void PutMessage(string str)
        {
            this.Invoke(new Action(() =>
            {
                messageCount++;

                if (messageCount > 100)
                {
                    messageCount = 0;
                    textBox1.Text = string.Empty;
                }

                textBox1.Text += DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + str + "\r\n\r\n";

                textBox1.SelectionStart = this.textBox1.Text.Length;

                textBox1.ScrollToCaret();
            }));
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = false;

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Thread th = new Thread(new ThreadStart(() =>
                {
                    PutMessage("开始读取用户信息");

                    users.Clear();

                    File.ReadAllLines(openFileDialog1.FileName).ToList().ForEach(item =>
                    {
                        var user = item.Split(',');

                        users.Add(new AddPassengerUser
                        {
                            UserGuid = user[0],
                            UserName = user[1],
                            PassWord = user[2],
                            IsSuccess = false
                        });
                    });
                    PutMessage("读取用户信息完毕");

                    var number = Convert.ToInt32(toolStripTextBox1.Text);

                    var data = PD.Business.DataTransaction.Create();

                    PutMessage("开始读取乘车人");

                    var passengers = data.DoGetDataTable("select idNo,name from   t_userpassenger where state="+IdNoType+" limit 0," + users.Count * number);
                    PutMessage("读取乘车人完毕");
                    var source1 = passengers.DefaultView.ToTable("idNo");
                    PutMessage("开始更改乘车人");
                   
                    List<string> sqls = new List<string>();

                    List<T_Passenger> passengerCollection = new List<T_Passenger>();

                    foreach (DataRow row in passengers.Rows)
                    {
                        T_Passenger passenger = new T_Passenger()
                        {
                            IdNo = row["idNo"] + string.Empty,
                            Name = row["name"] + string.Empty
                        };
                        passengerCollection.Add(passenger);
                        sqls.Add("update t_userpassenger set state=3 where idNo='" + row["idNo"] + "'");
                    }

                    data.ExecuteMultiSql(sqls.ToArray());

                    PutMessage("更改乘车人完毕");

                    this.Invoke(new Action(() =>
                    {
                        btnAdd.Enabled = false;
                    }));

                    foreach (var item in users)
                    {
                        if (AddPassengerManagerCollection.Current.Count >= 6)
                        {
                            orderResetEvent.WaitOne();
                        }

                        var currentPassengers = passengerCollection.GetRange(users.IndexOf(item) * number, number);
                        AddPassengerManager manager = new AddPassengerManager();
                        manager.Passengers = currentPassengers;
                        manager.AddPassengerCompleted += manager_AddPassengerCompleted;
                        manager.OutputMessage += manager_OutputMessage;
                        manager.Excute(new Fangbian.Tickets.Trains.WFDataItem.Account12306Item { UserName = item.UserName, PassWord = item.PassWord });
                    }
                    this.Invoke(new Action(() =>
                    {
                        btnAdd.Enabled = true;
                    }));
                }));


                th.Start();
            }
        }

        void manager_OutputMessage(object sender, Fangbian.DataStruct.Business.ConsoleMessageEventArgs e)
        {
            PutMessage(e.Message);
        }

        void manager_AddPassengerCompleted(object sender, AddPassengerEventArgs e)
        {
            var user = sender as AddPassengerManager;
            var s = users.FirstOrDefault(item => item.UserName.Equals(user.CurrentUser.UserName));
            s.IsSuccess = e.IsSuccess;
            if (e.IsSuccess)
            {
                SuccessCount++;
            }
            else
            {
                ErrorCount++;
            }
            orderResetEvent.Set();
        }

        private void lbSuccess_Click(object sender, EventArgs e)
        {
            ShowUserPage p = new ShowUserPage();
            p.SetSource(users.Where(item => item.IsSuccess).ToList());
            p.ShowDialog();
        }

        private void lbError_Click(object sender, EventArgs e)
        {
            ShowUserPage p = new ShowUserPage();
            p.SetSource(users.Where(item => item.IsSuccess==false).ToList());
            p.ShowDialog();
        }

        private void AddPassenger_Load(object sender, EventArgs e)
        {
            DataTypePage page = new DataTypePage();
            page.ShowDialog();
            IdNoType = page.Type.ToString();
            toolStripLabel1.Text = "类型:"+IdNoType+"0后";
        }
    }

    public class AddPassengerUser : T_ValidUser 
    {
        public bool IsSuccess { get; set; }
    }
}
