﻿using System;
using System.Diagnostics;
using System.Xml;
using AATool.Winforms.Forms;
using AATool.Settings;
using AATool.UI.Controls;
using System.IO;
using Microsoft.Xna.Framework;

namespace AATool.UI.Screens
{
    public class MainScreen : Screen
    {
        private MainSettings settings = MainSettings.Instance;
        private UIButton settingsButton;
        private UIButton patreonButton;
        private UIProgressBar progressBar;
        private UITextBlock progressLabel;
        private UITextBlock saveLabel;
        private UIEnchantmentTable status;
        private FSettings settingsMenu;

        public MainScreen(Main main) : base(main, main.Window, 0, 0)
        {
            ReloadLayout();
            Show();
        }

        private void ReloadLayout()
        {
            Children.Clear();
            if (!LoadXml(Paths.GetLayoutFor("main")))
                Main.ForceQuit();

            //find named controls
            saveLabel = GetControlByName("label_save", true) as UITextBlock;
            progressLabel = GetControlByName("label_progress", true) as UITextBlock;
            progressBar = GetControlByName("progress_bar", true) as UIProgressBar;
            progressBar?.SetMin(0);
            progressBar?.SetMax(AdvancementTracker.AdvancementCount);
            status = GetControlByName("enchantment_table", true) as UIEnchantmentTable;
            status?.SetTint(Color.White * 0.85f);
            settingsButton = GetControlByName("button_settings", true) as UIButton;
            if (settingsButton != null)
                settingsButton.Click += OnClick;
            patreonButton = GetControlByName("button_patreon", true) as UIButton;
            if (patreonButton != null)
            {
                patreonButton.UseCustomColor = true;
                patreonButton.SetTextColor(Color.Black);
                patreonButton.Click += OnClick;
            } 
        }

        private void OnClick(object sender)
        {
            if (sender == settingsButton)
            {
                if (settingsMenu == null || settingsMenu.IsDisposed)
                {
                    settingsMenu = new FSettings(Form, AdvancementTracker, StatisticsTracker);
                    settingsMenu.Show(Form);
                }
            }
            else if (sender == patreonButton)
                Process.Start("https://www.patreon.com/_ctm");
        }

        protected override void UpdateThis(Time time)
        {
            //update game version
            if (TrackerSettings.Instance.ValueChanged(TrackerSettings.GAME_VERSION))
                ReloadLayout();

            //update save name display
            if (saveLabel != null)
            {
                string save = AdvancementTracker.CurrentSaveName; 
                if (save == null)
                    saveLabel.SetText("Not Currently Reading a Save.");
                else
                {
                    saveLabel.SetText("Reading Save: \"");
                    saveLabel.Append(save);
                    saveLabel.Append("\"");
                }
                status?.UpdateState(save != null);
            }

            //update total completion progress display
            if (progressLabel != null)
            {
                progressLabel.SetText("(" + TrackerSettings.Instance.GameVersion + ") ");
                progressLabel.Append("Advancements Completed: ");
                progressLabel.Append(AdvancementTracker.CompletedCount.ToString());
                progressLabel.Append(" / ");
                progressLabel.Append(AdvancementTracker.AdvancementCount.ToString());
                progressLabel.Append(" (");
                progressLabel.Append(AdvancementTracker.CompletedPercent.ToString());
                progressLabel.Append("%)");
            }

            progressBar?.SetValue(AdvancementTracker.CompletedCount);

            var grid = GetFirstOfType(typeof(UIGrid)) as UIGrid;
            if (grid == null || grid.CollapsedRows.Count == 0)
                return;

            //update whether or not top row should be shown
            if (settings.ShowBasic == grid.CollapsedRows[0])
            {
                if (settings.ShowBasic)
                {
                    grid.ExpandRow(0);
                    SetWindowSize(grid.Width, grid.Height);
                }
                else
                {
                    grid.CollapseRow(0);
                    SetWindowSize(grid.Width, grid.Height);
                }
            }

            if (TrackerSettings.Instance.ValueChanged(TrackerSettings.GAME_VERSION))
                settingsMenu?.UpdateGameVersion();
        }

        public override void Prepare(Display display)
        {
            base.Prepare(display);
            GraphicsDevice.Clear(settings.BackColor);
        }

        public override void SetWindowSize(int width, int height)
        {
            Main.GraphicsManager.PreferredBackBufferWidth = width;
            Main.GraphicsManager.PreferredBackBufferHeight = height;
            Main.GraphicsManager.ApplyChanges();
        }

        public override void ReadDocument(XmlDocument document)
        {
            base.ReadDocument(document);
            SetWindowSize(Width, Height);
        }

        public override void DrawThis(Display display)
        {
            var color = display.RainbowColor;
            settingsMenu?.UpdateRainbow(color);
            if (patreonButton != null)
            {
                patreonButton.BackColor   = color;
                patreonButton.BorderColor = Color.FromNonPremultiplied((int)(color.R / 1.5f), (int)(color.G / 1.5f), (int)(color.B / 1.5f), 255);
            }
            if (settings.RainbowMode)
            {
                settings.BackColor   = color;
                settings.BorderColor = new Color((int)(color.R / 1.25f), (int)(color.G / 1.25f), (int)(color.B / 1.25f), 255);
                settings.TextColor   = Color.Black;
            }
        }
    }
}
