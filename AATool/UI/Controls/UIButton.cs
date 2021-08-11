using AATool.Graphics;
using AATool.Settings;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Xml;

namespace AATool.UI.Controls
{
    public class UIButton : UIPanel
    {
        public delegate void ClickEventHandler(UIControl sender);
        public event ClickEventHandler OnClick;

        public UITextBlock TextBlock { get; private set; }

        public bool UseCustomColor;

        private UIButtonState state;
        private MouseState mouseNow;
        private MouseState mousePrev;

        public void SetText(string text)              => this.TextBlock?.SetText(text);
        public void SetTextColor(Color color)         => this.TextBlock?.SetTextColor(color);
        private void SetState(UIButtonState newState) => this.state = newState;

        public UIButton()
        {
            this.TextBlock = new UITextBlock();
            this.AddControl(this.TextBlock);
        }

        public override void InitializeRecursive(UIScreen screen)
        {
            this.TextBlock.FlexHeight = new Size(13, SizeMode.Absolute);
            this.TextBlock.VerticalAlign = VerticalAlign.Center;
            this.TextBlock.TextAlign = HorizontalAlign.Center;
            base.InitializeRecursive(screen);
        }

        protected override void UpdateThis(Time time)
        {
            //update button state
            this.mouseNow = Mouse.GetState();
            if (this.Bounds.Contains(this.mouseNow.Position))
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
            if (this.UseCustomColor)
            {
                switch (this.state)
                {
                    case UIButtonState.Released:
                        display.DrawRectangle(this.Bounds, this.BackColor, this.BorderColor, 2);
                        break;
                    case UIButtonState.Hovered:
                        display.DrawRectangle(this.Bounds, this.BackColor, this.BorderColor * 1.25f, 2);
                        break;
                    case UIButtonState.Pressed:
                        display.DrawRectangle(this.Bounds, this.BackColor * 1.25f, this.BorderColor * 1.5f, 3);
                        break;
                }
            }
            else
            {
                switch (this.state)
                {
                    case UIButtonState.Released:
                        display.DrawRectangle(this.Bounds, Config.Main.BackColor, Config.Main.BorderColor, 2);
                        break;
                    case UIButtonState.Hovered:
                        display.DrawRectangle(this.Bounds, Config.Main.BackColor * 1.25f, Config.Main.BorderColor, 2);
                        break;
                    case UIButtonState.Pressed:
                        display.DrawRectangle(this.Bounds, Config.Main.BackColor * 1.25f, Config.Main.BorderColor * 1.5f, 3);
                        break;
                }
            }
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            string text = ParseAttribute(node, "text", string.Empty);
            if (!string.IsNullOrEmpty(text))
                this.TextBlock?.SetText(text);
        }
    }
}
