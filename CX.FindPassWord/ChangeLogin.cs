using ChangePassWord.Properties;
using MyTool.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ChangePassWord
{
    public partial class ChangeLogin : Form
    {
        public static ChangeLogin Current { get; set; }
        public ChangeLogin()
        {
            InitializeComponent();
            Current = this;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var db = DB.Get(); 

            var userName = tbUserName.Text;

            var pw = tbPassWord.Text;

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(pw))
            {
                MessageBox.Show("用户名密码为空");
                return;
            }

            var user = db.Query("select * from t_updateuser where state=1 and username='" + userName + "' and password='" + pw + "'").Tables[0];

            if (user.Rows.Count <= 0)
            {
                MessageBox.Show("用户名密码错误");
                return;
            }  
            LoginUserInfo.Current.Id = user.Rows[0]["idt_updateUser"] + string.Empty;
            LoginUserInfo.Current.Price = user.Rows[0]["price"] + string.Empty;
            LoginUserInfo.Current.InvterPrice = user.Rows[0]["invertPrice"] + string.Empty;
            LoginUserInfo.Current.Invter = user.Rows[0]["invter"] + string.Empty;
            LoginUserInfo.Current.UserName = userName;
            LoginUserInfo.Current.PassWord = pw;
            this.Hide();

            NewGetPassWord p = new NewGetPassWord();

            p.Show();
        }
    }

    public class LoginUserInfo {
        private static LoginUserInfo current;

        public static LoginUserInfo Current
        {
            get {
                if (LoginUserInfo.current == null)
                    LoginUserInfo.current = new LoginUserInfo();
                return LoginUserInfo.current; }
           
        }

        public string Id { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public string Price { get; set; }
        /// <summary>
        /// 给上线的提成价格
        /// </summary>
        public string InvterPrice { get; set; }

        /// <summary>
        /// 上线
        /// </summary>
        public string Invter { get; set; }

    }

    public class DB {
        public static PD.Business.DataTransaction Get()
        {
            return PD.Business.DataTransaction.Create(CXDataCipher.DecipheringUserPW(Resources.temp), "");
        }
    }
}
