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
        public DynamicSpriteFont Font    { get; private set; }
        public string WrappedText        { get; private set; }
        public Rectangle TextBounds      { get; private set; }

        public HorizontalAlign TextAlign { get; set; }
        public Color TextColor           { get; set; }

        public bool DrawBackground;
        private StringBuilder builder;

        public bool IsEmpty                         => Font == null || builder.Length == 0;
        public override string ToString()           => builder.ToString();
        public void SetFont(string font, int size) => Font = FontSet.Get(font, size);
        public void SetTextColor(Color color)       => TextColor = color;

        public UITextBlock() : this("minecraft", 12) { }
        public UITextBlock(string font, int scale)
        {
            this.builder = new StringBuilder();
            this.SetFont(font, scale);
        }

        public void SetText(string text) 
        {
            this.builder = new StringBuilder(text);
            this.UpdateWrappedText();
        }

        public void Append(string text) 
        {
            this.builder.Append(text);
            this.UpdateWrappedText();
        }

        public void AppendLine(string text) 
        {
            this.builder.Append(text + "\n");
            this.UpdateWrappedText();
        }

        public void Clear() 
        {
            this.builder.Clear();
            this.UpdateWrappedText();
        }

        public override void DrawThis(Display display)
        {
            if (this.IsEmpty)
                return;

            string text = this.ToString();
            if (string.IsNullOrWhiteSpace(text))
                return;

            if (this.DrawBackground)
                display.DrawRectangle(new Rectangle(this.TextBounds.Left, this.TextBounds.Top, this.TextBounds.Width, this.TextBounds.Height + 4), MainSettings.Instance.BackColor);

            //get what color text should be based on root screen
            Color color = this.TextColor;
            if (this.TextColor == Color.Transparent)
                color = this.GetRootScreen() is UIMainScreen ? MainSettings.Instance.TextColor : OverlaySettings.Instance.TextColor;
            if (this.Bounds.Size == Point.Zero)
            {
                display.DrawString(this.Font, text, this.Location.ToVector2(), color);
            }
            else
            {
                //draw text wrapped and aligned in bounding box
                float y = this.Content.Top;
                foreach (string line in this.WrappedText.Split('\n'))
                {
                    var size = this.Font.MeasureString(line).ToPoint();

                    //calculate horizontal position of this line
                    int x = this.TextAlign switch
                    {
                        HorizontalAlign.Center => this.Content.Left + this.Margin.Left + ((this.Content.Width / 2) - (size.X / 2)),
                        HorizontalAlign.Left   => this.Content.Left,
                        _                      => this.Content.Right + this.Margin.Right - size.X
                    };

                    display.DrawString(this.Font, line, new Vector2(x, y), color);
                    
                    //next line
                    y += size.Y;
                }
            }
        }

        public override void DrawDebugRecursive(Display display)
        {
            base.DrawDebugRecursive(display);
            if (this.IsCollapsed)
                return;
            if (this.DebugColor != Color.Transparent)
                display.DrawRectangle(new Rectangle(this.TextBounds.Left, this.TextBounds.Top, this.TextBounds.Width, this.TextBounds.Height), DebugColor * 0.3f);
            for (int i = 0; i < this.Children.Count; i++)
                this.Children[i].DrawDebugRecursive(display);
        }

        public void UpdateWrappedText()
        {
            if (this.IsEmpty)
                return;

            var wrappedBuilder = new StringBuilder();
            float lineWidth = 0;
            float spaceWidth = this.Font.MeasureString(" ").X;

            //split text into array of words
            string[] words = this.ToString().Replace("\n", "\n ").Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                Vector2 currentSize = this.Font.MeasureString(words[i]);
                if (lineWidth + currentSize.X < this.Content.Width)
                {
                    wrappedBuilder.Append(words[i]);
                }
                else
                {
                    //text overflowed; start on new line
                    lineWidth = 0;
                    if (words.Length > 1)
                        wrappedBuilder.Append("\n" + words[i]);
                    else
                        wrappedBuilder.Append(words[i]);
                }
                    
                lineWidth += currentSize.X + spaceWidth;
                if (i < words.Length - 1)
                {
                    Vector2 nextSize = this.Font.MeasureString(words[i + 1]);
                    if (lineWidth + nextSize.X < this.Width)
                        wrappedBuilder.Append(" ");
                }
            }
            this.WrappedText = wrappedBuilder.ToString();
            var size = this.Font.MeasureString(this.WrappedText).ToPoint();

            //calculate horizontal offset of text align
            int x = this.TextAlign switch
            {
                HorizontalAlign.Center => this.Content.Left + ((this.Content.Width / 2) - (size.X / 2)),
                HorizontalAlign.Left   => this.Content.Left,
                HorizontalAlign.Right  => this.Content.Right - size.X
            };
            this.TextBounds = new Rectangle(x, this.Content.Top, size.X, size.Y);
        }

        public override void ResizeThis(Rectangle parentRectangle)
        {
            base.ResizeThis(parentRectangle);
            this.UpdateWrappedText();
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            this.SetText(ParseAttribute(node, "text", string.Empty));
            this.SetFont("minecraft", ParseAttribute(node, "font_size", 12));
            this.SetTextColor(ParseAttribute(node, "color", Color.Transparent));
            this.TextAlign = ParseAttribute(node, "text_align", this.TextAlign);
        }
    }
}
