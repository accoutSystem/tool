using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MyTool
{
    public partial class LockPage : Form
    {
        ToolMain main = null;

        public static LockPage Current
        {
            get;
            set;
        }

        public LockPage()
        {
            InitializeComponent();
            Current = this;
            Load += LockPage_Load;
        }

        void LockPage_Load(object sender, EventArgs e)
        {
            button1_Click(this, EventArgs.Empty);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "123li")
            {
                if (main == null)
                {
                    main = new ToolMain();
                    main.Load += main_Load;
                    main.Show();
                }
                else
                {
                    main.Visible = true;
                }
            }
            else
            {
                MessageBox.Show("授权失败!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void main_Load(object sender, EventArgs e)
        {
            this.Hide();
            this.Visible = false;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Convert.ToInt32(e.KeyChar) == 13)
            {
                button1_Click(null, null);
            }
        }
    }
}
