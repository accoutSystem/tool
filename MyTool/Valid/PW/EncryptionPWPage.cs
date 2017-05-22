using MyTool.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MyTool.Valid.PW
{
    public partial class EncryptionPWPage : Form
    {
        public EncryptionPWPage()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = CXDataCipher.EncryptionUserPW(textBox2.Text);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
