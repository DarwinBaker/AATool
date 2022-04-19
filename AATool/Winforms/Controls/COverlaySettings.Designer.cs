
namespace AATool.Winforms.Controls
{
    partial class COverlaySettings
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
            this.overlayGroupTheme = new System.Windows.Forms.GroupBox();
            this.restoreDefaultGreen = new System.Windows.Forms.LinkLabel();
            this.label11 = new System.Windows.Forms.Label();
            this.copyColorKey = new System.Windows.Forms.LinkLabel();
            this.greenscreenColor = new System.Windows.Forms.Button();
            this.textColor = new System.Windows.Forms.Button();
            this.overlayGroupAppearance = new System.Windows.Forms.GroupBox();
            this.obsHelpLink = new System.Windows.Forms.LinkLabel();
            this.clarifyAmbiguous = new System.Windows.Forms.CheckBox();
            this.igt = new System.Windows.Forms.CheckBox();
            this.showCriteria = new System.Windows.Forms.CheckBox();
            this.showText = new System.Windows.Forms.CheckBox();
            this.showCounts = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.frameStyle = new System.Windows.Forms.ComboBox();
            this.backColor = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.borderColor = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.overlayWidth = new System.Windows.Forms.NumericUpDown();
            this.overlayGroupBehavior = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pickupPosition = new System.Windows.Forms.ComboBox();
            this.enabled = new System.Windows.Forms.CheckBox();
            this.direction = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.speed = new System.Windows.Forms.TrackBar();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label16 = new System.Windows.Forms.Label();
            this.startupPosition = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.startupMonitor = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.overlayGroupTheme.SuspendLayout();
            this.overlayGroupAppearance.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.overlayWidth)).BeginInit();
            this.overlayGroupBehavior.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.speed)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // overlayGroupTheme
            // 
            this.overlayGroupTheme.Controls.Add(this.restoreDefaultGreen);
            this.overlayGroupTheme.Controls.Add(this.label11);
            this.overlayGroupTheme.Controls.Add(this.copyColorKey);
            this.overlayGroupTheme.Controls.Add(this.greenscreenColor);
            this.overlayGroupTheme.Location = new System.Drawing.Point(209, 237);
            this.overlayGroupTheme.Name = "overlayGroupTheme";
            this.overlayGroupTheme.Size = new System.Drawing.Size(324, 67);
            this.overlayGroupTheme.TabIndex = 34;
            this.overlayGroupTheme.TabStop = false;
            this.overlayGroupTheme.Text = "Green Screen";
            // 
            // restoreDefaultGreen
            // 
            this.restoreDefaultGreen.AutoSize = true;
            this.restoreDefaultGreen.Location = new System.Drawing.Point(47, 43);
            this.restoreDefaultGreen.Name = "restoreDefaultGreen";
            this.restoreDefaultGreen.Size = new System.Drawing.Size(79, 13);
            this.restoreDefaultGreen.TabIndex = 41;
            this.restoreDefaultGreen.TabStop = true;
            this.restoreDefaultGreen.Text = "Restore default";
            this.restoreDefaultGreen.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnClicked);
            this.restoreDefaultGreen.Click += new System.EventHandler(this.OnClicked);
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label11.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.label11.Location = new System.Drawing.Point(129, 24);
            this.label11.Margin = new System.Windows.Forms.Padding(0, 0, 3, 10);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(192, 37);
            this.label11.TabIndex = 40;
            this.label11.Text = "🛈 Make sure to use \"Color Key\" NOT \"Chroma Key\" for best results!";
            // 
            // copyColorKey
            // 
            this.copyColorKey.AutoSize = true;
            this.copyColorKey.Location = new System.Drawing.Point(47, 24);
            this.copyColorKey.Name = "copyColorKey";
            this.copyColorKey.Size = new System.Drawing.Size(77, 13);
            this.copyColorKey.TabIndex = 29;
            this.copyColorKey.TabStop = true;
            this.copyColorKey.Text = "Copy #00aa00";
            this.copyColorKey.Click += new System.EventHandler(this.OnClicked);
            // 
            // greenscreenColor
            // 
            this.greenscreenColor.Location = new System.Drawing.Point(9, 24);
            this.greenscreenColor.Name = "greenscreenColor";
            this.greenscreenColor.Size = new System.Drawing.Size(32, 32);
            this.greenscreenColor.TabIndex = 10;
            this.greenscreenColor.UseVisualStyleBackColor = true;
            this.greenscreenColor.BackColorChanged += new System.EventHandler(this.GreenscreenColorChanged);
            this.greenscreenColor.Click += new System.EventHandler(this.OnClicked);
            // 
            // textColor
            // 
            this.textColor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textColor.Location = new System.Drawing.Point(104, 110);
            this.textColor.Name = "textColor";
            this.textColor.Size = new System.Drawing.Size(32, 32);
            this.textColor.TabIndex = 13;
            this.textColor.UseVisualStyleBackColor = true;
            this.textColor.Click += new System.EventHandler(this.OnClicked);
            // 
            // overlayGroupAppearance
            // 
            this.overlayGroupAppearance.Controls.Add(this.obsHelpLink);
            this.overlayGroupAppearance.Controls.Add(this.clarifyAmbiguous);
            this.overlayGroupAppearance.Controls.Add(this.igt);
            this.overlayGroupAppearance.Controls.Add(this.showCriteria);
            this.overlayGroupAppearance.Controls.Add(this.showText);
            this.overlayGroupAppearance.Controls.Add(this.showCounts);
            this.overlayGroupAppearance.Location = new System.Drawing.Point(209, 3);
            this.overlayGroupAppearance.Name = "overlayGroupAppearance";
            this.overlayGroupAppearance.Size = new System.Drawing.Size(153, 228);
            this.overlayGroupAppearance.TabIndex = 33;
            this.overlayGroupAppearance.TabStop = false;
            this.overlayGroupAppearance.Text = "Layout";
            // 
            // obsHelpLink
            // 
            this.obsHelpLink.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.obsHelpLink.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.obsHelpLink.Location = new System.Drawing.Point(3, 193);
            this.obsHelpLink.Name = "obsHelpLink";
            this.obsHelpLink.Size = new System.Drawing.Size(147, 32);
            this.obsHelpLink.TabIndex = 35;
            this.obsHelpLink.TabStop = true;
            this.obsHelpLink.Text = "For help setting up your overlay, click here!";
            this.obsHelpLink.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.obsHelpLink.Click += new System.EventHandler(this.OnClicked);
            // 
            // clarifyAmbiguous
            // 
            this.clarifyAmbiguous.AutoSize = true;
            this.clarifyAmbiguous.Location = new System.Drawing.Point(6, 112);
            this.clarifyAmbiguous.Name = "clarifyAmbiguous";
            this.clarifyAmbiguous.Size = new System.Drawing.Size(144, 17);
            this.clarifyAmbiguous.TabIndex = 42;
            this.clarifyAmbiguous.Text = "Clarify Ambiguous Criteria";
            this.clarifyAmbiguous.UseVisualStyleBackColor = true;
            // 
            // igt
            // 
            this.igt.AutoSize = true;
            this.igt.Location = new System.Drawing.Point(6, 89);
            this.igt.Name = "igt";
            this.igt.Size = new System.Drawing.Size(122, 17);
            this.igt.TabIndex = 28;
            this.igt.Text = "Show In-Game Time";
            this.igt.UseVisualStyleBackColor = true;
            this.igt.CheckedChanged += new System.EventHandler(this.OnCheckChanged);
            // 
            // showCriteria
            // 
            this.showCriteria.AutoSize = true;
            this.showCriteria.Location = new System.Drawing.Point(6, 20);
            this.showCriteria.Name = "showCriteria";
            this.showCriteria.Size = new System.Drawing.Size(113, 17);
            this.showCriteria.TabIndex = 26;
            this.showCriteria.Text = "Show Criteria Row";
            this.showCriteria.UseVisualStyleBackColor = true;
            this.showCriteria.CheckedChanged += new System.EventHandler(this.OnCheckChanged);
            // 
            // showText
            // 
            this.showText.AutoSize = true;
            this.showText.Location = new System.Drawing.Point(6, 43);
            this.showText.Name = "showText";
            this.showText.Size = new System.Drawing.Size(135, 17);
            this.showText.TabIndex = 15;
            this.showText.Text = "Show Objective Labels";
            this.showText.UseVisualStyleBackColor = true;
            this.showText.CheckedChanged += new System.EventHandler(this.OnCheckChanged);
            // 
            // showCounts
            // 
            this.showCounts.AutoSize = true;
            this.showCounts.Location = new System.Drawing.Point(6, 66);
            this.showCounts.Name = "showCounts";
            this.showCounts.Size = new System.Drawing.Size(145, 17);
            this.showCounts.TabIndex = 27;
            this.showCounts.Text = "Show Pickup Count Row";
            this.showCounts.UseVisualStyleBackColor = true;
            this.showCounts.CheckedChanged += new System.EventHandler(this.OnCheckChanged);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.label2.Location = new System.Drawing.Point(19, 177);
            this.label2.Margin = new System.Windows.Forms.Padding(0, 0, 3, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(133, 42);
            this.label2.TabIndex = 49;
            this.label2.Text = "🛈 Your \"Custom\" preset is remembered even after changing themes";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(23, 91);
            this.label4.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(110, 13);
            this.label4.TabIndex = 49;
            this.label4.Text = "Custom Preset Colors:";
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(21, 25);
            this.label6.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(45, 13);
            this.label6.TabIndex = 41;
            this.label6.Text = "Presets:";
            // 
            // frameStyle
            // 
            this.frameStyle.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.frameStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.frameStyle.FormattingEnabled = true;
            this.frameStyle.Location = new System.Drawing.Point(24, 41);
            this.frameStyle.Name = "frameStyle";
            this.frameStyle.Size = new System.Drawing.Size(112, 21);
            this.frameStyle.TabIndex = 40;
            this.frameStyle.SelectedIndexChanged += new System.EventHandler(this.OnIndexChanged);
            // 
            // backColor
            // 
            this.backColor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.backColor.Location = new System.Drawing.Point(24, 110);
            this.backColor.Name = "backColor";
            this.backColor.Size = new System.Drawing.Size(32, 32);
            this.backColor.TabIndex = 43;
            this.backColor.UseVisualStyleBackColor = true;
            this.backColor.Click += new System.EventHandler(this.OnClicked);
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.Location = new System.Drawing.Point(104, 144);
            this.label7.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(32, 13);
            this.label7.TabIndex = 46;
            this.label7.Text = "Text";
            this.label7.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.Location = new System.Drawing.Point(62, 144);
            this.label9.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(38, 13);
            this.label9.TabIndex = 48;
            this.label9.Text = "Border";
            this.label9.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // borderColor
            // 
            this.borderColor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.borderColor.Location = new System.Drawing.Point(64, 110);
            this.borderColor.Name = "borderColor";
            this.borderColor.Size = new System.Drawing.Size(32, 32);
            this.borderColor.TabIndex = 47;
            this.borderColor.UseVisualStyleBackColor = true;
            this.borderColor.Click += new System.EventHandler(this.OnClicked);
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.Location = new System.Drawing.Point(24, 144);
            this.label8.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(32, 13);
            this.label8.TabIndex = 44;
            this.label8.Text = "Back";
            this.label8.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(132, 120);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 24;
            this.label3.Text = "Width:";
            // 
            // overlayWidth
            // 
            this.overlayWidth.Location = new System.Drawing.Point(135, 136);
            this.overlayWidth.Maximum = new decimal(new int[] {
            3840,
            0,
            0,
            0});
            this.overlayWidth.Minimum = new decimal(new int[] {
            640,
            0,
            0,
            0});
            this.overlayWidth.Name = "overlayWidth";
            this.overlayWidth.Size = new System.Drawing.Size(56, 20);
            this.overlayWidth.TabIndex = 22;
            this.overlayWidth.Value = new decimal(new int[] {
            1920,
            0,
            0,
            0});
            this.overlayWidth.ValueChanged += new System.EventHandler(this.OnValueChanged);
            // 
            // overlayGroupBehavior
            // 
            this.overlayGroupBehavior.Controls.Add(this.label1);
            this.overlayGroupBehavior.Controls.Add(this.pickupPosition);
            this.overlayGroupBehavior.Controls.Add(this.enabled);
            this.overlayGroupBehavior.Controls.Add(this.direction);
            this.overlayGroupBehavior.Controls.Add(this.label5);
            this.overlayGroupBehavior.Controls.Add(this.speed);
            this.overlayGroupBehavior.Controls.Add(this.overlayWidth);
            this.overlayGroupBehavior.Controls.Add(this.label3);
            this.overlayGroupBehavior.Location = new System.Drawing.Point(3, 3);
            this.overlayGroupBehavior.Name = "overlayGroupBehavior";
            this.overlayGroupBehavior.Size = new System.Drawing.Size(200, 175);
            this.overlayGroupBehavior.TabIndex = 32;
            this.overlayGroupBehavior.TabStop = false;
            this.overlayGroupBehavior.Text = "Behavior";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 70);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 13);
            this.label1.TabIndex = 30;
            this.label1.Text = "Pickup Row Position:";
            // 
            // pickupPosition
            // 
            this.pickupPosition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.pickupPosition.FormattingEnabled = true;
            this.pickupPosition.Items.AddRange(new object[] {
            "Normal",
            "Opposite"});
            this.pickupPosition.Location = new System.Drawing.Point(6, 86);
            this.pickupPosition.Name = "pickupPosition";
            this.pickupPosition.Size = new System.Drawing.Size(120, 21);
            this.pickupPosition.TabIndex = 29;
            this.pickupPosition.SelectedIndexChanged += new System.EventHandler(this.OnIndexChanged);
            // 
            // enabled
            // 
            this.enabled.AutoSize = true;
            this.enabled.Location = new System.Drawing.Point(6, 20);
            this.enabled.Name = "enabled";
            this.enabled.Size = new System.Drawing.Size(140, 17);
            this.enabled.TabIndex = 9;
            this.enabled.Text = "Enable Overlay Window";
            this.enabled.UseVisualStyleBackColor = true;
            this.enabled.CheckedChanged += new System.EventHandler(this.OnCheckChanged);
            // 
            // direction
            // 
            this.direction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.direction.FormattingEnabled = true;
            this.direction.Items.AddRange(new object[] {
            "Scroll Right to Left",
            "Scroll Left to Right"});
            this.direction.Location = new System.Drawing.Point(6, 43);
            this.direction.Name = "direction";
            this.direction.Size = new System.Drawing.Size(120, 21);
            this.direction.TabIndex = 28;
            this.direction.SelectedIndexChanged += new System.EventHandler(this.OnIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 120);
            this.label5.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(70, 13);
            this.label5.TabIndex = 23;
            this.label5.Text = "Scroll Speed:";
            // 
            // speed
            // 
            this.speed.AutoSize = false;
            this.speed.BackColor = System.Drawing.SystemColors.Window;
            this.speed.LargeChange = 1;
            this.speed.Location = new System.Drawing.Point(4, 136);
            this.speed.Maximum = 4;
            this.speed.Name = "speed";
            this.speed.Size = new System.Drawing.Size(122, 23);
            this.speed.TabIndex = 25;
            this.speed.Value = 2;
            this.speed.ValueChanged += new System.EventHandler(this.OnValueChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.startupPosition);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.startupMonitor);
            this.groupBox2.Location = new System.Drawing.Point(3, 184);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 120);
            this.groupBox2.TabIndex = 50;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Startup Behavior";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(4, 68);
            this.label16.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(45, 13);
            this.label16.TabIndex = 42;
            this.label16.Text = "Monitor:";
            // 
            // startupPosition
            // 
            this.startupPosition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.startupPosition.FormattingEnabled = true;
            this.startupPosition.Items.AddRange(new object[] {
            "Centered",
            "Remember",
            "TopLeft",
            "TopRight",
            "BottomLeft",
            "BottomRight"});
            this.startupPosition.Location = new System.Drawing.Point(7, 38);
            this.startupPosition.Name = "startupPosition";
            this.startupPosition.Size = new System.Drawing.Size(139, 21);
            this.startupPosition.TabIndex = 39;
            this.startupPosition.SelectedIndexChanged += new System.EventHandler(this.OnIndexChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(6, 22);
            this.label15.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(47, 13);
            this.label15.TabIndex = 40;
            this.label15.Text = "Position:";
            // 
            // startupMonitor
            // 
            this.startupMonitor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.startupMonitor.FormattingEnabled = true;
            this.startupMonitor.Location = new System.Drawing.Point(7, 84);
            this.startupMonitor.Name = "startupMonitor";
            this.startupMonitor.Size = new System.Drawing.Size(139, 21);
            this.startupMonitor.TabIndex = 41;
            this.startupMonitor.SelectedIndexChanged += new System.EventHandler(this.OnIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.borderColor);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.frameStyle);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.backColor);
            this.groupBox1.Controls.Add(this.textColor);
            this.groupBox1.Location = new System.Drawing.Point(368, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(165, 226);
            this.groupBox1.TabIndex = 51;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Frame Styling";
            // 
            // COverlaySettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.overlayGroupTheme);
            this.Controls.Add(this.overlayGroupAppearance);
            this.Controls.Add(this.overlayGroupBehavior);
            this.Name = "COverlaySettings";
            this.Size = new System.Drawing.Size(538, 307);
            this.overlayGroupTheme.ResumeLayout(false);
            this.overlayGroupTheme.PerformLayout();
            this.overlayGroupAppearance.ResumeLayout(false);
            this.overlayGroupAppearance.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.overlayWidth)).EndInit();
            this.overlayGroupBehavior.ResumeLayout(false);
            this.overlayGroupBehavior.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.speed)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox overlayGroupTheme;
        private System.Windows.Forms.LinkLabel copyColorKey;
        private System.Windows.Forms.Button textColor;
        private System.Windows.Forms.Button greenscreenColor;
        private System.Windows.Forms.GroupBox overlayGroupAppearance;
        private System.Windows.Forms.CheckBox showCriteria;
        private System.Windows.Forms.CheckBox showText;
        private System.Windows.Forms.CheckBox showCounts;
        private System.Windows.Forms.GroupBox overlayGroupBehavior;
        private System.Windows.Forms.CheckBox enabled;
        private System.Windows.Forms.ComboBox direction;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TrackBar speed;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown overlayWidth;
        private System.Windows.Forms.CheckBox igt;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox frameStyle;
        private System.Windows.Forms.CheckBox clarifyAmbiguous;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox pickupPosition;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button backColor;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button borderColor;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.LinkLabel restoreDefaultGreen;
        private System.Windows.Forms.LinkLabel obsHelpLink;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.ComboBox startupPosition;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox startupMonitor;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}
