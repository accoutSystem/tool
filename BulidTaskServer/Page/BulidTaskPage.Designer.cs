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
            this.toolStripLabel5 = new System.Windows.Forms.ToolStripLabel();
            this.cmbPassengerMove = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.btnReadPassenger = new System.Windows.Forms.ToolStripButton();
            this.btnAsyPassenger = new System.Windows.Forms.ToolStripButton();
            this.btnDeletePassenger = new System.Windows.Forms.ToolStripButton();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.添加删除账号联系人任务ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.账号数据迁移ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.生僻身份证转移ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.释放身份证ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.释放锁定邮件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.释放读取乘车人ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.释放注册添加联系人的账号ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.恢复分析账号乘车人状态ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.释放锁定的优质身份证ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.解锁ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.解锁邮件核验资源ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.解锁手机核验资源ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.解锁正在手机核验资源ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.出完手机核验队列ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.btnCache = new System.Windows.Forms.ToolStripButton();
            this.btnAutoTask = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnShowLog = new System.Windows.Forms.ToolStripButton();
            this.btnReadValidAccount = new System.Windows.Forms.ToolStripButton();
            this.btnDayCache = new System.Windows.Forms.ToolStripButton();
            this.toolStrip3 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripComboBox1 = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel4 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTextBox2 = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
            this.btnMove = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.toolStrip3.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel5,
            this.cmbPassengerMove,
            this.toolStripButton1,
            this.btnReadPassenger,
            this.btnAsyPassenger,
            this.btnDeletePassenger,
            this.toolStripDropDownButton1,
            this.toolStripButton3,
            this.toolStripButton2});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(833, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            this.toolStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.toolStrip1_ItemClicked);
            // 
            // toolStripLabel5
            // 
            this.toolStripLabel5.Name = "toolStripLabel5";
            this.toolStripLabel5.Size = new System.Drawing.Size(68, 22);
            this.toolStripLabel5.Text = "身份证类型";
            // 
            // cmbPassengerMove
            // 
            this.cmbPassengerMove.Items.AddRange(new object[] {
            "0|正常抓取身份证",
            "1|迁移身份证",
            "2|生僻身份证"});
            this.cmbPassengerMove.Name = "cmbPassengerMove";
            this.cmbPassengerMove.Size = new System.Drawing.Size(121, 25);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = global::BulidTaskServer.Properties.Resources.start;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(76, 22);
            this.toolStripButton1.Text = "开启注册";
            this.toolStripButton1.ToolTipText = "开启注册\r\n注册需要的联系人和邮件数据";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // btnReadPassenger
            // 
            this.btnReadPassenger.Image = global::BulidTaskServer.Properties.Resources.start;
            this.btnReadPassenger.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnReadPassenger.Name = "btnReadPassenger";
            this.btnReadPassenger.Size = new System.Drawing.Size(136, 22);
            this.btnReadPassenger.Text = "开始提取账号身份证";
            this.btnReadPassenger.ToolTipText = "为分析账号中的乘车人数据提供队列 数据\r\n";
            this.btnReadPassenger.Click += new System.EventHandler(this.ReadPassengerClick);
            // 
            // btnAsyPassenger
            // 
            this.btnAsyPassenger.Image = global::BulidTaskServer.Properties.Resources.start;
            this.btnAsyPassenger.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAsyPassenger.Name = "btnAsyPassenger";
            this.btnAsyPassenger.Size = new System.Drawing.Size(112, 22);
            this.btnAsyPassenger.Text = "开始分析身份证";
            this.btnAsyPassenger.Click += new System.EventHandler(this.toolStripButton4_Click_4);
            // 
            // btnDeletePassenger
            // 
            this.btnDeletePassenger.Image = global::BulidTaskServer.Properties.Resources.start;
            this.btnDeletePassenger.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDeletePassenger.Name = "btnDeletePassenger";
            this.btnDeletePassenger.Size = new System.Drawing.Size(136, 22);
            this.btnDeletePassenger.Text = "开始清理账号联系人";
            this.btnDeletePassenger.Click += new System.EventHandler(this.DeletePassengerClick);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.添加删除账号联系人任务ToolStripMenuItem,
            this.账号数据迁移ToolStripMenuItem,
            this.生僻身份证转移ToolStripMenuItem,
            this.释放身份证ToolStripMenuItem,
            this.释放锁定邮件ToolStripMenuItem,
            this.释放读取乘车人ToolStripMenuItem,
            this.释放注册添加联系人的账号ToolStripMenuItem,
            this.恢复分析账号乘车人状态ToolStripMenuItem,
            this.释放锁定的优质身份证ToolStripMenuItem,
            this.解锁ToolStripMenuItem,
            this.解锁邮件核验资源ToolStripMenuItem,
            this.解锁手机核验资源ToolStripMenuItem,
            this.解锁正在手机核验资源ToolStripMenuItem,
            this.出完手机核验队列ToolStripMenuItem});
            this.toolStripDropDownButton1.Image = global::BulidTaskServer.Properties.Resources.export_1_;
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(109, 22);
            this.toolStripDropDownButton1.Text = "数据状态操作";
            this.toolStripDropDownButton1.ToolTipText = "数据管理";
            // 
            // 添加删除账号联系人任务ToolStripMenuItem
            // 
            this.添加删除账号联系人任务ToolStripMenuItem.Name = "添加删除账号联系人任务ToolStripMenuItem";
            this.添加删除账号联系人任务ToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.添加删除账号联系人任务ToolStripMenuItem.Text = "添加删除账号联系人任务";
            this.添加删除账号联系人任务ToolStripMenuItem.Click += new System.EventHandler(this.添加删除账号联系人任务ToolStripMenuItem_Click);
            // 
            // 账号数据迁移ToolStripMenuItem
            // 
            this.账号数据迁移ToolStripMenuItem.Name = "账号数据迁移ToolStripMenuItem";
            this.账号数据迁移ToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.账号数据迁移ToolStripMenuItem.Text = "账号数据迁移";
            this.账号数据迁移ToolStripMenuItem.Click += new System.EventHandler(this.账号数据迁移ToolStripMenuItem_Click);
            // 
            // 生僻身份证转移ToolStripMenuItem
            // 
            this.生僻身份证转移ToolStripMenuItem.Name = "生僻身份证转移ToolStripMenuItem";
            this.生僻身份证转移ToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.生僻身份证转移ToolStripMenuItem.Text = "生僻身份证转移";
            this.生僻身份证转移ToolStripMenuItem.Click += new System.EventHandler(this.生僻身份证转移ToolStripMenuItem_Click);
            // 
            // 释放身份证ToolStripMenuItem
            // 
            this.释放身份证ToolStripMenuItem.Name = "释放身份证ToolStripMenuItem";
            this.释放身份证ToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.释放身份证ToolStripMenuItem.Text = "释放锁定身份证";
            this.释放身份证ToolStripMenuItem.Click += new System.EventHandler(this.释放身份证ToolStripMenuItem_Click);
            // 
            // 释放锁定邮件ToolStripMenuItem
            // 
            this.释放锁定邮件ToolStripMenuItem.Name = "释放锁定邮件ToolStripMenuItem";
            this.释放锁定邮件ToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.释放锁定邮件ToolStripMenuItem.Text = "释放锁定邮件";
            this.释放锁定邮件ToolStripMenuItem.Click += new System.EventHandler(this.释放锁定邮件ToolStripMenuItem_Click);
            // 
            // 释放读取乘车人ToolStripMenuItem
            // 
            this.释放读取乘车人ToolStripMenuItem.Name = "释放读取乘车人ToolStripMenuItem";
            this.释放读取乘车人ToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.释放读取乘车人ToolStripMenuItem.Text = "释放读取乘车人数据";
            this.释放读取乘车人ToolStripMenuItem.Click += new System.EventHandler(this.释放读取乘车人ToolStripMenuItem_Click);
            // 
            // 释放注册添加联系人的账号ToolStripMenuItem
            // 
            this.释放注册添加联系人的账号ToolStripMenuItem.Name = "释放注册添加联系人的账号ToolStripMenuItem";
            this.释放注册添加联系人的账号ToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.释放注册添加联系人的账号ToolStripMenuItem.Text = "释放注册添加联系人的账号";
            this.释放注册添加联系人的账号ToolStripMenuItem.Click += new System.EventHandler(this.释放注册添加联系人的账号ToolStripMenuItem_Click);
            // 
            // 恢复分析账号乘车人状态ToolStripMenuItem
            // 
            this.恢复分析账号乘车人状态ToolStripMenuItem.Name = "恢复分析账号乘车人状态ToolStripMenuItem";
            this.恢复分析账号乘车人状态ToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.恢复分析账号乘车人状态ToolStripMenuItem.Text = "恢复分析账号乘车人状态";
            this.恢复分析账号乘车人状态ToolStripMenuItem.Click += new System.EventHandler(this.恢复分析账号乘车人状态ToolStripMenuItem_Click);
            // 
            // 释放锁定的优质身份证ToolStripMenuItem
            // 
            this.释放锁定的优质身份证ToolStripMenuItem.Enabled = false;
            this.释放锁定的优质身份证ToolStripMenuItem.Image = global::BulidTaskServer.Properties.Resources.success;
            this.释放锁定的优质身份证ToolStripMenuItem.Name = "释放锁定的优质身份证ToolStripMenuItem";
            this.释放锁定的优质身份证ToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.释放锁定的优质身份证ToolStripMenuItem.Text = "释放锁定的优质身份证";
            this.释放锁定的优质身份证ToolStripMenuItem.Visible = false;
            this.释放锁定的优质身份证ToolStripMenuItem.Click += new System.EventHandler(this.释放锁定的优质身份证ToolStripMenuItem_Click);
            // 
            // 解锁ToolStripMenuItem
            // 
            this.解锁ToolStripMenuItem.Enabled = false;
            this.解锁ToolStripMenuItem.Image = global::BulidTaskServer.Properties.Resources.success;
            this.解锁ToolStripMenuItem.Name = "解锁ToolStripMenuItem";
            this.解锁ToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.解锁ToolStripMenuItem.Text = "解锁锁定邮件核验资源";
            this.解锁ToolStripMenuItem.Visible = false;
            this.解锁ToolStripMenuItem.Click += new System.EventHandler(this.解锁ToolStripMenuItem_Click);
            // 
            // 解锁邮件核验资源ToolStripMenuItem
            // 
            this.解锁邮件核验资源ToolStripMenuItem.Enabled = false;
            this.解锁邮件核验资源ToolStripMenuItem.Image = global::BulidTaskServer.Properties.Resources.success;
            this.解锁邮件核验资源ToolStripMenuItem.Name = "解锁邮件核验资源ToolStripMenuItem";
            this.解锁邮件核验资源ToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.解锁邮件核验资源ToolStripMenuItem.Text = "解锁正在核验邮件资源";
            this.解锁邮件核验资源ToolStripMenuItem.Visible = false;
            this.解锁邮件核验资源ToolStripMenuItem.Click += new System.EventHandler(this.解锁邮件核验资源ToolStripMenuItem_Click);
            // 
            // 解锁手机核验资源ToolStripMenuItem
            // 
            this.解锁手机核验资源ToolStripMenuItem.Enabled = false;
            this.解锁手机核验资源ToolStripMenuItem.Image = global::BulidTaskServer.Properties.Resources.success;
            this.解锁手机核验资源ToolStripMenuItem.Name = "解锁手机核验资源ToolStripMenuItem";
            this.解锁手机核验资源ToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.解锁手机核验资源ToolStripMenuItem.Text = "解锁锁定手机核验资源";
            this.解锁手机核验资源ToolStripMenuItem.Visible = false;
            this.解锁手机核验资源ToolStripMenuItem.Click += new System.EventHandler(this.解锁手机核验资源ToolStripMenuItem_Click);
            // 
            // 解锁正在手机核验资源ToolStripMenuItem
            // 
            this.解锁正在手机核验资源ToolStripMenuItem.Enabled = false;
            this.解锁正在手机核验资源ToolStripMenuItem.Image = global::BulidTaskServer.Properties.Resources.success;
            this.解锁正在手机核验资源ToolStripMenuItem.Name = "解锁正在手机核验资源ToolStripMenuItem";
            this.解锁正在手机核验资源ToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.解锁正在手机核验资源ToolStripMenuItem.Text = "解锁正在手机核验资源";
            this.解锁正在手机核验资源ToolStripMenuItem.Visible = false;
            this.解锁正在手机核验资源ToolStripMenuItem.Click += new System.EventHandler(this.解锁正在手机核验资源ToolStripMenuItem_Click);
            // 
            // 出完手机核验队列ToolStripMenuItem
            // 
            this.出完手机核验队列ToolStripMenuItem.Enabled = false;
            this.出完手机核验队列ToolStripMenuItem.Image = global::BulidTaskServer.Properties.Resources.success;
            this.出完手机核验队列ToolStripMenuItem.Name = "出完手机核验队列ToolStripMenuItem";
            this.出完手机核验队列ToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.出完手机核验队列ToolStripMenuItem.Text = "出完手机核验队列";
            this.出完手机核验队列ToolStripMenuItem.Visible = false;
            this.出完手机核验队列ToolStripMenuItem.Click += new System.EventHandler(this.出完手机核验队列ToolStripMenuItem_Click);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.Enabled = false;
            this.toolStripButton3.Image = global::BulidTaskServer.Properties.Resources.start;
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(100, 22);
            this.toolStripButton3.Text = "开始核验手机";
            this.toolStripButton3.Visible = false;
            this.toolStripButton3.Click += new System.EventHandler(this.toolStripButton3_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.Enabled = false;
            this.toolStripButton2.Image = global::BulidTaskServer.Properties.Resources.start;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(100, 22);
            this.toolStripButton2.Text = "开始邮箱核验";
            this.toolStripButton2.ToolTipText = "开始邮箱核验\r\n、";
            this.toolStripButton2.Visible = false;
            this.toolStripButton2.Click += new System.EventHandler(this.ValidEmailClick);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.BackColor = System.Drawing.SystemColors.InfoText;
            this.textBox1.ForeColor = System.Drawing.SystemColors.Info;
            this.textBox1.Location = new System.Drawing.Point(0, 78);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(833, 243);
            this.textBox1.TabIndex = 9;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 324);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(833, 22);
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
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnCache,
            this.btnAutoTask,
            this.toolStripSeparator1,
            this.btnShowLog,
            this.btnReadValidAccount,
            this.btnDayCache});
            this.toolStrip2.Location = new System.Drawing.Point(0, 25);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(833, 25);
            this.toolStrip2.TabIndex = 11;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // btnCache
            // 
            this.btnCache.Image = global::BulidTaskServer.Properties.Resources.start;
            this.btnCache.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCache.Name = "btnCache";
            this.btnCache.Size = new System.Drawing.Size(76, 22);
            this.btnCache.Text = "开始汇总";
            this.btnCache.Click += new System.EventHandler(this.CreateCollectClick);
            // 
            // btnAutoTask
            // 
            this.btnAutoTask.Image = global::BulidTaskServer.Properties.Resources.start;
            this.btnAutoTask.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAutoTask.Name = "btnAutoTask";
            this.btnAutoTask.Size = new System.Drawing.Size(100, 22);
            this.btnAutoTask.Text = "开始自动任务";
            this.btnAutoTask.ToolTipText = "11点后自动任务执行任务====\r\n1迁移已使用的乘车人\r\n2迁移已使用的邮件\r\n3正在使用的邮件变为可用\r\n4正在使用的乘车人变为可用\r\n5优质乘车人锁定修改为" +
    "优质乘车人\r\n6清理缓存联系人，优质联系人，邮件，";
            this.btnAutoTask.Click += new System.EventHandler(this.btnAutoTask_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnShowLog
            // 
            this.btnShowLog.Image = global::BulidTaskServer.Properties.Resources.stop;
            this.btnShowLog.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnShowLog.Name = "btnShowLog";
            this.btnShowLog.Size = new System.Drawing.Size(100, 22);
            this.btnShowLog.Text = "禁用日志输出";
            this.btnShowLog.Click += new System.EventHandler(this.btnShowLog_Click);
            // 
            // btnReadValidAccount
            // 
            this.btnReadValidAccount.Enabled = false;
            this.btnReadValidAccount.Image = global::BulidTaskServer.Properties.Resources.start;
            this.btnReadValidAccount.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnReadValidAccount.Name = "btnReadValidAccount";
            this.btnReadValidAccount.Size = new System.Drawing.Size(136, 22);
            this.btnReadValidAccount.Text = "开始读取乘车人账号";
            this.btnReadValidAccount.ToolTipText = "注册账号时为了避免身份证出现问题，\r\n需要先添加身份证到一个账号中作为乘车人 \r\n为注册账号提供核验账号数据";
            this.btnReadValidAccount.Visible = false;
            this.btnReadValidAccount.Click += new System.EventHandler(this.toolStripButton4_Click);
            // 
            // btnDayCache
            // 
            this.btnDayCache.Enabled = false;
            this.btnDayCache.Image = global::BulidTaskServer.Properties.Resources.start;
            this.btnDayCache.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDayCache.Name = "btnDayCache";
            this.btnDayCache.Size = new System.Drawing.Size(100, 22);
            this.btnDayCache.Text = "开始按天汇总";
            this.btnDayCache.Visible = false;
            this.btnDayCache.Click += new System.EventHandler(this.CreateCollectInDayClick);
            // 
            // toolStrip3
            // 
            this.toolStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolStripComboBox1,
            this.toolStripLabel4,
            this.toolStripLabel3,
            this.toolStripTextBox2,
            this.toolStripLabel2,
            this.toolStripTextBox1,
            this.btnMove});
            this.toolStrip3.Location = new System.Drawing.Point(0, 50);
            this.toolStrip3.Name = "toolStrip3";
            this.toolStrip3.Size = new System.Drawing.Size(833, 25);
            this.toolStrip3.TabIndex = 12;
            this.toolStrip3.Text = "toolStrip3";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(32, 22);
            this.toolStripLabel1.Text = "状态";
            // 
            // toolStripComboBox1
            // 
            this.toolStripComboBox1.Items.AddRange(new object[] {
            "0,已使用历史数据",
            "4,未分析的生僻身份证",
            "5,未知状态"});
            this.toolStripComboBox1.Name = "toolStripComboBox1";
            this.toolStripComboBox1.Size = new System.Drawing.Size(121, 25);
            this.toolStripComboBox1.SelectedIndexChanged += new System.EventHandler(this.PassengerStateChanged);
            // 
            // toolStripLabel4
            // 
            this.toolStripLabel4.IsLink = true;
            this.toolStripLabel4.LinkVisited = true;
            this.toolStripLabel4.Name = "toolStripLabel4";
            this.toolStripLabel4.Size = new System.Drawing.Size(79, 22);
            this.toolStripLabel4.Text = "Min:0 Max:0";
            this.toolStripLabel4.Click += new System.EventHandler(this.toolStripLabel4_Click);
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(56, 22);
            this.toolStripLabel3.Text = "启始索引";
            // 
            // toolStripTextBox2
            // 
            this.toolStripTextBox2.Name = "toolStripTextBox2";
            this.toolStripTextBox2.Size = new System.Drawing.Size(100, 25);
            this.toolStripTextBox2.Text = "8711541";
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(32, 22);
            this.toolStripLabel2.Text = "数量";
            // 
            // toolStripTextBox1
            // 
            this.toolStripTextBox1.Name = "toolStripTextBox1";
            this.toolStripTextBox1.Size = new System.Drawing.Size(100, 25);
            this.toolStripTextBox1.Text = "100";
            // 
            // btnMove
            // 
            this.btnMove.Image = global::BulidTaskServer.Properties.Resources.move;
            this.btnMove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnMove.Name = "btnMove";
            this.btnMove.Size = new System.Drawing.Size(88, 22);
            this.btnMove.Text = "转移并分析";
            this.btnMove.Click += new System.EventHandler(this.toolStripButton4_Click_3);
            // 
            // BulidTaskPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(833, 346);
            this.Controls.Add(this.toolStrip3);
            this.Controls.Add(this.toolStrip2);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BulidTaskPage";
            this.Text = "数据调度中心";
            this.Load += new System.EventHandler(this.BulidTaskPage_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.toolStrip3.ResumeLayout(false);
            this.toolStrip3.PerformLayout();
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
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem 解锁邮件核验资源ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 解锁手机核验资源ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 解锁正在手机核验资源ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 出完手机核验队列ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 解锁ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 释放身份证ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 释放锁定邮件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 释放读取乘车人ToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton btnReadPassenger;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton btnCache;
        private System.Windows.Forms.ToolStripButton btnDayCache;
        private System.Windows.Forms.ToolStripButton btnAutoTask;
        private System.Windows.Forms.ToolStripButton btnReadValidAccount;
        private System.Windows.Forms.ToolStripMenuItem 释放注册添加联系人的账号ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 释放锁定的优质身份证ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 账号数据迁移ToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton btnDeletePassenger;
        private System.Windows.Forms.ToolStripMenuItem 恢复分析账号乘车人状态ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStrip toolStrip3;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox1;
        private System.Windows.Forms.ToolStripButton btnMove;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox1;
        private System.Windows.Forms.ToolStripButton btnShowLog;
        private System.Windows.Forms.ToolStripLabel toolStripLabel4;
        private System.Windows.Forms.ToolStripLabel toolStripLabel5;
        private System.Windows.Forms.ToolStripComboBox cmbPassengerMove;
        private System.Windows.Forms.ToolStripMenuItem 生僻身份证转移ToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton btnAsyPassenger;
        private System.Windows.Forms.ToolStripMenuItem 添加删除账号联系人任务ToolStripMenuItem;

    }
}

