using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MyEntiry;

namespace MyTool
{
    public partial class SelectBusinessPage : Form
    {
        public T_Business CurrentBusiness
        {
            get;
            set;
        }
        public SelectBusinessPage()
        {
            InitializeComponent();
            Load += new EventHandler(SelectBusinessPage_Load);
        }

        void SelectBusinessPage_Load(object sender, EventArgs e)
        {
            listBox1.DisplayMember = "Name";
            listBox1.ValueMember = "Id";
            listBox1.DataSource = ToolMain.BusinessCollection;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var currentBusiness = listBox1.SelectedItem as T_Business;
            if (currentBusiness == null)
            {
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            }
            else{
                CurrentBusiness = currentBusiness;
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
        }
    }
}
