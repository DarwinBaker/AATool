namespace AATool.Winforms.Forms
{
    partial class FSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FSettings));
            this.trackerCustomSavesFolder = new System.Windows.Forms.TextBox();
            this.labelCustomPath = new System.Windows.Forms.Label();
            this.save = new System.Windows.Forms.Button();
            this.cancel = new System.Windows.Forms.Button();
            this.trackerUseDefault = new System.Windows.Forms.CheckBox();
            this.colors = new System.Windows.Forms.ColorDialog();
            this.trackerBrowse = new System.Windows.Forms.Button();
            this.mainShowBasic = new System.Windows.Forms.CheckBox();
            this.groupOverlay = new System.Windows.Forms.GroupBox();
            this.copyColorKey = new System.Windows.Forms.LinkLabel();
            this.overlayHideCompleted = new System.Windows.Forms.CheckBox();
            this.overlayShowText = new System.Windows.Forms.CheckBox();
            this.overlayDirection = new System.Windows.Forms.ComboBox();
            this.overlayOnlyShowFavorites = new System.Windows.Forms.CheckBox();
            this.overlayShowCounts = new System.Windows.Forms.CheckBox();
            this.overlayPickFavorites = new System.Windows.Forms.Button();
            this.overlayShowCriteria = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.overlayBackColor = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.overlayWidth = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.overlayShowOverview = new System.Windows.Forms.CheckBox();
            this.overlayTextColor = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.overlaySpeed = new System.Windows.Forms.TrackBar();
            this.overlayEnabled = new System.Windows.Forms.CheckBox();
            this.groupTracker = new System.Windows.Forms.GroupBox();
            this.trackerRefreshDelay = new System.Windows.Forms.NumericUpDown();
            this.labelRefresh = new System.Windows.Forms.Label();
            this.trackerGameVersion = new System.Windows.Forms.ComboBox();
            this.groupMain = new System.Windows.Forms.GroupBox();
            this.mainCompletionGlow = new System.Windows.Forms.CheckBox();
            this.mainLayoutDebug = new System.Windows.Forms.CheckBox();
            this.mainFancyCorners = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.mainTheme = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.mainBorderColor = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.mainBackColor = new System.Windows.Forms.Button();
            this.mainTextColor = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.defaults = new System.Windows.Forms.Button();
            this.about = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.trackerAutoVersion = new System.Windows.Forms.CheckBox();
            this.update = new System.Windows.Forms.Button();
            this.credits = new AATool.Winforms.Controls.CCredits();
            this.groupOverlay.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.overlayWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.overlaySpeed)).BeginInit();
            this.groupTracker.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackerRefreshDelay)).BeginInit();
            this.groupMain.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // trackerCustomSavesFolder
            // 
            this.trackerCustomSavesFolder.Location = new System.Drawing.Point(6, 67);
            this.trackerCustomSavesFolder.Name = "trackerCustomSavesFolder";
            this.trackerCustomSavesFolder.Size = new System.Drawing.Size(377, 20);
            this.trackerCustomSavesFolder.TabIndex = 0;
            // 
            // labelCustomPath
            // 
            this.labelCustomPath.AutoSize = true;
            this.labelCustomPath.Location = new System.Drawing.Point(3, 51);
            this.labelCustomPath.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.labelCustomPath.Name = "labelCustomPath";
            this.labelCustomPath.Size = new System.Drawing.Size(135, 13);
            this.labelCustomPath.TabIndex = 3;
            this.labelCustomPath.Text = "Custom Saves Folder Path:";
            // 
            // save
            // 
            this.save.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.save.Location = new System.Drawing.Point(12, 376);
            this.save.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(100, 32);
            this.save.TabIndex = 4;
            this.save.Text = "Save";
            this.save.UseVisualStyleBackColor = true;
            this.save.Click += new System.EventHandler(this.OnButtonClick);
            // 
            // cancel
            // 
            this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel.Location = new System.Drawing.Point(125, 376);
            this.cancel.Margin = new System.Windows.Forms.Padding(10, 10, 9, 3);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(100, 32);
            this.cancel.TabIndex = 5;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.OnButtonClick);
            // 
            // trackerUseDefault
            // 
            this.trackerUseDefault.AutoSize = true;
            this.trackerUseDefault.Location = new System.Drawing.Point(6, 23);
            this.trackerUseDefault.Name = "trackerUseDefault";
            this.trackerUseDefault.Size = new System.Drawing.Size(264, 17);
            this.trackerUseDefault.TabIndex = 6;
            this.trackerUseDefault.Text = "Use Default (AppData\\Roaming\\.minecraft\\saves)";
            this.trackerUseDefault.UseVisualStyleBackColor = true;
            this.trackerUseDefault.CheckedChanged += new System.EventHandler(this.OnCheckedChanged);
            // 
            // colors
            // 
            this.colors.AnyColor = true;
            this.colors.Color = System.Drawing.SystemColors.Control;
            this.colors.SolidColorOnly = true;
            // 
            // trackerBrowse
            // 
            this.trackerBrowse.Location = new System.Drawing.Point(389, 66);
            this.trackerBrowse.Name = "trackerBrowse";
            this.trackerBrowse.Size = new System.Drawing.Size(100, 22);
            this.trackerBrowse.TabIndex = 1;
            this.trackerBrowse.Text = "Browse";
            this.trackerBrowse.UseVisualStyleBackColor = true;
            this.trackerBrowse.Click += new System.EventHandler(this.OnButtonClick);
            // 
            // mainShowBasic
            // 
            this.mainShowBasic.AutoSize = true;
            this.mainShowBasic.Location = new System.Drawing.Point(9, 19);
            this.mainShowBasic.Name = "mainShowBasic";
            this.mainShowBasic.Size = new System.Drawing.Size(156, 17);
            this.mainShowBasic.TabIndex = 8;
            this.mainShowBasic.Text = "Show Basic Advancements";
            this.mainShowBasic.UseVisualStyleBackColor = true;
            this.mainShowBasic.CheckedChanged += new System.EventHandler(this.OnCheckedChanged);
            // 
            // groupOverlay
            // 
            this.groupOverlay.Controls.Add(this.copyColorKey);
            this.groupOverlay.Controls.Add(this.overlayHideCompleted);
            this.groupOverlay.Controls.Add(this.overlayShowText);
            this.groupOverlay.Controls.Add(this.overlayDirection);
            this.groupOverlay.Controls.Add(this.overlayOnlyShowFavorites);
            this.groupOverlay.Controls.Add(this.overlayShowCounts);
            this.groupOverlay.Controls.Add(this.overlayPickFavorites);
            this.groupOverlay.Controls.Add(this.overlayShowCriteria);
            this.groupOverlay.Controls.Add(this.label3);
            this.groupOverlay.Controls.Add(this.overlayBackColor);
            this.groupOverlay.Controls.Add(this.label2);
            this.groupOverlay.Controls.Add(this.overlayWidth);
            this.groupOverlay.Controls.Add(this.label4);
            this.groupOverlay.Controls.Add(this.overlayShowOverview);
            this.groupOverlay.Controls.Add(this.overlayTextColor);
            this.groupOverlay.Controls.Add(this.label5);
            this.groupOverlay.Controls.Add(this.overlaySpeed);
            this.groupOverlay.Controls.Add(this.overlayEnabled);
            this.groupOverlay.Location = new System.Drawing.Point(234, 112);
            this.groupOverlay.Name = "groupOverlay";
            this.groupOverlay.Size = new System.Drawing.Size(377, 252);
            this.groupOverlay.TabIndex = 9;
            this.groupOverlay.TabStop = false;
            this.groupOverlay.Text = "Stream Overlay";
            // 
            // copyColorKey
            // 
            this.copyColorKey.AutoSize = true;
            this.copyColorKey.Location = new System.Drawing.Point(203, 229);
            this.copyColorKey.Name = "copyColorKey";
            this.copyColorKey.Size = new System.Drawing.Size(168, 13);
            this.copyColorKey.TabIndex = 29;
            this.copyColorKey.TabStop = true;
            this.copyColorKey.Text = "Copy BG Color (#00aa00) for OBS";
            this.copyColorKey.Click += new System.EventHandler(this.OnButtonClick);
            // 
            // overlayHideCompleted
            // 
            this.overlayHideCompleted.AutoSize = true;
            this.overlayHideCompleted.Location = new System.Drawing.Point(162, 19);
            this.overlayHideCompleted.Name = "overlayHideCompleted";
            this.overlayHideCompleted.Size = new System.Drawing.Size(175, 17);
            this.overlayHideCompleted.TabIndex = 10;
            this.overlayHideCompleted.Text = "Hide Completed Advancements";
            this.overlayHideCompleted.UseVisualStyleBackColor = true;
            // 
            // overlayShowText
            // 
            this.overlayShowText.AutoSize = true;
            this.overlayShowText.Location = new System.Drawing.Point(161, 42);
            this.overlayShowText.Name = "overlayShowText";
            this.overlayShowText.Size = new System.Drawing.Size(156, 17);
            this.overlayShowText.TabIndex = 15;
            this.overlayShowText.Text = "Show Advancement Labels";
            this.overlayShowText.UseVisualStyleBackColor = true;
            // 
            // overlayDirection
            // 
            this.overlayDirection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.overlayDirection.FormattingEnabled = true;
            this.overlayDirection.Items.AddRange(new object[] {
            "Scroll Right to Left",
            "Scroll Left to Right"});
            this.overlayDirection.Location = new System.Drawing.Point(6, 111);
            this.overlayDirection.Name = "overlayDirection";
            this.overlayDirection.Size = new System.Drawing.Size(144, 21);
            this.overlayDirection.TabIndex = 28;
            // 
            // overlayOnlyShowFavorites
            // 
            this.overlayOnlyShowFavorites.AutoSize = true;
            this.overlayOnlyShowFavorites.Location = new System.Drawing.Point(161, 65);
            this.overlayOnlyShowFavorites.Name = "overlayOnlyShowFavorites";
            this.overlayOnlyShowFavorites.Size = new System.Drawing.Size(123, 17);
            this.overlayOnlyShowFavorites.TabIndex = 18;
            this.overlayOnlyShowFavorites.Text = "Show Favorites Only";
            this.overlayOnlyShowFavorites.UseVisualStyleBackColor = true;
            // 
            // overlayShowCounts
            // 
            this.overlayShowCounts.AutoSize = true;
            this.overlayShowCounts.Location = new System.Drawing.Point(6, 88);
            this.overlayShowCounts.Name = "overlayShowCounts";
            this.overlayShowCounts.Size = new System.Drawing.Size(132, 17);
            this.overlayShowCounts.TabIndex = 27;
            this.overlayShowCounts.Text = "Show Item Count Row";
            this.overlayShowCounts.UseVisualStyleBackColor = true;
            // 
            // overlayPickFavorites
            // 
            this.overlayPickFavorites.Location = new System.Drawing.Point(161, 110);
            this.overlayPickFavorites.Name = "overlayPickFavorites";
            this.overlayPickFavorites.Size = new System.Drawing.Size(110, 23);
            this.overlayPickFavorites.TabIndex = 19;
            this.overlayPickFavorites.Text = "Pick Favorites";
            this.overlayPickFavorites.UseVisualStyleBackColor = true;
            this.overlayPickFavorites.Click += new System.EventHandler(this.OnButtonClick);
            // 
            // overlayShowCriteria
            // 
            this.overlayShowCriteria.AutoSize = true;
            this.overlayShowCriteria.Location = new System.Drawing.Point(6, 65);
            this.overlayShowCriteria.Name = "overlayShowCriteria";
            this.overlayShowCriteria.Size = new System.Drawing.Size(113, 17);
            this.overlayShowCriteria.TabIndex = 26;
            this.overlayShowCriteria.Text = "Show Criteria Row";
            this.overlayShowCriteria.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 141);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 24;
            this.label3.Text = "Width:";
            // 
            // overlayBackColor
            // 
            this.overlayBackColor.Location = new System.Drawing.Point(298, 194);
            this.overlayBackColor.Name = "overlayBackColor";
            this.overlayBackColor.Size = new System.Drawing.Size(32, 32);
            this.overlayBackColor.TabIndex = 10;
            this.overlayBackColor.UseVisualStyleBackColor = true;
            this.overlayBackColor.Click += new System.EventHandler(this.OnButtonClick);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(298, 178);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Back";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // overlayWidth
            // 
            this.overlayWidth.Location = new System.Drawing.Point(6, 156);
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
            this.overlayWidth.Size = new System.Drawing.Size(144, 20);
            this.overlayWidth.TabIndex = 22;
            this.overlayWidth.Value = new decimal(new int[] {
            1920,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(336, 178);
            this.label4.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Text";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // overlayShowOverview
            // 
            this.overlayShowOverview.AutoSize = true;
            this.overlayShowOverview.Location = new System.Drawing.Point(6, 42);
            this.overlayShowOverview.Name = "overlayShowOverview";
            this.overlayShowOverview.Size = new System.Drawing.Size(148, 17);
            this.overlayShowOverview.TabIndex = 17;
            this.overlayShowOverview.Text = "Show Completion Percent";
            this.overlayShowOverview.UseVisualStyleBackColor = true;
            // 
            // overlayTextColor
            // 
            this.overlayTextColor.Location = new System.Drawing.Point(336, 194);
            this.overlayTextColor.Name = "overlayTextColor";
            this.overlayTextColor.Size = new System.Drawing.Size(32, 32);
            this.overlayTextColor.TabIndex = 13;
            this.overlayTextColor.UseVisualStyleBackColor = true;
            this.overlayTextColor.Click += new System.EventHandler(this.OnButtonClick);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 184);
            this.label5.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(70, 13);
            this.label5.TabIndex = 23;
            this.label5.Text = "Scroll Speed:";
            // 
            // overlaySpeed
            // 
            this.overlaySpeed.AutoSize = false;
            this.overlaySpeed.LargeChange = 1;
            this.overlaySpeed.Location = new System.Drawing.Point(1, 200);
            this.overlaySpeed.Maximum = 5;
            this.overlaySpeed.Minimum = 1;
            this.overlaySpeed.Name = "overlaySpeed";
            this.overlaySpeed.Size = new System.Drawing.Size(154, 23);
            this.overlaySpeed.TabIndex = 25;
            this.overlaySpeed.Value = 5;
            // 
            // overlayEnabled
            // 
            this.overlayEnabled.AutoSize = true;
            this.overlayEnabled.Location = new System.Drawing.Point(6, 19);
            this.overlayEnabled.Name = "overlayEnabled";
            this.overlayEnabled.Size = new System.Drawing.Size(140, 17);
            this.overlayEnabled.TabIndex = 9;
            this.overlayEnabled.Text = "Enable Overlay Window";
            this.overlayEnabled.UseVisualStyleBackColor = true;
            // 
            // groupTracker
            // 
            this.groupTracker.Controls.Add(this.labelCustomPath);
            this.groupTracker.Controls.Add(this.trackerCustomSavesFolder);
            this.groupTracker.Controls.Add(this.trackerBrowse);
            this.groupTracker.Controls.Add(this.trackerUseDefault);
            this.groupTracker.Controls.Add(this.trackerRefreshDelay);
            this.groupTracker.Controls.Add(this.labelRefresh);
            this.groupTracker.Location = new System.Drawing.Point(12, 12);
            this.groupTracker.Margin = new System.Windows.Forms.Padding(3, 3, 6, 3);
            this.groupTracker.Name = "groupTracker";
            this.groupTracker.Size = new System.Drawing.Size(495, 94);
            this.groupTracker.TabIndex = 10;
            this.groupTracker.TabStop = false;
            this.groupTracker.Text = "Tracker Settings";
            // 
            // trackerRefreshDelay
            // 
            this.trackerRefreshDelay.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.trackerRefreshDelay.Location = new System.Drawing.Point(389, 40);
            this.trackerRefreshDelay.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.trackerRefreshDelay.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.trackerRefreshDelay.Name = "trackerRefreshDelay";
            this.trackerRefreshDelay.Size = new System.Drawing.Size(100, 20);
            this.trackerRefreshDelay.TabIndex = 3;
            this.trackerRefreshDelay.Value = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            // 
            // labelRefresh
            // 
            this.labelRefresh.AutoSize = true;
            this.labelRefresh.Location = new System.Drawing.Point(386, 24);
            this.labelRefresh.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.labelRefresh.Name = "labelRefresh";
            this.labelRefresh.Size = new System.Drawing.Size(107, 13);
            this.labelRefresh.TabIndex = 7;
            this.labelRefresh.Text = "Refresh Interval (ms):";
            // 
            // trackerGameVersion
            // 
            this.trackerGameVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.trackerGameVersion.FormattingEnabled = true;
            this.trackerGameVersion.Location = new System.Drawing.Point(6, 66);
            this.trackerGameVersion.Name = "trackerGameVersion";
            this.trackerGameVersion.Size = new System.Drawing.Size(83, 21);
            this.trackerGameVersion.TabIndex = 18;
            this.trackerGameVersion.SelectedIndexChanged += new System.EventHandler(this.OnIndexChanged);
            // 
            // groupMain
            // 
            this.groupMain.Controls.Add(this.mainCompletionGlow);
            this.groupMain.Controls.Add(this.mainLayoutDebug);
            this.groupMain.Controls.Add(this.mainFancyCorners);
            this.groupMain.Controls.Add(this.label10);
            this.groupMain.Controls.Add(this.mainTheme);
            this.groupMain.Controls.Add(this.label9);
            this.groupMain.Controls.Add(this.mainShowBasic);
            this.groupMain.Controls.Add(this.mainBorderColor);
            this.groupMain.Controls.Add(this.label7);
            this.groupMain.Controls.Add(this.mainBackColor);
            this.groupMain.Controls.Add(this.mainTextColor);
            this.groupMain.Controls.Add(this.label8);
            this.groupMain.Location = new System.Drawing.Point(12, 112);
            this.groupMain.Margin = new System.Windows.Forms.Padding(3, 3, 6, 3);
            this.groupMain.Name = "groupMain";
            this.groupMain.Size = new System.Drawing.Size(213, 252);
            this.groupMain.TabIndex = 11;
            this.groupMain.TabStop = false;
            this.groupMain.Text = "Main Window";
            // 
            // mainCompletionGlow
            // 
            this.mainCompletionGlow.AutoSize = true;
            this.mainCompletionGlow.Location = new System.Drawing.Point(9, 65);
            this.mainCompletionGlow.Name = "mainCompletionGlow";
            this.mainCompletionGlow.Size = new System.Drawing.Size(136, 17);
            this.mainCompletionGlow.TabIndex = 26;
            this.mainCompletionGlow.Text = "Completion Glow Effect";
            this.mainCompletionGlow.UseVisualStyleBackColor = true;
            // 
            // mainLayoutDebug
            // 
            this.mainLayoutDebug.AutoSize = true;
            this.mainLayoutDebug.Location = new System.Drawing.Point(9, 88);
            this.mainLayoutDebug.Name = "mainLayoutDebug";
            this.mainLayoutDebug.Size = new System.Drawing.Size(119, 17);
            this.mainLayoutDebug.TabIndex = 25;
            this.mainLayoutDebug.Text = "Layout Debug View";
            this.mainLayoutDebug.UseVisualStyleBackColor = true;
            // 
            // mainFancyCorners
            // 
            this.mainFancyCorners.AutoSize = true;
            this.mainFancyCorners.Location = new System.Drawing.Point(9, 42);
            this.mainFancyCorners.Name = "mainFancyCorners";
            this.mainFancyCorners.Size = new System.Drawing.Size(132, 17);
            this.mainFancyCorners.TabIndex = 24;
            this.mainFancyCorners.Text = "Render Fancy Corners";
            this.mainFancyCorners.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 114);
            this.label10.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(43, 13);
            this.label10.TabIndex = 23;
            this.label10.Text = "Theme:";
            // 
            // mainTheme
            // 
            this.mainTheme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mainTheme.FormattingEnabled = true;
            this.mainTheme.Location = new System.Drawing.Point(9, 130);
            this.mainTheme.Name = "mainTheme";
            this.mainTheme.Size = new System.Drawing.Size(108, 21);
            this.mainTheme.TabIndex = 22;
            this.mainTheme.SelectedIndexChanged += new System.EventHandler(this.OnIndexChanged);
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(83, 194);
            this.label9.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(38, 13);
            this.label9.TabIndex = 20;
            this.label9.Text = "Border";
            this.label9.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // mainBorderColor
            // 
            this.mainBorderColor.Location = new System.Drawing.Point(85, 210);
            this.mainBorderColor.Name = "mainBorderColor";
            this.mainBorderColor.Size = new System.Drawing.Size(32, 32);
            this.mainBorderColor.TabIndex = 19;
            this.mainBorderColor.UseVisualStyleBackColor = true;
            this.mainBorderColor.Click += new System.EventHandler(this.OnButtonClick);
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(47, 194);
            this.label7.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(32, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "Text";
            this.label7.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // mainBackColor
            // 
            this.mainBackColor.Location = new System.Drawing.Point(9, 210);
            this.mainBackColor.Name = "mainBackColor";
            this.mainBackColor.Size = new System.Drawing.Size(32, 32);
            this.mainBackColor.TabIndex = 15;
            this.mainBackColor.UseVisualStyleBackColor = true;
            this.mainBackColor.Click += new System.EventHandler(this.OnButtonClick);
            // 
            // mainTextColor
            // 
            this.mainTextColor.Location = new System.Drawing.Point(47, 210);
            this.mainTextColor.Name = "mainTextColor";
            this.mainTextColor.Size = new System.Drawing.Size(32, 32);
            this.mainTextColor.TabIndex = 17;
            this.mainTextColor.UseVisualStyleBackColor = true;
            this.mainTextColor.Click += new System.EventHandler(this.OnButtonClick);
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(9, 194);
            this.label8.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(32, 13);
            this.label8.TabIndex = 16;
            this.label8.Text = "Back";
            this.label8.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // defaults
            // 
            this.defaults.Location = new System.Drawing.Point(237, 376);
            this.defaults.Name = "defaults";
            this.defaults.Size = new System.Drawing.Size(100, 32);
            this.defaults.TabIndex = 20;
            this.defaults.Text = "Reset to Defaults";
            this.defaults.UseVisualStyleBackColor = true;
            this.defaults.Click += new System.EventHandler(this.OnButtonClick);
            // 
            // about
            // 
            this.about.Location = new System.Drawing.Point(491, 376);
            this.about.Name = "about";
            this.about.Size = new System.Drawing.Size(120, 32);
            this.about.TabIndex = 21;
            this.about.Text = "About this Tool";
            this.about.UseVisualStyleBackColor = true;
            this.about.Click += new System.EventHandler(this.OnButtonClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.trackerAutoVersion);
            this.groupBox1.Controls.Add(this.trackerGameVersion);
            this.groupBox1.Location = new System.Drawing.Point(516, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(95, 94);
            this.groupBox1.TabIndex = 22;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Game Version";
            // 
            // trackerAutoVersion
            // 
            this.trackerAutoVersion.AutoSize = true;
            this.trackerAutoVersion.Location = new System.Drawing.Point(6, 43);
            this.trackerAutoVersion.Name = "trackerAutoVersion";
            this.trackerAutoVersion.Size = new System.Drawing.Size(83, 17);
            this.trackerAutoVersion.TabIndex = 26;
            this.trackerAutoVersion.Text = "Auto-Detect";
            this.trackerAutoVersion.UseVisualStyleBackColor = true;
            this.trackerAutoVersion.CheckedChanged += new System.EventHandler(this.OnCheckedChanged);
            // 
            // update
            // 
            this.update.Location = new System.Drawing.Point(365, 376);
            this.update.Name = "update";
            this.update.Size = new System.Drawing.Size(120, 32);
            this.update.TabIndex = 23;
            this.update.Text = "Check for Updates";
            this.update.UseVisualStyleBackColor = true;
            this.update.Click += new System.EventHandler(this.OnButtonClick);
            // 
            // credits
            // 
            this.credits.Location = new System.Drawing.Point(617, 12);
            this.credits.Name = "credits";
            this.credits.Size = new System.Drawing.Size(224, 396);
            this.credits.TabIndex = 24;
            // 
            // FSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancel;
            this.ClientSize = new System.Drawing.Size(851, 420);
            this.Controls.Add(this.credits);
            this.Controls.Add(this.update);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.about);
            this.Controls.Add(this.defaults);
            this.Controls.Add(this.groupMain);
            this.Controls.Add(this.groupTracker);
            this.Controls.Add(this.groupOverlay);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.save);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FSettings";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Application Settings";
            this.groupOverlay.ResumeLayout(false);
            this.groupOverlay.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.overlayWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.overlaySpeed)).EndInit();
            this.groupTracker.ResumeLayout(false);
            this.groupTracker.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackerRefreshDelay)).EndInit();
            this.groupMain.ResumeLayout(false);
            this.groupMain.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox trackerCustomSavesFolder;
        private System.Windows.Forms.Label labelCustomPath;
        private System.Windows.Forms.Button save;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.CheckBox trackerUseDefault;
        private System.Windows.Forms.ColorDialog colors;
        private System.Windows.Forms.Button trackerBrowse;
        private System.Windows.Forms.CheckBox mainShowBasic;
        private System.Windows.Forms.GroupBox groupOverlay;
        private System.Windows.Forms.CheckBox overlayEnabled;
        private System.Windows.Forms.Button overlayBackColor;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button overlayTextColor;
        private System.Windows.Forms.GroupBox groupTracker;
        private System.Windows.Forms.GroupBox groupMain;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button mainTextColor;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button mainBackColor;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button mainBorderColor;
        private System.Windows.Forms.ComboBox mainTheme;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox overlayShowOverview;
        private System.Windows.Forms.CheckBox overlayHideCompleted;
        private System.Windows.Forms.CheckBox overlayShowText;
        private System.Windows.Forms.Button overlayPickFavorites;
        private System.Windows.Forms.CheckBox overlayOnlyShowFavorites;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown overlayWidth;
        private System.Windows.Forms.TrackBar overlaySpeed;
        private System.Windows.Forms.Button defaults;
        private System.Windows.Forms.CheckBox overlayShowCounts;
        private System.Windows.Forms.CheckBox overlayShowCriteria;
        private System.Windows.Forms.CheckBox mainFancyCorners;
        private System.Windows.Forms.Button about;
        private System.Windows.Forms.ComboBox trackerGameVersion;
        private System.Windows.Forms.ComboBox overlayDirection;
        private System.Windows.Forms.CheckBox mainLayoutDebug;
        private System.Windows.Forms.NumericUpDown trackerRefreshDelay;
        private System.Windows.Forms.Label labelRefresh;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox trackerAutoVersion;
        private System.Windows.Forms.LinkLabel copyColorKey;
        private System.Windows.Forms.Button update;
        private Controls.CCredits credits;
        private System.Windows.Forms.CheckBox mainCompletionGlow;
    }
}