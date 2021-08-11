
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
            this.label1 = new System.Windows.Forms.Label();
            this.viewMode = new System.Windows.Forms.ComboBox();
            this.showBasic = new System.Windows.Forms.CheckBox();
            this.layoutDebug = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.autoVersion = new System.Windows.Forms.CheckBox();
            this.gameVersion = new System.Windows.Forms.ComboBox();
            this.mainGroupTheme = new System.Windows.Forms.GroupBox();
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
            this.mainGroupBasic = new System.Windows.Forms.GroupBox();
            this.savesCustom = new System.Windows.Forms.RadioButton();
            this.savesDefault = new System.Windows.Forms.RadioButton();
            this.browse = new System.Windows.Forms.Button();
            this.customSavePath = new System.Windows.Forms.TextBox();
            this.groupBox2.SuspendLayout();
            this.mainGroupMain.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.mainGroupTheme.SuspendLayout();
            this.mainGroupBasic.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.notesEnabled);
            this.groupBox2.Location = new System.Drawing.Point(3, 237);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 67);
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
            this.mainGroupMain.Controls.Add(this.label1);
            this.mainGroupMain.Controls.Add(this.viewMode);
            this.mainGroupMain.Controls.Add(this.showBasic);
            this.mainGroupMain.Controls.Add(this.layoutDebug);
            this.mainGroupMain.Location = new System.Drawing.Point(3, 104);
            this.mainGroupMain.Name = "mainGroupMain";
            this.mainGroupMain.Size = new System.Drawing.Size(200, 128);
            this.mainGroupMain.TabIndex = 25;
            this.mainGroupMain.TabStop = false;
            this.mainGroupMain.Text = "General";
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
            this.layoutDebug.Location = new System.Drawing.Point(6, 91);
            this.layoutDebug.Name = "layoutDebug";
            this.layoutDebug.Size = new System.Drawing.Size(119, 17);
            this.layoutDebug.TabIndex = 25;
            this.layoutDebug.Text = "Layout Debug View";
            this.layoutDebug.UseVisualStyleBackColor = true;
            this.layoutDebug.CheckedChanged += new System.EventHandler(this.OnCheckChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.autoVersion);
            this.groupBox1.Controls.Add(this.gameVersion);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(95, 95);
            this.groupBox1.TabIndex = 27;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Game Version";
            // 
            // autoVersion
            // 
            this.autoVersion.AutoSize = true;
            this.autoVersion.Location = new System.Drawing.Point(6, 20);
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
            this.gameVersion.Location = new System.Drawing.Point(6, 43);
            this.gameVersion.Name = "gameVersion";
            this.gameVersion.Size = new System.Drawing.Size(83, 21);
            this.gameVersion.TabIndex = 18;
            this.gameVersion.SelectedIndexChanged += new System.EventHandler(this.OnIndexChanged);
            // 
            // mainGroupTheme
            // 
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
            this.mainGroupTheme.Location = new System.Drawing.Point(209, 104);
            this.mainGroupTheme.Name = "mainGroupTheme";
            this.mainGroupTheme.Size = new System.Drawing.Size(324, 200);
            this.mainGroupTheme.TabIndex = 24;
            this.mainGroupTheme.TabStop = false;
            this.mainGroupTheme.Text = "Appearance";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(117, 45);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 13);
            this.label2.TabIndex = 28;
            this.label2.Text = "Refresh Indicator:";
            // 
            // refreshIcon
            // 
            this.refreshIcon.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.refreshIcon.FormattingEnabled = true;
            this.refreshIcon.Items.AddRange(new object[] {
            "Xp Orb",
            "Compass"});
            this.refreshIcon.Location = new System.Drawing.Point(120, 60);
            this.refreshIcon.Name = "refreshIcon";
            this.refreshIcon.Size = new System.Drawing.Size(108, 21);
            this.refreshIcon.TabIndex = 27;
            this.refreshIcon.SelectedIndexChanged += new System.EventHandler(this.OnIndexChanged);
            // 
            // completionGlow
            // 
            this.completionGlow.AutoSize = true;
            this.completionGlow.Location = new System.Drawing.Point(6, 19);
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
            this.label10.Location = new System.Drawing.Point(3, 45);
            this.label10.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(43, 13);
            this.label10.TabIndex = 23;
            this.label10.Text = "Theme:";
            // 
            // theme
            // 
            this.theme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.theme.FormattingEnabled = true;
            this.theme.Location = new System.Drawing.Point(6, 60);
            this.theme.Name = "theme";
            this.theme.Size = new System.Drawing.Size(108, 21);
            this.theme.TabIndex = 22;
            this.theme.SelectedIndexChanged += new System.EventHandler(this.OnIndexChanged);
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(6, 146);
            this.label8.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(32, 13);
            this.label8.TabIndex = 16;
            this.label8.Text = "Back";
            this.label8.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(42, 146);
            this.label9.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(38, 13);
            this.label9.TabIndex = 20;
            this.label9.Text = "Fore";
            this.label9.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // textColor
            // 
            this.textColor.Location = new System.Drawing.Point(82, 162);
            this.textColor.Name = "textColor";
            this.textColor.Size = new System.Drawing.Size(32, 32);
            this.textColor.TabIndex = 17;
            this.textColor.UseVisualStyleBackColor = true;
            this.textColor.Click += new System.EventHandler(this.OnClicked);
            // 
            // borderColor
            // 
            this.borderColor.Location = new System.Drawing.Point(44, 162);
            this.borderColor.Name = "borderColor";
            this.borderColor.Size = new System.Drawing.Size(32, 32);
            this.borderColor.TabIndex = 19;
            this.borderColor.UseVisualStyleBackColor = true;
            this.borderColor.Click += new System.EventHandler(this.OnClicked);
            // 
            // backColor
            // 
            this.backColor.Location = new System.Drawing.Point(6, 162);
            this.backColor.Name = "backColor";
            this.backColor.Size = new System.Drawing.Size(32, 32);
            this.backColor.TabIndex = 15;
            this.backColor.UseVisualStyleBackColor = true;
            this.backColor.Click += new System.EventHandler(this.OnClicked);
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(82, 146);
            this.label7.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(32, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "Text";
            this.label7.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // mainGroupBasic
            // 
            this.mainGroupBasic.Controls.Add(this.savesCustom);
            this.mainGroupBasic.Controls.Add(this.savesDefault);
            this.mainGroupBasic.Controls.Add(this.browse);
            this.mainGroupBasic.Controls.Add(this.customSavePath);
            this.mainGroupBasic.Location = new System.Drawing.Point(104, 3);
            this.mainGroupBasic.Name = "mainGroupBasic";
            this.mainGroupBasic.Size = new System.Drawing.Size(429, 95);
            this.mainGroupBasic.TabIndex = 23;
            this.mainGroupBasic.TabStop = false;
            this.mainGroupBasic.Text = "Tracker";
            // 
            // savesCustom
            // 
            this.savesCustom.AutoSize = true;
            this.savesCustom.Location = new System.Drawing.Point(9, 42);
            this.savesCustom.Name = "savesCustom";
            this.savesCustom.Size = new System.Drawing.Size(175, 17);
            this.savesCustom.TabIndex = 8;
            this.savesCustom.TabStop = true;
            this.savesCustom.Text = "Use Custom Saves Folder Path:";
            this.savesCustom.UseVisualStyleBackColor = true;
            this.savesCustom.CheckedChanged += new System.EventHandler(this.OnCheckChanged);
            // 
            // savesDefault
            // 
            this.savesDefault.AutoSize = true;
            this.savesDefault.Location = new System.Drawing.Point(9, 19);
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
            this.browse.Location = new System.Drawing.Point(369, 64);
            this.browse.Name = "browse";
            this.browse.Size = new System.Drawing.Size(53, 22);
            this.browse.TabIndex = 1;
            this.browse.Text = "Browse";
            this.browse.UseVisualStyleBackColor = true;
            this.browse.Click += new System.EventHandler(this.OnClicked);
            // 
            // customSavePath
            // 
            this.customSavePath.Location = new System.Drawing.Point(9, 65);
            this.customSavePath.Name = "customSavePath";
            this.customSavePath.Size = new System.Drawing.Size(354, 20);
            this.customSavePath.TabIndex = 0;
            this.customSavePath.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // CMainSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.mainGroupMain);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.mainGroupTheme);
            this.Controls.Add(this.mainGroupBasic);
            this.Name = "CMainSettings";
            this.Size = new System.Drawing.Size(538, 307);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.mainGroupMain.ResumeLayout(false);
            this.mainGroupMain.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.mainGroupTheme.ResumeLayout(false);
            this.mainGroupTheme.PerformLayout();
            this.mainGroupBasic.ResumeLayout(false);
            this.mainGroupBasic.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox notesEnabled;
        private System.Windows.Forms.GroupBox mainGroupMain;
        private System.Windows.Forms.CheckBox showBasic;
        private System.Windows.Forms.CheckBox layoutDebug;
        private System.Windows.Forms.GroupBox groupBox1;
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
        private System.Windows.Forms.GroupBox mainGroupBasic;
        private System.Windows.Forms.Button browse;
        private System.Windows.Forms.TextBox customSavePath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox viewMode;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox refreshIcon;
        private System.Windows.Forms.RadioButton savesCustom;
        private System.Windows.Forms.RadioButton savesDefault;
    }
}
