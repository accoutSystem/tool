using ChangePassWord.PassCode;
using Fangbian.DataStruct.Business;
using Fangbian.Log;
using Fangbian.Ticket.Server.AdvanceLogin;
using Fangbian.Tickets.Trains;
using Fangbian.Tickets.Trains.DataTransferObject;
using Fangbian.Tickets.Trains.DataTransferObject.Response.ChangeUser;
using Fangbian.Tickets.Trains.DataTransferObject.Response.Login;
using Fangbian.Tickets.Trains.WFDataItem;
using FangBian.Common;
using Maticsoft.Model;
using System;
using System.Activities;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyTool.FlowManager
{
   
    /// <summary>
    /// 激活账号(初始化和预先登录)
    /// </summary>
    public class UpdateUserActivation
    {
        /// <summary>
        /// 账号预登录完成
        /// </summary>
        public event EventHandler<UpdateUserEventArgs> UpdateUserCompleted;

        public RequestSession CurrentSession{get;set;}

        public Account12306Item currentUser { get; set; }

        public InitUserResponse UserInfo { get; set; }

        public object Data { get; set; }

        private ActivityExcuteResult currentLoginResult { get; set; }

        public event EventHandler<ConsoleMessageEventArgs> OutputMessage;

        /// <summary>
        /// 对账号乘客和订单进行检查
        /// </summary>
        public bool CheckAccount { get; set; }

        public UpdateUserActivation()
        {
            CheckAccount = true;
        }

        /// <summary>
        /// 激活当前账号
        /// </summary>
        /// <param name="user"></param>
        public void StartUpdateUser(Account12306Item user)
        {
            Message(user.UserName + "开始登录");

            currentUser = user;

            CurrentSession = new RequestSession() { UserName = user.UserName, UserPassWord = user.PassWord };

            var wfFile = ActivityXamlServices.Load(ToolCommon.Path + "用户登录初始化.XAML");

            var dic = new Dictionary<string, object>();

            dic.Add("userAccount", user);

            dic.Add("requestSession", CurrentSession);

            dic.Add("activityResult", new ActivityExcuteResult { ExcuteCode = ActivityResultCode.Success });

            var myInstance = new WorkflowApplication(wfFile, dic);

            myInstance.Completed = eventArgs =>
            {
                var requestSession = eventArgs.Outputs.ContainsKey("requestSession") ? eventArgs.Outputs["requestSession"] as RequestSession : null;

                var activityResult = dic["activityResult"] as ActivityExcuteResult;

                currentLoginResult = activityResult;

                if (activityResult.ExcuteCode == ActivityResultCode.Success)
                {
                    Logger.Info("主服务程序预登录成功:" + user.UserName);
                    Message(user.UserName + "登录成果");
                    GetUserInfo();
                }
                else
                {
                    string message=user.UserName + "登录失败" + activityResult.Error_Message;

                    Excute(message,false);

                    Message(message);

                    Logger.Error("主服务程序预登录错误:" + activityResult.Error_Message);
                }
            };
            myInstance.Run();
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        private void GetUserInfo()
        {
            var wfFile = ActivityXamlServices.Load(ToolCommon.Path + "获取用户信息.XAML");

            var dic = new Dictionary<string, object>();

            dic.Add("requestSession", CurrentSession);

            dic.Add("activityResult", new ActivityExcuteResult { ExcuteCode = ActivityResultCode.Success });

            var myInstance = new WorkflowApplication(wfFile, dic);

            myInstance.Completed = eventArgs =>
            {
                var requestSession = eventArgs.Outputs.ContainsKey("requestSession") ? eventArgs.Outputs["requestSession"] as RequestSession : null;

                var activityResult = dic["activityResult"] as ActivityExcuteResult;

                currentLoginResult = activityResult;

                if (activityResult.ExcuteCode == ActivityResultCode.Success)
                {
                    Message(currentUser.UserName + "获取用户信息成功");

                    UserInfo = eventArgs.Outputs["userResult"] as InitUserResponse;

                    if (UserInfo.born_date.Equals("19800101") && UserInfo.sex_code.Equals("M"))
                    {
                        GetPassCode();
                    }
                    else
                    {
                        Message(currentUser.UserName + "已经修改成功，无需再修改");
                        Excute("已经修改成功，无需再修改", true);
                    }
                }
                else
                {
                    string message = currentUser.UserName + "获取用户信息失败" + activityResult.Error_Message;

                    Excute(message, false);

                    Message(message);
                }
            };
            myInstance.Run();
        }

        /// <summary>
        /// 获取和识别验证码
        /// </summary>
        public void GetPassCode() 
        { 
 
            var wfFile = ActivityXamlServices.Load(ToolCommon.Path + "获取验证码.XAML");

            var dic = new Dictionary<string, object>();

            dic.Add("requestSession", CurrentSession);

            dic.Add("activityResult", new ActivityExcuteResult { ExcuteCode = ActivityResultCode.Success });

            var myInstance = new WorkflowApplication(wfFile, dic);

            myInstance.Completed = eventArgs =>
            {
                var requestSession = eventArgs.Outputs.ContainsKey("requestSession") ? eventArgs.Outputs["requestSession"] as RequestSession : null;

                var activityResult = dic["activityResult"] as ActivityExcuteResult;

                currentLoginResult = activityResult;

                if (activityResult.ExcuteCode == ActivityResultCode.Success)
                {
                    Message(currentUser.UserName + "获取验证码成功");

                    var passCodeResult = eventArgs.Outputs["passCode"] as string;

                    string passCode = ReadPassCode(passCodeResult);

                    ChangeUserInfo(passCode);
                }
                else
                {
                    string message = currentUser.UserName + "获取验证码失败" + activityResult.Error_Message;

                    Excute(message, false);
                    Message(message);
                }
            };
            myInstance.Run();
        }

        /// <summary>
        /// 修改用户信息
        /// </summary>
        public void ChangeUserInfo(string passCode) 
        {
            this.UserInfo.born_date = GetBornDate(UserInfo.id_no);
            this.UserInfo.sex_code = GetSex(UserInfo.id_no);
          
            var wfFile = ActivityXamlServices.Load(ToolCommon.Path + "修改用户信息.XAML");

            var dic = new Dictionary<string, object>();

            dic.Add("requestSession", CurrentSession);

            dic.Add("initUser", UserInfo);

            dic.Add("passCode", passCode);

            dic.Add("passWord", Md5Helper.Md5(CurrentSession.UserPassWord));

            dic.Add("activityResult", new ActivityExcuteResult { ExcuteCode = ActivityResultCode.Success });

            var myInstance = new WorkflowApplication(wfFile, dic);

            myInstance.Completed = eventArgs =>
            {
                var requestSession = eventArgs.Outputs.ContainsKey("requestSession") ? eventArgs.Outputs["requestSession"] as RequestSession : null;

                var activityResult = dic["activityResult"] as ActivityExcuteResult;

                currentLoginResult = activityResult;

                if (activityResult.ExcuteCode == ActivityResultCode.Success)
                {
                    Message(currentUser.UserName + "修改用户信息成功");
                    var changeResult = eventArgs.Outputs["changeResult"] as BusinessBaseResponse;
                    Excute("修改用户信息成功", true);
                }
                else
                {
                    string message = currentUser.UserName + "修改用户信息失败" + activityResult.Error_Message;
                 
                    Message(message);

                    if (activityResult.Error_Message.Contains("验证码不正确"))
                    {
                        GetPassCode();
                    }
                    else
                    {
                        Excute(message, false);
                    }
                }
            };
            myInstance.Run();
        }

        private static string GetBornDate(string identityCard)
        {
            try
            {
                if (identityCard.Length == 18)//处理18位的身份证号码从号码中得到生日和性别代码
                {
                    return identityCard.Substring(6, 4) + identityCard.Substring(10, 2) + identityCard.Substring(12, 2);
                }
                if (identityCard.Length == 15)
                {
                    return "19" + identityCard.Substring(6, 2) + identityCard.Substring(8, 2) + identityCard.Substring(10, 2);
                }
            }
            catch { }
            return "19800101";
        }

        private static string GetSex(string identityCard)
        {
            try
            {
                string sex = string.Empty;
                if (identityCard.Length == 18)//处理18位的身份证号码从号码中得到生日和性别代码
                {
                    sex = identityCard.Substring(14, 3);
                }
                if (identityCard.Length == 15)
                {
                    sex = identityCard.Substring(12, 3);
                }

                if (int.Parse(sex) % 2 == 0)//性别代码为偶数是女性奇数为男性
                {
                    return "F";
                }
                else
                {
                    return "M";
                }
            }
            catch { }
            return "M";
        }

        private string ReadPassCode(string base64PassCode) {
            return new AnalysePassCode().GetCheckCode(base64PassCode);
        }

        private void Message(string message)
        {
            Console.WriteLine(message);
            if (OutputMessage != null)
            {
                OutputMessage(this, new ConsoleMessageEventArgs { Message = message });
            }
        }

        private void Excute(string message, bool success) {
            if (UpdateUserCompleted != null) {
                UpdateUserCompleted(this, new UpdateUserEventArgs { Message=message,UpdateUserSeccess=success });
            }
        }
    }

    public class UpdateUserEventArgs : EventArgs
    {
        public UpdateUserEventArgs()
        {
            UpdateUserSeccess = false;
        }
        public bool UpdateUserSeccess { get; set; }
        public string Message { get; set; }
    }
}
