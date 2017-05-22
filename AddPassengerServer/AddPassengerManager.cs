using Fangbian.Common;
using Fangbian.DataStruct.Business;
using Fangbian.Tickets.Trains;
using Fangbian.Tickets.Trains.DataTransferObject.Response.Login;
using Fangbian.Tickets.Trains.WFDataItem;
using MyEntiry;
using System;
using System.Activities;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace AddPassengerServer
{
    public class AddPassengerManager
    {
        public event EventHandler<AddPassengerEventArgs> AddPassengerCompleted;

        public event EventHandler<ConsoleMessageEventArgs> OutputMessage;

        public Account12306Item CurrentUser { get; set; }

        private int loginRetryCount;

        private int index = 0;


        private RequestSession currentSession = null;

        public List<T_Passenger> Passengers { get; set; }

        public void Excute(Account12306Item currentUser)
        {
            AddPassengerManagerCollection.Current.Add(this);
            CurrentUser = currentUser;
            Login();
        }

        private void Login()
        {
            ExcuteConsole("开始登录");

            currentSession = new RequestSession() { OrderId = Guid.NewGuid().ToString() };

            var wfFile = ActivityXamlServices.Load(CommonMethod.GetXamlPath("用户登录初始化"));

            var dic = new Dictionary<string, object>();

            dic.Add("userAccount", new Account12306Item { UserName = CurrentUser.UserName, PassWord = CurrentUser.PassWord });

            dic.Add("requestSession", currentSession);

            dic.Add("activityResult", new ActivityExcuteResult { ExcuteCode = ActivityResultCode.Success });

            var myInstance = new WorkflowApplication(wfFile, dic);

            myInstance.Completed = eventArgs =>
            {
                var requestSession = dic["requestSession"] as RequestSession;

                var activityResult = dic["activityResult"] as ActivityExcuteResult;

                if (activityResult.ExcuteCode == ActivityResultCode.Success)
                {
                    requestSession.CurrentAccount = eventArgs.Outputs["accountInfo"] as AccountInfo;

                    ExcuteConsole("登录成功");
                    AddPassenger();
                }
                else
                {
                    if ((activityResult.ExcuteCode == ActivityResultCode.LoginError ||
                     activityResult.ExcuteCode == ActivityResultCode.ActivityException) &&
                     (activityResult.Error_Message.Contains("登录名不存在")
                     || activityResult.Error_Message.Contains("请核实您注册用户信息是否真实")
                     || activityResult.Error_Message.Contains("密码输入错误")
                     || activityResult.Error_Message.Contains("该用户已被暂停使用")
                     || activityResult.Error_Message.Contains("您的用户信息被他人冒用，请重新在网上注册新的账户")
                     || activityResult.ResponseSource.Contains("system error")
                     || activityResult.ResponseSource.Contains("bad request")
                     || activityResult.ResponseSource.Contains("Runtime: Failed to parse JSON string")
                     || loginRetryCount > 2))
                    {
                        ExcuteOrderCompleted(false, "用户登录失败," + activityResult.Error_Message);
                    }
                    else
                    {
                        ExcuteConsole("登录失败," + activityResult.Error_Message + ",正在重试");
                        loginRetryCount++;
                        Login();
                    }
                }
            };
            myInstance.Run();
        }

        private int retryCount = 0;


        public void AddPassenger()
        {
            if (index >= Passengers.Count)
            {
                ExcuteOrderCompleted(true, "添加乘车人成功");
            }

            var passengerItem = Passengers[index];

            index++;

            Thread.Sleep(1000);

            var wfFile = ActivityXamlServices.Load(CommonMethod.GetXamlPath("添加乘车人"));

            var dic = new Dictionary<string, object>();

            dic.Add("requestSession", currentSession);

            dic.Add("accountInfo", currentSession.CurrentAccount);

            var item = new PassengerItem
            {
                UserName = passengerItem.Name,
                MobileNo = "",
                IdNo = passengerItem.IdNo,
                IdType = "1",
                UserType = "1"
            };

            dic.Add("newPassenger", new List<PassengerItem> { item });

            var myInstance = new WorkflowApplication(wfFile, dic);

            myInstance.Completed = eventArgs =>
            {
                var activityResult = eventArgs.Outputs["activityResult"] as ActivityExcuteResult;

                if (activityResult.ExcuteCode == ActivityResultCode.Success)
                {
                    retryCount = 0;
                    ExcuteConsole("添加" + passengerItem.Name + "成功");
                }
                else
                {
                    ExcuteConsole("添加" + passengerItem.Name + "失败" + activityResult.Error_Message);

                    retryCount++;

                    if (retryCount <= 10)
                    {
                        index--;
                    }
                    else
                    {
                        retryCount = 0;
                    }
                }

                AddPassenger();
            };

            myInstance.Run();
        }
      

        private void ExcuteOrderCompleted(bool isSuccess, string message)
        {
            AddPassengerManagerCollection.Current.Remove(this);

            ExcuteConsole(message);

            if (AddPassengerCompleted != null)
            {
                AddPassengerCompleted(this, new AddPassengerEventArgs { IsSuccess = isSuccess, Message = message });
            }
        }

        private void ExcuteConsole(string message)
        {
            if (OutputMessage != null)
            {
                OutputMessage(this, new ConsoleMessageEventArgs { Message = CurrentUser.UserName + "=>" + message });
            }
        }
    }

    public class AddPassengerManagerCollection : List<AddPassengerManager>
    {
        private static AddPassengerManagerCollection current;

        public static AddPassengerManagerCollection Current
        {
            get
            {
                if (current == null)
                {
                    current = new AddPassengerManagerCollection();
                }
                return AddPassengerManagerCollection.current;
            }

        }
    }
    public class AddPassengerEventArgs : EventArgs
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }
    }
}
