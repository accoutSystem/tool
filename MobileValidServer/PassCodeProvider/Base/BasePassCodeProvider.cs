using Fangbian.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MobileValidServer.PassCodeProvider
{
    public abstract class BasePassCodeProvider : IPassCodeProvider
    {
        public UserItem CurrentUser
        {
            get;
            set;
        }

        public string Phone
        {
            get;
            set;
        }
        public void WriteLog(string message)
        {
            Logger.Info(CurrentUser.UserName + " 当前状态:" + CurrentUser.State + " 消息:" + message);
        }

        public void WriteErrorLog(string message)
        {
            Logger.Error(CurrentUser.UserName + " 当前状态:" + CurrentUser.State + " 消息:" + message);
        }

        public virtual string Login(string user, string password)
        {
            throw new NotImplementedException();
        }

        public virtual bool LoginSuccess(string result)
        {
            throw new NotImplementedException();
        }

        public  bool IsMobilePhone(string input)
        {
            Regex regex = new Regex("^1\\d{10}$");
            return regex.IsMatch(input);
        }

        public virtual string GetMobilePhone()
        {
            throw new NotImplementedException();
        }

        public virtual string GetValidCode(string phone)
        {
            throw new NotImplementedException();
        }

        public virtual void DisposePhone(string phone)
        {
            throw new NotImplementedException();
        }

        public virtual void AddBlackPhone(string phone, string reason)
        {
            throw new NotImplementedException();
        }
    }
}
