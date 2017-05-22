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

namespace WindowsFormsApplication1
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Dictionary<string, AccountItem> users = new Dictionary<string, AccountItem>();

            Thread th = new Thread(new ThreadStart(() =>
            {
                System.IO.Directory.GetDirectories(System.Environment.CurrentDirectory+@"\Data\").ToList().ForEach(currentDic =>
                {
                    System.IO.Directory.GetFiles(currentDic).ToList().ForEach(file =>
                    {
                        WriteMessage("正在分析" + file);
                        File.ReadAllLines(file).ToList().ForEach(currentUser =>
                        {
                            var sp = currentUser.Split(' ');
                            if (!users.ContainsKey(sp[0]))
                            {
                                AccountItem account = new AccountItem { UserName = sp[0], PassWord = sp[1] };
                                users.Add(account.UserName, account);
                            }
                        });
                        WriteMessage("分析" + file+"完成");
                    });
                });
                WriteMessage("分析完成");
                WriteFile(users.Values.ToList());
                users.Clear();
            }));
            th.Start();
        }

        private void WriteFile(List<AccountItem> source)
        {
            if (source.Count <= 0)
                return;
            WriteMessage("写入数据文件");

            string path = System.Environment.CurrentDirectory + @"\Data\" + DateTime.Now.ToString("yyyy-MM-dd") + "去重合并.txt";

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
                if (count == 200)
                {
                    count = 0;
                    textBox1.Text = string.Empty;
                }
                textBox1.Text += "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":" + message;
            }));
        }
    }
}
