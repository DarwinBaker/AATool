using System;
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
        private UIProgressBar progressBar;
        private UITextBlock progressLabel;
        private UITextBlock saveLabel;

        private System.Windows.Forms.Button donate;
        private FSettings settingsMenu;

        public MainScreen(Main main) : base(main, main.Window, 0, 0)
        {
            LoadXml(Path.Combine(Paths.DIR_SCREENS, "screen_main.xml"));

            saveLabel     = GetControlByName("label_save", true) as UITextBlock;
            progressLabel = GetControlByName("label_progress", true) as UITextBlock;
            progressBar   = GetControlByName("progress_bar", true) as UIProgressBar;

            progressBar?.SetMin(0);
            progressBar?.SetMax(AdvancementTracker.AdvancementCount);

            Show();
        }

        protected override void UpdateThis(Time time)
        {
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
            }

            //update total completion progress display
            if (progressLabel != null)
            {
                progressLabel.SetText("Advancements Completed: ");
                progressLabel.Append(AdvancementTracker.CompletedCount.ToString());
                progressLabel.Append(" / ");
                progressLabel.Append(AdvancementTracker.AdvancementCount.ToString());
                progressLabel.Append(" (");
                progressLabel.Append(AdvancementTracker.CompletedPercent.ToString());
                progressLabel.Append("%)");
            }

            progressBar?.SetValue(AdvancementTracker.CompletedPercent);

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

            //add winforms buttons because lazy
            var settings = new System.Windows.Forms.Button();
            settings.Name = "settings";
            settings.Text = "Settings";
            settings.Click += ButtonClick;
            settings.Size = new System.Drawing.Size(70, 30);
            settings.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            settings.Location = new System.Drawing.Point(260, Height - 6 - settings.Height);
            Form.Controls.Add(settings);

            donate = new System.Windows.Forms.Button();
            donate.Name = "donate";
            //donate.Text = "PayPal.me/DarwinBaker";
            donate.Text = "♥ Support me on Patreon ♥";
            donate.Click += ButtonClick;
            donate.Size = new System.Drawing.Size(170, 29);
            donate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;  
            donate.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            donate.Location = new System.Drawing.Point(Width - donate.Width - 6, Height - 6 - donate.Height);
            donate.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            Form.Controls.Add(donate);
        }

        private void ButtonClick(object sender, EventArgs e)
        {
            switch ((sender as System.Windows.Forms.Control).Name)
            {
                case "settings":
                    using (var dialog = new FSettings(AdvancementTracker))
                    {
                        settingsMenu = dialog;
                        dialog.ShowDialog();
                    }
                    break;
                case "donate":
                    Process.Start("https://www.patreon.com/_ctm");
                    break;
            }
        }

        public override void DrawThis(Display display)
        {
            var color = display.RainbowColor;
            settingsMenu?.UpdateRainbow(color);
            donate.BackColor = System.Drawing.Color.FromArgb(255, color.R, color.G, color.B);
            if (settings.RainbowMode)
            {
                settings.BackColor = color;
                settings.BorderColor = new Color((int)(color.R / 1.25f), (int)(color.G / 1.25f), (int)(color.B / 1.25f), 255);
                settings.TextColor = Color.Black;
            }
        }
    }
}
