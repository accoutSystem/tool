using ResourceBulidTool.Entity;
using ResourceBulidTool.Page;
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
    public partial class SelectBulidType : Form
    {
        public BulidType BulidType { get; set; }
        public SelectBulidType()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (rbDefault.Checked)
            {
                BulidType = ResourceBulidTool.BulidType.Default;
                this.Close();
            }
            if (rbSelect.Checked)
            {
                BulidType = ResourceBulidTool.BulidType.AppointUser;

                CustomUserCollection.Current.Clear();

                CreateUserInfoPage page = new CreateUserInfoPage();

                page.ShowDialog();

                if (CustomUserCollection.Current.Count <= 0)
                {
                    MessageBox.Show("没有填写用户信息");
                }
                else
                {
                    this.Close();
                }
            }
            if (rbLH.Checked)
            {
                BulidType = ResourceBulidTool.BulidType.Continuity;

                //CustomUserCollection.Current.Clear();

                //SelectContinuityPage page = new SelectContinuityPage();

                //page.ShowDialog();

                //if (CustomUserCollection.Current.Count <= 0)
                //{
                //    MessageBox.Show("没有填写用户信息");
                //}
                //else
                //{  //}
                    this.Close();
              
            }

            if (radioButton1.Checked)
            {
                BulidType = ResourceBulidTool.BulidType.PW;

                this.Close();
            }

        }

        private void rbSelect_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
    public enum BulidType
    {
        /// <summary>
        /// 默认类型
        /// </summary>
        Default = 0,
        /// <summary>
        /// 指定用户
        /// </summary>
        AppointUser = 1,
        /// <summary>
        /// 连号
        /// </summary>
        Continuity = 2,

        /// <summary>
        /// 密码一样
        /// </summary>
        PW = 3,
    }
}
