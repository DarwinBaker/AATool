using AATool.Graphics;
using AATool.Settings;
using AATool.UI.Controls;
using AATool.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Windows.Forms;

namespace AATool.UI.Screens
{
    sealed class UIOverlayScreen : UIScreen
    {
        private const int SCROLL_SPEED_MULTIPLIER = 15;
        private const int BASE_SCROLL_SPEED       = 30;
        private const int TITLE_MOVE_SPEED        = 4;

        private readonly SequenceTimer titleTimer;
        
        private UITextBlock progress;
        private UITextBlock text;
        private UICarousel advancements;
        private UICarousel criteria;
        private UIFlowPanel counts;
        private UIControl runCompletePanel;
        private UIControl carouselPanel;
        private bool isResizing;

        private float titleY;

        public UIOverlayScreen(Main main) : base(main, GameWindow.Create(main, 360, 360))
        {
            this.Form.Text         = "Stream Overlay";
            this.Form.ControlBox   = false;
            this.Form.ResizeBegin += this.OnResizeBegin;
            this.Form.ResizeEnd   += this.OnResizeEnd;
            this.Form.Resize      += this.OnResize;
            this.Window.AllowUserResizing = true;
            this.titleTimer = new SequenceTimer(60, 1, 10, 1, 10, 1, 10, 1);
            
            this.ReloadLayout();
            this.Form.MinimumSize = new System.Drawing.Size(1260 + this.Form.Width - this.Form.ClientSize.Width, 
                this.Height + this.Form.Height - this.Form.ClientSize.Height);

            this.Form.MaximumSize = new System.Drawing.Size(4096 + this.Form.Width - this.Form.ClientSize.Width, 
                this.Height + this.Form.Height - this.Form.ClientSize.Height);

            this.MoveTo(Point.Zero);
        }

        protected override void ReloadLayout()
        {
            //clear and load layout if window just opened or game version changed
            this.ClearControls();
            if (!this.TryLoadXml("assets/ui/screens/screen_overlay.xml"))
                Main.QuitBecause("Error loading overlay layout!");

            this.InitializeRecursive(this);
            this.ResizeRecursive(new Rectangle(0, 0, Config.Overlay.Width, this.Height));
        }

        protected override void ConstrainWindow()
        {
            int width  = Config.Overlay.Width;
            int height = this.Height;
            if (this.Form.MinimumSize.Width > 0)
                width  = Math.Max(width, this.Form.MinimumSize.Width - (this.Form.Width - this.Form.ClientSize.Width));
            if (this.Form.MinimumSize.Height > 0)
                height = Math.Max(height, this.Form.MinimumSize.Height - (this.Form.Height - this.Form.ClientSize.Height));

            if (width is 0 || height is 0)
                return;

            //resize window and create new render target of proper size
            if (this.SwapChain is null || this.SwapChain.Width != width || this.SwapChain.Height != height)
            {
                this.Form.ClientSize = new System.Drawing.Size(width, height);
                this.SwapChain?.Dispose();
                this.SwapChain = new SwapChainRenderTarget(this.GraphicsDevice, this.Window.Handle, width, height);
                this.ResizeRecursive(new Rectangle(0, 0, width, height));
                this.UpdateCarouselLocations();
            }
        }

        public override void InitializeRecursive(UIScreen screen)
        {
            this.runCompletePanel = this.First("panel_congrats");
            this.carouselPanel = this.First("panel_carousel");

            int textScale = 12 * ((Config.Overlay.Scale + 1) / 2);

            this.progress = new UITextBlock("minecraft", textScale) {
                Margin        = new Margin(16, 0, 8, 0),
                TextAlign     = HorizontalAlign.Left,
                VerticalAlign = VerticalAlign.Top
            };
            this.AddControl(this.progress);

            this.text = new UITextBlock("minecraft", textScale) {
                Padding       = new Margin(16, 16, 8, 0),
                TextAlign     = HorizontalAlign.Left,
                VerticalAlign = VerticalAlign.Top
            };
            this.AddControl(this.text);

            //find named controls
            this.advancements = this.First<UICarousel>("advancements");
            this.criteria     = this.First<UICarousel>("criteria");
            this.counts       = this.First<UIFlowPanel>("counts");

            this.UpdateSpeed();

            this.advancements.SetScrollDirection(Config.Overlay.RightToLeft);
            this.criteria.SetScrollDirection(Config.Overlay.RightToLeft);

            this.counts.FlowDirection = Config.Overlay.RightToLeft
                ? FlowDirection.RightToLeft
                : FlowDirection.LeftToRight;

            foreach (string item in Tracker.AllItems.Keys.Reverse())
            {
                var count = new UIItemCount(3){
                    ItemName = item,
                };
                this.counts.AddControl(count);
            }
            base.InitializeRecursive(screen);
        }

        private void OnResizeBegin(object sender, EventArgs e)
        {
            this.isResizing = true;
            this.advancements.Break();
            this.criteria.Break();
        }

        private void OnResize(object sender, EventArgs e)
        {
            if (this.Form.WindowState is FormWindowState.Minimized)
                this.Form.WindowState = FormWindowState.Normal;

            if (this.isResizing)
                Config.Overlay.Width = this.Form.ClientSize.Width;
        }

        private void OnResizeEnd(object sender, EventArgs e)
        {
            this.isResizing = false;
            this.advancements.Continue();
            this.criteria.Continue();
            Config.Overlay.Width = this.Form.ClientSize.Width;
            Config.Overlay.Save();
        }

        protected override void UpdateThis(Time time)
        {
            //update enabled state
            if (!this.Form.IsDisposed)
                this.Form.Visible = Config.Overlay.Enabled;
            if (!this.Form.Visible)
                return;

            if (Config.Tracker.GameVersionChanged() || Config.Overlay.EnabledChanged())
                this.ReloadLayout();

            this.ConstrainWindow();
            this.UpdateVisibility();
            this.UpdateCarouselLocations();

            //reset carousels if settings change
            if (Config.Overlay.DirectionChanged())
            {
                this.advancements?.SetScrollDirection(Config.Overlay.RightToLeft);
                this.criteria?.SetScrollDirection(Config.Overlay.RightToLeft);
            }

            if (Config.Overlay.SpeedChanged())
                this.UpdateSpeed();

            if (Tracker.IsComplete)
                this.text.Collapse();
            else
                this.text.Expand();
            
            if (this.counts is not null)
            {
                this.counts.FlowDirection = Config.Overlay.RightToLeft
                    ? FlowDirection.RightToLeft
                    : FlowDirection.LeftToRight;

                this.progress.TextAlign = Config.Overlay.RightToLeft
                    ? HorizontalAlign.Left
                    : HorizontalAlign.Right;
            }

            this.titleTimer.Update(time);
            if (this.titleTimer.IsExpired)
            {
                if (this.titleTimer.Index is 3)
                    this.text.SetText(Main.FullTitle);
                else if (this.titleTimer.Index is 5)
                    this.text.SetText(Paths.URL_PATREON_FRIENDLY);

                this.titleTimer.Continue();
            }

            if (this.titleTimer.Index is 0)
            {
                this.text.SetText($"{Tracker.CompletedAdvancements} / {Tracker.AdvancementCount} ");
                if (Config.IsPostExplorationUpdate)
                    this.text.Append("Advancements Complete");
                else
                    this.text.Append("Achievements Complete");
            }
            else if (this.titleTimer.Index is 2)
            {
                this.text.SetText($"Minecraft JE: All Advancements ({Config.Tracker.GameVersion})");
            }

            this.text.TextAlign = Config.Overlay.RightToLeft
                ? HorizontalAlign.Left
                : HorizontalAlign.Right;

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
            this.advancements?.SetSpeed(speed);
            this.criteria?.SetSpeed(speed);
        }

        private void UpdateCarouselLocations()
        {
            //update vertical position of rows based on what is or isn't enabled
            int title = 42;

            int criteria = this.criteria is null || this.criteria.IsCollapsed 
                ? 0 
                : 64;

            int advancement = this.advancements is null || this.advancements.IsCollapsed 
                ? 0 
                : Config.Overlay.ShowLabels ? 160 : 110;

            int count = this.counts is null || this.counts.IsCollapsed 
                ? 0 
                : 128;

            this.criteria?.MoveTo(new Point(0, title));
            this.advancements?.MoveTo(new Point(0, title + criteria));
            this.counts?.MoveTo(new Point(this.counts.X, title + criteria + advancement));
        }

        private void UpdateVisibility()
        {
            //handle completion message
            if (Tracker.IsComplete)
            {
                if (this.runCompletePanel.IsCollapsed)
                {
                    this.runCompletePanel.Expand();
                    this.runCompletePanel.First<UIRunComplete>().Show();
                }         
                this.carouselPanel.Collapse();  
            }
            else
            {
                this.runCompletePanel.Collapse();
                this.carouselPanel.Expand();
            }

            //update criteria visibility
            if (this.criteria.IsCollapsed == Config.Overlay.ShowCriteria)
            {
                if (Config.Overlay.ShowCriteria)
                    this.criteria.Expand();
                else
                    this.criteria.Collapse();
            }

            if (this.counts is null)
                return;

            //update item count visibility
            if (this.counts.IsCollapsed == Config.Overlay.ShowCounts)
            {
                if (Config.Overlay.ShowCounts)
                    this.counts.Expand();
                else
                    this.counts.Collapse();
            }

            //update visibility for individual item counters (favorites)
            foreach (UIControl control in this.counts.Children)
            {
                if (control.IsCollapsed == Config.Overlay.ShowCounts)
                {
                    if (Config.Overlay.ShowCounts)
                        control.Expand();
                    else
                        control.Collapse();
                    this.counts.ReflowChildren();
                }
            }
        }

        public override void Prepare(Display display)
        {
            base.Prepare(display);
            this.GraphicsDevice.Clear(Config.Overlay.BackColor);
        }
    }
}
