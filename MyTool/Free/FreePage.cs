using MyTool.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MyTool.Free
{
    public partial class FreePage : Form
    {
        public FreePage()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var sql = string.Format(@"select a.*,t_useremail.passWord emailPassword from (select  t_newaccount.*  from t_newaccount
                                where t_newaccount.state=10 
and   t_newaccount.accountType=0   limit 0,{0}) a
left join t_useremail on  a.email=t_useremail.email ", textBox1.Text);

            var data = PD.Business.DataTransaction.Create();

            var source = data.Query(sql).Tables[0];

            foreach (DataRow row in source.Rows)
            {
                var info = "用户名:" + row["UserName"] + "密码:" + CXDataCipher.DecipheringUserPW(row["PassWord"]+"") + "邮箱:" + row["Email"] + "邮箱密码:" + row["emailPassword"];

                var pw = ToolCommonMethod.GetRandom(8);

                var add = "insert into t_freeaccount(GetCode,Info,state,createTime) values('" + pw + "','" + info + "',0,'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";

                var update = "update t_freeaccount set state=0 where GetCode='" + pw + "'";
                var update1 = "update t_newaccount set state=12,businessId='13',buyTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where userGuid='" + row["userGuid"] + "'";

                List<string> sqls = new List<string>();
                sqls.Add(add);
                sqls.Add(update);
                sqls.Add(update1);

                data.ExecuteMultiSql(sqls.ToArray());
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var data = PD.Business.DataTransaction.Create();
            var source = data.Query("select * from t_freeaccount where state=0").Tables[0];
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = source;
        }
    }
}
