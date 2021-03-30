using AATool.Settings;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Xml;

namespace AATool.UI.Controls
{
    public class UIButton : UIPanel
    {
        public delegate void ClickEventHandler(object sender);
        public event ClickEventHandler Click;

        public bool UseCustomColor;

        private UITextBlock textBlock;
        private UIButtonState state;
        private MouseState mouseNow;
        private MouseState mousePrev;

        public void SetTextColor(Color color) => textBlock?.SetTextColor(color);
        private void SetState(UIButtonState newState) => state = newState;

        public UIButton()
        {
            textBlock = new UITextBlock();
            AddControl(textBlock);
        }

        public override void InitializeRecursive(Screen screen)
        {

            textBlock.FlexHeight = new Size(13, SizeMode.Absolute);
            textBlock.VerticalAlign = VerticalAlign.Center;

            base.InitializeRecursive(screen);
        }

        protected override void UpdateThis(Time time)
        {
            //update button state
            mouseNow = Mouse.GetState();
            if (Rectangle.Contains(mouseNow.Position))
            {
                if (mouseNow.LeftButton == ButtonState.Pressed)
                    SetState(UIButtonState.Pressed);
                else
                {
                    if (mousePrev.LeftButton == ButtonState.Pressed && (GetRootScreen()?.HasFocus ?? false))
                        Click(this);
                    SetState(UIButtonState.Hovered);
                }
            }
            else
                SetState(UIButtonState.Released);
            mousePrev = mouseNow;
        }

        public override void DrawThis(Display display)
        {
            if (UseCustomColor)
            {
                switch (state)
                {
                    case UIButtonState.Released:
                        display.DrawRectangle(Rectangle, BackColor, BorderColor, 2);
                        break;
                    case UIButtonState.Hovered:
                        display.DrawRectangle(Rectangle, BackColor, BorderColor * 1.25f, 2);
                        break;
                    case UIButtonState.Pressed:
                        display.DrawRectangle(Rectangle, BackColor * 1.25f, BorderColor * 1.5f, 3);
                        break;
                }
            }
            else
            {
                switch (state)
                {
                    case UIButtonState.Released:
                        display.DrawRectangle(Rectangle, MainSettings.Instance.BackColor,         MainSettings.Instance.BorderColor, 2);
                        break;
                    case UIButtonState.Hovered:
                        display.DrawRectangle(Rectangle, MainSettings.Instance.BackColor * 1.25f, MainSettings.Instance.BorderColor, 2);
                        break;
                    case UIButtonState.Pressed:
                        display.DrawRectangle(Rectangle, MainSettings.Instance.BackColor * 1.25f, MainSettings.Instance.BorderColor * 1.5f, 3);
                        break;
                }
            }
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            string text = ParseAttribute(node, "text", string.Empty);
            if (!string.IsNullOrEmpty(text))
                textBlock?.SetText(text);
        }
    }
}
