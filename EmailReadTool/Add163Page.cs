using MyTool.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace VaildTool
{
    public partial class Add163Page : Form
    {
        public Add163Page()
        {
            InitializeComponent();
        }
        private int success = 0;

        public int Success
        {
            get { return success; }
            set { success = value;

            Invoke(new Action(() => {
                lbSuccess.Text = value + string.Empty;
            }));
            }
        }

        private int exit = 0;

        public int Exit
        {
            get { return exit; }
            set
            {
                exit = value;
                Invoke(new Action(() =>
                {
                    lbExit.Text = value + string.Empty;
                }));
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            var emails = textBox1.Lines.ToList();
            Thread th = new Thread(new ThreadStart(() =>
            {
                var data = PD.Business.DataTransaction.Create();
                foreach (var email in emails)
                {
                    var emialdata = email.Split(',');
                    if (emialdata.Length <= 1)
                        continue;
                    var addEmail = emialdata[0];
                    var addEmailPW = emialdata[1];
                    var userEmail = data.Query("select count(*) from t_useremail where email='" + addEmail + "'").Tables[0];
                    if (Convert.ToInt32(userEmail.Rows[0][0] + string.Empty) <= 0)
                    {
                        var noUserEmail = data.Query("select count(*)  from t_email  where email='" + addEmail + "'").Tables[0];
                        if (Convert.ToInt32(noUserEmail.Rows[0][0] + string.Empty) <= 0)
                        {
                            string sql = string.Format("insert into t_email(emailId,Email,PassWord,createTime,lastTime,state)values('{0}','{1}','{2}','{3}','{4}',0)",
                                Guid.NewGuid().ToString(),
                                addEmail,
                                addEmailPW,
                                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                            data.ExecuteSql(sql);
                            Success++;
                        }
                        else
                        {
                            Exit++;
                        }
                    }
                    else
                    {
                        Exit++;
                    }
                }
            }));
            th.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int number = Convert.ToInt32(textBox2.Text);
            StringBuilder bulid = new StringBuilder();
            for (int i = 0; i < number; i++) 
            {
                var name = ToolCommonMethod.GetCharRandom(4) + ToolCommonMethod.GetRandom(8) + ToolCommonMethod.GetCharRandom(2);
                bulid.Append(name+"@163.com,");
                var pw= ToolCommonMethod.GetCharRandom(2) +  ToolCommonMethod.GetRandom(5)+"LC";
                bulid.Append(pw);
                bulid.Append("\r\n");
            }
            textBox1.Text = bulid.ToString();
            //shunbi71520137ta@163.com ak35946
        }
    }
}
