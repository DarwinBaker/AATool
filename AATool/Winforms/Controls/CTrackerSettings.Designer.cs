
namespace AATool.Winforms.Controls
{
    partial class CTrackerSettings
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
            this.autoVersion = new System.Windows.Forms.CheckBox();
            this.gameVersion = new System.Windows.Forms.ComboBox();
            this.localGroup = new System.Windows.Forms.GroupBox();
            this.savesCustom = new System.Windows.Forms.RadioButton();
            this.savesDefault = new System.Windows.Forms.RadioButton();
            this.browse = new System.Windows.Forms.Button();
            this.customSavePath = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.sftpCompatibility = new System.Windows.Forms.LinkLabel();
            this.worldLocal = new System.Windows.Forms.RadioButton();
            this.worldRemote = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.remoteGroup = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.sftpRoot = new System.Windows.Forms.TextBox();
            this.sftpPort = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.toggleCredentials = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.sftpValidate = new System.Windows.Forms.Button();
            this.sftpPass = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.sftpHost = new System.Windows.Forms.TextBox();
            this.sftpUser = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.category = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.serverPathHelp = new System.Windows.Forms.LinkLabel();
            this.localGroup.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.remoteGroup.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // autoVersion
            // 
            this.autoVersion.AutoSize = true;
            this.autoVersion.Location = new System.Drawing.Point(9, 50);
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
            this.gameVersion.Location = new System.Drawing.Point(9, 22);
            this.gameVersion.Name = "gameVersion";
            this.gameVersion.Size = new System.Drawing.Size(96, 21);
            this.gameVersion.TabIndex = 18;
            this.gameVersion.SelectedIndexChanged += new System.EventHandler(this.OnIndexChanged);
            // 
            // localGroup
            // 
            this.localGroup.Controls.Add(this.savesCustom);
            this.localGroup.Controls.Add(this.savesDefault);
            this.localGroup.Controls.Add(this.browse);
            this.localGroup.Controls.Add(this.customSavePath);
            this.localGroup.Location = new System.Drawing.Point(116, 90);
            this.localGroup.Name = "localGroup";
            this.localGroup.Size = new System.Drawing.Size(417, 104);
            this.localGroup.TabIndex = 23;
            this.localGroup.TabStop = false;
            this.localGroup.Text = "Local Save Folder";
            // 
            // savesCustom
            // 
            this.savesCustom.AutoSize = true;
            this.savesCustom.Location = new System.Drawing.Point(6, 53);
            this.savesCustom.Name = "savesCustom";
            this.savesCustom.Size = new System.Drawing.Size(145, 17);
            this.savesCustom.TabIndex = 8;
            this.savesCustom.TabStop = true;
            this.savesCustom.Text = "Use Custom Save Folder:";
            this.savesCustom.UseVisualStyleBackColor = true;
            this.savesCustom.CheckedChanged += new System.EventHandler(this.OnCheckChanged);
            // 
            // savesDefault
            // 
            this.savesDefault.AutoSize = true;
            this.savesDefault.Location = new System.Drawing.Point(6, 27);
            this.savesDefault.Margin = new System.Windows.Forms.Padding(3, 3, 3, 6);
            this.savesDefault.Name = "savesDefault";
            this.savesDefault.Size = new System.Drawing.Size(347, 17);
            this.savesDefault.TabIndex = 7;
            this.savesDefault.TabStop = true;
            this.savesDefault.Text = "Use Default Save Folder:     (...AppData\\Roaming\\.minecraft\\saves)";
            this.savesDefault.UseVisualStyleBackColor = true;
            this.savesDefault.CheckedChanged += new System.EventHandler(this.OnCheckChanged);
            // 
            // browse
            // 
            this.browse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.browse.Location = new System.Drawing.Point(358, 74);
            this.browse.Name = "browse";
            this.browse.Size = new System.Drawing.Size(53, 22);
            this.browse.TabIndex = 1;
            this.browse.Text = "Browse";
            this.browse.UseVisualStyleBackColor = true;
            this.browse.Click += new System.EventHandler(this.OnClicked);
            // 
            // customSavePath
            // 
            this.customSavePath.Location = new System.Drawing.Point(6, 75);
            this.customSavePath.Name = "customSavePath";
            this.customSavePath.Size = new System.Drawing.Size(346, 20);
            this.customSavePath.TabIndex = 0;
            this.customSavePath.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.radioButton1);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.sftpCompatibility);
            this.groupBox3.Controls.Add(this.worldLocal);
            this.groupBox3.Controls.Add(this.worldRemote);
            this.groupBox3.Location = new System.Drawing.Point(3, 90);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(107, 214);
            this.groupBox3.TabIndex = 29;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "World Type";
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Enabled = false;
            this.radioButton1.Location = new System.Drawing.Point(6, 79);
            this.radioButton1.Margin = new System.Windows.Forms.Padding(3, 3, 3, 6);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(91, 17);
            this.radioButton1.TabIndex = 58;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Multi-Instance";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.label2.Location = new System.Drawing.Point(3, 110);
            this.label2.Margin = new System.Windows.Forms.Padding(0, 0, 3, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 71);
            this.label2.TabIndex = 57;
            this.label2.Text = "🛈 Multi/Wall support is currently in development :D";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // sftpCompatibility
            // 
            this.sftpCompatibility.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.sftpCompatibility.Location = new System.Drawing.Point(3, 193);
            this.sftpCompatibility.Name = "sftpCompatibility";
            this.sftpCompatibility.Size = new System.Drawing.Size(101, 18);
            this.sftpCompatibility.TabIndex = 56;
            this.sftpCompatibility.TabStop = true;
            this.sftpCompatibility.Text = "SFTP compatibility";
            this.sftpCompatibility.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.sftpCompatibility.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnLinkClicked);
            // 
            // worldLocal
            // 
            this.worldLocal.AutoSize = true;
            this.worldLocal.Location = new System.Drawing.Point(6, 27);
            this.worldLocal.Margin = new System.Windows.Forms.Padding(3, 3, 3, 6);
            this.worldLocal.Name = "worldLocal";
            this.worldLocal.Size = new System.Drawing.Size(51, 17);
            this.worldLocal.TabIndex = 35;
            this.worldLocal.TabStop = true;
            this.worldLocal.Text = "Local";
            this.worldLocal.UseVisualStyleBackColor = true;
            this.worldLocal.CheckedChanged += new System.EventHandler(this.OnCheckChanged);
            // 
            // worldRemote
            // 
            this.worldRemote.AutoSize = true;
            this.worldRemote.Location = new System.Drawing.Point(6, 53);
            this.worldRemote.Margin = new System.Windows.Forms.Padding(3, 3, 3, 6);
            this.worldRemote.Name = "worldRemote";
            this.worldRemote.Size = new System.Drawing.Size(98, 17);
            this.worldRemote.TabIndex = 36;
            this.worldRemote.TabStop = true;
            this.worldRemote.Text = "Remote (SFTP)";
            this.worldRemote.UseVisualStyleBackColor = true;
            this.worldRemote.CheckedChanged += new System.EventHandler(this.OnCheckChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.autoVersion);
            this.groupBox1.Controls.Add(this.gameVersion);
            this.groupBox1.Location = new System.Drawing.Point(164, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(113, 81);
            this.groupBox1.TabIndex = 30;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Version";
            // 
            // remoteGroup
            // 
            this.remoteGroup.Controls.Add(this.serverPathHelp);
            this.remoteGroup.Controls.Add(this.label5);
            this.remoteGroup.Controls.Add(this.sftpRoot);
            this.remoteGroup.Controls.Add(this.sftpPort);
            this.remoteGroup.Controls.Add(this.label11);
            this.remoteGroup.Controls.Add(this.toggleCredentials);
            this.remoteGroup.Controls.Add(this.label4);
            this.remoteGroup.Controls.Add(this.sftpValidate);
            this.remoteGroup.Controls.Add(this.sftpPass);
            this.remoteGroup.Controls.Add(this.label3);
            this.remoteGroup.Controls.Add(this.label6);
            this.remoteGroup.Controls.Add(this.sftpHost);
            this.remoteGroup.Controls.Add(this.sftpUser);
            this.remoteGroup.Location = new System.Drawing.Point(116, 200);
            this.remoteGroup.Name = "remoteGroup";
            this.remoteGroup.Size = new System.Drawing.Size(417, 104);
            this.remoteGroup.TabIndex = 31;
            this.remoteGroup.TabStop = false;
            this.remoteGroup.Text = "Remote Server Login";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(87, 3);
            this.label5.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(66, 13);
            this.label5.TabIndex = 57;
            this.label5.Text = "Server Path:";
            // 
            // sftpRoot
            // 
            this.sftpRoot.Location = new System.Drawing.Point(262, 34);
            this.sftpRoot.Name = "sftpRoot";
            this.sftpRoot.Size = new System.Drawing.Size(63, 20);
            this.sftpRoot.TabIndex = 56;
            this.sftpRoot.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // sftpPort
            // 
            this.sftpPort.Location = new System.Drawing.Point(225, 34);
            this.sftpPort.Name = "sftpPort";
            this.sftpPort.Size = new System.Drawing.Size(31, 20);
            this.sftpPort.TabIndex = 38;
            this.sftpPort.Text = "7767";
            this.sftpPort.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(222, 18);
            this.label11.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(29, 13);
            this.label11.TabIndex = 39;
            this.label11.Text = "Port:";
            // 
            // toggleCredentials
            // 
            this.toggleCredentials.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.toggleCredentials.Location = new System.Drawing.Point(331, 73);
            this.toggleCredentials.Name = "toggleCredentials";
            this.toggleCredentials.Size = new System.Drawing.Size(80, 22);
            this.toggleCredentials.TabIndex = 55;
            this.toggleCredentials.Text = "Show Login";
            this.toggleCredentials.UseVisualStyleBackColor = true;
            this.toggleCredentials.Click += new System.EventHandler(this.OnClicked);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(172, 58);
            this.label4.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 30;
            this.label4.Text = "Password:";
            // 
            // sftpValidate
            // 
            this.sftpValidate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.sftpValidate.Location = new System.Drawing.Point(331, 33);
            this.sftpValidate.Name = "sftpValidate";
            this.sftpValidate.Size = new System.Drawing.Size(80, 22);
            this.sftpValidate.TabIndex = 1;
            this.sftpValidate.Text = "Sync";
            this.sftpValidate.UseVisualStyleBackColor = true;
            this.sftpValidate.Click += new System.EventHandler(this.OnClicked);
            // 
            // sftpPass
            // 
            this.sftpPass.Location = new System.Drawing.Point(175, 74);
            this.sftpPass.Name = "sftpPass";
            this.sftpPass.Size = new System.Drawing.Size(150, 20);
            this.sftpPass.TabIndex = 29;
            this.sftpPass.UseSystemPasswordChar = true;
            this.sftpPass.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 58);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 28;
            this.label3.Text = "Username:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 18);
            this.label6.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(32, 13);
            this.label6.TabIndex = 53;
            this.label6.Text = "Host:";
            // 
            // sftpHost
            // 
            this.sftpHost.Location = new System.Drawing.Point(9, 34);
            this.sftpHost.Name = "sftpHost";
            this.sftpHost.Size = new System.Drawing.Size(210, 20);
            this.sftpHost.TabIndex = 50;
            this.sftpHost.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // sftpUser
            // 
            this.sftpUser.Location = new System.Drawing.Point(9, 74);
            this.sftpUser.Name = "sftpUser";
            this.sftpUser.Size = new System.Drawing.Size(160, 20);
            this.sftpUser.TabIndex = 0;
            this.sftpUser.UseSystemPasswordChar = true;
            this.sftpUser.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.category);
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(154, 81);
            this.groupBox2.TabIndex = 32;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Category";
            // 
            // category
            // 
            this.category.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.category.Enabled = false;
            this.category.FormattingEnabled = true;
            this.category.Items.AddRange(new object[] {
            "All Advancements",
            "Half Percent",
            "All Achievements",
            "Monsters Hunted",
            "Adventuring Time",
            "Balanced Diet"});
            this.category.Location = new System.Drawing.Point(8, 22);
            this.category.Name = "category";
            this.category.Size = new System.Drawing.Size(137, 21);
            this.category.TabIndex = 18;
            this.category.SelectedIndexChanged += new System.EventHandler(this.OnIndexChanged);
            // 
            // label1
            // 
            this.label1.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.label1.Location = new System.Drawing.Point(280, 8);
            this.label1.Margin = new System.Windows.Forms.Padding(0, 0, 3, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(255, 81);
            this.label1.TabIndex = 36;
            this.label1.Text = "🛈 Changes to settings are applied instantly";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // serverPathHelp
            // 
            this.serverPathHelp.AutoSize = true;
            this.serverPathHelp.Location = new System.Drawing.Point(261, 18);
            this.serverPathHelp.Name = "serverPathHelp";
            this.serverPathHelp.Size = new System.Drawing.Size(66, 13);
            this.serverPathHelp.TabIndex = 58;
            this.serverPathHelp.TabStop = true;
            this.serverPathHelp.Text = "Server Path:";
            this.serverPathHelp.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.serverPathHelp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnLinkClicked);
            // 
            // CTrackerSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.remoteGroup);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.localGroup);
            this.Name = "CTrackerSettings";
            this.Size = new System.Drawing.Size(538, 307);
            this.localGroup.ResumeLayout(false);
            this.localGroup.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.remoteGroup.ResumeLayout(false);
            this.remoteGroup.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.CheckBox autoVersion;
        private System.Windows.Forms.ComboBox gameVersion;
        private System.Windows.Forms.GroupBox localGroup;
        private System.Windows.Forms.Button browse;
        private System.Windows.Forms.TextBox customSavePath;
        private System.Windows.Forms.RadioButton savesCustom;
        private System.Windows.Forms.RadioButton savesDefault;
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
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox category;
        private System.Windows.Forms.RadioButton worldLocal;
        private System.Windows.Forms.RadioButton worldRemote;
        private System.Windows.Forms.LinkLabel sftpCompatibility;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox sftpRoot;
        private System.Windows.Forms.LinkLabel serverPathHelp;
    }
}
