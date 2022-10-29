
using AATool.Configuration;
using AATool.Data.Objectives;
using AATool.Graphics;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    public class UIPinnedObjectiveFrame : UIObjectiveFrame
    {
        public bool Hovering { get; set; }
        public bool HoveringUnpin { get; set; }

        private  UIPinnedRow container;

        private const int SelectionPaddingTop = 5;
        private const int SelectionPaddingBottom = 30;
        private const int SelectionPaddingVertical = SelectionPaddingTop + SelectionPaddingBottom;

        private static readonly Color UnpinHoverColor = new (165, 36, 45);
        private static readonly Color UnpinNormalColor = new (232, 17, 35);

        private Rectangle selectBounds;
        private Rectangle unpinBounds;

        public float PreciseCenterX { get; set; } = float.MinValue;

        private Color textColor;

        public UIPinnedObjectiveFrame(UIPinnedRow container, Objective objective)
        {
            this.container = container;
            this.SetObjective(objective);
            this.BuildFromTemplate();
            this.Scale = 3;
        }

        public override void InitializeRecursive(UIScreen screen)
        {
            base.InitializeRecursive(screen);
            this.Layer = Layer.Fore;
        }

        public override void ResizeThis(Rectangle parent) 
        {
            base.ResizeThis(parent);
            this.UpdateBounds();
        }

        private void UpdateBounds()
        {
            this.selectBounds = new Rectangle(
                this.Left,
                this.Top - SelectionPaddingTop,
                this.Width,
                this.Height + SelectionPaddingVertical);

            const int UnpinSize = 20;
            this.unpinBounds = new Rectangle(
                this.Frame.Right - UnpinSize,
                this.Frame.Top,
                UnpinSize,
                UnpinSize);
        }

        protected override void UpdateThis(Time time)
        {
            if (this.Root().HasFocus)
                this.UpdateCursor(time);
            else
                this.Hovering = false;

            if (Config.Overlay.Width.Changed)
                this.UpdateBounds();

            this.UpdateLabelColor();

            base.UpdateThis(time);
        }

        private void UpdateLabelColor()
        {
            this.textColor = Config.Overlay.FrameStyle == "Custom Theme"
                ? Config.Overlay.CustomTextColor
                : Color.White;
        }

        private void UpdateCursor(Time time)
        {
            Point cursor = Input.Cursor(this.Root());
            if (this.Frame.Bounds.Contains(cursor))
            {
                this.Hovering = true;
                if (Input.LeftClickStarted)
                    _= this.container.TryStartDragging(this, cursor);
            }
            else if (!this.container.IsDragging(this))
            {
                this.Hovering = false;
            }

            if (this.container.IsDragging(this))
            {
                if (Input.LeftClicking)
                {
                    this.container.ContinueDrag(this, cursor, time);
                }
                else
                {
                    this.container.StopDragging(this);
                }
            }

            this.HoveringUnpin = this.unpinBounds.Contains(cursor);
            if (this.HoveringUnpin && Input.LeftClickStarted)
                this.container.Unpin(this);
        }

        public override void DrawThis(Canvas canvas)
        { 
            base.DrawThis(canvas);
            if ((this.Hovering && !this.container.Dragging) || this.container.IsDragging(this))
            {
                int offsetY = this.container.Dragging ? 0 : 8;

                var topArrow = new Rectangle(
                    this.selectBounds.Center.X - 16,
                    this.selectBounds.Top - offsetY, 
                    32, 16);
                canvas.Draw("overlay_arrow_top", topArrow, this.textColor);
            }

            if (this.container.IsDragging(this))
                this.DrawDragBounds(canvas);
        }

        public override void DrawRecursive(Canvas canvas)
        {
            if (!this.IsCollapsed)
            {
                this.DrawThis(canvas);
                this.Icon.DrawRecursive(canvas);
                if (!this.container.Dragging)
                { 
                    this.Label.DrawRecursive(canvas);
                    if (this.Hovering)
                        this.DrawUnpinButton(canvas);
                }
            }
        }

        private void DrawUnpinButton(Canvas canvas)
        {
            if (Input.LeftClicking)
                return;

            if (this.HoveringUnpin)
                canvas.DrawRectangle(this.unpinBounds, UnpinNormalColor, UnpinNormalColor, 3, Layer.Fore);
            else
                canvas.DrawRectangle(this.unpinBounds, UnpinHoverColor, UnpinHoverColor, 3, Layer.Fore);
            canvas.Draw("unpin", this.unpinBounds, Color.White, Layer.Fore);
        }

        private void DrawDragBounds(Canvas canvas)
        {
            var top = new Rectangle(this.selectBounds.Center.X - 48, this.selectBounds.Top, 96, 8);
            //canvas.Draw("overlay_drag_top", top, Config.Overlay.CustomTextColor, Layer.Fore);
            
            var bottom = new Rectangle(this.selectBounds.Center.X - 48, this.selectBounds.Bottom - 8, 96, 8);
            //canvas.Draw("overlay_drag_bottom", bottom, Config.Overlay.CustomTextColor, Layer.Fore);

            var bottomArrow = new Rectangle(
                    this.selectBounds.Center.X - 16,
                    this.selectBounds.Top + 96,
                    32, 16);
            canvas.Draw("overlay_arrow_bottom", bottomArrow, this.textColor);

            //canvas.DrawRectangle(left, Color.White, null, 0, Layer.Fore);
            //canvas.Draw("overlay_drag_bottom", right, Color.White, Layer.Fore);
        }

        public void LerpToIndex(int i, float amount)
        {
            if (this.container.IsDragging(this))
                return;

            if (this.PreciseCenterX is float.MinValue)
                this.PreciseCenterX = this.Center.X;

            int targetX;
            if (this.container.HorizontalAlign == HorizontalAlign.Left)
                targetX = i * UIPinnedRow.FrameWidth;
            else
                targetX = this.container.Width - ((i + 1) * UIPinnedRow.FrameWidth);

            this.PreciseCenterX = (int)MathHelper.Lerp(this.PreciseCenterX, targetX, amount);
            if (this.X != (int)this.PreciseCenterX)
            {
                this.MoveTo(new Point((int)this.PreciseCenterX, this.Parent.Top));
            }
        }
    }
}
