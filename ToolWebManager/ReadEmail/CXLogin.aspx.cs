using glib.Email;
using MySql.Data.MySqlClient;
using PD.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ToolWebManager.Email
{
    public partial class CXLogin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
              //MimeReader readMail = new MimeReader();

              //var email = readMail.GetEmail(@"D:\WorkSpace\Resource\李超\Too工具集合\WindowsFormsApplication1\{E288C93C-3A82-44B5-AEF9-3702FF7FF99B}.eml");
             
        }

        protected void Login1_Authenticate(object sender, AuthenticateEventArgs e)
        {
            var userName = Login1.UserName;// System.Configuration.ConfigurationManager.AppSettings["userName"];

            var userPW = Login1.Password;//System.Configuration.ConfigurationManager.AppSettings["userPW"];

            string sql = "select * from email.hm_accounts where accountaddress=@accountaddress ";

            DataTransaction data = EmailHelper.GetConnection(userName);
          
            List<MySqlParameter> parameters = new List<MySqlParameter>()
            {
					new MySqlParameter("@accountaddress", MySqlDbType.VarChar,200) 
            };
            parameters[0].Value = userName;
            var source = data.Query(sql, parameters.ToArray());

            if (source.Tables[0].Rows.Count > 0)
            {
                var currentPW = source.Tables[0].Rows[0]["accountpassword"] + "";
                var key = currentPW.Substring(0, 6);
                var pw = GetSHA256(key + userPW);
                if (currentPW.Equals(key + pw))
                {
                    e.Authenticated = true;
                    LoginUserInfo info = new LoginUserInfo()
                    {
                        AccountId = Convert.ToInt32(source.Tables[0].Rows[0]["accountid"]),
                        EmailAddress = userName
                    };
                    Session["info"] = info;
                    Response.Redirect("CXEmaiList.aspx");
                }
                else
                {
                    e.Authenticated = false;
                }
            }
            else
            {
                e.Authenticated = false;
            }
        }

        private string GetSHA256(string text)
        {
            byte[] hashValue;
            byte[] message = Encoding.UTF8.GetBytes(text);

            SHA256Managed hashString = new SHA256Managed();
            string hex = "";

            hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }
            return hex;
        }
    }


    public class EmailHelper 
    {
        public static RxMailMessage GetEmaiLStream(string email, string fileName, bool isOther)
        {
            if (isOther)
            {
                var temp = fileName.Substring(1, 2);
                string url = string.Format("http://120.55.81.17:7548/{0}/{1}/{2}/{3}",
                   EmailHelper.GetDomain(email), EmailHelper.GetName(email), temp, fileName);
                WebClient client = new WebClient();
                var data = client.DownloadData(url);
                System.IO.MemoryStream stream = new System.IO.MemoryStream(data, 0, data.Length);
                MimeReader readMail = new MimeReader();

                return readMail.GetEmail(stream);
            }
            else
            {
                MimeReader readMail = new MimeReader();

                return readMail.GetEmail(fileName);
            }
        }
        /// <summary>
        /// 获取域名
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static string GetDomain(string email)
        {
            var index = email.IndexOf("@");

            var currentDomain = email.Substring(index, email.Length - index).Replace("@", "");

            return currentDomain;
        }

        /// <summary>
        /// 获取姓名
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static string GetName(string email)
        {
            var index = email.IndexOf("@");

            var currentName = email.Substring(0, index);

            return currentName;
        }
        public static bool IsOtherEmail(string userName) 
        {
            var index = userName.IndexOf("@");

            var currentDomain = userName.Substring(index, userName.Length - index).Replace("@", "");

            var otherDomain = System.Configuration.ConfigurationManager.AppSettings["otherDomain"];

            bool isOtherDomain = otherDomain.Contains(currentDomain);
            return isOtherDomain;          
        }

        public static DataTransaction GetConnection(string userName)
        {
            if (IsOtherEmail(userName))
            {
               return PD.Business.DataTransaction.Create("otherDomainCon");
            }
            else
            {
                return PD.Business.DataTransaction.Create();
            }
        }
    }
    public class LoginUserInfo {
        public int AccountId { get; set; }
        public string EmailAddress { get; set; }

        private static LoginUserInfo current = null;

        public static LoginUserInfo Current
        {
            get {
                return HttpContext.Current.Session["info"] as LoginUserInfo;
            }
            set { LoginUserInfo.current = value; }
        }
       
    }
}