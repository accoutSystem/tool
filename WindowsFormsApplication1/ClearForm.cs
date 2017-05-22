using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class ClearForm : Form
    {
        public ClearForm()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Thread th = new Thread(new ThreadStart(() => 
            {
                var start = tbStart.Text;
                var end = tbDate.Text;
                WriteMessage("读取数据");

                string sql = "select * from T_NewRegisterAccount where createTime >=Convert(datetime,'" + tbStart.Text + " 00:00:01.000') and createTime <=Convert(datetime,'" + tbDate.Text + " 23:59:59.000') and IsEmailActivation=1";

                 var source=DoData.Get(sql, "system");

                var count=Convert.ToInt32( tbCount.Text);

                WriteMessage("读取数据完成");

                List<User> newUsers = new List<User>();

                WriteMessage("分析数据");

                Random r = new Random();

                for (int i = 0; i < count; i++)
                {
                    int index = r.Next(source.Rows.Count - 1);

                    var currentData = source.Rows[index];

                    var userName = currentData["UserName"] + string.Empty;

                    if (newUsers.Count(item => item.UserName.Equals(userName)) <= 0)
                    {
                        User newUser = new User
                        {
                            CreateTime = Convert.ToDateTime(currentData["CreateTime"]).ToString("yyyy-MM-dd HH:mm:ss"),
                            Email = currentData["Email"] + string.Empty,
                            IsEmailActivation = currentData["IsEmailActivation"] + string.Empty,
                            PassengerId = currentData["PassengerId"] + string.Empty,
                            PassengerName = currentData["PassengerName"] + string.Empty,
                            PassWord = currentData["PassWord"] + string.Empty,
                            Phone = currentData["Phone"] + string.Empty,
                            UserGuid = currentData["UserGuid"] + string.Empty,
                            UserName = userName
                        };

                        newUsers.Add(newUser);
                    }
                    //写入文件
                }

                WriteMessage("分析数据完成");
                
                Storage(newUsers);
            }));

            th.Start();
        }


        private void Storage(List<User> newUsers)
        {
            WriteMessage("写入文件" );

            try
            {
                string path = System.Environment.CurrentDirectory + @"\Data\User\"+DateTime.Now.ToString("yyyy-MM-dd HH mm")+".txt";

                FileStream fs = new FileStream(path, FileMode.Append);

                StreamWriter sw = new StreamWriter(fs);
                newUsers.ForEach(item => 
                {
                    sw.WriteLine(item.UserGuid + " " + item.UserName + " " + item.PassWord + " " + item.PassengerName + " " + item.PassengerId + " " + item.CreateTime + " " + item.Email + " " + item.Phone  );
                });

                sw.Close();

                sw.Dispose();

                WriteMessage("写入文件成功"  );
            }
            catch (Exception ex)
            {
                WriteMessage("写入文件失败"    + ex.Message);
            }
        }

        private int count = 0;
        private void WriteMessage(string message)
        {
            this.Invoke(new Action(() =>
            {
                count++;
                if (count == 20)
                {
                    count = 0;
                    textBox1.Text = string.Empty;
                }
                textBox1.Text += "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":" + message;
            }));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog()== System.Windows.Forms.DialogResult.OK)
            {
                Thread th = new Thread(new ThreadStart(() =>
                {
                    File.ReadAllLines(openFileDialog1.FileName).ToList().ForEach(item =>
                    {
                        var user = item.Split(' ');

                        string sql = "delete T_NewRegisterAccount where UserGuid='" + user[0] + "'";

                        var source = DoData.ExecuteSql(sql, "system");

                        WriteMessage("D =>" + user[1]);
                    });
                }));
                th.Start();
            }
        }
    }
    public class User {
        public string UserGuid { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public string PassengerName { get; set; }
        public string PassengerId { get; set; }
        public string CreateTime { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string IsEmailActivation { get; set; }
    }
}
