using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using ToolWebManager.Buy;

namespace ToolWebManager.Base
{
    public class SellBasePage : Page
    {
        protected override void OnLoad(EventArgs e)
        {
        
            if (SellBusiness.Current == null)
            {
                Response.Redirect("CXSellLogin.aspx");
            } base.OnLoad(e);
        }
    }
}