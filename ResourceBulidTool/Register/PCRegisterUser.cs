using AccountRegister;
using Fangbian.Tickets.Trains;
using Fangbian.Tickets.Trains.DataTransferObject.Request;
using Fangbian.Tickets.Trains.DataTransferObject.Request.PC;
using Fangbian.Tickets.Trains.DataTransferObject.Response.PC;
using Fangbian.Tickets.Trains.WFDataItem;
using FangBian.Common;
using MobileValidServer.PassCodeProvider;
using MyTool.Common;
using ResourceBulidTool.PassCodeProvider;
using System;
using System.Activities;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ResourceBulidTool.Register
{
    public class PCRegisterUser : BaseRegisterUser
    {
        PCRegisterParam register = null;

        /// <summary>
        /// 需要更换IP
        /// </summary>
        public event EventHandler IPChangeEvent;

        public override void Register(MyEntiry.T_Passenger item, MyEntiry.T_Email emailItem)
        {
            base.Register(item, emailItem);

            IsValidPassenger = false;

            Init();
        }
        int initCount = 0;
        protected override void Init()
        {
            LastOperationTime = DateTime.Now;

            OutPutInfo("开始初始化");
            var wfFile = ActivityXamlServices.Load(RegisterMain.Path + "PC初始化.XAML");

            var dic = new Dictionary<string, object>();

            currentSession = new RequestSession();

            dic.Add("requestSession", currentSession);

            var myInstance = new WorkflowApplication(wfFile, dic);

            myInstance.Completed = eventArgs =>
            {
                LastOperationTime = DateTime.Now;

                var activityResult = eventArgs.Outputs["activityResult"] as ActivityExcuteResult;

                if (activityResult.ExcuteCode == ActivityResultCode.Success)
                {
                    OutPutInfo("初始化成功");

                    CheckRegisterUserInfo();
                }
                else if (activityResult.ExcuteCode == ActivityResultCode.PCIpError)
                {
                    RollBackResource(activityResult);
                    //封IP了
                    if (IPChangeEvent != null) {
                        IPChangeEvent(this, EventArgs.Empty);
                    }
                }
                else if (activityResult.Error_Message.Contains("网络可能存在问题，请您重试一下") && initCount<=7)
                {
                    initCount++;
                    OutPutInfo("初始化失败:网络可能存在问题，请您重试一下");
                    Thread.Sleep(200);
                    Init();
                }
                else
                {
                    RollBackResource(activityResult);
                }
            };
            myInstance.Run();
        }

        int checkCount = 0;
        /// <summary>
        /// 检查注册信息
        /// </summary>
        protected override void CheckRegisterUserInfo()
        {
            initCount = 0;
            retryCount = 0;
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

            var wfFile = ActivityXamlServices.Load(RegisterMain.Path + "PC检查信息.XAML");

         

            if (string.IsNullOrEmpty(registerInfo.MobileNo))
            {
                IsPlatformPhone = false;

                registerInfo.MobileNo = ToolCommonMethod.GetPhoneSegment() + ToolCommonMethod.GetRandom(8);
            }

            var dic = new Dictionary<string, object>();

            register = new PCRegisterParam()
            {
                UserName = registerInfo.UserName,
                password = PassWordTo,
                confirmPassWord = PassWordTo,
                Name = CurrentRegisterPassenger.Name,
                id_type_code = "1",
                id_no = CurrentRegisterPassenger.IdNo,
                email = CurrentRegisterEmail.Email,
                country_code = "CN",
                born_date = ToolCommonMethod.GetBirthdayIDNoTo(CurrentRegisterPassenger.IdNo),
                mobile_no = registerInfo.MobileNo,
                enter_year = DateTime.Now.Year + string.Empty,
                passenger_type = "1",
                province_code = "11",
                school_name = "简码/汉字",
                school_system = "1",
                preference_from_station_name = "简码/汉字",
                preference_to_station_name = "简码/汉字"
            };

            dic.Add("requestSession", currentSession);

            dic.Add("register", register);

            var myInstance = new WorkflowApplication(wfFile, dic);

            OutPutInfo("开始检查注册信息【用户 手机号 邮件】");

            myInstance.Completed = eventArgs =>
            {
                var activityResult = eventArgs.Outputs["activityResult"] as ActivityExcuteResult;

                if (activityResult.ExcuteCode == ActivityResultCode.Success)
                {
                    retryCount = 0;

                    var result = eventArgs.Outputs["check"] as PCValidResponse;

                    if (result.data.info_show.Equals("Y"))
                    {
                        CheckSuccessOperation();
                    }
                    else
                    {
                        OutPutInfo("检查注册信息【用户 手机号 邮件】失败" + result.data.msg);
                        activityResult.Error_Message = result.data.msg;
                        CheckErrorOperation(activityResult);
                    }
                }
                else if (activityResult.ExcuteCode == ActivityResultCode.PCIpError)
                {
                    OutPutInfo("检查注册信息【用户 手机号 邮件】失败" + activityResult.Error_Message );

                    if (IPChangeEvent != null)
                    {
                        IPChangeEvent(this, EventArgs.Empty);
                    }
                    CheckErrorOperation(activityResult);
                }
                else if (activityResult.Error_Message.Contains("网络可能存在问题，请您重试一下") && checkCount<=7)
                {
                    checkCount++;
                    OutPutInfo("检查注册信息【用户 手机号 邮件】失败:网络可能存在问题，请您重试一下");
                    Thread.Sleep(200);
                    CheckRegisterUserInfo();
                }
                else
                {
                    OutPutInfo("检查注册信息【用户 手机号 邮件】失败" + activityResult.Error_Message);

                    CheckErrorOperation(activityResult);
                }
            };
            myInstance.Run();
        }

        int retryCount = 0;
        protected override void RegisterUserInfo(string passCode)
        {
            checkCount = 0;
            LastOperationTime = DateTime.Now;

            OutPutInfo("获取验证码成功,提交验证码");

            var wfFile = ActivityXamlServices.Load(RegisterMain.Path + "PC注册.XAML");

            var dic = new Dictionary<string, object>();

            currentSession = new RequestSession();

            dic.Add("requestSession", currentSession);

            dic.Add("register", register);

            dic.Add("passcode", passCode);

            var myInstance = new WorkflowApplication(wfFile, dic);
         
            myInstance.Completed = eventArgs =>
            {
                var activityResult = eventArgs.Outputs["activityResult"] as ActivityExcuteResult;

                OutPutInfo("提交验证码"+activityResult.ResponseSource);

                if (activityResult.ExcuteCode == ActivityResultCode.Success)
                {
                    var result = eventArgs.Outputs["check"] as PCValidResponse;

                    if (("Y").Equals(result.data.flag)
                        &&
                        register.email.Equals(result.data.email))
                    {
                        OutPutInfo("提交验证码成功,执行存储");

                        AddUser(0);

                        ExcuteRegisterEvent(new RegisterUserEventArgs { User = registerInfo, Success = true });

                        CurrentPassCodeProvider.AddBlackPhone(registerInfo.MobileNo, string.Empty);
                    }
                    else
                    {
                        Fangbian.Log.Logger.Error("提交验证码失败-" + "手机号:" + registerInfo.MobileNo + activityResult.ResponseSource);

                        activityResult.Error_Message = result.data.msg;

                        OutPutInfo("提交验证码失败:" + activityResult.Error_Message + "Response:" + activityResult.ResponseSource + " Data:" +
                            Newtonsoft.Json.JsonConvert.SerializeObject(register));

                        if (activityResult.Error_Message.Contains("您的短信验证码已失效，请重新获取")
                            || activityResult.Error_Message.Contains("很抱歉，您输入的验证码有误"))
                        {
                            (CurrentPassCodeProvider as CXPasscodeProvider).RetryPhone(registerInfo.MobileNo);
                            registerInfo.MobileNo = string.Empty;
                            CheckRegisterUserInfo();
                        }
                        else if (activityResult.Error_Message.Contains("验证码") && activityResult.Error_Message.Contains("误"))
                        {
                            (CurrentPassCodeProvider as CXPasscodeProvider).RetryPhone(registerInfo.MobileNo);
                            registerInfo.MobileNo = string.Empty;
                            CheckRegisterUserInfo();
                        } 
                        else
                        {
                            Fangbian.Log.Logger.Error("注册错误:"+ activityResult.Error_Message+Newtonsoft.Json.JsonConvert.SerializeObject(register));

                            (CurrentPassCodeProvider as CXPasscodeProvider).RetryPhone(registerInfo.MobileNo);

                            if (activityResult.Error_Message.Contains("您填写的身份信息重复"))
                            {
                               // RegisterUserInfo(passCode);
                                WritePassengerState(1); 
                            }
                            else
                            {
                                WritePassengerState(0);
                            }
                            if (activityResult.Error_Message.Contains("该邮箱已被注册"))
                            {
                                WriteEmailState(1);
                            }
                            else
                            {
                                WriteEmailState(0);
                            }

                            ExcuteRegisterEvent(new RegisterUserEventArgs { User = registerInfo, Success = false, Message = Newtonsoft.Json.JsonConvert.SerializeObject(result) });
                        }
                    }
                }
                else if (activityResult.ExcuteCode == ActivityResultCode.PCIpError)
                {
                    Fangbian.Log.Logger.Error("提交验证码失败" + activityResult.ResponseSource);

                    OutPutInfo("提交验证码失败:" + activityResult.Error_Message);
                    
                    if (IPChangeEvent != null)
                    {
                        IPChangeEvent(this, EventArgs.Empty);
                    }
                    ExcuteRegisterEvent(new RegisterUserEventArgs { User = registerInfo, Success = false, Message = activityResult.Error_Message });
                }
                else if (activityResult.Error_Message.Contains("网络可能存在问题，请您重试一下") && retryCount < 7)
                {
                    retryCount++;
                    OutPutInfo("提交验证码失败:网络可能存在问题，请您重试一下");
                    Thread.Sleep(700);
                    RegisterUserInfo(passCode);
                }
                else
                {
                    Fangbian.Log.Logger.Error("提交验证码失败:" + registerInfo.UserName + " " + registerInfo.PassWord + " "+activityResult.Error_Message);

                    OutPutInfo("提交验证码失败:" + activityResult.Error_Message);

                    if ((activityResult.Error_Message+string.Empty).Contains("该注册名已存在"))
                    {
                        retryCount = 0;
                        checkCount = 0;
                        CurrentPassCodeProvider.DisposePhone(registerInfo.MobileNo);
                        registerInfo.MobileNo = string.Empty;
                        CheckRegisterUserInfo();
                    }
                    else
                    {
                        ExcuteRegisterEvent(new RegisterUserEventArgs { User = registerInfo, Success = false, Message = activityResult.Error_Message });
                    }
                }
            };
            myInstance.Run();
        }
    }
}
