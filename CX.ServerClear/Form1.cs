using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CX.ServerClear
{
    public partial class Form1 : Form
    {
        bool isRun = false;

        DateTime deleteTime = DateTime.Now;

        private int successCount;

        public int SuccessCount
        {
            get { return successCount; }
            set { successCount = value;
            Invoke(new Action(() => { toolStripStatusLabel1.Text = value.ToString(); }));
            }
        }

        private int errorCount;

        public int ErrorCount
        {
            get { return errorCount; }
            set { errorCount = value;
            Invoke(new Action(() => { toolStripStatusLabel2.Text = value.ToString(); }));
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (isRun)
            {
                button1.Text = "开始";
                isRun = false;
                return;
            }
            else 
            {
                button1.Text = "结束";
                isRun = true;
            }
            var basePath=textBox2.Text;
            deleteTime = dateTimePicker1.Value;
            if (MessageBox.Show("确定删除?", "", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.Cancel)
            {
                button1_Click(null, null);
                return;
            }
            Thread th = new Thread(new ThreadStart(() =>
            {
                System.IO.DirectoryInfo info = new System.IO.DirectoryInfo(basePath);
                DeleteDic(info);
                Invoke(new Action(() =>
                {
                    button1.Text = "开始";
                    isRun = false;
                }));
            }));
            th.Start();
        }

        private void DeleteDic(DirectoryInfo path) 
        {
            path.GetFiles().ToList().ForEach(file =>
            {
                if (isRun)
                {
                    DeleteFile(file);
                }
            });

            path.GetDirectories().ToList().ForEach(dic =>
            {
                if (isRun) 
                {
                    WriteMessage("开始分析目录" + dic.FullName);

                    DeleteDic(dic);

                    WriteMessage("分析目录" + dic.FullName+"完成");
                }
            });

            if (path.GetFiles().Length == 0 && path.GetDirectories().Length == 0)
            {
                WriteMessage("开始删除" + path.FullName);
                try
                {
                    path.Delete();
                    SuccessCount++;
                    WriteMessage("删除" + path.FullName + "成功");
                }
                catch (Exception ex)
                {
                    ErrorCount++;
                    WriteMessage("删除" + path.FullName + "失败"+ex.Message);
                }
            }
        }

        private void DeleteFile(FileInfo file)
        {
            WriteMessage("开始删除" + file.FullName);

            try
            {
                if (file.CreationTime <= deleteTime && System.IO.Path.GetExtension(file.FullName).Contains("eml"))
                {
                    file.Delete();
                    SuccessCount++;
                    WriteMessage("删除成功:" + file.FullName+" 创建时间"+file.CreationTime.ToString("yyyy-MM-dd"));
                }
                else
                {
                    WriteMessage("不符合删除条件:" + file.FullName);
                }
            }
            catch (Exception ex)
            {
                ErrorCount++;
                WriteMessage("删除文件失败:" + ex.Message);
            }
            Thread.Sleep(5);
        }

        int count = 0;

        private void WriteMessage(string message)
        {
            if (isShow == false)
                return;
            this.Invoke(new Action(() =>
            {
                count++;

                if (count == 100)
                {
                    count = 0;
                    textBox1.Text = string.Empty;
                }
                if (message.Length > 200)
                {
                    message = message.Substring(0, 200) + "...";
                }
                textBox1.Text += "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":" + message;

                textBox1.SelectionStart = this.textBox1.Text.Length;

                textBox1.ScrollToCaret();
            }));
        }
        bool isShow = true;
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            isShow = checkBox1.Checked;
        }
    }
}
