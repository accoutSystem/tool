using MyEntiry;
using MyTool.Common;
using PD.Business;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ResourceBulidTool
{
    public class GetPassengerServer
    {
        public int i = 0;

        public static List<T_Passenger> Get(int count)
        {
            return Get(count, 0);
        }

        public static List<T_Passenger> Get(int count,int type)
        {
            return Get(count, type, false);
        }

        public static List<T_Passenger> GetAll(int count, int type )
        {
            var passengers = new List<T_Passenger>();

            var data = DataTransaction.Create();

            string sql = string.Empty;

            var move = System.Configuration.ConfigurationManager.AppSettings["movePassenger"];

            sql = "select * from t_passenger where state=" + type + " limit 0," + count;

            var source = data.DoGetDataTable(sql);

            if (source.Rows.Count > 0)
            {
                foreach (DataRow row in source.Rows)
                {
                    string passengerId = row["passengerId"] + string.Empty;
                    string idno = row["idno"] + string.Empty;
                    try
                    {
                        var birthday = Convert.ToDateTime(ToolCommonMethod.GetBirthdayIDNoTo(idno));

                        var ss = (DateTime.Now - birthday);

                        if (ss.Days / 365 >= 18)
                        {
                            var currentPassenger = new T_Passenger
                            {
                                IdNo = row["idno"] + string.Empty,
                                Name = row["name"] + string.Empty,
                                State = Convert.ToInt32(row["state"] + string.Empty),
                                PassengerId = passengerId,
                            };
                            passengers.Add(currentPassenger);
                        }
                        else
                        {
                            try
                            {
                                data.ExecuteSql("update t_passenger set state=1 where passengerid='" + passengerId + "' ");
                            }
                            catch
                            {
                            }
                        }
                    }
                    catch
                    {
                        data.ExecuteSql("update t_passenger set state=1 where passengerid='" + passengerId + "'");
                    }
                }
            }
            return passengers;

        }

        public static List<T_Passenger> Get(int count, int type, bool isNew,string move)
        {
            var passengers = new List<T_Passenger>();

            var data = DataTransaction.Create();

            string sql = string.Empty;

           

            sql = "select * from t_passenger where state=" + type + " and move=" + move + " limit 0," + count;

            var source = data.DoGetDataTable(sql);

            if (source.Rows.Count > 0)
            {
                foreach (DataRow row in source.Rows)
                {
                    string passengerId = row["passengerId"] + string.Empty;
                    string idno = row["idno"] + string.Empty;
                    try
                    {

                        var birthday = Convert.ToDateTime(ToolCommonMethod.GetBirthdayIDNoTo(idno));

                        var ss = (DateTime.Now - birthday);

                        if (ss.Days / 365 >= 18)
                        {
                            var currentPassenger = new T_Passenger
                            {
                                IdNo = row["idno"] + string.Empty,
                                Name = row["name"] + string.Empty,
                                State = Convert.ToInt32(row["state"] + string.Empty),
                                Move = row["move"] + string.Empty,
                                PassengerId = passengerId,
                            };
                            passengers.Add(currentPassenger);
                        }
                        else
                        {
                            try
                            {
                                data.ExecuteSql("update t_passenger set state=1 where passengerid='" + passengerId + "' ");
                            }
                            catch
                            {
                            }
                        }
                    }
                    catch
                    {
                        data.ExecuteSql("update t_passenger set state=1 where passengerid='" + passengerId + "'");
                    }
                }
            }

            return passengers;
        }

        public static List<T_Passenger> Get(int count,int type,bool isNew)
        {
            var move = System.Configuration.ConfigurationManager.AppSettings["movePassenger"];
            return Get(count, type, isNew, move);
        }
    }


}
