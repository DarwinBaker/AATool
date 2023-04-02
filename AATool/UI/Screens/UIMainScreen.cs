using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AATool.Configuration;
using AATool.Data.Categories;
using AATool.Data.Objectives;
using AATool.Data.Objectives.Complex;
using AATool.Graphics;
using AATool.Net;
using AATool.Net.Requests;
using AATool.UI.Badges;
using AATool.UI.Controls;
using AATool.Utilities;
using AATool.Winforms.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AATool.UI.Screens
{
    public class UIMainScreen : UIScreen
    {
        public const string TrackerTab = "tracker";
        public const string HelpTab = "help";
        public const string MultiboardTab = "multiboard";
        public const string RunnersTab = "runners_1.16";

        public static string ActiveTab { get; private set; } = TrackerTab;
        public static RenderTarget2D RenderCache { get; private set; }
        public static bool Invalidated { get; private set; }
        public static bool RefreshingCache { get; set; }
        public static bool NeedsLayoutRefresh { get; private set; }

        public static bool Invalidate() => Invalidated = true;

        public static void ForceLayoutRefresh() => NeedsLayoutRefresh = true;

        public static FSettings Settings;
        public static bool SettingsJustClosed => SettingsCooldown.IsRunning;

        private static readonly Utilities.Timer SettingsCooldown = new (0.25f);

        private UIGrid grid;
        private UILobby lobby;
        private UIStatusBar status;
        private UILeaderboard leaderboard; 
        private UIRunOverview overview;
        private UIPotionGroup potions;
        private UITextBlock debugLog;
        private UIBlockGrid blockGrid;

        private UIControl complexOverworld;
        private UIControl complexNether;

        private readonly List<UIPicture> labelTintedIcons = new ();

        private int logOffset;
        private int logLines;
        private int pendingRequests;
        private int activeRequests;
        private int completedRequests;
        private int timedOutRequests;

        public bool DimScreen => this.blockGrid?.DimScreen is true;
        public override Color FrameBackColor() => Config.Main.BackColor;
        public override Color FrameBorderColor() => Config.Main.BorderColor;

        public UIMainScreen(Main main) : base(main, main.Window)
        {
            //set window title
            this.Form.Text = Main.FullTitle;
            this.Form.FormClosing += this.OnClosing;
            this.Form.TopMost = Config.Main.AlwaysOnTop;
        }

        public void RegisterLabelTint(UIPicture control)
        {
            if (!this.labelTintedIcons.Contains(control))
                this.labelTintedIcons.Add(control);
        }

        public void CloseSettingsMenu()
        {
            //prevent settings from re-opening immediately if escape is used to close the window
            SettingsCooldown.Reset();
        }

        public void OpenSettingsMenu()
        {
            if (Settings is null || Settings.IsDisposed)
            {
                Settings = new FSettings();
                Settings.Show(this.Form);
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
                Config.Main.TrySave();
            }

            Uuid mainPlayer = Tracker.GetMainPlayer();
            if (mainPlayer != Uuid.Empty)
            {
                Config.Tracking.LastUuid.Set(mainPlayer);

                //remember last main player
                if (Player.TryGetName(mainPlayer, out string name))
                {
                    Config.Tracking.LastPlayer.Set(name);
                    Config.Tracking.TrySave();
                }
            }
        }

        public override string GetCurrentView()
        {
            string view = Tracker.Category.ViewName;
            string version = Tracker.Category.CurrentMajorVersion ?? Tracker.Category.CurrentVersion;

            //return Path.Combine(Paths.System.ViewsFolder, "other", "primary_version.xml");

            if (ActiveTab is HelpTab)
                return Path.Combine(Paths.System.ViewsFolder, view, version, $"help.xml");

            if (ActiveTab is not TrackerTab)
                return Path.Combine(Paths.System.ViewsFolder, "other", $"{ActiveTab}.xml");

            string path = Path.Combine(Paths.System.ViewsFolder, view, version, "main.xml");

            string layout = string.IsNullOrEmpty(Config.Main.Layout.Value) ? Config.Main.Layout.Default : Config.Main.Layout.Value;
            //check for conditional layout variant if needed
            if (!File.Exists(path))
                path = Path.Combine(Paths.System.ViewsFolder, view, version, $"main_{layout}.xml");

            if (!File.Exists(path) && Config.Main.Layout == Config.OptimizedLayout)
                path = Path.Combine(Paths.System.ViewsFolder, view, version, $"main_relaxed.xml");

            return path;
        }

        public override void ReloadView()
        {
            Peer.UnbindController(this.status);

            this.Children.Clear();
            this.labelTintedIcons.Clear();
            if (this.TryLoadXml(this.GetCurrentView()))
            {
                base.InitializeRecursive(this);
                this.ResizeRecursive(this.Bounds);
                this.SetIcon(Tracker.Category.ViewName);
                foreach (UIPicture picture in this.labelTintedIcons)
                    picture.SetTint(Config.Main.TextColor);
                //this.Positioned = false;
            }
            else
            {
                this.ShowErrorScreen();
            }
            NeedsLayoutRefresh = false;
        }

        public override void InitializeThis(UIScreen screen)
        {
            this.TryGetFirst(out this.grid);
            this.TryGetFirst(out this.lobby);
            this.TryGetFirst(out this.status);
            this.TryGetFirst(out this.leaderboard, "Leaderboard");
            this.TryGetFirst(out this.overview);
            this.TryGetFirst(out this.potions);
            this.TryGetFirst(out this.debugLog, "debug_log");
            this.TryGetFirst(out this.blockGrid);
            this.TryGetFirst(out this.complexOverworld, "complex_overworld");
            this.TryGetFirst(out this.complexNether, "complex_nether");

            this.logLines = -1;

            if (Tracker.Category is AllBlocks)
                this.AddControl(new UIBlockMessage());

            Peer.BindController(this.status);
        }

        public override void ResizeRecursive(Rectangle rectangle)
        {
            base.ResizeRecursive(rectangle);
            this.UpdateCollapsedStates(true);

            if (ActiveTab is MultiboardTab)
            {
                //this.First<UIPicture>("main_player_avatar")?.SetTexture(Tracker.GetMainPlayer().ToString().Replace("-", ""));
                this.Root().First<UIAvatar>("100hc_avatar")?.SetBadge(new HundredHardcoreBadge());
            }
        }

        private void UpdateShortcuts()
        {
            //escape to open settings
            if (Input.Started(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                if (this.blockGrid?.IsSearching is true)
                    return;

                if (this.HasFocus && SettingsCooldown.IsExpired)
                    this.OpenSettingsMenu();
            }
        }

        public override void UpdateRecursive(Time time)
        {
            base.UpdateRecursive(time);
            //this needs to be done after everything else
            this.blockGrid?.Finalize(time);
        }

        protected override void UpdateThis(Time time)
        {
            if (Tracker.ObjectivesChanged)
                ActiveTab = TrackerTab;
            SettingsCooldown.Update(time);

            //update game version
            bool reload = Tracker.ObjectivesChanged || Config.Main.Layout.Changed || NeedsLayoutRefresh;
            if (reload)
                this.ReloadView();

            this.UpdateCollapsedStates(reload);
            this.UpdateShortcuts();

            //keep settings menu version up to date
            if (Tracker.ObjectivesChanged || Peer.StateChanged)
                Settings?.InvalidateSettings();

            if (Config.Overlay.Width.Changed)
                Settings?.UpdateOverlayWidth();

            if (Config.Main.AlwaysOnTop.Changed)
                this.Form.TopMost = Config.Main.AlwaysOnTop;

            _= Tracker.GetMainPlayer();
            if (Tracker.MainPlayerChanged)
            {
                Settings?.UpdateBadgeList();
                Settings?.UpdateFrameList();
            }

            Settings?.UpdateNotesState();

            if (Config.Main.AppearanceChanged)
            {
                foreach (UIPicture icon in this.labelTintedIcons)
                    icon.SetTint(Config.Main.TextColor);
                Invalidate();
            }
            this.UpdateLog();

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

        public override void Click(UIControl sender)
        {
            if (sender.Name == "show_settings")
            {
                this.OpenSettingsMenu();
            }
            else if (sender.Name == "show_all_blocks_welcome")
            {
                this.First<UIBlockMessage>()?.Show();
            }
            else if (sender.Name == "clear_deaths")
            {
                ActiveInstance.SetLogStart();
                foreach (Death death in Tracker.Deaths.All.Values)
                    death.Clear();
            }
            else if (sender.Name == "clear_deaths")
            {
                ActiveInstance.SetLogStart();
                foreach (Death death in Tracker.Deaths.All.Values)
                    death.Clear();
            }
            else if (sender.Name == "clear_deaths")
            {
                ActiveInstance.SetLogStart();
                foreach (Death death in Tracker.Deaths.All.Values)
                    death.Clear();
            }
            else if (sender.Name is "manual_check")
            {
                string id = sender.Tag?.ToString() ?? string.Empty;
                if (Tracker.TryGetComplexObjective(id, out ComplexObjective pickup))
                    pickup.ToggleManualCheck();
                else if (Tracker.TryGetBlock(id, out Block block))
                    block.ToggleManualCheck();
                else if (Tracker.TryGetDeath(id, out Death death))
                    death.ToggleManualCheck();
            }
            else if (sender.Name.StartsWith("show_"))
            {
                ActiveTab = sender.Name.Replace("show_", "");
                if (ActiveTab == MultiboardTab)
                {
                    new AnyPercentRecordRequest(true).EnqueueOnce();
                    new AnyPercentRecordRequest(false).EnqueueOnce();
                }
                ForceLayoutRefresh();
            }
            else if (sender.Name.StartsWith("https://"))
            {
                Process.Start(sender.Name);
            }
        }

        private void UpdateLog()
        {
            if (this.debugLog is null)
                return;

            var builder = new StringBuilder();
            string log = Debug.GetLog(Debug.RequestSection);
            if (string.IsNullOrEmpty(log))
                return;

            string[] lines = Debug.GetLog(Debug.RequestSection).Split('\n');
            int oldOffset = this.logOffset;

            //scroll the log
            if (this.debugLog.Bounds.Contains(Input.Cursor(this)))
            {
                if (Input.ScrolledUp())
                {
                    this.logOffset = MathHelper.Max(this.logOffset - 3, -(lines.Length - 15));
                }
                else if (Input.ScrolledDown())
                {
                    this.logOffset = MathHelper.Min(this.logOffset + 3, 0);
                }
            }
            if (this.logLines != lines.Length)
                this.logOffset = 0;

            bool invalidated = this.logLines != lines.Length
                || this.logOffset != oldOffset
                || NetRequest.CompletedCount != this.completedRequests
                || NetRequest.PendingCount != this.pendingRequests
                || NetRequest.TimedOutCount != this.timedOutRequests
                || NetRequest.ActiveCount != this.activeRequests;

            //update log text if lines changed
            if (invalidated)
            {
                this.completedRequests = NetRequest.CompletedCount;
                this.pendingRequests = NetRequest.PendingCount;
                this.timedOutRequests = NetRequest.TimedOutCount;
                this.activeRequests = NetRequest.ActiveCount;

                this.logLines = lines.Length;
                for (int i = Math.Max(lines.Length - 15 + this.logOffset, 0); i < lines.Length + this.logOffset; i++)
                    builder.AppendLine(lines[i]);
                this.debugLog.SetText(builder.ToString().Trim());
                if (this.logOffset is 0)
                {
                    string info = new ('-', 55);
                    info += $" Completed: {NetRequest.CompletedCount}, Pending: {NetRequest.PendingCount},";
                    info += $" Names: x{NameRequest.Downloads}, UUIDs: x{UuidRequest.Downloads}, Avatars: x{AvatarRequest.Downloads}";
                    this.debugLog.Append(Environment.NewLine + info);
                }
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
            {
                label.SetText($"The current vertical layout is still under development :)");
            }
            else
            { 
                label.SetText($"There was an error attempting to load layout file:\n{this.GetCurrentView().Replace("\\", "/")}");
            }
            this.AddControl(label);

            var settings = new UIButton() {
                Name = "show_settings",
                FlexWidth = new Size(200),
                FlexHeight = new Size(75),
                VerticalAlign = VerticalAlign.Top,
                Margin = new Margin(0, 0, 300, 0),
                BorderThickness = 4,
            };
            settings.OnClick += this.Click;
            settings.TextBlock.SetFont("minecraft", 36);
            settings.SetText("Settings");
            this.AddControl(settings);

            this.InitializeRecursive(this);
            this.ResizeRecursive(this.Bounds);
        }

        private void UpdateCollapsedStates(bool forceInfoPanelRefresh)
        {

            if (this.grid is null)
                return;

            //update which info panel to show
            bool infoPanelInvalidated = Config.Main.InfoPanel.Changed
                || Config.Main.Layout.Changed || Tracker.Invalidated
                || forceInfoPanelRefresh;

            if (infoPanelInvalidated)
            {
                if (Tracker.Category is not AllAchievements)
                {
                    this.leaderboard?.Collapse();
                    this.potions?.Collapse();
                    this.overview?.Collapse();

                    string panel = this.GetActiveInfoPanel();
                    this.complexOverworld?.SetVisibility(panel is not Config.RunOverviewPanel);
                    this.complexNether?.SetVisibility(panel is not Config.RunOverviewPanel);
                    this.First(panel)?.Expand();
                }
                else
                {
                    this.leaderboard?.Expand();
                }
            }

            //update whether or not top row should be shown
            if (Tracker.Category is AllAdvancements && Config.Main.ShowBasicAdvancements == this.grid.CollapsedRows[0])
            {
                if (Config.Main.ShowBasicAdvancements || Config.Main.UseOptimizedLayout)
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

        private string GetActiveInfoPanel()
        {
            string setting = Config.Main.InfoPanel;
            if (string.IsNullOrWhiteSpace(setting))
                return Config.LeaderboardPanel;

            if (setting is Config.AutoSwitchPanel)
                return Tracker.IsWorking ? Config.RunOverviewPanel : Config.LeaderboardPanel;
            return setting;
        }

        public override void Prepare()
        {
            base.Prepare();
            this.GraphicsDevice.Clear(Config.Main.BackColor);
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
                Settings?.UpdateRainbow(back);
                Config.Main.BackColor.Set(back);
                Config.Main.BorderColor.Set(border);
                Config.Main.TextColor.Set(Color.Black);
            }

            if (Invalidated && Config.Main.CacheDebugMode)
                canvas.DrawRectangle(this.Bounds, ColorHelper.Fade(Color.Magenta, 0.5f), null, 0, Layer.Fore);
        }
    }
}
