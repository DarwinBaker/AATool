using System;
using System.Xml;
using AATool.Configuration;
using AATool.Graphics;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace AATool.UI.Controls
{
    class UISlider : UIControl
    {
        public delegate void ValueChangeEventHandler(UISlider sender);
        public event ValueChangeEventHandler OnValueChange;

        double value;
        double displayValue;
        bool isVertical;

        public bool Enabled { get; set; } = true;

        Rectangle trackStartBounds;
        Rectangle trackMiddleBounds;
        Rectangle trackEndBounds;
        Rectangle gripTextureBounds;
        Rectangle gripBounds;

        bool grabbed;
        ControlState state;

        string startTexture;
        string endTexture;
        string gripTexture;

        bool drawTrack;

        public double Value
        {
            get => this.value;
            set {
                double valuePrevious = this.value;
                this.value = value;
                this.value = Math.Min(this.value, 1);
                this.value = Math.Max(this.value, 0);
                if (this.value != valuePrevious)
                    OnValueChange?.Invoke(this);
            }
        }

        public bool IsVertical
        {
            get => this.isVertical;
            set {
                this.isVertical = value;
                this.startTexture = this.isVertical ? "slider_track_top" : "slider_track_left";
                this.endTexture = this.isVertical ? "slider_track_bottom" : "slider_track_right";
                this.gripTexture = this.isVertical ? "slider_grip_vertical" : "slider_grip_horizontal";
            }
        }

        private void SetState(ControlState newState) => this.state = newState;

        public override void ResizeThis(Rectangle parent)
        {
            base.ResizeThis(parent);
            this.UpdateBounds();
        }

        protected override void UpdateThis(Time time)
        {
            this.UpdateState();
            this.UpdateValue(time);
        }

        private void UpdateState()
        {
            ControlState previousState = this.state;
            if (!this.Enabled)
            {
                this.SetState(ControlState.Disabled);
                return;
            }

            Point cursor = Input.Cursor(this.Root());
            bool containsCursor = this.Bounds.Contains(cursor);

            //update button state
            if (containsCursor && Input.LeftClickStarted)
                this.SetState(ControlState.Pressed);

            if (!Input.LeftClicking)
            {
                if (containsCursor)
                    this.SetState(ControlState.Hovered);
                else
                    this.SetState(ControlState.Released);
            }

            if (this.state != previousState && this.Root() is UIMainScreen)
                UIMainScreen.Invalidate();
        }

        private void UpdateValue(Time time)
        {
            if (this.state is ControlState.Pressed)
            {
                Point cursor = Input.Cursor(this.Root());
                this.Value = this.IsVertical
                    ? 1 - ((cursor.Y - this.Inner.Top) / (float)this.Inner.Height)
                    : (cursor.X - this.Inner.Left) / (float)this.Inner.Width;
            }

            double previousDisplayValue = this.displayValue;
            float lerpSpeed = this.state is ControlState.Pressed ? 30 : 8;
            this.displayValue += (this.Value - this.displayValue) * (float)(lerpSpeed * time.Delta);
            this.displayValue = Math.Min(this.displayValue, 1);
            this.displayValue = Math.Max(this.displayValue, 0);
            if (this.displayValue != previousDisplayValue)
            {
                this.UpdateBounds();
                UIMainScreen.Invalidate();
            }

            if (Math.Abs(this.Value - this.displayValue) < 0.001f)
                this.displayValue = this.Value;
        }

        private void UpdateBounds()
        {
            const int GripThickness = 16;
            int width = this.IsVertical ? this.Inner.Width : GripThickness;
            int height = this.IsVertical ? GripThickness : this.Inner.Height;

            int top = this.IsVertical
                ? this.Inner.Top - (height / 2) + (int)(this.Inner.Height * (1 - this.displayValue))
                : this.Inner.Top;

            int left = this.IsVertical
                ? this.Inner.Left
                : this.Inner.Left - (width / 2) + (int)(this.Inner.Width * this.displayValue);

            this.gripBounds = new Rectangle(left, top, width, height);

            const int TrackThickness = 8;
            if (this.IsVertical)
            {
                this.trackStartBounds = new Rectangle(this.Inner.Center.X - (TrackThickness / 2), this.Inner.Top, TrackThickness, TrackThickness);
                this.trackMiddleBounds = new Rectangle(this.Inner.Center.X - (TrackThickness / 2), this.Inner.Top + TrackThickness - 1, TrackThickness, this.Inner.Height - (TrackThickness * 2) + 2);
                this.trackEndBounds = new Rectangle(this.Inner.Center.X - (TrackThickness / 2), this.Inner.Bottom - TrackThickness, TrackThickness, TrackThickness);
                this.gripTextureBounds = new Rectangle(this.gripBounds.Center.X - 11, this.gripBounds.Center.Y - 5, 22, 10);
            }
            else
            {
                this.trackStartBounds = new Rectangle(this.Inner.Left, this.Inner.Center.Y - (TrackThickness / 2), TrackThickness, TrackThickness);
                this.trackMiddleBounds = new Rectangle(this.Inner.Left + TrackThickness, this.Inner.Center.Y - (TrackThickness / 2), this.Inner.Width - (TrackThickness * 2), TrackThickness);
                this.trackEndBounds = new Rectangle(this.Inner.Right - TrackThickness, this.Inner.Center.Y - (TrackThickness / 2), TrackThickness, TrackThickness);
                this.gripTextureBounds = new Rectangle(this.gripBounds.Center.X - 5, this.gripBounds.Center.Y - 11, 10, 22);
            }
        }

        public override void DrawThis(Canvas canvas)
        {
            if (this.SkipDraw)
                return;

            base.DrawThis(canvas);
            this.DrawTrack(canvas);
            this.DrawGrip(canvas);
        }

        private void DrawTrack(Canvas canvas)
        {
            canvas.Draw(this.startTexture, this.trackStartBounds, Config.Main.BorderColor, this.Layer);
            canvas.DrawRectangle(this.trackMiddleBounds, Config.Main.BorderColor, null, 0, this.Layer);
            canvas.Draw(this.endTexture, this.trackEndBounds, Config.Main.BorderColor, this.Layer);
        }

        private void DrawGrip(Canvas canvas)
        {
            Color backColor = Config.Main.BackColor;
            Color borderColor = Config.Main.BorderColor;
            int borderThickness = 2;

            switch (this.state)
            {
                case ControlState.Released:
                    canvas.DrawRectangle(this.gripBounds, backColor, borderColor, borderThickness, this.Layer);
                    canvas.Draw(this.gripTexture, this.gripTextureBounds, borderColor, this.Layer);
                    break;
                case ControlState.Hovered:
                    borderColor = (Config.Main.RainbowMode || borderColor == Color.White) ? borderColor * 0.5f : borderColor * 1.25f;
                    canvas.DrawRectangle(this.gripBounds, backColor, borderColor * 1.25f, borderThickness, this.Layer);
                    canvas.Draw(this.gripTexture, this.gripTextureBounds, borderColor * 1.25f, this.Layer);
                    break;
                case ControlState.Pressed:
                    canvas.DrawRectangle(this.gripBounds, backColor * 1.25f, borderColor * 1.5f, borderThickness + (borderThickness / 2), this.Layer);
                    canvas.Draw(this.gripTexture, this.gripTextureBounds, borderColor * 1.5f, this.Layer);
                    break;
                case ControlState.Disabled:
                    canvas.DrawRectangle(this.gripBounds, backColor * 1.1f, backColor * 1.2f, borderThickness, this.Layer);
                    canvas.Draw(this.gripTexture, this.gripTextureBounds, backColor * 1.2f, this.Layer);
                    break;
            }
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            this.IsVertical = Attribute(node, "vertical", false);
        }
    }
}
