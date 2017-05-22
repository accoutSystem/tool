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
    public class GetPassengerActivation
    {
        /// <summary>
        /// 账号预登录完成
        /// </summary>
        public event EventHandler<GetPassengerEventArgs> ReadPassengerCompleted;

        public Account12306Item CurrentUser { get; set; }

        public object Data { get; set; }

        private ActivityExcuteResult currentLoginResult { get; set; }

        public event EventHandler<ConsoleMessageEventArgs> OutputMessage;


        public GetPassengerActivation()
        {
           
        }

        /// <summary>
        /// 激活当前账号
        /// </summary>
        /// <param name="user"></param>
        public void Read(Account12306Item user)
        {
            Message(user.UserName + " " + user.PassWord + "开始登录");

            CurrentUser = user;

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
                    requestSession.CurrentAccount = eventArgs.Outputs["accountInfo"] as AccountInfo;

                    if (ReadPassengerCompleted != null)
                    {
                        ReadPassengerCompleted(this, new GetPassengerEventArgs
                        {
                            IsBadUser = false,
                            IsLogin = true,
                            CurrentAccount = requestSession.CurrentAccount
                        });
                    }
                }
                else
                {
                    Message(user.UserName + " " + user.PassWord + "登录完成,失败" + activityResult.Error_Message);

                    Log.Logger.Error("主服务程序预登录错误:" + activityResult.Error_Message);

                    var manager = activityResult.Error_Message;

                    bool isBad = false;

                    if (manager.Contains("登录名不存在")
                     || manager.Contains("请核实您注册用户信息是否真实")
                     || manager.Contains("密码输入错误")
                     || manager.Contains("该用户已被暂停使用")
                     || manager.Contains("您的用户信息被他人冒用"))
                    {
                        isBad = true;
                    }

                    bool isSystemError = activityResult.Error_Message.Contains("system error") ||
                       activityResult.Error_Message.Contains("非法请求");

                    bool isFormatError=activityResult.Error_Message.Contains("After parsing a value an unexpected character was encountered");

                    if (ReadPassengerCompleted != null)
                    {
                        ReadPassengerCompleted(this, new GetPassengerEventArgs
                        {
                            IsBadUser = isBad,
                            IsFormatError = isFormatError,
                            IsLogin = false,
                            IsSystemError = isSystemError
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

    public class GetPassengerEventArgs : EventArgs
    {
        public GetPassengerEventArgs()
        {
            IsLogin = false;
            IsBadUser = false;
            IsYQ = false;
            IsTicket = false;
            IsDelete = false;
            IsFormatError = false;
            IsSystemError = false;
        }

        public string Message { get; set; }

        /// <summary>
        /// 是否登录成功
        /// </summary>
        public bool IsLogin { get; set; }
        /// <summary>
        /// 是否是系统错误
        /// </summary>
        public bool IsSystemError { get; set; }

        /// <summary>
        /// 是否需要在指定的某天才能删除
        /// </summary>
        public bool IsYQ { get; set; }

        /// <summary>
        /// 是否存在票
        /// </summary>
        public bool IsTicket { get; set; }

        /// <summary>
        /// 是否格式错误
        /// </summary>
        public bool IsFormatError{ get; set; }
        /// <summary>
        /// 是否是损坏用户
        /// </summary>
        public bool IsBadUser { get; set; }

        /// <summary>
        /// 删除联系人完成
        /// </summary>
        public bool IsDelete { get; set; }

        public AccountInfo CurrentAccount { get; set; }
    }
}
