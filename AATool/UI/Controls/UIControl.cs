using AATool.Graphics;
using AATool.UI.Screens;
using AATool.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace AATool.UI.Controls
{
    public abstract class UIControl : XmlObject
    {
        //static dictionary to hold all control types for dynamic instantiation
        private static readonly Dictionary<string, Type> Types = (
            from t in Assembly.GetExecutingAssembly().GetTypes()
            where t.IsClass && t.Namespace == "AATool.UI.Controls"
            select t).ToDictionary(t => t.Name.ToString().ToLower(),
            t => t, StringComparer.OrdinalIgnoreCase);

        //static dictionary to hold source documents for initializing controls
        private static Dictionary<Type, XmlDocument> SourceDocs;

        //static dictionary to hold colors for drawing debug layout
        private static readonly Dictionary<Type, Color> DebugColors = new () {
            { typeof(UIFlowPanel),          Color.Blue },
            { typeof(UIGrid),               Color.Blue },
            { typeof(UIAchievementTree),    Color.Violet },
            { typeof(UIAdvancementGroup),   Color.Red },
            { typeof(UITextBlock),          Color.Yellow },
            { typeof(UIButton),             Color.Orange },
            { typeof(UIPicture),            Color.Lime },
            { typeof(UICriterion),          Color.Magenta },
            { typeof(UIEnchantmentTable),   Color.Lime },
            { typeof(UIProgressBar),        Color.Magenta }
        };

        private UIScreen rootScreen;

        public Margin Margin    { get; set; } = new (0, SizeMode.Absolute);
        public Margin Padding   { get; set; } = new (0, SizeMode.Absolute);
        public Size FlexWidth   { get; set; } = new (1, SizeMode.Relative);
        public Size FlexHeight  { get; set; } = new (1, SizeMode.Relative);
        public Size MinWidth    { get; set; } = new (0, SizeMode.Relative);
        public Size MinHeight   { get; set; } = new (0, SizeMode.Relative);
        public Size MaxWidth    { get; set; } = new (1, SizeMode.Relative);
        public Size MaxHeight   { get; set; } = new (1, SizeMode.Relative);

        public string Name      { get; set; } = string.Empty;
        public object Tag       { get; set; } = null;
        public int Row          { get; set; } = 0;
        public int Column       { get; set; } = 0;
        public int RowSpan      { get; set; } = 1;
        public int ColumnSpan   { get; set; } = 1;
        public bool IsSquare    { get; set; } = false;
        public bool IsCollapsed { get; private set; } = false;

        public List<UIControl> Children { get; protected set; }
        public UIControl Parent { get; protected set; }
        public Rectangle Bounds { get; protected set; }
        public Rectangle Inner  { get; protected set; }

        public HorizontalAlign HorizontalAlign = HorizontalAlign.Center;
        public VerticalAlign VerticalAlign = VerticalAlign.Center;
        public DrawMode DrawMode = DrawMode.All;
        public Layer Layer = Layer.Main;

        public Point Location => this.Bounds.Location;
        public Point Center => this.Bounds.Center;
        public Point Size => this.Bounds.Size;
        public int Top    => this.Y;
        public int Bottom => this.Y + this.Height;
        public int Left   => this.X;
        public int Right  => this.X + this.Width;

        public int X {
            get => this.Bounds.X;
            set => this.Bounds = new Rectangle(value, this.Y, this.Width, this.Height);
        }

        public int Y {
            get => this.Bounds.Y;
            set => this.Bounds = new Rectangle(this.X, value, this.Width, this.Height);
        }

        public int Width {
            get => this.Bounds.Width;
            set => this.Bounds = new Rectangle(this.X, this.Y, value, this.Height);
        }

        public int Height {
            get => this.Bounds.Height;
            set => this.Bounds = new Rectangle(this.X, this.Y, this.Width, value);
        }

        public bool SkipDraw => this.IsCollapsed || 
            (this.Root() is UIMainScreen && this.Layer is Layer.Main && !UIMainScreen.RefreshingCache);

        public Color DebugColor => DebugColors.TryGetValue(this.GetType(),
            out Color val) ? val : Color.Transparent;

        public virtual void MoveTo(Point point)  => 
            this.ResizeRecursive(new Rectangle(point, this.Size));

        public virtual void MoveBy(Point point)  => 
            this.ResizeRecursive(new Rectangle(this.Location + point, this.Size));

        public virtual void ScaleTo(Point point) => 
            this.ResizeRecursive(new Rectangle(this.Location, point));

        public virtual void Expand()
        {
            if (this.IsCollapsed && this.Layer is Layer.Main && this.Root() is UIMainScreen)
                UIMainScreen.Invalidate();
            this.IsCollapsed = false;
        }

        public virtual void Collapse()
        {
            if (!this.IsCollapsed && this.Layer is Layer.Main && this.Root() is UIMainScreen)
                UIMainScreen.Invalidate();
            this.IsCollapsed = true;
        }

        public UIScreen Root()
        {
            //if not yet cached, recursively search tree for root screen and cache
            this.rootScreen ??= this as UIScreen ?? this.Parent?.Root();
            return this.rootScreen;
        }

        public UIControl()
        {
            this.Children = new List<UIControl>();
        }

        public void BuildFromTemplate()
        {
            //attempt to construct control from template xml file
            SourceDocs ??= new Dictionary<Type, XmlDocument>();
            Type type = this.GetType();
            if (!SourceDocs.ContainsKey(type))
            {
                //this control type doesn't have a cached template; try and load one
                try
                {
                    var newTemplate = new XmlDocument();
                    foreach (string file in Directory.GetFiles(Paths.System.TemplatesFolder, "*.xml"))
                    {
                        string name = Path.GetFileNameWithoutExtension(file).Replace("_", string.Empty);
                        if (name == "control" + type.Name.ToLower().Substring("ui".Length))
                        {
                            newTemplate.Load(file);
                            SourceDocs[type] = newTemplate;
                            break;
                        }
                    }
                }
                catch { }
            }          
            if (SourceDocs.TryGetValue(type, out XmlDocument template))
                this.ReadNode(template.SelectSingleNode("control"));
        }

        public static UIControl CreateInstance(string type)
        {
            //if type name is valid, create instance of type
            return Types.TryGetValue(type, out Type realType) 
                ? Activator.CreateInstance(realType) as UIControl 
                : null;
        }

        public void ParentTo(UIControl parent) => this.Parent = parent;

        public void SetLayer(Layer layer)
        {
            if (this.Layer != layer && this.Layer is Layer.Main && this.Root() is UIMainScreen)
                UIMainScreen.Invalidate();
            this.Layer = layer;
        }

        public virtual void InitializeRecursive(UIScreen screen)
        {
            this.InitializeThis(screen);
            foreach (UIControl control in this.Children)
                control.InitializeRecursive(screen);
        }

        public virtual void InitializeThis(UIScreen screen) { }

        public void ClearControls()
        {
            while (this.Children.Count > 0)
                this.RemoveControl(this.Children[0]);
        }

        public virtual void AddControl(UIControl control)
        {
            if (control is not null && !this.Children.Contains(control))
            {
                this.Children.Add(control);
                control.ParentTo(this);
            }
        }

        public virtual void RemoveControl(UIControl control)
        {
            this.Children.Remove(control);
            control?.ParentTo(null);
        }

        public virtual void ResizeRecursive(Rectangle rectangle)
        {
            this.ResizeThis(rectangle);
            this.ResizeChildren();
        }

        public virtual void ResizeThis(Rectangle parent)
        {
            //scale all relative values to parent rectangle
            this.Margin.Resize(parent.Size);
            this.FlexWidth.Resize(parent.Width);
            this.FlexHeight.Resize(parent.Height);
            this.MaxWidth.Resize(parent.Width);
            this.MinWidth.Resize(parent.Width);
            this.MaxHeight.Resize(parent.Height);
            this.MinHeight.Resize(parent.Height);

            //clamp size to min and max
            this.Width = MathHelper.Clamp(this.FlexWidth, this.MinWidth, this.MaxWidth);
            this.Height = MathHelper.Clamp(this.FlexHeight, this.MinHeight, this.MaxHeight);

            //if control is square, conform both width and height to the larger of the two
            if (this.IsSquare)
                this.Width = this.Height = Math.Min(this.Width, this.Height);

            this.X = this.HorizontalAlign switch {
                HorizontalAlign.Center => parent.X + (parent.Width / 2) - (this.Width / 2),
                HorizontalAlign.Left => parent.Left + this.Margin.Left,
                _ => parent.Right - this.Width - this.Margin.Right
            };

            this.Y = this.VerticalAlign switch {
                VerticalAlign.Center => parent.Top + (parent.Height / 2) - (this.Height / 2),
                VerticalAlign.Top => parent.Top + this.Margin.Top,
                _ => parent.Bottom - this.Height - this.Margin.Bottom
            }; 

            this.Padding.Resize(this.Size);

            //calculate internal rectangle
            this.Inner = new Rectangle(
                this.X + this.Padding.Left, 
                this.Y + this.Padding.Top, 
                this.Width - this.Padding.Horizontal, 
                this.Height - this.Padding.Vertical);
        }

        public virtual void ResizeChildren()
        {
            foreach (UIControl child in this.Children) 
                child.ResizeRecursive(this.Inner);
        }

        public UIControl First(string name) => this.First<UIControl>(name);

        public T First<T>(string name = null) where T : UIControl
        {
            //search child controls (depth-first)
            foreach (UIControl child in this.Children)
            {
                if (child is T control)
                {
                    if (name is null || control.Name == name)
                        return control;
                }

                //recursively search child's children
                control = child.First<T>(name);
                if (control is not null)
                    return control;
            }
            return null;
        }

        public void GetTreeRecursive(List<UIControl> children)
        {
            children.Add(this);
            foreach (UIControl child in this.Children)
                child.GetTreeRecursive(children);
        }

        protected virtual void UpdateThis(Time time) { }
        public virtual void UpdateRecursive(Time time)
        {
            if (this.IsCollapsed) 
                return;

            this.UpdateThis(time);
            foreach (UIControl control in this.Children) 
                control.UpdateRecursive(time);
        }

        public virtual void DrawThis(Canvas canvas)  { }
        public virtual void DrawRecursive(Canvas canvas)
        {
            if (this.IsCollapsed) 
                return;

            if (this.DrawMode is DrawMode.All || this.DrawMode is DrawMode.ThisOnly)
                this.DrawThis(canvas);

            if (this.DrawMode is DrawMode.All || this.DrawMode is DrawMode.ChildrenOnly)
            {
                foreach (UIControl control in this.Children)
                    control.DrawRecursive(canvas);
            }
        }

        public virtual void DrawDebugRecursive(Canvas canvas)
        {
            if (this.IsCollapsed)
                return;
            if (this.DebugColor != Color.Transparent)
            {
                //fill
                canvas.DrawRectangle(this.Bounds, ColorHelper.Fade(this.DebugColor, 0.1f), null, 0, Layer.Fore);

                //edges
                canvas.DrawRectangle(new Rectangle(this.Bounds.Left, this.Bounds.Top, this.Bounds.Width, 1),
                    ColorHelper.Fade(this.DebugColor, 0.8f), null, 0, Layer.Fore);
                canvas.DrawRectangle(new Rectangle(this.Bounds.Right - 1, this.Bounds.Top + 1, 1, this.Bounds.Height - 2),
                    ColorHelper.Fade(this.DebugColor, 0.8f), null, 0, Layer.Fore);
                canvas.DrawRectangle(new Rectangle(this.Bounds.Left + 1, this.Bounds.Bottom - 1, this.Bounds.Width - 1, 1),
                    ColorHelper.Fade(this.DebugColor, 0.8f), null, 0, Layer.Fore);
                canvas.DrawRectangle(new Rectangle(this.Bounds.Left, this.Bounds.Top + 1, 1, this.Bounds.Height - 1),
                    ColorHelper.Fade(this.DebugColor, 0.8f), null, 0, Layer.Fore);
            }
            for (int i = 0; i < this.Children.Count; i++)
                this.Children[i].DrawDebugRecursive(canvas);
        }

        public override void ReadNode(XmlNode node)
        {
            if (node is null)
                return;

            //properties
            this.Name            = Attribute(node, "name",             this.Name);
            this.FlexWidth       = Attribute(node, "width",            this.FlexWidth);
            this.FlexHeight      = Attribute(node, "height",           this.FlexHeight);
            this.MaxWidth        = Attribute(node, "max_width",        this.MaxWidth);
            this.MaxHeight       = Attribute(node, "max_height",       this.MaxHeight);
            this.MinWidth        = Attribute(node, "min_width",        this.MinWidth);
            this.MinHeight       = Attribute(node, "min_height",       this.MinHeight);
            this.Row             = Attribute(node, "row",              this.Row);
            this.Column          = Attribute(node, "col",              this.Column);
            this.RowSpan         = Attribute(node, "rowspan",          this.RowSpan);
            this.ColumnSpan      = Attribute(node, "colspan",          this.ColumnSpan);
            this.IsCollapsed     = Attribute(node, "collapsed",        this.IsCollapsed);
            this.IsSquare        = Attribute(node, "square",           this.IsSquare);
            this.DrawMode        = Attribute(node, "draw_mode",        this.DrawMode);
            this.VerticalAlign   = Attribute(node, "vertical_align",   this.VerticalAlign);
            this.HorizontalAlign = Attribute(node, "horizontal_align", this.HorizontalAlign);
			
			this.SetLayer(Attribute(node, "layer", this.Layer));

            //margin
            if (node.Attributes["margin"] is not null)
            {
                this.Margin = Attribute(node, "margin", this.Margin);
            }
            else
            {
                Size left   = Attribute(node, "margin_left",   new Size());
                Size right  = Attribute(node, "margin_right",  new Size());
                Size top    = Attribute(node, "margin_top",    new Size());
                Size bottom = Attribute(node, "margin_bottom", new Size());
                this.Margin = new Margin(left, right, top, bottom);
            }

            //padding
            if (node.Attributes["padding"] is not null)
            {
                this.Padding = Attribute(node, "padding", this.Padding);
            }
            else
            {
                Size left    = Attribute(node, "padding_left",   new Size());
                Size right   = Attribute(node, "padding_right",  new Size());
                Size top     = Attribute(node, "padding_top",    new Size());
                Size bottom  = Attribute(node, "padding_bottom", new Size());
                this.Padding = new Margin(left, right, top, bottom);
            }

            foreach (XmlNode subNode in node.ChildNodes)
            {
                //skip "rows" and "columns" elements, as they aren't controls
                if (subNode.Name is "rows" or "columns")
                    continue;

                UIControl subControl = CreateInstance("ui" + subNode.Name.Replace("_", string.Empty));
                subControl?.ReadNode(subNode);
                this.AddControl(subControl);
            }
        }

        public override void ReadDocument(XmlDocument document)
        {
            if (document?.DocumentElement is null)
                return;

            this.ReadNode(document.DocumentElement);
            this.Width      = Attribute(document.DocumentElement, "width", 640);
            this.Height     = Attribute(document.DocumentElement, "height", 360);
            this.FlexWidth  = new Size(this.Width, SizeMode.Absolute);
            this.FlexHeight = new Size(this.Height, SizeMode.Absolute);

            if (document.DocumentElement.Attributes["padding"] is not null)
            {
                this.Padding = Attribute(document.DocumentElement, "padding", this.Padding);
            }
            else
            {
                Size left    = Attribute(document.DocumentElement, "padding_left",   new Size());
                Size right   = Attribute(document.DocumentElement, "padding_right",  new Size());
                Size top     = Attribute(document.DocumentElement, "padding_top",    new Size());
                Size bottom  = Attribute(document.DocumentElement, "padding_bottom", new Size());
                this.Padding = new Margin(left, right, top, bottom);
            }
        }
    }
}
