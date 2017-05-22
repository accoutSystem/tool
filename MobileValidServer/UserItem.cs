using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileValidServer
{
    public class UserItemCollection : List<UserItem>
    {
        private static UserItemCollection current;

        public static UserItemCollection Current
        {
            get {
                if (current == null) {
                    current = new UserItemCollection();
                }
                return UserItemCollection.current; }
           
        }
    }

    public class PropertyChangeEventArgs : EventArgs {
        public string PropertyName { get; set; }
        public object Value { get; set; }
    }

    public class UserItem
    {
        public UserItem() 
        {
            LoginTime=CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public event EventHandler<PropertyChangeEventArgs> PropertyChange;

        private string userGuid;

        public string UserGuid
        {
            get { return userGuid; }
            set { userGuid = value; Change("UserGuid", value); }
        }
       
        private string userName;

        public string UserName
        {
            get { return userName; }
            set { userName = value; Change("UserName", value); }
        }

        private string passWord;
       
        public string PassWord
        {
            get { return passWord; }
            set { passWord = value; Change("PassWord", value); }
        }


        private string createTime;
         
        public string CreateTime
        {
            get { return createTime; }
            set { createTime = value; Change("CreateTime", value); }
        }

        private string loginTime;
     
        public string LoginTime
        {
            get { return loginTime; }
            set { loginTime = value; Change("LoginTime", value); }
        }


        private UserState state;
      
        public UserState State
        {
            get { return state; }
            set { state = value; Change("State", value); }
        }
        private string message;
           
        public string Message
        {
            get { return message; }
            set { message = value; Change("Message", value); }
        }
        private string phone;

        public string Phone
        {
            get { return phone; }
            set { phone = value; Change("Phone", value); }
        }
        public void Change(string name,object value)
        {
            if (PropertyChange != null)
            {
                PropertyChange(this, new PropertyChangeEventArgs {  PropertyName=name, Value=value});
            }
        }

        public ValidPhoneManager Valid { get; set; }

    }
   public enum UserState
   {
       未使用 = 0,
       正在登陆 = 1,
       登陆成功 = 2,
       登陆失败=3,
       获取手机号码失败=4,
       拉取验证码失败=5,
       提交验证码失败=6,
       核验成功=7,
       验证码手机号码是否可用=8,
       提交验证码中 = 9,
       手机号码已经被使用= 10,
       余额不足 = 11,
       邮件未核验 = 12,
       身份未验证 = 13,
       已用 = 14,
       身份通过邮件未通过 = 15,

   }
}
