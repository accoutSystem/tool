using AccountRegister;
using Fangbian.Common;
using Fangbian.Data.Struct.Event;
using Fangbian.DataStruct.Business;
using Fangbian.DataStruct.Business.TrainTicket;
using Fangbian.Tickets.Trains;
using Fangbian.Tickets.Trains.DataTransferObject.Response.Login;
using Fangbian.Tickets.Trains.WFDataItem;
using Maticsoft.Model;
using PD.Business;
using ResourceBulidTool.LoginPool;
using System;
using System.Activities;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Fangbian.Ticket.Server.AdvanceLogin
{
    /// <summary>
    /// 激活账号(初始化和预先登录)
    /// </summary>
    public class AccountActivation
    {
        /// <summary>
        /// 账号预登录完成
        /// </summary>
        public event EventHandler<AccountLoginEventArgs> UserLoginCompleted;

        public event EventHandler AddPassengerCompleted;

        public event EventHandler DeletePassengerCompleted;

        public DateTime LastOperationTime { get; set; }

        private bool isLock = false;

        private object lockObj = new object();
        public bool IsLock
        {
            get { return isLock; }
            set
            {
                lock (lockObj)
                {
                    isLock = value;
                }
            }
        }

        public event EventHandler<ConsoleMessageEventArgs> OutputMessage;

        public AccountTaskState State { get; set; }

        public Account12306Item CurrentUser { get; set; }

        public RequestSession CurrentSession { get; set; }

        public string CurrentMessage { get; set; }
        public int UseCount { get; set; }

        public int QueryCount { get; set; }

        List<PassengerItem> deletePassenger = new List<PassengerItem>();

        public object Data { get; set; }

        public DateTime CreateTime { get; set; }

        public AccountActivation()
        {
            CreateTime = DateTime.Now;
            LastOperationTime = DateTime.Now;
        }

        /// <summary>
        /// 激活当前账号
        /// </summary>
        /// <param name="user"></param>
        public void Activation(Account12306Item user)
        {
            LastOperationTime = DateTime.Now;

            IsLock = true;
            State = AccountTaskState.正在登录;

            Message(user.UserName + " " + user.PassWord + "开始登录");

            CurrentUser = user;

            CurrentSession = new RequestSession() { UserName = user.UserName, UserPassWord = user.PassWord };

            var wfFile = ActivityXamlServices.Load(RegisterMain.Path + "用户登录初始化.XAML");

            var dic = new Dictionary<string, object>();

            dic.Add("userAccount", user);

            dic.Add("requestSession", CurrentSession);

            dic.Add("activityResult", new ActivityExcuteResult { ExcuteCode = ActivityResultCode.Success });

            var myInstance = new WorkflowApplication(wfFile, dic);

            myInstance.Completed = eventArgs =>
            {
                LastOperationTime = DateTime.Now;

                var requestSession = eventArgs.Outputs.ContainsKey("requestSession") ? eventArgs.Outputs["requestSession"] as RequestSession : null;

                var activityResult = dic["activityResult"] as ActivityExcuteResult;

                Message(user.UserName + " " + user.PassWord + "登录完成");

                if (activityResult.ExcuteCode == ActivityResultCode.Success)
                {
                    requestSession.CurrentAccount = eventArgs.Outputs["accountInfo"] as AccountInfo;

                    var currentPassenger=requestSession.CurrentAccount.CurrentUserPassengers.FirstOrDefault(item=>item.IdNo.Equals(requestSession.CurrentAccount.IdNo));
                    //InsertPassenger(requestSession.CurrentAccount);
                    if (currentPassenger != null)
                    {
                        if (currentPassenger.Status == 0&&requestSession.CurrentAccount.DisplayControlFlag.Equals("1"))
                        {
                            foreach (var passenger in requestSession.CurrentAccount.CurrentUserPassengers)
                            {
                                if (passenger.Status != 0)
                                {
                                    deletePassenger.Add(passenger);
                                }
                            }

                            var passengerCount = requestSession.CurrentAccount.CurrentUserPassengers.Count;

                            var deleteCount = deletePassenger.Count;

                            if ((passengerCount - deleteCount) < 15)
                            {
                                State = AccountTaskState.登陆后清理联系人;
                                DeletePassengerManager deleteP = new DeletePassengerManager { DeletePassengers = deletePassenger, CurrentSession = requestSession };
                                deleteP.DeletePassengerCompleted += ClearPassengerCompleted;
                                deleteP.Start();
                            }
                            else
                            {
                                ExcuteLoginCompleted(true, true, true);
                            }
                        }
                        else
                        {
                            State = AccountTaskState.身份未通过;
                            ExcuteLoginCompleted(false, false, false);
                        }
                    }
                    else
                    {
                        State = AccountTaskState.身份未通过;
                        ExcuteLoginCompleted(false, false, false);
                    }
                }
                else
                {
                    if (activityResult.Error_Message.Contains("登录名不存在")
                    || activityResult.Error_Message.Contains("请核实您注册用户信息是否真实")
                    || activityResult.Error_Message.Contains("密码输入错误")
                    || activityResult.Error_Message.Contains("该用户已被暂停使用")
                    || activityResult.Error_Message.Contains("您的用户信息被他人冒用"))
                    {
                        State = AccountTaskState.账号已损坏;
                    }
                    else
                    {
                        State = AccountTaskState.登录失败;
                    }
                    ExcuteLoginCompleted(false, false, false);
                    Message(user.UserName + " " + user.PassWord + "登录完成,失败" + activityResult.Error_Message);
                }
            };
            myInstance.Run();
        }

        private void InsertPassenger(AccountInfo account)
        {
            try
            {
                Message("开始提取"+account.User_name+"身份证");
                var data = DataTransaction.Create();
                List<string> sqls = new List<string>();
                foreach (var passenger in account.CurrentUserPassengers)
                {
                    if (passenger.IdNo.ToLower().Equals(account.IdNo.ToLower()))
                    {
                        continue;
                    }
                    if (passenger.Status == 0)
                    {
                        string sql = "select count(*) from t_passenger where idNo='" + passenger.IdNo + "'";

                        var count = Convert.ToInt32(data.DoGetDataTable(sql).Rows[0][0]);

                        if (count <= 0)
                        {
                            sql = "select count(*) from t_userpassenger where idNo='" + passenger.IdNo + "'";
                            count = Convert.ToInt32(data.DoGetDataTable(sql).Rows[0][0]);
                            if (count <= 0)
                            {
                                sqls.Add("insert into  t_passenger(passengerId,name,idNo,state) values('" + Guid.NewGuid().ToString() + "','" + passenger.UserName + "','" + passenger.IdNo + "',0)");
                            }
                        }
                    }
                }
                if (sqls.Count > 0)
                {
                    data.ExecuteMultiSql(DataUpdateBehavior.Transactional, sqls.ToArray());
                    Message("提取" + account.User_name + "身份证完成");
                }
            }
            catch (Exception ex)
            {
                Message("提取身份证失败" + ex.Message);
            }
        }

        void ClearPassengerCompleted(object sender, EventArgs e)
        {
            LastOperationTime = DateTime.Now;

            if (CurrentSession.CurrentAccount.CurrentUserPassengers.Count >= 15)
            {
                ExcuteLoginCompleted(true, true, true);
            }
            else
            {
                var currentPassenger = CurrentSession.CurrentAccount.CurrentUserPassengers.FirstOrDefault(item => item.IdNo.ToLower().Equals(CurrentSession.CurrentAccount.IdNo.ToLower()));

                if (currentPassenger == null)
                {
                    ExcuteLoginCompleted(true, false, false);
                }
                else
                {
                    if (currentPassenger.Status == 0)
                    {
                        ExcuteLoginCompleted(true, false, true);
                        IsLock = false;
                    }
                    else
                    {
                        ExcuteLoginCompleted(true, false, false);
                    }
                }
            }
        }

        /// <summary>
        /// 添加乘车人 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="idno"></param>
        public void AddPassenger(string name, string idno)
        {
            LastOperationTime = DateTime.Now;

            UseCount++;
            IsLock = true;
            CurrentMessage = "添加联系人" + name + " " + idno;
            WriteMessage("添加联系人" + name + " " + idno);
            State = AccountTaskState.正在添加联系人;
            var addPassenger = new PassengerItem
          {
              UserName = name,
              IdNo = idno,
              IdType = "1",
              UserType = "1",
              MobileNo = string.Empty,
              EnterYear = string.Empty,
              PreferenceFromStationCode = string.Empty,
              PreferenceToStationCode = string.Empty,
              ProvinceCode = string.Empty,
              SchoolCode = string.Empty,
              SchoolSystem = string.Empty,
              StudentNo = string.Empty
          };

            var wfFile = ActivityXamlServices.Load(RegisterMain.Path + "添加乘车人.xaml");

            var dic = new Dictionary<string, object>();

            dic.Add("requestSession", this.CurrentSession);

            dic.Add("activityResult", new ActivityExcuteResult { ExcuteCode = ActivityResultCode.Success });

            dic.Add("accountInfo", CurrentSession.CurrentAccount);

            dic.Add("newPassenger", new List<PassengerItem>() { addPassenger });

            var myInstance = new WorkflowApplication(wfFile, dic);

            myInstance.Completed = eventArgs =>
            {
                LastOperationTime = DateTime.Now;

                var activityResult = dic["activityResult"] as ActivityExcuteResult;

                if (activityResult.ExcuteCode == ActivityResultCode.Success)
                {
                    WriteMessage("添加联系人" + name + " " + idno + "完成");
                    State = AccountTaskState.添加联系人完成;
                    Thread.Sleep(1500);
                    QueryPassenger(idno);
                }
                else
                {
                 
                    WriteMessage("添加联系人失败:" + activityResult.Error_Message);

                    Log.Logger.Debug("添加联系人失败:" + activityResult.Error_Message);

                    State = AccountTaskState.添加联系人失败;
                    
                    if (activityResult.Error_Message.Contains("请先登录"))
                    {
                        Activation(CurrentUser);
                    }
                    else if (activityResult.Error_Message.Contains("但您的常用联系人数量超过上限")||
                        activityResult.Error_Message.Contains("联系人数量已超过上限"))
                    {
                        AccountState.UpdateState(this.Data + string.Empty, 22);
                        AccountActivationPool.Current.RemoveAccount(this);
                    }
                    else if (activityResult.Error_Message.Contains("到就近办理客运售票业务的铁路车站完成身份核验，通过后即可在网上办理购票业务，谢谢。") )
                    {
                        Thread.Sleep(1500);
                        QueryPassenger(idno);
                        return;
                    }
                    if (activityResult.Error_Message.Contains("您的身份信息未通过核验"))
                    {
                        Log.Logger.Fatal(activityResult.Error_Message + " " + CurrentSession.UserName + " " + CurrentSession.UserPassWord);
                        State = AccountTaskState.身份未通过;
                    }
                    if (AddPassengerCompleted != null)
                    {
                        AddPassengerCompleted(this, EventArgs.Empty);
                    }
                    IsLock = false;
                }
            };
            try
            {
                myInstance.Run();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        /// <summary>
        /// 查询乘车人
        /// </summary>
        public void QueryPassenger(string idNo)
        {
            LastOperationTime = DateTime.Now;

            QueryCount++;

            IsLock = true;

            CurrentMessage = idNo + "开始执行查询锁定对象。。。";

            State = AccountTaskState.正在查询联系人;

            WriteMessage("开始查询联系人");

            var wfFile = ActivityXamlServices.Load(RegisterMain.Path + "查询乘车人.xaml");

            var dic = new Dictionary<string, object>();

            dic.Add("requestSession", this.CurrentSession);

            dic.Add("activityResult", new ActivityExcuteResult { ExcuteCode = ActivityResultCode.Success });

            dic.Add("accountInfo", this.CurrentSession.CurrentAccount);

            var myInstance = new WorkflowApplication(wfFile, dic);

            myInstance.Completed = eventArgs =>
            {
                LastOperationTime = DateTime.Now;

                var activityResult = dic["activityResult"] as ActivityExcuteResult;

                if (activityResult.ExcuteCode == ActivityResultCode.Success)
                {
                    var newAccount = eventArgs.Outputs["accountInfo"] as AccountInfo;

                    CurrentSession.CurrentAccount.CurrentUserPassengers = newAccount.CurrentUserPassengers;

                    var passenger = CurrentSession.CurrentAccount.CurrentUserPassengers.FirstOrDefault(item => item.IdNo.ToLower().Equals(idNo.ToLower()));

                    if (passenger != null)
                    {
                        QueryCount = 0;

                        State = AccountTaskState.查询联系人完成;

                        CurrentMessage = idNo + "查询联系人完成，开始执行事件。。。";

                        if (AddPassengerCompleted != null)
                        {
                            AddPassengerCompleted(this, EventArgs.Empty);
                        }
                        CurrentMessage = idNo + "查询联系人完成，执行事件完毕。。。";
                        IsLock = false;
                        CurrentMessage = idNo + "查询联系人完成，执行事件完毕后释放锁。。。";
                        State = AccountTaskState.登录完成;
                        CurrentMessage = idNo + "查询联系人完成，执行事件完毕后释放锁变为登录完成。。。";
                    }
                    else
                    {
                        CurrentMessage = idNo + "不存在重新查询。。。";
                        WriteMessage(idNo + "不存在重新查询");
                        if (QueryCount <= 5)
                        {
                            Thread.Sleep(1500);
                            QueryPassenger(idNo);
                        }
                        else
                        {
                            QueryCount = 0;
                        
                            if (AddPassengerCompleted != null)
                            {
                                AddPassengerCompleted(this, EventArgs.Empty);
                            }
                            IsLock = false;
                        }
                    }
                }
                else
                {
                    WriteMessage("查询联系人失败:" + activityResult.Error_Message);

                    Log.Logger.Debug("查询联系人失败:" + activityResult.Error_Message);

                    State = AccountTaskState.查询联系人失败;
                    
                    if (QueryCount > 5)
                    {
                        QueryCount = 0;
                        if (AddPassengerCompleted != null)
                        {
                            AddPassengerCompleted(this, EventArgs.Empty);
                        }
                        IsLock = false;
                        return;
                    }

                    if (activityResult.Error_Message.Contains("请先登录"))
                    {
                        Activation(CurrentUser);
                    }
                    else
                    {
                        Thread.Sleep(1500);
                        QueryPassenger(idNo);
                        return;
                    }

                    if (AddPassengerCompleted != null)
                    {
                        AddPassengerCompleted(this, EventArgs.Empty);
                    }
                }
            };

            myInstance.Run();
        }

        /// <summary>
        /// 删除乘车人 
        /// </summary>
        public void DeletePassenger(string idno)
        {
            LastOperationTime = DateTime.Now;

            IsLock = true;

            State = AccountTaskState.删除联系人;

            var deletePassenger = CurrentSession.CurrentAccount.CurrentUserPassengers.FirstOrDefault(item => item.IdNo.ToLower().Equals(idno.ToLower()));

            if (deletePassenger == null)
            {
                State = AccountTaskState.删除联系人失败;

                if (DeletePassengerCompleted != null)
                {
                    DeletePassengerCompleted(this, EventArgs.Empty);
                }
                return;
            }
            var wfFile = ActivityXamlServices.Load(RegisterMain.Path + "删除联系人.xaml");

            var dic = new Dictionary<string, object>();

            dic.Add("passenger", deletePassenger);

            dic.Add("requestSession", CurrentSession);

            dic.Add("activityResult", new ActivityExcuteResult { ExcuteCode = ActivityResultCode.Success });

            var myInstance = new WorkflowApplication(wfFile, dic);

            myInstance.Completed = eventArgs =>
            {
                LastOperationTime = DateTime.Now;

                var requestSession = dic["requestSession"];

                var activityResult = dic["activityResult"] as ActivityExcuteResult;

                if (activityResult.ExcuteCode == ActivityResultCode.Success)
                {
                    State = AccountTaskState.删除联系人成功;
                }
                else
                {
                    WriteMessage("删除联系人失败:" + activityResult.Error_Message);

                    Log.Logger.Debug("删除联系人失败:" + activityResult.Error_Message);

                    State = AccountTaskState.删除联系人失败;
                 
                }
                if (DeletePassengerCompleted != null)
                {
                    DeletePassengerCompleted(this, EventArgs.Empty);
                }
                State = AccountTaskState.登录完成;
                IsLock = false;

                if ((activityResult.Error_Message+string.Empty).Contains("请先登录"))
                {
                    Activation(CurrentUser);
                }

            };
            myInstance.Run();
        }

        private void ExcuteLoginCompleted(bool isLogin, bool fullPassenger, bool vali)
        {
            if (UserLoginCompleted != null)
            {
                UserLoginCompleted(this, new AccountLoginEventArgs { FullPassenger = fullPassenger, ValidPassenger = vali, IsLogin = isLogin });
            }
        }

        private void Message(string message)
        {
            WriteMessage(message);
        }

        private void WriteMessage(string message)
        {
            CurrentMessage = message;
            if (CurrentUser != null)
            {
                RegisterMain.Current.WriteMessage(CurrentUser.UserName + " " + CurrentUser.PassWord + ":" + message);
            }
        }
    }

    public class DeletePassengerManager
    {
        private List<PassengerItem> deletePassengers = new List<PassengerItem>();

        public event EventHandler DeletePassengerCompleted;
 
        public List<PassengerItem> DeletePassengers
        {
            get { return deletePassengers; }
            set { deletePassengers = value; }
        }

        public RequestSession CurrentSession { get; set; }

        private int index = 0;

        /// <summary>
        /// 删除乘车人 
        /// </summary>
        public void DeletePassenger(string idno)
        {
            var deletePassenger = CurrentSession.CurrentAccount.CurrentUserPassengers.FirstOrDefault(item => item.IdNo.ToLower().Equals(idno.ToLower()));

            if (deletePassenger == null)
            {
                index++;
                Start();
                return;
            }

            var wfFile = ActivityXamlServices.Load(RegisterMain.Path + "删除联系人.xaml");

            var dic = new Dictionary<string, object>();

            dic.Add("passenger", deletePassenger);

            dic.Add("requestSession", CurrentSession);

            dic.Add("activityResult", new ActivityExcuteResult { ExcuteCode = ActivityResultCode.Success });

            var myInstance = new WorkflowApplication(wfFile, dic);

            myInstance.Completed = eventArgs =>
            {
                var requestSession = dic["requestSession"];

                var activityResult = dic["activityResult"] as ActivityExcuteResult;

                if (activityResult.ExcuteCode == ActivityResultCode.Success)
                {
                    CurrentSession.CurrentAccount.CurrentUserPassengers.Remove(deletePassenger);
                }
                else
                {
                    Log.Logger.Debug("核验乘车人账号进行删除联系人失败:" + activityResult.Error_Message);
                    Console.WriteLine("核验乘车人账号进行删除联系人失败:" + activityResult.Error_Message);
                }
                index++;
                Thread.Sleep(1000);
                Start();
            };
            myInstance.Run();
        }

        internal void Start()
        {
            if (index >= DeletePassengers.Count || DeletePassengers.Count <= 0)
            {
                if (DeletePassengerCompleted != null)
                {
                    DeletePassengerCompleted(this, EventArgs.Empty);
                }
            }
            DeletePassenger(DeletePassengers[index].IdNo);
        }
    }

    public class AccountLoginEventArgs : EventArgs
    {
        public AccountLoginEventArgs()
        {
            IsLogin = false;
            FullPassenger = false;
            ValidPassenger = false;
        }

        /// <summary>
        /// 是否登录成功
        /// </summary>
        public bool IsLogin { get; set; }

        /// <summary>
        /// 是否邮件激活
        /// </summary>
        public bool FullPassenger { get; set; }

        /// <summary>
        /// 是否通过身份核验
        /// </summary>
        public bool ValidPassenger { get; set; }
    }

    public enum AccountTaskState
    {
        正在登录 = 0,
        登录完成 = 2,
        登录失败 = 3,
        正在添加联系人 = 4,
        添加联系人完成 = 5,
        添加联系人失败 = 6,
        正在查询联系人 = 7,
        查询联系人完成 = 8,
        查询联系人失败 = 9,
        删除联系人 = 10,
        删除联系人成功 = 11,
        删除联系人失败 = 12,
        登陆后清理联系人 = 13,
        身份未通过 = 14,
        账号已损坏 = 15,
    }

    public class AccountActivationPool : List<AccountActivation>
    {

        private static AccountActivationPool current;

        public static AccountActivationPool Current
        {
            get
            {
                if (current == null)
                {
                    current = new AccountActivationPool();
                }
                return AccountActivationPool.current;
            }
        }

        public event EventHandler ItemChange;

        public void AddAccount(AccountActivation item)
        {
            this.Add(item);
            if (ItemChange != null)
            {
                ItemChange(this, EventArgs.Empty);
            }
        }

        public void RemoveAccount(AccountActivation item)
        {
            this.Remove(item);
            if (ItemChange != null)
            {
                ItemChange(this, EventArgs.Empty);
            }
        }

        public void ClearAccount() {
            this.Clear();
            if (ItemChange != null)
            {
                ItemChange(this, EventArgs.Empty);
            }
        }

        public AccountActivation Get()
        {
            while (true)
            {
                try
                {
                    var current = this.FirstOrDefault(item => item.State == AccountTaskState.登录完成 && item.IsLock == false);

                    if (current == null)
                    {
                        Thread.Sleep(2000);
                    }
                    else
                    {
                        if (current.CurrentSession.CurrentAccount.CurrentUserPassengers.Count >= 15)
                        {
                            AccountState.UpdateState(current.Data + string.Empty, 22);
                            this.RemoveAccount(current);
                            continue;
                        }
                        return current;
                    }
                }
                catch(Exception ex)
                {
                    Log.Logger.Debug("获取核验的乘车人的账号失败"+ex.Message);
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
