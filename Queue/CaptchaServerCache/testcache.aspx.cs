using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CaptchaServerCache
{
    public partial class testcache : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            HttpRuntime.Cache.Add("test", "1111111111111111", null, DateTime.Now.AddSeconds(3), TimeSpan.Zero,
                CacheItemPriority.NotRemovable, onRemoveCallback);
            Label1.Text = "set ok";
        }

        private void onRemoveCallback(string key, object value, CacheItemRemovedReason reason)
        {
            if (HttpRuntime.Cache[key] != null)
                HttpRuntime.Cache.Remove(key);
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            Label1.Text = HttpRuntime.Cache["test"] + "";
        }
    }
}