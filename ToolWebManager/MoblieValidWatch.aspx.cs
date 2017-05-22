using CaptchaServerCacheClient;
using FangBian.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ToolWebManager
{
    public partial class ValidWatch : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GridView1.DataSource = GetSource();
                GridView1.DataBind();
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            GridView1.DataSource = GetSource();
            GridView1.DataBind();
        }

        private DataTable GetSource()
        {
            CacheClient client = new CacheClient() { };

            client.Url = System.Configuration.ConfigurationManager.AppSettings["cacheServer"];
            DataTable source = new DataTable();
            source.Columns.Add("进程名称");
            source.Columns.Add("检查线程数");
          //  source.Columns.Add("乘车人账号填充情况");
            source.Columns.Add("是否检查");
            source.Columns.Add("检查成功数");
            source.Columns.Add("检查失败数");
            source.Columns.Add("检查总数");
            source.Columns.Add("正在执行任务数");
            //source.Columns.Add("所用平台");

            var sou = client.Like("AnalyPassenger_");
            var totalRow = source.NewRow();
            totalRow["进程名称"] = "汇总";
            totalRow["检查线程数"] = 0;
            totalRow["检查成功数"] = 0;
            totalRow["检查失败数"] = 0;
            totalRow["检查总数"] = 0;
            totalRow["正在执行任务数"] = 0;
           
            foreach (var key in sou)
            {
                if (string.IsNullOrEmpty(key))
                    continue;
                var watchValue = client.Get(key);
                JObject data = JObject.Parse(watchValue);
                var newRow = source.NewRow();
                newRow["进程名称"] = key;
                newRow["检查线程数"] = data["检查线程数"];
                //newRow["乘车人账号填充情况"] = data["乘车人账号填充情况"];
                newRow["是否检查"] = data["是否检查"];
                newRow["检查成功数"] = data["检查成功数"];
                newRow["检查失败数"] = data["检查失败数"];
                newRow["检查总数"] = data["检查总数"];
                newRow["正在执行任务数"] = data["正在执行任务数"];
                //newRow["所用平台"] = data["所用平台"];
                totalRow["检查线程数"] = Convert.ToInt32(totalRow["检查线程数"] + string.Empty) + Convert.ToInt32(data["检查线程数"] + string.Empty);
                totalRow["检查成功数"] = Convert.ToInt32(totalRow["检查成功数"] + string.Empty) + Convert.ToInt32(data["检查成功数"] + string.Empty);
                totalRow["检查失败数"] = Convert.ToInt32(totalRow["检查失败数"] + string.Empty) + Convert.ToInt32(data["检查失败数"] + string.Empty);
                totalRow["检查总数"] = Convert.ToInt32(totalRow["检查总数"] + string.Empty) + Convert.ToInt32(data["检查总数"] + string.Empty);
                totalRow["正在执行任务数"] = Convert.ToInt32(totalRow["正在执行任务数"] + string.Empty) + Convert.ToInt32(data["正在执行任务数"] + string.Empty);
                source.Rows.Add(newRow);
            }
            source.Rows.Add(totalRow);
            source.AcceptChanges();
            return source;
        }
    }
}