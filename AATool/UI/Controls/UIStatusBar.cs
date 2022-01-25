using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using AATool.Configuration;
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
            this.progress      = this.First("progress");
            this.progressLabel = this.progress.First<UITextBlock>();
            this.progressBar   = this.progress.First<UIProgressBar>();
            this.progressBar.SetMin(0);
            this.progressBar.SetMax(100);

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
            
            this.progressBar.SkipToValue(Tracker.Category.GetCompletionPercent());
            base.ResizeRecursive(parent);
        }

        protected override void UpdateThis(Time time)
        {
            //update save name display
            if (Peer.IsConnected)
                this.readIcon.UpdateState(true);
            else
                this.readIcon.UpdateState(Tracker.SaveState is SaveState.Valid);
            
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
                progress += $"\n{Tracker.InGameTime} IGT\n{new string('-', this.progressBar.Width / 7)}";
            }
                
            this.progressLabel.SetText(progress);
            this.progressBar.LerpToValue(Tracker.Category.GetCompletionPercent());
        }

        private string GetLabelText()
        {
            if (Client.TryGet(out Client client))
                return client.GetStatusText();

            if (Config.Tracking.UseSftp)
                return SftpSave.GetStatusText();

            //determine label text
            return Tracker.SaveState switch {
                SaveState.Valid => (Peer.IsServer && Peer.IsConnected 
                    ? $"Hosting:"
                    : $"Tracking:") + $" \"{Tracker.WorldName}\"",
                SaveState.NoWorlds => 
                    $"No worlds in {(Config.Tracking.UseDefaultPath ? "default" : "custom") } save path",
                SaveState.PathNonExistent => Config.Tracking.UseDefaultPath 
                    ? $"Default save folder is missing" 
                    : $"Custom save path doesn't exist",
                SaveState.PathInvalid     => 
                    $"Illegal character(s) in custom save path",
                SaveState.PathTooLong     => 
                    $"Custom save path is too long",
                SaveState.PathEmpty       => 
                    $"Custom save path is empty",
                SaveState.PermissionError => 
                    $"A read permission error has occurred",
                _ => 
                    "An unknown error has occurred",
            };
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
