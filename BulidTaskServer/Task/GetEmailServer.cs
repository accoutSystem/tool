using MyEntiry;
using PD.Business;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ResourceBulidTool
{
    public class GetEmailServer
    {
        public static int index = 0;

        public static List<string> domains = new List<string>();

        public static List<T_Email> Get(int count)
        {
            List<T_Email> emailSource = new List<T_Email>();

            var data = DataTransaction.Create();

            //if (domains.Count <= 0)
            //{
            //    string dominConfin = System.Configuration.ConfigurationManager.AppSettings["emailDomain"];
            //    domains = dominConfin.Split(',').ToList();
            //}

            //if (index >= domains.Count)
            //    index = 0;
            //string domin = domains[index];

            //index++;
            //and email like '%" + domin + "' 
            string sql = "select * from t_email where state=0  limit 0 ," + count;

            var source = data.DoGetDataTable(sql);

            if (source.Rows.Count > 0)
            {
                foreach (DataRow row in source.Rows)
                {
                    emailSource.Add(new T_Email
                   {
                       Email = row["email"] + string.Empty,
                       EmailId = row["emailid"] + string.Empty,
                       PassWord = row["PassWord"] + string.Empty,
                   });
                }
            }
            return emailSource;
        }
    }
}
