using AATool.UI.Screens;
using AATool.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace AATool.UI.Controls
{
    public abstract class UIControl : XmlObject
    {
        //static dictionary to hold all control types for dynamic instantiation
        private static Dictionary<string, Type> types;

        //static dictionary to hold source documents for initializing controls
        private static Dictionary<Type, XmlDocument> sourceDocs;

        //static dictionary to hold colors for drawing debug layout
        private static Dictionary<Type, Color> debugColors = new Dictionary<Type, Color>()
        {

            { typeof(UIFlowPanel),   Color.Blue },
            { typeof(UIGrid),        Color.Blue },
            { typeof(UITextBlock),   Color.Yellow },
            { typeof(UIPicture),     Color.Lime },
            { typeof(UIAdvancement), Color.Red },
            { typeof(UICriterion),   Color.Magenta }
        };

        public List<UIControl> Children     { get; protected set; }
        public UIControl Parent             { get; protected set; }
        public Rectangle ContentRectangle   { get; protected set; }

        public Point Location;
        public Point Size;

        public Margin Margin    = new Margin(0, SizeMode.Absolute);
        public Margin Padding   = new Margin(0, SizeMode.Absolute);
        public Size FlexWidth   = new Size(1, SizeMode.Relative);
        public Size FlexHeight  = new Size(1, SizeMode.Relative);
        public Size MaxWidth    = new Size(1, SizeMode.Relative);     
        public Size MinWidth    = new Size(0, SizeMode.Relative);
        public Size MaxHeight   = new Size(1, SizeMode.Relative);
        public Size MinHeight   = new Size(0, SizeMode.Relative);

        public HorizontalAlign HorizontalAlign = HorizontalAlign.Center;
        public VerticalAlign VerticalAlign     = VerticalAlign.Center;
        public DrawMode DrawMode               = DrawMode.All;

        public string Name      = string.Empty;
        public int Row          = 0;
        public int Column       = 0;
        public int RowSpan      = 1;
        public int ColumnSpan   = 1;
        public bool IsClickable = true;
        public bool IsCollapsed = false;
        public bool IsSquare    = false;

        public Rectangle Rectangle  => new Rectangle(Location, Size);
        public Point Center         => new Point(Location.X + Size.X / 2, Location.Y + Size.Y / 2);
        public int X          { get => Location.X; set => Location.X = value; }
        public int Y          { get => Location.Y; set => Location.Y = value; }
        public int Width      { get => Size.X; set => Size.X = value; }
        public int Height     { get => Size.Y; set => Size.Y = value; }
        public int Top        => Y;
        public int Bottom     => Y + Height;
        public int Left       => X;
        public int Right      => X + Width;

        public Screen GetRootScreen()            => this as Screen ?? Parent.GetRootScreen();
        public Color DebugColor                  => debugColors.TryGetValue(GetType(), out var val) ? val : Color.Transparent;
        public void Expand()                     => IsCollapsed = false;
        public void Collapse()                   => IsCollapsed = true;
        public void ParentTo(UIControl parent)   => Parent = parent;
        public virtual void MoveTo(Point point)  => ResizeRecursive(new Rectangle(point, Size));
        public virtual void MoveBy(Point point)  => ResizeRecursive(new Rectangle(Location + point, Size));
        public virtual void ScaleTo(Point point) => ResizeRecursive(new Rectangle(Location, point));

        public UIControl()
        {
            Children = new List<UIControl>();
        }

        public void InitializeFromSourceDocument()
        {
            //attempt to construct control from blueprint xml file
            sourceDocs ??= new Dictionary<Type, XmlDocument>();
            var type = GetType();
            if (!sourceDocs.ContainsKey(type))
            {
                //this control type doesn't have a cached blueprint; try and load one
                try
                {
                    var newDocument = new XmlDocument();
                    foreach (var file in Directory.GetFiles(Paths.DIR_UI, "*.xml"))
                    {
                        if (Path.GetFileNameWithoutExtension(file).Replace("_", string.Empty) == "control" + type.Name.ToLower().Substring(2))
                        {
                            newDocument.Load(file);
                            sourceDocs[type] = newDocument;
                        }
                    }
                }
                catch { }
            }          
            if (sourceDocs.TryGetValue(type, out XmlDocument document))
                ReadNode(document.SelectSingleNode("control"));
        }

        public static UIControl CreateInstance(string type)
        {
            //if static type list is null, create and populate with all control types
            types ??= (from t in Assembly.GetExecutingAssembly().GetTypes()
                       where t.IsClass && t.Namespace == "AATool.UI.Controls"
                       select t).ToDictionary(t => t.Name.ToString().ToLower(), t => t, StringComparer.OrdinalIgnoreCase);

            //if type name is valid, create instance of type
            if (types.TryGetValue(type, out Type realType))
                return Activator.CreateInstance(realType) as UIControl;
            return null;
        }

        public virtual void InitializeRecursive(Screen screen)
        {
            foreach (var control in Children)
                control.InitializeRecursive(screen);
        }

        public virtual void AddControl(UIControl control)
        {
            if (control != null && !Children.Contains(control))
            {
                Children.Add(control);
                control.ParentTo(this);
            }
        }

        public virtual void RemoveControl(UIControl control)
        {
            Children.Remove(control);
        }

        public virtual void ResizeRecursive(Rectangle rectangle)
        {
            ResizeThis(rectangle);
            ResizeChildren();
        }

        public virtual void ResizeThis(Rectangle parentRectangle)
        {
            //scale all relative values to parent rectangle
            Margin.Resize(parentRectangle.Size);
            FlexWidth.Resize(parentRectangle.Width);
            FlexHeight.Resize(parentRectangle.Height);
            MaxWidth.Resize(parentRectangle.Width);
            MinWidth.Resize(parentRectangle.Width);
            MaxHeight.Resize(parentRectangle.Height);
            MinHeight.Resize(parentRectangle.Height);

            //clamp size to min and max
            Width = MathHelper.Clamp(FlexWidth.Absolute, MinWidth.Absolute, MaxWidth.Absolute);
            Height = MathHelper.Clamp(FlexHeight.Absolute, MinHeight.Absolute, MaxHeight.Absolute);

            //if control is square, conform both width and height to the larger of the two
            if (IsSquare) Width = Height = Math.Min(Width, Height);

            X = HorizontalAlign switch {
                HorizontalAlign.Center => (parentRectangle.X + (parentRectangle.Width / 2) - (Width / 2)),
                HorizontalAlign.Left => parentRectangle.Left + Margin.Left,
                _ => parentRectangle.Right - Width - Margin.Right
            };

            Y = VerticalAlign switch {
                VerticalAlign.Center => (parentRectangle.Top + parentRectangle.Height / 2 - Height / 2),
                VerticalAlign.Top => parentRectangle.Top + Margin.Top,
                _ => parentRectangle.Bottom - Height - Margin.Bottom
            };

            Padding.Resize(Size);

            //calculate internal rectangle
            ContentRectangle = new Rectangle(X + Margin.Left + Padding.Left, Y + Margin.Top + Padding.Top, Width - Padding.Horizontal, Height - Padding.Vertical);
        }

        public virtual void ResizeChildren()
        {
            foreach (var control in Children) 
                control.ResizeRecursive(ContentRectangle);
        }

        public UIControl GetControlByName(string name, bool recursive = false)
        {
            //search child controls for control with name
            foreach (var control in Children)
            {
                if (control.Name == name) 
                    return control;
                else if (recursive)
                {
                    UIControl subControl = control.GetControlByName(name, true);
                    if (subControl != null) 
                        return subControl;
                }
            }
            return null;
        }

        public UIControl GetFirstOfType(Type type, bool recursive = false)
        {
            //search child controls for first control of type
            foreach (var control in Children)
            {
                if (control.GetType() == type) 
                    return control;
                else if (recursive)
                {
                    UIControl subControl = control.GetFirstOfType(type, true);
                    if (subControl != null) 
                        return subControl;
                }
            }
            return null;
        }

        protected virtual void UpdateThis(Time time) { }
        public void UpdateRecursive(Time time)
        {
            if (IsCollapsed) 
                return;

            UpdateThis(time);
            foreach (var control in Children) 
                control.UpdateRecursive(time);
        }

        public virtual void DrawThis(Display display)  { }
        public virtual void DrawRecursive(Display display)
        {
            if (IsCollapsed) 
                return;
            if (DrawMode == DrawMode.All || DrawMode == DrawMode.ThisOnly) 
                DrawThis(display);
            if (DrawMode == DrawMode.All || DrawMode == DrawMode.ChildrenOnly)
                for (int i = 0; i < Children.Count; i++)
                    Children[i].DrawRecursive(display);
        }

        public virtual void DrawDebugRecursive(Display display)
        {
            if (IsCollapsed)
                return;
            if (DebugColor != Color.Transparent)
            {
                display.DrawRectangle(new Rectangle(ContentRectangle.Left, ContentRectangle.Top, ContentRectangle.Width, 1), DebugColor * 0.8f);
                display.DrawRectangle(new Rectangle(ContentRectangle.Right - 1, ContentRectangle.Top + 1, 1, ContentRectangle.Height - 2), DebugColor * 0.8f);
                display.DrawRectangle(new Rectangle(ContentRectangle.Left + 1, ContentRectangle.Bottom - 1, ContentRectangle.Width - 1, 1), DebugColor * 0.8f);
                display.DrawRectangle(new Rectangle(ContentRectangle.Left, ContentRectangle.Top + 1, 1, ContentRectangle.Height - 1), DebugColor * 0.8f);
                display.DrawRectangle(new Rectangle(ContentRectangle.Left, ContentRectangle.Top, ContentRectangle.Width, ContentRectangle.Height), DebugColor * 0.2f);
            }
            for (int i = 0; i < Children.Count; i++)
                Children[i].DrawDebugRecursive(display);
        }

        public override void ReadNode(XmlNode node)
        {
            if (node == null)
                return;

            Name            = ParseAttribute(node, "name", Name);
            FlexWidth       = ParseAttribute(node, "width", FlexWidth);
            FlexHeight      = ParseAttribute(node, "height", FlexHeight);
            MaxWidth        = ParseAttribute(node, "max_width", MaxWidth);
            MaxHeight       = ParseAttribute(node, "max_height", MaxHeight);
            MinWidth        = ParseAttribute(node, "min_width", MinWidth);
            MinHeight       = ParseAttribute(node, "min_height", MinHeight);
            Row             = ParseAttribute(node, "row", Row);
            Column          = ParseAttribute(node, "col", Column);
            RowSpan         = ParseAttribute(node, "rowspan", RowSpan);
            ColumnSpan      = ParseAttribute(node, "colspan", ColumnSpan);
            IsCollapsed     = ParseAttribute(node, "collapsed", IsCollapsed);
            IsSquare        = ParseAttribute(node, "square", IsSquare);
            DrawMode        = ParseAttribute(node, "draw_mode", DrawMode);
            VerticalAlign   = ParseAttribute(node, "vertical_align", VerticalAlign);
            HorizontalAlign = ParseAttribute(node, "horizontal_align", HorizontalAlign);


            if (node.Attributes["margin"] != null)
                Margin = ParseAttribute(node, "margin", Margin);
            else
            {
                Size left   = ParseAttribute(node, "margin_left",   new Size());
                Size right  = ParseAttribute(node, "margin_right",  new Size());
                Size top    = ParseAttribute(node, "margin_top",    new Size());
                Size bottom = ParseAttribute(node, "margin_bottom", new Size());
                Margin      = new Margin(left, right, top, bottom);
            }

            if (node.Attributes["padding"] != null)
                Padding = ParseAttribute(node, "padding", Padding);
            else
            {
                Size left   = ParseAttribute(node, "padding_left",   new Size());
                Size right  = ParseAttribute(node, "padding_right",  new Size());
                Size top    = ParseAttribute(node, "padding_top",    new Size());
                Size bottom = ParseAttribute(node, "padding_bottom", new Size());
                Padding     = new Margin(left, right, top, bottom);
            }

            foreach (XmlNode subNode in node.ChildNodes)
            {
                //skip "rows" and "columns" elements, as they aren't controls
                if (subNode.Name == "rows" || subNode.Name == "columns")
                    continue;

                UIControl subControl = CreateInstance("ui" + subNode.Name.Replace("_", string.Empty));
                subControl?.ReadNode(subNode);
                AddControl(subControl);
            }
        }

        public override void ReadDocument(XmlDocument document)
        {
            if (document == null)
                return;

            XmlNode root = document.SelectSingleNode("control");
            if (root == null)
                return;

            ReadNode(root);
            Width = ParseAttribute(root, "width", 640);
            Height = ParseAttribute(root, "height", 360);
            FlexWidth = new Size(Width, SizeMode.Absolute);
            FlexHeight = new Size(Height, SizeMode.Absolute);

            if (root.Attributes["padding"] != null)
                Padding = ParseAttribute(root, "padding", Padding);
            else
            {
                Size left   = ParseAttribute(root, "padding_left",   new Size());
                Size right  = ParseAttribute(root, "padding_right",  new Size());
                Size top    = ParseAttribute(root, "padding_top",    new Size());
                Size bottom = ParseAttribute(root, "padding_bottom", new Size());
                Padding     = new Margin(left, right, top, bottom);
            }
        }
    }
}
