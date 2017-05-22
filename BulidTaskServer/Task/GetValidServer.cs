using MyEntiry;
using PD.Business;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BulidTaskServer
{
    public class GetValidServer
    {
        internal static List<T_ValidUser> Get(int addCount, int accountType)
        {
            var passengers = new List<T_ValidUser>();

            var data = DataTransaction.Create();

            string sql = " select UserGuid,Email,username,password ,state from cx_user1.t_newaccount where  state=0  and accounttype=" + accountType + " limit " + addCount;

            var source = data.DoGetDataTable(sql);

            //if (source.Rows.Count <= 0)
            //{
            //    sql = "    select UserGuid,Email,username,password ,state from cx_user1.t_newaccount where  state=0 and createtime>'2015-09-28 01:26:22'   and accounttype=" + accountType + " limit " + addCount;
            //    source = data.DoGetDataTable(sql);
            //}

            if (source.Rows.Count > 0)
            {
                foreach (DataRow row in source.Rows)
                {
                    var currentPassenger = new T_ValidUser
                    {
                        Email = row["Email"] + string.Empty,
                        State = row["state"] + string.Empty,
                        UserName = row["username"] + string.Empty,
                        UserGuid = row["UserGuid"] + string.Empty,
                        PassWord = row["password"] + string.Empty,
                    };
                    passengers.Add(currentPassenger);
                }
            }

            return passengers;
        }
    }


    public class GetValidPassengerServer
    {
        internal static List<T_ValidUser> Get(int addCount )
        {
            var passengers = new List<T_ValidUser>();

            var data = DataTransaction.Create();

            string sql = " select UserGuid,Email,username,password ,state from cx_user1.t_newaccount where  state=20 limit " + addCount;

            var source = data.DoGetDataTable(sql);

            if (source.Rows.Count > 0)
            {
                foreach (DataRow row in source.Rows)
                {
                    var currentPassenger = new T_ValidUser
                    {
                        Email = row["Email"] + string.Empty,
                        State = row["state"] + string.Empty,
                        UserName = row["username"] + string.Empty,
                        UserGuid = row["UserGuid"] + string.Empty,
                        PassWord = row["password"] + string.Empty,
                    };
                    passengers.Add(currentPassenger);
                }
            }

            return passengers;
        }
    }
}
