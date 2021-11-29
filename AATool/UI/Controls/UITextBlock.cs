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
        public DynamicSpriteFont Font { get; private set; }
        public string WrappedText { get; private set; }
        public Rectangle TextBounds { get; private set; }

        public Color TextColor { get; set; }
        public HorizontalAlign HorizontalTextAlign { get; set; }
        public VerticalAlign VerticalTextAlign { get; set; }
        public bool DrawBackground { get; set; }

        private StringBuilder builder;

        public bool IsEmpty => Font == null || builder.Length == 0;
        public override string ToString() => builder.ToString();
        public void SetFont(string font, int size) => Font = FontSet.Get(font, size);
        public void SetTextColor(Color color)
        {
            if (this.TextColor != color && this.Root() is UIMainScreen)
                UIMainScreen.Invalidate();
            this.TextColor = color;
        }

        private string rawValue;

        public UITextBlock() : this("minecraft", 12) { }
        public UITextBlock(string font, int scale)
        {
            this.builder = new StringBuilder();
            this.SetFont(font, scale);
        }

        public override void Expand()
        {
            if (this.IsCollapsed)
            {
                base.Expand();
                if (this.Root() is UIMainScreen)
                    UIMainScreen.Invalidate();
            }
            
        }

        public override void Collapse()
        {
            if (!this.IsCollapsed)
            {
                base.Collapse();
                if (this.Root() is UIMainScreen)
                    UIMainScreen.Invalidate();
            }
        }

        public override void InitializeRecursive(UIScreen screen)
        {
            base.InitializeRecursive(screen);
        }

        public void SetText(string text) 
        {
            if (text != this.rawValue)
            {
                this.rawValue = text;
                this.builder  = new StringBuilder(text);
                this.UpdateWrappedText();
            }
        }

        public void Append(string text) 
        {
            this.rawValue += text;
            this.builder.Append(text);
            this.UpdateWrappedText();
        }

        public void AppendLine(string text)
        {
            this.rawValue += text + "\n";
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
            if (this.IsEmpty || this.SkipDraw)
                return;

            string text = this.ToString();
            if (string.IsNullOrWhiteSpace(text))
                return;

            if (this.DrawBackground)
                display.DrawRectangle(new Rectangle(this.TextBounds.Left, this.TextBounds.Top, this.TextBounds.Width, this.TextBounds.Height + 4), MainSettings.Instance.BackColor);
            
            //get what color text should be based on root screen
            Color color = this.TextColor;
            if (this.TextColor == Color.Transparent)
                color = this.Root() is not UIOverlayScreen ? MainSettings.Instance.TextColor : OverlaySettings.Instance.TextColor;
            if (this.Bounds.Size == Point.Zero)
            {
                display.DrawString(this.Font, text, this.Location.ToVector2(), color, this.Layer);
            }
            else
            {
                //draw text wrapped and aligned in bounding box
                int blockHeight = this.Font.MeasureString(this.WrappedText).ToPoint().Y;
                float y = this.VerticalTextAlign switch
                {
                    VerticalAlign.Center => this.Content.Top + this.Margin.Top + (this.Content.Height / 2) - (blockHeight / 2),
                    VerticalAlign.Top    => this.Content.Top,
                    _                    => this.Content.Bottom + this.Margin.Bottom - blockHeight
                };
                foreach (string line in this.WrappedText.Split('\n'))
                {
                    var lineSize = this.Font.MeasureString(line).ToPoint();

                    //calculate horizontal position of this line
                    int x = this.HorizontalTextAlign switch
                    {
                        HorizontalAlign.Center => this.Content.Left + this.Margin.Left + ((this.Content.Width / 2) - (lineSize.X / 2)),
                        HorizontalAlign.Left   => this.Content.Left,
                        _                      => this.Content.Right + this.Margin.Right - lineSize.X
                    };

                    display.DrawString(this.Font, line, new Vector2(x, y), color, this.Layer);
                    
                    //next line
                    y += lineSize.Y;
                }
            }
        }

        public override void DrawDebugRecursive(Display display)
        {
            if (this.IsCollapsed || this.IsEmpty)
                return;

            base.DrawDebugRecursive(display);
            if (this.DebugColor != Color.Transparent)
            {
                var bounds = new Rectangle(
                    this.TextBounds.Left,
                    this.TextBounds.Top,
                    this.TextBounds.Width,
                    this.TextBounds.Height);

                display.DrawRectangle(bounds, this.DebugColor * 0.3f, null, 0, Layer.Fore);
            }
                
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
                    if (string.IsNullOrEmpty(words[i]))
                        wrappedBuilder.Append(' ');
                    else
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
                    if (string.IsNullOrEmpty(words[i]))
                        continue;

                    Vector2 nextSize = this.Font.MeasureString(words[i + 1]);
                    if (words[i][0] is not '\n' && lineWidth + nextSize.X < this.Width)
                        wrappedBuilder.Append(" ");
                }
            }
            this.WrappedText = wrappedBuilder.ToString();
            var size = this.Font.MeasureString(this.WrappedText).ToPoint();

            //calculate horizontal offset of text align
            int x = this.HorizontalTextAlign switch
            {
                HorizontalAlign.Center => this.Content.Left + ((this.Content.Width / 2) - (size.X / 2)),
                HorizontalAlign.Left   => this.Content.Left,
                _                      => this.Content.Right - size.X
            };
            this.TextBounds = new Rectangle(x, this.Content.Top, size.X, size.Y);
            if (this.Root() is UIMainScreen)
                UIMainScreen.Invalidate();
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
            this.HorizontalTextAlign = ParseAttribute(node, "text_align", this.HorizontalTextAlign);
            this.VerticalTextAlign   = ParseAttribute(node, "text_align", VerticalAlign.Top);

            this.HorizontalTextAlign = ParseAttribute(node, "h_text_align", this.HorizontalTextAlign);
            this.VerticalTextAlign   = ParseAttribute(node, "v_text_align", this.VerticalTextAlign);
        }
    }
}
