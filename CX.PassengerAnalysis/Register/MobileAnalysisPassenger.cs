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
using MyEntiry;
using MyTool.Common;
using Newtonsoft.Json.Linq;
using PD.Business;
using ResourceBulidTool;
using ResourceBulidTool.LoginPool;
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
    public class MobileAnalysisPassenger : BaseAnalysisUser
    {
        public MobileAnalysisPassenger()
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

            BulidRegisterInfo();

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
                    Logger.Info(registerInfo.MobileNo+"未使用，可以用来注册");
                    OutPutInfo("该身份证可用");
                    WritePassengerState(0);
                    ExcuteRegisterEvent(new RegisterUserEventArgs { User = registerInfo, Success = true, Message = "成功" });
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
        }
    }
}
