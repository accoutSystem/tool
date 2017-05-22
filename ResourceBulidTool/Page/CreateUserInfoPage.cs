using ResourceBulidTool.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ResourceBulidTool
{
    public partial class CreateUserInfoPage : Form
    {
        public CreateUserInfoPage()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("信息不能为空");
                return;
            }

            textBox1.Lines.ToList().ForEach(line =>
            {
                var user = line.Split(',');

                CustomUserCollection.Current.Add(new CustomUserItem
                {
                    State = 0,
                    UserName = user[0],
                    PassWord = user[1]
                });
            });
            this.Close();
        }
    }
}
