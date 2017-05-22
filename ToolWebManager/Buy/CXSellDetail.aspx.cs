using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ToolWebManager.Base;

namespace ToolWebManager.Buy
{
    public partial class CXSellDetail : SellBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) {
                Query();
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Query();
        }

        public void Query() {

            string sql = String.Format(@"SELECT  idt_sell,businessid,
createtime,
sellNumber,
case when sellType=1 then '手机核验资源' else '普通资源' end sellType,
sellMoney,
storageaddress,'' downLoadAddress,'' address,'' txtDownLoadAddress 
FROM  cx_sell.t_sell
where
businessid='{0}' order by createtime desc limit 0,20", SellBusiness.Current.SellBusinessId);

            var data = PD.Business.DataTransaction.Create();

            var source = data.DoGetDataTable(sql);

            foreach (DataRow row in source.Rows)
            {
                var storageAddress = row["storageaddress"] + string.Empty;

                if (!string.IsNullOrEmpty(storageAddress))
                {

                    row["downLoadAddress"] = "DownLoad/" + SellBusiness.Current.SellBusinessId + "/" + storageAddress;
                    row["txtDownLoadAddress"] = "DownLoad/" + SellBusiness.Current.SellBusinessId + "/" + storageAddress.Replace("xls", "txt");

                   
                    row["address"] = storageAddress;
                }
            }
            source.AcceptChanges();
            GridView1.DataSource = source;
            GridView1.DataBind();
        }
    }
}