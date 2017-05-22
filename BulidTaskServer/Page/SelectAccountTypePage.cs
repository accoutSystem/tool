using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BulidTaskServer
{
    public partial class SelectAccountTypePage : Form
    {
        public SelectAccountTypePage()
        {
            InitializeComponent();
            AccountType = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                AccountType = 0;
            }
            if (radioButton2.Checked)
            {
                AccountType = 1;
            }
            if (radioButton3.Checked)
            {
                AccountType = 3;
            }
            if (radioButton4.Checked)
            {
                AccountType = 4;
            }
            this.Close();
        }

        public int AccountType { get; set; }
    }
}
