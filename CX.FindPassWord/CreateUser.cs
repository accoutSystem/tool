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
    public partial class CreateUser : Form
    {
        public CreateUser()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string userName = tbUserName.Text;
            string pw = tbPW.Text;
            string cpw = tbConfirmPW.Text;
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(pw))
            {
                MessageBox.Show("用户名和密码不能为空");
                return;
            }
            if (pw != cpw)
            {
                MessageBox.Show("密码不一致");
                return;
            }

            var db = DB.Get();

            string sql = @"insert into  t_updateuser(username,password,state,invter,createtime,price,invertPrice)
value('" + userName + "','" + pw + "',1,'" + LoginUserInfo .Current.Id+ "','"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+"','0.3','0.03')";

            db.ExecuteSql(sql);

            MessageBox.Show("创建用户"+userName+" "+pw+"成功");
        }
    }
}
