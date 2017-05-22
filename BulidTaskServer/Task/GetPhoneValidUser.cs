using MyEntiry;
using PD.Business;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BulidTaskServer
{
    public class GetPhoneValidUser
    {
        public int i = 0;

        public static List<T_ValidUser> Get(int count,int acountType)
        {
            var users = new List<T_ValidUser>();

            var data = DataTransaction.Create();

            string sql = "select * from t_newaccount where state=2  and accountType=" + acountType + " limit 0," + count;

            var source = data.DoGetDataTable(sql);

            if (source.Rows.Count > 0)
            {
                foreach (DataRow row in source.Rows)
                {
                    var currentPassenger = new T_ValidUser
                    {
                        Email = row["Email"] + string.Empty,
                        UserName = row["username"] + string.Empty,
                        UserGuid = row["UserGuid"] + string.Empty,
                        PassWord = row["password"] + string.Empty,
                    };
                    users.Add(currentPassenger);
                }
            }

            return users;
        }
    }
}
