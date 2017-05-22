using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using PD.Business;

namespace MyTool.Passenger
{
    public partial class PassengerManagerPage : Form
    {
        public PassengerManagerPage()
        {
            InitializeComponent();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var badPassengers = File.ReadAllLines(openFileDialog1.FileName).ToList();

                var data = DataTransaction.Create();

                int i = 0;

                List<string> sqls = new List<string>();

                badPassengers.ForEach(passengerItem =>
                {
                    var passengers = passengerItem.Split(' ');

                    var passengerName = passengers[0];

                    var passenerId = passengers[1];

                    WriteMessage("开始搜索" + passengerName);

                    string sql = "select count(*) from t_badpassenger where passengerName='" + passengerName + "'";

                    var count = Convert.ToInt32(data.DoGetDataTable(sql).Rows[0][0]);
                 
                    if (count <= 0)
                    {
                        i++;

                        sql = string.Format("insert into t_badpassenger(passengerGuid,passengerName,passengerId) values('{0}','{1}','{2}')",  Guid.NewGuid().ToString(), passengerName, passenerId);

                        sqls.Add(sql);

                        if (i > 100)
                        {
                            WriteMessage("开始插入" + passengerName);
                            data.ExecuteMultiSql( DataUpdateBehavior.Transactional, sqls.ToArray());
                            WriteMessage("成功插入" + passengerName);
                            i = 0;
                            sqls.Clear();
                        }
                    }
                      
                    else
                    {
                        WriteMessage(passengerName + "已存在");
                    }
                });
            }
        }

        int count = 0;

        private void WriteMessage(string message)
        {
            this.Invoke(new Action(() =>
            {
                count++;

                if (count == 50)
                {
                    count = 0;
                    textBox1.Text = string.Empty;
                }

                textBox1.Text += "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":" + message;

                textBox1.SelectionStart = this.textBox1.Text.Length;

                textBox1.ScrollToCaret();
            }));
        }
    }
}
