using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MyTool.Valid.PW
{
    public partial class ConvertNewAccountPage : Form
    {
        public ConvertNewAccountPage()
        {
            InitializeComponent();
        }

        private void ConvertNewAccountPage_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;

            Thread th = new Thread(new ThreadStart(() =>
            {
                var data = PD.Business.DataTransaction.Create();

                var source = data.Query("select * from t_hisnewaccount where state=10 limit 0,"+textBox1.Text).Tables[0];

                if (source.Rows.Count > 0)
                {


                    foreach (DataRow row in source.Rows)
                    {
                        List<string> sqls = new List<string>();

                        string userGuid = row["UserGuid"] + string.Empty;
                        string userName = row["userName"] + string.Empty;
                        string PassWord = row["PassWord"] + string.Empty;
                        string PassengerName = row["PassengerName"] + string.Empty;
                        string PassengerId = row["PassengerId"] + string.Empty;
                        string Email = row["Email"] + string.Empty;
                        string Phone = row["Phone"] + string.Empty;
                        string State = row["State"] + string.Empty;
                        string CreateTime = row["CreateTime"] + string.Empty;
                        string LastTime = row["LastTime"] + string.Empty;
                        string PwdQuestion = row["PwdQuestion"] + string.Empty;
                        string PwdAnswer = row["PwdAnswer"] + string.Empty;
                        string IVR_passwd = row["IVR_passwd"] + string.Empty;
                        string businessId = row["businessId"] + string.Empty;
                        string buyTime = row["buyTime"] + string.Empty;
                        buyTime = string.IsNullOrEmpty(buyTime) ? "null" : "'" + buyTime + "'";
                        string isActive = row["isActive"] + string.Empty;
                        string accountType = "5"; 
                        string readPassengerState = row["readPassengerState"] + string.Empty;

                        string addHis = @"insert into t_newaccount(UserGuid,UserName,PassWord,PassengerName,PassengerId,Email,Phone,State,CreateTime,LastTime,PwdQuestion,PwdAnswer,IVR_passwd,businessId,buyTime,isActive,accountType,readPassengerState)
values('" + userGuid + "','" + userName + "','" + PassWord + "','" + PassengerName + "','" + PassengerId + "','" + Email + "','" + Phone + "','" + State + "','" + CreateTime + "','" + LastTime + "','" + PwdQuestion + "','" + PwdAnswer + "','" + IVR_passwd + "','" + businessId + "'," + buyTime + ",'" + isActive + "','" + accountType + "','" + readPassengerState + "')";
                       
                        string deleteAccount = "delete from t_hisnewaccount where UserGuid='" + userGuid + "'";

                        sqls.Add(addHis);

                        sqls.Add(deleteAccount);

                        data.ExecuteMultiSql(sqls.ToArray());
                        Console.WriteLine(userName+"迁移成功");
                    }

                    MessageBox.Show("迁移成功");

                    Invoke(new Action(() => {
                        button1.Enabled = true;
                    }));
                }
                    
            }));

            th.Start();
        }
    }
}
