using Fangbian.Ticket.Server.AdvanceLogin;
using MyTool.Common;
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

namespace ChangePassWord
{
    public partial class NewGetPassWord : Form
    {
        private UserInfo CurrentUser { get; set; }

        public static NewGetPassWord Current { get; set; }

        private bool passengerValid = false;

        public NewGetPassWord()
        {
            InitializeComponent();

            Current = this;

            this.Text = "数据修改辅助工具:当前登录人" + LoginUserInfo.Current.UserName;

            webBrowser1.DocumentCompleted += webBrowser1_DocumentCompleted;

            webBrowser2.DocumentCompleted += webBrowser2_DocumentCompleted;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (CurrentUser == null)
            {
                MessageBox.Show("请先获取需要修改的数据", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            webBrowser1.Url = new Uri("https://kyfw.12306.cn/otn/forgetPassword/initforgetMyPassword");
            tabControl1.SelectedIndex = 0;
            SetButton(sender as Button);
        }

        Button hisBtn = null;

        private void SetButton(Button btb)
        {
            if (hisBtn != null)
            {
                hisBtn.BackColor =  System.Drawing.SystemColors.Control;
            }
            if (btb != null)
            {
                btb.BackColor = System.Drawing.Color.Red;
            }
            hisBtn = btb;
        }

        void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var email = webBrowser1.Document.GetElementById("userEmail");

            var code = webBrowser1.Document.GetElementById("e_cardCode");

            if (email != null && code != null)
            {
                email.SetAttribute("value", CurrentUser.Email);
                code.SetAttribute("value", CurrentUser.PassengerId);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (CurrentUser == null)
            {
                MessageBox.Show("请先获取需要修改的数据", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SetButton(sender as Button);

            tabControl1.SelectedIndex = 1;

            webBrowser2.Url = new Uri("http://mail.163.com/");
          
        }

        void webBrowser2_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //textBox1.Text += e.Url.ToString() + "\r\n";
        }

        bool isRunChange = false;
        private void button8_Click(object sender, EventArgs e)
        {
            webBrowser3.Url = new Uri(textBox2.Text);

            Thread th = new Thread(new ThreadStart(() =>
            {
                bool run = true;

                while (run)
                {
                    Thread.Sleep(1500);

                    this.Invoke(new Action(() =>
                    {
                        if (webBrowser3.Document != null)
                        {
                            var newp = webBrowser3.Document.GetElementById("reset_password_new");
                            var newp1 = webBrowser3.Document.GetElementById("reset_confirmPassWord");

                            if (newp != null && newp1 != null)
                            {
                                newp.SetAttribute("value", CurrentUser.NewPassWord);
                                newp1.SetAttribute("value", CurrentUser.NewPassWord);
                                isRunChange = true;
                                run = false;
                            }
                        }
                    }));

                }
            }));
            th.Start();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (CurrentUser == null)
            {
                MessageBox.Show("请先获取需要修改的数据", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (passengerValid == false)
            {
                MessageBox.Show("请先执行验证,如果恶意发送指令修改将停用账号", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (isRunChange == false)
            {
                MessageBox.Show("当前任务没有成功,如果恶意发送指令修改将停用账号", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            

            if (MessageBox.Show("确认当前数据已经处理成功,执行正确提交", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Cancel)
                return;

         
            var userName = CurrentUser.UserName;

            Thread th = new Thread(new ThreadStart(() =>
            {
                var data = DB.Get();

                var source = data.Query("select * from t_hisnewaccount where state=27 and username='" + userName + "'").Tables[0];

                if (source.Rows.Count > 0)
                {
                    List<string> sqls = new List<string>();

                    #region 创建新资源
                    var row = source.Rows[0];
                    string userGuid = row["UserGuid"] + string.Empty;
                    string PassWord = CXDataCipher.EncryptionUserPW(CurrentUser.NewPassWord);
                    string PassengerName = row["PassengerName"] + string.Empty;
                    string PassengerId = row["PassengerId"] + string.Empty;
                    string Email = row["Email"] + string.Empty;
                    string Phone = row["Phone"] + string.Empty;
                    string State = "10";// row["State"] + string.Empty;
                    string CreateTime = row["CreateTime"] + string.Empty;
                    string LastTime = row["LastTime"] + string.Empty;
                    string PwdQuestion = row["PwdQuestion"] + string.Empty;
                    string PwdAnswer = row["PwdAnswer"] + string.Empty;
                    string IVR_passwd = row["IVR_passwd"] + string.Empty;
                    string businessId = row["businessId"] + string.Empty;
                    string buyTime = row["buyTime"] + string.Empty;
                    buyTime = string.IsNullOrEmpty(buyTime) ? "null" : "'" + buyTime + "'";
                    string isActive = row["isActive"] + string.Empty;
                    string accountType = row["accountType"] + string.Empty;
                    string readPassengerState = row["readPassengerState"] + string.Empty;

                    string addHis = @"insert into t_newaccount(UserGuid,UserName,PassWord,PassengerName,PassengerId,Email,Phone,State,CreateTime,LastTime,PwdQuestion,PwdAnswer,IVR_passwd,businessId,buyTime,isActive,accountType,readPassengerState)
values('" + userGuid + "','" + userName + "','" + PassWord + "','" + PassengerName + "','" + PassengerId + "','" + Email + "','" + Phone + "','" + State + "','" + CreateTime + "','" + LastTime + "','" + PwdQuestion + "','" + PwdAnswer + "','" + IVR_passwd + "','" + businessId + "'," + buyTime + ",'" + isActive + "','" + accountType + "','" + readPassengerState + "')";
                    string deleteAccount = "delete from t_hisnewaccount where UserGuid='" + userGuid + "'";

                    sqls.Add(addHis);

                    sqls.Add(deleteAccount);
                    #endregion

                    #region 计费

                    var cost = data.Query("select * from t_updatecost where updateUserId=" + LoginUserInfo.Current.Id).Tables[0];

                    if (cost.Rows.Count > 0)
                    {
                        sqls.Add(string.Format(@" update t_updatecost set allPrice=allPrice+{1} where idt_updateCost={0} ", cost.Rows[0]["idt_updateCost"] + string.Empty, LoginUserInfo.Current.Price));
                    }
                    else
                    {
                        sqls.Add(string.Format(@" insert into t_updatecost(updateUserId,allPrice,allInvterPrice) value({0},{1},0)", LoginUserInfo.Current.Id, LoginUserInfo.Current.Price));
                    }

                    #endregion

                    #region 给上线计费
                    if (!string.IsNullOrEmpty(LoginUserInfo.Current.Invter))
                    {
                        var invterCost = data.Query("select * from t_updatecost where updateUserId=" + LoginUserInfo.Current.Invter).Tables[0];

                        if (invterCost.Rows.Count > 0)
                        {
                            sqls.Add(string.Format(@" update t_updatecost set allInvterPrice=allInvterPrice+{1} where idt_updateCost={0} ", invterCost.Rows[0]["idt_updateCost"] + string.Empty, LoginUserInfo.Current.InvterPrice));
                        }
                        else
                        {
                            sqls.Add(string.Format(@" insert into t_updatecost(updateUserId,allPrice,allInvterPrice) value({0},0,{1})", LoginUserInfo.Current.Invter, LoginUserInfo.Current.InvterPrice));
                        }
                    }
                    #endregion

                    data.ExecuteMultiSql(sqls.ToArray());

                    CurrentUser = null;

                    GetPayInfo(null, null);

                    MessageBox.Show("处理当前数据成功,已计费", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    ClearInput();

                    isRunChange = false;
                }

            }));

            th.Start();

            SetButton(null);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (CurrentUser != null) {
                MessageBox.Show("请先处理完当前的数据在获取新数据!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var db = DB.Get();

            var sourceData = db.Query(@"select a.*,b.passWord EmailPW
from
    t_hisnewaccount a
        left join
    t_useremail b ON a.Email = b.email
where
    state = 10 limit 0,1").Tables[0];

            if (sourceData.Rows.Count > 0)
            {
                UserInfo info = new UserInfo()
                {
                    Email = sourceData.Rows[0]["Email"] + string.Empty,
                    EmailPW = sourceData.Rows[0]["EmailPW"] + string.Empty,
                    PassengerId = sourceData.Rows[0]["PassengerId"] + string.Empty,
                    PassengerName = sourceData.Rows[0]["PassengerName"] + string.Empty,
                    PassWord = sourceData.Rows[0]["PassWord"] + string.Empty,
                    UserName = sourceData.Rows[0]["UserName"] + string.Empty,
                    GUID = sourceData.Rows[0]["UserGuid"] + string.Empty
                };
                var item = ToolCommonMethod.GetChineseSpellCode(info.PassengerName);

                Random es = new Random();

                info.NewPassWord = item;

                for (int i = 0; i < 6; i++)
                {
                    info.NewPassWord += es.Next(10);
                }

                lbPW.Text = "（" + CXDataCipher.DecipheringUserPW(info.PassWord) + "）";

                CurrentUser = info;

                tbUserName.Text = info.UserName;
                tbOldPW.Text = info.PassWord;
                tbNewPW.Text = info.NewPassWord;
                tbIdCard.Text = info.PassengerId;
                tbName.Text = info.PassengerName;
                tbEmail.Text = info.Email.Replace("@163.com", "");
                tbEmailPW.Text = info.EmailPW;

                db.ExecuteSql(@"update t_hisnewaccount set State=27 where UserName='" + info.UserName + "'");
            }
            else
            {
                MessageBox.Show("系统数据已经全部处理完成，请通知管理员添加数据");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (CurrentUser == null)
            {
                MessageBox.Show("请先获取需要修改的数据", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            tabControl1.SelectedIndex = 2;
            textBox2.Text = string.Empty;
            SetButton(sender as Button);
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (CurrentUser == null)
            {
                MessageBox.Show("请先获取需要修改的数据", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            tabControl1.SelectedIndex = 1;

            SetButton(sender as Button);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var data = DB.Get();

            if (CurrentUser == null)
            {
                MessageBox.Show("请先获取需要修改的数据", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (MessageBox.Show("确定修改为错误,如果恶意发送该指令将锁定你的账号", "错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == System.Windows.Forms.DialogResult.OK)
            {

                data.ExecuteSql(@"  update t_hisnewaccount set state=26 where username='" + CurrentUser.UserName + "'");
                CurrentUser = null;
                ClearInput();
            }
        }

        private void ClearInput()
        {
            Invoke(new Action(() =>
            {
                lbPW.Text = "()";
                tbNewPW.Text = tbEmail.Text = tbUserName.Text = tbOldPW.Text = tbIdCard.Text = tbName.Text = tbEmailPW.Text = string.Empty;
                webBrowser1.Url = new Uri("about:blank");
                webBrowser2.Url = new Uri("about:blank");
                webBrowser3.Url = new Uri("about:blank");
            }));
        }

        private void NewGetPassWord_FormClosed(object sender, FormClosedEventArgs e)
        {
            ChangeLogin.Current.Close();
        }

        private void GetPayInfo(object sender, EventArgs e)
        {
            var data = DB.Get();

            var user = data.Query("select allPrice,allInvterPrice from t_updatecost where updateUserId=" + LoginUserInfo.Current.Id).Tables[0];

            if (user.Rows.Count> 0)
            {
                lbNoPay.Text = user.Rows[0]["allPrice"] + "元";
                lbInvterNoPay.Text = user.Rows[0]["allInvterPrice"] + "元";
            }  
        }

        private void NewGetPassWord_Load(object sender, EventArgs e)
        {
            GetPayInfo(null, null);
        }

        private void toolStripStatusLabel5_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定退出?", "错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == System.Windows.Forms.DialogResult.OK)
            {
                this.FormClosed -= NewGetPassWord_FormClosed;
                this.Close();
                ChangeLogin.Current.Show();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var data = DB.Get();

            if (CurrentUser == null)
            {
                MessageBox.Show("数据为空", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (MessageBox.Show("确定释放该条数据?", "错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == System.Windows.Forms.DialogResult.OK)
            {

                data.ExecuteSql(@"  update t_hisnewaccount set state=10 where username='" + CurrentUser.UserName + "'");
                CurrentUser = null;
                ClearInput();
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            var data = DB.Get();

            if (CurrentUser == null)
            {
                MessageBox.Show("请先获取需要修改的数据", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (MessageBox.Show("确定修改", "错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == System.Windows.Forms.DialogResult.OK)
            {

                data.ExecuteSql(@"  update t_hisnewaccount set state=28 where username='" + CurrentUser.UserName + "'");
                CurrentUser = null;
                ClearInput();
            }
        }

        private void toolStripStatusLabel5_Click_1(object sender, EventArgs e)
        {
            CreateUser u = new CreateUser();
            u.ShowDialog();
        }

        private void button11_Click(object sender, EventArgs e)
        { 
            if (CurrentUser == null)
            {
                MessageBox.Show("请先获取需要修改的数据", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            button11.Enabled = false;

            button11.Text = "正在验证结果...";

            GetPassengerActivation_PC pc = new GetPassengerActivation_PC();
            pc.Read(new Fangbian.Tickets.Trains.WFDataItem.Account12306Item
            {
                UserName = CurrentUser.UserName,
                PassWord =CXDataCipher.DecipheringUserPW( CurrentUser.PassWord)
            });
            pc.ReadPassengerCompleted += pc_ReadPassengerCompleted;
        }

        void pc_ReadPassengerCompleted(object sender, GetPassengerEventArgs e)
        {
            if (e.IsBadUser || e.IsFormatError || e.IsSystemError)
            {
                MessageBox.Show("验证失败，请转处理失败");
                return;
            }

            MessageBox.Show(e.CurrentAccount.CurrentUserPassengers.Count + "个联系人");

            if (e.CurrentAccount.CurrentUserPassengers.Count == 1)
            {
                passengerValid = true;
            }
            else
            {

            }
            Invoke(new Action(() =>
            {
                button11.Enabled = true;
                button11.Text = "验证并反馈结果";
            }));
        }
    }

    public class UserInfo
    {
        public string GUID { get; set; }
        public string UserName { get; set; }

        public string PassWord { get; set; }
        public string NewPassWord { get; set; }

        public string PassengerName { get; set; }

        public string PassengerId { get; set; }

        public string Email { get; set; }

        public string EmailPW { get; set; }

        public int Index { get; set; }

        public override string ToString()
        {
            return PassengerName + " " + PassengerId + " " + Email + " " + EmailPW;
        }
    }
}
