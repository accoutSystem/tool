using PD.Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BulidTaskServer.Page
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var data = DataTransaction.Create();
            var start =dateTimePicker1.Value.ToString("yyyy-MM-dd")+ " 00:00:01";
            var end = dateTimePicker2.Value.ToString("yyyy-MM-dd") + " 23:59:59";
            data.ExecuteSql("update t_hisnewaccount set readPassengerState=0,LastTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where  createtime>'" + start + "' and createtime<='"+end+"'");

        }
    }
}
