using CaptchaServerCacheClient;
using Maticsoft.Model;
using MyEntiry;
using PD.Business;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ResourceBulidTool
{
    public class GetAccountServer
    {
        public static int index = 0;

        public static List<string> domains = new List<string>();

        public static List<QueueAccount> GetData(int count)
        {
            List<QueueAccount> emailSource = new List<QueueAccount>();

            var data = DataTransaction.Create();

            string sql = "select username, password from t_hisnewaccount where state=24 limit 0," + count;

            var source = data.DoGetDataTable(sql);

            if (source.Rows.Count > 0)
            {
                foreach (DataRow row in source.Rows)
                {
                    emailSource.Add(new QueueAccount
                   {
                       UserName = row["username"] + string.Empty,
                       Password = row["password"] + string.Empty,
                   });
                }
            }
            return emailSource;
        }

     
    }
}
