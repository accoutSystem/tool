using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MyTool.Log
{
    public partial class ResourceLogPage : Form
    {
        public ResourceLogPage()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var data = PD.Business.DataTransaction.Create();

            string sql = @"select  t_buylog.lastTime,
t_buylog.detail,
 businessName from t_buylog left join t_business on t_buylog.businessId=t_business.businessId where username='"+this.textBox1.Text+"'";
            dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.DataSource = data.DoGetDataTable(sql);
        }
    }
}
