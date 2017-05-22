using Fangbian.Common;
using Fangbian.Data.Struct.Event;
using Fangbian.DataStruct.Business;
using Fangbian.DataStruct.Business.TrainTicket;
using Fangbian.Tickets.Trains;
using Fangbian.Tickets.Trains.DataTransferObject.Request.PC;
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
using YDMCSDemo;

namespace Fangbian.Ticket.Server.AdvanceLogin
{
    /// <summary>
    /// 激活账号(初始化和预先登录)
    /// </summary>
    public class DeletePassengerActivation_PC
    {
        /// <summary>
        /// 账号预登录完成
        /// </summary>
        public event EventHandler<GetPassengerEventArgs> DeletePassengerCompleted;

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
        public void Delete(Account12306Item user)
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
                    DeletePassengerInPC.Current.Invoke(new Action(() =>
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
        /// <summary>
        /// 提交验证码
        /// </summary>
        /// <param name="Result"></param>
        /// <param name="isContinue"></param>

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
                        YDMWrapper.YDM_Report(id, true);
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
                            var ret = YDMWrapper.YDM_Report(id, false);
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
                    var ret = YDMWrapper.YDM_Report(id, false);
                    GetPassCode();
                }
            };
            myInstance.Run();
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="Result"></param>
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
                        QueryNoCompleted();
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

                        if (DeletePassengerCompleted != null)
                        {
                            DeletePassengerCompleted(this, new GetPassengerEventArgs
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
        /// <summary>
        /// 查询乘车人
        /// </summary>
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

                    Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "查询联系人成功,共"+source.data.datas.Count+"个联系人");
                    
                    if (DeletePassengerCompleted != null)
                    {
                       var manPas= source.data.datas.FirstOrDefault(item => (item.isUserSelf + "").ToUpper().Equals("Y"));

                       if (manPas != null && manPas.total_times != "99" && manPas.total_times != "95")
                       {
                           DeletePassengerCompleted(this, new GetPassengerEventArgs
                           {
                               IsBadUser = true,
                               IsLogin = false
                           });
                       }
                       else
                       { 
                           if (source.data.datas.Count > 1)
                           {
                               this.DeletePassenger(source);
                           }
                           else
                           {
                               if (source.data.datas.Count <= 1)
                               {
                                   DeletePassengerCompleted(this, new GetPassengerEventArgs
                                   { 
                                       IsLogin = true, IsDelete=true
                                   });
                               }
                               else
                               {
                                   if (isDelete)
                                   {
                                       QueryPassengersJson();
                                   }
                               }
                           }
                       }
                    }
                }
                else if (activityResult.ExcuteCode == ActivityResultCode.PCIpError)
                {
                    Log.Logger.Error("查询联系人PCIpError:" + activityResult.ResponseSource);
                    Thread.Sleep(1000);
                    Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "查询联系人被封锁");
                    QueryPassengersJson();
                }
                else
                {
                    Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "获取联系人失败" + activityResult.Error_Message);
                    Thread.Sleep(1000);
                    QueryPassengersJson();
                }
            };
            myInstance.Run();
        }

        bool isDelete = false;
        private void DeletePassenger(PCPassengerResponse passenger)
        {
            if (passenger == null)
                return;

            var wfFile = ActivityXamlServices.Load(ToolCommon.Path  +"PC删除联系人.XAML");

            var dic = new Dictionary<string, object>();

            List<PCPassengerItem> deletep = new List<PCPassengerItem>();

            foreach (var item in passenger.data.datas)
            {
                if (item.isUserSelf != "N")
                {
                    continue;
                }
                deletep.Add(item);
            }
            Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "开始删除联系人,可以删除" + deletep.Count + "个联系人");

            dic.Add("requestSession", currentSession);

            dic.Add("deletePassenger", deletep);

            var myInstance = new WorkflowApplication(wfFile, dic);

            myInstance.Completed = eventArgs =>
            {
                var activityResult = eventArgs.Outputs["activityResult"] as ActivityExcuteResult;

                if (activityResult.ExcuteCode == ActivityResultCode.Success)
                {
                    isDelete = true;
                    Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "删除联系人成功");
                  
                    Thread.Sleep(2000);

                    QueryPassengersJson();
                }
                else if (activityResult.ExcuteCode == ActivityResultCode.PCIpError)
                {
                    Log.Logger.Error("删除联系人PCIpError:" + activityResult.ResponseSource);
                    Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "删除联系人被封锁");
                    DeletePassengerCompleted(this, new GetPassengerEventArgs
                    { 
                        IsLogin = true,
                        IsSystemError = true,
                    });
                }
                else
                {
                    Log.Logger.Error("删除联系人失败:" + activityResult.ResponseSource + activityResult.Error_Message);
                    var result = new GetPassengerEventArgs
                    { 
                        IsLogin = true,
                        IsSystemError = true
                    };
                    Message(CurrentUser.UserName + " " + CurrentUser.PassWord + "删除联系人失败" + activityResult.Error_Message);
                    string msg = activityResult.Error_Message;
                    if (msg.Contains("前不允许删除"))
                    {
                        result.IsYQ = true;
                        result.IsSystemError = false;
                    }
                    foreach (var item in passenger.data.datas)
                    {
                        msg = msg.Replace(item.passenger_name, "");
                    }
                    msg = msg.Replace("前不允许删除!", "");
                    result.Message = msg;
                    DeletePassengerCompleted(this, result);
                }
            };
            myInstance.Run();
        }

        private void QueryNoCompleted( )
        {
            var wfFile = ActivityXamlServices.Load(ToolCommon.Path   +"PC查询未完成订单.XAML");

            var dic = new Dictionary<string, object>();

            dic.Add("requestSession", currentSession);

            var myInstance = new WorkflowApplication(wfFile, dic);

            myInstance.Completed = eventArgs =>
            {
                var activityResult = eventArgs.Outputs["activityResult"] as ActivityExcuteResult;

                if (activityResult.ExcuteCode == ActivityResultCode.Success)
                {
                    var source = eventArgs.Outputs["order"] as PCNoCompleteOrderResponse;
                    if (source.status)
                    {
                        if (source.OrderInfo != null && source.OrderInfo.Orders != null)
                        {
                            Message("存在未完成订单近期:"+CurrentUser.UserName+" "+CurrentUser.PassWord);
                            if (DeletePassengerCompleted != null)
                            {
                                DeletePassengerCompleted(this, new GetPassengerEventArgs
                                { 
                                    IsLogin = true, IsTicket=true
                                });
                            }
                        } 
                        else
                        { 
                            Message("没有未完成的订单");
                            QueryHistoryOrder();
                        }
                    }
                }
                else
                {
                    Message("失败:" + activityResult.Error_Message);
                    Thread.Sleep(1000);
                    QueryNoCompleted();
                }
            };
            myInstance.Run();
        }

        private void QueryHistoryOrder( )
        {
            var wfFile = ActivityXamlServices.Load(ToolCommon.Path + "PC查询订单.XAML");

            var dic = new Dictionary<string, object>();

            PCQueryOrderRequest query = new PCQueryOrderRequest
            {
                QueryStartDate = DateTime.Now.AddDays(-60).ToString("yyyy-MM-dd"),
                QueryEndDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"),
                Query_where = "H"
            };
            dic.Add("requestSession", currentSession);
            dic.Add("query", query);


            var myInstance = new WorkflowApplication(wfFile, dic);

            myInstance.Completed = eventArgs =>
            {
                var activityResult = eventArgs.Outputs["activityResult"] as ActivityExcuteResult;

                if (activityResult.ExcuteCode == ActivityResultCode.Success)
                {
                    var source = eventArgs.Outputs["order"] as PCQueryOrderResponse;
                    if (source.status)
                    {
                        if (source.OrderInfo != null && source.OrderInfo.Orders != null
                            &&source.OrderInfo.Orders.Count>0)
                        {
                            Message("存在历史订单:" + CurrentUser.UserName + " " + CurrentUser.PassWord);
                            if (DeletePassengerCompleted != null)
                            {
                                DeletePassengerCompleted(this, new GetPassengerEventArgs
                                {
                                    IsLogin = true,
                                    IsTicket = true
                                });
                            }
                        }
                        else
                        {
                            Message("没有历史的订单");
                            QueryNoWayOrder();
                        }
                    }
                }
                else
                {
                    Message("失败:" + activityResult.Error_Message);
                    Thread.Sleep(1000);
                    QueryHistoryOrder(); 
                }
            };
            myInstance.Run();
        }

        private void QueryNoWayOrder( )
        {
            var wfFile = ActivityXamlServices.Load(ToolCommon.Path + "PC查询订单.XAML");

            var dic = new Dictionary<string, object>();

            PCQueryOrderRequest query = new PCQueryOrderRequest
            {
                QueryStartDate = DateTime.Now.AddDays(-60).ToString("yyyy-MM-dd"),
                QueryEndDate = DateTime.Now.ToString("yyyy-MM-dd"),
                Query_where = "G"
            };
            dic.Add("requestSession", currentSession);
            dic.Add("query", query);

            var myInstance = new WorkflowApplication(wfFile, dic);

            myInstance.Completed = eventArgs =>
            {
                var activityResult = eventArgs.Outputs["activityResult"] as ActivityExcuteResult;

                if (activityResult.ExcuteCode == ActivityResultCode.Success)
                {
                    var source = eventArgs.Outputs["order"] as PCQueryOrderResponse;
                    if (source.status)
                    {
                        if (source.OrderInfo != null && source.OrderInfo.Orders != null  &&source.OrderInfo.Orders.Count>0)
                        {
                            Message("存在未出行订单:" + CurrentUser.UserName + " " + CurrentUser.PassWord);
                            if (DeletePassengerCompleted != null)
                            {
                                DeletePassengerCompleted(this, new GetPassengerEventArgs
                                {
                                    IsLogin = true,
                                    IsTicket = true
                                });
                            }
                        } 
                        else
                        {
                            Message("没有未出行的订单");
                            QueryPassengersJson();
                        }
                    }
                }
                else
                {
                    Message("失败:" + activityResult.Error_Message);
                    Thread.Sleep(1000);
                    QueryNoWayOrder();
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
