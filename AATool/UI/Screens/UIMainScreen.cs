using System;
using System.IO;
using System.Windows.Forms;
using AATool.Configuration;
using AATool.Data.Categories;
using AATool.Graphics;
using AATool.Net;
using AATool.UI.Controls;
using AATool.Utilities;
using AATool.Winforms.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AATool.UI.Screens
{
    public class UIMainScreen : UIScreen
    {
        private const int MIN_WIDTH  = 1280;
        private const int MIN_HEIGHT = 720;

        public static RenderTarget2D RenderCache { get; private set; }
        public static bool Invalidated { get; private set; }
        public static bool RefreshingCache { get; set; }
        public static bool Invalidate() => Invalidated = true;

        private FSettings settingsMenu;
        private UIGrid grid;
        private UILobby lobby;
        private UIStatusBar status;
        private UIBlockPopup popup;
        private readonly Utilities.Timer settingsCooldown;

        private KeyboardState kbNow;
        private KeyboardState kbPrev;

        public override Color FrameBackColor() => Config.Main.BackColor;
        public override Color FrameBorderColor() => Config.Main.BorderColor;

        public UIMainScreen(Main main) : base(main, main.Window)
        {
            //set window title
            this.Form.Text = Main.FullTitle;
            this.Form.FormClosing += this.OnFormClosing;
            this.settingsCooldown = new Utilities.Timer(0.1f);
            //this.Form.Location = new System.Drawing.Point(0, 0);
            this.ReloadLayout();
            this.CenterWindow();
        }

        public void CloseSettingsMenu()
        {
            //prevent settings from re-opening immediately if escape is used to close the window
            this.settingsCooldown.Reset();
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
            int x = Math.Max(desktop.X, desktop.X + ((desktop.Width  - this.grid?.GetExpandedWidth() ?? 0)  / 2));
            int y = Math.Max(desktop.Y, desktop.Y + ((desktop.Height - this.grid?.GetExpandedHeight() ?? 0) / 2));
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
                DialogResult result = System.Windows.Forms.MessageBox.Show(this.Form, text, caption,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Exclamation);

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
                Config.Main.CompactMode.Set(true);
                this.ReloadLayout();
                System.Windows.Forms.MessageBox.Show(this.Form, message, title,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        public override void ReloadLayout()
        {
            Peer.UnbindController(this.status);
            this.Children.Clear();
            if (this.TryLoadXml(Paths.System.GetLayoutFor(this)))
            {
                this.InitializeRecursive(this);
                this.ResizeRecursive(this.Bounds);
                if (Tracker.Category is AllBlocks)
                    this.Form.Icon = new System.Drawing.Icon(Path.Combine(Paths.System.AssetsFolder, "icons", "all_blocks.ico"));
                else
                    this.Form.Icon = new System.Drawing.Icon(Paths.System.MainIcon);
            }
            else
            {
                this.ShowErrorScreen();
            }
        }

        public override void InitializeThis(UIScreen screen)
        {
            this.grid   = this.First<UIGrid>();
            this.lobby  = this.First<UILobby>();
            this.status = this.First<UIStatusBar>();
            this.popup = this.First<UIBlockPopup>();
            Peer.BindController(this.status);
        }

        protected override void ConstrainWindow()
        {
            int width = this.grid?.GetExpandedWidth() ?? 640;
            int height = this.grid?.GetExpandedHeight() ?? 320;
            if (this.FormWidth != width || this.FormHeight != height || Tracker.ObjectivesChanged)
            {
                this.Form.ClientSize = new System.Drawing.Size(width, height);
                Main.Graphics.PreferredBackBufferWidth  = width;
                Main.Graphics.PreferredBackBufferHeight = height;
                Main.Graphics.ApplyChanges();
                this.ResizeRecursive(new Rectangle(0, 0, width, height));
                RenderCache?.Dispose();
                RenderCache = new RenderTarget2D(this.GraphicsDevice, width, height);
            }
        }

        public override void UpdateRecursive(Time time)
        {
            base.UpdateRecursive(time);
            this.popup?.Finalize(time);
        }

        protected override void UpdateThis(Time time)
        {
            //update game version
            if (Tracker.ObjectivesChanged || Config.Main.CompactMode.Changed)
                this.ReloadLayout();
            
            this.UpdateForcedCompactMode();
            this.UpdateCollapsedStates();

            //keep settings menu version up to date
            if (Tracker.ObjectivesChanged || Peer.StateChanged)
                this.settingsMenu?.InvalidateSettings();

            if (Config.Overlay.Width.Changed)
                this.settingsMenu?.UpdateOverlayWidth();

            if (Config.Main.BackColor.Changed
                || Config.Main.BorderColor.Changed
                || Config.Main.TextColor.Changed
                || Config.Main.FrameStyle.Changed
                || Config.Main.ProgressBarStyle.Changed)
            {
                Invalidate();
            }

            //escape to open settings
            this.settingsCooldown.Update(time);
            this.kbNow = Keyboard.GetState();
            var escape = Microsoft.Xna.Framework.Input.Keys.Escape;
            if (this.kbNow.IsKeyDown(escape) && this.kbPrev.IsKeyUp(escape) && this.settingsCooldown.IsExpired)
                this.OpenSettingsMenu();
            this.kbPrev = this.kbNow;

            //enforce window size
            this.ConstrainWindow();
        }

        private void ShowErrorScreen() 
        {
            Peer.UnbindController(this.status);
            this.Children.Clear();

            var label = new UITextBlock();
            label.SetFont("minecraft", 24);
            label.SetText($"There was an error attempting to load layout file:\n{Paths.System.GetLayoutFor(this)}");
            this.AddControl(label);

            var settings = new UIButton() {
                FlexWidth = new Size(140),
                FlexHeight = new Size(50),
                VerticalAlign = VerticalAlign.Top,
                Margin = new Margin(0, 0, 200, 0),
            };
            settings.OnClick += this.OnClick;
            settings.TextBlock.SetFont("minecraft", 24);
            settings.SetText("Settings");
            this.AddControl(settings);

            this.InitializeRecursive(this);
            this.ResizeRecursive(this.Bounds);
        }

        private void OnClick(UIControl sender)
        {
            this.OpenSettingsMenu();
        }

        private void UpdateCollapsedStates()
        {
            if (this.grid is null)
                return;

            //update whether or not top row should be shown
            if (Config.Main.ShowBasicAdvancements == this.grid.CollapsedRows[0])
            {
                if (Config.Main.ShowBasicAdvancements)
                    this.grid.ExpandRow(0);
                else if (Tracker.Category is not AllAchievements)
                    this.grid.CollapseRow(0);
            }

            //update whether or not the co-op column should be shown
            if (Peer.IsConnected || (Client.TryGet(out Client client) && client.LostConnection))
            {
                this.grid.ExpandCol(0);
            }
            else
            {
                if (!this.grid.CollapsedColumns[0])
                {
                    this.grid.CollapseCol(0);
                    this.lobby?.Clear();
                }
            }
        }

        public override void Prepare(Canvas canvas)
        {
            base.Prepare(canvas);
            this.GraphicsDevice.Clear(Tracker.Category is AllBlocks ? Config.Main.BorderColor : Config.Main.BackColor);
        }

        public override void Present(Canvas canvas) 
        { 
            base.Present(canvas);
            Invalidated = false;
            RefreshingCache = false;
        }

        public override void DrawRecursive(Canvas canvas)
        {
            base.DrawRecursive(canvas);
        }

        public override void DrawThis(Canvas canvas)
        {
            Color back = canvas.RainbowLight;
            var border = new Color((int)(back.R / 1.25f), (int)(back.G / 1.25f), (int)(back.B / 1.25f), 255);
            if (Config.Main.RainbowMode)
            {
                this.settingsMenu?.UpdateRainbow(back);
                Config.Main.BackColor.Set(back);
                Config.Main.BorderColor.Set(border);
                Config.Main.TextColor.Set(Color.Black);
            }

            if (Invalidated && Config.Main.CacheDebugMode)
                canvas.DrawRectangle(this.Bounds, ColorHelper.Fade(Color.Magenta, 0.5f), null, 0, Layer.Fore);
        }
    }
}
