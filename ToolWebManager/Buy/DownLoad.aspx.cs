using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ToolWebManager.Buy;

namespace ToolWebManager
{
    public partial class DownLoad : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) 
            {
                string name = Request["name"];

                Response.ContentType = "application/ms-excel";

                string file = HttpUtility.UrlEncode(name);

                Response.AddHeader("Content-Disposition", "attachment;filename=" + file);

                string filename = Server.MapPath("DownLoad/" + SellBusiness.Current.SellBusinessId + "/" + name);

                //Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");

                Response.TransmitFile(filename);

                Response.Flush();

               Response.Close();
            }
        }
    }
}