
namespace AATool.Winforms.Controls
{
    partial class CNetworkSettings
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
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.kick = new System.Windows.Forms.Button();
            this.runSetup = new System.Windows.Forms.Button();
            this.peers = new System.Windows.Forms.ListView();
            this.label13 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.networkType = new System.Windows.Forms.ComboBox();
            this.networkSwitch = new System.Windows.Forms.Button();
            this.toggleIP = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.togglePassword = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.password = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.pronouns = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.autoServerIP = new System.Windows.Forms.CheckBox();
            this.displayName = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.port = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.ip = new System.Windows.Forms.TextBox();
            this.mojangName = new System.Windows.Forms.TextBox();
            this.mainGroupNetwork = new System.Windows.Forms.GroupBox();
            this.console = new System.Windows.Forms.RichTextBox();
            this.help = new System.Windows.Forms.LinkLabel();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.mainGroupNetwork.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.kick);
            this.groupBox4.Controls.Add(this.runSetup);
            this.groupBox4.Controls.Add(this.peers);
            this.groupBox4.Controls.Add(this.label13);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.networkType);
            this.groupBox4.Controls.Add(this.networkSwitch);
            this.groupBox4.Location = new System.Drawing.Point(3, 151);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(309, 153);
            this.groupBox4.TabIndex = 15;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Control Panel";
            // 
            // kick
            // 
            this.kick.Location = new System.Drawing.Point(138, 124);
            this.kick.Name = "kick";
            this.kick.Size = new System.Drawing.Size(47, 23);
            this.kick.TabIndex = 46;
            this.kick.Text = "Kick";
            this.kick.UseVisualStyleBackColor = true;
            this.kick.Click += new System.EventHandler(this.OnClick);
            // 
            // runSetup
            // 
            this.runSetup.Location = new System.Drawing.Point(9, 101);
            this.runSetup.Name = "runSetup";
            this.runSetup.Size = new System.Drawing.Size(98, 45);
            this.runSetup.TabIndex = 10;
            this.runSetup.Text = "Run Setup Assistant";
            this.runSetup.UseVisualStyleBackColor = true;
            this.runSetup.Click += new System.EventHandler(this.OnClick);
            // 
            // peers
            // 
            this.peers.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.peers.GridLines = true;
            this.peers.HideSelection = false;
            this.peers.Location = new System.Drawing.Point(189, 35);
            this.peers.Name = "peers";
            this.peers.Size = new System.Drawing.Size(113, 111);
            this.peers.TabIndex = 45;
            this.peers.UseCompatibleStateImageBehavior = false;
            this.peers.View = System.Windows.Forms.View.List;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(186, 19);
            this.label13.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(99, 13);
            this.label13.TabIndex = 44;
            this.label13.Text = "Connected Players:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 46);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 24;
            this.label1.Text = "Network Type:";
            // 
            // networkType
            // 
            this.networkType.DisplayMember = "Client";
            this.networkType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.networkType.FormattingEnabled = true;
            this.networkType.Items.AddRange(new object[] {
            "Client",
            "Server"});
            this.networkType.Location = new System.Drawing.Point(9, 62);
            this.networkType.Name = "networkType";
            this.networkType.Size = new System.Drawing.Size(98, 21);
            this.networkType.TabIndex = 9;
            this.networkType.ValueMember = "Client";
            this.networkType.SelectedIndexChanged += new System.EventHandler(this.OnIndexChanged);
            // 
            // networkSwitch
            // 
            this.networkSwitch.Location = new System.Drawing.Point(9, 19);
            this.networkSwitch.Name = "networkSwitch";
            this.networkSwitch.Size = new System.Drawing.Size(98, 23);
            this.networkSwitch.TabIndex = 8;
            this.networkSwitch.Text = "Connect";
            this.networkSwitch.UseVisualStyleBackColor = true;
            this.networkSwitch.EnabledChanged += new System.EventHandler(this.OnEnabledChanged);
            this.networkSwitch.Click += new System.EventHandler(this.OnClick);
            // 
            // toggleIP
            // 
            this.toggleIP.Location = new System.Drawing.Point(9, 110);
            this.toggleIP.Name = "toggleIP";
            this.toggleIP.Size = new System.Drawing.Size(98, 22);
            this.toggleIP.TabIndex = 7;
            this.toggleIP.Text = "Show IP Address";
            this.toggleIP.UseVisualStyleBackColor = true;
            this.toggleIP.Click += new System.EventHandler(this.OnClick);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.togglePassword);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.password);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.pronouns);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.autoServerIP);
            this.groupBox3.Controls.Add(this.toggleIP);
            this.groupBox3.Controls.Add(this.displayName);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.port);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.ip);
            this.groupBox3.Controls.Add(this.mojangName);
            this.groupBox3.Location = new System.Drawing.Point(3, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(309, 142);
            this.groupBox3.TabIndex = 14;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Network Configuration";
            // 
            // togglePassword
            // 
            this.togglePassword.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.togglePassword.Location = new System.Drawing.Point(267, 82);
            this.togglePassword.Name = "togglePassword";
            this.togglePassword.Size = new System.Drawing.Size(36, 22);
            this.togglePassword.TabIndex = 49;
            this.togglePassword.Text = "Show";
            this.togglePassword.UseVisualStyleBackColor = true;
            this.togglePassword.Click += new System.EventHandler(this.OnClick);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(158, 67);
            this.label4.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 48;
            this.label4.Text = "Password:";
            // 
            // password
            // 
            this.password.Location = new System.Drawing.Point(161, 83);
            this.password.Name = "password";
            this.password.Size = new System.Drawing.Size(100, 20);
            this.password.TabIndex = 47;
            this.password.UseSystemPasswordChar = true;
            this.password.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(214, 22);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 46;
            this.label3.Text = "Pronouns:";
            // 
            // pronouns
            // 
            this.pronouns.FormattingEnabled = true;
            this.pronouns.Items.AddRange(new object[] {
            "He/Him",
            "She/Her",
            "They/Them",
            "Write Your Own"});
            this.pronouns.Location = new System.Drawing.Point(217, 38);
            this.pronouns.Name = "pronouns";
            this.pronouns.Size = new System.Drawing.Size(86, 21);
            this.pronouns.TabIndex = 3;
            this.pronouns.Text = "He/Him";
            this.pronouns.SelectedIndexChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(110, 22);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 13);
            this.label2.TabIndex = 44;
            this.label2.Text = "Display Name:";
            // 
            // autoServerIP
            // 
            this.autoServerIP.AutoSize = true;
            this.autoServerIP.Location = new System.Drawing.Point(113, 113);
            this.autoServerIP.Name = "autoServerIP";
            this.autoServerIP.Size = new System.Drawing.Size(166, 17);
            this.autoServerIP.TabIndex = 6;
            this.autoServerIP.Text = "Auto-detect local IP for server";
            this.autoServerIP.UseVisualStyleBackColor = true;
            this.autoServerIP.CheckedChanged += new System.EventHandler(this.OnCheckChanged);
            // 
            // displayName
            // 
            this.displayName.Location = new System.Drawing.Point(113, 38);
            this.displayName.Name = "displayName";
            this.displayName.Size = new System.Drawing.Size(98, 20);
            this.displayName.TabIndex = 2;
            this.displayName.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 22);
            this.label12.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(76, 13);
            this.label12.TabIndex = 36;
            this.label12.Text = "Mojang Name:";
            // 
            // port
            // 
            this.port.Location = new System.Drawing.Point(113, 83);
            this.port.Name = "port";
            this.port.Size = new System.Drawing.Size(42, 20);
            this.port.TabIndex = 5;
            this.port.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 67);
            this.label6.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(61, 13);
            this.label6.TabIndex = 34;
            this.label6.Text = "IP Address:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(110, 67);
            this.label11.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(29, 13);
            this.label11.TabIndex = 37;
            this.label11.Text = "Port:";
            // 
            // ip
            // 
            this.ip.Location = new System.Drawing.Point(9, 83);
            this.ip.Name = "ip";
            this.ip.Size = new System.Drawing.Size(98, 20);
            this.ip.TabIndex = 4;
            this.ip.UseSystemPasswordChar = true;
            this.ip.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // mojangName
            // 
            this.mojangName.Location = new System.Drawing.Point(9, 38);
            this.mojangName.Name = "mojangName";
            this.mojangName.Size = new System.Drawing.Size(98, 20);
            this.mojangName.TabIndex = 1;
            this.mojangName.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // mainGroupNetwork
            // 
            this.mainGroupNetwork.Controls.Add(this.console);
            this.mainGroupNetwork.Location = new System.Drawing.Point(318, 3);
            this.mainGroupNetwork.Name = "mainGroupNetwork";
            this.mainGroupNetwork.Size = new System.Drawing.Size(215, 285);
            this.mainGroupNetwork.TabIndex = 13;
            this.mainGroupNetwork.TabStop = false;
            // 
            // console
            // 
            this.console.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(33)))), ((int)(((byte)(96)))));
            this.console.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.console.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.console.ForeColor = System.Drawing.SystemColors.Window;
            this.console.Location = new System.Drawing.Point(2, 8);
            this.console.Name = "console";
            this.console.ReadOnly = true;
            this.console.Size = new System.Drawing.Size(211, 274);
            this.console.TabIndex = 41;
            this.console.Text = "";
            // 
            // help
            // 
            this.help.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.help.AutoSize = true;
            this.help.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.help.Location = new System.Drawing.Point(400, 292);
            this.help.Name = "help";
            this.help.Size = new System.Drawing.Size(130, 13);
            this.help.TabIndex = 33;
            this.help.TabStop = true;
            this.help.Text = "Setup guide coming soon!";
            // 
            // CNetworkSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.help);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.mainGroupNetwork);
            this.Name = "CNetworkSettings";
            this.Size = new System.Drawing.Size(538, 307);
            this.Load += new System.EventHandler(this.OnLoad);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.mainGroupNetwork.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button toggleIP;
        private System.Windows.Forms.ComboBox networkType;
        private System.Windows.Forms.Button networkSwitch;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox port;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox ip;
        private System.Windows.Forms.TextBox mojangName;
        private System.Windows.Forms.GroupBox mainGroupNetwork;
        private System.Windows.Forms.RichTextBox console;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox displayName;
        private System.Windows.Forms.ComboBox pronouns;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListView peers;
        private System.Windows.Forms.Button runSetup;
        private System.Windows.Forms.LinkLabel help;
        private System.Windows.Forms.CheckBox autoServerIP;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox password;
        private System.Windows.Forms.Button togglePassword;
        private System.Windows.Forms.Button kick;
    }
}
