using System;
using System.IO;
using System.Windows.Forms;
using AATool.Configuration;
using AATool.Data.Categories;
using AATool.Data.Objectives;
using AATool.Data.Objectives.Pickups;
using AATool.Graphics;
using AATool.Net;
using AATool.UI.Controls;
using AATool.Utilities;
using AATool.Winforms.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AATool.UI.Screens
{
    public class UIMainScreen : UIScreen
    {
        public static RenderTarget2D RenderCache { get; private set; }
        public static bool Invalidated { get; private set; }
        public static bool RefreshingCache { get; set; }
        public static bool Invalidate() => Invalidated = true;

        private FSettings settingsMenu;
        private UIGrid grid;
        private UILobby lobby;
        private UIStatusBar status;
        private UIBlockPopup popup;
        private UILeaderboard leaderboard;
        private UIPotionGroup potions;
        private UIButton resetDeaths;
        private UIButton toggleEGap;
        private readonly Utilities.Timer settingsCooldown;

        public override Color FrameBackColor() => Config.Main.BackColor;
        public override Color FrameBorderColor() => Config.Main.BorderColor;

        public UIMainScreen(Main main) : base(main, main.Window)
        {
            //set window title
            this.Form.Text = Main.FullTitle;
            this.Form.FormClosing += this.OnClosing;
            this.settingsCooldown = new Utilities.Timer(0.25f);
            //this.Form.Location = new System.Drawing.Point(0, 0);
            
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

        private void OnClosing(object sender, FormClosingEventArgs e)
        {
            if (Peer.IsServer && Peer.TryGetLobby(out Lobby lobby) && lobby.UserCount > 1)
            {
                int clients = lobby.UserCount - 1;
                string caption = "Co-op Shutdown Confirmation";
                string players = clients is 1 ? "1 player" : $"{clients} players";
                string text = $"You are currently hosting a Co-op lobby with {players} connected! Are you sure you want to quit?";
                DialogResult result = MessageBox.Show(this.Form, text, caption,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Exclamation);

                if (result is DialogResult.Yes)
                    Peer.StopInstance();
                else
                    e.Cancel = true;
            }

            if (Config.Main.StartupArrangement == WindowSnap.Remember)
            {
                Config.Main.LastWindowPosition.Set(new Point(this.Form.Location.X, this.Form.Location.Y));
                Config.Main.Save();
            }

            if (Config.Overlay.StartupArrangement == WindowSnap.Remember)
            {
                Config.Overlay.LastWindowPosition.Set(new Point(Main.OverlayScreen.Form.Location.X, Main.OverlayScreen.Form.Location.Y));
                Config.Overlay.Save();
            }
        }

        public override string GetCurrentView()
        {
            //get proper view for current category and version
            string path = Path.Combine(Paths.System.ViewsFolder,
                Tracker.Category.ViewName,
                Tracker.Category.CurrentMajorVersion ?? Tracker.Category.CurrentVersion,
                "main.xml");

            //check for conditional variant if needed
            if (!File.Exists(path))
            {
                path = Path.Combine(Paths.System.ViewsFolder,
                    Tracker.Category.ViewName,
                    Tracker.Category.CurrentMajorVersion ?? Tracker.Category.CurrentVersion,
                    $"main_{Config.Main.ViewMode}.xml");
            }

            //path = Path.Combine(Paths.System.ViewsFolder,
            //    "overview",
            //    "relaxed.xml");
            return path;
        }

        public override void ReloadView()
        {
            Peer.UnbindController(this.status);
            this.Children.Clear();
            if (this.TryLoadXml(this.GetCurrentView()))
            {
                base.InitializeRecursive(this);
                this.ResizeRecursive(this.Bounds);
                this.SetIcon(Tracker.Category.ViewName);
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
            this.leaderboard = this.First<UILeaderboard>();
            this.potions = this.First<UIPotionGroup>();
            this.toggleEGap = this.First<UIButton>("toggle_egap");
            if (this.toggleEGap is not null)
                this.toggleEGap.OnClick += this.Click;

            this.resetDeaths = this.First<UIButton>("reset");
            if (this.resetDeaths is not null)
            {
                this.resetDeaths.TextBlock.SetFont("minecraft", 24);
                this.resetDeaths.OnClick += this.Click;
            }            
            Peer.BindController(this.status);
        }

        private void Click(UIControl sender)
        {
            if (sender == this.resetDeaths)
            {
                ActiveInstance.SetLogStart();
                foreach (Death death in Tracker.Deaths.All.Values)
                    death.Clear();
            }
            else if (sender == this.toggleEGap)
            {
                if (Tracker.TryGetPickup(EGap.ItemId, out Pickup apple))
                    (apple as EGap).ToggleManualCompletion();
            }
        }

        public override void ResizeRecursive(Rectangle rectangle)
        {
            base.ResizeRecursive(rectangle);
            if (this.leaderboard is not null && Tracker.Category is not AllAchievements)
            {
                this.leaderboard.Collapse();
                this.potions?.Collapse();
            }
            this.First(Config.Main.InfoPanel)?.Expand();
        }

        protected override void ConstrainWindow()
        {
            int width = this.grid?.GetExpandedWidth() ?? 640;
            int height = this.grid?.GetExpandedHeight() ?? 320;
            if (this.Width != width || this.Height != height || Tracker.ObjectivesChanged)
            {
                //this.Form.ClientSize = new System.Drawing.Size(width * Config.Main.DisplayScale, height * Config.Main.DisplayScale);
                Main.GraphicsManager.PreferredBackBufferWidth  = width;
                Main.GraphicsManager.PreferredBackBufferHeight = height;
                Main.GraphicsManager.ApplyChanges();
                this.ResizeRecursive(new Rectangle(0, 0, width, height));
                RenderCache?.Dispose();
                RenderCache = new RenderTarget2D(this.GraphicsDevice, width, height);
            }
            this.Form.ClientSize = new System.Drawing.Size(width * Config.Main.DisplayScale, height * Config.Main.DisplayScale);

            //snap window to user's preferred location
            if (!this.Positioned || Config.Main.StartupArrangement.Changed || Config.Main.StartupDisplay.Changed)
            {
                if (this.Positioned && Config.Main.StartupArrangement == WindowSnap.Remember)
                    return;

                this.PositionWindow(Config.Main.StartupArrangement, 
                    Config.Main.StartupDisplay, 
                    Config.Main.LastWindowPosition);
                this.Positioned = true;
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
                this.ReloadView();
            
            this.UpdateCollapsedStates();

            //keep settings menu version up to date
            if (Tracker.ObjectivesChanged || Peer.StateChanged)
                this.settingsMenu?.InvalidateSettings();

            if (Config.Overlay.Width.Changed)
                this.settingsMenu?.UpdateOverlayWidth();

            if (Config.Main.StyleChanged)
                Invalidate();

            //escape to open settings
            this.settingsCooldown.Update(time);
            if (Input.Started(Microsoft.Xna.Framework.Input.Keys.Escape) && this.settingsCooldown.IsExpired)
                this.OpenSettingsMenu();

            this.UpdateEGapCheckbox();

            //enforce window size
            this.ConstrainWindow();
        }

        private void UpdateEGapCheckbox()
        {
            if (this.toggleEGap is null || !Tracker.TryGetPickup(EGap.ItemId, out Pickup pickup))
                return;

            //update the manual override god apple checkbox
            var eGap = pickup as EGap;
            if (Tracker.IsWorking && !eGap.Eaten)
            {
                this.toggleEGap.Expand();
                string variant = Config.Main.FrameStyle == "Modern" ? "modern" : "basic";
                variant = eGap.IsComplete() ? $"checked_{variant}" : $"unchecked_{variant}";
                this.toggleEGap.First<UIPicture>()?.SetTexture(variant);
            }
            else
            {
                this.toggleEGap.Collapse();
            }
        }

        private void ShowErrorScreen() 
        {
            Peer.UnbindController(this.status);
            this.Children.Clear();

            var label = new UITextBlock();
            label.SetFont("minecraft", 24);
            label.SetText($"There was an error attempting to load layout file:\n{this.GetCurrentView()}");
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

            //update which info panel to show
            if (Config.Main.InfoPanel.Changed && this.leaderboard is not null)
            {
                if (Tracker.Category is not AllAchievements)
                {
                    this.leaderboard?.Collapse();
                    this.potions?.Collapse();
                    this.First(Config.Main.InfoPanel)?.Expand();
                }
                else
                {
                    this.leaderboard?.Expand();
                }
            }

            //update whether or not top row should be shown
            if (Tracker.Category.GetType() == typeof(AllAdvancements) && Config.Main.ShowBasicAdvancements == this.grid.CollapsedRows[0])
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

        public override void Prepare()
        {
            base.Prepare();
            this.GraphicsDevice.Clear(Tracker.Category is AllBlocks ? Config.Main.BorderColor : Config.Main.BackColor);
        }

        public override void Present() 
        { 
            base.Present();
            Invalidated = false;
            RefreshingCache = false;
        }

        public override void DrawThis(Canvas canvas)
        {
            Color back = Canvas.RainbowLight;
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
