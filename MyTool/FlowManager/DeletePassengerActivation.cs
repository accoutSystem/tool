using Fangbian.Common;
using Fangbian.Data.Struct.Event;
using Fangbian.DataStruct.Business;
using Fangbian.DataStruct.Business.TrainTicket;
using Fangbian.Tickets.Trains;
using Fangbian.Tickets.Trains.DataTransferObject.Response.Login;
using Fangbian.Tickets.Trains.WFDataItem;
using Maticsoft.Model;
using MyTool.FlowManager;
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
    public class DeletePassengerActivation
    {
        /// <summary>
        /// 账号预登录完成
        /// </summary>
        public event EventHandler<DeletePassengerEventArgs> DeleteCompleted;
        public event EventHandler DeleteSuccessCount;

        public Account12306Item currentUser { get; set; }

        public T_NewAccountEntity Passenger { get; set; }

        public object Data { get; set; }

        private ActivityExcuteResult currentLoginResult { get; set; }

        private RequestSession currentSession = null;

        public event EventHandler<ConsoleMessageEventArgs> OutputMessage;

        /// <summary>
        /// 对账号乘客和订单进行检查
        /// </summary>
        public bool CheckAccount { get; set; }

        public DeletePassengerActivation()
        {
            CheckAccount = true;
        }

        /// <summary>
        /// 激活当前账号
        /// </summary>
        /// <param name="user"></param>
        public void Activation(Account12306Item user)
        {
            Message(user.UserName +" "+user.PassWord+ "开始登录");

            currentUser = user;

            currentSession = new RequestSession() { UserName = user.UserName, UserPassWord = user.PassWord };

            var wfFile = ActivityXamlServices.Load(ToolCommon.Path + "用户登录初始化.XAML");

            var dic = new Dictionary<string, object>();

            dic.Add("userAccount", user);

            dic.Add("requestSession", currentSession);

            dic.Add("activityResult", new ActivityExcuteResult { ExcuteCode = ActivityResultCode.Success });

            var myInstance = new WorkflowApplication(wfFile, dic);

            myInstance.Completed = eventArgs =>
            {
                var requestSession = eventArgs.Outputs.ContainsKey("requestSession") ? eventArgs.Outputs["requestSession"] as RequestSession : null;
                requestSession.CurrentAccount = eventArgs.Outputs["accountInfo"] as AccountInfo;
                var activityResult = dic["activityResult"] as ActivityExcuteResult;

                currentLoginResult = activityResult;

                Message(user.UserName  +" "+user.PassWord+ "登录完成");

                if (activityResult.ExcuteCode == ActivityResultCode.Success)
                {
                    Log.Logger.Info("主服务程序预登录成功:" + user.UserName);

                    var currentPassenger = requestSession.CurrentAccount.CurrentUserPassengers.FirstOrDefault(item => item.IdNo.Equals(requestSession.CurrentAccount.IdNo));

                    if (currentPassenger.Status == 0 && requestSession.CurrentAccount.DisplayControlFlag.Equals("1"))
                    {
                        List<PassengerItem> deletePassenger = new List<PassengerItem>();

                        foreach (var passenger in requestSession.CurrentAccount.CurrentUserPassengers)
                        {
                            if (passenger.Status != 0)
                            {
                                deletePassenger.Add(passenger);
                            }
                        }
                        if (deletePassenger.Count <= 0 && requestSession.CurrentAccount.CurrentUserPassengers.Count >= 15)
                        {
                            Message(user.UserName + "联系人已满且全部身份通过");
                            Excute(true, true, true, false);
                        }
                        else
                        {
                            Message(user.UserName + "开始删除" + deletePassenger.Count + "个乘客");
                            DeletePassengerManager deleteP = new DeletePassengerManager { DeletePassengers = deletePassenger, CurrentSession = requestSession };
                            deleteP.DeletePassengerCompleted += deleteP_DeletePassengerCompleted;
                            deleteP.OutputMessage += deleteP_OutputMessage;
                            deleteP.DeleteSuccessCount += deleteP_DeleteSuccessCount;
                            deleteP.Start();
                        }
                    }
                    else
                    {
                        Message(user.UserName + "身份未通过");
                        Excute(false, true, true, false);
                    }
                }
                else
                {
                    Message(user.UserName + " " + user.PassWord + "登录完成,失败" + activityResult.Error_Message);

                    Log.Logger.Error("主服务程序预登录错误:" + activityResult.Error_Message);
                    if (activityResult.Error_Message.Contains("登录名不存在")
                     || activityResult.Error_Message.Contains("请核实您注册用户信息是否真实")
                     || activityResult.Error_Message.Contains("密码输入错误")
                     || activityResult.Error_Message.Contains("该用户已被暂停使用")
                     || activityResult.Error_Message.Contains("您的用户信息被他人冒用"))
                    {
                        Excute(false, false, true,true);
                    }
                    else
                    {
                        Excute(false, false, false,false);
                    }
                }
            };
            myInstance.Run();
        }

        void deleteP_DeleteSuccessCount(object sender, EventArgs e)
        {
            if (DeleteSuccessCount != null) {
                DeleteSuccessCount(this, EventArgs.Empty);
            }
        }

        void deleteP_OutputMessage(object sender, ConsoleMessageEventArgs e)
        {
            Message(e.Message);
        }

        void deleteP_DeletePassengerCompleted(object sender, EventArgs e)
        {
            if (this.currentSession.CurrentAccount.CurrentUserPassengers.Count >= 15)
            {
                Excute(true, true, true,false);
            }
            else
            {
                Excute(true, false, true,false);
            }
        }

        private void Excute(bool isValid, bool isFull,bool isLogin,bool isBad) {
            if (DeleteCompleted != null) {
                DeleteCompleted(this, new DeletePassengerEventArgs { IsBad=isBad, IsFull=isFull, IsValid=isValid, IsLogin=isLogin });
            }
        }

        private void Message(string message)
        {
            if (OutputMessage != null)
            {
                OutputMessage(this, new ConsoleMessageEventArgs { Message = message });
            }
        }
    }

    public class DeletePassengerManager
    {
        private List<PassengerItem> deletePassengers = new List<PassengerItem>();

        public event EventHandler DeletePassengerCompleted;

        public event EventHandler DeleteSuccessCount;

        public event EventHandler<ConsoleMessageEventArgs> OutputMessage;

        public List<PassengerItem> DeletePassengers
        {
            get { return deletePassengers; }
            set { deletePassengers = value; }
        }

        public RequestSession CurrentSession { get; set; }

        private int deleteCount;

        public int DeleteCount
        {
            get { return deleteCount; }
            set { deleteCount = value; if (DeleteSuccessCount != null) {
                DeleteSuccessCount(this, EventArgs.Empty);
            } }
        }

        private int index = 0;
        private void Message(string message)
        {
            if (OutputMessage != null)
            {
                OutputMessage(this, new ConsoleMessageEventArgs { Message = message });
            }
        }
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
            Message("开始删除" + deletePassenger.UserName+" "+deletePassenger.IdNo);
            var wfFile = ActivityXamlServices.Load(ToolCommon.Path + "删除联系人.xaml");

            var dic = new Dictionary<string, object>();

            dic.Add("passenger", deletePassenger);

            dic.Add("requestSession", CurrentSession);

            dic.Add("activityResult", new ActivityExcuteResult { ExcuteCode = ActivityResultCode.Success });

            var myInstance = new WorkflowApplication(wfFile, dic);

            myInstance.Completed = eventArgs =>
            {
                var requestSession = dic["requestSession"];

                var activityResult = dic["activityResult"] as ActivityExcuteResult;

                Message("删除" + deletePassenger.UserName + " " + deletePassenger.IdNo + "" + activityResult.ExcuteCode + " " + activityResult.Error_Message);
                if (activityResult.ExcuteCode == ActivityResultCode.Success)
                {
                    CurrentSession.CurrentAccount.CurrentUserPassengers.Remove(deletePassenger);
                    DeleteCount++;
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
            if (index >= DeletePassengers.Count)
            {
                if (DeletePassengerCompleted != null)
                {
                    DeletePassengerCompleted(this, EventArgs.Empty);
                }
            }
            DeletePassenger(DeletePassengers[index].IdNo);
        }
    }

    public class DeletePassengerEventArgs : EventArgs
    {
        public DeletePassengerEventArgs()
        {
            IsLogin = IsValid = IsFull = false;
        }
        /// <summary>
        /// 账号是否通过验证码
        /// </summary>
        public bool IsLogin { get; set; }

        /// <summary>
        /// 账号是否通过验证码
        /// </summary>
        public bool IsBad { get; set; }

        /// <summary>
        /// 账号是否通过验证码
        /// </summary>
        public bool IsValid { get; set; }
        /// <summary>
        /// 是否删除后还是满的
        /// </summary>
        public bool IsFull { get; set; }
    }
}
