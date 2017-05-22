using BulidTaskServer.Page;
using BulidTaskServer.Properties;
using BulidTaskServer.Task;
using CaptchaServerCacheClient;
using FangBian.Common;
using MyEntiry;
using MyTool.Common;
using MyTool.Valid;
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
            //cmbPassengerMove.SelectedIndex = 0;
            //toolStripComboBox1.SelectedIndex = 1;
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
                toolStripButton1.Text = "开始注册";
                isRun = false;
                return;
            }
            else
            {
                isRun = true;
                toolStripButton1.Image = Resources.stop;
                toolStripButton1.Text = "停止注册";
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
          
            var goodPassengerQueue = new QueueClient()
            {
                Url = url,
                Auth = auth,
                Name = "goodPassenger"
            };
              
            int maxPassenger = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["maxData"]);
            int maxEmail = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["maxEmail"]);

            Thread th = new Thread(new ThreadStart(() =>
            {
                while (isRun)
                {
                    try
                    {

                        #region 乘客失败
                        //ExcuteMessage("开始填充优质乘客");
                        //#region 添加优质乘客
                        //count = goodPassengerQueue.GetCount();

                        //addCount = maxPassenger - count;
                        //ExcuteMessage("读取个数成功" + addCount + "个");
                        //if (addCount > 0)
                        //{
                        //    var passengers = GetPassengerServer.Get(addCount,4,isNew);

                        //    ExcuteMessage("填充" + passengers.Count + "个优质乘客");

                        //    foreach (var passenger in passengers)
                        //    {
                        //        if (UpdatePassenger(passenger,5))
                        //        {
                        //            goodPassengerQueue.Enqueue(Newtonsoft.Json.JsonConvert.SerializeObject(passenger));
                        //            ExcuteMessage("填充优质乘客" + passenger.Name + "成功");
                        //        }
                        //    }
                        //}
                        //#endregion

                        //ExcuteMessage("填充优质乘客队列完毕");
                        #endregion

                        int count, addCount = 0;

                        ExcuteMessage("开始填充乘客队列");
                        #region 添加乘客
                        count = passengerQueue.GetCount();

                        addCount = maxPassenger - count;
                        ExcuteMessage("读取个数成功" + addCount + "个");
                        if (addCount > 0)
                        {
                            var move = (cmbPassengerMove.SelectedItem + string.Empty).Split('|')[0];

                            var passengers = GetPassengerServer.Get(addCount, 0, false, move);

                            ExcuteMessage("填充" + passengers.Count + "个乘客");

                            foreach (var passenger in passengers)
                            {
                                if (UpdatePassenger(passenger,2))
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
                    Thread.Sleep(2000);
                }

            }));

            th.Start();
        }

        int messageCount = 0;
        private void ExcuteMessage(string message)
        {
            if (isShowLog) { 
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
        }

        private bool UpdatePassenger(T_Passenger passenger,int state)
        {
            var data = DataTransaction.Create();

            //try
            //{ 
            //    //if (passenger.Name.Length * 2 > 20)
            //    if (ToolCommonMethod.GetChineseSpellCode(passenger.Name).Contains("1"))
            //    {
            //        data.ExecuteSql("update t_passenger set state=3 where passengerId='" + passenger.PassengerId + "'");
            //        return false;
            //    }
            //}
            //catch
            //{
            //    data.ExecuteSql("update t_passenger set state=7 where passengerId='" + passenger.PassengerId + "'");
            //    return false;
            //}

            data.ExecuteSql("update t_passenger set state="+state+" where passengerId='" + passenger.PassengerId + "'");

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

            var readPassenger = new QueueClient()
            {
                Url = url,
                Auth = auth,
                Name = "readPassenger"
            };

            var goodPassenger = new QueueClient()
            {
                Url = url,
                Auth = auth,
                Name = "goodPassenger"
            };

            var validPassenger = new QueueClient()
            {
                Url = url,
                Auth = auth,
                Name = "readAddValidAccount"
            };

            var asyPassenger = new QueueClient()
            {
                Url = url,
                Auth = auth,
                Name = "asyPassenger"
            };

            var clearAccount = new QueueClient()
            {
                Url = url,
                Auth = auth,
                Name = "clearAccount"
            };
            //邮箱核验队列:" + validQueue.GetCount() +  【优质乘客队列:" + goodPassenger.GetCount() + "】 
            //    " 手机核验队列:" + validPhone.GetCount() + 
            //    " 注册验证账号队列:" + validPassenger.GetCount();
            toolStripStatusLabel1.Text =
                "【邮箱队列:" + emailQueue.GetCount() + "】【乘客队列:" + passengerQueue.GetCount() + "】【分析身份证队列:" + asyPassenger.GetCount() +
                "】【提取身份证账号队列:" + readPassenger.GetCount() + "】"
               + "【删除联系人账号队列:" + clearAccount.GetCount() + "】";
        }

        /// <summary>
        /// 邮箱核验任务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ValidEmailClick(object sender, EventArgs e)
        {
            if (isRunValid)
            {
                toolStripButton2.Image = Resources.start;
                toolStripButton2.Text = "开始邮箱核验";
                isRunValid = false;
                return;
            }
            else
            {
                isRunValid = true;
                toolStripButton2.Image = Resources.stop;
                toolStripButton2.Text = "停止邮箱核验";
            }
            SelectAccountTypePage page = new SelectAccountTypePage();
            page.ShowDialog();
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
                            var valid = GetValidServer.Get(addCount,page.AccountType);

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
                    Thread.Sleep(5000);
                }

            }));

            th.Start();
        }

        /// <summary>
        /// 开始创建汇总数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateCollectClick(object sender, EventArgs e)
        {
            if (isCache)
            {
                btnCache.Image = Resources.start;
                btnCache.Text = "开始汇总";
                isCache = false;
                return;
            }
            else
            {
                isCache = true;
                btnCache.Image = Resources.stop;
                btnCache.Text = "停止汇总";
            }

            Thread th = new Thread(new ThreadStart(() => 
            {
                while (isCache)
                {
                    try
                    {

                        var data = DataTransaction.Create();
                        string sql = string.Empty;

                        #region passenger
                       var source = data.DoGetDataTable(@"select sum(case state when 0 then 1 else 0 end) 可用, 
sum(case state when 2 then 1 else 0 end) 注册锁定,
sum(case state when 3 then 1 else 0 end) 生僻,
sum(case state when 4 then 1 else 0 end) 可用优质身份证,
sum(case state when 5 then 1 else 0 end) 注册锁定优质身份证,
sum(case state when 1 then 1 else 0 end) 已被注册,
sum(case state when 7 then 1 else 0 end) 待分析,
sum(case state when 8 then 1 else 0 end) 锁定待分析,
0 已存档,'抓取' 状态 from t_passenger where move=0
union
select sum(case state when 0 then 1 else 0 end) 可用, 
sum(case state when 2 then 1 else 0 end) 注册锁定,
sum(case state when 3 then 1 else 0 end) 生僻,
sum(case state when 4 then 1 else 0 end) 可用优质身份证,
sum(case state when 5 then 1 else 0 end) 注册锁定优质身份证,
sum(case state when 1 then 1 else 0 end) 已被注册,
sum(case state when 7 then 1 else 0 end) 待分析,
sum(case state when 8 then 1 else 0 end) 锁定待分析,
0 已存档,'迁移分析' 状态  from t_passenger where move=1
union
select sum(case state when 0 then 1 else 0 end) 可用, 
sum(case state when 2 then 1 else 0 end) 注册锁定,
sum(case state when 3 then 1 else 0 end) 生僻,
sum(case state when 4 then 1 else 0 end) 可用优质身份证,
sum(case state when 5 then 1 else 0 end) 注册锁定优质身份证,
sum(case state when 1 then 1 else 0 end) 已被注册,
sum(case state when 7 then 1 else 0 end) 待分析,
sum(case state when 8 then 1 else 0 end) 锁定待分析,
0 已存档,'生僻迁移' 状态  from t_passenger where move=2");
                        var count = data.DoGetDataTable("select count(*) from t_userpassenger").Rows[0][0];
                        source.Rows[0]["已存档"] = count;

                        HttpHelper.Post("http://121.43.110.247/BaseServer/api/writestatus.ashx", "taskid=passengerCache&val=" + SerializeDataTableXml(source), "UTF-8");
                        ExcuteMessage("写入联系人缓存成功");
                        #endregion

                        #region Email
                        sql = string.Format(@"select sum(case state when 0 then 1 else 0 end) 可用Email,
sum(case state when 1 then 1 else 0 end) 已用Email,
sum(case state when 2 then 1 else 0 end) 正在使用 ,0 已存档 from t_email ");

                        source = data.DoGetDataTable(sql);
                        count = data.DoGetDataTable("select count(*) from t_useremail").Rows[0][0];
                        source.Rows[0]["已存档"] = count;
                        HttpHelper.Post("http://121.43.110.247/BaseServer/api/writestatus.ashx", "taskid=emailCache&val=" + SerializeDataTableXml(source), "UTF-8");
                        ExcuteMessage("写入Email缓存成功");
                        #endregion

                        #region account cache

                        sql = @"select 
    '' 状态,
    0 新资源,
    0 核验资源,
    0 核验资源已售, 
    0 正常资源已售,
    0 身份未验证,
    0 无法登陆,
    0 删除联系人,
    0 删除成功,
    0 未到指定日期,
    0 入删除对列,
    0 最近有购票记录
from
    t_newaccount
where
    1 = 2"; 

//                        24 需要删除联系人的帐号
//25 帐号需要到指定的日期才能删除联系人
//31 删除联系人入对列
//28 删除联系人时发现最近一个月购买过票
                       source = data.DoGetDataTable(sql);
                       ExcuteMessage("开始统计t_newaccount数据");
                       var newAccount = data.DoGetDataTable("select count(*) cnt,state from t_newaccount where accounttype=0 group by state");

                       var newAccount2 = data.DoGetDataTable("select count(*) cnt,state from t_newaccount where accounttype=5 group by state");
                       var newAccount3 = data.DoGetDataTable("select count(*) cnt,state from t_newaccount where accounttype=3 group by state");
                       var newAccount4 = data.DoGetDataTable("select count(*) cnt,state from t_newaccount where accounttype=1 group by state");
                       ExcuteMessage("开始统计t_hisnewAccount数据");
                       var hisnewAccount = data.DoGetDataTable("select count(*) cnt,state from t_hisnewAccount group by state");

                       var newRows = source.NewRow();
                       source.Rows.Add(newRows);
                       newRows["状态"] = "普通号";
                       newRows["新资源"] = GetAccountType("0", newAccount, hisnewAccount);
                       newRows["核验资源"] = GetAccountType("10", newAccount, null);
                       newRows["核验资源已售"] = GetAccountType("12", newAccount, hisnewAccount);
                       newRows["正常资源已售"] = GetAccountType("6", newAccount, hisnewAccount);
                       newRows["身份未验证"] = GetAccountType("5", newAccount, hisnewAccount);
                       newRows["无法登陆"] = GetAccountType("8", newAccount, hisnewAccount);
                       newRows["删除联系人"] = GetAccountType("24", newAccount, hisnewAccount);
                       newRows["删除成功"] = GetAccountType("10", null, hisnewAccount);
                       newRows["未到指定日期"] = GetAccountType("25", newAccount, hisnewAccount);
                       newRows["入删除对列"] = GetAccountType("31", newAccount, hisnewAccount);
                       newRows["最近有购票记录"] = GetAccountType("28", newAccount, hisnewAccount);

                       var newRows1 = source.NewRow();
                       source.Rows.Add(newRows1);
                       newRows1["状态"] = "翻新号";
                       newRows1["新资源"] = GetAccountType("0", newAccount2, null);
                       newRows1["核验资源"] = GetAccountType("10", newAccount2, null);
                       newRows1["核验资源已售"] = GetAccountType("12", newAccount2, null);

                       var newRows2 = source.NewRow();
                       source.Rows.Add(newRows2);
                       newRows2["状态"] = "密码一致";
                       newRows2["新资源"] = GetAccountType("0", newAccount3, null);
                       newRows2["核验资源"] = GetAccountType("10", newAccount3, null);
                       newRows2["核验资源已售"] = GetAccountType("12", newAccount3, null);

                       var newRows3 = source.NewRow();
                       source.Rows.Add(newRows3);
                       newRows3["状态"] = "连号";
                       newRows3["新资源"] = GetAccountType("0", newAccount4, null);
                       newRows3["核验资源"] = GetAccountType("10", newAccount4, null);
                       newRows3["核验资源已售"] = GetAccountType("12", newAccount4, null);
                    
                        source.AcceptChanges();
 
                        HttpHelper.Post("http://121.43.110.247/BaseServer/api/writestatus.ashx", "taskid=userTotal&val=" + SerializeDataTableXml(source), "UTF-8");
                        ExcuteMessage("写入数据缓存成功");
                        #endregion
                    
                    }
                    catch(Exception ex)
                    {
                        ExcuteMessage(ex.Message);
                    }
                    for (int i = 0; i < 30; i++)
                    {
                        if (isCache)
                        {
                            Thread.Sleep(1000);
                        }
                    }
                }
            }));
            th.Start();
        }

        private string GetAccountType(string type,DataTable newAccount,DataTable hisNewAccount)
        {
            var count = 0;
           
            if (newAccount != null)
            {
                var source = newAccount.Select("state='" + type + "'");
                if (source.Length > 0)
                {
                    count += Convert.ToInt32(source[0][0] + string.Empty);
                }
            }
            if (hisNewAccount != null)
            {
                var his = hisNewAccount.Select("state='" + type + "'");

                if (his.Length > 0)
                {
                    count += Convert.ToInt32(his[0][0] + string.Empty);
                }
            }
        
           
            ExcuteMessage(type+"缓存写入成功");

            return count+string.Empty;
        }

        bool isDayCache = false;

        /// <summary>
        /// 开始按天创建汇总数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateCollectInDayClick(object sender, EventArgs e)
        {
            if (isDayCache)
            {
                btnDayCache.Image = Resources.start;
                btnDayCache.Text = "开始按天汇总";
                isDayCache = false;
                return;
            }
            else
            {
                isDayCache = true;
                btnDayCache.Image = Resources.stop;
                btnDayCache.Text = "停止按天汇总";
            }

            Thread th = new Thread(new ThreadStart(() =>
            {
                while (isDayCache)
                {
                    try
                    {
                        var data = DataTransaction.Create();

                        string sql = @"select date_format(CreateTime,'%Y-%c-%d') 日期,
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
                        var source = data.DoGetDataTable(sql);
                        HttpHelper.Post("http://121.43.110.247/BaseServer/api/writestatus.ashx", "taskid=userDetail&val=" + SerializeDataTableXml(source), "UTF-8");
                        ExcuteMessage("按天创建汇总数据写入缓存成功");
                    }
                    catch
                    {

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

            SelectAccountTypePage page = new SelectAccountTypePage();

            page.ShowDialog();

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
                            var valid = GetPhoneValidUser.Get(addCount, page.AccountType);

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
                    Thread.Sleep(3000);
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

        private void 解锁ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定解锁邮件核验后锁定的资源", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) == System.Windows.Forms.DialogResult.OK)
            {
                Thread th = new Thread(new ThreadStart(() =>
                {
                    var data = DataTransaction.Create();

                    var source = data.DoGetDataTable("select count(1) from t_newaccount where state=16");

                    var count = Convert.ToInt32(source.Rows[0][0]);

                    if (count > 0)
                    {
                        MessageBox.Show("可解锁" + count + "个数据");
                        data.ExecuteSql("update t_newaccount set state=0,LastTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where state=16");
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
                var item = GetQueueServer.Get();
                if (string.IsNullOrEmpty(item))
                {
                    return;
                }
            }
        }

        private void 释放身份证ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定释放身份证?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) == System.Windows.Forms.DialogResult.OK)
            {
                Thread th = new Thread(new ThreadStart(() =>
                {
                    var data = DataTransaction.Create();
                    var source = data.DoGetDataTable("select count(1) from t_passenger where state=2");

                    var count = Convert.ToInt32(source.Rows[0][0]);

                    if (count > 0)
                    {
                        MessageBox.Show("可解锁" + count + "个数据");
                        data.ExecuteSql("update t_passenger set state=0  where state=2");
                        MessageBox.Show("解锁成功");
                    }
                  
                }));
                th.Start();
            }
        }

        private void 释放锁定邮件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定释放锁定邮件?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) == System.Windows.Forms.DialogResult.OK)
            {
                var data = DataTransaction.Create();
                var source = data.DoGetDataTable("select count(1) from t_email where state=2");

                    var count = Convert.ToInt32(source.Rows[0][0]);

                    if (count > 0)
                    {
                        MessageBox.Show("可解锁" + count + "个数据");
                        data.ExecuteSql("update t_email set state=0  where state=2");
                        MessageBox.Show("解锁成功");
                    }
            }
        }

        bool isReadPassenger = false;
        private void ReadPassengerClick(object sender, EventArgs e)
        {
            if (isReadPassenger)
            {
                btnReadPassenger.Image = Resources.start;
                btnReadPassenger.Text = "开始提取账号乘车人";
                isReadPassenger = false;
                return;
            }
            else
            {
                isReadPassenger = true;
                btnReadPassenger.Image = Resources.stop;
                btnReadPassenger.Text = "停止提取账号乘车人";
            }

            string url = "http://121.43.110.247/BaseServer/";

            string auth = "showmethetask";

            var readPassenger = new QueueClient()
            {
                Url = url,
                Auth = auth,
                Name = "readPassenger"
            };

            Thread th = new Thread(new ThreadStart(() =>
          {
              while (isReadPassenger)
              {
                  int count, addCount = 0;

                  #region 

                  count = readPassenger.GetCount();

                  addCount = 200 - count;

                  if (addCount > 0)
                  {
                      var valid = GetReadPassengerServer.Get(addCount);

                      ExcuteMessage("填充" + valid.Count + "个读取乘车人账号");

                      foreach (var user in valid)
                      {
                          var data = DataTransaction.Create();

                          data.ExecuteSql("update t_hisnewaccount set readPassengerState=1,LastTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where UserGuid='" + user.UserGuid + "'");

                          readPassenger.Enqueue(Newtonsoft.Json.JsonConvert.SerializeObject(user));

                          ExcuteMessage("填充读取乘车人账号" + user.UserName + "成功");
                      }
                  }
                  #endregion
                  ExcuteMessage("填充读取乘车人账号完毕");
                  Thread.Sleep(3000);
              }
          }));
            th.Start();
        }

        private void 释放读取乘车人ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定释放锁定的正在读取联系人账号?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) == System.Windows.Forms.DialogResult.OK)
            {
                var data = DataTransaction.Create();
                var source = data.DoGetDataTable("select count(1) from t_newaccount where readPassengerState=1");

                var count = Convert.ToInt32(source.Rows[0][0]);

                if (count > 0)
                {
                    MessageBox.Show("可解锁" + count + "个数据");
                    data.ExecuteSql("update t_newaccount set readPassengerState=0  where readPassengerState=1");
                    MessageBox.Show("解锁正在读取联系人账号成功");
                }
            }
        }


        TaskTickBulid autoTaskBulid = null;
        /// <summary>
        /// 11点后的自动任务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAutoTask_Click(object sender, EventArgs e)
        {
            if (autoTaskBulid == null)
            {
                autoTaskBulid = new TaskTickBulid();
                autoTaskBulid.OutputMessage += autoTaskBulid_OutputMessage;
                autoTaskBulid.RunChange += autoTaskBulid_RunChange;
            }
            if (autoTaskBulid.IsRun)
            {
                autoTaskBulid.Stop();
                autoTaskBulid = null;
            }
            else
            {
                autoTaskBulid.Start();
            }
        }

        void autoTaskBulid_RunChange(object sender, EventArgs e)
        {
            var item = sender as TaskTickBulid;
            Invoke(new Action(() =>
            {
                if (item.IsRun)
                {
                    btnAutoTask.Image = Resources.stop;
                    btnAutoTask.Text = "停止自动任务";
                }
                else
                {
                    btnAutoTask.Image = Resources.start;
                    btnAutoTask.Text = "开始自动任务";
                }
            }));
        }

        void autoTaskBulid_OutputMessage(object sender, ConsoleMessageEventArgs e)
        {
            ExcuteMessage(e.Message);
        }

        private bool isReadValidAccount = false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (isReadValidAccount)
            {
                btnReadValidAccount.Image = Resources.start;
                btnReadValidAccount.Text = "开始读取乘车人账号";
                isReadValidAccount = false;
                return;
            }
            else
            {
                isReadValidAccount = true;
                btnReadValidAccount.Image = Resources.stop;
                btnReadValidAccount.Text = "停止读取乘车人账号";
            }

            string url = "http://121.43.110.247/BaseServer/";

            string auth = "showmethetask";

            var validPassengerQueue = new QueueClient()
            {
                Url = url,
                Auth = auth,
                Name = "readAddValidAccount"
            };

            int maxValid =30;

            Thread th = new Thread(new ThreadStart(() =>
            {
                while (isReadValidAccount)
                {
                    try
                    {
                        int count, addCount = 0;

                        #region 添加注册使用的验证码身份证账号
                        count = validPassengerQueue.GetCount();

                        addCount = maxValid - count;

                        if (addCount > 0)
                        {
                            var valid = GetValidPassengerServer.Get(addCount);

                            ExcuteMessage("开始填充" + valid.Count + "个注册使用的验证身份证账号");

                            foreach (var user in valid)
                            {
                                var data = DataTransaction.Create();

                                data.ExecuteSql("update t_newaccount set state=21,LastTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where UserGuid='" + user.UserGuid + "'");

                                validPassengerQueue.Enqueue(Newtonsoft.Json.JsonConvert.SerializeObject(user));

                                ExcuteMessage("填充验证身份证账号" + user.UserName + "成功");
                            }
                        }
                        #endregion
                        ExcuteMessage("填充注册使用的验证码身份证账号完毕");
                    }
                    catch (Exception ex)
                    {
                        ExcuteMessage("错误:" + ex.Message);
                    }
                    Thread.Sleep(10000);
                }

            }));

            th.Start();
        }
        /// <summary>
        /// 释放注册添加联系人的账号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param> 
        private void 释放注册添加联系人的账号ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定解锁核验注册联系人的账号?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) == System.Windows.Forms.DialogResult.OK)
            {
                Thread th = new Thread(new ThreadStart(() =>
                {
                    var data = DataTransaction.Create();

                    var source = data.DoGetDataTable("select count(1) from t_newaccount where state=21");

                    var count = Convert.ToInt32(source.Rows[0][0]);

                    if (count > 0)
                    {
                        MessageBox.Show("可解锁" + count + "个数据");
                        data.ExecuteSql("update t_newaccount set state=20,LastTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where state=21");
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

        private void 释放锁定的优质身份证ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var data = PD.Business.DataTransaction.Create();
            ExcuteMessage("开始修改修改乘车人状态从优质锁定到优质可用");
            data.ExecuteSql("update t_passenger set state=4  where state=5");
            ExcuteMessage("修改修改乘车人状态从优质锁定到优质可用成功");
        }

        private void 账号数据迁移ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MoveAccount a = new MoveAccount();
            a.Show();
        }

        bool isDeletePassenger = false;
        private void DeletePassengerClick(object sender, EventArgs e)
        {
            if (isDeletePassenger)
            {
                btnDeletePassenger.Image = Resources.start;
                btnDeletePassenger.Text = "开始清理账号联系人";
                isDeletePassenger = false;
                return;
            }
            else
            {
                isDeletePassenger = true;
                btnDeletePassenger.Image = Resources.stop;
                btnDeletePassenger.Text = "停止清理账号联系人";
            }

            string url = "http://121.43.110.247/BaseServer/";

            string auth = "showmethetask";

            var clearAccount = new QueueClient()
            {
                Url = url,
                Auth = auth,
                Name = "clearAccount"
            };

            int max = 20;

            Thread th = new Thread(new ThreadStart(() =>
            {
                while (isDeletePassenger)
                {
                    try
                    { 
                        int count, addCount = 0;

                        ExcuteMessage("开始获取需要清理的联系人账号数据");
                        #region 添加清理的联系人账号数据
                        count = clearAccount.GetCount();

                        addCount = max - count;

                        ExcuteMessage("需要填充" + addCount + "个清理的联系人账号数据");

                        if (addCount > 0)
                        {
                            var accounts = GetAccountServer.GetData(addCount);

                            ExcuteMessage("填充" + accounts.Count + "个清理的联系人账号数据");

                            foreach (var account in accounts)
                            {
                                if (UpdateAccountState("31", account.UserName))
                                {
                                    clearAccount.Enqueue(Newtonsoft.Json.JsonConvert.SerializeObject(account));
                                    ExcuteMessage("填充清理账号" + account.UserName+ "成功");
                                }
                            }
                        }
                        #endregion
                        ExcuteMessage("填充需要清理的账号数据成功");
                    }
                    catch (Exception ex)
                    {
                        ExcuteMessage("错误:" + ex.Message);
                    }
                    Thread.Sleep(4000);
                }

            }));

            th.Start();
        }

        private bool UpdateAccountState(string state, string userName )
        {
            try
            {
                var data = DataTransaction.Create();

                data.ExecuteSql("update t_hisnewaccount set   state=" + state + ",LastTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where username='" + userName + "'");

                ExcuteMessage("修改" + userName + "状态成功");

                return true;
            }
            catch (Exception ex)
            {
                ExcuteMessage("修改" + userName + "状态失败" + ex.Message);
                return false;
            }
        }

        private void 恢复分析账号乘车人状态ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 s = new Form1();
            s.ShowDialog();
        }

        private void toolStripButton4_Click_3(object sender, EventArgs e)
        { 
            var state = toolStripComboBox1.SelectedItem+string.Empty;
            if (string.IsNullOrEmpty(state))
            {
                MessageBox.Show("选择状态");
                return;
            }
            state = state.Split(',')[0];
            var startNumber=toolStripTextBox2.Text;
            var count = Convert.ToInt32(toolStripTextBox1.Text);
            var number = count / 50;
            btnMove.Enabled = false;
            Thread th = new Thread(new ThreadStart(() =>
            {
                //for (int i = 0; i < number; i++)
                //{
                    try
                    {
                        string sql = "select * from t_userpassenger where state=" + state + " and idt_userPassenger>=" + startNumber + "   limit 0,  " + count;

                        var data = DataTransaction.Create();

                        DataTable source = data.Query(sql).Tables[0];

                        //if (source.Rows.Count <= 0)
                        //{
                        //    break;
                        //}
                        List<string> sqls = new List<string>();
                        foreach (DataRow row in source.Rows)
                        {
                            string name = row["name"] + string.Empty;
                            string idNo = row["idNo"] + string.Empty;
                            sqls.Add("delete from t_userpassenger where idt_userPassenger='" + row["idt_userPassenger"] + "'");

                            sqls.Add("insert into t_passenger(passengerId,name,idNo,state,move) values('" + Guid.NewGuid().ToString() + "','" + name + "','" + idNo + "','7','1')");
                            if (sqls.Count >= 100)
                            {
                                data.ExecuteMultiSql(DataUpdateBehavior.Transactional, sqls.ToArray());
                                sqls = new List<string>();
                            }
                            ExcuteMessage("构建分析生僻身份证" + name + " " + idNo + "脚本成功");
                            Invoke(new Action(() =>
                            {
                                btnMove.Text = "已转移" + (source.Rows.IndexOf(row) + 1) + "行";
                            }));
                        }

                        if (sqls.Count > 0) {
                            data.ExecuteMultiSql(DataUpdateBehavior.Transactional, sqls.ToArray());
                        }
                      
                        //ExcuteMessage("需重新分析生僻身份证转移50个成功");
                    }
                    catch (Exception ex)
                    {
                        ExcuteMessage("转移50个联系人失败" + ex.Message);
                    }
                //}

                Invoke(new Action(() =>
                {
                    btnMove.Text = "转移并分析";

                    btnMove.Enabled = true;
                }));
            }));
            th.Start();
        }
        bool isShowLog = true;
        private void btnShowLog_Click(object sender, EventArgs e)
        {
            if (isShowLog)
            {
                btnShowLog.Image = Resources.start;
                btnShowLog.Text = "启用日志输出";
            }
            else {
                btnShowLog.Image = Resources.stop;
                btnShowLog.Text = "禁用日志输出";
            }

            isShowLog = !isShowLog;
        }

        private void PassengerStateChanged(object sender, EventArgs e)
        {
            GetMinMaxValue();
        }

        private void toolStripLabel4_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定重新刷新值", "", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK) {
                GetMinMaxValue();
            }
        }

        private void GetMinMaxValue() {
            toolStrip3.Enabled = false;
            var state = toolStripComboBox1.SelectedItem + string.Empty;
            Thread th = new Thread(new ThreadStart(() =>
            {
                try
                {
                    if (string.IsNullOrEmpty(state))
                    {
                        MessageBox.Show("选择状态");
                        return;
                    }
                    state = state.Split(',')[0];

                    var db = PD.Business.DataTransaction.Create();

                    var minsource = db.Query(@"select  idt_userPassenger from t_userpassenger where state=" + state + " limit 0,1 ").Tables[0];

                    var maxsource = db.Query(@"select  idt_userPassenger from t_userpassenger where state=" + state + " order by idt_userPassenger  desc limit 0,1").Tables[0];
                    Invoke(new Action(() =>
                    {
                        if (minsource.Rows.Count > 0)
                        {
                            toolStripLabel4.Text = "Min:" + minsource.Rows[0][0] + string.Empty;
                        }
                        if (maxsource.Rows.Count > 0)
                        {
                            toolStripLabel4.Text = toolStripLabel4.Text + " Max:" + maxsource.Rows[0][0] + string.Empty;
                        }
                        toolStrip3.Enabled = true;
                    }));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Invoke(new Action(() =>
                    {

                        toolStrip3.Enabled = true;
                    }));
                }
            }));
            th.Start();
        }

        private void 生僻身份证转移ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PassengerMove move = new PassengerMove();
            move.Show();
        }
        bool isAsyPassenger = false;
        private void toolStripButton4_Click_4(object sender, EventArgs e)
        {
            if (isAsyPassenger)
            {
                btnAsyPassenger.Image = Resources.start;
                btnAsyPassenger.Text = "开始分析身份证";
                isAsyPassenger = false;
                return;
            }
            else
            {
                isAsyPassenger = true;
                btnAsyPassenger.Image = Resources.stop;
                btnAsyPassenger.Text = "停止分析身份证";
            }

            string url = "http://121.43.110.247/BaseServer/";

            string auth = "showmethetask";

            var asyPassenger = new QueueClient()
            {
                Url = url,
                Auth = auth,
                Name = "asyPassenger"
            };

            Thread th = new Thread(new ThreadStart(() =>
            {
                while (isAsyPassenger)
                {
                    int count, addCount = 0;

                    ExcuteMessage("开始填充需要分析的身份证");
                    #region 添加优质乘客
                    count = asyPassenger.GetCount();

                    addCount = 200 - count;

                    ExcuteMessage("队列未满,需要填充分析的身份证" + addCount + "个");
                    if (addCount > 0)
                    {
                        var passengers = GetPassengerServer.GetAll(addCount, 7);

                        ExcuteMessage("填充" + passengers.Count + "个需要分析的身份证");

                        foreach (var passenger in passengers)
                        {
                            if (UpdatePassenger(passenger, 8))
                            {
                                asyPassenger.Enqueue(Newtonsoft.Json.JsonConvert.SerializeObject(passenger));
                                ExcuteMessage("填充需要分析的身份证" + passenger.Name + "成功");
                            }
                            else
                            {
                                Fangbian.Log.Logger.Debug(passenger.Name + "-生僻");
                                ExcuteMessage(passenger.Name + "存在生僻字,不符合注册规则");
                            }
                        }
                    }
                    #endregion

                    ExcuteMessage("填充需要分析的身份证完毕");

                    Thread.Sleep(2000);
                }
            }));
            th.Start();
        }

        private void 添加删除账号联系人任务ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddDeletePassenger s = new AddDeletePassenger();
            s.Show();
        }

        private void BulidTaskPage_Load(object sender, EventArgs e)
        {

        }
    }
    public class GetQueueServer
    {
        public int i = 0;

        public static string Get()
        {
            return Get("validPhone");
        }

        public static string Get(string queueName) {
            string url = "http://121.43.110.247/BaseServer/";

            string auth = "showmethetask";

            var passengerQueue = new QueueClient()
            {
                Url = url,
                Auth = auth,
                Name = queueName
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
    public class ConsoleMessageEventArgs : EventArgs
    {
        public string DataItem { get; set; }

        public string Message { get; set; }
    }

    public class TaskTickBulid {

        private bool isRun = false;

        public string Guid { get; set; }

        public bool IsRun
        {
            get { return isRun; }
            set
            {
                isRun = value;
                if (RunChange != null)
                {
                    RunChange(this, EventArgs.Empty);
                }
            }
        }
        public event EventHandler RunChange;

        public event EventHandler<ConsoleMessageEventArgs> OutputMessage;

       public void ExcuteMessage(string message)
       {
           if (OutputMessage != null) 
           {
               OutputMessage(this, new ConsoleMessageEventArgs { Message =  message });
           }
       }
        public TaskTickBulid()
        {
            Guid = System.Guid.NewGuid().ToString();
            IsRun = false;
        }

        public bool IsProvideServer()
        {
            if (DateTime.Now > DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 23:02:01"))||
                DateTime.Now < DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 06:58:01")))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Start() 
        {
            IsRun = true;

            Thread th = new Thread(new ThreadStart(() =>
            {
                while (IsRun)
                {
                    var data = DataTransaction.Create();

                    #region 开始迁移已用联系人
                    while (IsRun)
                    {
                        try
                        {
                            string sql = "select * from t_passenger where state=1 limit 0,100 ";// or state=3 

                            DataTable source = data.Query(sql).Tables[0];

                            if (source.Rows.Count <= 0)
                            {
                                break;
                            }

                            foreach (DataRow row in source.Rows)
                            {
                                List<string> sqls = new List<string>();

                                sqls.Add("delete from t_passenger where passengerId='" + row["passengerId"] + "'");

                                var state = row["state"] + "";

                                sqls.Add("insert into t_userpassenger(name,idNo,state,move) values('" + row["name"] + "','" + row["idNo"] + "','" + state + "'," + row["move"] + ")");
                              
                                data.ExecuteMultiSql(DataUpdateBehavior.Transactional, sqls.ToArray());

                                ExcuteMessage("迁移" + row["name"] + "联系人成功");
                            }
                        }
                        catch (Exception ex)
                        {
                            ExcuteMessage("迁移100个联系人失败" + ex.Message);
                        }
                    }
                    #endregion

                    #region 开始迁移生僻身份证 
                    while (IsRun)
                    {
                        var isRemoveSP = System.Configuration.ConfigurationManager.AppSettings["isRemoveSP"];
                        if (isRemoveSP == "1")
                        {
                            try
                            {
                                string sql = "select * from t_passenger where state=3 limit 0,100 ";// or state=3 

                                DataTable source = data.Query(sql).Tables[0];

                                if (source.Rows.Count <= 0)
                                {
                                    break;
                                }

                                foreach (DataRow row in source.Rows)
                                {
                                    List<string> sqls = new List<string>();

                                    sqls.Add("delete from t_passenger where passengerId='" + row["passengerId"] + "'");

                                    var state = row["state"] + "";
                                    sqls.Add("insert into t_userpassenger(name,idNo,state,move) values('" + row["name"] + "','" + row["idNo"] + "','" + state + "'," + row["move"] + ")");

                                    data.ExecuteMultiSql(DataUpdateBehavior.Transactional, sqls.ToArray());

                                    ExcuteMessage("迁移生僻" + row["name"] + "联系人成功");
                                }
                            }
                            catch (Exception ex)
                            {
                                ExcuteMessage("迁移生僻100个联系人失败" + ex.Message);
                            }
                        }
                        else {
                            break;
                        }
                    }
                    #endregion

                    if (IsProvideServer() == false)
                    {
                        IsRun = false;
                        ExcuteMessage("等待执行定时任务....");
                    }

                    #region 开始迁移已用邮件
                    while (IsRun)
                    {
                        try
                        {
                            ExcuteMessage("开始读取邮件");

                            string sql = "select * from t_email where state=1 limit 0,100 ";

                            List<string> sqls = new List<string>();

                            DataTable source = data.Query(sql).Tables[0];
                            if (source.Rows.Count <= 0)
                            {
                                break;
                            }
                            foreach (DataRow row in source.Rows)
                            {
                                sqls.Add("delete from t_email where emailId='" + row["emailId"] + "'");
                                sqls.Add("insert into t_useremail(email,passWord) values('" + row["Email"] + "','" + row["PassWord"] + "')");
                            }
                            ExcuteMessage("开始迁移邮件数据");

                            data.ExecuteMultiSql(DataUpdateBehavior.Transactional, sqls.ToArray());

                            ExcuteMessage("迁移邮件成功");
                        }
                        catch (Exception ex)
                        {
                            ExcuteMessage("迁移邮件失败" + ex.Message);
                        }
                    }
                    #endregion

                    #region 修改正在使用的联系人和邮件变为可用
                    try
                    {
                        if (IsRun)
                        {
                            ExcuteMessage("开始修改正在手机核验的账号变为可用");
                            data.ExecuteSql("update t_newaccount set state=2,LastTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where state=11");
                            ExcuteMessage("修改正在手机核验的账号变为可用成功");
                        } if (IsRun)
                        {
                            ExcuteMessage("开始修改锁定手机核验的账号变为可用");
                            data.ExecuteSql("update t_newaccount set state=2,LastTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where state=13");
                            ExcuteMessage("修改锁定手机核验的账号变为可用成功");
                        } if (IsRun)
                        {
                            ExcuteMessage("开始修改修改乘车人状态从锁定到可用");
                            data.ExecuteSql("update t_passenger set state=0  where state=2");
                            ExcuteMessage("修改修改乘车人状态从锁定到可用成功");
                        } if (IsRun)
                        {
                            ExcuteMessage("开始修改修改乘车人状态从优质锁定到优质可用");
                            data.ExecuteSql("update t_passenger set state=4  where state=5");
                            ExcuteMessage("修改修改乘车人状态从优质锁定到优质可用成功");
                        } if (IsRun)
                        {
                            ExcuteMessage("开始修改邮件从锁定到可用");
                            data.ExecuteSql("update t_email set state=0  where state=2");
                            ExcuteMessage("修改邮件从锁定到可用成功");
                        }
                        if (IsRun)
                        {
                            ExcuteMessage("开始修改账号从锁定添加联系人账号到添加联系人账号可用");
                            data.ExecuteSql("update t_newaccount set state=20,LastTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where state=21");
                            ExcuteMessage("修改账号从锁定添加联系人账号到添加联系人账号可用成功");
                        }
                    }
                    catch
                    {

                    }
                    #endregion

                    #region 移除队列数据

                    ExcuteMessage("开始清理手机核验队列");
                    while (IsRun)
                    {
                        var item = GetQueueServer.Get("validPhone");
                        if (string.IsNullOrEmpty(item))
                        {
                            return;
                        }
                    }
                    ExcuteMessage("清理手机核验队列成功");

                    ExcuteMessage("开始清理联系人队列");
                    while (IsRun)
                    {
                        var item = GetQueueServer.Get("Passenger");
                        if (string.IsNullOrEmpty(item))
                        {
                            return;
                        }
                    }
                    ExcuteMessage("清理联系人队列成功");


                    ExcuteMessage("开始清理优质联系人队列");
                    while (IsRun)
                    {
                        var item = GetQueueServer.Get("goodPassenger");
                        if (string.IsNullOrEmpty(item))
                        {
                            return;
                        }
                    }
                    ExcuteMessage("清理优质联系人队列成功");


                    ExcuteMessage("开始清理邮件队列");
                    while (IsRun)
                    {
                        var item = GetQueueServer.Get("Email");
                        if (string.IsNullOrEmpty(item))
                        {
                            return;
                        }
                    }
                    ExcuteMessage("清理邮件队列成功");

                    ExcuteMessage("开始清理注册联系人队列");
                    while (IsRun)
                    {
                        var item = GetQueueServer.Get("readAddValidAccount");
                        if (string.IsNullOrEmpty(item))
                        {
                            return;
                        }
                    }
                    ExcuteMessage("清理注册联系人队列");

                    #endregion
                }
                ExcuteMessage("执行结束");
                IsRun = false;
            }));

            th.Start();
        }

        public void Stop() {
            IsRun = false;
        }
    }
}
