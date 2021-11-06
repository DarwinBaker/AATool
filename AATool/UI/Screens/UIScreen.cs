using System;
using System.Windows.Forms;
using System.Xml;
using AATool.Graphics;
using AATool.UI.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AATool.UI.Screens
{
    public abstract class UIScreen : UIControl
    {
        public SwapChainRenderTarget SwapChain { get; protected set; }
        public Form Form                       { get; private set; }
        public Main Main                       { get; private set; }
        public GameWindow Window               { get; private set; }
        public GraphicsDevice GraphicsDevice   { get; private set; }

        public int FormWidth     => this.Form.ClientRectangle.Width;
        public int FormHeight    => this.Form.ClientRectangle.Height;
        public bool HasFocus     => this.Form.Focused;

        public UIScreen(Main main, GameWindow window)
        {
            this.Main           = main;
            this.Window         = window;
            this.GraphicsDevice = main.GraphicsDevice;
            this.DrawMode       = DrawMode.All;
            this.Form           = Control.FromHandle(window.Handle) as Form;
            this.Form.Icon      = new System.Drawing.Icon("assets/graphics/system/icon.ico");
            this.Form.ShowIcon  = true;
        }

        public void Show() => this.Form.Show();
        public void Hide() => this.Form.Hide();

        public virtual void Dispose()
        {
            this.SwapChain?.Dispose();
            this.Form.Dispose();
        }

        public virtual void Prepare(Display display) =>  
            this.GraphicsDevice.SetRenderTarget(this.SwapChain);

        public virtual void Present(Display display) => 
            this.SwapChain?.Present();

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
            this.Content = new Rectangle(Point.Zero, parent.Size);
        }

        public override void DrawRecursive(Display display)
        {
            display.Begin();
            base.DrawRecursive(display);
            display.End();
        }

        public override void DrawDebugRecursive(Display display)
        {
            display.Begin(BlendState.AlphaBlend);
            base.DrawDebugRecursive(display);
            display.End();
        }

        protected abstract void ReloadLayout();
        protected abstract void ConstrainWindow();
    }
}
