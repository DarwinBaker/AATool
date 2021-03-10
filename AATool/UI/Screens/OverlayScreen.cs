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
        private UIFlowPanel counts;

        private bool isResizing;

        public OverlayScreen(Main main) : base(main, GameWindow.Create(main, 1920, 380), 1920, 380)
        {
            ReloadLayout();
            Form.Text = "All Advancements Stream Overlay";

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

        private void ReloadLayout()
        {
            //clear and load layout if window just opened or game version changed
            Children.Clear();
            if (!LoadXml(Paths.GetLayoutFor("overlay")))
                Main.ForceQuit();

            progress = new UITextBlock("minecraft", 24);
            progress.Margin = new Margin(12, 0, 8, 0);
            progress.HorizontalAlign = HorizontalAlign.Left;
            progress.VerticalAlign = VerticalAlign.Top;
            progress.ResizeRecursive(Rectangle);
            AddControl(progress);

            //find named controls
            advancements = GetControlByName("advancements") as UICarousel;
            criteria = GetControlByName("criteria") as UICarousel;
            counts = GetControlByName("counts") as UIFlowPanel;
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
            //update enabled state
            if (!Form.IsDisposed)
                Form.Visible = settings.Enabled;
            if (!Form.Visible)
                return;
            if (settings.ValueChanged(OverlaySettings.ENABLED) && settings.Enabled)
                ReloadLayout();

            //update game version
            if (TrackerSettings.Instance.ValueChanged(TrackerSettings.GAME_VERSION))
                ReloadLayout();

            UpdateVisibility();
            UpdateSpawnHeights();
            UpdateWidth();

            if (settings.OnlyShowFavorites && settings.Favorites.IsEmpty)
                progress.SetText("\"Show Favorites Only\" is enabled, but no user favorites have been picked!");
            else
                progress.SetText(AdvancementTracker.CompletedCount + " / " + AdvancementTracker.AdvancementCount);

            if (counts != null)
                counts.FlowDirection = OverlaySettings.Instance.RightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
        }

        private void UpdateSpawnHeights()
        {
            //update vertical position of rows based on what is or isn't enabled
            int progressHeight      = progress     == null || progress.IsCollapsed      ? 16 : 42;
            int criteriaHeight      = criteria     == null || criteria.IsCollapsed      ? 0  : 64;
            int advancementHeight   = advancements == null || advancements.IsCollapsed  ? 0  : settings.ShowLabels ? 160 : 110;
            int countHeight         = counts       == null || counts.IsCollapsed        ? 0  : 128;

            criteria?.MoveTo(new Point(0, progressHeight));
            advancements?.MoveTo(new Point(0, progressHeight + criteriaHeight));
            counts?.MoveTo(new Point(counts.X, progressHeight + criteriaHeight + advancementHeight));

            SetWindowSize(Width, progressHeight + criteriaHeight + advancementHeight + countHeight + 20);
        }

        private void UpdateVisibility()
        {
            //update overview visibility
            if (progress?.IsCollapsed == settings.ShowOverview)
                if (settings.ShowOverview)
                    progress.Expand();
                else
                    progress.Collapse();

            //update criteria visibility
            if (criteria?.IsCollapsed == settings.ShowCriteria)
                if (settings.ShowCriteria)
                    criteria.Expand();
                else
                    criteria.Collapse();

            if (counts == null)
                return;

            //update item count visibility
            if (counts.IsCollapsed == settings.ShowCounts)
                if (settings.ShowCounts)
                    counts.Expand();
                else
                    counts.Collapse();

            //update visibility for individual item counters (favorites)
            foreach (var control in counts.Children)
            {
                var count = control as UIItemCount;
                bool shouldBeCollapsed = false;
                if (settings.OnlyShowFavorites && !settings.Favorites.Statistics.Contains(count.ItemName))
                    shouldBeCollapsed = true;              
                if (!settings.ShowCounts)
                    shouldBeCollapsed = true;

                if (count?.IsCollapsed != shouldBeCollapsed)
                {
                    if (shouldBeCollapsed)
                        count.Collapse();
                    else
                        count.Expand();
                    counts.ReflowChildren();
                } 
            }
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
    }
}
