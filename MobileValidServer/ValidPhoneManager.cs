using Fangbian.DataStruct.Business;
using Fangbian.Log;
using Fangbian.Tickets.Trains;
using Fangbian.Tickets.Trains.DataTransferObject.Response.Login;
using Fangbian.Tickets.Trains.WFDataItem;
using MobileValidServer.PassCodeProvider;
using System;
using System.Activities;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MobileValidServer
{
    public class ValidPhoneManager
    {
        public string ServerId = "4291";

        public static string Path = Environment.CurrentDirectory + @"\Flow\";

        public event EventHandler<ConsoleMessageEventArgs> OutputMessage;

        public event EventHandler ValidCompleted;

        IPassCodeProvider codeProvider = PassCodeProviderFactory.GetPlatform();

        private RequestSession currentSession = null;

        public bool isStop = false;

        public UserItem CurrentUser { get; set; }

        public void WriteLog(string message)
        {
            Logger.Trace(CurrentUser.UserName + ":" + message);
        }

        /// <summary>
        /// 激活当前账号
        /// </summary>
        /// <param name="user"></param>
        public void Activation(UserItem user)
        {
            if (isStop)
            {
                return;
            }
           
            CurrentUser = user;

            Message(user.UserName + "开始登录");
           
            codeProvider.CurrentUser = user;

            CurrentUser.LoginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            CurrentUser.State = UserState.正在登陆;

            CurrentUser.Message = "当前正在登录" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            currentSession = new RequestSession() { UserName = user.UserName, UserPassWord = user.PassWord };

            var wfFile = ActivityXamlServices.Load(Path + "用户登录初始化.XAML");

            var dic = new Dictionary<string, object>();

            dic.Add("userAccount", new Account12306Item { UserName = user.UserName, PassWord = user.PassWord });

            dic.Add("requestSession", currentSession);

            dic.Add("activityResult", new ActivityExcuteResult { ExcuteCode = ActivityResultCode.Success });

            var myInstance = new WorkflowApplication(wfFile, dic);

            myInstance.Completed = eventArgs =>
            {
                var requestSession = eventArgs.Outputs.ContainsKey("requestSession") ? eventArgs.Outputs["requestSession"] as RequestSession : null;

                var activityResult = dic["activityResult"] as ActivityExcuteResult;

                Message(user.UserName + "登录完成");

                if (activityResult.ExcuteCode == ActivityResultCode.Success)
                {
                    requestSession.CurrentAccount = eventArgs.Outputs["accountInfo"] as AccountInfo;
                    PassengerItem mainPassenger = requestSession.CurrentAccount.CurrentUserPassengers[0];
                    //if (requestSession.CurrentAccount.CurrentUserPassengers.Count == 1)
                    //{
                    if (mainPassenger.Status != 0
                        || requestSession.CurrentAccount.DisplayControlFlag != "1")
                    {
                        Logger.Debug(user.UserName + " " + user.PassWord + "身份未核验");
                        user.State = UserState.身份未验证;
                        ExcuteValid();
                        return;
                    }
                    //if (requestSession.CurrentAccount.IsActive.ToLower().Equals("y"))
                    //{
                    if (requestSession.CurrentAccount.IsReceive.ToLower().Equals("y"))
                    {
                        Logger.Debug(user.UserName + " " + user.PassWord + "已经手机核验");
                        user.State = UserState.核验成功;
                        ExcuteValid();
                    }
                    else
                    {
                        if (mainPassenger.Status == 0)
                        {
                            ValidPhone();
                        }
                        else
                        {
                            Logger.Debug(user.UserName + " " + user.PassWord + "身份未核验");
                            CurrentUser.State = UserState.身份未验证;
                            ExcuteValid();
                        }
                    }
                    //}
                    //else
                    //{
                    //    if (requestSession.CurrentAccount.CurrentUserPassengers[0].Status == 0)
                    //    {
                    //        CurrentUser.State = UserState.身份通过邮件未通过;
                    //        ExcuteValid();
                    //    }
                    //    else 
                    //    {
                    //        CurrentUser.State = UserState.邮件未核验;
                    //        ExcuteValid();
                    //    }
                    //}
                    //}
                    //else 
                    //{
                    //    CurrentUser.State = UserState.已用;
                    //    ExcuteValid();
                    //}
                }
                else
                {
                    CurrentUser.Message = activityResult.Error_Message;

                    Message(user.UserName + "登录完成,失败" + activityResult.Error_Message);

                    CurrentUser.State = UserState.登陆失败;

                    ExcuteValid();
                }
            };
            myInstance.Run();
        }

        public void ValidPhone()
        {
            CurrentUser.LoginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            if (isStop)
            {
                return;
            }

            CurrentUser.State = UserState.登陆成功;

            codeProvider.Phone = codeProvider.GetMobilePhone();

            if (CurrentUser.State == UserState.登陆成功)
            {
                CurrentUser.State = UserState.验证码手机号码是否可用;
                CurrentUser.Phone = codeProvider.Phone;
                try
                {
                    GetCheckCode();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + ex.StackTrace);
                }
            }
            else
            {
                if (CurrentUser.State != UserState.余额不足)
                {
                    CurrentUser.State = UserState.获取手机号码失败;
                }
                ExcuteValid();
            }
        }

        private void GetCheckCode()
        {
            Console.Write("开始检查是否手机号在12306可用" + codeProvider.Phone);

            CurrentUser.LoginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (isStop)
            {
                return;
            }
            var wfFile = ActivityXamlServices.Load(Path + "获取验证码.XAML");

            var dic = new Dictionary<string, object>();

            dic.Add("phone", codeProvider.Phone);

            dic.Add("requestSession", currentSession);

            dic.Add("activityResult", new ActivityExcuteResult { ExcuteCode = ActivityResultCode.Success });

            var myInstance = new WorkflowApplication(wfFile, dic);
            Console.Write("即将执行");
          
            myInstance.Completed = eventArgs =>
            {
                var activityResult = dic["activityResult"] as ActivityExcuteResult;

                Console.Write("检查是否手机号在12306可用结果" + activityResult.Error_Message);

                if (activityResult.ExcuteCode == ActivityResultCode.Success)
                {
                    //拉取验证码
                    var validCode = codeProvider.GetValidCode(codeProvider.Phone);

                    if (!string.IsNullOrEmpty(validCode))
                    {
                        CurrentUser.Message = "获取验证码为:" + validCode;

                        CurrentUser.State = UserState.提交验证码中;

                        SumbitCheckCode(validCode);
                    }
                    else
                    {
                        codeProvider.DisposePhone(codeProvider.Phone);
                        CurrentUser.Message = activityResult.Error_Message;
                        CurrentUser.State = UserState.拉取验证码失败; 
                        ExcuteValid();
                    }
                }
                else
                {
                    if (activityResult.Error_Message.Contains("登录"))
                    {
                        codeProvider.DisposePhone(codeProvider.Phone);
                        Activation(CurrentUser);
                    }
                    else if (activityResult.Error_Message.Contains("由于您获取验证码短信次数过多"))
                    {
                        codeProvider.DisposePhone(codeProvider.Phone);

                        ExcuteValid();
                    }
                    else if (activityResult.Error_Message.Contains("system error"))
                    {
                        Thread.Sleep(1000);
                        GetCheckCode();
                    }
                    else
                    {
                        CurrentUser.State = UserState.手机号码已经被使用;

                        CurrentUser.Message = activityResult.Error_Message;

                        if (activityResult.Error_Message.Contains("已被其他用户用于在本网站注册用户"))
                        {
                            codeProvider.AddBlackPhone(codeProvider.Phone, "4");
                        }
                        else
                        {
                            codeProvider.DisposePhone(codeProvider.Phone);
                        }
                        ExcuteValid();
                    }
                }
            };
            myInstance.Run();
        }
        int sumbit = 0;
        private void SumbitCheckCode(string validCode)
        {
            if (isStop)
            {
                return;
            }

            var wfFile = ActivityXamlServices.Load(Path + "提交验证码.XAML");

            var dic = new Dictionary<string, object>();

            dic.Add("validcode", validCode);

            dic.Add("phone", codeProvider.Phone);

            dic.Add("requestSession", currentSession);

            dic.Add("activityResult", new ActivityExcuteResult { ExcuteCode = ActivityResultCode.Success });

            var myInstance = new WorkflowApplication(wfFile, dic);

            myInstance.Completed = eventArgs =>
            {
                var activityResult = dic["activityResult"] as ActivityExcuteResult;

                if (activityResult.ExcuteCode == ActivityResultCode.Success)
                {
                    //拉取验证码
                    CurrentUser.State = UserState.核验成功;
                    ExcuteValid();
                    codeProvider.AddBlackPhone(codeProvider.Phone, "0");
                }
                else
                {
                    if (activityResult.Error_Message.Contains("system error"))
                    {
                        Thread.Sleep(2000);
                        SumbitCheckCode(validCode);
                    }
                    else
                    {
                        Logger.Debug( CurrentUser.UserName+" "+CurrentUser.PassWord+ "提交验证码失败"+activityResult.Error_Message+"重试"+sumbit+"验证码："+validCode);
                         
                        if (sumbit <= 10&&
                            !activityResult.Error_Message.Contains("您输入的验证码有误")&&
                            !activityResult.Error_Message.Contains("验证码已失效，请重新获取")&&
                            !activityResult.Error_Message.Contains("请求次数过多，验证码已失效")&&
                             !activityResult.Error_Message.Contains("请先获取验证码"))
                        { 
                            CurrentUser.Message = "提交验证码"+validCode+"失败2秒后重试。。。"+activityResult.Error_Message;
                            Thread.Sleep(2000);
                            sumbit++;
                            SumbitCheckCode(validCode);
                        }
                        else
                        {
                            codeProvider.DisposePhone(codeProvider.Phone);
                            sumbit = 0;
                            CurrentUser.Message = activityResult.Error_Message;
                            CurrentUser.State = UserState.提交验证码失败;
                            ExcuteValid();
                        }
                    }
                }
            };
            myInstance.Run();
        }

        public void ExcuteValid()
        {
            if (ValidCompleted != null)
            {
                ValidCompleted(this, EventArgs.Empty);
            }
        }

        public void Message(string message)
        {
            if (OutputMessage != null)
            {
                OutputMessage(this, new ConsoleMessageEventArgs { Message = message });
            }
        }

        internal void Stop()
        {
            isStop = true;
        }
    }
}
