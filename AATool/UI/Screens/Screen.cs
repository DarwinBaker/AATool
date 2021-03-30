using AATool.Trackers;
using AATool.UI.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Windows.Forms;
using System.Xml;

namespace AATool.UI.Screens
{
    public abstract class Screen : UIControl
    {
        public Form Form;
        public Main Main;
        public GameWindow Window;
        public SwapChainRenderTarget SwapChain;
        public GraphicsDevice GraphicsDevice;
        public AdvancementTracker AdvancementTracker;
        public AchievementTracker AchievementTracker;
        public StatisticsTracker StatisticsTracker;

        public bool HasFocus => Main?.IsActive ?? false;

        public Screen(Main main, GameWindow window, int width, int height)
        {
            Main = main;
            Window = window;
            GraphicsDevice = main.GraphicsDevice;
            AdvancementTracker = main.AdvancementTracker;
            AchievementTracker = main.AchievementTracker;
            StatisticsTracker  = main.StatisticsTracker;
            Form = Control.FromHandle(window.Handle) as Form;
            Form.Resize += OnResize;
            DrawMode = DrawMode.All;

            //only create swap chain if this is a secondary window
            if (window != main.Window)
                SetWindowSize(width, height);
            ResizeThis(new Rectangle(0, 0, width, height));
        }
       
        public void Show() => Form?.Show();
        public void Hide() => Form?.Hide();
        private void OnResize(object sender, System.EventArgs e) => SetWindowSize(Form.ClientSize.Width, Form.ClientSize.Height);
        public override void MoveTo(Point point) => Form.Location = new System.Drawing.Point(point.X, point.Y);
        public virtual void Prepare(Display display) => GraphicsDevice.SetRenderTarget(SwapChain);
        public virtual void Present(Display display) => SwapChain?.Present();

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

        public virtual void SetWindowSize(int width, int height)
        {
            //if window size doesn't match, resize it and create new render target of proper size
            if (width > 0 && height > 0)
            {
                if (SwapChain == null || width != SwapChain.Bounds.Width || height != SwapChain.Bounds.Height)
                {
                    Form.ClientSize = new System.Drawing.Size(width, height);
                    SwapChain?.Dispose();
                    SwapChain = new SwapChainRenderTarget(GraphicsDevice, Window.Handle, width, height);
                    ResizeRecursive(new Rectangle(0, 0, width, height));
                }
            }
        }

        public override void ResizeThis(Rectangle parentRectangle)
        {
            Size = parentRectangle.Size;
            ContentRectangle = new Rectangle(0, 0, Size.X, Size.Y);
        }

        public override void ReadDocument(XmlDocument document)
        {
            base.ReadDocument(document);
            foreach (var control in Children)
                control.InitializeRecursive(this);
            ResizeRecursive(Rectangle);
        }
    }
}
