using AATool.Settings;
using AATool.UI.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Windows.Forms;

namespace AATool.UI.Screens
{
    class OverlayScreen : Screen
    {
        private OverlaySettings settings = OverlaySettings.Instance;
        private UITextBlock progress;
        private UICarousel advancements;
        private UICarousel criteria;
        private UIControl counts;

        private bool isResizing;

        public OverlayScreen(Main main) : base(main, GameWindow.Create(main, 1920, 380), 1920, 380)
        {
            LoadXml(Path.Combine(Paths.DIR_SCREENS, "screen_overlay.xml"));
            Form.Text = "CTM's Advancement Overlay";

            progress = new UITextBlock("minecraft", 24);
            progress.Margin = new Margin(12, 0, 8, 0);
            progress.HorizontalAlign = HorizontalAlign.Left;
            progress.VerticalAlign = VerticalAlign.Top;
            progress.ResizeRecursive(Rectangle);
            AddControl(progress);

            advancements = GetControlByName("advancements") as UICarousel;
            criteria     = GetControlByName("criteria") as UICarousel;
            counts       = GetControlByName("grid_counts");

            Window.AllowUserResizing = true;
            Form.ControlBox = false;
            Form.MinimumSize = new System.Drawing.Size(Form.Width - Form.ClientSize.Width + 768, 128);
            Form.MaximumSize = new System.Drawing.Size(5120, 512);
            Form.ResizeBegin += OnResizeBegin;
            Form.ResizeEnd += OnResizeEnd;

            MoveTo(new Point(0, 0));
            if (settings.Enabled)
                Show();
        }

        private void OnResizeBegin(object sender, EventArgs e)
        {
            isResizing = true;
        }

        private void OnResizeEnd(object sender, EventArgs e)
        {
            isResizing = false;
            settings.Width = Form.ClientSize.Width;
            settings.Save();
        }

        protected override void UpdateThis(Time time)
        {
            if (!Form.IsDisposed)
                Form.Visible = settings.Enabled;
            if (!Form.Visible)
                return;

            UpdateVisibility();
            UpdateSpawnHeights();
            UpdateWidth();

            progress.SetText(AdvancementTracker.CompletedCount + " / " + AdvancementTracker.AdvancementCount);
        }

        private void UpdateSpawnHeights()
        {
            //update vertical position of rows based on what is or isn't enabled
            int progressHeight      = progress.IsCollapsed      ? 16 : 42;
            int criteriaHeight      = criteria.IsCollapsed      ? 0  : 64;
            int advancementHeight   = advancements.IsCollapsed  ? 0  : settings.ShowLabels ? 160 : 110;
            int countHeight         = counts.IsCollapsed        ? 0  : 128;

            criteria?.MoveTo(new Point(0, progressHeight));
            advancements?.MoveTo(new Point(0, progressHeight + criteriaHeight));
            counts?.MoveTo(new Point(counts.X, progressHeight + criteriaHeight + advancementHeight));

            SetWindowSize(Width, progressHeight + criteriaHeight + advancementHeight + countHeight + 20);
        }

        private void UpdateVisibility()
        {
            //update overview visibility
            if (progress.IsCollapsed == settings.ShowOverview)
                if (settings.ShowOverview)
                    progress.Expand();
                else
                    progress.Collapse();

            //update criteria visibility
            if (criteria.IsCollapsed == settings.ShowCriteria)
                if (settings.ShowCriteria)
                    criteria.Expand();
                else
                    criteria.Collapse();

            //update item count visibility
            if (counts.IsCollapsed == settings.ShowCounts)
                if (settings.ShowCounts)
                    counts.Expand();
                else
                    counts.Collapse();
        }

        private void UpdateWidth()
        {
            if (!isResizing && SwapChain.Width != settings.Width)
                SetWindowSize(settings.Width, Height);
        }

        public override void Prepare(Display display)
        {
            base.Prepare(display);
            GraphicsDevice.Clear(settings.BackColor);
        }

        public override void Present(Display display)
        {
            if (isResizing)
                display.DrawRectangle(Rectangle, settings.BackColor);
            base.Present(display);
        }
    }
}
