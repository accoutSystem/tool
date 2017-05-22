namespace BulidTaskServer
{
    partial class BulidTaskPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BulidTaskPage));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.btnCache = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.解锁邮件核验资源ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.解锁手机核验资源ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.解锁正在手机核验资源ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.出完手机核验队列ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton2,
            this.btnCache,
            this.toolStripButton3,
            this.toolStripDropDownButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(616, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            this.toolStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.toolStrip1_ItemClicked);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = global::BulidTaskServer.Properties.Resources.start;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(76, 22);
            this.toolStripButton1.Text = "开启任务";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.Image = global::BulidTaskServer.Properties.Resources.start;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(100, 22);
            this.toolStripButton2.Text = "开启核验队列";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // btnCache
            // 
            this.btnCache.Image = global::BulidTaskServer.Properties.Resources.start;
            this.btnCache.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCache.Name = "btnCache";
            this.btnCache.Size = new System.Drawing.Size(124, 22);
            this.btnCache.Text = "开始生成缓存数据";
            this.btnCache.Click += new System.EventHandler(this.toolStripButton4_Click);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.Image = global::BulidTaskServer.Properties.Resources.start;
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(100, 22);
            this.toolStripButton3.Text = "开始核验手机";
            this.toolStripButton3.Click += new System.EventHandler(this.toolStripButton3_Click);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.解锁邮件核验资源ToolStripMenuItem,
            this.解锁手机核验资源ToolStripMenuItem,
            this.解锁正在手机核验资源ToolStripMenuItem,
            this.出完手机核验队列ToolStripMenuItem});
            this.toolStripDropDownButton1.Image = global::BulidTaskServer.Properties.Resources.export_1_;
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(85, 22);
            this.toolStripDropDownButton1.Text = "数据操作";
            // 
            // 解锁邮件核验资源ToolStripMenuItem
            // 
            this.解锁邮件核验资源ToolStripMenuItem.Image = global::BulidTaskServer.Properties.Resources.success;
            this.解锁邮件核验资源ToolStripMenuItem.Name = "解锁邮件核验资源ToolStripMenuItem";
            this.解锁邮件核验资源ToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.解锁邮件核验资源ToolStripMenuItem.Text = "解锁邮件核验资源";
            this.解锁邮件核验资源ToolStripMenuItem.Click += new System.EventHandler(this.解锁邮件核验资源ToolStripMenuItem_Click);
            // 
            // 解锁手机核验资源ToolStripMenuItem
            // 
            this.解锁手机核验资源ToolStripMenuItem.Image = global::BulidTaskServer.Properties.Resources.success;
            this.解锁手机核验资源ToolStripMenuItem.Name = "解锁手机核验资源ToolStripMenuItem";
            this.解锁手机核验资源ToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.解锁手机核验资源ToolStripMenuItem.Text = "解锁手机核验资源";
            this.解锁手机核验资源ToolStripMenuItem.Click += new System.EventHandler(this.解锁手机核验资源ToolStripMenuItem_Click);
            // 
            // 解锁正在手机核验资源ToolStripMenuItem
            // 
            this.解锁正在手机核验资源ToolStripMenuItem.Image = global::BulidTaskServer.Properties.Resources.success;
            this.解锁正在手机核验资源ToolStripMenuItem.Name = "解锁正在手机核验资源ToolStripMenuItem";
            this.解锁正在手机核验资源ToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.解锁正在手机核验资源ToolStripMenuItem.Text = "解锁正在手机核验资源";
            this.解锁正在手机核验资源ToolStripMenuItem.Click += new System.EventHandler(this.解锁正在手机核验资源ToolStripMenuItem_Click);
            // 
            // 出完手机核验队列ToolStripMenuItem
            // 
            this.出完手机核验队列ToolStripMenuItem.Image = global::BulidTaskServer.Properties.Resources.success;
            this.出完手机核验队列ToolStripMenuItem.Name = "出完手机核验队列ToolStripMenuItem";
            this.出完手机核验队列ToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.出完手机核验队列ToolStripMenuItem.Text = "出完手机核验队列";
            this.出完手机核验队列ToolStripMenuItem.Click += new System.EventHandler(this.出完手机核验队列ToolStripMenuItem_Click);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.BackColor = System.Drawing.SystemColors.InfoText;
            this.textBox1.ForeColor = System.Drawing.SystemColors.Info;
            this.textBox1.Location = new System.Drawing.Point(0, 24);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(616, 226);
            this.textBox1.TabIndex = 9;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 253);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(616, 22);
            this.statusStrip1.TabIndex = 10;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.IsLink = true;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(32, 17);
            this.toolStripStatusLabel1.Text = "信息";
            this.toolStripStatusLabel1.Click += new System.EventHandler(this.toolStripStatusLabel1_Click);
            // 
            // BulidTaskPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(616, 275);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BulidTaskPage";
            this.Text = "生成任务(缓存)数据";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton btnCache;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem 解锁邮件核验资源ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 解锁手机核验资源ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 解锁正在手机核验资源ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 出完手机核验队列ToolStripMenuItem;

    }
}

