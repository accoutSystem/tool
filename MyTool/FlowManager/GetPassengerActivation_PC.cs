using Fangbian.Common;
using Fangbian.Data.Struct.Event;
using Fangbian.DataStruct.Business;
using Fangbian.DataStruct.Business.TrainTicket;
using Fangbian.Log;
using Fangbian.Tickets.Trains;
using Fangbian.Tickets.Trains.DataTransferObject.Response.Login;
using Fangbian.Tickets.Trains.DataTransferObject.Response.PC;
using Fangbian.Tickets.Trains.WFDataItem;
using Fangbian.WebTickets.Trains.DataTransferObject.Request.Passenger;
using Maticsoft.Model;
using MyTool.FlowManager;
using MyTool.Valid;
using System;
using System.Activities;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using YDMCSDemo;

namespace Fangbian.Ticket.Server.AdvanceLogin
{
    /// <summary>
    /// 激活账号(初始化和预先登录)
    /// </summary>
    public class GetPassengerActivation_PC
    {
        /// <summary>
        /// 账号预登录完成
        /// </summary>
        public event EventHandler<GetPassengerEventArgs> ReadPassengerCompleted;

        public Account12306Item CurrentUser { get; set; }

        public string CurrentMessage { get; set; }

        int sumbitCodeErrorCount = 0;
        int loginRetryCount = 0;

        public DateTime CurrentTime{ get; set; }

        public object Data { get; set; }

        private ActivityExcuteResult currentLoginResult { get; set; }

        public event EventHandler<ConsoleMessageEventArgs> OutputMessage;

        /// <summary>
        /// 激活当前账号
        /// </summary>
        /// <param name="user"></param>
        public void Read(Account12306Item user)
        {
            Message(user.UserName + " " + user.PassWord + "开始登录初始化");

            CurrentUser = user;
            Init();
        }

        RequestSession currentSession = null;

        private void Init()
        {
            loginRetryCount = 0;

            sumbitCodeErrorCount = 0;

            Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "开始初始化");

            currentSession = new RequestSession();

            var wfFile = ActivityXamlServices.Load(ToolCommon.Path + "PC初始化.XAML");

            var dic = new Dictionary<string, object>();

            dic.Add("requestSession", currentSession);

            var myInstance = new WorkflowApplication(wfFile, dic);

            myInstance.Completed = eventArgs =>
            {
                var activityResult = eventArgs.Outputs["activityResult"] as ActivityExcuteResult;

                if (activityResult.ExcuteCode == ActivityResultCode.Success)
                {
                    Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "初始化成功");
                    GetPassCode();
                }
                else if (activityResult.ExcuteCode == ActivityResultCode.PCIpError)
                {
                    Logger.Error("初始化PCIpError:" + activityResult.ResponseSource);
                    Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "初始化失败被封锁"  );
                    Init();
                }
                else
                {
                    Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "初始化失败:" + activityResult.Error_Message);
                    Init();
                }
            };
            myInstance.Run();
        }

        private void GetPassCode()
        {
            loginRetryCount = 0;
            sumbitCodeErrorCount = 0;
            Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "开始获取登录验证码");
            var wfFile = ActivityXamlServices.Load(ToolCommon.Path + "PC验证码.XAML");

            var dic = new Dictionary<string, object>();

            dic.Add("requestSession", currentSession);

            var myInstance = new WorkflowApplication(wfFile, dic);

            myInstance.Completed = eventArgs =>
            {
                var activityResult = eventArgs.Outputs["activityResult"] as ActivityExcuteResult;

                if (activityResult.ExcuteCode == ActivityResultCode.Success)
                {
                    Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "获取登录验证码成功,开始打码");

                    var form =(Form) ReadPassengerPage.Current;
                    if (form == null)
                    {
                        form = AnalysisPassengerInPC.Current;
                    }
                   

                    form.Invoke(new Action(() =>
                    {
                        var passCodeStr=eventArgs.Outputs["passCode"] + string.Empty;
                        try
                        {
                            Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "获取登录验证码成功,开始构建打码界面");
                            ReadPassCode code = new ReadPassCode();
                            code.ImageStr = passCodeStr;
                            code.Show();
                            code.ReadCodeCompleted += code_ReadCodeCompleted;
                        }
                        catch (Exception ex)
                        {
                            Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "获取登录验证码成功,构建打码界面失败:" + ex.Message +
                               passCodeStr);
                        }
                    }));
                }
                else if (activityResult.ExcuteCode == ActivityResultCode.PCIpError)
                {
                    Logger.Error("获取验证码PCIpError:" + activityResult.ResponseSource);
                    Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "获取登录验证码失败IP被封锁");
                    Thread.Sleep(700);
                    GetPassCode();
                }
                else if (activityResult.Error_Message.Contains("网络可能存在问题，请您重试一下"))
                {
                    Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "获取登录验证码失败,网络可能存在问题,请您重试一下");
                    Thread.Sleep(700);
                    GetPassCode();
                }
                else
                {
                    Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "获取登录验证码失败" + activityResult.Error_Message);
                    GetPassCode();
                }
            };
            myInstance.Run();
        }

        int id = 0;
        void code_ReadCodeCompleted(object sender, PassCodeEventArgs e)
        {

            id = e.ID;
            SumbitValidCode(e.Result, false); 
        }

        private void SumbitValidCode(string Result,bool isContinue)
        {
            Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "开始提交登录验证码,结果" + Result);

            var wfFile = ActivityXamlServices.Load(ToolCommon.Path + "PC识别验证码.XAML");

            var dic = new Dictionary<string, object>();

            dic.Add("requestSession", currentSession);

            dic.Add("point", Result);

            var myInstance = new WorkflowApplication(wfFile, dic);

            myInstance.Completed = eventArgs =>
            {
                var activityResult = eventArgs.Outputs["activityResult"] as ActivityExcuteResult;

                if (activityResult.ExcuteCode == ActivityResultCode.Success)
                {
                    var source = eventArgs.Outputs["check"] as PCValidResponse;
                    if (source.data.msg.Equals("TRUE") && source.data.result == "1")
                    {
                        Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "提交登录验证码识别成功");
                        YDMWrapper.YDM_Report(id, true);
                        Login(Result);
                    }
                    else
                    {
                        if (isContinue)
                        {
                            Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "验证码识别失败,继续提交一次确保");

                            SumbitValidCode(Result, false);
                        }
                        else
                        {
                            Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "验证码识别失败，重新获取验证码");

                            var ret = YDMWrapper.YDM_Report(id, false);
                            GetPassCode();
                        }
                    }
                }
                else if (activityResult.ExcuteCode == ActivityResultCode.PCIpError&&sumbitCodeErrorCount<10)
                {
                    Logger.Error("提交验证码PCIpError:" + activityResult.ResponseSource);
                    Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "提交登录验证码识别失败" + activityResult.ResponseSource);
                    sumbitCodeErrorCount++;
                    Thread.Sleep(700);
                    SumbitValidCode(Result, true);
                }
                else if (activityResult.Error_Message.Contains("网络可能存在问题，请您重试一下") && sumbitCodeErrorCount<10)
                {
                    Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "提交验证码识别失败,网络可能存在问题，请您重试一下");
                    sumbitCodeErrorCount++;
                    Thread.Sleep(700);
                    SumbitValidCode(Result, true);
                }
                else
                {
                    Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "提交登录验证码识别失败"+ activityResult.Error_Message);
                    var ret = YDMWrapper.YDM_Report(id, false);
                    Init();
                }
            };
            myInstance.Run();
        }

        private void Login(string Result)
        {
            Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "识别验证码成功,开始登录验证");

            var wfFile = ActivityXamlServices.Load(ToolCommon.Path + "PC登录验证.XAML");

            var dic = new Dictionary<string, object>();

            dic.Add("requestSession", currentSession);
            dic.Add("point", Result);
            dic.Add("pw", CurrentUser.PassWord);
            dic.Add("username", CurrentUser.UserName);

            var myInstance = new WorkflowApplication(wfFile, dic);

            myInstance.Completed = eventArgs =>
            {
                var activityResult = eventArgs.Outputs["activityResult"] as ActivityExcuteResult;

                if (activityResult.ExcuteCode == ActivityResultCode.Success)
                {
                    var source = eventArgs.Outputs["check"] as PCValidResponse;
                    if (source.data.loginCheck == "Y")
                    {
                        Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "登录验证成功");
                       QueryPassengersJson();
                    }
                    else
                    {
                        Logger.Fatal("登录错误:"+activityResult.ResponseSource);
                        Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "登录验证失败" + activityResult.ResponseSource);

                        var manager = activityResult.ResponseSource;

                        bool isBad = false;

                        if (manager.Contains("登录名不存在")
                         || manager.Contains("请核实您注册用户信息是否真实")
                         || manager.Contains("密码输入错误")
                         || manager.Contains("该用户已被暂停使用")
                         || manager.Contains("您的用户信息被他人冒用"))
                        {
                            isBad = true;
                        }

                        if (ReadPassengerCompleted != null)
                        {
                            ReadPassengerCompleted(this, new GetPassengerEventArgs
                            {
                                IsBadUser = isBad,
                                IsFormatError = false ,
                                IsLogin = false,
                                IsSystemError = false
                            });
                        }
                    }
                }
                else if (activityResult.ExcuteCode == ActivityResultCode.PCIpError  )
                {
                    Logger.Error("登录错误PCIpError:" + activityResult.ResponseSource);

                    Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "登录验证失败" + activityResult.ResponseSource);

                    if (activityResult.ResponseSource.Contains("系统繁忙")
                        && activityResult.ResponseSource.Contains("请稍后重试")
                        && loginRetryCount < 10)
                    {
                        Thread.Sleep(2000);
                        Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "登录失败系统繁忙,请稍后重试 正在重试"+loginRetryCount+"次");
                        loginRetryCount++;
                        Login(Result);
                    }
                    else 
                    {
                        Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "登录失败系统繁忙,请稍后重试 重试10次失败 恢复初始化");
                        Init();
                    } 
                }
                else if (activityResult.Error_Message.Contains("网络可能存在问题，请您重试一下")&&loginRetryCount<10)
                {
                    loginRetryCount++;
                    Thread.Sleep(700);
                    Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "登录验证失败,网络可能存在问题,请您重试一下");
                    Login(Result);
                }
                else
                {
                    Logger.Fatal("登录错误:" + activityResult.ResponseSource);
                    Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "登录验证失败" + activityResult.Error_Message);
                    Init();
                }
            };
            myInstance.Run();
        }

        private void QueryPassengersJson()
        {
            Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "开始查询联系人");

            var wfFile = ActivityXamlServices.Load(ToolCommon.Path + "PC获取联系人JSON.XAML");

            var dic = new Dictionary<string, object>();
            string pagesize = "30";
            string pageindex = "1";
            dic.Add("requestSession", currentSession);
            dic.Add("pageindex", pageindex);
            dic.Add("pagesize", pagesize);
            var myInstance = new WorkflowApplication(wfFile, dic);

            myInstance.Completed = eventArgs =>
            {
                var activityResult = eventArgs.Outputs["activityResult"] as ActivityExcuteResult;

                if (activityResult.ExcuteCode == ActivityResultCode.Success)
                {
                    var source = eventArgs.Outputs["passengers"] as PCPassengerResponse;

                    Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "查询联系人成功");
                    
                    if (ReadPassengerCompleted != null)
                    {
                        var acc = new AccountInfo() { User_name=this.CurrentUser.UserName };
                        
                        acc.CurrentUserPassengers = new List<PassengerItem>();

                        source.data.datas.ForEach(passenger =>
                        {
                            var item = new PassengerItem
                            {
                                IdNo = passenger.passenger_id_no,
                                IdType = passenger.passenger_id_type_code,
                                UserName = passenger.passenger_name,
                                TotalTimes = passenger.total_times
                            };
                            if (passenger.total_times == "99" || passenger.total_times == "95")
                            {
                                item.Status = 0;
                            }
                            else
                            {
                                item.Status = 1;
                            }
                            acc.CurrentUserPassengers.Add(item);
                        });

                        ReadPassengerCompleted(this, new GetPassengerEventArgs
                        {
                            IsBadUser = false,
                            IsLogin = true,
                            CurrentAccount = acc
                        });
                    }
                }
                else if (activityResult.ExcuteCode == ActivityResultCode.PCIpError)
                {
                    //98 未核验 94 未通过 99 通过

                    //Invoke(new Action(() =>
                    //{
                    //    textBox2.Text = Newtonsoft.Json.JsonConvert.SerializeObject(source);
                    //}));
                    //}
                    Logger.Error("查询联系人PCIpError:" + activityResult.ResponseSource);
                    Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "查询联系人失败" + activityResult.ResponseSource);
                    QueryPassengersJson();
                }
                else if (activityResult.Error_Message.Contains("网络可能存在问题，请您重试一下"))
                {
                    Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "查询联系人失败,网络可能存在问题,请您重试一下");
                    QueryPassengersJson();
                }
                else
                {
                    Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "查询联系人失败" + activityResult.Error_Message);
                    QueryPassengersJson();
                }
            };
            myInstance.Run();
        }

        private void Message(string message)
        {
            CurrentMessage = message;
            CurrentTime = DateTime.Now;
            if (OutputMessage != null)
            {
                OutputMessage(this, new ConsoleMessageEventArgs { Message = message });
            }
        }
    }
}
