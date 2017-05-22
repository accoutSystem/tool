using Maticsoft.Model;
using MyDB;
using PD.Business;
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
using AccountRegister;

namespace MyTool
{
    public partial class DataLinkPage : Form
    {
        public DataLinkPage()
        {
            InitializeComponent();
            Load += new EventHandler(DataLinkPage_Load);
        }

        void DataLinkPage_Load(object sender, EventArgs e)
        {
            string s = @" select count(*) from  dbo.T_NewRegisterAccount where  IsEmailActivation=1";

            var data = DoData.Get(s, "system");

            lbCount.Text = data.Rows[0][0].ToString();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定将远程资源数据导出到本地数据库?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Cancel)
                return;

            Thread th = new Thread(new ThreadStart(() =>
            {
                var start = dateTimePicker1.Value.ToString("yyyy-MM-dd");

                var end = dateTimePicker2.Value.ToString("yyyy-MM-dd");

                WriteMessage("读取数据");

                string sql = "select * from T_NewRegisterAccount where createTime >=Convert(datetime,'" + start + " 00:00:01.000') and createTime <=Convert(datetime,'" + end + " 23:59:59.000') and IsEmailActivation=1";

                var source = DoData.Get(sql, "system");

                var count = Convert.ToInt32(tbCount.Text);

                WriteMessage("读取数据完成");

                List<User> newUsers = new List<User>();

                WriteMessage("分析数据");

                Random r = new Random();
                if (source.Rows.Count <= 0) {
                    MessageBox.Show("没有数据");
                    return;
                }
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

                WriteMessage("提取数据完成");

                Storage(newUsers);

                StorageMySql(newUsers);
            }));

            th.Start();
        }

        private void Storage(List<User> newUsers)
        {
            WriteMessage("写入文件");

            try
            {
                string path = System.Environment.CurrentDirectory + @"\Data\User\" + DateTime.Now.ToString("yyyy-MM-dd HH mm") + ".txt";

                FileStream fs = new FileStream(path, FileMode.Append);

                StreamWriter sw = new StreamWriter(fs);

                newUsers.ForEach(item =>
                {
                    sw.WriteLine(item.UserGuid + " " + item.UserName + " " + item.PassWord + " " + item.PassengerName + " " + item.PassengerId + " " + item.CreateTime + " " + item.Email + " " + item.Phone);
                });

                sw.Close();

                sw.Dispose();

                WriteMessage("写入文件成功");
            }
            catch (Exception ex)
            {
                WriteMessage("写入文件失败" + ex.Message);
            }
        }

        private void StorageMySql(List<User> newUsers)
        {
            var data = DataTransaction.Create();

            foreach (var user in newUsers)
            {
                try
                {
                    WriteMessage(user.UserName + "开始存储MySql");

                    data.ExecuteSql(Add(new T_NewAccountEntity
                    {
                        PassengerId = user.PassengerId,
                        IVR_passwd = "198911",
                        PwdQuestion = "changsha",
                        PwdAnswer = "您的出生地是？",
                        CreateTime = Convert.ToDateTime(user.CreateTime),
                        State = 1,
                        LastTime = DateTime.Now,
                        businessId = string.Empty,
                        Email = user.Email,
                        PassengerName = user.PassengerName,
                        Phone = user.Phone,
                        PassWord = user.PassWord,
                        UserGuid = user.UserGuid,
                        UserName = user.UserName
                    }));
                    WriteMessage(user.UserName + "存储MySql完毕");

                    WriteMessage(user.UserName + "开始删除");

                    string sql = "delete T_NewRegisterAccount where UserGuid='" + user.UserGuid + "'";

                    var source = DoData.ExecuteSql(sql, "system");

                    WriteMessage(user.UserName + "删除完毕");
                }
                catch (Exception ex)
                {
                    WriteMessage(user.UserName + "存储失败" + ex.Message);
                }
            }
        }

        public SqlParamterItem Add(T_NewAccountEntity model)
        {
            var sql = new SqlParamterItem();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into t_newaccount(");
            strSql.Append("UserGuid,UserName,PassWord,PassengerName,PassengerId,Email,Phone,State,CreateTime,LastTime,PwdQuestion,PwdAnswer,IVR_passwd,businessId)");
            strSql.Append(" values (");
            strSql.Append("@UserGuid,@UserName,@PassWord,@PassengerName,@PassengerId,@Email,@Phone,@State,@CreateTime,@LastTime,@PwdQuestion,@PwdAnswer,@IVR_passwd,@businessId)");
            ParameterInfo[] parameters = {
					new ParameterInfo("@UserGuid", DbType.String ,36),
					new ParameterInfo("@UserName", DbType.String,40),
					new ParameterInfo("@PassWord", DbType.String,30),
					new ParameterInfo("@PassengerName", DbType.String,20),
					new ParameterInfo("@PassengerId", DbType.String,39),
					new ParameterInfo("@Email", DbType.String,45),
					new ParameterInfo("@Phone", DbType.String,20),
					new ParameterInfo("@State", DbType.Int32  ,11),
					new ParameterInfo("@CreateTime",DbType.DateTime,0),
					new ParameterInfo("@LastTime", DbType.DateTime,0),
					new ParameterInfo("@PwdQuestion", DbType.String,45),
					new ParameterInfo("@PwdAnswer", DbType.String,45),
					new ParameterInfo("@IVR_passwd", DbType.String,45),
					new ParameterInfo("@businessId", DbType.String,45)};
            parameters[0].Value = model.UserGuid;
            parameters[1].Value = model.UserName;
            parameters[2].Value = model.PassWord;
            parameters[3].Value = model.PassengerName;
            parameters[4].Value = model.PassengerId;
            parameters[5].Value = model.Email;
            parameters[6].Value = model.Phone;
            parameters[7].Value = model.State;
            parameters[8].Value = model.CreateTime;
            parameters[9].Value = model.LastTime;
            parameters[10].Value = model.PwdQuestion;
            parameters[11].Value = model.PwdAnswer;
            parameters[12].Value = model.IVR_passwd;
            parameters[13].Value = model.businessId;
            sql.Sql = strSql.ToString();
            sql.ParamterCollection = parameters.ToList();
            return sql;
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
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
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

        private void toolStripStatusLabel3_Click(object sender, EventArgs e)
        {
            TaskStatisticsPage page = new TaskStatisticsPage();
            page.Show();
        }

        private void DataImport()
        {
            Thread th = new Thread(new ThreadStart(() =>
            {
                var data = DataTransaction.Create();

                var source = data.DoGetDataTable("select userName from cx_user1.t_newaccount ");

                var addSource = data.DoGetDataTable("select * from cx_user.t_newaccount ");

                List<DataRow> addRows = new List<DataRow>();

                foreach (DataRow row in addSource.Rows)
                {
                    if (source.Select("userName='" + row["userName"] + "'").Length <= 0)
                    {
                        addRows.Add(row);
                    }
                }

                MessageBox.Show("共"+addRows.Count.ToString()+"数据不匹配"); 
            }));
            th.Start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DataImport();
        }
    }
   
}
