using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using PD.Business;
using MyDB;
using System.IO;
using Fangbian.Tickets.Trains.DataTransferObject.Request;
using MyTool.Common;
using AccountRegister;

namespace MyTool
{
    public partial class BulidUserPage : Form
    {
        public BulidUserPage()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Thread th=new Thread(new ThreadStart(()=>
            {
                var data = DoData.Get(@"select a.Email from dbo.T_NewRegisterAccount a inner join t_email b on a. Email=b.Email
where b.state=0","system");

                WriteMessage("需要修改" + data.Rows.Count+"行");
                int i = 0;
                StringBuilder str = new StringBuilder();
                foreach (DataRow row in data.Rows)
                {
                    str.Append("'" + row["Email"] + "',");
                    if (i > 100)
                    {
                        str.Remove(str.Length - 1, 1);
                        Update(str);
                        str.Clear();
                        i=0;
                    }
                    i++;
                }
                if (i > 0)
                {
                    str.Remove(str.Length - 1, 1);
                    Update(str);
                }
            }));
            th.Start();
        }

        private void Update( StringBuilder str  )
        {
            WriteMessage("开始修改" );
            DoData.ExecuteSql("update T_Email set state=1 where email in(" + str.ToString() + ")", "system");
            WriteMessage("修改成功");
        }

        int count = 0;

        private void WriteMessage(string message)
        {
            this.Invoke(new Action(() =>
            {
                count++;

                if (count == 100)
                {
                    count = 0;
                    textBox1.Text = string.Empty;
                }

                textBox1.Text += "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":" + message;

                textBox1.SelectionStart = this.textBox1.Text.Length;

                textBox1.ScrollToCaret();
            }));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var startTime = Convert.ToDateTime(dateTimePicker1.Value.ToString("yyyy-MM-dd"));

            var day = Convert.ToDateTime(dateTimePicker2.Value.ToString("yyyy-MM-dd")) - startTime;

            Random r = new Random();

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                
                var lines = File.ReadAllLines(openFileDialog1.FileName,Encoding.Default).ToList();

                var emailSource = DoData.Get("select top " + lines.Count + " email from T_Email where state=0", "system");

                int index = 0;

                lines.ForEach(passenger =>
                {
                    var userName = passenger.Split(' ');

                    var item = RegisterMethod.GetChineseSpellCode(userName[0]);

                    var birthday = RegisterMethod.GetBirthdayIDNo(userName[1]);

                    var random = RegisterMethod.GetRandom(2);

                    Random es = new Random();

                    string newCode = RegisterMethod.GetRandom(6);
 
                    newCode = item + newCode;

                    var newUserItem = new RegisterUserInfo
                    {
                        Email = emailSource.Rows[index]["email"] + string.Empty,
                        UserName = item + birthday + random,
                        PassWord = newCode,
                        MobileNo = "15675871661",
                        IdNo = userName[1] ,
                        Name = userName[0]
                    };

                    index++;

                    DateTime currentDate = startTime.AddDays( r.Next(day.Days));
                    
                    var dateTime = currentDate.ToString("yyyy-MM-dd 1" + RegisterMethod.GetRandom(1) + ":" + RegisterMethod.GetRandom(1, 59) + ":" + RegisterMethod.GetRandom(1, 59));
                     
                    WriteDB(newUserItem, newCode, "15675871661", dateTime);
                });
            }
        }

        public void WriteDB(RegisterUserInfo item, string password, string phone,string createTime)
        {
            try
            {
                WriteMessage("开始写入数据库" + item.UserName);

                string sql = string.Format("insert into T_NewRegisterAccount(UserGuid,UserName,PassWord,PassengerName,PassengerId,CreateTime,Email,Phone,IsEmailActivation)values('{0}','{1}','{2}','{3}','{4}','{7}','{5}','{6}',5)",
                    Guid.NewGuid().ToString(), item.UserName, password, item.Name, item.IdNo, item.Email, phone,  createTime);

                DoData.ExecuteSql(sql, "system");

                sql = string.Format("update dbo.T_Email set state=2 where Email='{0}'", item.Email);

                DoData.ExecuteSql(sql, "system");

                WriteMessage("写入数据库成功" + item.UserName);

                WriteMessage("开始写入MySql数据库" + item.UserName);

                sql = string.Format("insert into t_badaccount(badGuid,userName,createTime) values('{0}','{1}','{2}')",
                  Guid.NewGuid().ToString(), item.UserName,DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                DataTransaction.Create().ExecuteSql(sql);
                WriteMessage("写入MySql数据库成功" + item.UserName);

            }
            catch (Exception ex)
            {
                WriteMessage("写入数据库失败" + item.UserName + ex.Message);

            }
        }

        private void BulidUserPage_Load(object sender, EventArgs e)
        {

        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {
            new TaskStatisticsPage().Show();
        }
    }
}
