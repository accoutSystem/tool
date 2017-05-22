using FangBian.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TestWebRequest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (rbGet.Checked)
            {
                var str = HttpHelper.Get(textBox1.Text);
                MessageBox.Show(str);
            }
            if (rbPost.Checked) {
                var str = HttpHelper.Post(textBox1.Text,"");
                MessageBox.Show(str);
            }
        }
    }
}
