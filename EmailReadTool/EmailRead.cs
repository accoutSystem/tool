using CaptchaServerCacheClient;
using glib.Email;
using hMailServer;
using MyEntiry;
using MyTool.Common;
using PD.Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using VaildTool;
using VaildTool.Properties;

namespace EmailReadTool
{
    public partial class EmailRead : Form
    {
        private AutoResetEvent orderResetEvent = new AutoResetEvent(false);
        private AutoResetEvent otherResetEvent = new AutoResetEvent(false);

        bool isRunEmail = false;

        bool isReadEmail = false;

        int messageCount = 0;

        string userName = "";

        int addCount = 0;

        public int AddCount
        {
            get { return addCount; }
            set
            {
                addCount = value;
                this.lbAddCount.Text = value.ToString();
            }
        }

        int badCount = 0;

        public int BadCount
        {
            get { return badCount; }
            set { badCount = value; this.lbBadCount.Text = value.ToString(); }
        }

        public EmailRead()
        {
            InitializeComponent();
            Load += EmailRead_Load;
            webEmailPath.DocumentCompleted += ReadEmailValidPathCompleted;
            webEmailValid.DocumentCompleted -= ValidEmailCompleted;
        }
        bool isClose = true;

        void EmailRead_Load(object sender, EventArgs e)
        {
            //Query();
            //btnReadEmail_Click(this, EventArgs.Empty);
            //this.WindowState = FormWindowState.Minimized;
            Thread th = new Thread(new ThreadStart(() =>
            {
                while (isClose)
                {
                    Thread.Sleep(1000);
                    DeleteIEProcess();
                }
            }));
            th.Start();
        }

        #region 注册邮箱
        public   List<string> domains = new List<string>();
        int index = 0;
        private void StartRegisterEmailClick(object sender, EventArgs e)
        {
            if (isRunEmail)
            {
                btnRegEmail.Image = Resources.start;
                btnRegEmail.Text = "开始预登录";
                isRunEmail = false;
                return;
            }
            else
            {
                isRunEmail = true;
                btnRegEmail.Image = Resources.stop;
                btnRegEmail.Text = "停止预登录";

            }
            ExcuteMessage("初始化创建邮件");
            
            hMailServer.Application application = new hMailServer.ApplicationClass();

            application.Authenticate("Administrator", System.Configuration.ConfigurationManager.AppSettings["pw"]);

            if (domains.Count <= 0)
            {
                string dominConfin = System.Configuration.ConfigurationManager.AppSettings["domain"];
                domains = dominConfin.Split(',').ToList();
            }
             
          
            ExcuteMessage("初始化创建邮件完成");
           
            Thread th = new Thread(new ThreadStart(() =>
            {
                while (isRunEmail)
                {
                    try
                    {
                        if (index >= domains.Count)
                            index = 0;
                        string domain = domains[index];

                        index++;

                        string email = BulidEmail() + "@" + domain;

                        string password = BulidPassWord();

                        ExcuteMessage("开始创建邮件" + email);

                        Account account = application.Domains.get_ItemByName(domain).Accounts.Add();

                        account.Address = email;

                        account.Password = password;

                        account.PersonLastName = "李";

                        account.PersonFirstName = "超";

                        account.MaxSize = 1000;

                        account.DomainID = 1;

                        account.Active = true;

                        account.Save();

                        ExcuteMessage("创建邮件" + email + "成功");

                        InsertMail(email, password);

                        Thread.Sleep(1000);

                        AddCount++;
                        GC.Collect();
                    }
                    catch (Exception ex)
                    {
                        ExcuteMessage("注册邮箱失败:" + ex.Message);
                        Thread.Sleep(1000);
                        BadCount++;
                    }
                }
            }));
            th.IsBackground = true;
            th.Start();
        }

        private void InsertMail(string email, string pw)
        {
            ExcuteMessage(email + "写入业务数据数据库");
            string sql = string.Format("INSERT INTO cx_user1.t_email (emailId, Email, PassWord,createTime , state) VALUES ('{0}', '{1}', '{2}', '{3}', '0')",
                Guid.NewGuid().ToString(), email, pw, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            DataTransaction.Create().ExecuteSql(sql);
            ExcuteMessage(email + "写入业务数据数据库成功");
        }

        private string BulidEmail()
        {
            return ToolCommonMethod.GetCharRandom(3) + ToolCommonMethod.GetRandom(6);
        }

        private string BulidPassWord()
        {
            return ToolCommonMethod.GetCharRandom(3) + ToolCommonMethod.GetRandom(3);
        }
        #endregion

        #region 读取邮箱并且打开浏览器

        int read163Count = 0;

        int currentValidCount = 0;
        private void StopRun() {
            this.Invoke(new Action(() =>
            {
                btnReadEmail.Image = Resources.start;
                btnReadEmail.Text = "开始核验服务";
            }));

            isReadEmail = false;
        }


          [DllImport("KERNEL32.DLL", EntryPoint = "SetProcessWorkingSetSize", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool SetProcessWorkingSetSize(IntPtr pProcess, int dwMinimumWorkingSetSize, int dwMaximumWorkingSetSize);

        [DllImport("KERNEL32.DLL", EntryPoint = "GetCurrentProcess", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern IntPtr GetCurrentProcess();

           
        private void btnReadEmail_Click(object sender, EventArgs e)
        {
            if (isReadEmail)
            {
                StopRun();
                return;
            }
            else
            {
                isReadEmail = true;

                btnReadEmail.Image = Resources.stop;

                this.WindowState = FormWindowState.Normal;
                this.TopMost = false;

                btnReadEmail.Text = "停止核验服务";
            }

            var otherDomain = System.Configuration.ConfigurationManager.AppSettings["otherDomain"];

            var s = PD.Business.DataTransaction.Create();

            Thread th = new Thread(new ThreadStart(() =>
           {
               currentValidCount = 0;

               while (isReadEmail)
               {
                   try
                   {
                       SetExcute("开始出队列");

                       var validUser = ReadEmailQueue.GetEmail();

                       SetExcute("出队列完毕"  );

                       if (validUser == null)
                       {
                           Thread.Sleep(3000);
                           continue;
                       }

                       SetExcute(validUser.UserName + " " + validUser.PassWord);

                       try
                       {
                           this.Invoke(new Action(() =>
                               {
                                   toolStripTextBox2.Text = validUser.UserName + " " + validUser.PassWord;
                               }));

                           var index = validUser.Email.IndexOf("@");

                           var currentDomain = validUser.Email.Substring(index, validUser.Email.Length - index).Replace("@", "");

                           if (currentDomain.Contains("163.com"))
                           {
                               #region 163邮件核验
                           
                               
                               var emailS = s.Query("select * from t_email where email='" + validUser.Email + "'").Tables[0];
                               if (emailS.Rows.Count <= 0)
                               {
                                   emailS = s.Query("select * from t_useremail where email='" + validUser.Email + "'").Tables[0];
                               }
                               if (emailS.Rows.Count <= 0)
                               {
                                   continue;
                               }
                               //写入文件
                               var emailInfo = System.Environment.CurrentDirectory + @"/Inner/Email.txt";
                               File.WriteAllText(emailInfo, validUser.Email + "," + emailS.Rows[0]["password"]);
                               Invoke(new Action(() =>
                               {
                                   //启动进程
                                   var exePath = System.Configuration.ConfigurationManager.AppSettings["validExe"];
                                   System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(exePath);
                                   psi.RedirectStandardOutput = true;
                                   psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                                   psi.UseShellExecute = false;
                                   System.Diagnostics.Process listFiles;
                                   listFiles = System.Diagnostics.Process.Start(psi);
                                   System.IO.StreamReader myOutput = listFiles.StandardOutput;
                                   listFiles.WaitForExit();
                               }));

                               bool isValid = false;
                               bool isStop = false;
                               var emailResult = System.Environment.CurrentDirectory + @"/Inner/Result.txt";

                               if (File.Exists(emailResult))
                               {
                                   var result = File.ReadAllText(emailResult).Split(',');
                                   isValid = Convert.ToBoolean(result[0]);
                                   isStop = Convert.ToBoolean(result[1]);
                               }

                               if (isValid)
                               {
                                   Storage(validUser, string.Empty);
                                   AddCount++;
                                   SetExcute("核验163邮件成功");
                               }
                               else
                               {
                                   SetExcute("核验163邮件失败");
                                   BadCount++;
                                   ResestUser(validUser.UserGuid);
                               }
                               if (isStop)
                               {
                                   StopRun();
                               }
                               read163Count++;

                               var maxReadCount = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["reConnecton"]);

                               if (read163Count > maxReadCount)
                               {
                                   ADSLConnection.Connection();
                                   read163Count = 0;
                                   Thread.Sleep(1000);
                               }
                               continue;
                               #endregion
                           }

                           #region 其他邮件核验
                           string sql = string.Format(@"select a.messageid,a.messagefilename  from email.hm_messages a inner join email.hm_accounts b on 
                                                                     b.accountid=a.messageaccountid 
where b.accountaddress='{0}'
 and messageflags=96 and messagefrom like '12306%'
order by messagecreatetime desc  ", validUser.Email);
                           DataTable emailPath = null;
                           DataTransaction data = null;
                           bool isOtherDomain = otherDomain.Contains(currentDomain);
                           if (isOtherDomain)
                           {
                               data = DataTransaction.Create("otherDomainCon");
                               emailPath = data.DoGetDataTable(sql);
                           }
                           else
                           {
                               data = DataTransaction.Create();
                               emailPath = data.DoGetDataTable(sql);
                           }

                           SetExcute("读取邮件物理路径");

                           if (emailPath.Rows.Count > 0)
                           {
                               string path = emailPath.Rows[0]["messagefilename"] + string.Empty;

                               if (isOtherDomain)
                               {
                                   var temp = path.Substring(1, 2);

                                   path = "http://120.55.81.17:7548/" + currentDomain + @"/" + validUser.Email.Substring(0, index) + @"/" + temp + @"/" + path;
                               }
                               else
                               {
                                   path = "http://121.43.110.247/Email" + path.Replace(@"C:\Program Files (x86)\hMailServer\Data", "").Replace(@"\", "/");
                               }
                               SetExcute("读取核验地址路径成功");

                               this.Invoke(new Action(() =>
                               {
                                   webEmailPath.Url = new Uri(path);// webEmailValid.Url = new Uri(vailUrl);
                               }));

                               isEmailValid = false;

                               orderResetEvent.WaitOne();

                               if (isEmailValid)
                               {
                                   Storage(validUser, emailPath.Rows[0]["messageid"] + string.Empty);
                               }
                               else
                               {
                                   ResestUser(validUser.UserGuid);
                               }

                               currentValidCount++;

                               //webEmailValid.Url = new Uri("http://localhost/iisstart.htm");

                               var count = System.Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["reConnecton"]);

                               var isADSL = System.Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["isADSL"]);

                               if (isADSL && currentValidCount >= count)
                               {
                                   SetExcute("开始重新拨号");
                                   //重新拨号
                                   ADSLConnection.Connection();

                                   SetExcute("重新拨号完成");
                                   currentValidCount = 0;
                               }
                           }
                           else
                           {
                               SetExcute("12306没有发送核验邮件");
                               ResestUser(validUser.UserGuid);
                           }
                           #endregion
                       }
                       catch
                       {
                           ResestUser(validUser.UserGuid);
                       }
                   }
                   catch (Exception ex)
                   {
                       ExcuteMessage("注册读取邮箱:" + ex.Message);
                       BadCount++;
                   }
               } 
           }));
            th.IsBackground = true;
            th.Start();
        }
        private void DeleteIEProcess()
        {
            ///关闭IE进程
            var ss = System.Diagnostics.Process.GetProcessesByName("iexplore");

            foreach (System.Diagnostics.Process process in ss)
            {
                Invoke(new Action(() =>
                {
                    try
                    {
                        process.Kill();
                    }
                    catch { 
                    
                    }
                }));
            }
        }
       

        #region laji 
        ///// <summary>
        ///// 核验状态
        ///// </summary>
        //public class ValidDataState 
        //{
        //    /// <summary>
        //    /// 当前状态 Free 自由 Wait
        //    /// </summary>
        //    public ValidState CurrentState { get; set; }

        //    public int Count { get; set; }
        //}

        //public enum ValidState
        //{
        //    Free = 0,
        //    Bad = 1,
        //    Wait = 2,
        //}

        //public void SetCount(ValidDataState state) 
        //{
        //    var souce = Newtonsoft.Json.JsonConvert.SerializeObject(state);

        //    try
        //    {
        //        File.WriteAllText(System.Environment.CurrentDirectory + @"\count.txt", souce);
        //    }
        //    catch
        //    { 
            
        //    }
        //}

        //public ValidDataState GetCount()
        //{
        //    try
        //    {
        //        var source = File.ReadAllText(System.Environment.CurrentDirectory + @"\count.txt");
        //        if (string.IsNullOrEmpty(source))
        //        {
        //            ValidDataState nullState = new ValidDataState { Count = 0, CurrentState = ValidState.Free };
        //            SetCount(nullState);
        //            return nullState;
        //        }
        //        return Newtonsoft.Json.JsonConvert.DeserializeObject<ValidDataState>(source);
        //    }
        //    catch
        //    {
        //        return new ValidDataState { CurrentState = ValidState.Bad, Count = 0 };
        //    }
        //}
        #endregion

        /// <summary>
        /// 复位账号状态(到邮件核验锁定状态)
        /// </summary>
        /// <param name="userGuid"></param>
        private void ResestUser(string userGuid) 
        { 
            string sql = "update t_newaccount set State=16 ,LastTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where UserGuid='" + userGuid + "'";
            DataTransaction.Create().ExecuteSql(sql);
        }
        
        /// <summary>
        /// 存储数据
        /// </summary>
        /// <param name="userGuid"></param>
        private void Storage(T_ValidUser user , string messageId)
        {
            var data = DataTransaction.Create();

            List<string> sqls = new List<string>();
            if (user.State.Equals("0"))
            {
                sqls.Add("update t_newaccount set  State=1 ,LastTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where UserGuid='" + user.UserGuid + "'");
            }
            if (user.State.Equals("19"))
            {
                sqls.Add("update t_newaccount set  State=10,isActive=1,LastTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where UserGuid='" + user.UserGuid + "'");
            }
            sqls.Add("update t_email set state=1,lastTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where Email='" + user.Email + "'");

            sqls.Add("update email.hm_messages set messageflags=97 where messageid='" + messageId + "'");

            data.ExecuteMultiSql(DataUpdateBehavior.Transactional, sqls.ToArray());
            data.CloseDb();
        }
        string emailContent = string.Empty;
      
        void ReadEmailValidPathCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            isEmailValid = false;
              emailContent = webEmailPath.DocumentText;

            if (!string.IsNullOrEmpty(emailContent))
            {
                int index = emailContent.IndexOf("<a href=https://kyfw.12306.cn/otn");

                emailContent = emailContent.Substring(index, emailContent.Length - index);

                emailContent = emailContent.Substring(0, emailContent.IndexOf(">https")).Replace("<a href=", "");
              
                SetExcute("读取核验地址路径完成");
                this.Invoke(new Action(() =>
                {
                    tabControl1.TabPages[1].Text = string.Format("核验消息(数量{0})",  currentValidCount);
                    webEmailValid.DocumentCompleted += ValidEmailCompleted;
                    toolStripTextBox1.Text = emailContent;
                    SetExcute("开始打开核验地址..."+emailContent);
                    webEmailValid.Navigate(new Uri(emailContent));
                    //webEmailValid.Refresh();
                }));
            }
            else 
            {
                MessageBox.Show("读取核验路径为空,终止运行");
            }
        }

        bool isEmailValid = false;
        private void ValidEmailCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            isEmailValid = false;

            if (!string.IsNullOrEmpty(webEmailValid.DocumentText))
            {
                if (webEmailValid.DocumentText.Contains("邮箱已成功验证，请您重新登录"))
                {
                    SetExcute("--------------------------12306账号邮箱验证已成功");
                    //通过邮箱验证
                    isEmailValid = true;
                }
                else 
                {
                    this.TopMost = true;
                    this.WindowState = FormWindowState.Maximized;
                    //if (false)
                    //{
                    //    MessageBox.Show("核验失败，阻止运行" + webEmailValid.DocumentText);
                    //}
                    ExcuteMessage("核验失败，阻止运行" + webEmailValid.DocumentText);
                    StopRun();
                }

                webEmailValid.DocumentCompleted -= ValidEmailCompleted;
                //webEmailValid.DocumentText = "核验完成,正在等待下一个...";
                //webEmailValid.Refresh(  WebBrowserRefreshOption.) 
                orderResetEvent.Set();
            }
            else
            {
                MessageBox.Show("核验页面为空,终止运行");
                SetExcute("核验失败,终止运行");
                StopRun();
            }
        }

        #endregion

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

        private void toolStripStatusLabel3_Click(object sender, EventArgs e)
        {
            Query();
        }

        private void Query() 
        {
            Thread th = new Thread(new ThreadStart(() => 
            {
                try
                {
                    string sql = string.Format(@"select sum(case state when 0 then 1 else 0 end) noUser,
sum(case state when 1 then 1 else 0 end) user,
sum(case state when 2 then 1 else 0 end) usering from t_email ");

                    var source = DataTransaction.Create().DoGetDataTable(sql);

                    var count = Convert.ToInt32(DataTransaction.Create().DoGetDataTable("select count(1) from t_useremail").Rows[0][0]);

                    this.Invoke(new Action(() =>
                    {
                        lbDetail.Text = string.Format("详情:未使用{0} 已使用{1} 正在使用{2}", source.Rows[0]["noUser"], Convert.ToInt32(source.Rows[0]["user"]) + count, source.Rows[0]["usering"]);
                    }));
                }
                catch { }
            }));
            th.Start();
        }

        public void SetExcute(string message) 
        {
            this.Invoke(new Action(() =>
            {
                lbExcute.Text = DateTime.Now.ToString("HH:mm:ss") + ":" + userName + "->" + message;
                ExcuteMessage(message);
                Console.WriteLine(lbExcute.Text);
            }));
        }

        bool isMove = false;
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (isMove)
            {
                btnConvertPassenger.Text = "开始迁移数据";
                btnConvertPassenger.Image = Resources.start;
                isMove = false;
                return;
            }
            else
            {
                btnConvertPassenger.Text = "停止迁移数据";
                btnConvertPassenger.Image = Resources.stop;
                isMove = true;
            }

            Thread th = new Thread(new ThreadStart(() =>
            {
                var data = DataTransaction.Create();

                while (isMove)
                {
                    try
                    {
                        ExcuteMessage("开始读取邮件");

                        string sql = "select * from t_email where state=1 limit 0,1000 ";

                        List<string> sqls = new List<string>();

                        DataTable source = data.Query(sql).Tables[0];
                        if (source.Rows.Count <= 0)
                        {
                            Invoke(new Action(()=>
                            {
                                btnConvertPassenger.Text = "开始迁移数据";
                                btnConvertPassenger.Image = Resources.start;
                                isMove = false;
                            }));
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
            }));

            th.Start();
        }

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            ADSLConnection.Connection();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Add163Page page = new Add163Page();
            page.ShowDialog();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
             
        }

        private void EmailRead_Load_1(object sender, EventArgs e)
        {

        }

        private void EmailRead_FormClosing(object sender, FormClosingEventArgs e)
        {
            isClose = false;
        }
    }
}
