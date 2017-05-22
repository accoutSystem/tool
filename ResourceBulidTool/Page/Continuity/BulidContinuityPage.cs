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
    public partial class BulidContinuityPage : Form
    {
        public bool IsAdd { get; set; }
        public BulidContinuityPage()
        {
            IsAdd = false;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var name = tbName.Text;

            var pw =tbPW.Text;

            var number =Convert.ToInt32(tbNumber.Text);

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("前缀不能为空");
                return;
            }
            if (string.IsNullOrEmpty(pw))
            {
                MessageBox.Show("密码不能为空");
                return;
              
            }
            if (number<=0)
            {
                MessageBox.Show("数量不能为空");
                return;
            }
            var data = PD.Business.DataTransaction.Create();

            List<string> sqls = new List<string>();

            string guid = Guid.NewGuid().ToString();

            string sql = "insert into t_accountgroup(idt_accountGroup,groupName,passWord,number) values('" + guid + "','" + name + "','" + pw + "','" + number + "') ";
            
            sqls.Add(sql);

            for (int i = 0; i <= (number); i++)
            {
                sqls.Add("insert into t_tempaccount(groupId,accountName,isUser) values('" + guid + "','" + (name + i.ToString()) + "','0')");
            }

            data.ExecuteMultiSql(PD.Business.DataUpdateBehavior.Transactional, sqls.ToArray());

            IsAdd = true;

            this.Close();
        }
    }
}
