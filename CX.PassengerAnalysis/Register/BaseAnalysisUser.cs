using AccountRegister;
using Fangbian.DataStruct.Business;
using Fangbian.Log;
using Fangbian.Ticket.Server.AdvanceLogin;
using Fangbian.Tickets.Trains;
using Fangbian.Tickets.Trains.DataTransferObject.Request;
using Fangbian.Tickets.Trains.WFDataItem;
using FangBian.Common;
using Maticsoft.Model;
using MyEntiry;
using MyTool.Common;
using PD.Business;
using ResourceBulidTool.LoginPool;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace AccountRegister
{
   public abstract class BaseAnalysisUser
    {

        protected RequestSession currentSession = null;

        public event EventHandler ErrorPassengerEvent;


        /// <summary>
        /// 是否是平台号
        /// </summary>
        private bool isPlatformPhone = false;
        /// <summary>
        /// 是否是平台号
        /// </summary>
        public bool IsPlatformPhone
        {
            get { return isPlatformPhone; }
            set { isPlatformPhone = value; ChangeProperty("IsPlatformPhone", value); }
        }

        /// <summary>
        /// 是否检查身份证
        /// </summary>
        public bool IsCheckIdNo { get; set; }

        public bool IsActivity { get; set; }

        /// <summary>
        /// 是否验证码注册的乘车人
        /// </summary>
        protected bool IsValidPassenger { get; set; }

        protected string PassWordTo { get; set; }

        public string UserName { get; set; }

        public string PassWord { get; set; }

        private int taskRegisterCount = 0;

        public int TaskRegisterCount
        {
            get { return taskRegisterCount; }
            set
            {
                taskRegisterCount = value;
                ChangeProperty("TaskRegisterCount", value);
            }
        }

        private DateTime bulidTime = DateTime.Now;

        public DateTime BulidTime
        {
            get { return bulidTime; }
            set { bulidTime = value; ChangeProperty("BulidTime", value); }
        }

        private DateTime lastOperationTime = DateTime.Now;

        public DateTime LastOperationTime
        {
            get { return lastOperationTime; }
            set { lastOperationTime = value; ChangeProperty("LastOperationTime", value); }
        }

        protected string linkUser;

        public string LinkUser
        {
            get { return linkUser; }
            set { linkUser = value; ChangeProperty("LinkUser", value); }
        }

        protected RegisterUserInfo registerInfo = null;

        public RegisterUserInfo RegisterInfo
        {
            get { return registerInfo; }
            set
            {
                registerInfo = value;
                ChangeProperty("UserName", value);
                if (value == null)
                {
                    LinkUser = string.Empty;
                }
            }
        }

        private T_Email currentRegisterEmail;

        public T_Email CurrentRegisterEmail
        {
            get { return currentRegisterEmail; }
            set { currentRegisterEmail = value; 
                ChangeProperty("CurrentRegisterEmail", value); }
        }

        private T_Passenger currentRegisterPassenger;

        public T_Passenger CurrentRegisterPassenger
        {
            get { return currentRegisterPassenger; }
            set
            {
                currentRegisterPassenger = value;
                ChangeProperty("CurrentRegisterPassenger", value);
            }
        }

        private string message = string.Empty;

        public string Message
        {
            get { return message; }
            set { message = value; ChangeProperty("Message", value); }

        }

       public event EventHandler<ConsoleMessageEventArgs> OutputMessage;

       public event EventHandler<RegisterUserEventArgs> RegisterUserCompleted;

       public event EventHandler<PropertyChangeEventArgs> PropertyChange;

       protected void ExcuteRegisterEvent(RegisterUserEventArgs data)
       {
           if (RegisterUserCompleted != null)
           {
               RegisterUserCompleted(this, data);
           }
       }

       protected void ChangeProperty(string name, object value)
       {
           if (PropertyChange != null)
           {
               PropertyChange(this, new PropertyChangeEventArgs { PropertyName = name, Value = value });
           }
       }

       public BaseAnalysisUser()
       {
           IsActivity = true;
       }

       protected void WritePassengerState(int state)
       {
           try
           {
               var data = DataTransaction.Create();

               data.ExecuteSql("update t_passenger set state=" + state + " where passengerid='" + CurrentRegisterPassenger.PassengerId + "' ");
           }
           catch { }
       }

       protected void WriteEmailState(int state)
       {
       }

       protected void ExcuteMessage(string message)
       {
           if (OutputMessage != null)
           {
               OutputMessage(this, new ConsoleMessageEventArgs { Message = message });
           }
       }


       /// <summary>
       /// 写入数据库失败后存储到本地
       /// </summary>
       /// <param name="item"></param>
       protected void Storage(T_NewAccountEntity item)
       {
           ExcuteMessage("写入文件" + item.UserName);

           try
           {
               string path = System.Environment.CurrentDirectory + @"\Data\BadWriteDBUser.txt";

               FileStream fs = new FileStream(path, FileMode.Append);

               StreamWriter sw = new StreamWriter(fs);

               sw.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(item));

               sw.Close();

               sw.Dispose();

               ExcuteMessage("写入文件成功" + item.UserName);
           }
           catch (Exception ex)
           {
               ExcuteMessage("写入文件失败" + item.UserName + ex.Message);
           }
       }

       /// <summary>
       /// 写入数据库失败后存储到本地
       /// </summary>
       /// <param name="item"></param>
       protected void StorageSuccess(T_NewAccountEntity item)
       {
           ExcuteMessage("写入文件" + item.UserName);

           try
           {
               string path = System.Environment.CurrentDirectory + @"\Data\GoodWriteDBUser.txt";

               FileStream fs = new FileStream(path, FileMode.Append);

               StreamWriter sw = new StreamWriter(fs);

               sw.WriteLine(item.UserGuid + "," + item.UserName + "," + item.PassWord);

               sw.Close();

               sw.Dispose();

               ExcuteMessage("写入文件成功" + item.UserName);
           }
           catch (Exception ex)
           {
               ExcuteMessage("写入文件失败" + item.UserName + ex.Message);
           }
       }

       /// <summary>
       /// 添加新注册的用户更
       /// </summary>
       /// <param name="user"></param>
       protected void CreateNewAccountInDB(T_NewAccountEntity user)
       {
           var data = DataTransaction.Create();

           data.ExecuteSql(CreateNewAccountScript(user));

           if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(PassWord))
           {
               data.ExecuteSql(UpdateTempAccount(1, UserName));
           }
       }

       protected SqlParamterItem UpdateTempAccount(int isUser, string userName)
       {
           var sql = new SqlParamterItem();

           StringBuilder strSql = new StringBuilder();

           strSql.Append("update  t_tempaccount set isUser=@isUser where accountName=@accountName");
           ParameterInfo[] parameters = {
					new ParameterInfo("@isUser", DbType.Int32  ,45),
					new ParameterInfo("@accountName", DbType.String,40) 
                                         };
           parameters[0].Value = isUser;
           parameters[1].Value = userName;

           sql.Sql = strSql.ToString();

           sql.ParamterCollection = parameters.ToList();

           return sql;
       }

       protected SqlParamterItem AddLink(T_NewAccountEntity model)
       {
           var sql = new SqlParamterItem();
           StringBuilder strSql = new StringBuilder();
           strSql.Append("insert into t_customaccount(");
           strSql.Append("userguid,accounttype)");
           strSql.Append(" values (");
           strSql.Append("@userguid,@accounttype)");
           ParameterInfo[] parameters = {
					new ParameterInfo("@userguid", DbType.String ,45),
					new ParameterInfo("@accounttype", DbType.Int32,40) };
           parameters[0].Value = model.UserGuid;
           parameters[1].Value = 1;

           sql.Sql = strSql.ToString();

           sql.ParamterCollection = parameters.ToList();

           return sql;
       }

       protected SqlParamterItem CreateNewAccountScript(T_NewAccountEntity model)
       {
           var sql = new SqlParamterItem();
           StringBuilder strSql = new StringBuilder();
           strSql.Append("insert into t_newaccount(");
           strSql.Append("UserGuid,UserName,PassWord,PassengerName,PassengerId,Email,Phone,State,CreateTime,LastTime,PwdQuestion,PwdAnswer,IVR_passwd,businessId,accountType)");
           strSql.Append(" values (");
           strSql.Append("@UserGuid,@UserName,@PassWord,@PassengerName,@PassengerId,@Email,@Phone,@State,@CreateTime,@LastTime,@PwdQuestion,@PwdAnswer,@IVR_passwd,@businessId,@accountType)");
           ParameterInfo[] parameters = {
					new ParameterInfo("@UserGuid", DbType.String ,36),
					new ParameterInfo("@UserName", DbType.String,40),
					new ParameterInfo("@PassWord", DbType.String,30),
					new ParameterInfo("@PassengerName", DbType.String,20),
					new ParameterInfo("@PassengerId", DbType.String,39),
					new ParameterInfo("@Email", DbType.String,45),
					new ParameterInfo("@Phone", DbType.String,20),
					new ParameterInfo("@State", DbType.Int32  ,11),
					new ParameterInfo("@CreateTime",DbType.DateTime,0),
					new ParameterInfo("@LastTime", DbType.DateTime,0),
					new ParameterInfo("@PwdQuestion", DbType.String,45),
					new ParameterInfo("@PwdAnswer", DbType.String,45),
					new ParameterInfo("@IVR_passwd", DbType.String,45),
					new ParameterInfo("@businessId", DbType.String,45),
                    new ParameterInfo("@accountType", DbType.Int32,11),};
           parameters[0].Value = model.UserGuid;
           parameters[1].Value = model.UserName;
           parameters[2].Value = model.PassWord;
           parameters[3].Value = model.PassengerName;
           parameters[4].Value = model.PassengerId;
           parameters[5].Value = model.Email;
           parameters[6].Value = model.Phone;
           parameters[7].Value = model.State;
           parameters[8].Value = model.CreateTime;
           parameters[9].Value = model.LastTime;
           parameters[10].Value = model.PwdQuestion;
           parameters[11].Value = model.PwdAnswer;
           parameters[12].Value = model.IVR_passwd;
           parameters[13].Value = model.businessId;
           
           sql.Sql = strSql.ToString();
           sql.ParamterCollection = parameters.ToList();
           return sql;
       }

       protected void OutPutInfo(string message)
       {
           //"使用" + CurrentRegisterPassenger.Name + " " + CurrentRegisterEmail.Email + "注册"
           StringBuilder str = new StringBuilder();
           if (registerInfo != null)
           {
               str.Append(registerInfo.UserName);
           }
           //str.Append(":" + message);
           str.Append(message);
           Message = str.ToString();
           ExcuteMessage(str.ToString());
       }

       public virtual void Register(T_Passenger item, T_Email emailItem)
       {
           CurrentRegisterEmail = emailItem;

           CurrentRegisterPassenger = item;

           LastOperationTime = BulidTime = DateTime.Now;
       }

       protected virtual void Init()
       {
           throw new Exception("未实现");
       }

       protected virtual void CheckRegisterUserInfo()
       {
           throw new Exception("未实现");
       }

       protected virtual void RegisterUserInfo(string passCode)
       {
           throw new Exception("未实现");
       }

       /// <summary>
       /// get phone
       /// </summary>
       protected void GetMobileNo()
       {
          
       }

       /// <summary>
       /// 初始化失败回滚资源
       /// </summary>
       protected void RollBackResource(ActivityExcuteResult activityResult)
       {
           OutPutInfo("初始化失败,回滚资源状态" + activityResult.Error_Message);
           try
           {
               WriteEmailState(7);

               ExcuteRegisterEvent(new RegisterUserEventArgs { User = null, Success = false, Message = activityResult.Error_Message });

               OutPutInfo("初始化失败,回滚资源状态完成");
           }
           catch (Exception ex)
           {
               OutPutInfo("初始化失败,回滚资源状态失败" + ex.Message);

               ExcuteRegisterEvent(new RegisterUserEventArgs { User = null, Success = false, Message = activityResult.Error_Message });
           }
       }
   
       /// <summary>
       /// 生成注册信息
       /// </summary>
       protected void BulidRegisterInfo() 
       {
           if (registerInfo == null)
           {
               Random es = new Random();
               string userName = string.Empty;

               string passWord = string.Empty;

               #region 生成用户名密码
               if (string.IsNullOrEmpty(UserName) && string.IsNullOrEmpty(PassWord))
               {
                   var item = ToolCommonMethod.GetChineseSpellCode(CurrentRegisterPassenger.Name);
                   if (item.Contains("1"))
                   {
                       item = item.Replace("1", "A");
                   }
                   var birthday = ToolCommonMethod.GetBirthdayIDNo(CurrentRegisterPassenger.IdNo);

                   var random = ToolCommonMethod.GetRandom(2);

                   userName = item + birthday + random;

                   passWord = item;

                   for (int i = 0; i < 6; i++)
                   {
                       passWord += es.Next(10);
                   }
               }
               else
               {
                   userName = UserName;
                   passWord = PassWord;
               }
               #endregion

               PassWordTo = passWord;

               RegisterInfo = new RegisterUserInfo
               {
                   Email = CurrentRegisterEmail.Email,
                   UserName = userName,
                   PassWord = Md5Helper.Md5(passWord),
                   IdNo = CurrentRegisterPassenger.IdNo,
                   Name = CurrentRegisterPassenger.Name
               };
           }
       
       }


       /// <summary>
       /// 检查成功后的操作
       /// </summary>
       protected void CheckSuccessOperation()
       {
           #region 检查成功后获取验证码发送验证码
           OutPutInfo("检查用户信息成功");

           if (IsCheckIdNo && IsValidPassenger == false && CurrentRegisterPassenger.State == 0)
           {
                
           }
           else
           {
               WritePassengerState(5); 
           }
           #endregion
       }

       /// <summary>
       /// 检查失败后合理化错误的操作
       /// </summary>
       /// <param name="activityResult"></param>
       protected void CheckErrorOperation(ActivityExcuteResult activityResult)
       {
           #region 合理化错误
           if (activityResult.Error_Message.Contains("该证件号码已被注册") ||
               activityResult.Error_Message.Contains("用户名格式错误"))
           {
               if (ErrorPassengerEvent != null)
               {
                   ErrorPassengerEvent(this, EventArgs.Empty);
               }
               OutPutInfo("在一个任务内重新获取证件号码成功，重新检查");
               WritePassengerState(1);
               this.CurrentRegisterPassenger = RegisterMain.Current.GetPassenger();
               RegisterInfo = null;
               IsValidPassenger = false;
               CheckRegisterUserInfo();
               return;
           }
           else if (activityResult.Error_Message.Contains("该邮箱已被注册"))
           {
               WriteEmailState(1);
               this.CurrentRegisterEmail = RegisterMain.Current.GetEmail();
               OutPutInfo("在一个任务内重新获取邮件成功，重新检查");
               registerInfo.Email = this.CurrentRegisterEmail.Email;
               CheckRegisterUserInfo();
               return;
           }
           else if (activityResult.Error_Message.Contains("该注册名已存在")
               && !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(PassWord))
           {
               UpdateTempAccount(1, UserName);
           }
           else
           {
               WritePassengerState(7);
               WriteEmailState(0);
           }

           ExcuteRegisterEvent(new RegisterUserEventArgs { User = registerInfo, Success = false, Message = activityResult.Error_Message });

           #endregion

       }
    }


   public class PropertyChangeEventArgs : EventArgs
   {
       public string PropertyName { get; set; }
       public object Value { get; set; }
   }

   public class RegisterUserEventArgs : EventArgs
   {
       public RegisterUserInfo User { get; set; }

       public bool Success { get; set; }

       public string Message { get; set; }

   }

   public class RegisterUserCollection : List<BaseAnalysisUser>
   {
       private static RegisterUserCollection current = null;
       public static RegisterUserCollection Current
       {
           get
           {
               if (current == null)
                   current = new RegisterUserCollection();
               return current;
           }
       }
   }
}
