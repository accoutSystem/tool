using Fangbian.Common;
using Fangbian.Data.Struct.Event;
using Fangbian.DataStruct.Business;
using Fangbian.DataStruct.Business.TrainTicket;
using Fangbian.Tickets.Trains;
using Fangbian.Tickets.Trains.DataTransferObject.Response.Login;
using Fangbian.Tickets.Trains.WFDataItem;
using Maticsoft.Model;
using MyTool.FlowManager;
using Newtonsoft.Json.Linq;
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
        public event EventHandler<EmailValidLoginEventArgs> AccountCompleted;

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
            Message(user.UserName + "开始登录");

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

                Message(user.UserName + "登录完成");

                if (activityResult.ExcuteCode == ActivityResultCode.Success)
                {
                    var result = eventArgs.Outputs["accountInfo"] as AccountInfo;
                    var isActive = (result.IsActive + string.Empty).Equals("Y");
                    if (AccountCompleted != null)
                    {
                        AccountCompleted(this, new EmailValidLoginEventArgs
                        {
                            IsLogin = true,
                            IsValid = isActive,
                            Message = activityResult.Error_Message
                        });
                    }
                }
                else
                {
                    Message(user.UserName + "登录完成,失败" + activityResult.Error_Message);

                    if (AccountCompleted != null)
                    {
                        AccountCompleted(this, new EmailValidLoginEventArgs
                        {
                            IsLogin = false,
                            IsValid = false,
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

    public class EmailValidLoginEventArgs : EventArgs
    {
        public bool IsLogin { get; set; }
        public bool IsValid { get; set; }
        public string Message { get; set; }
    }
}
