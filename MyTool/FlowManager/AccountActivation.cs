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
    public class AccountActivation
    {
        /// <summary>
        /// 账号预登录完成
        /// </summary>
        public event EventHandler<AccountLoginEventArgs> AccountCompleted;

        public Account12306Item currentUser { get; set; }

        public T_NewAccountEntity Passenger { get; set; }

        public object Data { get; set; }

        private ActivityExcuteResult currentLoginResult { get; set; }

        public event EventHandler<ConsoleMessageEventArgs> OutputMessage;

        /// <summary>
        /// 对账号乘客和订单进行检查
        /// </summary>
        public bool CheckAccount { get; set; }

        public AccountActivation()
        {
            CheckAccount = true;
        }

        /// <summary>
        /// 激活当前账号
        /// </summary>
        /// <param name="user"></param>
        public void Activation(Account12306Item user)
        {
            Message(user.UserName + " " + user.PassWord + "开始登录");
            currentUser = user;

            var currentSession = new RequestSession() { UserName = user.UserName, UserPassWord = user.PassWord };

            var wfFile = ActivityXamlServices.Load(ToolCommon.Path + "用户登录初始化.XAML");

            var dic = new Dictionary<string, object>();

            dic.Add("userAccount", user);

            dic.Add("requestSession", currentSession);

            dic.Add("activityResult", new ActivityExcuteResult { ExcuteCode = ActivityResultCode.Success });

            var myInstance = new WorkflowApplication(wfFile, dic);

            myInstance.Completed = eventArgs =>
            {
                var requestSession = eventArgs.Outputs.ContainsKey("requestSession") ? eventArgs.Outputs["requestSession"] as RequestSession : null;

                var activityResult = dic["activityResult"] as ActivityExcuteResult;

                currentLoginResult = activityResult;

                Message(user.UserName + " " + user.PassWord + "登录完成");

                if (activityResult.ExcuteCode == ActivityResultCode.Success)
                {
                    Log.Logger.Info("主服务程序预登录成功:" + user.UserName);

                    if (AccountCompleted != null)
                    {
                        requestSession.CurrentAccount = eventArgs.Outputs["accountInfo"] as AccountInfo;

                        if (Passenger == null)
                        {
                            Passenger = new T_NewAccountEntity { PassengerId = requestSession.CurrentAccount.IdNo };
                        }

                        var currentItem = requestSession.CurrentAccount.CurrentUserPassengers.FirstOrDefault(item => item.IdNo.Trim().ToLower().Equals(Passenger.PassengerId.Trim().ToLower()));

                        if (currentItem == null)
                        {
                            if (requestSession.CurrentAccount.DisplayControlFlag != "1")
                            {
                                AccountCompleted(this, new AccountLoginEventArgs
                                {
                                    IsLogin = true,
                                    IsValid = false,
                                });
                            }
                            else 
                            {
                                AccountCompleted(this, new AccountLoginEventArgs
                                {
                                    IsLogin = true,
                                    IsValid = true,
                                    Message = "联系人" + requestSession.CurrentAccount.CurrentUserPassengers.Count + "个",
                                    IsActive = requestSession.CurrentAccount.IsActive.ToUpper().Equals("Y"),
                                    IsReceive = requestSession.CurrentAccount.IsReceive.ToUpper().Equals("Y")
                                });
                            }
                        }
                        else
                        {
                            if (currentItem != null && currentItem.Status != 0 || requestSession.CurrentAccount.DisplayControlFlag != "1")
                            {
                                AccountCompleted(this, new AccountLoginEventArgs
                                {
                                    IsLogin = true,
                                    IsValid = false,
                                    PassengerCount=requestSession.CurrentAccount.CurrentUserPassengers.Count,
                                    Message = currentItem.UserName + "未核验"
                                });
                            }
                            else
                            {
                                AccountCompleted(this, new AccountLoginEventArgs
                                {
                                    IsLogin = true,
                                    IsValid = true,
                                    PassengerCount= requestSession.CurrentAccount.CurrentUserPassengers.Count ,
                                    Message = "联系人" + requestSession.CurrentAccount.CurrentUserPassengers.Count + "个",
                                    IsActive = requestSession.CurrentAccount.IsActive.ToUpper().Equals("Y"),
                                    IsReceive = requestSession.CurrentAccount.IsReceive.ToUpper().Equals("Y")
                                });
                            }
                        }

                    }
                }
                else
                {
                    Message(user.UserName + " " + user.PassWord + "登录完成,失败" + activityResult.Error_Message);

                    Log.Logger.Error("主服务程序预登录错误:" + activityResult.Error_Message);

                    if (AccountCompleted != null)
                    {
                        AccountCompleted(this, new AccountLoginEventArgs
                        {
                            IsLogin = false,
                            Message = activityResult.Error_Message
                        });
                    }
                }
            };
            myInstance.Run();
        }

        private void Message(string message)
        {
            if (OutputMessage != null)
            {
                OutputMessage(this, new ConsoleMessageEventArgs { Message = message });
            }
        }
    }

    public class AccountLoginEventArgs : EventArgs
    {
        public AccountLoginEventArgs()
        {
            IsValid = IsActive = IsReceive = IsLogin = false;
        }
        /// <summary>
        /// 是否登录成功
        /// </summary>
        public bool IsLogin { get; set; }
        /// <summary>
        /// 身份是否核验
        /// </summary>
        public bool IsValid { get; set; }
        /// <summary>
        /// 是否邮件激活
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 是否通过手机核验
        /// </summary>
        public bool IsReceive { get; set; }

        public string Message { get; set; }

        public int PassengerCount { get; set; }

        public string ErrorMessage { get; set; }
    }
}
