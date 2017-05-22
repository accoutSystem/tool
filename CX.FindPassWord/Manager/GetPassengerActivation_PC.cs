using ChangePassWord;
using Fangbian.Tickets.Trains;
using Fangbian.Tickets.Trains.DataTransferObject.Response.Login;
using Fangbian.Tickets.Trains.DataTransferObject.Response.PC;
using Fangbian.Tickets.Trains.WFDataItem;
using Fangbian.WebTickets.Trains.DataTransferObject.Request.Passenger;
using MyTool.Valid;
using System;
using System.Activities;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

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
                    GetPassCode();
                    Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "初始化成功");
                }
                else if (activityResult.ExcuteCode == ActivityResultCode.PCIpError)
                {
                    Log.Logger.Error("初始化PCIpError:" + activityResult.ResponseSource);
                    Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "初始化失败被封锁"  );
                }
                else
                {
                    Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "初始化失败:" + activityResult.Error_Message);
                }
            };
            myInstance.Run();
        }

        private void GetPassCode()
        {
            Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "获取登录验证码");
            var wfFile = ActivityXamlServices.Load(ToolCommon.Path + "PC验证码.XAML");

            var dic = new Dictionary<string, object>();

            dic.Add("requestSession", currentSession);

            var myInstance = new WorkflowApplication(wfFile, dic);

            myInstance.Completed = eventArgs =>
            {
                var activityResult = eventArgs.Outputs["activityResult"] as ActivityExcuteResult;

                if (activityResult.ExcuteCode == ActivityResultCode.Success)
                {
                    Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "获取登录验证码成功");

                    var form = NewGetPassWord.Current;

                    form.Invoke(new Action(() =>
                    {
                        ReadPassCode code = new ReadPassCode();
                        code.ImageStr = eventArgs.Outputs["passCode"] + string.Empty;
                        code.Show();
                        code.ReadCodeCompleted += code_ReadCodeCompleted;
                    }));
                }
                else if (activityResult.ExcuteCode == ActivityResultCode.PCIpError)
                {
                    Log.Logger.Error("获取验证码PCIpError:" + activityResult.ResponseSource);
                    Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "获取登录验证码失败IP被封锁");
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
            SumbitValidCode(e.Result, true); 
        }

        private void SumbitValidCode(string Result,bool isContinue)
        {
            Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "提交登录验证码");

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
                        Login(Result);
                    }
                    else
                    {
                        Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "提交登录验证码识别失败,继续提交");

                        if (isContinue)
                        {
                            SumbitValidCode(Result, false);
                        }
                        else
                        {
                            GetPassCode();
                        }
                    }
                }
                else if (activityResult.ExcuteCode == ActivityResultCode.PCIpError)
                {
                    Log.Logger.Error("提交验证码PCIpError:" + activityResult.ResponseSource); 
                    Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "提交登录验证码识别失败,被封锁");
                }
                else
                {
                    Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "提交登录验证码识别失败"+ activityResult.Error_Message);
                    GetPassCode();
                }
            };
            myInstance.Run();
        }

        private void Login(string Result)
        {
            Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "登录验证");

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
                        Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "登录成功");
                       QueryPassengersJson();
                    }
                    else
                    {
                        Log.Logger.Fatal("登录错误:"+activityResult.ResponseSource);
                        Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "登录失败" + activityResult.ResponseSource);

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
                else if (activityResult.ExcuteCode == ActivityResultCode.PCIpError)
                {
                    Log.Logger.Error("登录错误PCIpError:" + activityResult.ResponseSource);
                    Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "登录失败被封锁");
                }
                else
                {
                    Log.Logger.Fatal("登录错误:" + activityResult.ResponseSource);
                    Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "登录失败"   +activityResult.Error_Message);
                    Init();
                }
            };
            myInstance.Run();
        }

        private void QueryPassengersJson()
        {
            Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "查询联系人");

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

                    //98 未核验 94 未通过 99 通过

                    //Invoke(new Action(() =>
                    //{
                    //    textBox2.Text = Newtonsoft.Json.JsonConvert.SerializeObject(source);
                    //}));
                //}
                else if (activityResult.ExcuteCode == ActivityResultCode.PCIpError)
                {
                    Log.Logger.Error("查询联系人PCIpError:" + activityResult.ResponseSource);
                    Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "查询联系人被封锁");
                }
                else
                {
                    Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "获取联系人失败" + activityResult.Error_Message);
                }
            };
            myInstance.Run();
        }

        private void Message(string message)
        {
            Console.WriteLine(message);
            CurrentMessage = message;
            CurrentTime = DateTime.Now;
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

        public bool IsYQ { get; set; }
        /// <summary>
        /// 是否格式错误
        /// </summary>
        public bool IsFormatError { get; set; }
        /// <summary>
        /// 是否是损坏用户
        /// </summary>
        public bool IsBadUser { get; set; }

        public AccountInfo CurrentAccount { get; set; }
    }
    public class ConsoleMessageEventArgs : EventArgs
    {
        public ConsoleMessageEventArgs() { }

        public string DataItem { get; set; }
        public string Message { get; set; }
    }

    public class ToolCommon
    {
        public static string Path = Environment.CurrentDirectory + @"\Flow\";
    }
}
