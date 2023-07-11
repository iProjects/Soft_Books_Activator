namespace Soft_Books_Activator
{
    partial class main_form
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(main_form));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblrunningtime = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbltimelapsed = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtlog = new System.Windows.Forms.RichTextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btn_systems_paths = new System.Windows.Forms.LinkLabel();
            this.chkremember = new System.Windows.Forms.CheckBox();
            this.btnconnect = new System.Windows.Forms.Button();
            this.chkIntegratedSecurity = new System.Windows.Forms.CheckBox();
            this.btnclose = new System.Windows.Forms.Button();
            this.txtpassword = new System.Windows.Forms.TextBox();
            this.btnactivate = new System.Windows.Forms.Button();
            this.txtusername = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnfetchdatabases = new System.Windows.Forms.LinkLabel();
            this.btnfetchservers = new System.Windows.Forms.LinkLabel();
            this.label3 = new System.Windows.Forms.Label();
            this.cbodatabase = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cboserver = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbosystem = new System.Windows.Forms.ComboBox();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.appNotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStripSystemNotification = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.homeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.statusStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.contextMenuStripSystemNotification.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.Color.CornflowerBlue;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar,
            this.toolStripStatusLabel1,
            this.lblrunningtime,
            this.toolStripStatusLabel2,
            this.lbltimelapsed});
            this.statusStrip1.Location = new System.Drawing.Point(0, 504);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(503, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripProgressBar
            // 
            this.toolStripProgressBar.Name = "toolStripProgressBar";
            this.toolStripProgressBar.Size = new System.Drawing.Size(150, 16);
            this.toolStripProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(37, 17);
            this.toolStripStatusLabel1.Text = "          ";
            // 
            // lblrunningtime
            // 
            this.lblrunningtime.Name = "lblrunningtime";
            this.lblrunningtime.Size = new System.Drawing.Size(73, 17);
            this.lblrunningtime.Text = "runningtime";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(37, 17);
            this.toolStripStatusLabel2.Text = "          ";
            // 
            // lbltimelapsed
            // 
            this.lbltimelapsed.Name = "lbltimelapsed";
            this.lbltimelapsed.Size = new System.Drawing.Size(65, 17);
            this.lbltimelapsed.Text = "timelapsed";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtlog);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(503, 179);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // txtlog
            // 
            this.txtlog.BackColor = System.Drawing.Color.Black;
            this.txtlog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtlog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtlog.ForeColor = System.Drawing.Color.Lime;
            this.txtlog.Location = new System.Drawing.Point(3, 16);
            this.txtlog.Name = "txtlog";
            this.txtlog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.txtlog.Size = new System.Drawing.Size(497, 160);
            this.txtlog.TabIndex = 0;
            this.txtlog.Text = "";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btn_systems_paths);
            this.groupBox2.Controls.Add(this.chkremember);
            this.groupBox2.Controls.Add(this.btnconnect);
            this.groupBox2.Controls.Add(this.chkIntegratedSecurity);
            this.groupBox2.Controls.Add(this.btnclose);
            this.groupBox2.Controls.Add(this.txtpassword);
            this.groupBox2.Controls.Add(this.btnactivate);
            this.groupBox2.Controls.Add(this.txtusername);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.btnfetchdatabases);
            this.groupBox2.Controls.Add(this.btnfetchservers);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.cbodatabase);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.cboserver);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.cbosystem);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(503, 321);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            // 
            // btn_systems_paths
            // 
            this.btn_systems_paths.AutoSize = true;
            this.btn_systems_paths.Location = new System.Drawing.Point(399, 271);
            this.btn_systems_paths.Name = "btn_systems_paths";
            this.btn_systems_paths.Size = new System.Drawing.Size(73, 13);
            this.btn_systems_paths.TabIndex = 12;
            this.btn_systems_paths.TabStop = true;
            this.btn_systems_paths.Text = "systems paths";
            this.btn_systems_paths.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.btn_systems_paths_LinkClicked);
            // 
            // chkremember
            // 
            this.chkremember.AutoSize = true;
            this.chkremember.Checked = true;
            this.chkremember.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkremember.Location = new System.Drawing.Point(256, 224);
            this.chkremember.Name = "chkremember";
            this.chkremember.Size = new System.Drawing.Size(72, 17);
            this.chkremember.TabIndex = 6;
            this.chkremember.Text = "remember";
            this.chkremember.UseVisualStyleBackColor = true;
            // 
            // btnconnect
            // 
            this.btnconnect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.btnconnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnconnect.Location = new System.Drawing.Point(70, 260);
            this.btnconnect.Name = "btnconnect";
            this.btnconnect.Size = new System.Drawing.Size(85, 34);
            this.btnconnect.TabIndex = 7;
            this.btnconnect.Text = "connect";
            this.btnconnect.UseVisualStyleBackColor = false;
            this.btnconnect.Click += new System.EventHandler(this.btnconnect_Click);
            // 
            // chkIntegratedSecurity
            // 
            this.chkIntegratedSecurity.AutoSize = true;
            this.chkIntegratedSecurity.Location = new System.Drawing.Point(70, 224);
            this.chkIntegratedSecurity.Name = "chkIntegratedSecurity";
            this.chkIntegratedSecurity.Size = new System.Drawing.Size(137, 17);
            this.chkIntegratedSecurity.TabIndex = 5;
            this.chkIntegratedSecurity.Text = "Use Integrated Security";
            this.chkIntegratedSecurity.UseVisualStyleBackColor = true;
            // 
            // btnclose
            // 
            this.btnclose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.btnclose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnclose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnclose.Location = new System.Drawing.Point(274, 260);
            this.btnclose.Name = "btnclose";
            this.btnclose.Size = new System.Drawing.Size(75, 34);
            this.btnclose.TabIndex = 9;
            this.btnclose.Text = "exit";
            this.btnclose.UseVisualStyleBackColor = false;
            this.btnclose.Click += new System.EventHandler(this.btnclose_Click);
            // 
            // txtpassword
            // 
            this.txtpassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtpassword.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.txtpassword.Location = new System.Drawing.Point(70, 141);
            this.txtpassword.Name = "txtpassword";
            this.txtpassword.PasswordChar = '*';
            this.txtpassword.Size = new System.Drawing.Size(290, 25);
            this.txtpassword.TabIndex = 3;
            // 
            // btnactivate
            // 
            this.btnactivate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.btnactivate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnactivate.Location = new System.Drawing.Point(177, 260);
            this.btnactivate.Name = "btnactivate";
            this.btnactivate.Size = new System.Drawing.Size(75, 34);
            this.btnactivate.TabIndex = 8;
            this.btnactivate.Text = "activate";
            this.btnactivate.UseVisualStyleBackColor = false;
            this.btnactivate.Click += new System.EventHandler(this.btnactivate_Click);
            // 
            // txtusername
            // 
            this.txtusername.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtusername.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.txtusername.Location = new System.Drawing.Point(70, 100);
            this.txtusername.Name = "txtusername";
            this.txtusername.Size = new System.Drawing.Size(290, 25);
            this.txtusername.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.LimeGreen;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(16, 146);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "password";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.LimeGreen;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(12, 105);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "user name";
            // 
            // btnfetchdatabases
            // 
            this.btnfetchdatabases.AutoSize = true;
            this.btnfetchdatabases.Location = new System.Drawing.Point(399, 185);
            this.btnfetchdatabases.Name = "btnfetchdatabases";
            this.btnfetchdatabases.Size = new System.Drawing.Size(78, 13);
            this.btnfetchdatabases.TabIndex = 11;
            this.btnfetchdatabases.TabStop = true;
            this.btnfetchdatabases.Text = "fetch database";
            this.btnfetchdatabases.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.btnfetchdatabases_LinkClicked);
            // 
            // btnfetchservers
            // 
            this.btnfetchservers.AutoSize = true;
            this.btnfetchservers.Location = new System.Drawing.Point(399, 66);
            this.btnfetchservers.Name = "btnfetchservers";
            this.btnfetchservers.Size = new System.Drawing.Size(68, 13);
            this.btnfetchservers.TabIndex = 10;
            this.btnfetchservers.TabStop = true;
            this.btnfetchservers.Text = "fetch servers";
            this.btnfetchservers.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.btnfetchservers_LinkClicked);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.LimeGreen;
            this.label3.Location = new System.Drawing.Point(12, 185);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "database*";
            // 
            // cbodatabase
            // 
            this.cbodatabase.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbodatabase.FormattingEnabled = true;
            this.cbodatabase.Location = new System.Drawing.Point(70, 182);
            this.cbodatabase.Name = "cbodatabase";
            this.cbodatabase.Size = new System.Drawing.Size(290, 21);
            this.cbodatabase.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "server*";
            // 
            // cboserver
            // 
            this.cboserver.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboserver.FormattingEnabled = true;
            this.cboserver.Location = new System.Drawing.Point(70, 63);
            this.cboserver.Name = "cboserver";
            this.cboserver.Size = new System.Drawing.Size(290, 21);
            this.cboserver.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "system*";
            // 
            // cbosystem
            // 
            this.cbosystem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbosystem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbosystem.FormattingEnabled = true;
            this.cbosystem.Location = new System.Drawing.Point(70, 26);
            this.cbosystem.Name = "cbosystem";
            this.cbosystem.Size = new System.Drawing.Size(290, 21);
            this.cbosystem.TabIndex = 0;
            this.cbosystem.SelectedIndexChanged += new System.EventHandler(this.cbosystem_SelectedIndexChanged);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // appNotifyIcon
            // 
            this.appNotifyIcon.Text = "notifyIcon1";
            this.appNotifyIcon.Visible = true;
            // 
            // contextMenuStripSystemNotification
            // 
            this.contextMenuStripSystemNotification.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.homeToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.contextMenuStripSystemNotification.Name = "contextMenuStripSystemNotification";
            this.contextMenuStripSystemNotification.Size = new System.Drawing.Size(108, 54);
            // 
            // homeToolStripMenuItem
            // 
            this.homeToolStripMenuItem.Name = "homeToolStripMenuItem";
            this.homeToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.homeToolStripMenuItem.Text = "Home";
            this.homeToolStripMenuItem.Click += new System.EventHandler(this.homeToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(104, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer1.Size = new System.Drawing.Size(503, 504);
            this.splitContainer1.SplitterDistance = 321;
            this.splitContainer1.TabIndex = 3;
            // 
            // main_form
            // 
            this.AcceptButton = this.btnconnect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LimeGreen;
            this.CancelButton = this.btnclose;
            this.ClientSize = new System.Drawing.Size(503, 526);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "main_form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Soft Books Activator";
            this.Load += new System.EventHandler(this.main_form_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.contextMenuStripSystemNotification.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbodatabase;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboserver;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbosystem;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Button btnconnect;
        private System.Windows.Forms.Button btnclose;
        private System.Windows.Forms.Button btnactivate;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.LinkLabel btnfetchdatabases;
        private System.Windows.Forms.LinkLabel btnfetchservers;
        private System.Windows.Forms.CheckBox chkIntegratedSecurity;
        private System.Windows.Forms.TextBox txtpassword;
        private System.Windows.Forms.TextBox txtusername;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NotifyIcon appNotifyIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripSystemNotification;
        private System.Windows.Forms.ToolStripMenuItem homeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.RichTextBox txtlog;
        private System.Windows.Forms.ToolStripStatusLabel lblrunningtime;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel lbltimelapsed;
        private System.Windows.Forms.CheckBox chkremember;
        private System.Windows.Forms.LinkLabel btn_systems_paths;
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}

