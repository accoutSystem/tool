using MySql.Data.MySqlClient;
using PD.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ToolWebManager.ReadEmail
{
    public partial class CXGetEmailPW : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string pw = GetUserEmail(TextBox1.Text);
            if (string.IsNullOrEmpty(pw))
            {
                pw = GetEmail(TextBox1.Text);
            }
            if (string.IsNullOrEmpty(pw))
            {
                lbPW.Text = "没有找到邮件密码";

            }
            else
            {
                lbPW.Text = pw;
            }

        }

        private string GetUserEmail(string email)
        {
            string sql = "select * from t_useremail where email=@email ";

            DataTransaction data = PD.Business.DataTransaction.Create();

            List<MySqlParameter> parameters = new List<MySqlParameter>()
            {
					new MySqlParameter("@email", MySqlDbType.VarChar,200) 
            };
            parameters[0].Value = TextBox1.Text;
            var source = data.Query(sql, parameters.ToArray());
            if (source.Tables[0].Rows.Count > 0)
            {
                return source.Tables[0].Rows[0]["passWord"] + "";
            }
            return string.Empty;
        }

        private string GetEmail(string email)
        {
            string sql = "select Email,PassWord from  t_email where email=@email ";

            DataTransaction data = PD.Business.DataTransaction.Create();

            List<MySqlParameter> parameters = new List<MySqlParameter>()
            {
					new MySqlParameter("@email", MySqlDbType.VarChar,200) 
            };
            parameters[0].Value = TextBox1.Text;
            var source = data.Query(sql, parameters.ToArray());
            if (source.Tables[0].Rows.Count > 0)
            {
                return source.Tables[0].Rows[0]["passWord"] + "";
            }
            return string.Empty;
        }
    }
}