using Fangbian.Common;
using Fangbian.Data.Client;
using Fangbian.DataStruct.Business;
using Fangbian.Log;
using Fangbian.Ticket.Server.AdvanceLogin;
using Fangbian.Tickets.Trains;
using Fangbian.Tickets.Trains.DataTransferObject.Request;
using Fangbian.Tickets.Trains.DataTransferObject.Request.PC;
using Fangbian.Tickets.Trains.DataTransferObject.Response.Login;
using Fangbian.Tickets.Trains.DataTransferObject.Response.PC;
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
    public class PCAnalysisPassenger : BaseAnalysisUser
    {
        public PCAnalysisPassenger()
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

            OutPutInfo(CurrentRegisterPassenger.Name + " " + CurrentRegisterPassenger.IdNo + ":初始化");

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
                    OutPutInfo(CurrentRegisterPassenger.Name + CurrentRegisterPassenger.IdNo + ":初始化成功");
                    CheckRegisterUserInfo();
                }
                else if (activityResult.ExcuteCode == ActivityResultCode.PCIpError)
                {
                    RollBackResource(activityResult);
                }
                else
                {
                    RollBackResource(activityResult);
                    //OutPutInfo(CurrentRegisterPassenger.IdNo + "验证是否可用初始化失败"+activityResult.Error_Message);
                    OutPutInfo(CurrentRegisterPassenger.Name+" " + CurrentRegisterPassenger.IdNo + ":初始化失败"+activityResult.Error_Message);

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
        

            var wfFile = ActivityXamlServices.Load(RegisterMain.Path + "PC检查信息.XAML");

            BulidRegisterInfo();

            if (string.IsNullOrEmpty(registerInfo.MobileNo))
            {
                IsPlatformPhone = false;

                registerInfo.MobileNo = ToolCommonMethod.GetPhoneSegment() + ToolCommonMethod.GetRandom(8);
            }

            var dic = new Dictionary<string, object>();

            BulidRegisterInfo();
            
            PCRegisterParam register = new PCRegisterParam()
            {
                UserName = registerInfo.UserName,
                password = registerInfo.PassWord,
                confirmPassWord = registerInfo.PassWord,
                Name = registerInfo.Name,
                id_type_code = "1",
                id_no = registerInfo.IdNo,
                email = registerInfo.Email,
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
            OutPutInfo(registerInfo.Name + " " + registerInfo.IdNo + ":开始检查注册信息");
            //OutPutInfo("开始检查注册信息【用户:"+registerInfo.UserName+"手机号:"+registerInfo.MobileNo+"邮件:"+registerInfo.Email+"身份证:"+registerInfo.Name+registerInfo.IdNo+"】");
            myInstance.Completed = eventArgs =>
            {
                var activityResult = eventArgs.Outputs["activityResult"] as ActivityExcuteResult;

                if (activityResult.ExcuteCode == ActivityResultCode.Success)
                {
                    var result = eventArgs.Outputs["check"] as PCValidResponse;

                    if (result.data.info_show.Equals("Y"))
                    {
                        if (ToolCommonMethod.GetChineseSpellCode(registerInfo.Name).Contains("1"))
                        {
                            Logger.Info("生僻身份证"+register.Name+" "+registerInfo.IdNo);
                            OutPutInfo(registerInfo.Name + registerInfo.IdNo + ":身份证信息可用,但是是生僻省份证");
                            WritePassengerState(3);
                        }
                        else
                        {
                            Logger.Info("可用身份证" + register.Name + " " + registerInfo.IdNo);
                            OutPutInfo(registerInfo.Name + registerInfo.IdNo + ":身份证信息可用");
                            WritePassengerState(0);
                        }
                        ExcuteRegisterEvent(new RegisterUserEventArgs { User = registerInfo, Success = true, Message = "成功" });
                    }
                    else
                    {
                        OutPutInfo(registerInfo.Name+ registerInfo.IdNo+":" + result.data.msg);
                        activityResult.Error_Message = result.data.msg;
                        CheckErrorOperation(activityResult);
                    }
                }
                else if (activityResult.ExcuteCode == ActivityResultCode.PCIpError)
                {
                    OutPutInfo(registerInfo.Name + " " + registerInfo.IdNo + ":检查注册信息失败" + activityResult.Error_Message);

                    CheckErrorOperation(activityResult);
                }
                else if (activityResult.Error_Message.Contains("网络可能存在问题，请您重试一下") && retryCount < 10)
                {
                    retryCount++;
                    OutPutInfo("检查注册信息失败:网络可能存在问题，请您重试一下");
                    Thread.Sleep(500);
                    CheckRegisterUserInfo();
                }
                else
                {
                    OutPutInfo(registerInfo.Name + " " + registerInfo.IdNo + ":检查注册信息失败" + activityResult.Error_Message);
                    CheckErrorOperation(activityResult);
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
