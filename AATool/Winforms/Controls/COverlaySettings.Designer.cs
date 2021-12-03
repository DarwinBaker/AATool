
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
            this.obsHelpLink = new System.Windows.Forms.LinkLabel();
            this.overlayGroupTheme = new System.Windows.Forms.GroupBox();
            this.copyColorKey = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.textColor = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.backColor = new System.Windows.Forms.Button();
            this.overlayGroupAppearance = new System.Windows.Forms.GroupBox();
            this.showCriteria = new System.Windows.Forms.CheckBox();
            this.showText = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.showCounts = new System.Windows.Forms.CheckBox();
            this.overlayWidth = new System.Windows.Forms.NumericUpDown();
            this.overlayGroupBehavior = new System.Windows.Forms.GroupBox();
            this.enabled = new System.Windows.Forms.CheckBox();
            this.direction = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.speed = new System.Windows.Forms.TrackBar();
            this.igt = new System.Windows.Forms.CheckBox();
            this.overlayGroupTheme.SuspendLayout();
            this.overlayGroupAppearance.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.overlayWidth)).BeginInit();
            this.overlayGroupBehavior.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.speed)).BeginInit();
            this.SuspendLayout();
            // 
            // obsHelpLink
            // 
            this.obsHelpLink.AutoSize = true;
            this.obsHelpLink.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.obsHelpLink.Location = new System.Drawing.Point(324, 292);
            this.obsHelpLink.Name = "obsHelpLink";
            this.obsHelpLink.Size = new System.Drawing.Size(209, 13);
            this.obsHelpLink.TabIndex = 35;
            this.obsHelpLink.TabStop = true;
            this.obsHelpLink.Text = "For help setting up your overlay, click here!";
            this.obsHelpLink.Click += new System.EventHandler(this.OnClicked);
            // 
            // overlayGroupTheme
            // 
            this.overlayGroupTheme.Controls.Add(this.copyColorKey);
            this.overlayGroupTheme.Controls.Add(this.label2);
            this.overlayGroupTheme.Controls.Add(this.textColor);
            this.overlayGroupTheme.Controls.Add(this.label4);
            this.overlayGroupTheme.Controls.Add(this.backColor);
            this.overlayGroupTheme.Location = new System.Drawing.Point(3, 174);
            this.overlayGroupTheme.Name = "overlayGroupTheme";
            this.overlayGroupTheme.Size = new System.Drawing.Size(200, 131);
            this.overlayGroupTheme.TabIndex = 34;
            this.overlayGroupTheme.TabStop = false;
            this.overlayGroupTheme.Text = "Theme";
            // 
            // copyColorKey
            // 
            this.copyColorKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.copyColorKey.AutoSize = true;
            this.copyColorKey.Location = new System.Drawing.Point(6, 109);
            this.copyColorKey.Name = "copyColorKey";
            this.copyColorKey.Size = new System.Drawing.Size(168, 13);
            this.copyColorKey.TabIndex = 29;
            this.copyColorKey.TabStop = true;
            this.copyColorKey.Text = "Copy BG Color (#00aa00) for OBS";
            this.copyColorKey.Click += new System.EventHandler(this.OnClicked);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(6, 26);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Back";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // textColor
            // 
            this.textColor.Location = new System.Drawing.Point(44, 42);
            this.textColor.Name = "textColor";
            this.textColor.Size = new System.Drawing.Size(32, 32);
            this.textColor.TabIndex = 13;
            this.textColor.UseVisualStyleBackColor = true;
            this.textColor.Click += new System.EventHandler(this.OnClicked);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(44, 26);
            this.label4.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Text";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // backColor
            // 
            this.backColor.Location = new System.Drawing.Point(6, 42);
            this.backColor.Name = "backColor";
            this.backColor.Size = new System.Drawing.Size(32, 32);
            this.backColor.TabIndex = 10;
            this.backColor.UseVisualStyleBackColor = true;
            this.backColor.Click += new System.EventHandler(this.OnClicked);
            // 
            // overlayGroupAppearance
            // 
            this.overlayGroupAppearance.Controls.Add(this.igt);
            this.overlayGroupAppearance.Controls.Add(this.showCriteria);
            this.overlayGroupAppearance.Controls.Add(this.showText);
            this.overlayGroupAppearance.Controls.Add(this.label3);
            this.overlayGroupAppearance.Controls.Add(this.showCounts);
            this.overlayGroupAppearance.Controls.Add(this.overlayWidth);
            this.overlayGroupAppearance.Location = new System.Drawing.Point(209, 3);
            this.overlayGroupAppearance.Name = "overlayGroupAppearance";
            this.overlayGroupAppearance.Size = new System.Drawing.Size(324, 165);
            this.overlayGroupAppearance.TabIndex = 33;
            this.overlayGroupAppearance.TabStop = false;
            this.overlayGroupAppearance.Text = "Appearance";
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
            this.showText.Size = new System.Drawing.Size(156, 17);
            this.showText.TabIndex = 15;
            this.showText.Text = "Show Advancement Labels";
            this.showText.UseVisualStyleBackColor = true;
            this.showText.CheckedChanged += new System.EventHandler(this.OnCheckChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 123);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 24;
            this.label3.Text = "Width:";
            // 
            // showCounts
            // 
            this.showCounts.AutoSize = true;
            this.showCounts.Location = new System.Drawing.Point(6, 66);
            this.showCounts.Name = "showCounts";
            this.showCounts.Size = new System.Drawing.Size(132, 17);
            this.showCounts.TabIndex = 27;
            this.showCounts.Text = "Show Item Count Row";
            this.showCounts.UseVisualStyleBackColor = true;
            this.showCounts.CheckedChanged += new System.EventHandler(this.OnCheckChanged);
            // 
            // overlayWidth
            // 
            this.overlayWidth.Location = new System.Drawing.Point(6, 139);
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
            this.overlayGroupBehavior.Controls.Add(this.enabled);
            this.overlayGroupBehavior.Controls.Add(this.direction);
            this.overlayGroupBehavior.Controls.Add(this.label5);
            this.overlayGroupBehavior.Controls.Add(this.speed);
            this.overlayGroupBehavior.Location = new System.Drawing.Point(3, 3);
            this.overlayGroupBehavior.Name = "overlayGroupBehavior";
            this.overlayGroupBehavior.Size = new System.Drawing.Size(200, 165);
            this.overlayGroupBehavior.TabIndex = 32;
            this.overlayGroupBehavior.TabStop = false;
            this.overlayGroupBehavior.Text = "Behavior";
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
            this.direction.Size = new System.Drawing.Size(144, 21);
            this.direction.TabIndex = 28;
            this.direction.SelectedIndexChanged += new System.EventHandler(this.OnIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 73);
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
            this.speed.Location = new System.Drawing.Point(1, 89);
            this.speed.Maximum = 4;
            this.speed.Name = "speed";
            this.speed.Size = new System.Drawing.Size(154, 23);
            this.speed.TabIndex = 25;
            this.speed.Value = 2;
            this.speed.ValueChanged += new System.EventHandler(this.OnValueChanged);
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
            // COverlaySettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.obsHelpLink);
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel obsHelpLink;
        private System.Windows.Forms.GroupBox overlayGroupTheme;
        private System.Windows.Forms.LinkLabel copyColorKey;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button textColor;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button backColor;
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
    }
}
