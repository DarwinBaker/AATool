using AATool.Configuration;
using AATool.Data.Categories;
using AATool.Data.Objectives;
using AATool.Graphics;
using AATool.Net;
using AATool.Saves;
using AATool.UI.Controls;
using AATool.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace AATool.UI.Screens
{
    public sealed class UIOverlayScreen : UIScreen
    {
        private const int SCROLL_SPEED_MULTIPLIER = 15;
        private const int BASE_SCROLL_SPEED = 30;
        private const int TITLE_MOVE_SPEED = 4;

        private readonly SequenceTimer titleTimer;
        
        private UITextBlock progress;
        private UITextBlock text;
        private UITextBlock status;
        private UICarousel advancements;
        private UICarousel criteria;
        private UIFlowPanel counts;
        private UIControl runCompletePanel;
        private UIControl carouselPanel;
        private bool isResizing;

        private float titleY;

        public bool FastForwarding { get; set; }

        private Color frameBack;
        private Color frameBorder;

        public override Color FrameBackColor() => this.frameBack;
        public override Color FrameBorderColor() => this.frameBorder;

        public UIOverlayScreen(Main main) : base(main, GameWindow.Create(main, 360, 360))
        {
            this.Form.Text         = "Stream Overlay";
            this.Form.ControlBox   = false;
            this.Form.ResizeBegin += this.OnResizeBegin;
            this.Form.ResizeEnd   += this.OnResizeEnd;
            this.Form.Resize      += this.OnResize;
            this.Form.FormClosing += this.OnClosing;
            this.Window.AllowUserResizing = true;
            this.titleTimer = new SequenceTimer(60, 1, 10, 1, 10, 1, 10, 1);
            
            this.ReloadView();
            this.Form.MinimumSize = new System.Drawing.Size(1260 + this.Form.Width - this.Form.ClientSize.Width, 
                this.Height + this.Form.Height - this.Form.ClientSize.Height);

            this.Form.MaximumSize = new System.Drawing.Size(4096 + this.Form.Width - this.Form.ClientSize.Width, 
                this.Height + this.Form.Height - this.Form.ClientSize.Height);
        }

        private void OnClosing(object sender, FormClosingEventArgs e)
        {
            if (Config.Overlay.StartupArrangement == WindowSnap.Remember)
            {
                Config.Overlay.LastWindowPosition.Set(new Point(this.Form.Location.X, this.Form.Location.Y));
                Config.Overlay.Save();
            }
        }

        public override string GetCurrentView()
        {
            return Path.Combine(Paths.System.ViewsFolder,
                Tracker.Category.ViewName,
                "overlay.xml");
        }

        public override void ReloadView()
        {
            this.ClearControls();
            if (!this.TryLoadXml(this.GetCurrentView()))
                Main.QuitBecause("Error loading overlay layout!");

            this.InitializeRecursive(this);
            this.ResizeRecursive(new Rectangle(0, 0, Config.Overlay.Width, this.Height));
        }

        protected override void ConstrainWindow()
        {
            int width  = Config.Overlay.Width;
            int height = this.Height;

            //enforce minimum size
            if (this.Form.MinimumSize.Width > 0)
                width  = Math.Max(width, this.Form.MinimumSize.Width - (this.Form.Width - this.Form.ClientSize.Width));
            if (this.Form.MinimumSize.Height > 0)
                height = Math.Max(height, this.Form.MinimumSize.Height - (this.Form.Height - this.Form.ClientSize.Height));

            if (width is 0 || height is 0)
                return;

            //resize window and create new render target of proper size
            if (this.Target is null || this.Target.Width != width || this.Target.Height != height)
            {
                this.Form.ClientSize = new System.Drawing.Size(width, height);
                this.Target?.Dispose();
                this.Target = new SwapChainRenderTarget(this.GraphicsDevice, this.Window.Handle, width, height);
                this.ResizeRecursive(new Rectangle(0, 0, width, height));
                this.UpdateCarouselLocations();
            }

            //snap window to user's preferred location
            if (!this.Positioned || Config.Overlay.StartupArrangement.Changed || Config.Overlay.StartupDisplay.Changed)
            {
                if (this.Positioned && Config.Overlay.StartupArrangement == WindowSnap.Remember)
                    return;

                this.PositionWindow(Config.Overlay.StartupArrangement,
                    Config.Overlay.StartupDisplay, 
                    Config.Overlay.LastWindowPosition);
                this.Positioned = true;
            }
        }

        public override void InitializeThis(UIScreen screen)
        {
            this.runCompletePanel = this.First("panel_congrats");
            this.carouselPanel = this.First("panel_carousel");

            int textScale = 24;

            this.progress = new UITextBlock("minecraft", textScale) {
                Margin              = new Margin(16, 0, 8, 0),
                HorizontalTextAlign = HorizontalAlign.Left,
                VerticalAlign       = VerticalAlign.Top
            };
            this.AddControl(this.progress);

            this.text = new UITextBlock("minecraft", textScale) {
                Padding             = new Margin(16, 16, 8, 0),
                HorizontalTextAlign = HorizontalAlign.Center,
                VerticalTextAlign   = VerticalAlign.Top
            };
            this.AddControl(this.text);

            //initialize main objective carousel
            this.advancements = this.First<UICarousel>("advancements");
            if (this.advancements is not null)
            {
                this.advancements.SetScrollDirection(Config.Overlay.RightToLeft);
            }

            //initialize criteria carousel
            this.criteria = this.First<UICarousel>("criteria");
            if (this.criteria is not null)
            {
                this.criteria.SetScrollDirection(Config.Overlay.RightToLeft);
            }

            //initialize item counters
            this.counts = this.First<UIFlowPanel>("counts");
            if (this.counts is not null)
            {
                this.counts.FlowDirection = Config.Overlay.RightToLeft
                    ? FlowDirection.RightToLeft
                    : FlowDirection.LeftToRight;

                if (Tracker.Category is not (AllBlocks or AllDeaths))
                {
                    //add pickup counters
                    foreach (Pickup pickup in Tracker.Pickups.All.Values.Reverse())
                        this.counts.AddControl(new UIObjectiveFrame(pickup, 3));
                }

                //status label
                this.status = new UITextBlock("minecraft", 24) {
	                FlexWidth = new Size(180),
	                FlexHeight = new Size(72),
	                VerticalTextAlign = VerticalAlign.Center
	            };
	            this.status.HorizontalTextAlign = Config.Overlay.RightToLeft
	                ? HorizontalAlign.Right
	                : HorizontalAlign.Left;
	
	            this.counts.AddControl(this.status);
            }

            this.UpdateSpeed();
        }

        private void OnResizeBegin(object sender, EventArgs e)
        {
            this.isResizing = true;
            this.advancements?.Break();
            this.criteria?.Break();
        }

        private void OnResize(object sender, EventArgs e)
        {
            if (this.Form.WindowState is FormWindowState.Minimized)
                this.Form.WindowState = FormWindowState.Normal;

            if (this.isResizing)
                Config.Overlay.Width.Set(this.Form.ClientSize.Width);
        }

        private void OnResizeEnd(object sender, EventArgs e)
        {
            this.isResizing = false;
            this.advancements?.Continue();
            this.criteria?.Continue();
            Config.Overlay.Width.Set(this.Form.ClientSize.Width);
            Config.Overlay.Save();
        }

        protected override void UpdateThis(Time time)
        {
            //update enabled state
            if (!this.Form.IsDisposed)
                this.Form.Visible = Config.Overlay.Enabled;
            if (!this.Form.Visible)
                return;

            if (Tracker.ObjectivesChanged || Config.Overlay.Enabled.Changed)
                this.ReloadView();

            this.ConstrainWindow();
            this.UpdateVisibility();
            this.UpdateCarouselLocations();

            if (Config.Overlay.Enabled.Changed || Config.Overlay.AppearanceChanged)
            {
                if (Config.Overlay.FrameStyle == "Custom Theme")
                {
                    this.frameBack = Config.Overlay.CustomBackColor;
                    this.frameBorder = Config.Overlay.CustomBorderColor;
                }
                else if (Config.MainConfig.Themes.TryGetValue(Config.Overlay.FrameStyle, out var theme))
                {
                    this.frameBack = theme.back;
                    this.frameBorder = theme.border;
                }
            }

            //reset carousels if settings change
            if (Config.Overlay.RightToLeft.Changed)
            {
                this.advancements?.SetScrollDirection(Config.Overlay.RightToLeft);
                this.criteria?.SetScrollDirection(Config.Overlay.RightToLeft);
                if (this.status is not null)
                {
                    this.status.HorizontalTextAlign = Config.Overlay.RightToLeft
                        ? HorizontalAlign.Right
                        : HorizontalAlign.Left;
                }
            }

            this.UpdateSpeed();

            if (Tracker.Category.IsComplete())
                this.text.Collapse();
            else
                this.text.Expand();
            
            if (this.counts is not null)
            {
                this.counts.FlowDirection = Config.Overlay.RightToLeft ^ Config.Overlay.PickupsOpposite
                    ? FlowDirection.RightToLeft
                    : FlowDirection.LeftToRight;
            }

            if (Client.TryGet(out Client client))
            {
                this.status.SetFont("minecraft", 24);
                int seconds = (int)Math.Ceiling((client.NextRefresh - DateTime.UtcNow).TotalSeconds);
                if (seconds <= 0)
                {
                    string hostName = "host";
                    if (Peer.TryGetLobby(out Lobby lobby) && lobby.TryGetHost(out User host))
                    {
                        Player.TryGetName(host.Id, out string name);
                            hostName = name;
                    }
                    this.status?.SetText($"Syncing with {hostName}");
                }
                else
                {
                    this.status?.SetText($"Refreshing in {MinecraftServer.GetEstimateString(seconds).Replace(" ", "\0")}");
                }
            }
            else if (MinecraftServer.IsEnabled)
            {
                this.status?.SetFont("minecraft", 24);
                if (MinecraftServer.State is SyncState.Ready)
                {
                    if (MinecraftServer.CredentialsValidated)
                        this.status?.SetText($"Refreshing in {MinecraftServer.GetEstimateString(MinecraftServer.GetNextRefresh()).Replace(" ", "\0")}");
                    else
                        this.status?.SetText($"SFTP Offline");
                }                
                else if (MinecraftServer.State is SyncState.Connecting)
                {
                    this.status?.SetText($"Connecting...");
                }
                else
                {
                    this.status?.SetText($"Syncing...");
                }
            }
            else
            {
                this.status?.SetFont("minecraft", 48);
                if (Config.Overlay.ShowIgt && Tracker.InGameTime != default)
                    this.status?.SetText(Tracker.GetPrettyIgt());
                else
                    this.status?.SetText(string.Empty);
            }

            this.titleTimer.Update(time);
            if (this.titleTimer.IsExpired)
            {
                if (this.titleTimer.Index is 3)
                    this.text.SetText(Main.ShortTitle);
                else if (this.titleTimer.Index is 5)
                    this.text.SetText(Paths.Web.PatreonShort);

                this.titleTimer.Continue();
            }

            if (this.titleTimer.Index is 0)
            {
                this.text.SetText($"{Tracker.Category.GetCompletedCount()} / {Tracker.Category.GetTargetCount()}");
                this.text.Append($" {Tracker.Category.Objective} {Tracker.Category.Action}");
            }
            else if (this.titleTimer.Index is 2)
            {
                this.text.SetText($"Minecraft JE: {Tracker.Category.Name} ({Tracker.Category.CurrentVersion})");
            }

            //this.text.HorizontalTextAlign = Config.Overlay.RightToLeft
            //    ? HorizontalAlign.Left
            //    : HorizontalAlign.Right;

            if (this.titleTimer.Index % 2 is 0)
            {
                //slide in from top
                this.titleY = MathHelper.Lerp(this.titleY, 0, (float)(TITLE_MOVE_SPEED * time.Delta));
            }
            else
            {
                //slide out from top
                this.titleY = MathHelper.Lerp(this.titleY, -48, (float)(TITLE_MOVE_SPEED * time.Delta));
            }
            this.text.MoveTo(new Point(0, (int)this.titleY));
        }

        private void UpdateSpeed()
        {
            double speed = BASE_SCROLL_SPEED + (Config.Overlay.Speed * SCROLL_SPEED_MULTIPLIER);
            if (this.FastForwarding)
                speed *= 20;
            this.advancements?.SetSpeed(speed);
            this.criteria?.SetSpeed(speed);
        }

        private void UpdateCarouselLocations()
        {
            //update vertical position of rows based on what is or isn't enabled
            int title = 42;

            int criteria = this.criteria is null || this.criteria.IsCollapsed 
                ? 0 : 64;

            int advancement = this.advancements is null || this.advancements.IsCollapsed 
                ? 0 : Config.Overlay.ShowLabels ? 160 : 110;

            int count = this.counts is null || this.counts.IsCollapsed 
                ? 0 : 128;

            this.criteria?.MoveTo(new Point(0, title));
            this.advancements?.MoveTo(new Point(0, title + criteria));
            this.counts?.MoveTo(new Point(this.counts.X, title + criteria + advancement));
        }

        private void UpdateVisibility()
        {
            //handle completion message
            if (Tracker.Category.IsComplete())
            {
                if (this.runCompletePanel?.IsCollapsed is true)
                {
                    this.runCompletePanel?.Expand();
                    this.runCompletePanel?.First<UIRunComplete>()?.Show();
                }         
                this.carouselPanel?.Collapse();  
            }
            else
            {
                this.runCompletePanel?.Collapse();
                this.carouselPanel?.Expand();
            }

            //update criteria visibility
            if (Config.Overlay.ShowCriteria)
                this.criteria?.Expand();
            else
                this.criteria?.Collapse();

            if (this.counts is not null)
            {
                //update item count visibility
                if (Config.Overlay.ShowPickups)
                    this.counts.Expand();
                else
                    this.counts.Collapse();

                //update visibility for individual item counters (favorites)
                foreach (UIControl control in this.counts.Children)
                {
                    if (control.IsCollapsed == Config.Overlay.ShowPickups)
                    {
                        if (Config.Overlay.ShowPickups)
                            control.Expand();
                        else
                            control.Collapse();
                        this.counts.ReflowChildren();
                    }
                }
            }
        }

        public override void Prepare()
        {
            base.Prepare();
            this.GraphicsDevice.Clear(Config.Overlay.GreenScreen);
        }
    }
}
