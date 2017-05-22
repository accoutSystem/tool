using Fangbian.Ticket.Server.AdvanceLogin;
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
    public partial class ValidUserPage : Form
    {
        public ValidUserPage()
        {
            InitializeComponent();
        }

        private void ValidUserPage_Load(object sender, EventArgs e)
        {
            Refurbish();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Refurbish();
        }

        private void Refurbish()
        {
            DataTable source = new DataTable();
            source.Columns.Add("UserName");
            source.Columns.Add("PassWord");
            source.Columns.Add("Number");
            source.Columns.Add("CreateTime");
            source.Columns.Add("State");
            source.Columns.Add("Lock");
            source.Columns.Add("CurrentMessage");
            source.Columns.Add("UseCount");
            source.Columns.Add("LastOperationTime");

            AccountActivationPool.Current.ForEach(item =>
            {
                source.Rows.Add(item.CurrentUser.UserName,
                    item.CurrentUser.PassWord,
                  (  item.CurrentSession != null && item.CurrentSession.CurrentAccount!=null) ? item.CurrentSession.CurrentAccount.CurrentUserPassengers.Count : 0,
                    item.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    item.State.ToString(),
                    item.IsLock ? "锁定中" : "未锁定",
                item.CurrentMessage, item.UseCount.ToString() + "次", item.LastOperationTime.ToString("yyyy-MM-dd HH:mm:ss"));
            });
            dataGridView1.DataSource = source;
        }
    }
}
