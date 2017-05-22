using AccountRegister;
using CaptchaServerCacheClient;
using Fangbian.Ticket.Server.AdvanceLogin;
using FangBian.Common;
using Maticsoft.Model;
using MyTool.Common;
using PD.Business;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace ResourceBulidTool.LoginPool
{
    public class UserPoolManager
    {
        private AutoResetEvent orderPayResetEvent = new AutoResetEvent(false);

        private int taskCount = 0;

        bool run = false;

        public bool Run
        {
            get { return run; }
            set { run = value; }
        }
        public void Stop()
        {
            run = false;
            orderPayResetEvent.Set();
        }
        private void WriteMessage(string message)
        {
            RegisterMain.Current.WriteMessage(message);
        }
        public static DataTable DeserializeDataTable(string pXml)
        {

            StringReader strReader = new StringReader(pXml);
            XmlReader xmlReader = XmlReader.Create(strReader);
            XmlSerializer serializer = new XmlSerializer(typeof(DataTable));

            DataTable dt = serializer.Deserialize(xmlReader) as DataTable;

            return dt;
        }

        public int MaxTaskNumber { get; set; }

        public void Start(int maxTaskNumber)
        {
            if (run)
                return;

            MaxTaskNumber = maxTaskNumber;
           //maxTaskNumber= maxTaskNumber + 5;
            if (run == false)
            {
                run = true;
            }

            Thread th = new Thread(new ThreadStart(() =>
            {
                while (run)
                {
                    try
                    {
                        var source = DeserializeDataTable(HttpHelper.Get("http://121.43.110.247/BaseServer/api/getstatus.ashx?taskid=passengerCache"));

                        if (source != null)
                        {
                            var count = Convert.ToInt32(source.Rows[0]["可用优质身份证"]);
                            if (count < 100)
                            {
                                break;
                            }
                        }
                        WriteMessage("还存在优质身份证...");
                    }
                    catch {
                        WriteMessage("异常...");
                    }
                    Thread.Sleep(10000);
                }

                var data = DataTransaction.Create();

                while (run)
                {
                    if (ToolCommonMethod.IsProvideServer()==false)
                    {
                        WriteMessage("12306系统维护，不填充账号数据...");
                        Thread.Sleep(30000);
                        continue;
                    }
                    if (AccountActivationPool.Current.Count >= MaxTaskNumber)
                    {
                        WriteMessage("池满，等待中。。。");
                        Thread.Sleep(2000);
                        continue;
                    }

                    if (taskCount >= MaxTaskNumber)
                    {
                        WriteMessage("池满，等待中。。。");

                        orderPayResetEvent.WaitOne();
                    }

                    if (AccountActivationPool.Current.Count >= MaxTaskNumber)
                    {
                        WriteMessage("池满，等待中。。。");

                        Thread.Sleep(2000);

                        continue;
                    }

                    var source = GetAccountServer.Get();

                    if (source != null && !string.IsNullOrEmpty((source.UserName+"").Trim()) && !string.IsNullOrEmpty((source.PassWord+"").Trim()))
                    {
                        string userName = source.UserName;
                        string passWord = source.PassWord;

                        if (AccountActivationPool.Current.FirstOrDefault(acc => acc.CurrentUser.UserName.Equals(userName)) == null)
                        {
                            AccountActivation account = new AccountActivation();
                         
                            account.Data = source.UserGuid;

                            account.Activation(new Fangbian.Tickets.Trains.WFDataItem.Account12306Item
                            {
                                UserName = userName,
                                PassWord = passWord
                            });

                            WriteMessage(account.CurrentUser.UserName + " " + account.CurrentUser.PassWord + "开始入池");
                            account.UserLoginCompleted += account_AccountCompleted;
                            AccountActivationPool.Current.AddAccount(account);
                            taskCount++;
                        }
                        else
                        {
                            WriteMessage(userName + " " + passWord + "已存在池中");
                        }
                    }
                    else
                    {
                        WriteMessage("注册用来核验的账号的账号队列为空");
                        Thread.Sleep(1000);
                    }
                }
            }));
            th.Start();
        }


        void account_AccountCompleted(object sender, AccountLoginEventArgs e)
        {
            var item = sender as AccountActivation;

            if (e.IsLogin)
            {
                if (e.FullPassenger)
                {
                    AccountState.UpdateState(item.Data + string.Empty, 23);
                    AccountActivationPool.Current.RemoveAccount(item);
                }
                if (e.ValidPassenger && e.FullPassenger == false)
                {
                    WriteMessage(item.CurrentUser.UserName + " " + item.CurrentUser.PassWord + "入池成功");

                    item.State = AccountTaskState.登录完成;
                }
            }
            else
            {
                AccountActivationPool.Current.RemoveAccount(item);

                WriteMessage(item.CurrentUser.UserName + " " + item.CurrentUser.PassWord + "入池失败:" + item.State);

                if (item.State == AccountTaskState.身份未通过)
                {
                    AccountState.UpdateState(item.Data + string.Empty, 5);
                }
                else if(item.State== AccountTaskState.账号已损坏)
                {
                    AccountState.UpdateState(item.Data + string.Empty, 8);
                }
            }
            taskCount--;

            orderPayResetEvent.Set();
        }
    }

    public class AccountState
    {
        public static void UpdateState(string user, int newState)
        {
            var sql = new SqlParamterItem();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update t_newaccount set state=" + newState + " ,LastTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where userGuid=@userGuid");
            ParameterInfo[] parameters = {
					new ParameterInfo("@UserGuid", DbType.String ,36) };
            parameters[0].Value = user;

            sql.Sql = strSql.ToString();

            sql.ParamterCollection = parameters.ToList();

            DataTransaction.Create().ExecuteSql(sql);
        }
    }
}
