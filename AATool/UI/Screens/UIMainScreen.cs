using System;
using System.Diagnostics;
using System.Xml;
using AATool.Winforms.Forms;
using AATool.Settings;
using AATool.UI.Controls;
using Microsoft.Xna.Framework;
using AATool.Net;
using System.Windows.Forms;
using System.Security.Cryptography;
using Microsoft.Xna.Framework.Graphics;
using AATool.Utilities.Easings;
using AATool.Graphics;
using System.Reflection;

namespace AATool.UI.Screens
{
    public class UIMainScreen : UIScreen
    {
        private const int MIN_WIDTH  = 1280;
        private const int MIN_HEIGHT = 720;

        private FSettings settingsMenu;
        private UIGrid grid;
        private UILobby lobby;
        private UIStatusBar status;

        public UIMainScreen(Main main) : base(main, main.Window)
        {
            //set window title
            this.Form.Text = Main.FullTitle;
            this.Form.FormClosing += this.OnFormClosing;
            //this.Form.Location = new System.Drawing.Point(0, 0);
            this.ReloadLayout();
            this.CenterWindow();
        }

        public void OpenSettingsMenu()
        {
            if (this.settingsMenu is null || this.settingsMenu.IsDisposed)
            {
                this.settingsMenu = new FSettings();
                this.settingsMenu.Show(this.Form);
            }
        }

        private void CenterWindow()
        {
            System.Drawing.Rectangle desktop = Screen.FromControl(this.Form).WorkingArea;
            int x = Math.Max(desktop.X, desktop.X + ((desktop.Width  - this.grid.GetExpandedWidth())  / 2));
            int y = Math.Max(desktop.Y, desktop.Y + ((desktop.Height - this.grid.GetExpandedHeight()) / 2));
            this.Form.Location = new System.Drawing.Point(x, y);
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            if (Peer.IsServer && Peer.TryGetLobby(out Lobby lobby) && lobby.UserCount > 1)
            {
                int clients = lobby.UserCount - 1;
                string caption = "Co-op Shutdown Confirmation";
                string players = clients is 1 ? "1 player" : $"{clients} players";
                string text = $"You are currently hosting a Co-op lobby with {players} connected! Are you sure you want to quit?";
                DialogResult result = MessageBox.Show(this.Form, text, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (result is DialogResult.Yes)
                    Peer.StopInstance();
                else
                    e.Cancel = true;
            }
            Main.IsClosing = !e.Cancel;
        }

        private void UpdateForcedCompactMode()
        {
            System.Drawing.Rectangle desktop = Screen.FromControl(this.Form).WorkingArea;
            if (desktop.Width < MIN_WIDTH || desktop.Height < MIN_HEIGHT)
            {
                string title = "Compact Mode Enabled";
                string message = "Your display resolution is too small for Relaxed View. Compact View has been enabled.";
                Config.Main.CompactMode = true;
                MessageBox.Show(this.Form, message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        protected override void ReloadLayout()
        {
            Peer.UnbindController(this.status);
            this.Children.Clear();
            string variant = Config.Main.CompactMode ? "compact" : "relaxed";
             if (!this.TryLoadXml(Paths.GetLayoutFor("main", variant)))
                Main.QuitBecause("Error loading main layout!");
            this.InitializeRecursive(this);
            this.ResizeRecursive(this.Bounds);
        }

        public override void InitializeRecursive(UIScreen screen)
        {
            this.grid   = this.First<UIGrid>();
            this.lobby  = this.First<UILobby>();
            this.status = this.First<UIStatusBar>();
            Peer.BindController(this.status);
            base.InitializeRecursive(screen);
        }

        protected override void ConstrainWindow()
        {
            if (this.grid is not null)
            {
                int width  = this.grid.GetExpandedWidth();
                int height = this.grid.GetExpandedHeight();
                if (this.FormWidth != width || this.FormHeight != height || Config.Tracker.GameVersionChanged())
                {
                    this.Form.ClientSize = new System.Drawing.Size(width, height);
                    Main.Graphics.PreferredBackBufferWidth  = width;
                    Main.Graphics.PreferredBackBufferHeight = height;
                    Main.Graphics.ApplyChanges();
                    this.ResizeRecursive(new Rectangle(0, 0, width, height));
                }
            }
        }

        protected override void UpdateThis(Time time)
        {
            //update game version
            if (Config.Tracker.GameVersionChanged() || Config.Main.ValueChanged(MainSettings.COMPACT_MODE))
                this.ReloadLayout();
            
            this.UpdateForcedCompactMode();
            this.UpdateCollapsedStates();

            //keep settings menu version up to date
            if (Config.Tracker.GameVersionChanged() || Peer.StateChangedFlag)
                this.settingsMenu?.UpdateGameVersion();

            if (Config.Overlay.WidthChanged())
                this.settingsMenu?.UpdateOverlayWidth();

            this.ConstrainWindow();
        }

        private void UpdateCollapsedStates()
        {
            //update whether or not top row should be shown
            if (Config.Main.ShowBasic == this.grid.CollapsedRows[0])
            {
                if (Config.Main.ShowBasic)
                    this.grid.ExpandRow(0);
                else if (Config.IsPostExplorationUpdate)
                    this.grid.CollapseRow(0);
            }

            //update whether or not the co-op column should be shown
            if (Peer.IsConnected)
            {
                if (this.grid.CollapsedColumns[0])
                {
                    this.grid.ExpandCol(0);
                }   
            }
            else
            {
                if (!this.grid.CollapsedColumns[0])
                {
                    this.grid.CollapseCol(0);
                    this.lobby.Clear();
                }
            }
        }

        public override void Prepare(Display display)
        {
            base.Prepare(display);
            this.GraphicsDevice.Clear(Config.Main.BackColor);
        }

        public override void DrawThis(Display display)
        {
            Color color = display.RainbowColor;
            this.settingsMenu?.UpdateRainbow(color);

            if (Config.Main.RainbowMode)
            {
                Config.Main.BackColor   = color;
                Config.Main.BorderColor = new Color((int)(color.R / 1.25f), (int)(color.G / 1.25f), (int)(color.B / 1.25f), 255);
                Config.Main.TextColor   = Color.Black;
            }
        }
    }
}
