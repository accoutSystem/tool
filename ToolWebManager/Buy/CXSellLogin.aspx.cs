using MySql.Data.MySqlClient;
using PD.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ToolWebManager.Buy
{
    public partial class CXSellLogin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Login1_Authenticate(object sender, AuthenticateEventArgs e)
        {
            var userName = Login1.UserName; 

            var userPW = Login1.Password; 

            string sql = "select * from cx_sell.t_businessuser where userName=@userName and passWord=@passWord ";

            var data = PD.Business.DataTransaction.Create();

            var parameters = new List<MySqlParameter>(){
					new MySqlParameter("@userName", MySqlDbType.VarChar,45) ,
                    new MySqlParameter("@passWord", MySqlDbType.VarChar,45) 
            };
            parameters[0].Value = userName;

            parameters[1].Value = userPW;

            var source = data.Query(sql, parameters.ToArray());

            if (source.Tables[0].Rows.Count > 0)
            {
             
                var row = source.Tables[0].Rows[0];

                SellBusiness sell = new SellBusiness
                {
                    BusinessName = row["businessName"] + string.Empty,
                    PassWord = row["passWord"] + string.Empty,
                    SellBusinessId = row["idt_businessUser"] + string.Empty,
                    SellMoney = Convert.ToDecimal(row["sell"] + string.Empty),
                    UserName = row["userName"] + string.Empty,
                };

                    e.Authenticated = false;
               
                    SellBusiness.Current = sell;

                    Response.Redirect("CXSellMain.aspx");
            }
            else
            {
                e.Authenticated = false;
            }
        }
    }

    public class SellBusiness
    {
        private static SellBusiness current = null;

        public static SellBusiness Current
        {
            get
            {
                return HttpContext.Current.Session["sellInfo"] as SellBusiness;
            }
            set { HttpContext.Current.Session["sellInfo"] = value;
                SellBusiness.current = value; }
        }
        public string SellBusinessId { get; set; }

        public string BusinessName { get; set; }

        public string UserName { get; set; }

        public string PassWord { get; set; }

        public decimal SellMoney{get;set;}

        public decimal PlainPrice { get; set; }

        public decimal MobilePrice { get; set; }
    }
}