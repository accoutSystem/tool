using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AddPassengerServer
{
    public partial class DataTypePage : Form
    {
        public int Type { get; set; }
        public DataTypePage()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                Type = Convert.ToInt32(radioButton1.Tag + string.Empty);
            } 
            if (radioButton2.Checked)
            {
                Type = Convert.ToInt32(radioButton2.Tag + string.Empty);
            } 
            if (radioButton3.Checked)
            {
                Type = Convert.ToInt32(radioButton3.Tag + string.Empty);
            }
            if (radioButton4.Checked)
            {
                Type = Convert.ToInt32(radioButton4.Tag + string.Empty);
            }
            if (radioButton5.Checked)
            {
                Type = Convert.ToInt32(radioButton5.Tag + string.Empty);
            } 
            if (radioButton6.Checked)
            {
                Type = Convert.ToInt32(radioButton6.Tag + string.Empty);
            } 
            if (radioButton7.Checked)
            {
                Type = Convert.ToInt32(radioButton7.Tag + string.Empty);
            } this.Close();
           
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}