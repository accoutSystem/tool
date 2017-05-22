using Fangbian.Common;
using Fangbian.Data.Client;
using Fangbian.DataStruct.Business;
using Fangbian.Log;
using Fangbian.Ticket.Server.AdvanceLogin;
using Fangbian.Tickets.Trains;
using Fangbian.Tickets.Trains.DataTransferObject.Request;
using Fangbian.Tickets.Trains.DataTransferObject.Response.Login;
using Fangbian.Tickets.Trains.WFDataItem;
using FangBian.Common;
using Maticsoft.Model;
using MobileValidServer.PassCodeProvider;
using MyEntiry;
using MyTool.Common;
using Newtonsoft.Json.Linq;
using PD.Business;
using ResourceBulidTool;
using ResourceBulidTool.LoginPool;
using ResourceBulidTool.PassCodeProvider;
using System;
using System.Activities;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

//先动态生成一个手机号码 如果该手机号码注册了 在生成一个 直到没有注册
//在核验身份信息 如果身份信息被注册 重新开始一个任务
//身份信息没有被注册 从平台拉号码 注册 直到注册成功
namespace AccountRegister
{
    public class MobileRegisterUser : BaseRegisterUser
    {
        public MobileRegisterUser()
        {
            IsActivity = true;
        }

        private int retryCount = 0;

        private bool registerError = false;

        private bool IsNext = true;

        public override void Register(T_Passenger item, T_Email emailItem)
        {
            base.Register(item, emailItem);

            IsValidPassenger = false;

            Init();
        }

        /// <summary>
        /// 初始化设备
        /// </summary>
        protected override void Init()
        {
            LastOperationTime = DateTime.Now;

            OutPutInfo("开始初始化");

            currentSession = new RequestSession() { OrderId = Guid.NewGuid().ToString() };

            var wfFile = ActivityXamlServices.Load(RegisterMain.Path + "初始化.XAML");

            var dic = new Dictionary<string, object>();

            dic.Add("requestSession", currentSession);

            dic.Add("activityResult", new ActivityExcuteResult { ExcuteCode = ActivityResultCode.Success });

            var myInstance = new WorkflowApplication(wfFile, dic);

            myInstance.Completed = eventArgs =>
            {
                LastOperationTime = DateTime.Now;

                var requestSession = dic["requestSession"];

                var activityResult = dic["activityResult"] as ActivityExcuteResult;

                if (activityResult.ExcuteCode == ActivityResultCode.Success && IsNext)
                {
                    OutPutInfo("初始化成功");

                    CheckRegisterUserInfo();
                }
                else
                {
                    RollBackResource(activityResult);
                }
            };
            myInstance.Run();
        }

        /// <summary>
        /// 开始提交注册信息
        /// </summary>
        protected override void CheckRegisterUserInfo()
        {
            BulidRegisterInfo();

            if (!IsActivity)
            {
                OutPutInfo("进程更新，强制终止任务" + Guid.NewGuid().ToString());

                ExcuteRegisterEvent(new RegisterUserEventArgs
                {
                    User = registerInfo,
                    Success = false,
                    Message = string.Empty
                });

                return;
            }

            TaskRegisterCount++;

            LastOperationTime = DateTime.Now;

            OutPutInfo("检查信息是否可用");

            var wfFile = ActivityXamlServices.Load(RegisterMain.Path + "检查注册.XAML");

            var dic = new Dictionary<string, object>();

            dic.Add("requestSession", currentSession);

          

            if (string.IsNullOrEmpty(registerInfo.MobileNo))
            {
                IsPlatformPhone = false;
                registerInfo.MobileNo = ToolCommonMethod.GetPhoneSegment() + ToolCommonMethod.GetRandom(8);//用一个已经注册的手机号来分析身份是否合法
            }
            dic.Add("newUser", registerInfo);

            dic.Add("activityResult", new ActivityExcuteResult { ExcuteCode = ActivityResultCode.Success });

            var myInstance = new WorkflowApplication(wfFile, dic);

            myInstance.Completed = eventArgs =>
            {
                LastOperationTime = DateTime.Now;

                var requestSession = dic["requestSession"];

                var activityResult = dic["activityResult"] as ActivityExcuteResult;

                if (activityResult.ExcuteCode == ActivityResultCode.Success)
                {
                    Logger.Info(registerInfo.MobileNo+"未使用，可以用来注册"  );
                    CheckSuccessOperation();
                }
                else
                {
                    OutPutInfo("检查信息失败" + activityResult.Error_Message);
                    Logger.Info(registerInfo.MobileNo + activityResult.Error_Message);
                    if (!activityResult.Error_Message.Contains("该证件号码已被注册") &&
                        !activityResult.Error_Message.Contains("该邮箱已被注册") &&
                        !activityResult.Error_Message.Contains("该手机号码已被注册") &&
                        !activityResult.Error_Message.Contains("该注册名已存在") &&
                        !activityResult.Error_Message.Contains("您输入的手机号码已被其他注册用户") &&
                        !activityResult.Error_Message.Contains("用户名格式错误") &&//不是省份证注册
                        !activityResult.Error_Message.Contains("您获取验证码短信次数过多") &&
                        retryCount < 5)
                    {
                        if (activityResult.Error_Message.ToLower().Contains("string index out of") ||
                            activityResult.Error_Message.ToLower().Contains("null"))
                        {
                            string request = activityResult.RequestSource;
                        }
                        retryCount++;
                        CheckRegisterUserInfo();

                    }
                    else
                    {
                        CheckErrorOperation(activityResult);
                    }
                }
            };
            myInstance.Run();
        }

        /// <summary>
        /// start regisger user
        /// </summary>
        protected override void RegisterUserInfo(string passCode)
        {
            LastOperationTime = DateTime.Now;

            OutPutInfo("获取验证码成功,提交验证码");
            Logger.Info(registerInfo.MobileNo + "开始提交验证码:" + passCode);
            var wfFile = ActivityXamlServices.Load(RegisterMain.Path + "注册.XAML");

            var dic = new Dictionary<string, object>();

            dic.Add("requestSession", currentSession);

            dic.Add("newUser", registerInfo);

            dic.Add("passCode", passCode);

            dic.Add("activityResult", new ActivityExcuteResult { ExcuteCode = ActivityResultCode.Success });

            ExcuteMessage(" start register user " + registerInfo.UserName + "  Phone:" + registerInfo.MobileNo);

            var myInstance = new WorkflowApplication(wfFile, dic);

            myInstance.Completed = eventArgs =>
            {
                LastOperationTime = DateTime.Now;

                var requestSession = dic["requestSession"];

                var activityResult = dic["activityResult"] as ActivityExcuteResult;

                if (activityResult.ExcuteCode == ActivityResultCode.Success)
                {
                    OutPutInfo("提交验证码成功,执行存储");

                    Logger.Info(registerInfo.MobileNo + "提交验证码" + passCode + "成功,执行存储");

                    if (registerError)
                    {
                        registerError = false;
                        Logger.Warn(registerInfo.UserName + " " + PassWordTo + "注册遇到加解密失败string index out of后重试成功");
                    }

                    AddUser(0);

                    ExcuteRegisterEvent(new RegisterUserEventArgs { User = registerInfo, Success = true });

                    //CurrentPassCodeProvider.AddBlackPhone(registerInfo.MobileNo, string.Empty);
                }
                else
                {
                    Logger.Info(registerInfo.MobileNo + "提交验证码失败" + activityResult.Error_Message);
                    if (activityResult.Error_Message.ToLower().Contains("string index out of") ||
                        activityResult.Error_Message.ToLower().Contains("system error") ||
                            activityResult.Error_Message.ToLower().Contains("null") ||
                              activityResult.Error_Message.Contains("非法请求") ||
                       activityResult.Error_Message.Contains("SyntaxError: String contains control character"))
                    {
                        registerError = true;
                        Logger.Warn(registerInfo.UserName + " " + PassWordTo + "注册遇到加解密失败后进行重试" + activityResult.Error_Message);
                        RegisterUserInfo(passCode);
                        return;
                    }

                    Logger.Debug(" register user error:" + registerInfo.UserName + " " + PassWordTo + " " + activityResult.Error_Message + " 验证码:" + passCode + " 手机:" + registerInfo.MobileNo);
                    //很抱歉，您输入的验证码有误 请先获取验证码
                    //AddUser(1);
                    if (activityResult.Error_Message.Contains("由于连接方在一段时间后没有正确答复或连接的主机没有反应"))
                    {
                        Logger.Debug("重新提交" + registerInfo.UserName + " " + PassWordTo + " 验证码 " + passCode);

                        RegisterUserInfo(passCode);
                    } 
                    else if (activityResult.Error_Message.Contains("您的短信验证码已失效，请重新获取"))
                    {
                        CheckRegisterUserInfo();
                    }
                    else if ((activityResult.Error_Message.Contains("验证码") && activityResult.Error_Message.Contains("误"))
                            || activityResult.Error_Message.Contains("请先获取验证码"))
                    {
                        //释放手机号
                        (CurrentPassCodeProvider as CXPasscodeProvider).RetryPhone(registerInfo.MobileNo);
                        registerInfo.MobileNo = string.Empty;
                        CheckRegisterUserInfo();
                    }
                    else
                    {
                        retryCount++;

                        if (activityResult.Error_Message.Contains("抱歉，您填写的身份信息重复"))
                        {
                            WritePassengerState(1);
                        }
                        else
                        {
                            WritePassengerState(0);
                        }

                        WriteEmailState(0);

                        OutPutInfo("提交验证码失败" + activityResult.Error_Message);

                        ExcuteRegisterEvent(new RegisterUserEventArgs { User = registerInfo, Success = false, Message = activityResult.Error_Message });
                    }
                }
            };
            myInstance.Run();
        }
     
    }
}
