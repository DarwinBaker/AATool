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

        public bool Enabled          { get; set; }
        public ButtonState State   { get; private set; }
        public UITextBlock TextBlock { get; private set; }

        public bool UseCustomColor;
        public bool ShowBorder;

        protected MouseState MouseNow;
        protected MouseState MousePrev;

        public void SetText(string text) => this.TextBlock?.SetText(text);
        public void SetTextColor(Color color) => this.TextBlock?.SetTextColor(color);
        private void SetState(ButtonState newState) => this.State = newState;

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
            ButtonState previousState = this.State;
            if (!this.Enabled)
            {
                this.SetState(ButtonState.Disabled);
                return;
            }

            //update current mouse position
            this.MouseNow = Mouse.GetState();
            Point cursor = this.MouseNow.Position;
            UIScreen root = this.Root();
            if (root != Main.PrimaryScreen)
            {
                //normalize cursor position on secondary windows
                cursor += new Point(Main.PrimaryScreen.Form.Location.X - root.Form.Location.X, 
                    Main.PrimaryScreen.Form.Location.Y - root.Form.Location.Y);
            }

            //update button state
            if (this.Bounds.Contains(cursor))
            {
                if (this.MouseNow.LeftButton is Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                {
                    this.SetState(ButtonState.Pressed);
                }
                else
                {
                    if (this.MousePrev.LeftButton is Microsoft.Xna.Framework.Input.ButtonState.Pressed && this.Root().HasFocus)
                        OnClick?.Invoke(this);
                    this.SetState(ButtonState.Hovered);
                }
            }
            else
            {
                this.SetState(ButtonState.Released);
            }
            this.MousePrev = this.MouseNow;

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
                case ButtonState.Released:
                    canvas.DrawRectangle(this.Bounds, backColor, borderColor, 2, this.Layer);
                    break;
                case ButtonState.Hovered:
                    canvas.DrawRectangle(this.Bounds, backColor, borderColor * 1.25f, 2, this.Layer);
                    break;
                case ButtonState.Pressed:
                    canvas.DrawRectangle(this.Bounds, backColor * 1.25f, borderColor * 1.5f, 3, this.Layer);
                    break;
                case ButtonState.Disabled:
                    canvas.DrawRectangle(this.Bounds, backColor * 1.1f, backColor * 1.2f, 2, this.Layer);
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

            this.Enabled = Attribute(node, "enabled", true);
            this.TextBlock.HorizontalAlign = Attribute(node, "text_align", this.TextBlock.HorizontalAlign);
            this.ShowBorder = Attribute(node, "border", true);
			this.TextBlock.SetLayer(this.Layer);
        }
    }
}
