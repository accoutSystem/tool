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
    public partial class ShowUserPage : Form
    {
        public ShowUserPage()
        {
            InitializeComponent();
        }

        private void ShowUserPage_Load(object sender, EventArgs e)
        {

        }

        public void SetSource(List<AddPassengerUser> source) {
            dataGridView1.DataSource = source;
        }
    }
}
