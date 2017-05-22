/**  版本信息模板在安装目录下，可自行修改。
* t_newaccount.cs
*
* 功 能： N/A
* 类 名： t_newaccount
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2015/7/17 15:41:56   N/A    初版
*
* Copyright (c) 2012 Maticsoft Corporation. All rights reserved.
*┌──────────────────────────────────┐
*│　此技术信息为本公司机密信息，未经本公司书面同意禁止向第三方披露．　│
*│　版权所有：动软卓越（北京）科技有限公司　　　　　　　　　　　　　　│
*└──────────────────────────────────┘
*/
using System;
namespace Maticsoft.Model
{
	/// <summary>
	/// t_newaccount:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class T_NewAccountEntity
	{
		public T_NewAccountEntity()
		{}
		#region Model
		private string _userguid;
		private string _username;
		private string _password;
		private string _passengername;
		private string _passengerid;
		private string _email;
		private string _phone;
		private int? _state;
		private DateTime? _createtime;
		private DateTime? _lasttime;
		private string _pwdquestion;
		private string _pwdanswer;
		private string _ivr_passwd;
		private string _businessid;

        public string AccountType { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string UserGuid
		{
			set{ _userguid=value;}
			get{return _userguid;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string UserName
		{
			set{ _username=value;}
			get{return _username;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string PassWord
		{
			set{ _password=value;}
			get{return _password;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string PassengerName
		{
			set{ _passengername=value;}
			get{return _passengername;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string PassengerId
		{
			set{ _passengerid=value;}
			get{return _passengerid;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Email
		{
			set{ _email=value;}
			get{return _email;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Phone
		{
			set{ _phone=value;}
			get{return _phone;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int? State
		{
			set{ _state=value;}
			get{return _state;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime? CreateTime
		{
			set{ _createtime=value;}
			get{return _createtime;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime? LastTime
		{
			set{ _lasttime=value;}
			get{return _lasttime;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string PwdQuestion
		{
			set{ _pwdquestion=value;}
			get{return _pwdquestion;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string PwdAnswer
		{
			set{ _pwdanswer=value;}
			get{return _pwdanswer;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string IVR_passwd
		{
			set{ _ivr_passwd=value;}
			get{return _ivr_passwd;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string businessId
		{
			set{ _businessid=value;}
			get{return _businessid;}
		}

        private DateTime buyTime;

        public DateTime BuyTime
        {
            get
            {
                return buyTime;
            }
            set
            {
                buyTime = value;
            }
        }

        public string EmailPassWord { get; set; }
		#endregion Model


        public string Move { get; set; }
	}


    public class QueueAccount {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}

