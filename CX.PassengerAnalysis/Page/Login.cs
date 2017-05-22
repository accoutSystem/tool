using AccountRegister;
using Fangbian.Tickets.Trains.Activities;
using FangBian.Common;
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
        }

        private void LoginClick(object sender, EventArgs e)
        {

            CurrentSystemType.Type = SystemRegisterType.手机通道;

            this.Hide();
            BoxConnectionManager.ConnectionString = this.textBox1.Text;
            RegisterMain validPage = new RegisterMain();
            validPage.IsCheckIdNo = false;
            validPage.TaskNumber = Convert.ToInt32(this.tbTaskNumber.Text);
            validPage.Show();

        }

        private void Login_Load(object sender, EventArgs e)
        {
            this.textBox1.Text = System.Configuration.ConfigurationManager.AppSettings["connection"];
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
