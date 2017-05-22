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

        public string Phone
        {
            get;
            set;
        }

        public bool IsMobilePhone(string input)
        {
            Regex regex = new Regex("^1\\d{10}$");
            return regex.IsMatch(input);
        }
        public void WriteLog(string message)
        {
            Console.WriteLine(message);
            Logger.Info(message);
        }

        public void WriteErrorLog(string message)
        {
            Console.WriteLine(message);
        }

        public virtual string Login(string user, string password)
        {
            throw new NotImplementedException();
        }

        public virtual bool LoginSuccess(string result)
        {
            throw new NotImplementedException();
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
