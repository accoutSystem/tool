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
using System.Xml;
using System.Xml.Serialization;

namespace BulidTaskServer
{
    public partial class BulidTaskPage : Form
    {
        public BulidTaskPage()
        {
            InitializeComponent();
            Load += Form1_Load;
        }

        void Form1_Load(object sender, EventArgs e)
        {
            //LoadData();
        }

        private bool isRun = false;

        private bool isRunValid = false;

        private bool isRunValidPhone = false;
        private bool isCache = false;

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
                        ExcuteMessage("读取个数成功" + addCount + "个");
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

            var validQueue = new QueueClient()
            {
                Url = url,
                Auth = auth,
                Name = "valid"
            };

            var validPhone = new QueueClient()
            {
                Url = url,
                Auth = auth,
                Name = "validPhone"
            };

            toolStripStatusLabel1.Text = "邮箱队列:" + emailQueue.GetCount() + " 乘客队列:" + passengerQueue.GetCount() + " 核验队列:" + validQueue.GetCount() + " 手机核验队列:" + validPhone.GetCount();

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

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (isCache)
            {
                btnCache.Image = Resources.start;
                btnCache.Text = "开始生成缓存数据";
                isCache = false;
                return;
            }
            else
            {
                isCache = true;
                btnCache.Image = Resources.stop;
                btnCache.Text = "停止生成缓存数据";
            }

            Thread th = new Thread(new ThreadStart(() => 
            {
                while (isCache)
                {
                    try
                    {

                        var data = DataTransaction.Create();


                       var source = data.DoGetDataTable(@"  select sum(case state when 0 then 1 else 0 end) 可用联系人,
sum(case state when 1 then 1 else 0 end) 已用联系人,
sum(case state when 2 then 1 else 0 end) 正在使用,0 已存档 from t_passenger   "); 
                        var count = data.DoGetDataTable("select count(*) from t_userpassenger").Rows[0][0];
                        source.Rows[0]["已存档"] = count;
                        HttpHelper.Post("http://121.43.110.247/BaseServer/api/writestatus.ashx", "taskid=passengerCache&val=" + SerializeDataTableXml(source), "UTF-8");

                      var  sql = string.Format(@"select sum(case state when 0 then 1 else 0 end) 可用Email,
sum(case state when 1 then 1 else 0 end) 已用Email,
sum(case state when 2 then 1 else 0 end) 正在使用 ,0 已存档 from t_email ");
                       
                        source = data.DoGetDataTable(sql);
                        count = data.DoGetDataTable("select count(*) from t_useremail").Rows[0][0];
                        source.Rows[0]["已存档"] = count;
                        HttpHelper.Post("http://121.43.110.247/BaseServer/api/writestatus.ashx", "taskid=emailCache&val=" + SerializeDataTableXml(source), "UTF-8");

                          sql = @"select date_format(CreateTime,'%Y-%c-%d') 日期,
           SUM(CASE State WHEN 0 THEN 1 ELSE 0 END) 新资源 ,
           SUM(CASE State WHEN 1 THEN 1 ELSE 0 END) 通过邮箱核验 ,
SUM(CASE State WHEN 5 THEN 1 ELSE 0 END) 未验证 ,
SUM((CASE  State when 2 then  CASE  isActive when 1 then 1  ELSE 0 end   ELSE 0 end)) 可用 ,
SUM(CASE State WHEN 3 THEN 1 ELSE 0 END) 已出 ,
SUM(CASE State WHEN 4 THEN 1 ELSE 0 END) 已出未验证 , 
SUM(CASE State WHEN 6 THEN 1 ELSE 0 END) 已出且验证 ,
SUM(CASE State WHEN 8 THEN 1 ELSE 0 END) 已损坏 ,
SUM(CASE State WHEN 9 THEN 1 ELSE 0 END) 损坏已入库 , 
SUM((CASE  State when 10 then  CASE  isActive when 1 then 1  ELSE 0 end   ELSE 0 end) ) 优质资源 , 
SUM(CASE State WHEN 12 THEN 1 ELSE 0 END) 优质资源已出 , 
SUM(CASE State WHEN 13 THEN 1 ELSE 0 END) 资源被锁定 , 
SUM(CASE State WHEN 11 THEN 1 ELSE 0 END) 正在手机核验 , 
SUM(CASE State WHEN 7 THEN 1 ELSE 0 END)  正在邮件核验,
SUM(CASE State WHEN 14 THEN 1 ELSE 0 END) 核验通过邮件未激活,
SUM(CASE State WHEN 15 THEN 1 ELSE 0 END) 手机核验通过邮件未激活
                            from t_newaccount  
 group by  date_format(CreateTime,'%Y-%c-%d')";
                          source = data.DoGetDataTable(sql);
                        HttpHelper.Post("http://121.43.110.247/BaseServer/api/writestatus.ashx", "taskid=userDetail&val=" + SerializeDataTableXml(source), "UTF-8");

                        sql = @"select  SUM(CASE State WHEN 0 THEN 1 ELSE 0 END) 新资源 ,
           SUM(CASE State WHEN 1 THEN 1 ELSE 0 END) 通过邮箱核验,
SUM(CASE State WHEN 5 THEN 1 ELSE 0 END) 未验证 ,
SUM((CASE  State when 2 then  CASE  isActive when 1 then 1  ELSE 0 end   ELSE 0 end) ) 可用 ,
SUM(CASE State WHEN 3 THEN 1 ELSE 0 END) 已出,
SUM(CASE State WHEN 4 THEN 1 ELSE 0 END) 已出未验证, 
SUM(CASE State WHEN 6 THEN 1 ELSE 0 END) 已出且验证 ,
SUM(CASE State WHEN 8 THEN 1 ELSE 0 END) 已损坏 ,
SUM(CASE State WHEN 9 THEN 1 ELSE 0 END) 损坏已入库 , 
SUM((CASE  State when 10 then  CASE  isActive when 1 then 1  ELSE 0 end   ELSE 0 end) ) 优质资源 , 
SUM(CASE State WHEN 12 THEN 1 ELSE 0 END) 优质资源已出 , 
SUM(CASE State WHEN 13 THEN 1 ELSE 0 END) 资源被锁定 ,
SUM(CASE State WHEN 11 THEN 1 ELSE 0 END) 正在手机核验 , 
SUM(CASE State WHEN 7 THEN 1 ELSE 0 END) 正在邮件核验,
SUM(CASE State WHEN 14 THEN 1 ELSE 0 END) 核验通过邮件未激活,
SUM(CASE State WHEN 15 THEN 1 ELSE 0 END) 手机核验通过邮件未激活
                            from t_newaccount   ";
                        source = data.DoGetDataTable(sql);
                        HttpHelper.Post("http://121.43.110.247/BaseServer/api/writestatus.ashx", "taskid=userTotal&val=" + SerializeDataTableXml(source), "UTF-8");
                    }
                    catch { 
                    
                    }
                    Thread.Sleep(60000);
                }
            }));
            th.Start();
        }
        private static string SerializeDataTableXml(DataTable pDt)
        {
            // 序列化DataTable
            StringBuilder sb = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(sb);
            XmlSerializer serializer = new XmlSerializer(typeof(DataTable));
            serializer.Serialize(writer, pDt);
            writer.Close();

            return sb.ToString();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (isRunValidPhone)
            {
                toolStripButton3.Image = Resources.start;
                toolStripButton3.Text = "开始核验手机";
                isRunValidPhone = false;
                return;
            }
            else
            {
                isRunValidPhone = true;
                toolStripButton3.Image = Resources.stop;
                toolStripButton3.Text = "停止核验手机";
            }

            string url = "http://121.43.110.247/BaseServer/";

            string auth = "showmethetask";

            var validQueue = new QueueClient()
            {
                Url = url,
                Auth = auth,
                Name = "validPhone"
            };

            int maxValid = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["maxValidPhone"]);

            Thread th = new Thread(new ThreadStart(() =>
            {
                while (isRunValidPhone)
                {
                    try
                    {
                        int count, addCount = 0;

                        #region 添加手机核验
                        count = validQueue.GetCount();

                        addCount = maxValid - count;
                        if (addCount > 0)
                        {
                            var valid = GetPhoneValidUser.Get(addCount);

                            ExcuteMessage("填充" + valid.Count + "个手机核验账号");

                            foreach (var user in valid)
                            {
                                var data = DataTransaction.Create();

                                data.ExecuteSql("update t_newaccount set state=11,LastTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where UserGuid='" + user.UserGuid + "'");

                                validQueue.Enqueue(Newtonsoft.Json.JsonConvert.SerializeObject(user));

                                ExcuteMessage("填充" + user.UserName + "成功");
                            }
                        }
                        #endregion
                        ExcuteMessage("填充手机核验账号完毕");
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

        private void toolStripButton4_Click_1(object sender, EventArgs e)
        {
           
        }

        private void 解锁手机核验资源ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定解锁手机核验后失败的资源,建议每天早上解锁一次", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) == System.Windows.Forms.DialogResult.OK)
            {
                Thread th = new Thread(new ThreadStart(() =>
                {
                    var data = DataTransaction.Create();

                    var source = data.DoGetDataTable("select count(1) from t_newaccount where state=13");

                    var count = Convert.ToInt32(source.Rows[0][0]);

                    if (count > 0)
                    {
                        MessageBox.Show("可解锁" + count + "个数据");
                        data.ExecuteSql("update t_newaccount set state=2,LastTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where state=13");
                        MessageBox.Show("解锁成功");
                    }
                    else
                    {
                        MessageBox.Show("没有解锁数据");
                    }
                }));
                th.Start();
            }
        }

        private void 解锁邮件核验资源ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定解锁邮件核验后失败的资源", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) == System.Windows.Forms.DialogResult.OK)
            {
                Thread th = new Thread(new ThreadStart(() =>
                {
                    var data = DataTransaction.Create();

                    var source = data.DoGetDataTable("select count(1) from t_newaccount where state=7");

                    var count = Convert.ToInt32(source.Rows[0][0]);

                    if (count > 0)
                    {
                        MessageBox.Show("可解锁" + count + "个数据");
                        data.ExecuteSql("update t_newaccount set state=0,LastTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where state=7");
                        MessageBox.Show("解锁成功");
                    }
                    else
                    {
                        MessageBox.Show("没有解锁数据");
                    }
                }));
                th.Start();
            }
        }

        private void 解锁正在手机核验资源ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定解锁正在手机核验资源?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) == System.Windows.Forms.DialogResult.OK)
            {
                Thread th = new Thread(new ThreadStart(() =>
                {
                    var data = DataTransaction.Create();

                    var source = data.DoGetDataTable("select count(1) from t_newaccount where state=11");

                    var count = Convert.ToInt32(source.Rows[0][0]);

                    if (count > 0)
                    {
                        MessageBox.Show("可解锁" + count + "个数据");
                        data.ExecuteSql("update t_newaccount set state=2,LastTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where state=11");
                        MessageBox.Show("解锁成功");
                    }
                    else
                    {
                        MessageBox.Show("没有解锁数据");
                    }
                }));
                th.Start();
            }
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void 出完手机核验队列ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            while (true)
            {
                var item = GetPhoneValidServer.Get();
                if (string.IsNullOrEmpty(item))
                {
                    return;
                }
            }
        }

    }
    public class GetPhoneValidServer
    {
        public int i = 0;

        public static string Get()
        {
            string url = "http://121.43.110.247/BaseServer/";

            string auth = "showmethetask";

            var passengerQueue = new QueueClient()
            {
                Url = url,
                Auth = auth,
                Name = "validPhone"
            };
            string data = passengerQueue.Dequeue();

            if (data == "HTTPSQS_GET_END" || string.IsNullOrEmpty(data))
            {
                return string.Empty;
            }
            else
            {
                return data;
            }
        }
    }
}
