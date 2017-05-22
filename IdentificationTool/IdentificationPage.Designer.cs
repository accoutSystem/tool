namespace IdentificationTool
{
    partial class IdentificationPage
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IdentificationPage));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnAddID = new System.Windows.Forms.ToolStripButton();
            this.btnAddBadID = new System.Windows.Forms.ToolStripButton();
            this.btnConvertPassenger = new System.Windows.Forms.ToolStripButton();
            this.btnAC = new System.Windows.Forms.ToolStripButton();
            this.btnMessage = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbcount = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel5 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbAddCount1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbAddCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbBadCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbSum = new System.Windows.Forms.ToolStripStatusLabel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btnAnysic = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAddID,
            this.btnAddBadID,
            this.btnConvertPassenger,
            this.btnAC,
            this.btnAnysic,
            this.btnMessage});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(637, 25);
            this.toolStrip1.TabIndex = 10;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnAddID
            // 
            this.btnAddID.Image = global::IdentificationTool.Properties.Resources.start;
            this.btnAddID.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddID.Name = "btnAddID";
            this.btnAddID.Size = new System.Drawing.Size(88, 22);
            this.btnAddID.Text = "导入身份证";
            this.btnAddID.Click += new System.EventHandler(this.AddGoodPassengerClick);
            // 
            // btnAddBadID
            // 
            this.btnAddBadID.Image = global::IdentificationTool.Properties.Resources.start;
            this.btnAddBadID.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddBadID.Name = "btnAddBadID";
            this.btnAddBadID.Size = new System.Drawing.Size(112, 22);
            this.btnAddBadID.Text = "导入错误身份证";
            this.btnAddBadID.Click += new System.EventHandler(this.AddBadPassengerClick);
            // 
            // btnConvertPassenger
            // 
            this.btnConvertPassenger.Image = global::IdentificationTool.Properties.Resources.start;
            this.btnConvertPassenger.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnConvertPassenger.Name = "btnConvertPassenger";
            this.btnConvertPassenger.Size = new System.Drawing.Size(100, 22);
            this.btnConvertPassenger.Text = "开始迁移数据";
            this.btnConvertPassenger.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // btnAC
            // 
            this.btnAC.Image = global::IdentificationTool.Properties.Resources.start;
            this.btnAC.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAC.Name = "btnAC";
            this.btnAC.Size = new System.Drawing.Size(76, 22);
            this.btnAC.Text = "开始去重";
            this.btnAC.Click += new System.EventHandler(this.toolStripButton1_Click_2);
            // 
            // btnMessage
            // 
            this.btnMessage.Image = global::IdentificationTool.Properties.Resources.stop;
            this.btnMessage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnMessage.Name = "btnMessage";
            this.btnMessage.Size = new System.Drawing.Size(76, 22);
            this.btnMessage.Text = "关闭消息";
            this.btnMessage.Click += new System.EventHandler(this.toolStripButton1_Click_1);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel2,
            this.lbcount,
            this.toolStripStatusLabel5,
            this.lbAddCount1,
            this.lbAddCount,
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel3,
            this.lbBadCount,
            this.lbSum});
            this.statusStrip1.Location = new System.Drawing.Point(0, 226);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(637, 22);
            this.statusStrip1.TabIndex = 11;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Image = global::IdentificationTool.Properties.Resources.count;
            this.toolStripStatusLabel2.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(75, 17);
            this.toolStripStatusLabel2.Text = "执行总数:";
            // 
            // lbcount
            // 
            this.lbcount.Name = "lbcount";
            this.lbcount.Size = new System.Drawing.Size(15, 17);
            this.lbcount.Text = "0";
            // 
            // toolStripStatusLabel5
            // 
            this.toolStripStatusLabel5.Name = "toolStripStatusLabel5";
            this.toolStripStatusLabel5.Size = new System.Drawing.Size(11, 17);
            this.toolStripStatusLabel5.Text = "|";
            // 
            // lbAddCount1
            // 
            this.lbAddCount1.Image = global::IdentificationTool.Properties.Resources.success;
            this.lbAddCount1.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.lbAddCount1.Name = "lbAddCount1";
            this.lbAddCount1.Size = new System.Drawing.Size(87, 17);
            this.lbAddCount1.Text = "执行成功数:";
            // 
            // lbAddCount
            // 
            this.lbAddCount.Name = "lbAddCount";
            this.lbAddCount.Size = new System.Drawing.Size(15, 17);
            this.lbAddCount.Text = "0";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(11, 17);
            this.toolStripStatusLabel1.Text = "|";
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Image = global::IdentificationTool.Properties.Resources.error;
            this.toolStripStatusLabel3.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(63, 17);
            this.toolStripStatusLabel3.Text = "重复数:";
            // 
            // lbBadCount
            // 
            this.lbBadCount.Name = "lbBadCount";
            this.lbBadCount.Size = new System.Drawing.Size(15, 17);
            this.lbBadCount.Text = "0";
            // 
            // lbSum
            // 
            this.lbSum.IsLink = true;
            this.lbSum.Name = "lbSum";
            this.lbSum.Size = new System.Drawing.Size(101, 17);
            this.lbSum.Text = "总数:可用0 已用0";
            this.lbSum.Click += new System.EventHandler(this.lbSum_Click);
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.InfoText;
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.ForeColor = System.Drawing.Color.White;
            this.textBox1.Location = new System.Drawing.Point(0, 25);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(637, 201);
            this.textBox1.TabIndex = 12;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // btnAnysic
            // 
            this.btnAnysic.Image = global::IdentificationTool.Properties.Resources.start;
            this.btnAnysic.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAnysic.Name = "btnAnysic";
            this.btnAnysic.Size = new System.Drawing.Size(88, 22);
            this.btnAnysic.Text = "分析身份证";
            this.btnAnysic.Click += new System.EventHandler(this.toolStripButton1_Click_3);
            // 
            // IdentificationPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(637, 248);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "IdentificationPage";
            this.Text = "身份证管理";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnAddID;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lbAddCount1;
        private System.Windows.Forms.ToolStripStatusLabel lbAddCount;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel lbBadCount;
        private System.Windows.Forms.ToolStripButton btnAddBadID;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripButton btnMessage;
        private System.Windows.Forms.ToolStripStatusLabel lbSum;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ToolStripStatusLabel lbcount;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel5;
        private System.Windows.Forms.ToolStripButton btnConvertPassenger;
        private System.Windows.Forms.ToolStripButton btnAC;
        private System.Windows.Forms.ToolStripButton btnAnysic;
    }
}

