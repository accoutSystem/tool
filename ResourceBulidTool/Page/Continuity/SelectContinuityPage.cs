using ResourceBulidTool.Entity;
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
    public partial class SelectContinuityPage : Form
    {
        public SelectContinuityPage()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            this.Load += SelectContinuityPage_Load;

        }

        void SelectContinuityPage_Load(object sender, EventArgs e)
        {
            ReLoad();
        }

        public void ReLoad()
        {
            var data = PD.Business.DataTransaction.Create();

            var source = data.DoGetDataTable("select * from t_accountgroup");

            dataGridView2.AutoGenerateColumns = false;

            dataGridView2.DataSource = source;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            BulidContinuityPage bulid = new BulidContinuityPage();

            bulid.ShowDialog();

            if (bulid.IsAdd)
            {
                ReLoad();
            }
        }
        DataTable currentSource = null;
        private void dataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count <= 0)
            {
                return;
            }
            var select = (dataGridView2.SelectedRows[0].DataBoundItem as DataRowView);
            var ss = select.Row["idt_accountGroup"];
            var data = PD.Business.DataTransaction.Create();
            var source = data.DoGetDataTable(string.Format(@" select * from  t_tempaccount where groupId='{0}'", ss));
            currentSource = source;
            comboBox1_SelectedIndexChanged(this, null);
        }

        public void Bind(List<DataRow> source)
        {
            dataGridView1.AutoGenerateColumns = false;

            dataGridView1.DataSource = source;

            foreach (DataRow row in source)
            {
                dataGridView1.Rows[source.IndexOf(row)].Cells[0].Value = row["accountName"] + "";

                dataGridView1.Rows[source.IndexOf(row)].Cells[1].Value = ((row["isUser"] + "").Equals("1")) ? "否" : "是";
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (currentSource != null)
            {
                switch (comboBox1.SelectedItem + string.Empty)
                {
                    case "全部": 
                        Bind(currentSource.Select().ToList());
                        break;
                    case "可注册":
                        Bind(currentSource.Select("isUser=0").ToList());
                        break;
                    case "已用":
                        Bind(currentSource.Select("isUser=2").ToList());
                        break;
                }
            }
        }

      

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            //var items = dataGridView1.SelectedRows;

            //foreach (DataGridViewRow item in items)
            //{
            //    CustomUserCollection.Current.Add(new CustomUserItem
            //    {
            //        State = 0,
            //        UserName =(item.DataBoundItem as DataRow)["accountName"]+string.Empty,
            //        PassWord = (dataGridView2.SelectedRows[0].DataBoundItem as DataRowView).Row["passWord"] + string.Empty,
            //    });
            //}

            this.Close();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            var items = dataGridView1.SelectedRows;

            foreach (DataGridViewRow item in items)
            {
                string userName = (item.DataBoundItem as DataRow)["accountName"] + string.Empty;
                string isUser = (item.DataBoundItem as DataRow)["isUser"] + string.Empty;

                var current=CustomUserCollection.Current.FirstOrDefault(a=>a.UserName.Equals(userName));

                if (current == null && isUser.Equals("0"))
                {
                    CustomUserCollection.Current.Add(new CustomUserItem
                    {
                        State = 0,
                        UserName = userName,
                        PassWord = (dataGridView2.SelectedRows[0].DataBoundItem as DataRowView).Row["passWord"] + string.Empty,
                    });
                }
            }
            toolStripLabel1.Text = CustomUserCollection.Current.Count + string.Empty;
        }
    }
}
