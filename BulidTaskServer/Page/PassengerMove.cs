using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace BulidTaskServer.Page
{
    public partial class PassengerMove : Form
    {
        public PassengerMove()
        {
            InitializeComponent();
        }

        private void PassengerMove_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var number = textBox1.Text;
            Thread th = new Thread(new ThreadStart(new Action(() =>
            {
                var db = PD.Business.DataTransaction.Create();

                var source = db.Query("select passengerid,name,idno from t_passenger where move=0 and state=3 limit 0," + number).Tables[0];

                foreach (DataRow row in source.Rows)
                {
                    var name = row["name"] + string.Empty;
                    var id = row["passengerid"] + string.Empty;
                    var idNo = row["idno"] + string.Empty;

                    db.ExecuteSql("update t_passenger set state=0,move=2 where passengerid='" + id + "'");

                    ExcuteMessage("迁移生僻身份证【"+name+" "+idNo+"】成功");
                }

            })));
            th.Start();
        }

        int messageCount = 0;
        private void ExcuteMessage(string message)
        {
            this.Invoke(new Action(() =>
            {
                messageCount++;

                if (messageCount == 100)
                {
                    messageCount = 0;
                    textBox2.Text = string.Empty;
                }

                textBox2.Text += "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "->" + message;

                textBox2.SelectionStart = this.textBox1.Text.Length;

                textBox2.ScrollToCaret();
            }));
        }
    }
}
