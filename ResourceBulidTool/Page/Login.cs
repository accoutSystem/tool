using AccountRegister;
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
            this.tbUserName.Text = "lichao73141";
            this.tbPassWord.Text = "li7314CHAO1989";
            this.textBox1.Text = System.Configuration.ConfigurationManager.AppSettings["connection"];
            comboBox1.Items.Add("畅行短信平台");//lichao7314 li7314CHAO
            comboBox1.Items.Add("呀呀呀");//lichao7314 li7314CHAO

            comboBox1.Items.Add("飞码");//lichao7314 li7314CHAO
            comboBox1.Items.Add("一片云新");//lichao7314 li7314CHAO
            comboBox1.Items.Add("一片云");//lichao7314 li7314CHAO
            comboBox1.Items.Add("壹码");//lichao7314 li7314CHAO
            comboBox1.Items.Add("爱码");//lichao7314 li7314CHAO
            //comboBox1.Items.Add("卓码");//lichao7314 li7314CHAO
            //comboBox1.Items.Add("云码");//lichao7314 li7314CHAO
            //comboBox1.Items.Add("快码");//lichao73141 li7314CHAO1989
            //comboBox1.Items.Add("飞Q");//lichao7314 li7314CHAO

            //comboBox1.Items.Add("淘码");//淘码
            // comboBox1.Items.Add("云码");
            // comboBox1.Items.Add("飞Q");
            comboBox1.SelectedIndex = 0;
        }

        private void LoginClick(object sender, EventArgs e)
        {
            int type = 4;

            switch (comboBox1.SelectedItem + string.Empty)
            {
                case "一片云": type = 1; break;
                case "一片云新": type = 10; break;
                case "爱码": type = 2; break;
                case "壹码": type = 3; break;
                case "畅行短信平台": type = 4; break;
                case "卓码": type = 7; break;
                case "快码": type = 6; break;
                case "飞Q": type = 5; break;
                case "飞码": type = 5; break;
                case "呀呀呀": type = 11; break;
            }

            switch (comboBox2.SelectedItem + string.Empty) {
                case "手机通道":
                    CurrentSystemType.Type = SystemRegisterType.手机通道;
                    break;
                case "PC通道":
                    CurrentSystemType.Type = SystemRegisterType.PC通道;
                    break;
            }

            if (CurrentSystemType.Type == SystemRegisterType.手机通道)
            {
                MessageBox.Show("手机通道已封闭，无法使用");
                return;
            }

            PassCodeProviderFactory.PlatformType = type;

            IPassCodeProvider p = PassCodeProviderFactory.GetPlatform();

            var source = p.Login(tbUserName.Text, tbPassWord.Text);

            if (p.LoginSuccess(source))
            {
                this.Hide();
                BoxConnectionManager.ConnectionString = this.textBox1.Text;
                RegisterMain validPage = new RegisterMain();
                validPage.IsCheckIdNo = checkBox1.Checked;
                validPage.TaskNumber = Convert.ToInt32(this.tbTaskNumber.Text);
                validPage.Show();
            }
            else
            {
                MessageBox.Show("登陆失败");
            }
        }

        private void Login_Load(object sender, EventArgs e)
        {
            comboBox2.SelectedIndex = 0;
        }
    }

    public class CurrentSystemType
    {

        public static SystemRegisterType Type { get; set; }
    }

    public enum SystemRegisterType
    {
        手机通道 = 0,
        PC通道 = 1
    }
}
