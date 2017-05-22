using Fangbian.Tickets.Trains.Activities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CX.Config
{
    public partial class SetConfig : Form
    {
        public SetConfig()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = BoxConnectionManager.ConnectionString;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BoxConnectionManager.ConnectionString = textBox1.Text;
            BoxConnectionManager.Reset();
            this.Close();
        }

    }
}
