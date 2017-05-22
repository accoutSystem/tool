using BulidTaskServer.Properties;
using CaptchaServerCacheClient;
using FangBian.Common;
using MyEntiry;
using MyTool.Common;
using PD.Business;
using ResourceBulidTool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace BulidTaskServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Load += Form1_Load;
        }

        void Form1_Load(object sender, EventArgs e)
        {
            //LoadData();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            QueueClient s = new QueueClient()
            {
                Url = "http://121.43.110.247/BaseServer/",
                Auth = "showmethetask",
                Name = "Passenger"
            };
            s.Enqueue("Passenger:" + Guid.NewGuid().ToString());
            QueueClient sa = new QueueClient()
            {
                Url = "http://121.43.110.247/BaseServer/",
                Auth = "showmethetask",
                Name = "Email"
            };
            sa.Enqueue("Email" + Guid.NewGuid().ToString());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            QueueClient s = new QueueClient()
            {
                Url = "http://121.43.110.247/BaseServer/",
                Auth = "showmethetask",
                Name = "Passenger"
            };
            MessageBox.Show(s.Dequeue());

            QueueClient sa = new QueueClient()
            {
                Url = "http://121.43.110.247/BaseServer/",
                Auth = "showmethetask",
                Name = "Email"
            };
            MessageBox.Show(sa.Dequeue());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            QueueClient s = new QueueClient()
            {
                Url = "http://121.43.110.247/BaseServer/",
                Auth = "showmethetask",
                Name = "Passenger"
            };
            MessageBox.Show(s.GetCount() + "");

            QueueClient sa = new QueueClient()
            {
                Url = "http://121.43.110.247/BaseServer/",
                Auth = "showmethetask",
                Name = "Email"
            };
            MessageBox.Show(sa.GetCount() + "");
        }

        private bool isRun = false;
        private bool isRunValid = false;
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (isRun)
            {
                toolStripButton1.Image = Resources.start;
                toolStripButton1.Text = "开始任务";
                isRun = false;
                return;
            }
            else
            {
                isRun = true;
                toolStripButton1.Image = Resources.stop;
                toolStripButton1.Text = "停止任务";

            }

            string url = "http://121.43.110.247/BaseServer/";

            string auth = "showmethetask";

            var emailQueue = new QueueClient()
            {
                Url = url,
                Auth = auth,
                Name = "Email"
            };

            var passengerQueue = new QueueClient()
            {
                Url = url,
                Auth = auth,
                Name = "Passenger"
            };

            int maxPassenger = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["maxData"]);
            int maxEmail = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["maxEmail"]);

            Thread th = new Thread(new ThreadStart(() =>
            {
                while (isRun)
                {
                    try
                    {
                        int count, addCount = 0;
                        ExcuteMessage("开始填充乘客队列");
                        #region 添加乘客
                        count = passengerQueue.GetCount();

                        addCount = maxPassenger - count;
                        ExcuteMessage("读取个数成功"+addCount+"个");
                        if (addCount > 0)
                        {
                            var passengers = GetPassengerServer.Get(addCount);

                            ExcuteMessage("填充" + passengers.Count + "个乘客");

                            foreach (var passenger in passengers)
                            {
                                if (UpdatePassenger(passenger))
                                {
                                    passengerQueue.Enqueue(Newtonsoft.Json.JsonConvert.SerializeObject(passenger));
                                    ExcuteMessage("填充" + passenger.Name + "成功");
                                }
                            }
                        }
                        #endregion
                        ExcuteMessage("填充乘客队列完毕");

                        ExcuteMessage("开始填充邮箱队列");
                        #region 添加邮箱
                        count = emailQueue.GetCount();

                        addCount = maxEmail - count;
                        ExcuteMessage("读取个数成功" + addCount + "个");
                        if (addCount > 0)
                        {
                            var emails = GetEmailServer.Get(addCount);

                            ExcuteMessage("填充" + emails.Count + "个邮箱");

                            foreach (var email in emails)
                            {
                                var data = DataTransaction.Create();
                                data.ExecuteSql("update t_email set state=2,lastTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where emailid='" + email.EmailId + "'");
                                emailQueue.Enqueue(Newtonsoft.Json.JsonConvert.SerializeObject(email));
                                ExcuteMessage("填充" + email.Email + "成功");
                            }
                        }
                        #endregion
                        ExcuteMessage("填充邮箱队列完毕");
                    }
                    catch (Exception ex)
                    {
                        ExcuteMessage("错误:" + ex.Message);
                    }
                    Thread.Sleep(500);
                }

            }));

            th.Start();
        }

        int messageCount = 0;
        private void ExcuteMessage(string message)
        {
            this.Invoke(new Action(() =>
            {
                messageCount++;

                if (messageCount == 100)
                {
                    messageCount = 0;
                    textBox1.Text = string.Empty;
                }

                textBox1.Text += "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "->" + message;

                textBox1.SelectionStart = this.textBox1.Text.Length;

                textBox1.ScrollToCaret();
            }));
        }

        private bool UpdatePassenger(T_Passenger passenger)
        {
            var data = DataTransaction.Create();

            try
            {
                if (ToolCommonMethod.GetChineseSpellCode(passenger.Name).Contains("1"))
                {
                    data.ExecuteSql("update t_passenger set state=3 where passengerId='" + passenger.PassengerId + "'");

                    return false;
                }
            }
            catch
            {
                data.ExecuteSql("update t_passenger set state=3 where passengerId='" + passenger.PassengerId + "'");

                return false;
            }
            data.ExecuteSql("update t_passenger set state=2 where passengerId='" + passenger.PassengerId + "'");

            return true;
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {

            string url = "http://localhost/BaseServer/";

            string auth = "showmethetask";

            var emailQueue = new QueueClient()
            {
                Url = url,
                Auth = auth,
                Name = "Email"
            };

            var passengerQueue = new QueueClient()
            {
                Url = url,
                Auth = auth,
                Name = "Passenger"
            };

            var validQueue = new QueueClient()
            {
                Url = url,
                Auth = auth,
                Name = "valid"
            };

            toolStripStatusLabel1.Text = "邮箱队列:" + emailQueue.GetCount() + " 乘客队列:" + passengerQueue.GetCount() + " 核验队列:" + validQueue.GetCount();

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (isRunValid)
            {
                toolStripButton2.Image = Resources.start;
                toolStripButton2.Text = "开始核验任务";
                isRunValid = false;
                return;
            }
            else
            {
                isRunValid = true;
                toolStripButton2.Image = Resources.stop;
                toolStripButton2.Text = "停止核验任务";

            }

            string url = "http://121.43.110.247/BaseServer/";

            string auth = "showmethetask";

            var validQueue = new QueueClient()
            {
                Url = url,
                Auth = auth,
                Name = "valid"
            };

            int maxValid = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["maxValid"]);

            Thread th = new Thread(new ThreadStart(() =>
            {
                while (isRunValid)
                {
                    try
                    {
                        int count, addCount = 0;

                        #region 添加核验
                        count = validQueue.GetCount();

                        addCount = maxValid - count;
                        if (addCount > 0)
                        {
                            var valid = GetValidServer.Get(addCount);

                            ExcuteMessage("填充" + valid.Count + "个核验邮箱");

                            foreach (var user in valid)
                            {
                                var data = DataTransaction.Create();

                                //7正在进行邮箱核验...

                                data.ExecuteSql("update t_newaccount set state=7,LastTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where UserGuid='" + user.UserGuid + "'");

                                validQueue.Enqueue(Newtonsoft.Json.JsonConvert.SerializeObject(user));

                                ExcuteMessage("填充" + user.UserName + "成功");
                            }
                        }
                        #endregion
                        ExcuteMessage("填充核验邮箱队列完毕");
                    }
                    catch (Exception ex)
                    {
                        ExcuteMessage("错误:" + ex.Message);
                    }
                    Thread.Sleep(500);
                }

            }));

            th.Start();
        }

    }
}
