using FangBian.Common;
using PD.Business;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Serialization;

namespace ToolWebManager
{
    public partial class UserDetail : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Query();
                Query1();
                Query2();
                Query3(); 
                Query5();
                Query6();
                Query7();
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Query();
        }

        void Query()
        {
            try
            {
                var source = DeserializeDataTable(HttpHelper.Get("http://121.43.110.247/BaseServer/api/getstatus.ashx?taskid=userDetail"));
                GridView1.DataSource = source;
                GridView1.DataBind();
            }
            catch
            {

            }
        }



        void Query1()
        {

            try
            {
                var source = DeserializeDataTable(HttpHelper.Get("http://121.43.110.247/BaseServer/api/getstatus.ashx?taskid=userTotal"));
                GridView2.DataSource = source;
                GridView2.DataBind();
            }
            catch
            {

            }
        }

        void Query5()
        {

            try
            {
                var source = DeserializeDataTable(HttpHelper.Get("http://121.43.110.247/BaseServer/api/getstatus.ashx?taskid=userTotalInLH"));
                GridView5.DataSource = source;
                GridView5.DataBind();
            }
            catch
            {

            }
        }

        void Query6()
        {

            try
            {
                var source = DeserializeDataTable(HttpHelper.Get("http://121.43.110.247/BaseServer/api/getstatus.ashx?taskid=userTotalInPW"));
                GridView6.DataSource = source;
                GridView6.DataBind();
            }
            catch
            {

            }
        }
        void Query7()
        {

            try
            {
                var source = DeserializeDataTable(HttpHelper.Get("http://121.43.110.247/BaseServer/api/getstatus.ashx?taskid=userTotalInWMY"));
              GridView7.DataSource = source;
              GridView7.DataBind();
            }
            catch
            {

            }
        }

        void Query2()
        {

            try
            {
                var source = DeserializeDataTable(HttpHelper.Get("http://121.43.110.247/BaseServer/api/getstatus.ashx?taskid=passengerCache"));
                GridView3.DataSource = source;
                GridView3.DataBind();
            }
            catch
            {

            }
        }
        void Query3()
        {

            try
            {
                var source = DeserializeDataTable(HttpHelper.Get("http://121.43.110.247/BaseServer/api/getstatus.ashx?taskid=emailCache"));
                GridView4.DataSource = source;
                GridView4.DataBind();
            }
            catch
            {

            }
        }
        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes.Add("onmouseover", "MouseOver(this)");
                //当鼠标移开时还原背景色
                e.Row.Attributes.Add("onmouseout", "MuseOut(this)");
            }
        }
        public static DataTable DeserializeDataTable(string pXml)
        {

            StringReader strReader = new StringReader(pXml);
            XmlReader xmlReader = XmlReader.Create(strReader);
            XmlSerializer serializer = new XmlSerializer(typeof(DataTable));

            DataTable dt = serializer.Deserialize(xmlReader) as DataTable;

            return dt;
        }
        protected void Button2_Click(object sender, EventArgs e)
        {
            Query1();
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            Query2();
        }

        protected void Button4_Click(object sender, EventArgs e)
        {
            Query3();
        }

        protected void Button5_Click(object sender, EventArgs e)
        {
            Query5();
        }

        protected void Button6_Click(object sender, EventArgs e)
        {
            Query6();
        }

        protected void Button7_Click(object sender, EventArgs e)
        {
            Query7();
        }

        protected void Button8_Click(object sender, EventArgs e)
        {
            var db = PD.Business.DataTransaction.Create();
            db.ExecuteSql("update t_newaccount set state=10 where state=0");
        }
    }
}