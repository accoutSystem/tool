using AccountRegister;
using Fangbian.DataStruct.Business;
using Fangbian.Log;
using Fangbian.Ticket.Server.AdvanceLogin;
using Fangbian.Tickets.Trains;
using Fangbian.Tickets.Trains.DataTransferObject.Request;
using Fangbian.Tickets.Trains.WFDataItem;
using FangBian.Common;
using Maticsoft.Model;
using MobileValidServer.PassCodeProvider;
using MyEntiry;
using MyTool.Common;
using PD.Business;
using ResourceBulidTool.LoginPool;
using ResourceBulidTool.PassCodeProvider;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace AccountRegister
{
   public abstract class BaseRegisterUser
    {
        protected IPassCodeProvider CurrentPassCodeProvider { get; set; }

        protected RequestSession currentSession = null;


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

       public BaseRegisterUser() {
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
           var data = DataTransaction.Create();

           data.ExecuteSql("update t_email set state=" + state + ",lastTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where emailid='" + CurrentRegisterEmail.EmailId + "'");
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
           strSql.Append("UserGuid,UserName,PassWord,PassengerName,PassengerId,Email,Phone,State,CreateTime,LastTime,PwdQuestion,PwdAnswer,IVR_passwd,businessId,accountType,move)");
           strSql.Append(" values (");
           strSql.Append("@UserGuid,@UserName,@PassWord,@PassengerName,@PassengerId,@Email,@Phone,@State,@CreateTime,@LastTime,@PwdQuestion,@PwdAnswer,@IVR_passwd,@businessId,@accountType,@move)");
           ParameterInfo[] parameters = {
					new ParameterInfo("@UserGuid", DbType.String ,36),
					new ParameterInfo("@UserName", DbType.String,40),
					new ParameterInfo("@PassWord", DbType.String,39),
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
                    new ParameterInfo("@accountType", DbType.Int32,11),
                     new ParameterInfo("@move", DbType.Int32,11)};
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
        
            
           if (RegisterMain.BulidType == ResourceBulidTool.BulidType.Default)
           {
               parameters[14].Value = 0;
           }
           if (RegisterMain.BulidType == ResourceBulidTool.BulidType.Continuity)
           {
               parameters[14].Value = 1;
           }
           if (RegisterMain.BulidType == ResourceBulidTool.BulidType.AppointUser)
           {
               parameters[14].Value = 2;
           }
           if (RegisterMain.BulidType == ResourceBulidTool.BulidType.PW)
           {
               parameters[14].Value = 3;
           }
           parameters[15].Value = model.Move;
           sql.Sql = strSql.ToString();

           sql.ParamterCollection = parameters.ToList();

           return sql;
       }

       protected void OutPutInfo(string message)
       {
           StringBuilder str = new StringBuilder("使用" + CurrentRegisterPassenger.Name + " " + CurrentRegisterEmail.Email + "注册");
           if (registerInfo != null)
           {
               str.Append(registerInfo.UserName);
           }
           str.Append(":" + message);

           Message = str.ToString();

           ExcuteMessage(str.ToString());

           //Logger.Debug(message);
       }

       public virtual void Register(T_Passenger item, T_Email emailItem)
       {
           CurrentPassCodeProvider = PassCodeProviderFactory.Current;

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
           while (true)
           {
               LastOperationTime = DateTime.Now;

               var phone = CurrentPassCodeProvider.GetMobilePhone();

               if (!string.IsNullOrEmpty(phone)
                   &&
                   !phone.Contains("message"))
               {
                   IsPlatformPhone = true;

                   OutPutInfo("从平台获取手机号码成功" + phone);
                   Logger.Info("从平台获取手机号码成功" + phone);
                   registerInfo.MobileNo = phone;

                   break;
               }
               OutPutInfo("从平台获取手机号码失败，继续获取...");

               Thread.Sleep(3000);
           }
       }

       /// <summary>
       /// 初始化失败回滚资源
       /// </summary>
       protected void RollBackResource(ActivityExcuteResult activityResult)
       {
           OutPutInfo("初始化失败,回滚资源状态" + activityResult.Error_Message);
           try
           {
               if (CurrentRegisterPassenger.State == 4)
               {
                   WritePassengerState(4);
               }
               if (CurrentRegisterPassenger.State == 0)
               {
                   WritePassengerState(0);
               }

               WriteEmailState(0);

               ExcuteRegisterEvent(new RegisterUserEventArgs { User = null, Success = false, Message = activityResult.Error_Message });

               OutPutInfo("初始化失败,回滚资源状态完成");
           }
           catch (Exception ex)
           {
               OutPutInfo("初始化失败,回滚资源状态失败" + ex.Message);

               ExcuteRegisterEvent(new RegisterUserEventArgs { User = null, Success = false, Message = activityResult.Error_Message });
           }
       }

       #region 添加用户并发送邮件
       protected void AddUser(int userType)
       {
           T_NewAccountEntity user = new T_NewAccountEntity()
           {
               Phone = registerInfo.MobileNo,
               CreateTime = DateTime.Now,
               PassengerId = CurrentRegisterPassenger.IdNo,
               PassengerName = CurrentRegisterPassenger.Name,
               UserName = registerInfo.UserName,
               PassWord = CXDataCipher.EncryptionUserPW(PassWordTo),
               UserGuid = Guid.NewGuid().ToString(),
               State = 0,
               Email = CurrentRegisterEmail.Email,
               IVR_passwd = "",
                Move=CurrentRegisterPassenger.Move,
               PwdAnswer = "",
               PwdQuestion = "",
               LastTime = DateTime.Now
           };
          
           try
           {
               if (userType == 0)
               {
                   WritePassengerState(1);

                   WriteEmailState(1);
               }

               CreateNewAccountInDB(user);

               StorageSuccess(user);

               if (CurrentPassCodeProvider is CXPasscodeProvider)
               {
                   (CurrentPassCodeProvider as CXPasscodeProvider).SuccessPhone(user.Phone);
               }
               Logger.Info("注册" + registerInfo.UserName + "  " + registerInfo.Name + " " + registerInfo.Email + " " + registerInfo.MobileNo+ "执行存储成功");
               ExcuteMessage("注册" + registerInfo.UserName + "  " + registerInfo.Name + " " + registerInfo.Email + "执行存储成功");

               //SendEmailActivation login = new SendEmailActivation() { };
               //login.Data = user;
               //login.SendEmailCompleted += SendEmailCompleted;
               //login.OutputMessage += SendEmailOutputMessage;
               //login.Activation(new Account12306Item { PassWord = CXDataCipher.DecipheringUserPW(user.PassWord), UserName = user.UserName });
           }
           catch
           {
               Storage(user);
           }
       }

       private void SendEmailOutputMessage(object sender, ConsoleMessageEventArgs e)
       {
           OutPutInfo(e.Message);
       }

       private void SendEmailCompleted(object sender, SendEmailEventArgs e)
       {
           var user = sender as SendEmailActivation;

           if (e.Message.Contains("登录名不存在"))
           {
               AccountState.UpdateState((user.Data as T_NewAccountEntity).UserGuid, 8);
           }
           else if (e.Message.Contains("邮件已经核验"))
           {
               AccountState.UpdateState((user.Data as T_NewAccountEntity).UserGuid, 1);
           }
           else if (e.Message.Contains("请核实您注册用户信息是否真实")
              || e.Message.Contains("密码输入错误")
              || e.Message.Contains("该用户已被暂停使用")
              || e.Message.Contains("您的用户信息被他人冒用"))
           {

               AccountState.UpdateState((user.Data as T_NewAccountEntity).UserGuid, 9);
           }
       }
       #endregion

       #region 添加联系人
       /// <summary>
       /// 检验乘车人是否可以注册
       /// </summary>
       protected void ValidPassenger()
       {
           LastOperationTime = DateTime.Now;
           OutPutInfo("开始获取添加联系人的账号,请等待...");
           var current = AccountActivationPool.Current.Get();
           LinkUser = current.CurrentUser.UserName + " " + current.CurrentUser.PassWord + " " + current.CurrentSession.CurrentAccount.CurrentUserPassengers.Count + "个";
           OutPutInfo("开始添加联系人使用账号:" + current.CurrentUser.UserName + " " + current.CurrentUser.PassWord);
           current.AddPassengerCompleted += ValidPassengerCompleted;
           current.AddPassenger(registerInfo.Name, registerInfo.IdNo);
       }

       protected void ValidPassengerCompleted(object sender, EventArgs e)
       {
           LastOperationTime = DateTime.Now;

           OutPutInfo("添加联系人完成");

           var current = sender as AccountActivation;

           current.AddPassengerCompleted -= ValidPassengerCompleted;

           if (current.State == AccountTaskState.查询联系人完成)
           {
               var passenger = current.CurrentSession.CurrentAccount.CurrentUserPassengers.FirstOrDefault(item => item.IdNo.ToLower().Equals(registerInfo.IdNo.ToLower()));

               if (passenger != null && passenger.Status == 0)
               {
                   OutPutInfo("联系人身份核验通过");
                   //可以注册
                   IsValidPassenger = true;

                   GetMobileNo();

                   CheckRegisterUserInfo();
               }
               else
               {
                   OutPutInfo("联系人身份核验未通过,删除联系人");
                   current.DeletePassengerCompleted += DeletePassengerCompleted;
                   current.DeletePassenger(registerInfo.IdNo);
               }
           }
           else if (current.State == AccountTaskState.身份未通过)
           {
               AccountState.UpdateState(current.Data + string.Empty, 5);

               Logger.Fatal(current.CurrentUser.UserName + " " + current.CurrentUser.PassWord + "由于您的身份信息未通过核验已经修改账号状态成功");

               ValidPassenger();
               return;
           }
           else
           {
               ValidPassenger();
           }
           current.State = AccountTaskState.登录完成;
       }

       protected void DeletePassengerCompleted(object sender, EventArgs e)
       {
           LastOperationTime = DateTime.Now;
           OutPutInfo("删除联系人完成");
           var current = sender as AccountActivation;
           current.DeletePassengerCompleted -= DeletePassengerCompleted;
           WritePassengerState(1);
           this.CurrentRegisterPassenger = RegisterMain.Current.GetPassenger();
           OutPutInfo("删除联系人完成,在一个任务内重新获取证件号码成功，重新检查");
           RegisterInfo = null;
           LinkUser = string.Empty;
           IsValidPassenger = false;
           CheckRegisterUserInfo();
           current.State = AccountTaskState.登录完成;
       }
        #endregion

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
               if (string.IsNullOrEmpty(UserName) && string.IsNullOrEmpty(PassWord))
               {
                   if (RegisterMain.BulidType == ResourceBulidTool.BulidType.Continuity)
                   {
                       #region 生成连号资源
                       while (true)
                       {
                           if (IsActivity == false) { 
                               break;
                           } 
                           userName = System.Configuration.ConfigurationManager.AppSettings["lhQZ"];
                           passWord = System.Configuration.ConfigurationManager.AppSettings["lhPW"];
                           var between = System.Configuration.ConfigurationManager.AppSettings["lhBetween"].Split(',');
                           var min = Convert.ToInt32(between[0]);
                           var max = Convert.ToInt32(between[1]);
                           Random r = new Random();
                           var value = r.Next(min, max).ToString();
                           var maxValue = max.ToString();
                           var addZero = maxValue.Length - value.Length;
                           for (int i = 0; i < addZero; i++)
                           {
                               userName += "0";
                               passWord += "0";
                           }
                           userName += value;
                           passWord += value;
                           Console.WriteLine("生成的用户名为" + userName); 
                           var db = PD.Business.DataTransaction.Create();
                           var source = db.Query(string.Format(@"select sum(c) c from(
select count(1) c from t_newaccount where username='{0}'
union all 
select count(1) c from t_hisnewaccount where username='{0}') a", userName)).Tables[0];
                           
                           if (Convert.ToInt32(source.Rows[0][0]) > 0)
                           {
                               Console.WriteLine(userName + "已经存在,重新构建");
                               Thread.Sleep(500);
                           }
                           else 
                           {
                               bool isContinue = false;
                               foreach (var item in RegisterUserCollection.Current) {
                                   if (item != this) 
                                   {
                                       if (item.RegisterInfo.UserName == userName)
                                       {
                                           isContinue = true;
                                           break;
                                       }
                                   }
                               }
                               if (isContinue)
                               {
                                   Thread.Sleep(200);
                                   continue; 
                               }
                               else
                               {
                                   break;
                               }
                           }
                       }
 
                       #endregion
                   }
                   else
                   {
                       #region 生成用户名密码

                       var item = ToolCommonMethod.GetChineseSpellCode(CurrentRegisterPassenger.Name);
                       //如果是生僻身份证的话且开始字符是数字就需要替换为字符串
                       string str = string.Empty;
                       foreach (var c in item)
                       {
                           if (Convert.ToInt32(c) >= 49 && Convert.ToInt32(c) <= 57)
                           {
                               str += "A";
                           }
                           else
                           {
                               str += c;
                           }
                       }
                       item = str;

                       var birthday = ToolCommonMethod.GetBirthdayIDNo(CurrentRegisterPassenger.IdNo);

                       var random = ToolCommonMethod.GetRandom(2);

                       userName = item + birthday + random;

                       if (RegisterMain.BulidType == ResourceBulidTool.BulidType.PW)
                       {
                           passWord = System.Configuration.ConfigurationManager.AppSettings["lhPW"];
                       }
                       else
                       {
                           passWord = item.Length > 4 ? item.Substring(0, 4) : item;

                           for (int i = 0; i < 6; i++)
                           {
                               passWord += es.Next(10);
                           }
                       }

                       #endregion
                   }
               }
               else
               {
                   userName = UserName;
                   passWord = PassWord;
               }
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
               ValidPassenger();
           }
           else
           {
               WritePassengerState(5);

               if (IsPlatformPhone == false)
               {
                   GetMobileNo();
                   CheckRegisterUserInfo();
                   return;
               }

               OutPutInfo("开始从平台获取手机" + registerInfo.MobileNo + "的验证码");

               while (true)
               {
                   string source = string.Empty;

                   if (CurrentPassCodeProvider is FMPassCodeProvider)
                   {
                       source = (CurrentPassCodeProvider as FMPassCodeProvider).SendSMS(registerInfo.MobileNo);
                   }
                   if (CurrentPassCodeProvider is CXPasscodeProvider)
                   {
                       source = (CurrentPassCodeProvider as CXPasscodeProvider).SendMessage(registerInfo.MobileNo);
                   }
                   if (CurrentPassCodeProvider is YYYPassCodeProvider)
                   {
                       source = (CurrentPassCodeProvider as YYYPassCodeProvider).SendSMS(registerInfo.MobileNo);
                   }
                   if (!string.IsNullOrEmpty(source) && "OK".Equals(source.ToUpper()))
                   {
                       break;
                   }
                   else
                   {
                       Thread.Sleep(1000);
                   }
               }

               string validCode = CurrentPassCodeProvider.GetValidCode(registerInfo.MobileNo);

               OutPutInfo("从平台获取手机" + registerInfo.MobileNo + "的验证码为" + validCode);

               if (!string.IsNullOrEmpty(validCode))
               {
                   Logger.Info( registerInfo.MobileNo + "的验证码为" + validCode+"开始注册");

                   RegisterUserInfo(validCode);
               }
               else
               {
                   if (CurrentPassCodeProvider is CXPasscodeProvider)
                   {
                       (CurrentPassCodeProvider as CXPasscodeProvider).RetryPhone(registerInfo.MobileNo);
                   }
                   else
                   {
                       CurrentPassCodeProvider.AddBlackPhone(registerInfo.MobileNo, "0");
                   }

                   GetMobileNo();
                   CheckRegisterUserInfo();
               }
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
               WritePassengerState(1);
               this.CurrentRegisterPassenger = RegisterMain.Current.GetPassenger();
               OutPutInfo("在一个任务内重新获取证件号码成功，重新检查");
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
           else if (activityResult.Error_Message.Contains("您获取验证码短信次数过多"))
           {
               try
               {
                   if (IsPlatformPhone)
                   {
                       CurrentPassCodeProvider.DisposePhone(registerInfo.MobileNo);
                   }
                   WriteEmailState(0);
                   WritePassengerState(5);
               }
               catch (Exception ex)
               {
                   OutPutInfo("您获取验证码短信次数过多回滚资源状态失败" + ex.Message);
               }
           }
           else if (activityResult.Error_Message.Contains("该手机号码已被注册") ||
               activityResult.Error_Message.Contains("手机号码格式错误") || 
              activityResult.Error_Message.Contains("您输入的手机号码已被其他注册用户"))
           {
               if (IsPlatformPhone)
               {
                   CurrentPassCodeProvider.AddBlackPhone(registerInfo.MobileNo, string.Empty);
               }

               GetMobileNo();

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
               WritePassengerState(0);
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

   public class RegisterUserCollection : List<BaseRegisterUser>
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
