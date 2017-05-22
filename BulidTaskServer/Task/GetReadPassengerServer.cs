using MyEntiry;
using PD.Business;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BulidTaskServer.Task
{
  public class GetReadPassengerServer
    {

      private static DateTime CurrentDate = DateTime.MinValue; 
      internal static List<T_ValidUser> Get(int addCount)
      { 
          var passengers = new List<T_ValidUser>();

          try
          {
              var data = DataTransaction.Create();

              var date = System.Configuration.ConfigurationManager.AppSettings["readPassengerMinDate"];

              var historyDate = DateTime.Parse(date);

              var countDay = (DateTime.Now - historyDate).Days;

              DataTable source = null;

              for (int i = 0; i < countDay; i++)
              {
                  var currentDate = historyDate.AddDays(i);

                  if (currentDate >= CurrentDate)
                  {
                      string sql = string.Format(@" select * from t_hisnewaccount where state=12
 and readPassengerState=0 and
 createtime>'{0} 00:00:01'  and createtime<='{0} 23:23:01'
 limit 0, {1} ", currentDate.ToString("yyyy-MM-dd"), addCount);

                      source = data.DoGetDataTable(sql);

                      if (source.Rows.Count > 0)
                      {
                          CurrentDate = currentDate;
                          break;
                      }
                  }
              }

              if (source.Rows.Count > 0)
              {
                  foreach (DataRow row in source.Rows)
                  {
                      var currentPassenger = new T_ValidUser
                      {
                          UserName = row["username"] + string.Empty,
                          UserGuid = row["UserGuid"] + string.Empty,
                          PassWord = row["password"] + string.Empty,
                      };
                      passengers.Add(currentPassenger);
                  }
              }
          }
          catch
          {

          }

          return passengers;
      }



    }
}
