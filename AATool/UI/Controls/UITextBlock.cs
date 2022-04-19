using System.Linq;
using System.Text;
using System.Xml;
using AATool.Configuration;
using AATool.Graphics;
using AATool.UI.Screens;
using FontStashSharp;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    public class UITextBlock : UIControl
    {
        public DynamicSpriteFont Font    { get; private set; }
        public string WrappedText        { get; private set; }
        public Rectangle TextBounds      { get; private set; }

        public Color TextColor                      { get; set; }
        public HorizontalAlign HorizontalTextAlign  { get; set; }
        public VerticalAlign   VerticalTextAlign    { get; set; }
        public bool DrawBackground                  { get; set; }

        private StringBuilder builder;

        public bool IsEmpty                         => this.Font == null || this.builder.Length == 0;
        public override string ToString()           => this.builder.ToString();
        public void SetFont(string font, int size)  => this.Font = FontSet.Get(font, size);
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

        public override void DrawThis(Canvas canvas)
        {
            if (this.IsEmpty || this.SkipDraw)
                return;

            string text = this.ToString();
            if (string.IsNullOrWhiteSpace(text))
                return;

            if (this.DrawBackground)
                canvas.DrawRectangle(new Rectangle(this.TextBounds.Left, this.TextBounds.Top, this.TextBounds.Width, this.TextBounds.Height + 4), Config.Main.BackColor);
            
            //get what color text should be based on root screen
            Color color = this.TextColor;
            if (this.TextColor == Color.Transparent)
            {
            	color = this.Root() is not UIOverlayScreen 
            		? Config.Main.TextColor 
            		: Config.Overlay.FrameStyle != "Custom Theme" ? Color.White : Config.Overlay.CustomTextColor;
            }  
                
            if (this.Bounds.Size == Point.Zero)
            {
                canvas.DrawString(this.Font, text, this.Location.ToVector2(), color, this.Layer);
            }
            else
            {
                //draw text wrapped and aligned in bounding box
                int blockHeight = this.Font.MeasureString(this.WrappedText).ToPoint().Y;
                float y = this.VerticalTextAlign switch
                {
                    VerticalAlign.Center => this.Inner.Top + this.Margin.Top + (this.Inner.Height / 2) - (blockHeight / 2),
                    VerticalAlign.Top    => this.Inner.Top,
                    _                    => this.Inner.Bottom + this.Margin.Bottom - blockHeight
                };
                foreach (string line in this.WrappedText.Split('\n'))
                {
                    var lineSize = this.Font.MeasureString(line).ToPoint();

                    //calculate horizontal position of this line
                    int x = this.HorizontalTextAlign switch
                    {
                        HorizontalAlign.Center => this.Inner.Left + this.Margin.Left + ((this.Inner.Width / 2) - (lineSize.X / 2)),
                        HorizontalAlign.Left   => this.Inner.Left,
                        _                      => this.Inner.Right + this.Margin.Right - lineSize.X
                    };

                    canvas.DrawString(this.Font, line, new Vector2(x, y), color, this.Layer);
                    
                    //next line
                    y += lineSize.Y;
                }
            }
        }

        public override void DrawDebugRecursive(Canvas canvas)
        {
            if (this.IsCollapsed || this.IsEmpty)
                return;

            base.DrawDebugRecursive(canvas);
            if (this.DebugColor != Color.Transparent)
            {
                var bounds = new Rectangle(
                    this.TextBounds.Left,
                    this.TextBounds.Top,
                    this.TextBounds.Width,
                    this.TextBounds.Height);

                canvas.DrawRectangle(bounds, this.DebugColor * 0.3f, null, 0, Layer.Fore);
            }
                
            for (int i = 0; i < this.Children.Count; i++)
                this.Children[i].DrawDebugRecursive(canvas);
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
                string word = words[i];
                Vector2 currentSize = this.Font.MeasureString(words[i]);
                if (lineWidth + currentSize.X < this.Inner.Width)
                {
                    wrappedBuilder.Append(string.IsNullOrEmpty(word) ? ' ' : word);
                }
                else
                {
                    //text overflowed; start on new line
                    lineWidth = 0;
                    if (words.Length > 1)
                        wrappedBuilder.Append("\n" + word);
                    else
                        wrappedBuilder.Append(word);
                }

                if (word.LastOrDefault() is '\n')
                    lineWidth = 0;
                else
                    lineWidth += currentSize.X + spaceWidth;

                if (i < words.Length - 1)
                {
                    if (string.IsNullOrEmpty(word))
                        continue;

                    Vector2 nextSize = this.Font.MeasureString(words[i + 1]);
                    if (word.LastOrDefault() is not '\n' && lineWidth + nextSize.X < this.Width)
                        wrappedBuilder.Append(" ");
                }
            }
            this.WrappedText = wrappedBuilder.ToString();
            var size = this.Font.MeasureString(this.WrappedText).ToPoint();

            //calculate horizontal offset of text align
            int x = this.HorizontalTextAlign switch
            {
                HorizontalAlign.Center => this.Inner.Left + ((this.Inner.Width / 2) - (size.X / 2)),
                HorizontalAlign.Left   => this.Inner.Left,
                _                      => this.Inner.Right - size.X
            };
            this.TextBounds = new Rectangle(x, this.Inner.Top, size.X, size.Y);
            if (this.Root() is UIMainScreen && this.Layer is Layer.Main)
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
            this.SetText(Attribute(node, "text", string.Empty));
            this.SetFont("minecraft", Attribute(node, "font_size", 12));
            this.SetTextColor(Attribute(node, "color", Color.Transparent));
            this.HorizontalTextAlign = Attribute(node, "text_align", this.HorizontalTextAlign);
            this.VerticalTextAlign   = Attribute(node, "text_align", VerticalAlign.Top);

            this.HorizontalTextAlign = Attribute(node, "h_text_align", this.HorizontalTextAlign);
            this.VerticalTextAlign   = Attribute(node, "v_text_align", this.VerticalTextAlign);
        }
    }
}
