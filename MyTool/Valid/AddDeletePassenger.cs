using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MyTool.Valid
{
    public partial class AddDeletePassenger : Form
    {
        public AddDeletePassenger()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var db = PD.Business.DataTransaction.Create();


            Thread th = new Thread(new ThreadStart(() => {
                List<string> str = textBox1.Text.Split('\r').ToList();

                foreach (var item in str)
                {
                    var data = item.Replace("\n", "").Split(',');
                    var username = data[0];
                    var source = db.Query(@"select state from t_hisnewaccount where username='" + username + "'").Tables[0];
                    if (source.Rows.Count > 0)
                    {
                        string state = (source.Rows[0]["state"] + string.Empty);
                        if (state == "12")
                        {
                            db.ExecuteSql(@"update t_hisnewaccount set state=24 where username='" + username + "'");
                            Console.WriteLine("分析" + username + "完成");
                        }
                        else 
                        {
                            Console.WriteLine(  username + "状态不是已售出"); 
                        }
                    }

                }
                MessageBox.Show("完成");
            }));

            th.Start();
           
        }
    }
}
