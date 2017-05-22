using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ToolWebManager.Free
{
    public partial class GetUser : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            var pw=this.TextBox1.Text;

            if (string.IsNullOrEmpty(pw))
            {
                Message("提取码为空");
                return;
            }

            if (pw.Length != 8)
            {
                Message("提取码长度错误");
                return;
            }

            var data = PD.Business.DataTransaction.Create();

            var source = data.Query("select * from t_freeaccount where GetCode='" + pw + "'").Tables[0];

            if (source.Rows.Count >0)
            {
                if ((source.Rows[0]["state"] + "").Equals("0"))
                {
                    lbInfo.Text = "恭喜你成功抢到："+source.Rows[0]["info"];

                    data.ExecuteSql("update t_freeaccount set state=1 where GetCode='"+pw+"' ");
                }
                else
                {
                    lbInfo.Text = "提取码已被手快的牛牛们提取了";
                }
            }
            else
            {
                lbInfo.Text = "提取码不存在";
            }
        }

        public void Message(string message)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "info", "alert('" + message + "');", true);
        }
    }
}