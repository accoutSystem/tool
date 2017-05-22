using ChangePassWord.Entiry;
using ChangePassWord.PassCode;
using ChangePassWord.Properties;
using ChangePassWord.WebRequest;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ChangePassWord
{
    public partial class ChangePage : Form
    {
        public ChangePage()
        {
            InitializeComponent();
        }

        string str = string.Empty;

        ChangePassWordSession session = new Entiry.ChangePassWordSession { };
        private void button5_Click(object sender, EventArgs e)
        {
            session = new Entiry.ChangePassWordSession { };
            InitforgetMyPasswordWebRequest ss = new InitforgetMyPasswordWebRequest();
            ss.Request(session);
            GetPassCode();
        }
        private void GetPassCode()
        {
            GetPassCodeNewWebRequest passCode = new GetPassCodeNewWebRequest();

            var source = passCode.Request(session);

            str = source;

            SetImage(source);
        }
        private void SetImage(string base64)
        {

            //if (!string.IsNullOrEmpty(base64))
            //{
            //    byte[] arr = Convert.FromBase64String(base64);
            //    MemoryStream ms = new MemoryStream(arr);
            //    Bitmap bmp = new Bitmap(ms);
            //    pictureBox1.Image = bmp;
            //}
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    textBox2.Text = AnalysePassCode.GetCheckCode(str);

            //    CheckRandCodeAnsynWebRequest request = new CheckRandCodeAnsynWebRequest();

            //    request.PassCode = textBox2.Text;

            //    var source = request.Request(session);

            //    JObject result = JObject.Parse(source);

            //    if ((result["status"] + "").ToLower().Equals("true") && (result["data"]["result"] + "").Equals("1"))
            //    {
            //        MessageBox.Show("识别成功");
            //    }
            //    else
            //    {
            //        GetPassCode();
            //    }
            //}
            //catch (Exception ex)
            //{

            //}
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            //FindPasswordByEmailWebReqeust request = new FindPasswordByEmailWebReqeust()
            //{
            //    Email = "978124155@qq.com",
            //    IdNo = "430121198911207314",
            //    RandCode = textBox2.Text
            //};

            //var source = request.Request(session);
        }

        bool isStart = false;

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (isStart)
            {
                btnStart.Text = "启动任务";
                btnStart.Image = Resources.Start_16px_1178732_easyicon_net;
                isStart = false;
                return;
            }
            else
            {
                btnStart.Image = Resources.Stop_16px_1191714_easyicon_net;
                btnStart.Text = "停止任务";
                isStart = true;
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            SetPassWordPage page = new SetPassWordPage();
            page.ShowDialog();
        }
    }
}
