using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using AATool.Configuration;
using AATool.Graphics;
using AATool.UI.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AATool.UI.Screens
{
    public abstract class UIScreen : UIControl
    {
        public SwapChainRenderTarget Target { get; protected set; }
        public Form Form                       { get; private set; }
        public Main Main                       { get; private set; }
        public GameWindow Window               { get; private set; }
        public GraphicsDevice GraphicsDevice   { get; private set; }

        public int FormWidth  => this.Form.ClientRectangle.Width;
        public int FormHeight => this.Form.ClientRectangle.Height;
        public bool HasFocus  => this.Form.Focused;

        public abstract Color FrameBackColor();
        public abstract Color FrameBorderColor();

        public readonly Canvas Canvas = new ();

        protected bool Positioned;

        public UIScreen(Main main, GameWindow window)
        {
            this.Main           = main;
            this.Window         = window;
            this.GraphicsDevice = main.GraphicsDevice;
            this.DrawMode       = DrawMode.All;
            this.Form           = Control.FromHandle(window.Handle) as Form;
            this.Form.Icon      = new System.Drawing.Icon(Paths.System.MainIcon);
            this.Form.ShowIcon  = true;
        }

        public void Show() => this.Form.Show();
        public void Hide() => this.Form.Hide();

        public void SetIcon(string name)
        {
            try
            {
                this.Form.Icon = new System.Drawing.Icon(
                Path.Combine(Paths.System.AssetsFolder, "icons", $"{name}.ico"));
            }
            catch
            { 
                //couldn't change icon, probably file missing. move on
            }
        }

        public abstract string GetCurrentView();
        public abstract void ReloadView();
        protected abstract void ConstrainWindow();

        public virtual void Dispose()
        {
            this.Target?.Dispose();
            this.Form.Dispose();
        }

        public virtual void Prepare() =>  
            this.GraphicsDevice.SetRenderTarget(this.Target);

        public void Render() => this.DrawRecursive(this.Canvas);

        public virtual void Present() => 
            this.Target?.Present();

        public override void MoveTo(Point point) =>
            this.Form.Location = new System.Drawing.Point(point.X, point.Y);

        public override void MoveBy(Point point) =>
            this.Form.Location = new System.Drawing.Point(this.Form.Location.X + point.X, this.Form.Location.Y + point.Y);

        public override void ScaleTo(Point point)
        {
            this.Form.ClientSize = new System.Drawing.Size(point.X, point.Y);
        }

        public override void ResizeThis(Rectangle parent)
        {
            this.Bounds  = new Rectangle(this.Bounds.Location, parent.Size);
            this.Inner = new Rectangle(Point.Zero, parent.Size);
        }

        public override void DrawRecursive(Canvas canvas)
        {
            this.Canvas.BeginDraw(this);
            base.DrawRecursive(this.Canvas);
            if (Config.Main.LayoutDebugMode)
                this.DrawDebugRecursive(this.Canvas);
            this.Canvas.EndDraw(this);
        }

        public override void DrawDebugRecursive(Canvas canvas)
        {
            for (int i = 0; i < this.Children.Count; i++)
                this.Children[i].DrawDebugRecursive(canvas);
        }

        protected void PositionWindow(WindowSnap snap, int monitor, Point lastPosition)
        {
            int monitorCount = Screen.AllScreens.Length;
            int displayIndex = MathHelper.Clamp(monitor - 1, 0, monitorCount);
            System.Drawing.Rectangle desktop = Screen.AllScreens[displayIndex].WorkingArea;

            System.Drawing.Point point = snap switch {
                WindowSnap.Remember => new (lastPosition.X, lastPosition.Y),
                WindowSnap.Centered => new (desktop.X + ((desktop.Width  - this.Form.Width)  / 2), (desktop.Height - this.Form.Height) / 2),
                WindowSnap.TopLeft => new (desktop.Left, desktop.Top),
                WindowSnap.TopRight => new (desktop.Right - this.Form.Width, desktop.Top),
                WindowSnap.BottomLeft => new(desktop.Left, desktop.Bottom - this.Form.Height),
                WindowSnap.BottomRight => new(desktop.Right - this.Form.Width, desktop.Bottom - this.Form.Height),
                _ => this.Form.Location
            };

            this.Form.Location = point;

            //make sure window is visible on screen
            if (!Screen.AllScreens.Any(screen => screen.WorkingArea.IntersectsWith(this.Form.Bounds)))
                this.Form.Location = new(desktop.X + ((desktop.Width  - this.Form.Width)  / 2), (desktop.Height - this.Form.Height) / 2);
        }
    }
}