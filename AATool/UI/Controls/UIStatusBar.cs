using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using AATool.Graphics;
using AATool.Net;
using AATool.Saves;
using AATool.Settings;
using AATool.UI.Screens;
using AATool.Utilities;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    class UIStatusBar : UIControl, INetworkController
    {
        private const double CONSOLE_OUTPUT_HOLD = 3;

        //settings panel
        private UIControl settings;
        private UIButton settingsButton;
        
        //status panel
        private UIControl status;
        private UIEnchantmentTable readIcon;
        private UIButton manualSync;
        private UITextBlock statusLabel;

        //progress panel
        private UIControl progress;
        private UITextBlock progressLabel;
        private UIProgressBar progressBar;

        //patreon panel
        private UIControl patreon;
        private UIButton patreonButton;
        private UITextBlock donate;

        private Timer refreshTimer;
        private int rightPanelWidth;


        public UIStatusBar() 
        {
            this.refreshTimer = new Timer(CONSOLE_OUTPUT_HOLD);
            this.BuildFromSourceDocument();
        }

        public override void InitializeRecursive(UIScreen screen)
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
            this.manualSync  = this.status.First<UIButton>("manual_sync");

            //progress panel
            this.progress      = this.First("progress");
            this.progressLabel = this.progress.First<UITextBlock>();
            this.progressBar   = this.progress.First<UIProgressBar>();
            this.progressBar.SetMin(0);
            this.progressBar.SetMax(Tracker.AdvancementCount);

            //patreon panel
            this.patreon          = this.First("patreon");
            this.patreonButton    = this.patreon.First<UIButton>();
            this.donate           = this.patreon.First<UITextBlock>();
            this.donate.FlexWidth = new Size(Math.Min(this.rightPanelWidth - 170, 270));
            if (this.patreonButton is not null)
            {
                this.patreonButton.UseCustomColor = true;
                this.patreonButton.SetTextColor(Color.Black);
                this.patreonButton.OnClick += this.OnClick;
            }
            this.statusLabel.HorizontalTextAlign = HorizontalAlign.Left;
            this.statusLabel.SetText(this.GetLabelText());
            base.InitializeRecursive(screen);
        }

        public override void ResizeRecursive(Rectangle parent)
        {
            this.patreon.FlexWidth = new Size(this.rightPanelWidth);
            string donationMessage = this.donate.FlexWidth.GetAbsoluteInt(0) >= 270
                    ? "If you like AATool and want to support its continued development, please consider donating! :)"
                    : "Please consider supporting AATool's development! :)";

            this.donate.SetText(donationMessage);
            this.progress.FlexWidth = new Size(parent.Width 
                - (this.settings.FlexWidth.GetAbsoluteInt(0) + this.status.FlexWidth.GetAbsoluteInt(0) + this.rightPanelWidth));
            
            this.progressBar.SkipToValue(Tracker.CompletedAdvancements);
            base.ResizeRecursive(parent);
        }

        protected override void UpdateThis(Time time)
        {
            //update save name display
            if (Peer.IsConnected)
                this.readIcon.UpdateState(true);
            else
                this.readIcon.UpdateState(Tracker.SaveState is SaveFolderState.Valid);
            
            this.refreshTimer.Update(time);
            if (this.refreshTimer.IsExpired)
            {
                this.refreshTimer.Reset();
                this.statusLabel.SetText(this.GetLabelText());
            }
            else if (Tracker.WorldFolderChanged || Peer.StateChangedFlag || !string.IsNullOrEmpty(SftpSave.GetStatusText()))
            {
                this.statusLabel.SetText(this.GetLabelText());
            }

            //update total completion progress
            string version  = Config.Tracker.GameVersion;
            string name     = Config.IsPostExplorationUpdate ? "Advancements" : "Achievements";
            string progress = $"{Tracker.CompletedAdvancements} / {Tracker.AllAdvancements.Count} {name} ({Tracker.Percent}%)";
            if (this.progressBar.Width > 250)
                progress += $"    -    {Tracker.InGameTime} IGT";
            this.progressLabel.SetText(progress);
            this.progressBar.LerpToValue(Tracker.CompletedAdvancements);
        }

        private string GetLabelText()
        {
            if (Client.TryGet(out Client client))
                return client.GetStatusText();

            if (Config.Tracker.UseRemoteWorld)
                return SftpSave.GetStatusText();

            //determine label text
            return Tracker.SaveState switch {
                SaveFolderState.Valid => (Peer.IsServer && Peer.IsConnected 
                    ? $"Hosting:"
                    : $"Tracking:") + $" \"{Tracker.WorldName}\"",
                SaveFolderState.NoWorlds => 
                    $"No worlds in {(Config.Tracker.UseDefaultPath ? "default" : "custom") } save path",
                SaveFolderState.NonExistentPath => Config.Tracker.UseDefaultPath 
                    ? $"Default save folder is missing" 
                    : $"Custom save path doesn't exist",
                SaveFolderState.InvalidPath     => 
                    $"Illegal character(s) in custom save path",
                SaveFolderState.PathTooLong     => 
                    $"Custom save path is too long",
                SaveFolderState.EmptyPath       => 
                    $"Custom save path is empty",
                SaveFolderState.PermissionError => 
                    $"A read permission error has occurred",
                _ => 
                    "An unknown error has occurred",
            };
        }

        public override void DrawThis(Display display)
        {
            base.DrawThis(display);
            Color color = display.RainbowColor;
            if (this.patreonButton is not null)
            {
                this.patreonButton.BackColor   = color;
                this.patreonButton.BorderColor = Color.FromNonPremultiplied((int)(color.R / 1.5f), (int)(color.G / 1.5f), (int)(color.B / 1.5f), 255);
            }
        }

        private void OnClick(UIControl sender)
        {
            if (sender == this.settingsButton)
            {
                if (this.GetRootScreen() is UIMainScreen mainScreen)
                    mainScreen.OpenSettingsMenu();
            }
            else if (sender == this.patreonButton)
            {
                Process.Start(Paths.URL_PATREON);
            }
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            this.rightPanelWidth = ParseAttribute(node, "right", 0);
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
