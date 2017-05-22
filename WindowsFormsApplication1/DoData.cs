using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace WindowsFormsApplication1
{
   public class DoData
    {
       public static DataTable Get(string sql, string conf)
       {
           string connStringUnUsePool = System.Configuration.ConfigurationManager.AppSettings[conf];

           using (SqlConnection connection = new SqlConnection(connStringUnUsePool))
           {
               DataSet ds = new DataSet();
               try
               {
                   string SQLString = sql;

                   connection.Open();

                   SqlDataAdapter command = new SqlDataAdapter(SQLString, connection);

                   command.Fill(ds, "ds");

                   return ds.Tables[0];
               }
               catch (System.Data.SqlClient.SqlException ex)
               {
                   throw new Exception(ex.Message);
               }
               finally
               {
                   connection.Close();
               }
           }
       }
       public static int ExecuteSql(string SQLString,string conf)
       {
           string connectionString = System.Configuration.ConfigurationManager.AppSettings[conf];
           using (SqlConnection connection = new SqlConnection(connectionString))
           {
               using (SqlCommand cmd = new SqlCommand(SQLString, connection))
               {
                   try
                   {
                       connection.Open();
                       int rows = cmd.ExecuteNonQuery();
                       return rows;
                   }
                   catch (System.Data.SqlClient.SqlException e)
                   {
                       connection.Close();
                       throw e;
                   }
               }
           }
           return -1;
       }
    }
}
