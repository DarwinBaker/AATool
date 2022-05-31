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

        private UIButton tabAAMultiboard;
        private UIButton tabABMultiboard;
        private UIButton tabRunners;
        private bool needsLayoutRefresh;
        private readonly Utilities.Timer settingsCooldown;

        public override Color FrameBackColor() => Config.Main.BackColor;
        public override Color FrameBorderColor() => Config.Main.BorderColor;

        public UIMainScreen(Main main) : base(main, main.Window)
        {
            //set window title
            this.Form.Text = Main.FullTitle;
            this.Form.FormClosing += this.OnClosing;
            this.settingsCooldown = new Utilities.Timer(0.25f);
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

            //remember main window position
            if (Config.Main.StartupArrangement == WindowSnap.Remember)
            {
                Config.Main.LastWindowPosition.Set(new Point(this.Form.Location.X, this.Form.Location.Y));
                Config.Main.Save();
            }

            //remember last main player
            if (Player.TryGetName(Tracker.GetMainPlayer(), out string name))
            {
                Config.Tracking.LastPlayer.Set(name);
                Config.Tracking.Save();
            }
        }

        public override string GetCurrentView()
        {
            //fullscreen multi-version leaderboards
            if (Config.Main.ActiveTab == "multiboard_aa")
            {
                //return Path.Combine(Paths.System.ViewsFolder,
                //    "leaderboards",
                //    $"multiboard_aa.xml");
            }
            else if (Config.Main.ActiveTab == "multiboard_ab")
            {
                //return Path.Combine(Paths.System.ViewsFolder,
                //    "leaderboards",
                //    $"multiboard_ab.xml");
            }
            else if (Config.Main.ActiveTab == "timeline")
            {
                //return Path.Combine(Paths.System.ViewsFolder,
                //    "leaderboards",
                //    $"timeline.xml");
            }
            //else if (Config.Main.ActiveTab == "runners_1.16")
            //{
                //return Path.Combine(Paths.System.ViewsFolder,
                //    "leaderboards",
                //    $"runners_1.16.xml");
            //}

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
                    $"main_{Config.Main.Layout.Value}.xml");
            }
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
            this.needsLayoutRefresh = false;
        }

        public override void InitializeThis(UIScreen screen)
        {
            this.grid   = this.First<UIGrid>();
            this.lobby  = this.First<UILobby>();
            this.status = this.First<UIStatusBar>();
            this.popup = this.First<UIBlockPopup>();
            this.leaderboard = this.First<UILeaderboard>("Leaderboard");
            this.potions = this.First<UIPotionGroup>();
            this.tabAAMultiboard = this.First<UIButton>("tab_multiboard_aa");
            if (this.tabAAMultiboard is not null)
                this.tabAAMultiboard.OnClick += this.Click;
            this.tabABMultiboard = this.First<UIButton>("tab_multiboard_ab");
            if (this.tabABMultiboard is not null)
                this.tabABMultiboard.OnClick += this.Click;
            this.tabRunners = this.First<UIButton>("tab_runners");
            if (this.tabRunners is not null)
                this.tabRunners.OnClick += this.Click;

            this.resetDeaths = this.First<UIButton>("reset");
            if (this.resetDeaths is not null)
            {
                this.resetDeaths.TextBlock.SetFont("minecraft", 24);
                this.resetDeaths.OnClick += this.Click;
            }            
            Peer.BindController(this.status);
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

        public override void UpdateRecursive(Time time)
        {
            base.UpdateRecursive(time);
            this.popup?.Finalize(time);
        }

        protected override void UpdateThis(Time time)
        {
            //update game version
            if (Tracker.ObjectivesChanged || Config.Main.Layout.Changed || Config.Main.ActiveTab.Changed || this.needsLayoutRefresh)
                this.ReloadView();
            
            this.UpdateCollapsedStates();

            //keep settings menu version up to date
            if (Tracker.ObjectivesChanged || Peer.StateChanged)
                this.settingsMenu?.InvalidateSettings();

            if (Config.Overlay.Width.Changed)
                this.settingsMenu?.UpdateOverlayWidth();

            if (Config.Main.AppearanceChanged)
                Invalidate();

            //escape to open settings
            this.settingsCooldown.Update(time);
            if (Input.Started(Microsoft.Xna.Framework.Input.Keys.Escape) && this.settingsCooldown.IsExpired)
                this.OpenSettingsMenu();

            //enforce window size
            this.ConstrainWindow();
        }

        protected override void ConstrainWindow()
        {
            int width = this.grid?.GetExpandedWidth() ?? 1200;
            int height = this.grid?.GetExpandedHeight() ?? 600;
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

        public void Click(UIControl sender)
        {
            if (sender == this.resetDeaths)
            {
                ActiveInstance.SetLogStart();
                foreach (Death death in Tracker.Deaths.All.Values)
                    death.Clear();
            }
            else if (sender.Name is "manual_check")
            {
                string id = sender.Tag?.ToString() ?? "";
                if (Tracker.TryGetPickup(id, out Pickup pickup))
                    pickup.ToggleManualCheck();
                else if (Tracker.TryGetBlock(id, out Block block))
                    block.ToggleManualCheck();
                else if (Tracker.TryGetDeath(id, out Death death))
                    death.ToggleManualCheck();
            }
            else if (sender == this.tabAAMultiboard)
            {
                Config.Main.ActiveTab.Set("multiboard_aa");
                Config.Main.Save();
                this.needsLayoutRefresh = true;
            }
            else if (sender == this.tabABMultiboard)
            {
                Config.Main.ActiveTab.Set("multiboard_ab");
                Config.Main.Save();
                this.needsLayoutRefresh = true;
            }
            else if (sender == this.tabRunners)
            {
                Config.Main.ActiveTab.Set("runners_1.16");
                Config.Main.Save();
                this.needsLayoutRefresh = true;
            }
        }

        private void ShowErrorScreen()
        {
            Peer.UnbindController(this.status);
            this.Children.Clear();

            var label = new UITextBlock() {
                VerticalTextAlign = VerticalAlign.Top,
                Padding = new Margin(0, 0, 200, 0),
            };
            label.SetFont("minecraft", 36);

            if (Config.Main.UseVerticalStyling 
                && (Tracker.Category.GetType() != typeof(AllAdvancements) || Tracker.Category.CurrentMajorVersion is not "1.16"))
                label.SetText($"The current vertical layout is still under development :)");
            else
                label.SetText($"There was an error attempting to load layout file:\n{this.GetCurrentView().Replace("\\", "/")}");
            this.AddControl(label);

            var settings = new UIButton() {
                FlexWidth = new Size(200),
                FlexHeight = new Size(75),
                VerticalAlign = VerticalAlign.Top,
                Margin = new Margin(0, 0, 300, 0),
                BorderThickness = 4,
            };
            settings.OnClick += this.OnSettingsClick;
            settings.TextBlock.SetFont("minecraft", 36);
            settings.SetText("Settings");
            this.AddControl(settings);

            this.InitializeRecursive(this);
            this.ResizeRecursive(this.Bounds);
        }

        private void OnSettingsClick(UIControl sender)
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
