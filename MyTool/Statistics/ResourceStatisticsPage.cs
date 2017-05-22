using Fangbian.Ticket.Server.AdvanceLogin;
using PD.Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MyTool.Statistics
{
    public partial class ResourceStatisticsPage : Form
    {
        public ResourceStatisticsPage()
        {
            InitializeComponent();
            Load += ResourceStatisticsPage_Load;

        }

        void ResourceStatisticsPage_Load(object sender, EventArgs e)
        {
            dateTimePicker1.Value = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-01"));
            dateTimePicker2.Value = DateTime.Now;
            Query();
        }

        private void Query()
        {
            Thread th = new Thread(new ThreadStart(() =>
            {
                //0 新注册 1通过邮箱核验 2 已通过验证 3已出 4已出未核验 5未核验 6已出且验证 7入队列 8损坏账号  9损坏账号资源回收
                string sql = string.Format(@"select  date_format(CreateTime,'%Y-%c-%d') date,
           SUM(CASE State WHEN 0 THEN 1 ELSE 0 END) newUser ,
           SUM(CASE State WHEN 1 THEN 1 ELSE 0 END) email ,
SUM(CASE State WHEN 2 THEN 1 ELSE 0 END) user ,
SUM(CASE State WHEN 3 THEN 1 ELSE 0 END) buy ,
SUM(CASE State WHEN 4 THEN 1 ELSE 0 END) buyNoValid ,
SUM(CASE State WHEN 5 THEN 1 ELSE 0 END) noValid ,
SUM(CASE State WHEN 6 THEN 1 ELSE 0 END) buyValid ,
SUM(CASE State WHEN 8 THEN 1 ELSE 0 END) bad ,
SUM(CASE State WHEN 9 THEN 1 ELSE 0 END) badInRecover , 
SUM(CASE State WHEN 7 THEN 1 ELSE 0 END) useing  
                            from t_newaccount 
where createTime>='{0}' and createTime<='{1}'
 group by  date_format(CreateTime,'%Y-%c-%d')", dateTimePicker1.Value.ToString("yyyy-MM-dd 00:00:01"),

                                              dateTimePicker2.Value.ToString("yyyy-MM-dd 23:59:59"));
                var source = DataTransaction.Create().DoGetDataTable(sql);
                this.Invoke(new Action(() =>
                {
                    dataGridView1.DataSource = source;

                }));
            }));
            th.Start();
        }


        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            ResourceDetailPage page = new ResourceDetailPage();
            page.Date = (dataGridView1.Rows[e.RowIndex].DataBoundItem as DataRowView).Row["date"] + string.Empty;
            page.ShowDialog();
        }

        private void CheckNewAccountStateClick(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count <= 0)
            {
                MessageBox.Show("选择一行");
                return;
            }
            VailMainPage page = new VailMainPage();

            page.CurrentDate = (dataGridView1.SelectedRows[0].DataBoundItem as DataRowView).Row["date"] + string.Empty;

            page.IsNewRegister = true;

            page.Show();
        }

        private void QueryClick(object sender, EventArgs e)
        {
            Query();
        }

        private void BadResourceRecoverClick(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count <= 0)
            {
                MessageBox.Show("选择一行");
                return;
            }
            if (MessageBox.Show("确定回收坏账号资源？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            //选中哪天记录
            //修改用户状态到9 损坏账号已回收
            //邮件资源状态变为0
            //身份证数据状态变为0
            var currentDate = (dataGridView1.SelectedRows[0].DataBoundItem as DataRowView).Row["date"] + string.Empty;
            var data = DataTransaction.Create();
            Thread th = new Thread(new ThreadStart(() =>
            {
                string sql = string.Format(@"select userGuid,username,password,passengerid,email from t_newaccount where state=8
and  createtime>='{0} 00:00:01' and createtime<='{0} 23:59:59'", currentDate);

                var source = data.DoGetDataTable(sql);
                List<string> sqls = new List<string>();
                int i = 0;
                foreach (DataRow currentItem in source.Rows)
                {
                    SetInfo("开始回收" + currentItem["username"] + "资源");

                    string updateAccountState = string.Format("update t_newaccount set state=9 where userguid='{0}'", currentItem["userGuid"]);
                    string updateEmailState = string.Format("update t_email set state=0 where email='{0}'", currentItem["email"]);
                    string updatePassengerState = string.Format("update t_passenger set state=0 where  idNo='{0}'", currentItem["passengerid"]);

                    if (i > 200)
                    {
                        data.ExecuteMultiSql(DataUpdateBehavior.Transactional, sqls.ToArray());
                        sqls.Clear(); 
                        i = 0;
                    }
                    else
                    {
                        i++; 
                        sqls.Add(updateAccountState);
                        sqls.Add(updateEmailState);
                        sqls.Add(updatePassengerState);
                    }
                    SetInfo("回收" + currentItem["username"] + "资源完成");
                }
                if (sqls.Count > 0)
                {
                    data.ExecuteMultiSql(DataUpdateBehavior.Transactional, sqls.ToArray());
                }
                SetInfo("回收资源完成");

            }));
            th.Start();
        }

        private void SetInfo(string message) {
            this.Invoke(new Action(() => {
                lbInfo.Text = message;
            }));
        }
    }
}
