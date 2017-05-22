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
    public partial class MoveAccount : Form
    {
        bool isStart = false;
        public MoveAccount()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (isStart)
                return;
            isStart = true;
            string value = (comboBox1.SelectedItem + "").Split('|')[0];

            Thread th = new Thread(new ThreadStart(() =>
            {
                var data = PD.Business.DataTransaction.Create();

                while (isStart)
                {
                    var number = tbNumber.Text;

                    var source = data.Query("select * from t_newaccount where state=" + value + "  limit 0," + number).Tables[0];

                    if (source.Rows.Count > 0)
                    {
                        StringBuilder message = new StringBuilder();

                        List<string> sqls = new List<string>();

                        foreach (DataRow row in source.Rows)
                        {
                            string userGuid = row["UserGuid"] + string.Empty;
                            string userName = row["UserName"] + string.Empty;
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
                            string accountType = row["accountType"] + string.Empty;
                            string readPassengerState = row["readPassengerState"] + string.Empty;
                            string move = row["move"] + string.Empty;
                            if (string.IsNullOrEmpty(move)) {
                                move = "0";
                            }
                            string deleteAccount = "delete from t_newaccount where UserGuid='" + userGuid + "'";
                           string deleteHisAccount = "delete from t_hisnewaccount where UserGuid='" + userGuid + "'";
                            string addHis = @"insert into t_hisnewaccount(UserGuid,UserName,PassWord,PassengerName,PassengerId,Email,Phone,State,CreateTime,LastTime,PwdQuestion,PwdAnswer,IVR_passwd,businessId,buyTime,isActive,accountType,readPassengerState,move)
values('" + userGuid + "','" + userName + "','" + PassWord + "','" + PassengerName + "','" + PassengerId + "','" + Email + "','" + Phone + "','" + State + "','" + CreateTime + "','" + LastTime + "','" + PwdQuestion + "','" + PwdAnswer + "','" + IVR_passwd + "','" + businessId + "'," + buyTime + ",'" + isActive + "','" + accountType + "','" + readPassengerState + "'," + move + ")";
                          sqls.Add(deleteHisAccount);
                            sqls.Add(addHis);
                            sqls.Add(deleteAccount);
                            message.Append(userName + ",");
                        }
                        try
                        {
                            data.ExecuteMultiSql( PD.Business.DataUpdateBehavior.Transactional, sqls.ToArray());
                        }
                        catch (Exception ex)
                        {
                            ExcuteMessage(ex+""); 
                        }
                        ExcuteMessage("迁移" + message.ToString() + "成功");
                    }
                    else
                    {
                        MessageBox.Show("迁移成功");
                        break;
                    }
                }
            }));

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
                    textBox1.Text = string.Empty;
                }

                textBox1.Text += "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "->" + message;

                textBox1.SelectionStart = this.textBox1.Text.Length;

                textBox1.ScrollToCaret();
            }));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            isStart = false;
        }
    }
}
