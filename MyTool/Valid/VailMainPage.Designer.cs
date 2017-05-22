namespace MyTool
{
    partial class VailMainPage
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VailMainPage));
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.cbNoValidEmail = new System.Windows.Forms.CheckBox();
            this.cbBad = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.cbUser = new System.Windows.Forms.CheckBox();
            this.cbGo = new System.Windows.Forms.CheckBox();
            this.cbEmailValid = new System.Windows.Forms.CheckBox();
            this.cbGoNoValid = new System.Windows.Forms.CheckBox();
            this.cbNoValid = new System.Windows.Forms.CheckBox();
            this.cbNew = new System.Windows.Forms.CheckBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnUpdateTaskNumber = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbValid = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbNoValid = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel4 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbTask = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel5 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel6 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.dateTimePicker1.Location = new System.Drawing.Point(68, 3);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(129, 21);
            this.dateTimePicker1.TabIndex = 0;
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.dateTimePicker2.Location = new System.Drawing.Point(268, 3);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(127, 21);
            this.dateTimePicker2.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 29);
            this.label1.TabIndex = 2;
            this.label1.Text = "开始时间:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(203, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 29);
            this.label2.TabIndex = 3;
            this.label2.Text = "结束时间:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 66.66666F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.dateTimePicker2, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.dateTimePicker1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBox1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 4, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(972, 320);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // panel1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.panel1, 5);
            this.panel1.Controls.Add(this.checkBox2);
            this.panel1.Controls.Add(this.cbNoValidEmail);
            this.panel1.Controls.Add(this.cbBad);
            this.panel1.Controls.Add(this.checkBox1);
            this.panel1.Controls.Add(this.cbUser);
            this.panel1.Controls.Add(this.cbGo);
            this.panel1.Controls.Add(this.cbEmailValid);
            this.panel1.Controls.Add(this.cbGoNoValid);
            this.panel1.Controls.Add(this.cbNoValid);
            this.panel1.Controls.Add(this.cbNew);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 32);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(966, 24);
            this.panel1.TabIndex = 5;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(630, 3);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(156, 16);
            this.checkBox2.TabIndex = 9;
            this.checkBox2.Tag = "4";
            this.checkBox2.Text = "手机核验通过邮件未激活";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // cbNoValidEmail
            // 
            this.cbNoValidEmail.AutoSize = true;
            this.cbNoValidEmail.ForeColor = System.Drawing.Color.DarkRed;
            this.cbNoValidEmail.Location = new System.Drawing.Point(807, 3);
            this.cbNoValidEmail.Name = "cbNoValidEmail";
            this.cbNoValidEmail.Size = new System.Drawing.Size(84, 16);
            this.cbNoValidEmail.TabIndex = 8;
            this.cbNoValidEmail.Text = "不验证邮箱";
            this.cbNoValidEmail.UseVisualStyleBackColor = true;
            // 
            // cbBad
            // 
            this.cbBad.AutoSize = true;
            this.cbBad.Location = new System.Drawing.Point(564, 3);
            this.cbBad.Name = "cbBad";
            this.cbBad.Size = new System.Drawing.Size(60, 16);
            this.cbBad.TabIndex = 7;
            this.cbBad.Tag = "4";
            this.cbBad.Text = "废账号";
            this.cbBad.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(462, 3);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(96, 16);
            this.checkBox1.TabIndex = 6;
            this.checkBox1.Tag = "4";
            this.checkBox1.Text = "邮箱核验锁定";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // cbUser
            // 
            this.cbUser.AutoSize = true;
            this.cbUser.Location = new System.Drawing.Point(408, 3);
            this.cbUser.Name = "cbUser";
            this.cbUser.Size = new System.Drawing.Size(48, 16);
            this.cbUser.TabIndex = 5;
            this.cbUser.Tag = "4";
            this.cbUser.Text = "可用";
            this.cbUser.UseVisualStyleBackColor = true;
            // 
            // cbGo
            // 
            this.cbGo.AutoSize = true;
            this.cbGo.Location = new System.Drawing.Point(148, 3);
            this.cbGo.Name = "cbGo";
            this.cbGo.Size = new System.Drawing.Size(48, 16);
            this.cbGo.TabIndex = 4;
            this.cbGo.Tag = "3";
            this.cbGo.Text = "已出";
            this.cbGo.UseVisualStyleBackColor = true;
            // 
            // cbEmailValid
            // 
            this.cbEmailValid.AutoSize = true;
            this.cbEmailValid.Location = new System.Drawing.Point(209, 3);
            this.cbEmailValid.Name = "cbEmailValid";
            this.cbEmailValid.Size = new System.Drawing.Size(96, 16);
            this.cbEmailValid.TabIndex = 3;
            this.cbEmailValid.Tag = "1";
            this.cbEmailValid.Text = "通过邮箱核验";
            this.cbEmailValid.UseVisualStyleBackColor = true;
            // 
            // cbGoNoValid
            // 
            this.cbGoNoValid.AutoSize = true;
            this.cbGoNoValid.Location = new System.Drawing.Point(318, 3);
            this.cbGoNoValid.Name = "cbGoNoValid";
            this.cbGoNoValid.Size = new System.Drawing.Size(84, 16);
            this.cbGoNoValid.TabIndex = 2;
            this.cbGoNoValid.Tag = "4";
            this.cbGoNoValid.Text = "已出未核验";
            this.cbGoNoValid.UseVisualStyleBackColor = true;
            // 
            // cbNoValid
            // 
            this.cbNoValid.AutoSize = true;
            this.cbNoValid.Location = new System.Drawing.Point(75, 3);
            this.cbNoValid.Name = "cbNoValid";
            this.cbNoValid.Size = new System.Drawing.Size(60, 16);
            this.cbNoValid.TabIndex = 1;
            this.cbNoValid.Tag = "5";
            this.cbNoValid.Text = "未核验";
            this.cbNoValid.UseVisualStyleBackColor = true;
            // 
            // cbNew
            // 
            this.cbNew.AutoSize = true;
            this.cbNew.Location = new System.Drawing.Point(2, 3);
            this.cbNew.Name = "cbNew";
            this.cbNew.Size = new System.Drawing.Size(60, 16);
            this.cbNew.TabIndex = 0;
            this.cbNew.Tag = "0";
            this.cbNew.Text = "新注册";
            this.cbNew.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.InfoText;
            this.tableLayoutPanel1.SetColumnSpan(this.textBox1, 5);
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.ForeColor = System.Drawing.SystemColors.Info;
            this.textBox1.Location = new System.Drawing.Point(3, 62);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(966, 255);
            this.textBox1.TabIndex = 6;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnUpdateTaskNumber);
            this.panel2.Controls.Add(this.textBox2);
            this.panel2.Controls.Add(this.button2);
            this.panel2.Controls.Add(this.button1);
            this.panel2.Location = new System.Drawing.Point(401, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(378, 23);
            this.panel2.TabIndex = 7;
            // 
            // btnUpdateTaskNumber
            // 
            this.btnUpdateTaskNumber.Location = new System.Drawing.Point(239, 0);
            this.btnUpdateTaskNumber.Name = "btnUpdateTaskNumber";
            this.btnUpdateTaskNumber.Size = new System.Drawing.Size(75, 23);
            this.btnUpdateTaskNumber.TabIndex = 8;
            this.btnUpdateTaskNumber.Text = "修改任务数";
            this.btnUpdateTaskNumber.UseVisualStyleBackColor = true;
            this.btnUpdateTaskNumber.Click += new System.EventHandler(this.btnUpdateTaskNumber_Click);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(133, 0);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 21);
            this.textBox2.TabIndex = 7;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(77, -1);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(49, 23);
            this.button2.TabIndex = 6;
            this.button2.Text = "停止";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(5, -1);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(66, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "开始验证";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.StartVailClick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.lbCount,
            this.toolStripStatusLabel2,
            this.lbValid,
            this.toolStripStatusLabel3,
            this.lbNoValid,
            this.toolStripStatusLabel4,
            this.lbTask,
            this.toolStripStatusLabel5,
            this.toolStripStatusLabel6});
            this.statusStrip1.Location = new System.Drawing.Point(0, 298);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(972, 22);
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(35, 17);
            this.toolStripStatusLabel1.Text = "总数:";
            // 
            // lbCount
            // 
            this.lbCount.Name = "lbCount";
            this.lbCount.Size = new System.Drawing.Size(15, 17);
            this.lbCount.Text = "0";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(95, 17);
            this.toolStripStatusLabel2.Text = "已登录进行验证:";
            // 
            // lbValid
            // 
            this.lbValid.Name = "lbValid";
            this.lbValid.Size = new System.Drawing.Size(15, 17);
            this.lbValid.Text = "0";
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(47, 17);
            this.toolStripStatusLabel3.Text = "待核验:";
            // 
            // lbNoValid
            // 
            this.lbNoValid.Name = "lbNoValid";
            this.lbNoValid.Size = new System.Drawing.Size(15, 17);
            this.lbNoValid.Text = "0";
            // 
            // toolStripStatusLabel4
            // 
            this.toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            this.toolStripStatusLabel4.Size = new System.Drawing.Size(47, 17);
            this.toolStripStatusLabel4.Text = "任务数:";
            // 
            // lbTask
            // 
            this.lbTask.Name = "lbTask";
            this.lbTask.Size = new System.Drawing.Size(15, 17);
            this.lbTask.Text = "0";
            // 
            // toolStripStatusLabel5
            // 
            this.toolStripStatusLabel5.IsLink = true;
            this.toolStripStatusLabel5.Name = "toolStripStatusLabel5";
            this.toolStripStatusLabel5.Size = new System.Drawing.Size(32, 17);
            this.toolStripStatusLabel5.Text = "详情";
            this.toolStripStatusLabel5.Click += new System.EventHandler(this.toolStripStatusLabel5_Click);
            // 
            // toolStripStatusLabel6
            // 
            this.toolStripStatusLabel6.Name = "toolStripStatusLabel6";
            this.toolStripStatusLabel6.Size = new System.Drawing.Size(15, 17);
            this.toolStripStatusLabel6.Text = "0";
            // 
            // VailMainPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(972, 320);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "VailMainPage";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "自动验证";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox cbGo;
        private System.Windows.Forms.CheckBox cbEmailValid;
        private System.Windows.Forms.CheckBox cbGoNoValid;
        private System.Windows.Forms.CheckBox cbNoValid;
        private System.Windows.Forms.CheckBox cbNew;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel lbCount;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel lbValid;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ToolStripStatusLabel lbNoValid;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel4;
        private System.Windows.Forms.ToolStripStatusLabel lbTask;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel5;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel6;
        private System.Windows.Forms.CheckBox cbUser;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button btnUpdateTaskNumber;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.CheckBox cbBad;
        private System.Windows.Forms.CheckBox cbNoValidEmail;
        private System.Windows.Forms.CheckBox checkBox2;
    }
}