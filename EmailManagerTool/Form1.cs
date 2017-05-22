using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmailManagerTool
{
    public partial class Form1 : Form
    {
        bool isRunEmail = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

            if (isRunEmail)
            {
                isRunEmail = false;
                this.btnRegEmail.Text = "开始注册邮箱";
            }
            else {
                isRunEmail = true;
                this.btnRegEmail.Text = "停止注册邮箱";
            }

            Thread th = new Thread(new ThreadStart(() =>
            {
                while (isRunEmail)
                {

                }
            }));
            th.Start();
        }
    }
}
