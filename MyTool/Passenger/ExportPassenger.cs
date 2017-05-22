using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MyTool.Passenger
{
    public partial class ExportPassenger : Form
    {
        public ExportPassenger()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int count = Convert.ToInt32(textBox1.Text);

            Thread th = new Thread(new ThreadStart(() =>
            {
                var data = PD.Business.DataTransaction.Create();

                var dataCountTable = data.Query(@"select count(*) c from t_passenger where state=0").Tables[0];

                var dataCount = Convert.ToInt32(dataCountTable.Rows[0]["c"]);

                string path = System.Environment.CurrentDirectory + @"\Data\" + DateTime.Now.ToString("yyyy-MM-dd HH mm") + ".txt";
                
                if (dataCount > count)
                {
                    var passengerData = data.Query("select * from t_passenger where state=0 limit 0,"+count).Tables[0];

                    foreach (DataRow row in passengerData.Rows)
                    {
                        string guid = row["passengerId"] + string.Empty;
                        string passengerName = row["name"] + string.Empty;
                        string passengerId = row["idNo"] + string.Empty;
                        try
                        {
                            data.ExecuteSql("update t_passenger set state=1 where passengerid='" + guid + "' ");
                            Storage(passengerName, passengerId, path);
                        }
                        catch { }
                    }
                }
                else 
                {
                    MessageBox.Show("身份证数量不足");
                }

            }));

            th.Start();
        }
       

        private void Storage(string passengerName,string passengerId, string path)
        {
            WriteMessage(passengerName + " " + passengerId + "写入文件");

            try
            {

                FileStream fs = new FileStream(path, FileMode.Append);

                StreamWriter sw = new StreamWriter(fs);

                sw.WriteLine(passengerName + "," + passengerId);

                sw.Close();

                sw.Dispose();

                WriteMessage(passengerName + " " + passengerId + "写入文件成功");
            }
            catch (Exception ex)
            {
                WriteMessage(passengerName + " " + passengerId + "写入文件失败" + ex.Message);
            }
        }
        int messageCount = 0;
        private void WriteMessage(string message)
        {
            this.Invoke(new Action(() =>
            {
                messageCount++;
                if (messageCount == 20)
                {
                    messageCount = 0;
                    textBox2.Text = string.Empty;
                }
                textBox2.Text += "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":" + message;
            }));
        }

    }
}
