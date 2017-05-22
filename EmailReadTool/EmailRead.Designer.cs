namespace EmailReadTool
{
    partial class EmailRead
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EmailRead));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnRegEmail = new System.Windows.Forms.ToolStripButton();
            this.btnReadEmail = new System.Windows.Forms.ToolStripButton();
            this.btnConvertPassenger = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripTextBox2 = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lbAddCount1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbAddCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbBadCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbDetail = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbExcute = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.webEmailValid = new System.Windows.Forms.WebBrowser();
            this.webEmailPath = new System.Windows.Forms.WebBrowser();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnRegEmail,
            this.btnReadEmail,
            this.btnConvertPassenger,
            this.toolStripButton2,
            this.toolStripButton1,
            this.toolStripTextBox2,
            this.toolStripTextBox1,
            this.toolStripButton3});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(804, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnRegEmail
            // 
            this.btnRegEmail.Image = global::VaildTool.Properties.Resources.start;
            this.btnRegEmail.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRegEmail.Name = "btnRegEmail";
            this.btnRegEmail.Size = new System.Drawing.Size(88, 22);
            this.btnRegEmail.Text = "开始预登录";
            this.btnRegEmail.Click += new System.EventHandler(this.StartRegisterEmailClick);
            // 
            // btnReadEmail
            // 
            this.btnReadEmail.Image = global::VaildTool.Properties.Resources.start;
            this.btnReadEmail.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnReadEmail.Name = "btnReadEmail";
            this.btnReadEmail.Size = new System.Drawing.Size(100, 22);
            this.btnReadEmail.Text = "开始核验服务";
            this.btnReadEmail.Click += new System.EventHandler(this.btnReadEmail_Click);
            // 
            // btnConvertPassenger
            // 
            this.btnConvertPassenger.Image = global::VaildTool.Properties.Resources.start;
            this.btnConvertPassenger.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnConvertPassenger.Name = "btnConvertPassenger";
            this.btnConvertPassenger.Size = new System.Drawing.Size(100, 22);
            this.btnConvertPassenger.Text = "开始迁移数据";
            this.btnConvertPassenger.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.Image = global::VaildTool.Properties.Resources.success;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(73, 22);
            this.toolStripButton2.Text = "导入163";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(36, 22);
            this.toolStripButton1.Text = "拨号";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click_1);
            // 
            // toolStripTextBox2
            // 
            this.toolStripTextBox2.Name = "toolStripTextBox2";
            this.toolStripTextBox2.Size = new System.Drawing.Size(150, 25);
            // 
            // toolStripTextBox1
            // 
            this.toolStripTextBox1.Name = "toolStripTextBox1";
            this.toolStripTextBox1.Size = new System.Drawing.Size(100, 25);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton3.Text = "toolStripButton3";
            this.toolStripButton3.Click += new System.EventHandler(this.toolStripButton3_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lbAddCount1,
            this.lbAddCount,
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2,
            this.lbBadCount,
            this.lbDetail,
            this.lbExcute});
            this.statusStrip1.Location = new System.Drawing.Point(0, 281);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(804, 22);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lbAddCount1
            // 
            this.lbAddCount1.Image = global::VaildTool.Properties.Resources.success;
            this.lbAddCount1.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.lbAddCount1.Name = "lbAddCount1";
            this.lbAddCount1.Size = new System.Drawing.Size(63, 17);
            this.lbAddCount1.Text = "成功数:";
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
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Image = global::VaildTool.Properties.Resources.error;
            this.toolStripStatusLabel2.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(63, 17);
            this.toolStripStatusLabel2.Text = "失败数:";
            // 
            // lbBadCount
            // 
            this.lbBadCount.Name = "lbBadCount";
            this.lbBadCount.Size = new System.Drawing.Size(15, 17);
            this.lbBadCount.Text = "0";
            // 
            // lbDetail
            // 
            this.lbDetail.IsLink = true;
            this.lbDetail.Name = "lbDetail";
            this.lbDetail.Size = new System.Drawing.Size(32, 17);
            this.lbDetail.Text = "详情";
            this.lbDetail.Click += new System.EventHandler(this.toolStripStatusLabel3_Click);
            // 
            // lbExcute
            // 
            this.lbExcute.Name = "lbExcute";
            this.lbExcute.Size = new System.Drawing.Size(56, 17);
            this.lbExcute.Text = "运行步骤";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 25);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(804, 256);
            this.tabControl1.TabIndex = 9;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.textBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(796, 230);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "运行消息";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.InfoText;
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.ForeColor = System.Drawing.SystemColors.Info;
            this.textBox1.Location = new System.Drawing.Point(3, 3);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(790, 224);
            this.textBox1.TabIndex = 8;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tableLayoutPanel1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(796, 230);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "核验消息";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.webEmailValid, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.webEmailPath, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(790, 224);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // webEmailValid
            // 
            this.webEmailValid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webEmailValid.Location = new System.Drawing.Point(398, 3);
            this.webEmailValid.MinimumSize = new System.Drawing.Size(20, 20);
            this.webEmailValid.Name = "webEmailValid";
            this.webEmailValid.Size = new System.Drawing.Size(389, 218);
            this.webEmailValid.TabIndex = 6;
            // 
            // webEmailPath
            // 
            this.webEmailPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webEmailPath.Location = new System.Drawing.Point(3, 3);
            this.webEmailPath.MinimumSize = new System.Drawing.Size(20, 20);
            this.webEmailPath.Name = "webEmailPath";
            this.webEmailPath.Size = new System.Drawing.Size(389, 218);
            this.webEmailPath.TabIndex = 2;
            // 
            // EmailRead
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(804, 303);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "EmailRead";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "核验管理服务";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EmailRead_FormClosing);
            this.Load += new System.EventHandler(this.EmailRead_Load_1);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnRegEmail;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lbAddCount1;
        private System.Windows.Forms.ToolStripStatusLabel lbAddCount;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel lbBadCount;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ToolStripButton btnReadEmail;
        private System.Windows.Forms.ToolStripStatusLabel lbDetail;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.WebBrowser webEmailPath;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox2;
        private System.Windows.Forms.ToolStripStatusLabel lbExcute;
        private System.Windows.Forms.ToolStripButton btnConvertPassenger;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.WebBrowser webEmailValid;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
    }
}

