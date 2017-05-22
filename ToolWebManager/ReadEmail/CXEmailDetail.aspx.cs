using glib.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ToolWebManager.Email
{
    public partial class CXEmailDetail : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Query();
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Query();
        }

        public void Query()
        {
            try
            {
                string sql = "select messagefilename from email.hm_messages where   messageid='" + Request["messageid"] + "'  ";

                var data = EmailHelper.GetConnection(LoginUserInfo.Current.EmailAddress);

                var source = data.DoGetDataTable(sql);
                bool isOtherEmail = EmailHelper.IsOtherEmail(LoginUserInfo.Current.EmailAddress);
                if (source.Rows.Count > 0)
                {
                    var file = source.Rows[0]["messagefilename"] + string.Empty;

                    var email = EmailHelper.GetEmaiLStream(LoginUserInfo.Current.EmailAddress, file, isOtherEmail);
                    if (email.TransferType.ToLower().Equals("base64"))
                    {
                        emailContent.InnerHtml = email.Body;
                    }
                    else
                    {
                        emailContent.InnerHtml = email.Entities[0].Body;
                    }
                }
                else
                {
                    emailContent.InnerHtml = "无邮件内容";
                }
            }
            catch
            {
                emailContent.InnerHtml = "读取邮件错误,请联系管理员";
            }
        }
    }
}