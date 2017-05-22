 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using MyDB;

namespace AccountRegister
{
    public partial class TaskStatisticsPage : Form
    {
        public TaskStatisticsPage()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
                string s = @"  select d,d1,d2,d1+d2 d3 from 
 ( 
 select  SUBSTRING( CONVERT(varchar(100), CreateTime, 20) ,0,11) d,
 SUM(CASE IsEmailActivation WHEN 1 THEN 1 ELSE 0 END) d1 ,
  SUM(CASE IsEmailActivation WHEN 5 THEN 1 ELSE 0 END) d2 
 from 
  T_NewRegisterAccount
group by  
SUBSTRING( CONVERT(varchar(100), CreateTime, 20) ,0,11)
 ) 
d order by d.d";
 
                var data = DoData.Get(s, "system");
 
                 dataGridView1.DataSource = data;
  
        }
    }
}
