using AATool.Graphics;
using AATool.Settings;
using AATool.UI.Screens;
using FontStashSharp;
using Microsoft.Xna.Framework;
using System.Text;
using System.Xml;

namespace AATool.UI.Controls
{
    public class UITextBlock : UIControl
    {
        public DynamicSpriteFont Font;
        public string WrappedText { get; private set; }
        public bool IsEmpty => Font == null || builder.Length == 0;
        public override string ToString() => builder.ToString();

        private StringBuilder builder;

        public UITextBlock() : this("minecraft", 12) { }
        public UITextBlock(string font, int scale)
        {
            builder = new StringBuilder();
            SetFont(font, scale);
        }

        public void SetFont(string font, int scale)
        {
            Font = FontSet.Get(font, scale);
        }

        public void SetText(string text) 
        { 
            builder = new StringBuilder(text);
            UpdateWrappedText();
        }

        public void Append(string text) 
        {
            builder.Append(text);
            UpdateWrappedText();
        }

        public void AppendLine(string text) 
        { 
            builder.Append(text + "\n");
            UpdateWrappedText();
        }

        public void Clear() 
        {
            builder.Clear();
            UpdateWrappedText();
        }

        public override void DrawThis(Display display)
        {
            if (IsEmpty)
                return;

            string text = ToString();
            if (string.IsNullOrWhiteSpace(text))
                return;

            //get what color text should be based on root screen
            var color = GetRootScreen() is MainScreen ? MainSettings.Instance.TextColor : OverlaySettings.Instance.TextColor;
            if (Rectangle.Size == Point.Zero)
                display.DrawString(Font, text, Location.ToVector2(), color);
            else
            {
                //draw text wrapped and aligned in bounding box
                var bounds = Rectangle;
                float y = bounds.Top;
                foreach (var line in WrappedText.Split('\n'))
                {
                    string trimmed = line.Trim();
                    var size = Font.MeasureString(trimmed).ToPoint();
                    int x = HorizontalAlign switch
                    {
                        HorizontalAlign.Center => (bounds.Left + (bounds.Width / 2 - size.X / 2)),
                        HorizontalAlign.Left => bounds.Left,
                        _ => bounds.Right - size.X
                    };

                    display.DrawString(Font, trimmed, new Vector2(x, y), color);
                    
                    //next line
                    y += size.Y;
                }
            }
        }

        public void UpdateWrappedText()
        {
            if (IsEmpty)
                return;

            var wrappedBuilder = new StringBuilder();
            float lineWidth = 0;
            float spaceWidth = Font.MeasureString(" ").X;

            //split text into array of words
            string[] words = ToString().Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                Vector2 currentSize = Font.MeasureString(words[i]);
                if (lineWidth + currentSize.X < Width)
                    wrappedBuilder.Append(words[i]);
                else
                {
                    //text overflowed; start on new line
                    lineWidth = 0;
                    wrappedBuilder.Append("\n" + words[i]);
                }
                    
                lineWidth += currentSize.X + spaceWidth;
                if (i < words.Length - 1)
                {
                    Vector2 nextSize = Font.MeasureString(words[i + 1]);
                    if (lineWidth + nextSize.X < Width)
                        wrappedBuilder.Append(" ");
                }
            }
            WrappedText = wrappedBuilder.ToString();
        }

        public override void ResizeThis(Rectangle parentRectangle)
        {
            base.ResizeThis(parentRectangle);
            UpdateWrappedText();
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            string text = ParseAttribute(node, "text", "");
            if (!string.IsNullOrEmpty(text))
                SetText(text);
        }
    }
}
