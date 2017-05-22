using MyTool.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MyTool.Export
{
    public partial class ExportConsecutiveResourcePage : BaseForm
    {
        public ExportConsecutiveResourcePage()
        {
            InitializeComponent();
        }

        private void ExportConsecutiveResourcePage_Load(object sender, EventArgs e)
        {
            ReLoad();
            this.Enabled = false;
        }

        public void ReLoad()
        {
            var data = PD.Business.DataTransaction.Create();

            var source = data.DoGetDataTable("select * from t_accountgroup");
            
            foreach (DataRow row in source.Rows)
            {
                listBox1.Items.Add(row["groupName"] + "");
            }
        }

    }
}
