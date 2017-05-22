using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ResourceBulidTool.Page
{
    public partial class ShowMessage : Form
    {
        public string Text { get; set; }
        public ShowMessage()
        {
            InitializeComponent();
        }

        private void ShowMessage_Load(object sender, EventArgs e)
        {
            textBox1.Text = Text;
        }
    }
}
