using MyEntiry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MyTool.Statistics
{
    public partial class ResourceDetailPage : Form
    {
        public ResourceDetailPage()
        {
            InitializeComponent();
            List<CustomComboxItem> source = new List<CustomComboxItem>();
            source.Add(new CustomComboxItem { Value = "0", Name = "新注册" });
            source.Add(new CustomComboxItem { Value = "1", Name = "通过邮箱核验" });
            source.Add(new CustomComboxItem { Value = "7", Name = "正在核验邮箱" });
            source.Add(new CustomComboxItem { Value = "5", Name = "未核验" });
            source.Add(new CustomComboxItem { Value = "2", Name = "已核验" });
            source.Add(new CustomComboxItem { Value = "3", Name = "已出" });
            source.Add(new CustomComboxItem { Value = "4", Name = "已出未核验" });
            source.Add(new CustomComboxItem { Value = "6", Name = "已出且验证" });
            source.Add(new CustomComboxItem { Value = "8", Name = "损坏账号" });
            source.Add(new CustomComboxItem { Value = "9", Name = "损坏账号已回收" });
            //0 新注册 1通过邮箱核验 2 已通过验证 3已出 4已出未核验 5未核验 6已出且验证
            comboBox1.DisplayMember = "Name";
            comboBox1.ValueMember = "Value";
            comboBox1.DataSource = source;
            comboBox1.SelectedIndex = 0;
            Load += ResourceDetailPage_Load;
        }

        void ResourceDetailPage_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
        }

        void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Query();
        }

        public string Date { get; set; }

        private void button1_Click(object sender, EventArgs e)
        {
            Query();
        }

        private void Query()
        {
            var state = (comboBox1.SelectedItem as CustomComboxItem).Value;
            var data = PD.Business.DataTransaction.Create();
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = data.DoGetDataTable(string.Format(@"select username,password from t_newaccount where createtime>='{0} 00:00:01' and createTime<='{0} 23:59:59' and state={1}",
                  Date, state));
        }
    }
}
