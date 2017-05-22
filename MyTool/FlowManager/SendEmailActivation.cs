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
    public class SendEmailActivation
    {
        /// <summary>
        /// 账号预登录完成
        /// </summary>
        public event EventHandler<SendEmailEventArgs> SendEmailCompleted;

        public Account12306Item currentUser { get; set; }

        public T_NewAccountEntity Passenger { get; set; }

        public object Data { get; set; }

        private ActivityExcuteResult currentLoginResult { get; set; }

        public event EventHandler<ConsoleMessageEventArgs> OutputMessage;

        /// <summary>
        /// 对账号乘客和订单进行检查
        /// </summary>
        public bool CheckAccount { get; set; }

        RequestSession currentSession = null;

        public SendEmailActivation()
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

                var activityResult = dic["activityResult"] as ActivityExcuteResult;

                currentLoginResult = activityResult;

                Message(user.UserName + "登录完成");

                if (activityResult.ExcuteCode == ActivityResultCode.Success)
                {
                    Log.Logger.Info("主服务程序预登录成功:" + user.UserName);

                    if (SendEmailCompleted != null)
                    {
                        requestSession.CurrentAccount = eventArgs.Outputs["accountInfo"] as AccountInfo;

                        if (requestSession.CurrentAccount.IsActive.ToLower().Equals("n"))
                        {
                            SendEmail();
                        }
                        else
                        {
                            if (requestSession.CurrentAccount.IsActive.ToLower().Equals("y") &&
                                requestSession.CurrentAccount.IsReceive.ToLower() != ("y"))
                            {
                                if (SendEmailCompleted != null)
                                {
                                    SendEmailCompleted(this, new SendEmailEventArgs
                                    {
                                        IsLogin = true,
                                        Message = "邮件已经核验"
                                    });
                                }
                            }

                            if (requestSession.CurrentAccount.IsActive.ToLower().Equals("y") &&
                               requestSession.CurrentAccount.IsReceive.ToLower().Equals("y"))
                            {
                                if (SendEmailCompleted != null)
                                {
                                    SendEmailCompleted(this, new SendEmailEventArgs
                                    {
                                        IsLogin = true,
                                        Message = "邮件手机均核验"
                                    });
                                }
                            }
                          
                        }
                    }
                }
                else
                {
                    Message(user.UserName + "登录完成,失败" + activityResult.Error_Message);

                    Log.Logger.Error("主服务程序预登录错误:" + activityResult.Error_Message);

                    if (SendEmailCompleted != null)
                    {
                        SendEmailCompleted(this, new SendEmailEventArgs
                        {
                            IsLogin = false,
                            Message = activityResult.Error_Message
                        });
                    }
                }
            };
            myInstance.Run();
        }

        private void SendEmail()
        {
            var wfFile = ActivityXamlServices.Load(ToolCommon.Path + "发送邮件.XAML");

            var dic = new Dictionary<string, object>();

            dic.Add("requestSession", currentSession);

            dic.Add("activityResult", new ActivityExcuteResult { ExcuteCode = ActivityResultCode.Success });

            var myInstance = new WorkflowApplication(wfFile, dic);

            myInstance.Completed = eventArgs =>
            {
                var activityResult = dic["activityResult"] as ActivityExcuteResult;

                currentLoginResult = activityResult;

                if (activityResult.ExcuteCode == ActivityResultCode.Success)
                {
                    if (SendEmailCompleted != null)
                    {
                        SendEmailCompleted(this, new SendEmailEventArgs
                        {
                            IsLogin = true,
                            Send = true,
                            Message = activityResult.Error_Message
                        });
                    }
                }
                else
                {
                    if (activityResult.Error_Message.Contains("由于连接方在一段时间后没有正确答复或连接的主机没有反应"))
                    {
                        SendEmail();
                    }
                    else
                    {
                        if (SendEmailCompleted != null)
                        {
                            SendEmailCompleted(this, new SendEmailEventArgs
                            {
                                IsLogin = true,
                                Send = false,
                                Message = activityResult.Error_Message
                            });
                        }
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

    public class SendEmailEventArgs : AccountLoginEventArgs
    {
        public SendEmailEventArgs()
        {
            Send = false;
        }
        public bool Send { get; set; }
    }
}
