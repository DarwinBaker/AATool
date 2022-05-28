using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using AATool.Configuration;
using AATool.Data.Categories;
using AATool.Data.Objectives;
using AATool.Data.Objectives.Pickups;
using AATool.Net;
using AATool.Net.Requests;
using AATool.Saves;
using AATool.UI.Controls;
using AATool.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AATool.UI.Screens
{
    public sealed class UIOverlayScreen : UIScreen
    {
        private const int ScrollSpeedMultiplier = 15;
        private const int BaseScrollSpeed = 30;
        private const int HeaderSlideSpeed = 4;
        private const int HeaderHeight = 42;

        private const int HeaderProgress = 60;
        private const int HeaderCategory = 10;
        private const int HeaderToolVersion = 10;
        private const int HeaderPatreon = 10;
        private const int HeaderSlide = 1;

        private const int BottomRowPickups = 60 * 5;
        private const int BottomRowComplex = 30;
        private const int BottomRowSlide = 2;

        public bool FastForwarding { get; set; }

        private readonly SequenceTimer titleTimer;
        private readonly SequenceTimer pickupTimer;

        private UITextBlock text;
        private UITextBlock status;
        private UICarousel advancements;
        private UICarousel criteria;
        private UIFlowPanel counts;
        private UIControl runCompletePanel;
        private UIControl carouselPanel;
        private bool isResizing;
        private Color frameBackColor;
        private Color frameBorderColor;
        private float titleY;

        public override Color FrameBackColor() => this.frameBackColor;
        public override Color FrameBorderColor() => this.frameBorderColor;

        public UIOverlayScreen(Main main) : base(main, GameWindow.Create(main, 360, 360))
        {
            //initialize window
            this.Form.Text         = "Stream Overlay";
            this.Form.ControlBox   = false;
            this.Form.ResizeBegin += this.OnResizeBegin;
            this.Form.ResizeEnd   += this.OnResizeEnd;
            this.Form.Resize      += this.OnResize;
            this.Form.FormClosing += this.OnClosing;
            this.Window.AllowUserResizing = true;

            //cycle overlay header text
            this.titleTimer = new SequenceTimer(
                HeaderProgress, HeaderSlide,
                HeaderCategory, HeaderSlide,
                HeaderToolVersion, HeaderSlide,
                HeaderPatreon, HeaderSlide);

            //cycle pickup row
            this.pickupTimer = new SequenceTimer(
                BottomRowPickups, BottomRowSlide, 
                BottomRowComplex, BottomRowSlide);

            //load layout
            this.ReloadView();

            //enforce minimum size
            this.Form.MinimumSize = new System.Drawing.Size(1260 + this.Form.Width - this.Form.ClientSize.Width, 
                this.Height + this.Form.Height - this.Form.ClientSize.Height);
            this.Form.MaximumSize = new System.Drawing.Size(4096 + this.Form.Width - this.Form.ClientSize.Width, 
                this.Height + this.Form.Height - this.Form.ClientSize.Height);
        }

        public override void Prepare()
        {
            base.Prepare();
            this.GraphicsDevice.Clear(Config.Overlay.GreenScreen);
        }

        public override string GetCurrentView()
        {
            return Tracker.Category is AllBlocks
                ? Path.Combine(Paths.System.ViewsFolder, Tracker.Category.ViewName, Tracker.Category.CurrentMajorVersion, "overlay.xml")
                : Path.Combine(Paths.System.ViewsFolder, Tracker.Category.ViewName, "overlay.xml");
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
                width = Math.Max(width, this.Form.MinimumSize.Width - (this.Form.Width - this.Form.ClientSize.Width));
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

            this.text = new UITextBlock("minecraft", 24) {
                Padding             = new Margin(16, 16, 8, 0),
                HorizontalTextAlign = HorizontalAlign.Center,
                VerticalTextAlign   = VerticalAlign.Top
            };
            this.AddControl(this.text);

            //initialize main objective carousel
            this.advancements = this.First<UICarousel>("advancements");
            this.advancements?.SetScrollDirection(Config.Overlay.RightToLeft);

            //initialize criteria carousel
            this.criteria = this.First<UICarousel>("criteria");
            this.criteria?.SetScrollDirection(Config.Overlay.RightToLeft);

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
                    {
                        if (pickup.Id is not (ShulkerShell.ItemId or Mycelium.BlockId))
                            this.counts.AddControl(new UIObjectiveFrame(pickup, 3));
                    }
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
            this.UpdateContentVisibility();
            this.UpdateCarouselLocations();
            this.UpdateSpeed();

            if (Config.Overlay.AppearanceChanged)
                this.UpdateTheme();

            if (Config.Overlay.ArrangementChanged)
                this.UpdateDirection();

            //top center header text
            this.text?.SetText(this.PrepareHeaderText(time));
            //text next to pickup counts
            this.status?.SetText(this.PrepareStatusText());
        }

        private void UpdateTheme()
        {
            if (Config.Overlay.FrameStyle == "Custom Theme")
            {
                this.frameBackColor = Config.Overlay.CustomBackColor;
                this.frameBorderColor = Config.Overlay.CustomBorderColor;
            }
            else if (Config.MainConfig.Themes.TryGetValue(Config.Overlay.FrameStyle,
                out (Color back, Color text, Color border) theme))
            {
                this.frameBackColor = theme.back;
                this.frameBorderColor = theme.border;
            }
        }

        private void UpdateDirection()
        {
            this.advancements?.SetScrollDirection(Config.Overlay.RightToLeft);
            this.criteria?.SetScrollDirection(Config.Overlay.RightToLeft);

            if (this.counts is not null)
            {
                this.counts.FlowDirection = Config.Overlay.RightToLeft ^ Config.Overlay.PickupsOpposite
                    ? FlowDirection.RightToLeft
                    : FlowDirection.LeftToRight;
            }

            if (this.status is not null)
            {
                this.status.HorizontalTextAlign = Config.Overlay.RightToLeft
                    ? HorizontalAlign.Right
                    : HorizontalAlign.Left;
            }
        }

        private string PrepareHeaderText(Time time)
        {
            if (Tracker.Category.IsComplete())
                return string.Empty;

            //slide header in/out from top of screen
            int targetY = this.titleTimer.Index % 2 is 0 ? 0 : -48;
            this.titleY = MathHelper.Lerp(this.titleY, targetY, (float)(HeaderSlideSpeed * time.Delta));
            this.text.MoveTo(new Point(0, (int)this.titleY));

            this.titleTimer.Update(time);
            if (this.titleTimer.IsExpired)
                this.titleTimer.Continue();

            //cycle header text
            return this.titleTimer.Index switch {
                2 or 3 => $"Minecraft JE: {Tracker.Category.Name} ({Tracker.Category.CurrentVersion})",
                4 or 5 => $"{Main.ShortTitle} {(UpdateRequest.UpdatesAreAvailable() ? "(Outdated)" : "")}",
                6 or 7 => Paths.Web.PatreonShort,
                _ => Tracker.Category.GetStatus()
            };
        }

        private string PrepareStatusText()
        {
            if (Client.TryGet(out Client client))
            {
                //use smaller font
                this.status?.SetFont("minecraft", 24);
                return client.GetShortStatusText();
            }
            else if (MinecraftServer.IsEnabled)
            {
                //use smaller font
                this.status?.SetFont("minecraft", 24);
                return MinecraftServer.GetShortStatusText();
            }
            else
            {
                //use larger font
                this.status?.SetFont("minecraft", 48);
                if (Config.Overlay.ShowIgt && Tracker.InGameTime != default)
                    return Tracker.GetFullIgt();
            }
            return string.Empty;
        }

        private void UpdateSpeed()
        {
            float speed = BaseScrollSpeed + (Config.Overlay.Speed * ScrollSpeedMultiplier);
            if (this.FastForwarding)
                speed *= 60;
            this.advancements?.SetSpeed(speed);
            this.criteria?.SetSpeed(speed);
        }

        private void UpdateCarouselLocations()
        {
            //update vertical position of rows based on what is or isn't enabled
            int criteria = this.criteria is null || this.criteria.IsCollapsed 
                ? 0 : 64;

            int advancement = this.advancements is null || this.advancements.IsCollapsed 
                ? 0 : Config.Overlay.ShowLabels ? 160 : 110;

            this.criteria?.MoveTo(new Point(0, HeaderHeight));
            this.advancements?.MoveTo(new Point(0, HeaderHeight + criteria));
            this.counts?.MoveTo(new Point(this.counts.X, HeaderHeight + criteria + advancement));
        }

        private void UpdateContentVisibility()
        {
            //handle completion screen popup
            if (Tracker.Category.IsComplete())
            {
                this.carouselPanel?.Collapse();  
                if (this.runCompletePanel?.IsCollapsed is true)
                {
                    this.runCompletePanel.Expand();
                    this.runCompletePanel.First<UIRunComplete>()?.Show();
                }         
            }
            else
            {
                this.carouselPanel?.Expand();
                this.runCompletePanel?.Collapse();
            }

            //update criteria visibility
            this.criteria?.SetVisibility(Config.Overlay.ShowCriteria);

            //update item count visibility
            if (this.counts is not null)
            {
                this.counts.SetVisibility(Config.Overlay.ShowPickups);
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

        private void RememberWindowPosition()
        {
            Config.Overlay.LastWindowPosition.Set(new Point(this.Form.Location.X, this.Form.Location.Y));
            Config.Overlay.Save();
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

        private void OnClosing(object sender, FormClosingEventArgs e)
        {
            this.RememberWindowPosition();
        }
    }
}
