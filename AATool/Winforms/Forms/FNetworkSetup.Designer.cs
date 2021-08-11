
namespace AATool.Winforms.Forms
{
    partial class FNetworkSetup
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
            this.next = new System.Windows.Forms.Button();
            this.back = new System.Windows.Forms.Button();
            this.title = new System.Windows.Forms.Label();
            this.mojangName = new System.Windows.Forms.TextBox();
            this.displayName = new System.Windows.Forms.TextBox();
            this.pronouns = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.page0 = new System.Windows.Forms.GroupBox();
            this.face = new System.Windows.Forms.PictureBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.page1 = new System.Windows.Forms.GroupBox();
            this.server = new System.Windows.Forms.RadioButton();
            this.client = new System.Windows.Forms.RadioButton();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.page2 = new System.Windows.Forms.GroupBox();
            this.toggleIP = new System.Windows.Forms.Button();
            this.autoServerIP = new System.Windows.Forms.CheckBox();
            this.portLabel = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.ipLabel = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.ip = new System.Windows.Forms.TextBox();
            this.port = new System.Windows.Forms.TextBox();
            this.keyboardTimer = new System.Windows.Forms.Timer(this.components);
            this.page0.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.face)).BeginInit();
            this.page1.SuspendLayout();
            this.page2.SuspendLayout();
            this.SuspendLayout();
            // 
            // next
            // 
            this.next.Location = new System.Drawing.Point(372, 239);
            this.next.Name = "next";
            this.next.Size = new System.Drawing.Size(100, 30);
            this.next.TabIndex = 2;
            this.next.Text = "Next";
            this.next.UseVisualStyleBackColor = true;
            this.next.Click += new System.EventHandler(this.OnClick);
            // 
            // back
            // 
            this.back.Location = new System.Drawing.Point(12, 239);
            this.back.Name = "back";
            this.back.Size = new System.Drawing.Size(100, 30);
            this.back.TabIndex = 3;
            this.back.Text = "Back";
            this.back.UseVisualStyleBackColor = true;
            this.back.Click += new System.EventHandler(this.OnClick);
            // 
            // title
            // 
            this.title.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.title.Location = new System.Drawing.Point(12, 9);
            this.title.Name = "title";
            this.title.Size = new System.Drawing.Size(460, 29);
            this.title.TabIndex = 4;
            this.title.Text = "Title";
            this.title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mojangName
            // 
            this.mojangName.Location = new System.Drawing.Point(33, 38);
            this.mojangName.Margin = new System.Windows.Forms.Padding(30, 3, 3, 3);
            this.mojangName.Name = "mojangName";
            this.mojangName.Size = new System.Drawing.Size(150, 20);
            this.mojangName.TabIndex = 0;
            this.mojangName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.mojangName.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // displayName
            // 
            this.displayName.Location = new System.Drawing.Point(277, 38);
            this.displayName.Margin = new System.Windows.Forms.Padding(3, 3, 30, 3);
            this.displayName.Name = "displayName";
            this.displayName.Size = new System.Drawing.Size(150, 20);
            this.displayName.TabIndex = 1;
            this.displayName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // pronouns
            // 
            this.pronouns.FormattingEnabled = true;
            this.pronouns.Items.AddRange(new object[] {
            "He/Him",
            "She/Her",
            "They/Them",
            "Write Your Own"});
            this.pronouns.Location = new System.Drawing.Point(189, 135);
            this.pronouns.Name = "pronouns";
            this.pronouns.Size = new System.Drawing.Size(82, 21);
            this.pronouns.TabIndex = 2;
            this.pronouns.Text = "He/Him";
            this.pronouns.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(33, 22);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(150, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Minecraft Name";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(33, 64);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(150, 92);
            this.label3.TabIndex = 5;
            this.label3.Text = "This is your actual Minecraft username. Used to sync your progress and to display" +
    " your skin. Not case sensitive, but otherwise must be typed exactly.";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // page0
            // 
            this.page0.Controls.Add(this.face);
            this.page0.Controls.Add(this.label5);
            this.page0.Controls.Add(this.label4);
            this.page0.Controls.Add(this.label1);
            this.page0.Controls.Add(this.label3);
            this.page0.Controls.Add(this.label2);
            this.page0.Controls.Add(this.mojangName);
            this.page0.Controls.Add(this.displayName);
            this.page0.Controls.Add(this.pronouns);
            this.page0.Location = new System.Drawing.Point(12, 41);
            this.page0.Name = "page0";
            this.page0.Size = new System.Drawing.Size(460, 192);
            this.page0.TabIndex = 8;
            this.page0.TabStop = false;
            this.page0.Tag = "Who are you?";
            // 
            // face
            // 
            this.face.Location = new System.Drawing.Point(205, 23);
            this.face.Name = "face";
            this.face.Size = new System.Drawing.Size(48, 48);
            this.face.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.face.TabIndex = 10;
            this.face.TabStop = false;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(189, 119);
            this.label5.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(82, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Your Pronouns";
            this.label5.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(277, 64);
            this.label4.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(150, 68);
            this.label4.TabIndex = 7;
            this.label4.Text = "This is optional, and allows you to use a name that\'s different from your Minecra" +
    "ft user name. Great for removing unwanted number suffixes.";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(277, 22);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(150, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Display Name";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // page1
            // 
            this.page1.Controls.Add(this.server);
            this.page1.Controls.Add(this.client);
            this.page1.Controls.Add(this.label7);
            this.page1.Controls.Add(this.label6);
            this.page1.Location = new System.Drawing.Point(540, 41);
            this.page1.Name = "page1";
            this.page1.Size = new System.Drawing.Size(460, 192);
            this.page1.TabIndex = 9;
            this.page1.TabStop = false;
            this.page1.Tag = "What\'s your network configuration?";
            // 
            // server
            // 
            this.server.Appearance = System.Windows.Forms.Appearance.Button;
            this.server.Location = new System.Drawing.Point(277, 38);
            this.server.Margin = new System.Windows.Forms.Padding(3, 3, 30, 3);
            this.server.Name = "server";
            this.server.Size = new System.Drawing.Size(150, 50);
            this.server.TabIndex = 9;
            this.server.TabStop = true;
            this.server.Text = "I\'m Hosting the Server";
            this.server.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.server.UseVisualStyleBackColor = true;
            // 
            // client
            // 
            this.client.Appearance = System.Windows.Forms.Appearance.Button;
            this.client.Location = new System.Drawing.Point(36, 38);
            this.client.Margin = new System.Windows.Forms.Padding(30, 3, 3, 3);
            this.client.Name = "client";
            this.client.Size = new System.Drawing.Size(150, 50);
            this.client.TabIndex = 8;
            this.client.TabStop = true;
            this.client.Text = "I\'m Joining the Server";
            this.client.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.client.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(277, 94);
            this.label7.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(150, 46);
            this.label7.TabIndex = 7;
            this.label7.Text = "Choose this if you\'re the one clicking \"Open to LAN\" in Minecraft.";
            this.label7.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(36, 94);
            this.label6.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(150, 46);
            this.label6.TabIndex = 6;
            this.label6.Text = "Choose this if you\'re going to be connecting to your friend\'s Minecraft server.";
            this.label6.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // page2
            // 
            this.page2.Controls.Add(this.toggleIP);
            this.page2.Controls.Add(this.autoServerIP);
            this.page2.Controls.Add(this.portLabel);
            this.page2.Controls.Add(this.label9);
            this.page2.Controls.Add(this.ipLabel);
            this.page2.Controls.Add(this.label11);
            this.page2.Controls.Add(this.ip);
            this.page2.Controls.Add(this.port);
            this.page2.Location = new System.Drawing.Point(1006, 41);
            this.page2.Name = "page2";
            this.page2.Size = new System.Drawing.Size(460, 192);
            this.page2.TabIndex = 10;
            this.page2.TabStop = false;
            this.page2.Tag = "What\'s your network configuration?";
            // 
            // toggleIP
            // 
            this.toggleIP.Location = new System.Drawing.Point(33, 64);
            this.toggleIP.Name = "toggleIP";
            this.toggleIP.Size = new System.Drawing.Size(150, 23);
            this.toggleIP.TabIndex = 44;
            this.toggleIP.Text = "Show IP Address";
            this.toggleIP.UseVisualStyleBackColor = true;
            this.toggleIP.Click += new System.EventHandler(this.OnClick);
            // 
            // autoServerIP
            // 
            this.autoServerIP.AutoSize = true;
            this.autoServerIP.Location = new System.Drawing.Point(189, 41);
            this.autoServerIP.Name = "autoServerIP";
            this.autoServerIP.Size = new System.Drawing.Size(48, 17);
            this.autoServerIP.TabIndex = 43;
            this.autoServerIP.Text = "Auto";
            this.autoServerIP.UseVisualStyleBackColor = true;
            this.autoServerIP.CheckedChanged += new System.EventHandler(this.OnCheckedChanged);
            // 
            // portLabel
            // 
            this.portLabel.Location = new System.Drawing.Point(277, 64);
            this.portLabel.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.portLabel.Name = "portLabel";
            this.portLabel.Size = new System.Drawing.Size(150, 125);
            this.portLabel.TabIndex = 13;
            this.portLabel.Text = "Port Label";
            this.portLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(277, 22);
            this.label9.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(150, 13);
            this.label9.TabIndex = 12;
            this.label9.Text = "Port";
            this.label9.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // ipLabel
            // 
            this.ipLabel.Location = new System.Drawing.Point(33, 93);
            this.ipLabel.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.ipLabel.Name = "ipLabel";
            this.ipLabel.Size = new System.Drawing.Size(150, 96);
            this.ipLabel.TabIndex = 11;
            this.ipLabel.Text = "IP Label";
            this.ipLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(33, 22);
            this.label11.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(150, 13);
            this.label11.TabIndex = 10;
            this.label11.Text = "IP Address";
            this.label11.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // ip
            // 
            this.ip.Location = new System.Drawing.Point(33, 38);
            this.ip.Margin = new System.Windows.Forms.Padding(30, 3, 3, 3);
            this.ip.Name = "ip";
            this.ip.Size = new System.Drawing.Size(150, 20);
            this.ip.TabIndex = 8;
            this.ip.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ip.UseSystemPasswordChar = true;
            this.ip.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // port
            // 
            this.port.Location = new System.Drawing.Point(277, 38);
            this.port.Margin = new System.Windows.Forms.Padding(3, 3, 30, 3);
            this.port.Name = "port";
            this.port.Size = new System.Drawing.Size(150, 20);
            this.port.TabIndex = 9;
            this.port.Text = "25562";
            this.port.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.port.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // keyboardTimer
            // 
            this.keyboardTimer.Interval = 500;
            this.keyboardTimer.Tick += new System.EventHandler(this.OnTimerTick);
            // 
            // FNetworkSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1491, 281);
            this.Controls.Add(this.page2);
            this.Controls.Add(this.page1);
            this.Controls.Add(this.back);
            this.Controls.Add(this.page0);
            this.Controls.Add(this.title);
            this.Controls.Add(this.next);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FNetworkSetup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Co-op Setup Assistant";
            this.page0.ResumeLayout(false);
            this.page0.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.face)).EndInit();
            this.page1.ResumeLayout(false);
            this.page2.ResumeLayout(false);
            this.page2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button next;
        private System.Windows.Forms.Button back;
        private System.Windows.Forms.Label title;
        private System.Windows.Forms.ComboBox pronouns;
        private System.Windows.Forms.TextBox displayName;
        private System.Windows.Forms.TextBox mojangName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox page0;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.PictureBox face;
        private System.Windows.Forms.GroupBox page1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.RadioButton client;
        private System.Windows.Forms.RadioButton server;
        private System.Windows.Forms.GroupBox page2;
        private System.Windows.Forms.Label portLabel;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label ipLabel;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox ip;
        private System.Windows.Forms.TextBox port;
        private System.Windows.Forms.Button toggleIP;
        private System.Windows.Forms.Timer keyboardTimer;
        private System.Windows.Forms.CheckBox autoServerIP;
    }
}