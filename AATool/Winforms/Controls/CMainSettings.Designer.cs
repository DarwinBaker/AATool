
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
            this.notesEnabled = new System.Windows.Forms.CheckBox();
            this.mainGroupMain = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.hideCompleted = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.viewMode = new System.Windows.Forms.ComboBox();
            this.showBasic = new System.Windows.Forms.CheckBox();
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.fpsCap = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.mainGroupMain.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // notesEnabled
            // 
            this.notesEnabled.AutoSize = true;
            this.notesEnabled.Location = new System.Drawing.Point(6, 121);
            this.notesEnabled.Name = "notesEnabled";
            this.notesEnabled.Size = new System.Drawing.Size(132, 17);
            this.notesEnabled.TabIndex = 27;
            this.notesEnabled.Text = "Enable Notes Window";
            this.notesEnabled.UseVisualStyleBackColor = true;
            this.notesEnabled.CheckedChanged += new System.EventHandler(this.OnCheckChanged);
            // 
            // mainGroupMain
            // 
            this.mainGroupMain.Controls.Add(this.label12);
            this.mainGroupMain.Controls.Add(this.notesEnabled);
            this.mainGroupMain.Controls.Add(this.hideCompleted);
            this.mainGroupMain.Controls.Add(this.label1);
            this.mainGroupMain.Controls.Add(this.viewMode);
            this.mainGroupMain.Controls.Add(this.showBasic);
            this.mainGroupMain.Location = new System.Drawing.Point(3, 3);
            this.mainGroupMain.Name = "mainGroupMain";
            this.mainGroupMain.Size = new System.Drawing.Size(263, 192);
            this.mainGroupMain.TabIndex = 25;
            this.mainGroupMain.TabStop = false;
            this.mainGroupMain.Text = "Layout";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.label12.Location = new System.Drawing.Point(3, 141);
            this.label12.Margin = new System.Windows.Forms.Padding(0, 0, 3, 10);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(221, 13);
            this.label12.TabIndex = 35;
            this.label12.Text = "🛈 Persistent, auto-cleared, per-world notepad";
            // 
            // hideCompleted
            // 
            this.hideCompleted.AutoSize = true;
            this.hideCompleted.Location = new System.Drawing.Point(6, 95);
            this.hideCompleted.Margin = new System.Windows.Forms.Padding(3, 3, 3, 6);
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
            this.showBasic.Margin = new System.Windows.Forms.Padding(3, 3, 3, 6);
            this.showBasic.Name = "showBasic";
            this.showBasic.Size = new System.Drawing.Size(156, 17);
            this.showBasic.TabIndex = 8;
            this.showBasic.Text = "Show Basic Advancements";
            this.showBasic.UseVisualStyleBackColor = true;
            this.showBasic.CheckedChanged += new System.EventHandler(this.OnCheckChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 73);
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
            this.refreshIcon.Location = new System.Drawing.Point(9, 88);
            this.refreshIcon.Name = "refreshIcon";
            this.refreshIcon.Size = new System.Drawing.Size(120, 21);
            this.refreshIcon.TabIndex = 27;
            this.refreshIcon.SelectedIndexChanged += new System.EventHandler(this.OnIndexChanged);
            // 
            // completionGlow
            // 
            this.completionGlow.AutoSize = true;
            this.completionGlow.Location = new System.Drawing.Point(9, 142);
            this.completionGlow.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
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
            this.label10.Location = new System.Drawing.Point(6, 22);
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
            this.theme.Location = new System.Drawing.Point(9, 37);
            this.theme.Name = "theme";
            this.theme.Size = new System.Drawing.Size(120, 21);
            this.theme.TabIndex = 22;
            this.theme.SelectedIndexChanged += new System.EventHandler(this.OnIndexChanged);
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label8.Location = new System.Drawing.Point(9, 224);
            this.label8.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(32, 13);
            this.label8.TabIndex = 16;
            this.label8.Text = "Back";
            this.label8.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label8.Click += new System.EventHandler(this.Label8_Click);
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label9.Location = new System.Drawing.Point(47, 224);
            this.label9.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(38, 13);
            this.label9.TabIndex = 20;
            this.label9.Text = "Fore";
            this.label9.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label9.Click += new System.EventHandler(this.Label9_Click);
            // 
            // textColor
            // 
            this.textColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textColor.Location = new System.Drawing.Point(88, 240);
            this.textColor.Name = "textColor";
            this.textColor.Size = new System.Drawing.Size(32, 32);
            this.textColor.TabIndex = 17;
            this.textColor.UseVisualStyleBackColor = true;
            this.textColor.Click += new System.EventHandler(this.OnClicked);
            // 
            // borderColor
            // 
            this.borderColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.borderColor.Location = new System.Drawing.Point(49, 240);
            this.borderColor.Name = "borderColor";
            this.borderColor.Size = new System.Drawing.Size(32, 32);
            this.borderColor.TabIndex = 19;
            this.borderColor.UseVisualStyleBackColor = true;
            this.borderColor.Click += new System.EventHandler(this.OnClicked);
            // 
            // backColor
            // 
            this.backColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.backColor.Location = new System.Drawing.Point(9, 240);
            this.backColor.Name = "backColor";
            this.backColor.Size = new System.Drawing.Size(32, 32);
            this.backColor.TabIndex = 15;
            this.backColor.UseVisualStyleBackColor = true;
            this.backColor.Click += new System.EventHandler(this.OnClicked);
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label7.Location = new System.Drawing.Point(88, 224);
            this.label7.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(32, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "Text";
            this.label7.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label7.Click += new System.EventHandler(this.Label7_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.backColor);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.completionGlow);
            this.groupBox1.Controls.Add(this.borderColor);
            this.groupBox1.Controls.Add(this.refreshIcon);
            this.groupBox1.Controls.Add(this.textColor);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.theme);
            this.groupBox1.Location = new System.Drawing.Point(272, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(263, 301);
            this.groupBox1.TabIndex = 27;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Appearance";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.label3.Location = new System.Drawing.Point(6, 112);
            this.label3.Margin = new System.Windows.Forms.Padding(0, 0, 3, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(239, 13);
            this.label3.TabIndex = 37;
            this.label3.Text = "🛈 The indicator shown when the tracker updates";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.label14.Location = new System.Drawing.Point(6, 275);
            this.label14.Margin = new System.Windows.Forms.Padding(0, 0, 3, 10);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(162, 13);
            this.label14.TabIndex = 36;
            this.label14.Text = "🛈 Define a custom color scheme";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.label6.Location = new System.Drawing.Point(6, 162);
            this.label6.Margin = new System.Windows.Forms.Padding(0, 0, 3, 10);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(247, 13);
            this.label6.TabIndex = 35;
            this.label6.Text = "🛈 No performance impact, purely visual preference";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 37);
            this.label5.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 13);
            this.label5.TabIndex = 32;
            this.label5.Text = "FPS Cap:";
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
            this.fpsCap.Location = new System.Drawing.Point(6, 53);
            this.fpsCap.Name = "fpsCap";
            this.fpsCap.Size = new System.Drawing.Size(68, 21);
            this.fpsCap.TabIndex = 32;
            this.fpsCap.SelectedIndexChanged += new System.EventHandler(this.OnIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.label4.Location = new System.Drawing.Point(3, 77);
            this.label4.Margin = new System.Windows.Forms.Padding(0, 0, 3, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(245, 13);
            this.label4.TabIndex = 34;
            this.label4.Text = "🛈 Reduces CPU and GPU load on slower systems";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.fpsCap);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(3, 201);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(263, 103);
            this.groupBox2.TabIndex = 35;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Performance";
            // 
            // CMainSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.mainGroupMain);
            this.Name = "CMainSettings";
            this.Size = new System.Drawing.Size(538, 307);
            this.mainGroupMain.ResumeLayout(false);
            this.mainGroupMain.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.CheckBox notesEnabled;
        private System.Windows.Forms.GroupBox mainGroupMain;
        private System.Windows.Forms.CheckBox showBasic;
        private System.Windows.Forms.CheckBox completionGlow;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox theme;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button textColor;
        private System.Windows.Forms.Button borderColor;
        private System.Windows.Forms.Button backColor;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox viewMode;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox refreshIcon;
        private System.Windows.Forms.CheckBox hideCompleted;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox fpsCap;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}
