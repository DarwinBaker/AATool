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
            this.done = new System.Windows.Forms.Button();
            this.colors = new System.Windows.Forms.ColorDialog();
            this.reset = new System.Windows.Forms.Button();
            this.about = new System.Windows.Forms.Button();
            this.update = new System.Windows.Forms.Button();
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabMain = new System.Windows.Forms.TabPage();
            this.tabOverlay = new System.Windows.Forms.TabPage();
            this.tabNetwork = new System.Windows.Forms.TabPage();
            this.main = new AATool.Winforms.Controls.CMainSettings();
            this.overlay = new AATool.Winforms.Controls.COverlaySettings();
            this.network = new AATool.Winforms.Controls.CNetworkSettings();
            this.credits = new AATool.Winforms.Controls.CCredits();
            this.tabs.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.tabOverlay.SuspendLayout();
            this.tabNetwork.SuspendLayout();
            this.SuspendLayout();
            // 
            // done
            // 
            this.done.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.done.Location = new System.Drawing.Point(12, 365);
            this.done.Margin = new System.Windows.Forms.Padding(10, 10, 9, 3);
            this.done.Name = "done";
            this.done.Size = new System.Drawing.Size(100, 32);
            this.done.TabIndex = 5;
            this.done.Text = "Done";
            this.done.UseVisualStyleBackColor = true;
            this.done.Click += new System.EventHandler(this.OnButtonClick);
            // 
            // colors
            // 
            this.colors.AnyColor = true;
            this.colors.Color = System.Drawing.SystemColors.Control;
            this.colors.SolidColorOnly = true;
            // 
            // reset
            // 
            this.reset.Location = new System.Drawing.Point(332, 365);
            this.reset.Name = "reset";
            this.reset.Size = new System.Drawing.Size(100, 32);
            this.reset.TabIndex = 20;
            this.reset.Text = "Reset to Defaults";
            this.reset.UseVisualStyleBackColor = true;
            this.reset.Click += new System.EventHandler(this.OnButtonClick);
            // 
            // about
            // 
            this.about.Location = new System.Drawing.Point(672, 365);
            this.about.Name = "about";
            this.about.Size = new System.Drawing.Size(120, 32);
            this.about.TabIndex = 21;
            this.about.Text = "About this Tool";
            this.about.UseVisualStyleBackColor = true;
            this.about.Click += new System.EventHandler(this.OnButtonClick);
            // 
            // update
            // 
            this.update.Location = new System.Drawing.Point(438, 365);
            this.update.Name = "update";
            this.update.Size = new System.Drawing.Size(120, 32);
            this.update.TabIndex = 23;
            this.update.Text = "Check for Updates";
            this.update.UseVisualStyleBackColor = true;
            this.update.Click += new System.EventHandler(this.OnButtonClick);
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.tabMain);
            this.tabs.Controls.Add(this.tabOverlay);
            this.tabs.Controls.Add(this.tabNetwork);
            this.tabs.Location = new System.Drawing.Point(12, 12);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(550, 340);
            this.tabs.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabs.TabIndex = 25;
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.main);
            this.tabMain.Location = new System.Drawing.Point(4, 22);
            this.tabMain.Name = "tabMain";
            this.tabMain.Padding = new System.Windows.Forms.Padding(3);
            this.tabMain.Size = new System.Drawing.Size(542, 314);
            this.tabMain.TabIndex = 0;
            this.tabMain.Text = "Main Settings";
            this.tabMain.UseVisualStyleBackColor = true;
            // 
            // tabOverlay
            // 
            this.tabOverlay.Controls.Add(this.overlay);
            this.tabOverlay.Location = new System.Drawing.Point(4, 22);
            this.tabOverlay.Name = "tabOverlay";
            this.tabOverlay.Padding = new System.Windows.Forms.Padding(3);
            this.tabOverlay.Size = new System.Drawing.Size(542, 314);
            this.tabOverlay.TabIndex = 1;
            this.tabOverlay.Text = "Overlay Settings";
            this.tabOverlay.UseVisualStyleBackColor = true;
            // 
            // tabNetwork
            // 
            this.tabNetwork.Controls.Add(this.network);
            this.tabNetwork.Location = new System.Drawing.Point(4, 22);
            this.tabNetwork.Name = "tabNetwork";
            this.tabNetwork.Padding = new System.Windows.Forms.Padding(3);
            this.tabNetwork.Size = new System.Drawing.Size(542, 314);
            this.tabNetwork.TabIndex = 2;
            this.tabNetwork.Text = "Co-op Settings";
            this.tabNetwork.UseVisualStyleBackColor = true;
            // 
            // main
            // 
            this.main.BackColor = System.Drawing.SystemColors.Window;
            this.main.Location = new System.Drawing.Point(3, 3);
            this.main.Name = "main";
            this.main.Size = new System.Drawing.Size(538, 307);
            this.main.TabIndex = 0;
            // 
            // overlay
            // 
            this.overlay.BackColor = System.Drawing.SystemColors.Window;
            this.overlay.Location = new System.Drawing.Point(3, 3);
            this.overlay.Name = "overlay";
            this.overlay.Size = new System.Drawing.Size(538, 307);
            this.overlay.TabIndex = 0;
            // 
            // network
            // 
            this.network.BackColor = System.Drawing.SystemColors.Window;
            this.network.Location = new System.Drawing.Point(3, 3);
            this.network.Name = "network";
            this.network.Size = new System.Drawing.Size(538, 307);
            this.network.TabIndex = 0;
            // 
            // credits
            // 
            this.credits.Location = new System.Drawing.Point(568, 12);
            this.credits.Name = "credits";
            this.credits.Size = new System.Drawing.Size(224, 340);
            this.credits.TabIndex = 24;
            // 
            // FSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.done;
            this.ClientSize = new System.Drawing.Size(807, 410);
            this.Controls.Add(this.tabs);
            this.Controls.Add(this.credits);
            this.Controls.Add(this.update);
            this.Controls.Add(this.about);
            this.Controls.Add(this.reset);
            this.Controls.Add(this.done);
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
            this.Activated += new System.EventHandler(this.OnActivated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.tabs.ResumeLayout(false);
            this.tabMain.ResumeLayout(false);
            this.tabOverlay.ResumeLayout(false);
            this.tabNetwork.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button done;
        private System.Windows.Forms.ColorDialog colors;
        private System.Windows.Forms.Button reset;
        private System.Windows.Forms.Button about;
        private System.Windows.Forms.Button update;
        private Controls.CCredits credits;
        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage tabMain;
        private System.Windows.Forms.TabPage tabOverlay;
        private System.Windows.Forms.TabPage tabNetwork;
        private Controls.CNetworkSettings network;
        private Controls.CMainSettings main;
        private Controls.COverlaySettings overlay;
    }
}