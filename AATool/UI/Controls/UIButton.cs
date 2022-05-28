using System.Xml;
using AATool.Configuration;
using AATool.Graphics;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AATool.UI.Controls
{
    public class UIButton : UIPanel
    {
        public delegate void ClickEventHandler(UIControl sender);
        public event ClickEventHandler OnClick;

        public bool Enabled { get; set; }
        public ControlState State { get; private set; }
        public UITextBlock TextBlock { get; private set; }

        public bool UseCustomColor;
        public bool ShowBorder;

        public void SetText(string text) => this.TextBlock?.SetText(text);
        public void SetTextColor(Color color) => this.TextBlock?.SetTextColor(color);
        private void SetState(ControlState newState) => this.State = newState;

        public UIButton()
        {
            this.Enabled = true;
            this.ShowBorder = true;
            this.TextBlock = new UITextBlock();
            this.AddControl(this.TextBlock);
        }

        public override void InitializeThis(UIScreen screen)
        {
            this.TextBlock.FlexHeight = new Size(13, SizeMode.Absolute);
        }

        protected override void UpdateThis(Time time)
        {
            ControlState previousState = this.State;
            if (!this.Enabled)
            {
                this.SetState(ControlState.Disabled);
                return;
            }

            Point cursor = Input.Cursor(this.Root());

            //update button state
            if (this.Bounds.Contains(cursor))
            {
                if (Input.LeftClicking)
                {
                    this.SetState(ControlState.Pressed);
                }
                else
                {
                    if (Input.MousePrev.LeftButton is ButtonState.Pressed && this.Root().HasFocus)
                        OnClick?.Invoke(this);
                    this.SetState(ControlState.Hovered);
                }
            }
            else
            {
                this.SetState(ControlState.Released);
            }

            if (this.State != previousState && this.Root() is UIMainScreen)
                UIMainScreen.Invalidate();
        }

        public override void DrawThis(Canvas canvas)
        {
            if (this.SkipDraw)
                return;

            Color backColor   = this.UseCustomColor ? this.BackColor   : Config.Main.BackColor;
            Color borderColor = this.UseCustomColor ? this.BorderColor : Config.Main.BorderColor;
            if (!this.ShowBorder)
                borderColor = backColor;

            switch (this.State)
            {
                case ControlState.Released:
                    canvas.DrawRectangle(this.Bounds, backColor, borderColor, this.BorderThickness, this.Layer);
                    break;
                case ControlState.Hovered:
                    canvas.DrawRectangle(this.Bounds, backColor, borderColor * 1.25f, this.BorderThickness, this.Layer);
                    break;
                case ControlState.Pressed:
                    canvas.DrawRectangle(this.Bounds, backColor * 1.25f, borderColor * 1.5f, this.BorderThickness + (this.BorderThickness / 2), this.Layer);
                    break;
                case ControlState.Disabled:
                    canvas.DrawRectangle(this.Bounds, backColor * 1.1f, backColor * 1.2f, this.BorderThickness, this.Layer);
                    break;
            }
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            string text = Attribute(node, "text", string.Empty);
            if (!string.IsNullOrEmpty(text))
                this.TextBlock?.SetText(text);

            int width = Attribute(node, "text_width", 0);
            if (width > 0)
                this.TextBlock.FlexWidth = new Size(width);

            int fontSize = Attribute(node, "font_size", 0);
            if (fontSize > 0)
                this.TextBlock.SetFont("minecraft", fontSize);

            this.Enabled = Attribute(node, "enabled", true);
            this.TextBlock.HorizontalAlign = Attribute(node, "text_align", this.TextBlock.HorizontalAlign);
            this.ShowBorder = Attribute(node, "border", true);
			this.TextBlock.SetLayer(this.Layer);

            this.BorderThickness = Attribute(node, "border_thickness", 2);
        }
    }
}
