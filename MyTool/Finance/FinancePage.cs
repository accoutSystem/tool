using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MyTool.Finance;
using MyEntiry;
using MyDB;
using PD.Business;
using Maticsoft.Model;

namespace MyTool
{
    public partial class FinancePage : Form
    {
        public FinancePage()
        {
            InitializeComponent();
            Load += new EventHandler(FinancePage_Load);
        }

        void FinancePage_Load(object sender, EventArgs e)
        {
            lbBusiness.DisplayMember = "Name";
            lbBusiness.ValueMember = "Id";
            lbBusiness.DataSource =ToolMain.BusinessCollection  ;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            AddFinancePage page = new AddFinancePage();

            if (page.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Query();
            }
        }

        private void lbBusiness_SelectedIndexChanged(object sender, EventArgs e)
        {
            Query();
        }

        private void Query()
        {
            var currentBusiness = lbBusiness.SelectedItem as T_Business;

            if (currentBusiness == null)
            {

                return;
            }
            string sql = string.Format(@"select businessfinance,amount,createTime,resourceNumber,
 FORMAT(amount/resourceNumber,2) price  from t_businessfinance where business_id='{0}' order by createTime desc", currentBusiness.Id);

            var source = DataTransaction.Create().DoGetDataTable(sql);
            dpFinace.AutoGenerateColumns = false;
            dpFinace.DataSource = source;
            if (source.Rows.Count > 0)
            {
                foreach (DataRow row in source.Rows)
                {
                    if (Convert.ToDecimal(row["amount"]) <= 0) {

                        dpFinace.Rows[source.Rows.IndexOf(row)].DefaultCellStyle.BackColor = Color.Red;
                        dpFinace.Rows[source.Rows.IndexOf(row)].DefaultCellStyle.SelectionBackColor = Color.Red;
                    }
                } 
            }
            decimal allAmount = 0;

            foreach (DataRow row in source.Rows)
            {
                allAmount += Convert.ToDecimal(row["amount"]);
            }
            lbCurrentBusinessAmount.Text = allAmount.ToString();
            sql = " select sum(amount) from t_businessfinance";
            source = DataTransaction.Create().DoGetDataTable(sql);
            lbAllAmount.Text = source.Rows[0][0].ToString();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (dpFinace.SelectedRows.Count <= 0)
            {
                MessageBox.Show("请选择一个商户数据");
                return;
            }
            var currentBusiness = lbBusiness.SelectedItem as T_Business;
            var data = (dpFinace.SelectedRows[0].DataBoundItem as DataRowView).Row;

            EditFinancePage page = new EditFinancePage();
            page.SetSource(new Maticsoft.Model.T_BusinessFinance
            {
                resourceNumber = Convert.ToInt32(data["resourceNumber"] + string.Empty),
                createTime = Convert.ToDateTime(data["createTime"]),
                businessfinance = data["businessfinance"] + string.Empty,
                business_id = currentBusiness.Id,
                amount = Convert.ToDecimal(data["amount"] + string.Empty),
            });
            if (page.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Query();


            }
        }

        private void dpFinace_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            //if (e.ColumnIndex == 3)
            //{
            //    if ((dpFinace.Rows[e.RowIndex].DataBoundItem as T_BusinessFinance).amount <= 0)
            //    {
            //        e.CellStyle.BackColor = SystemColors.GrayText;
            //    }
            //    else {
            //        e.CellStyle.BackColor = SystemColors.Window;
                
            //    }
            //}
            //else {
            //    e.CellStyle.BackColor = SystemColors.Window;
            //}
        }

        private void AddBusinessClick(object sender, EventArgs e)
        {
            AddBusinessPage page = new AddBusinessPage();
            page.ShowDialog();

        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            var currentBusiness = lbBusiness.SelectedItem as T_Business;

            if (currentBusiness != null)
            {
                AddBusinessPage page = new AddBusinessPage();
                page.IsEdit = true;
                page.BusinessId = currentBusiness.Id;
                page.BusinessName = currentBusiness.Name;
                page.Remark = currentBusiness.Remark;
                page.ShowDialog();
            }
            else {
                MessageBox.Show("请选择商户");
            }
        }
    }
}
