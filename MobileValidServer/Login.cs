using Fangbian.Tickets.Trains.Activities;
using FangBian.Common;
using MobileValidServer.PassCodeProvider;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobileValidServer
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
            this.tbUserName.Text = "lichao7314";
            this.tbPassWord.Text = "li7314CHAO";
            this.textBox1.Text = System.Configuration.ConfigurationManager.AppSettings["connection"];
            comboBox1.Items.Add("壹码");//lichao7314 li7314CHAO
            comboBox1.Items.Add("爱码");//lichao7314 li7314CHAO
            comboBox1.Items.Add("一片云");//lichao7314 li7314CHAO
            comboBox1.Items.Add("卓码");//lichao7314 li7314CHAO
            comboBox1.Items.Add("云码");//lichao7314 li7314CHAO
            comboBox1.Items.Add("快码");//lichao73141 li7314CHAO1989
            comboBox1.Items.Add("飞Q");//lichao7314 li7314CHAO
            comboBox1.Items.Add("飞码");//lichao7314 li7314CHAO
            //comboBox1.Items.Add("淘码");//淘码
            // comboBox1.Items.Add("云码");
            // comboBox1.Items.Add("飞Q");
            comboBox1.SelectedIndex = 0;
        }

        private void LoginClick(object sender, EventArgs e)
        {
            int type = 1;

            switch (comboBox1.SelectedItem + string.Empty)
            {
                case "一片云": type = 1; break;
                case "爱码": type = 2; break;
                case "壹码": type = 3; break;
                case "云码": type = 4; break;
                case "卓码": type = 5; break;
                case "快码": type = 6; break;
                case "飞Q": type = 7; break;
                case "飞码": type = 8; break;
            }

            PassCodeProviderFactory.PlatformType = type;

            IPassCodeProvider p = PassCodeProviderFactory.GetPlatform();

            var source = p.Login(tbUserName.Text, tbPassWord.Text);

            if (p.LoginSuccess(source))
            {
                //var sdfsdf = p.GetMobilePhone();
                this.Hide();
                BoxConnectionManager.ConnectionString = this.textBox1.Text;
                MobileValidPage validPage = new MobileValidPage();
                validPage.TaskNumber = Convert.ToInt32(tbTaskNumber.Text);
                validPage.CurrentPlatfoem = comboBox1.SelectedItem + string.Empty;
                validPage.Show();
            }
            else
            {
                MessageBox.Show("登陆失败");
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }
    }
}
