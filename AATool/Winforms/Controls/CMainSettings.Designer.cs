
namespace AATool.Winforms.Controls
{
    partial class CMainSettings
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.notesEnabled = new System.Windows.Forms.CheckBox();
            this.mainGroupMain = new System.Windows.Forms.GroupBox();
            this.hideCompleted = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.viewMode = new System.Windows.Forms.ComboBox();
            this.showBasic = new System.Windows.Forms.CheckBox();
            this.layoutDebug = new System.Windows.Forms.CheckBox();
            this.autoVersion = new System.Windows.Forms.CheckBox();
            this.gameVersion = new System.Windows.Forms.ComboBox();
            this.mainGroupTheme = new System.Windows.Forms.GroupBox();
            this.fpsCap = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.refreshIcon = new System.Windows.Forms.ComboBox();
            this.completionGlow = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.theme = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.textColor = new System.Windows.Forms.Button();
            this.borderColor = new System.Windows.Forms.Button();
            this.backColor = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.localGroup = new System.Windows.Forms.GroupBox();
            this.savesCustom = new System.Windows.Forms.RadioButton();
            this.savesDefault = new System.Windows.Forms.RadioButton();
            this.browse = new System.Windows.Forms.Button();
            this.customSavePath = new System.Windows.Forms.TextBox();
            this.worldLocal = new System.Windows.Forms.RadioButton();
            this.worldRemote = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.sftpCompatibility = new System.Windows.Forms.LinkLabel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.remoteGroup = new System.Windows.Forms.GroupBox();
            this.sftpPort = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.toggleCredentials = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.sftpPass = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.sftpValidate = new System.Windows.Forms.Button();
            this.sftpHost = new System.Windows.Forms.TextBox();
            this.sftpUser = new System.Windows.Forms.TextBox();
            this.groupBox2.SuspendLayout();
            this.mainGroupMain.SuspendLayout();
            this.mainGroupTheme.SuspendLayout();
            this.localGroup.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.remoteGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.notesEnabled);
            this.groupBox2.Location = new System.Drawing.Point(3, 260);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(301, 44);
            this.groupBox2.TabIndex = 26;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Secondary Windows";
            // 
            // notesEnabled
            // 
            this.notesEnabled.AutoSize = true;
            this.notesEnabled.Location = new System.Drawing.Point(6, 19);
            this.notesEnabled.Name = "notesEnabled";
            this.notesEnabled.Size = new System.Drawing.Size(126, 17);
            this.notesEnabled.TabIndex = 27;
            this.notesEnabled.Text = "Show Notes Window";
            this.notesEnabled.UseVisualStyleBackColor = true;
            this.notesEnabled.CheckedChanged += new System.EventHandler(this.OnCheckChanged);
            // 
            // mainGroupMain
            // 
            this.mainGroupMain.Controls.Add(this.hideCompleted);
            this.mainGroupMain.Controls.Add(this.label1);
            this.mainGroupMain.Controls.Add(this.viewMode);
            this.mainGroupMain.Controls.Add(this.showBasic);
            this.mainGroupMain.Controls.Add(this.layoutDebug);
            this.mainGroupMain.Location = new System.Drawing.Point(117, 113);
            this.mainGroupMain.Name = "mainGroupMain";
            this.mainGroupMain.Size = new System.Drawing.Size(187, 141);
            this.mainGroupMain.TabIndex = 25;
            this.mainGroupMain.TabStop = false;
            this.mainGroupMain.Text = "General";
            // 
            // hideCompleted
            // 
            this.hideCompleted.AutoSize = true;
            this.hideCompleted.Location = new System.Drawing.Point(6, 92);
            this.hideCompleted.Name = "hideCompleted";
            this.hideCompleted.Size = new System.Drawing.Size(175, 17);
            this.hideCompleted.TabIndex = 32;
            this.hideCompleted.Text = "Hide Completed Advancements";
            this.hideCompleted.UseVisualStyleBackColor = true;
            this.hideCompleted.CheckedChanged += new System.EventHandler(this.OnCheckChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 22);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 27;
            this.label1.Text = "View Mode:";
            // 
            // viewMode
            // 
            this.viewMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.viewMode.FormattingEnabled = true;
            this.viewMode.Items.AddRange(new object[] {
            "Relaxed",
            "Compact"});
            this.viewMode.Location = new System.Drawing.Point(6, 38);
            this.viewMode.Name = "viewMode";
            this.viewMode.Size = new System.Drawing.Size(108, 21);
            this.viewMode.TabIndex = 26;
            this.viewMode.SelectedIndexChanged += new System.EventHandler(this.OnIndexChanged);
            // 
            // showBasic
            // 
            this.showBasic.AutoSize = true;
            this.showBasic.Location = new System.Drawing.Point(6, 69);
            this.showBasic.Name = "showBasic";
            this.showBasic.Size = new System.Drawing.Size(156, 17);
            this.showBasic.TabIndex = 8;
            this.showBasic.Text = "Show Basic Advancements";
            this.showBasic.UseVisualStyleBackColor = true;
            this.showBasic.CheckedChanged += new System.EventHandler(this.OnCheckChanged);
            // 
            // layoutDebug
            // 
            this.layoutDebug.AutoSize = true;
            this.layoutDebug.Location = new System.Drawing.Point(6, 115);
            this.layoutDebug.Name = "layoutDebug";
            this.layoutDebug.Size = new System.Drawing.Size(119, 17);
            this.layoutDebug.TabIndex = 25;
            this.layoutDebug.Text = "Layout Debug View";
            this.layoutDebug.UseVisualStyleBackColor = true;
            this.layoutDebug.CheckedChanged += new System.EventHandler(this.OnCheckChanged);
            // 
            // autoVersion
            // 
            this.autoVersion.AutoSize = true;
            this.autoVersion.Location = new System.Drawing.Point(6, 69);
            this.autoVersion.Name = "autoVersion";
            this.autoVersion.Size = new System.Drawing.Size(83, 17);
            this.autoVersion.TabIndex = 26;
            this.autoVersion.Text = "Auto-Detect";
            this.autoVersion.UseVisualStyleBackColor = true;
            this.autoVersion.CheckedChanged += new System.EventHandler(this.OnCheckChanged);
            // 
            // gameVersion
            // 
            this.gameVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.gameVersion.FormattingEnabled = true;
            this.gameVersion.Location = new System.Drawing.Point(6, 38);
            this.gameVersion.Name = "gameVersion";
            this.gameVersion.Size = new System.Drawing.Size(96, 21);
            this.gameVersion.TabIndex = 18;
            this.gameVersion.SelectedIndexChanged += new System.EventHandler(this.OnIndexChanged);
            // 
            // mainGroupTheme
            // 
            this.mainGroupTheme.Controls.Add(this.fpsCap);
            this.mainGroupTheme.Controls.Add(this.label5);
            this.mainGroupTheme.Controls.Add(this.label2);
            this.mainGroupTheme.Controls.Add(this.refreshIcon);
            this.mainGroupTheme.Controls.Add(this.completionGlow);
            this.mainGroupTheme.Controls.Add(this.label10);
            this.mainGroupTheme.Controls.Add(this.theme);
            this.mainGroupTheme.Controls.Add(this.label8);
            this.mainGroupTheme.Controls.Add(this.label9);
            this.mainGroupTheme.Controls.Add(this.textColor);
            this.mainGroupTheme.Controls.Add(this.borderColor);
            this.mainGroupTheme.Controls.Add(this.backColor);
            this.mainGroupTheme.Controls.Add(this.label7);
            this.mainGroupTheme.Location = new System.Drawing.Point(310, 113);
            this.mainGroupTheme.Name = "mainGroupTheme";
            this.mainGroupTheme.Size = new System.Drawing.Size(223, 191);
            this.mainGroupTheme.TabIndex = 24;
            this.mainGroupTheme.TabStop = false;
            this.mainGroupTheme.Text = "Appearance";
            // 
            // fpsCap
            // 
            this.fpsCap.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fpsCap.FormattingEnabled = true;
            this.fpsCap.Items.AddRange(new object[] {
            "60",
            "45",
            "30",
            "24",
            "16",
            "10"});
            this.fpsCap.Location = new System.Drawing.Point(6, 108);
            this.fpsCap.Name = "fpsCap";
            this.fpsCap.Size = new System.Drawing.Size(68, 21);
            this.fpsCap.TabIndex = 32;
            this.fpsCap.SelectedIndexChanged += new System.EventHandler(this.OnIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 92);
            this.label5.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(213, 13);
            this.label5.TabIndex = 32;
            this.label5.Text = "FPS Cap:  (reduces load on slower systems)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(117, 22);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 28;
            this.label2.Text = "Refresh Icon:";
            // 
            // refreshIcon
            // 
            this.refreshIcon.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.refreshIcon.FormattingEnabled = true;
            this.refreshIcon.Items.AddRange(new object[] {
            "Xp Orb",
            "Compass"});
            this.refreshIcon.Location = new System.Drawing.Point(120, 37);
            this.refreshIcon.Name = "refreshIcon";
            this.refreshIcon.Size = new System.Drawing.Size(68, 21);
            this.refreshIcon.TabIndex = 27;
            this.refreshIcon.SelectedIndexChanged += new System.EventHandler(this.OnIndexChanged);
            // 
            // completionGlow
            // 
            this.completionGlow.AutoSize = true;
            this.completionGlow.Location = new System.Drawing.Point(6, 64);
            this.completionGlow.Name = "completionGlow";
            this.completionGlow.Size = new System.Drawing.Size(136, 17);
            this.completionGlow.TabIndex = 26;
            this.completionGlow.Text = "Completion Glow Effect";
            this.completionGlow.UseVisualStyleBackColor = true;
            this.completionGlow.CheckedChanged += new System.EventHandler(this.OnCheckChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 22);
            this.label10.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(66, 13);
            this.label10.TabIndex = 23;
            this.label10.Text = "Style Preset:";
            // 
            // theme
            // 
            this.theme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.theme.FormattingEnabled = true;
            this.theme.Location = new System.Drawing.Point(6, 37);
            this.theme.Name = "theme";
            this.theme.Size = new System.Drawing.Size(108, 21);
            this.theme.TabIndex = 22;
            this.theme.SelectedIndexChanged += new System.EventHandler(this.OnIndexChanged);
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label8.Location = new System.Drawing.Point(6, 137);
            this.label8.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(32, 13);
            this.label8.TabIndex = 16;
            this.label8.Text = "Back";
            this.label8.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label9.Location = new System.Drawing.Point(42, 137);
            this.label9.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(38, 13);
            this.label9.TabIndex = 20;
            this.label9.Text = "Fore";
            this.label9.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // textColor
            // 
            this.textColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textColor.Location = new System.Drawing.Point(82, 153);
            this.textColor.Name = "textColor";
            this.textColor.Size = new System.Drawing.Size(32, 32);
            this.textColor.TabIndex = 17;
            this.textColor.UseVisualStyleBackColor = true;
            this.textColor.Click += new System.EventHandler(this.OnClicked);
            // 
            // borderColor
            // 
            this.borderColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.borderColor.Location = new System.Drawing.Point(44, 153);
            this.borderColor.Name = "borderColor";
            this.borderColor.Size = new System.Drawing.Size(32, 32);
            this.borderColor.TabIndex = 19;
            this.borderColor.UseVisualStyleBackColor = true;
            this.borderColor.Click += new System.EventHandler(this.OnClicked);
            // 
            // backColor
            // 
            this.backColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.backColor.Location = new System.Drawing.Point(6, 153);
            this.backColor.Name = "backColor";
            this.backColor.Size = new System.Drawing.Size(32, 32);
            this.backColor.TabIndex = 15;
            this.backColor.UseVisualStyleBackColor = true;
            this.backColor.Click += new System.EventHandler(this.OnClicked);
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label7.Location = new System.Drawing.Point(82, 137);
            this.label7.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(32, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "Text";
            this.label7.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // localGroup
            // 
            this.localGroup.Controls.Add(this.savesCustom);
            this.localGroup.Controls.Add(this.savesDefault);
            this.localGroup.Controls.Add(this.browse);
            this.localGroup.Controls.Add(this.customSavePath);
            this.localGroup.Location = new System.Drawing.Point(117, 3);
            this.localGroup.Name = "localGroup";
            this.localGroup.Size = new System.Drawing.Size(416, 104);
            this.localGroup.TabIndex = 23;
            this.localGroup.TabStop = false;
            this.localGroup.Text = "Save Path";
            // 
            // savesCustom
            // 
            this.savesCustom.AutoSize = true;
            this.savesCustom.Location = new System.Drawing.Point(6, 42);
            this.savesCustom.Name = "savesCustom";
            this.savesCustom.Size = new System.Drawing.Size(172, 17);
            this.savesCustom.TabIndex = 8;
            this.savesCustom.TabStop = true;
            this.savesCustom.Text = "Use Custom Saves Folder Path";
            this.savesCustom.UseVisualStyleBackColor = true;
            this.savesCustom.CheckedChanged += new System.EventHandler(this.OnCheckChanged);
            // 
            // savesDefault
            // 
            this.savesDefault.AutoSize = true;
            this.savesDefault.Location = new System.Drawing.Point(6, 19);
            this.savesDefault.Name = "savesDefault";
            this.savesDefault.Size = new System.Drawing.Size(297, 17);
            this.savesDefault.TabIndex = 7;
            this.savesDefault.TabStop = true;
            this.savesDefault.Text = "Use Default Path (...AppData\\Roaming\\.minecraft\\saves)";
            this.savesDefault.UseVisualStyleBackColor = true;
            this.savesDefault.CheckedChanged += new System.EventHandler(this.OnCheckChanged);
            // 
            // browse
            // 
            this.browse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.browse.Location = new System.Drawing.Point(357, 75);
            this.browse.Name = "browse";
            this.browse.Size = new System.Drawing.Size(53, 22);
            this.browse.TabIndex = 1;
            this.browse.Text = "Browse";
            this.browse.UseVisualStyleBackColor = true;
            this.browse.Click += new System.EventHandler(this.OnClicked);
            // 
            // customSavePath
            // 
            this.customSavePath.Location = new System.Drawing.Point(7, 76);
            this.customSavePath.Name = "customSavePath";
            this.customSavePath.Size = new System.Drawing.Size(344, 20);
            this.customSavePath.TabIndex = 0;
            this.customSavePath.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // worldLocal
            // 
            this.worldLocal.AutoSize = true;
            this.worldLocal.Location = new System.Drawing.Point(6, 19);
            this.worldLocal.Name = "worldLocal";
            this.worldLocal.Size = new System.Drawing.Size(51, 17);
            this.worldLocal.TabIndex = 27;
            this.worldLocal.TabStop = true;
            this.worldLocal.Text = "Local";
            this.worldLocal.UseVisualStyleBackColor = true;
            this.worldLocal.CheckedChanged += new System.EventHandler(this.OnCheckChanged);
            // 
            // worldRemote
            // 
            this.worldRemote.AutoSize = true;
            this.worldRemote.Location = new System.Drawing.Point(6, 42);
            this.worldRemote.Name = "worldRemote";
            this.worldRemote.Size = new System.Drawing.Size(98, 17);
            this.worldRemote.TabIndex = 28;
            this.worldRemote.TabStop = true;
            this.worldRemote.Text = "Remote (SFTP)";
            this.worldRemote.UseVisualStyleBackColor = true;
            this.worldRemote.CheckedChanged += new System.EventHandler(this.OnCheckChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.sftpCompatibility);
            this.groupBox3.Controls.Add(this.worldLocal);
            this.groupBox3.Controls.Add(this.worldRemote);
            this.groupBox3.Location = new System.Drawing.Point(3, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(108, 104);
            this.groupBox3.TabIndex = 29;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "World Type";
            // 
            // sftpCompatibility
            // 
            this.sftpCompatibility.AutoSize = true;
            this.sftpCompatibility.Location = new System.Drawing.Point(7, 81);
            this.sftpCompatibility.Name = "sftpCompatibility";
            this.sftpCompatibility.Size = new System.Drawing.Size(94, 13);
            this.sftpCompatibility.TabIndex = 29;
            this.sftpCompatibility.TabStop = true;
            this.sftpCompatibility.Text = "SFTP compatibility";
            this.sftpCompatibility.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnLinkClicked);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.autoVersion);
            this.groupBox1.Controls.Add(this.gameVersion);
            this.groupBox1.Location = new System.Drawing.Point(3, 113);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(108, 141);
            this.groupBox1.TabIndex = 30;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Game Version";
            // 
            // remoteGroup
            // 
            this.remoteGroup.Controls.Add(this.sftpPort);
            this.remoteGroup.Controls.Add(this.label11);
            this.remoteGroup.Controls.Add(this.toggleCredentials);
            this.remoteGroup.Controls.Add(this.label4);
            this.remoteGroup.Controls.Add(this.sftpPass);
            this.remoteGroup.Controls.Add(this.label3);
            this.remoteGroup.Controls.Add(this.label6);
            this.remoteGroup.Controls.Add(this.sftpValidate);
            this.remoteGroup.Controls.Add(this.sftpHost);
            this.remoteGroup.Controls.Add(this.sftpUser);
            this.remoteGroup.Location = new System.Drawing.Point(551, 3);
            this.remoteGroup.Name = "remoteGroup";
            this.remoteGroup.Size = new System.Drawing.Size(416, 104);
            this.remoteGroup.TabIndex = 31;
            this.remoteGroup.TabStop = false;
            this.remoteGroup.Text = "SFTP Configuration";
            // 
            // sftpPort
            // 
            this.sftpPort.Location = new System.Drawing.Point(232, 35);
            this.sftpPort.Name = "sftpPort";
            this.sftpPort.Size = new System.Drawing.Size(42, 20);
            this.sftpPort.TabIndex = 38;
            this.sftpPort.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(229, 19);
            this.label11.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(29, 13);
            this.label11.TabIndex = 39;
            this.label11.Text = "Port:";
            // 
            // toggleCredentials
            // 
            this.toggleCredentials.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.toggleCredentials.Location = new System.Drawing.Point(280, 73);
            this.toggleCredentials.Name = "toggleCredentials";
            this.toggleCredentials.Size = new System.Drawing.Size(130, 22);
            this.toggleCredentials.TabIndex = 55;
            this.toggleCredentials.Text = "Show Login Credentials";
            this.toggleCredentials.UseVisualStyleBackColor = true;
            this.toggleCredentials.Click += new System.EventHandler(this.OnClicked);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(164, 59);
            this.label4.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 30;
            this.label4.Text = "Password:";
            // 
            // sftpPass
            // 
            this.sftpPass.Location = new System.Drawing.Point(167, 75);
            this.sftpPass.Name = "sftpPass";
            this.sftpPass.Size = new System.Drawing.Size(107, 20);
            this.sftpPass.TabIndex = 29;
            this.sftpPass.UseSystemPasswordChar = true;
            this.sftpPass.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 59);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 28;
            this.label3.Text = "Username:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 19);
            this.label6.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(32, 13);
            this.label6.TabIndex = 53;
            this.label6.Text = "Host:";
            // 
            // sftpValidate
            // 
            this.sftpValidate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.sftpValidate.Location = new System.Drawing.Point(280, 33);
            this.sftpValidate.Name = "sftpValidate";
            this.sftpValidate.Size = new System.Drawing.Size(130, 35);
            this.sftpValidate.TabIndex = 1;
            this.sftpValidate.Text = "Sync";
            this.sftpValidate.UseVisualStyleBackColor = true;
            this.sftpValidate.Click += new System.EventHandler(this.OnClicked);
            // 
            // sftpHost
            // 
            this.sftpHost.Location = new System.Drawing.Point(6, 35);
            this.sftpHost.Name = "sftpHost";
            this.sftpHost.Size = new System.Drawing.Size(220, 20);
            this.sftpHost.TabIndex = 50;
            this.sftpHost.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // sftpUser
            // 
            this.sftpUser.Location = new System.Drawing.Point(6, 75);
            this.sftpUser.Name = "sftpUser";
            this.sftpUser.Size = new System.Drawing.Size(155, 20);
            this.sftpUser.TabIndex = 0;
            this.sftpUser.UseSystemPasswordChar = true;
            this.sftpUser.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // CMainSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.remoteGroup);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.mainGroupMain);
            this.Controls.Add(this.mainGroupTheme);
            this.Controls.Add(this.localGroup);
            this.Name = "CMainSettings";
            this.Size = new System.Drawing.Size(983, 307);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.mainGroupMain.ResumeLayout(false);
            this.mainGroupMain.PerformLayout();
            this.mainGroupTheme.ResumeLayout(false);
            this.mainGroupTheme.PerformLayout();
            this.localGroup.ResumeLayout(false);
            this.localGroup.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.remoteGroup.ResumeLayout(false);
            this.remoteGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox notesEnabled;
        private System.Windows.Forms.GroupBox mainGroupMain;
        private System.Windows.Forms.CheckBox showBasic;
        private System.Windows.Forms.CheckBox layoutDebug;
        private System.Windows.Forms.CheckBox autoVersion;
        private System.Windows.Forms.ComboBox gameVersion;
        private System.Windows.Forms.GroupBox mainGroupTheme;
        private System.Windows.Forms.CheckBox completionGlow;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox theme;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button textColor;
        private System.Windows.Forms.Button borderColor;
        private System.Windows.Forms.Button backColor;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox localGroup;
        private System.Windows.Forms.Button browse;
        private System.Windows.Forms.TextBox customSavePath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox viewMode;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox refreshIcon;
        private System.Windows.Forms.RadioButton savesCustom;
        private System.Windows.Forms.RadioButton savesDefault;
        private System.Windows.Forms.RadioButton worldRemote;
        private System.Windows.Forms.RadioButton worldLocal;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox remoteGroup;
        private System.Windows.Forms.Button sftpValidate;
        private System.Windows.Forms.TextBox sftpUser;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox sftpPass;
        private System.Windows.Forms.Button toggleCredentials;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox sftpHost;
        private System.Windows.Forms.TextBox sftpPort;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox hideCompleted;
        private System.Windows.Forms.LinkLabel sftpCompatibility;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox fpsCap;
    }
}
