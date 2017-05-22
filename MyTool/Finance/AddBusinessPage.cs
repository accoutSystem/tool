using PD.Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MyTool.Finance
{
    public partial class AddBusinessPage : Form
    {
        public AddBusinessPage()
        {
            InitializeComponent();
            IsEdit = false;
            this.Load += AddBusinessPage_Load;
        }

        public bool Save { get; set; }

        void AddBusinessPage_Load(object sender, EventArgs e)
        {
            Save = false;
            tbBusinessName.Text = BusinessName;
            textBox2.Text = Remark;
        }

        public bool IsEdit { get; set; }

        public string BusinessId { get; set; }

        public string BusinessName { get; set; }

        public string Remark { get; set; }

        private void button1_Click(object sender, EventArgs e)
        {
            string businessName = tbBusinessName.Text;

            string remark = textBox2.Text;

            if (string.IsNullOrEmpty(businessName))
            {
                MessageBox.Show("商户不能为空");
                return;
            }
            if (IsEdit == false)
            {
                DataTransaction.Create().ExecuteSql(string.Format("insert into t_business(businessName,address) values('{0}','{1}')", businessName, remark));
            }
            else 
            {
                DataTransaction.Create().ExecuteSql(string.Format("update t_business set  businessName='{0}',address='{1}' where businessId='{2}'", businessName, remark, BusinessId));
            }
            Save = true;
            this.Close();
        }
    }
}
