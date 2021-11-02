using System.Xml;
using AATool.Graphics;
using AATool.Settings;
using AATool.UI.Screens;
using AATool.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AATool.UI.Controls
{
    public class UIButton : UIPanel
    {
        public delegate void ClickEventHandler(UIControl sender);
        public event ClickEventHandler OnClick;

        public bool Enabled          { get; set; }
        public UIButtonState State   { get; private set; }
        public UITextBlock TextBlock { get; private set; }

        public bool UseCustomColor;
        public bool ShowBorder;

        private MouseState mouseNow;
        private MouseState mousePrev;

        public void SetText(string text) => this.TextBlock?.SetText(text);
        public void SetTextColor(Color color) => this.TextBlock?.SetTextColor(color);
        private void SetState(UIButtonState newState) => this.State = newState;

        public UIButton()
        {
            this.Enabled = true;
            this.ShowBorder = true;
            this.TextBlock = new UITextBlock();
            this.AddControl(this.TextBlock);
        }

        public override void InitializeRecursive(UIScreen screen)
        {
            this.TextBlock.FlexHeight = new Size(13, SizeMode.Absolute);
            base.InitializeRecursive(screen);
        }

        protected override void UpdateThis(Time time)
        {
            if (!this.Enabled)
            {
                this.SetState(UIButtonState.Disabled);
                return;
            }

            //update current mouse position
            this.mouseNow = Mouse.GetState();
            Point cursor = this.mouseNow.Position;
            UIScreen root = this.GetRootScreen();
            if (root != Main.PrimaryScreen)
            {
                //normalize cursor position on secondary windows
                cursor += new Point(Main.PrimaryScreen.Form.Location.X - root.Form.Location.X, 
                    Main.PrimaryScreen.Form.Location.Y - root.Form.Location.Y);
            }

            //update button state
            if (this.Bounds.Contains(cursor))
            {
                if (this.mouseNow.LeftButton is ButtonState.Pressed)
                {
                    this.SetState(UIButtonState.Pressed);
                }
                else
                {
                    if (this.mousePrev.LeftButton is ButtonState.Pressed && this.GetRootScreen().HasFocus)
                        OnClick?.Invoke(this);
                    this.SetState(UIButtonState.Hovered);
                }
            }
            else
            {
                this.SetState(UIButtonState.Released);
            }
            this.mousePrev = this.mouseNow;
        }

        public override void DrawThis(Display display)
        {
            Color backColor   = this.UseCustomColor ? this.BackColor   : Config.Main.BackColor;
            Color borderColor = this.UseCustomColor ? this.BorderColor : Config.Main.BorderColor;
            if (!this.ShowBorder)
                borderColor = backColor;

            switch (this.State)
            {
                case UIButtonState.Released:
                    display.DrawRectangle(this.Bounds, backColor, borderColor, 2);
                    break;
                case UIButtonState.Hovered:
                    display.DrawRectangle(this.Bounds, backColor, borderColor * 1.25f, 2);
                    break;
                case UIButtonState.Pressed:
                    display.DrawRectangle(this.Bounds, backColor * 1.25f, borderColor * 1.5f, 3);
                    break;
                case UIButtonState.Disabled:
                    display.DrawRectangle(this.Bounds, backColor * 1.1f, backColor * 1.2f, 2);
                    break;
            }
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            string text = ParseAttribute(node, "text", string.Empty);
            if (!string.IsNullOrEmpty(text))
                this.TextBlock?.SetText(text);

            int width = ParseAttribute(node, "text_width", 0);
            if (width > 0)
                this.TextBlock.FlexWidth = new Size(width);

            this.Enabled = ParseAttribute(node, "enabled", true);
            this.TextBlock.HorizontalAlign = ParseAttribute(node, "text_align", this.TextBlock.HorizontalAlign);
            this.ShowBorder = ParseAttribute(node, "border", true);
        }
    }
}
