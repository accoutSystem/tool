using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    string connStringUsePool = "server=112.124.17.153,2533;database=FB_TicketMobileSystem;uid=fbmobile;pwd=6yhnmju7#*^;pooling=true;connection lifetime=0;min pool size = 1;max pool size=50";
        //    string connStringUnUsePool = "server=112.124.17.153,2533;database=FB_TicketMobileSystem;uid=fbmobile;pwd=6yhnmju7#*^;pooling=false";

        //    int count = 50;

        //    var time = new Stopwatch();

        //    time.Start();

        //    for (int i = 0; i < count; i++)
        //    {
        //        using (SqlConnection conn = new SqlConnection(connStringUsePool))
        //        {
        //            conn.Open();
        //            conn.Close();
        //        }
        //    }
        //    time.Stop();

        //    label1.Text = "使用连接池" + time.Elapsed.TotalMilliseconds;

        //    time.Reset();

        //    time.Start();

        //    for (int i = 0; i < count; i++)
        //    {
        //        using (SqlConnection conn = new SqlConnection(connStringUnUsePool))
        //        {
        //            conn.Open();
        //            conn.Close();
        //        }
        //    }
        //    time.Stop();

        //    label2.Text = "不使用连接池" + time.Elapsed.TotalMilliseconds;
        //}
        bool start = false;

        int startId = 0;

        private void button2_Click(object sender, EventArgs e)
        {
            if (start)
            {
                start = false;
                button2.Text = "开始";
            }
            else
            {
                button2.Text = "停止";
                start = true;
            }

            int dataCount = 0;

            List<AccountItem> dataSource = new List<AccountItem>();

            Thread th = new Thread(new ThreadStart(() =>
            {
                startId = Convert.ToInt32(textBox2.Text);
                int nullSource = 0;
                while (start)
                {
                    try
                    {
                        var source = GetSource(startId, startId += 200);

                        var t = ConverSource(source);

                        dataCount++;
                        
                        dataSource.AddRange(t);

                        if (dataCount == 100) 
                        {
                            WriteFile(dataSource);

                            dataSource.Clear();

                            dataCount = 0;
                        }

                        if (source.Rows.Count <= 0)
                        {
                            nullSource++;

                            if (nullSource > 30)
                            {
                                WriteFile(dataSource);
                                WriteMessage("读取完毕数据");
                                break;
                            }
                        }
                    }
                    catch (Exception ex) 
                    {
                        startId -= 100;

                        WriteMessage("重试读取" + startId + "到" + (startId + 100) + "数据");

                        WriteMessage("异常信息:"+ex.Message);
                    }
                }
            }));
            th.Start();
        }

        private DataTable GetSource(int startId, int endId)
        {
            WriteMessage("开始读取" + startId + "到" + endId + "数据");

            string connStringUnUsePool = System.Configuration.ConfigurationManager.AppSettings["connection"];

            using (SqlConnection connection = new SqlConnection(connStringUnUsePool))
            {
                DataSet ds = new DataSet();
                try
                {
                    string SQLString = " select OrderContent from dbo.T_TicketOrder  where id> " + startId + " and ID<=" + endId;

                    connection.Open();

                    SqlDataAdapter command = new SqlDataAdapter(SQLString, connection);

                    command.Fill(ds, "ds");

                    WriteMessage("读取数据完毕");

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

        private List<AccountItem> ConverSource(DataTable source)
        {
            List<AccountItem> accounts = new List<AccountItem>();
            foreach (DataRow row in source.Rows)
            {
                accounts.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<AccountItem>(row["OrderContent"] + string.Empty));
            }
            WriteMessage("序列化数据完毕");
            return accounts;
        }

        private void WriteFile(List<AccountItem> source)
        {
            if (source.Count <= 0)
                return;
            WriteMessage("写入数据文件");

            string path = System.Environment.CurrentDirectory + @"\Data\" + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".txt";

            FileStream fs = new FileStream(path, FileMode.Create);

            StreamWriter sw = new StreamWriter(fs);

            foreach (var item in source)
            {
                if (!string.IsNullOrEmpty(item.UserName) && !string.IsNullOrEmpty(item.PassWord))
                {
                    sw.WriteLine(item.UserName + " " + item.PassWord);
                }
            }

            sw.Close();

            sw.Dispose();

            WriteMessage("写入数据文件成功");
        }
        
        int count = 0; 

        private void WriteMessage(string message) 
        {
            this.Invoke(new Action(() =>
            {
                count++;
                if (count == 20) {
                    count = 0;
                    textBox1.Text = string.Empty;
                }
                textBox1.Text += "\r\n"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+":" + message;
            }));
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            start = false;
        }

        private void Form1_StyleChanged(object sender, EventArgs e)
        {
            //if (this.WindowState == FormWindowState.Minimized)  //判断是否最小化
            //{
            //    this.ShowInTaskbar = false;  //不显示在系统任务栏
            //    notifyIcon1.Visible = true;  //托盘图标可见
            //}
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Opacity =1; this.WindowState = FormWindowState.Normal; notifyIcon1.Visible = false;  //托盘图标可见
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)  //判断是否最小化
            {
                this.Opacity = 0;
                this.ShowInTaskbar = false;  //不显示在系统任务栏
                notifyIcon1.Visible = true;  //托盘图标可见
            }
        }
    }

    public class AccountItem 
    {
        [Newtonsoft.Json.JsonProperty("username")]
        public string UserName { get; set; }
        [Newtonsoft.Json.JsonProperty("userpassword")]
        public string PassWord { get; set; }
    }
}
