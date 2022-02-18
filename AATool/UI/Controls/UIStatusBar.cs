using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using AATool.Configuration;
using AATool.Data.Categories;
using AATool.Graphics;
using AATool.Net;
using AATool.Saves;
using AATool.UI.Screens;
using AATool.Utilities;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    class UIStatusBar : UIControl, INetworkController
    {
        private const double ConsoleHoldTime = 3;

        //settings panel
        private UIControl settings;
        private UIButton settingsButton;
        private UIButton manualClearButton;

        //status panel
        private UIControl status;
        private UIEnchantmentTable readIcon;
        private UITextBlock statusLabel;

        //progress panel
        private UIControl progress;
        private UITextBlock progressLabel;
        private UIProgressBar progressBar;

        //patreon panel
        private UIControl patreon;
        private UIButton patreonButton;
        private UITextBlock donateMessage;

        private readonly Timer refreshTimer;
        private int rightPanelWidth;


        public UIStatusBar() 
        {
            this.refreshTimer = new Timer(ConsoleHoldTime);
            this.BuildFromTemplate();
        }

        public override void InitializeThis(UIScreen screen)
        {
            //settings panel
            this.settings = this.First("settings");
            this.settingsButton = this.settings.First<UIButton>();
            if (this.settingsButton is not null)
                this.settingsButton.OnClick += this.OnClick;

            //status panel
            this.status      = this.First("status");
            this.readIcon    = this.First<UIEnchantmentTable>();
            this.statusLabel = this.status.First<UITextBlock>();

            //progress panel
            this.progress      = this.First(Tracker.Category is AllBlocks ? "progress_blocks" : "progress_advancements");
            this.progress.Expand();
            this.progressLabel = this.progress.First<UITextBlock>();
            this.progressBar   = this.progress.First<UIProgressBar>();
            this.progressBar.SetMin(0);
            this.progressBar.SetMax(1);

            //patreon panel
            this.patreon = this.First("patreon");
            this.patreonButton = this.patreon.First<UIButton>();
            this.donateMessage = this.patreon.First<UITextBlock>();
            this.donateMessage.FlexWidth = new Size(Math.Min(this.rightPanelWidth - 170, 270));
            if (this.patreonButton is not null)
            {
                if (this.rightPanelWidth > 0)
                {
                    this.patreonButton.UseCustomColor = true;
                    this.patreonButton.SetTextColor(Color.Black);
                    this.patreonButton.OnClick += this.OnClick;
                }
                else
                {
                    this.patreonButton.Collapse();
                }
            }
            this.statusLabel.HorizontalTextAlign = HorizontalAlign.Left;
            this.statusLabel.SetText(this.GetLabelText());

            if (Tracker.Category is AllBlocks)
            {
                //all blocks controls
                this.manualClearButton = this.progress.First<UIButton>("manual_clear");
                if (this.manualClearButton is not null)
                    this.manualClearButton.OnClick += this.OnClick;
            }
        }

        public override void ResizeRecursive(Rectangle parent)
        {
            this.patreon.FlexWidth = new Size(this.rightPanelWidth);
            if (this.rightPanelWidth > 250)
            {
                string donationMessage = this.donateMessage.FlexWidth >= 270
                    ? "If you like AATool and want to support its continued development, please consider donating! :)"
                    : "Please consider supporting AATool's development! :)";
                this.donateMessage.SetText(donationMessage);
            }
            else
            {
                this.donateMessage.Collapse();
            }

            this.progress.FlexWidth = new Size(parent.Width 
                - (this.settings.FlexWidth 
                + this.status.FlexWidth 
                + this.rightPanelWidth));
            
            this.progressBar.SkipToValue(Tracker.Category.GetCompletionRatio());
            base.ResizeRecursive(parent);
        }

        protected override void UpdateThis(Time time)
        {
            //update save name display
            if (Peer.IsConnected)
                this.readIcon.UpdateState(true);
            else
                this.readIcon.UpdateState(Tracker.IsWorking);
            
            this.refreshTimer.Update(time);
            if (this.refreshTimer.IsExpired)
            {
                this.refreshTimer.Reset();
                this.statusLabel.SetText(this.GetLabelText());
            }
            else if (Tracker.ProgressChanged || Peer.StateChanged || !string.IsNullOrEmpty(SftpSave.GetStatusText()))
            {
                this.statusLabel.SetText(this.GetLabelText());
            }

            //update total completion progress
            string progress = $"{Tracker.Category.GetCompletedCount()} " +
                $"/ {Tracker.Category.GetTargetCount()} {Tracker.Category.Objective} " +
                $"({Tracker.Category.GetCompletionPercent()}%)";

            if (Config.Main.ProgressBarStyle != "None")
            {
                if (this.progressBar.Width > 250)
                    progress += $"    -    {Tracker.InGameTime:hh':'mm':'ss} IGT";
            }
            else
            {
                progress += $"\n{Tracker.InGameTime:hh':'mm':'ss} IGT\n{new string('-', this.progressBar.Width / 7)}";
            }
                
            this.progressLabel.SetText(progress);
            this.progressBar.StartLerpToValue(Tracker.Category.GetCompletionRatio());
        }

        private string GetLabelText()
        {
            if (Client.TryGet(out Client client))
                return client.GetStatusText();

            if (Config.Tracking.UseSftp)
                return SftpSave.GetStatusText();

            return Tracker.GetStatusText();
        }

        public override void DrawThis(Canvas canvas)
        {
            if (this.patreonButton is not null)
            {
                this.patreonButton.BackColor = canvas.RainbowFast;
                this.patreonButton.BorderColor = ColorHelper.Blend(canvas.RainbowFast, Color.Black, 0.6f);
            }
        }

        private void OnClick(UIControl sender)
        {
            if (sender == this.settingsButton)
            {
                if (this.Root() is UIMainScreen mainScreen)
                    mainScreen.OpenSettingsMenu();
            }
            else if (sender == this.patreonButton)
            {
                Process.Start(Paths.Web.PatreonFull);
            }
            else if (sender == this.manualClearButton)
            {
                var controls = new List<UIControl>();
                this.Root().GetTreeRecursive(controls);
                foreach (UIControl control in controls)
                {
                    if (control is UIBlockTile block)
                        block.Block.ManuallyCompleted = false;
                }
            }
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            this.rightPanelWidth = Attribute(node, "right", 0);
        }

        public void SetControlStates(string buttonText, bool enableButton, bool enableDropDown) { }

        public void WriteToConsole(string text) 
        {
            this.refreshTimer.Reset();
            this.statusLabel.SetText(text);
        }

        public void SyncConsole() { }
        public void SyncUserList(IEnumerable<User> users) { }
    }
}
