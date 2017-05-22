using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileValidServer.PassCodeProvider
{
    public interface IPassCodeProvider
    {
        UserItem CurrentUser { get; set; }

        string Phone { get; set; }

        string Login(string user, string password);

        bool LoginSuccess(string result);

        string GetMobilePhone();

        string GetValidCode(string phone);

        void DisposePhone(string phone);

        void AddBlackPhone(string phone, string reason);
    }
}
