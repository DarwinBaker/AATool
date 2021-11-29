using System;
using System.Collections.Generic;
using System.Diagnostics;
using AATool.Graphics;
using AATool.Net.Requests;
using AATool.Settings;
using AATool.UI.Controls;
using AATool.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AATool.UI.Screens
{
    public class UIUpdateScreen : UIScreen
    {
        private UITextBlock versionLabel;
        private UITextBlock titleLabel;
        private UITextBlock hasLatestLabel;

        private UIButton nowButton;
        private UIButton laterButton;
        private UIButton githubButton;
        private UIButton closeButton;

        private UIFlowPanel upgrades;
        private UIFlowPanel fixes;
        private UIFlowPanel notes;

        private UIControl thumbnailBounds;

        private List<UIPicture> textTinted;

        private bool postInstall;

        public UIUpdateScreen(Main main, bool postInstall) : base(main, GameWindow.Create(main, 700, 360))
        {
            this.Form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.postInstall = postInstall;
            
            this.ReloadLayout();
            this.ConstrainWindow();
            int x = Main.PrimaryScreen.Form.Left + (Main.PrimaryScreen.Form.ClientSize.Width / 2) - (this.Window.ClientBounds.Width / 2);
            int y = Main.PrimaryScreen.Form.Top + (Main.PrimaryScreen.Form.ClientSize.Height / 2) - (this.Window.ClientBounds.Height / 2);
            this.Form.Location = new System.Drawing.Point(x, y);
            this.Form.Icon = new System.Drawing.Icon("assets/graphics/system/aaupdate.ico");
            this.Form.Show();
        }

        public override void ReloadLayout()
        {
            //clear and load layout if window just opened or game version changed
            this.ClearControls();
            if (this.TryLoadXml("assets/ui/screens/screen_update.xml"))
            {
                this.InitializeRecursive(this);
                this.ResizeRecursive(new Rectangle(0, 0, this.Width, this.Height));
            }
        }

        protected override void ConstrainWindow()
        {
            int width  = this.Form.ClientSize.Width;
            int height = this.Form.ClientSize.Height;
            if (width is 0 || height is 0)
                return;

            //resize window and create new render target of proper size
            if (this.SwapChain is null || this.SwapChain.Width != width || this.SwapChain.Height != height)
            {
                this.Form.ClientSize = new System.Drawing.Size(width, height);
                this.SwapChain?.Dispose();
                this.SwapChain = new SwapChainRenderTarget(this.GraphicsDevice, this.Window.Handle, width, height);
                this.ResizeRecursive(new Rectangle(0, 0, width, height));
            }
        }

        public override void InitializeRecursive(UIScreen screen)
        {
            base.InitializeRecursive(screen);

            this.textTinted = new List<UIPicture>();

            this.versionLabel = this.First<UITextBlock>("version");
            this.versionLabel.SetText("AATool " + UpdateRequest.LatestVersion.ToString());
            this.titleLabel = this.First<UITextBlock>("title");
            this.titleLabel.SetText(UpdateRequest.LatestTitle);

            this.hasLatestLabel = this.First<UITextBlock>("has_latest");
            this.thumbnailBounds = this.First("thumbnail");

            //buttons
            this.nowButton = this.First<UIButton>("now");
            if (this.nowButton is not null)
                this.nowButton.OnClick += this.OnClick;
            this.laterButton = this.First<UIButton>("later");
            if (this.laterButton is not null)
                this.laterButton.OnClick += this.OnClick;
            this.githubButton = this.First<UIButton>("github");
            if (this.githubButton is not null)
                this.githubButton.OnClick += this.OnClick;
            this.closeButton = this.First<UIButton>("close");
            if (this.closeButton is not null)
                this.closeButton.OnClick += this.OnClick;

            this.upgrades = this.First<UIFlowPanel>("upgrades");
            this.fixes = this.First<UIFlowPanel>("fixes");

            this.textTinted.Add(this.nowButton.First<UIPicture>());
            this.textTinted.Add(this.laterButton.First<UIPicture>());
            this.textTinted.Add(this.githubButton.First<UIPicture>());
            this.textTinted.Add(this.closeButton.First<UIPicture>());

            foreach ((string text, string icon) in UpdateRequest.LatestUpgrades)
                this.upgrades.AddControl(this.ConstructChangelistItem(text, icon));
            foreach ((string text, string icon) in UpdateRequest.LatestFixes)
                this.fixes.AddControl(this.ConstructChangelistItem(text, icon));

            //update button visibility
            if (UpdateRequest.UpdatesAreAvailable())
            {
                this.Form.Text = $"Updates are available!";
            }
            else if (this.postInstall && !Main.IsBeta)
            {
                this.Form.Text = $"Welcome to AATool {Main.Version}!";
                this.closeButton.Expand();
                this.nowButton.Collapse();
                this.laterButton.Collapse();
            }
            else
            {
                this.hasLatestLabel.Expand();
                this.nowButton.Collapse();
                this.laterButton.Collapse();

                //update text
                if (Main.IsModded)
                {
                    this.Form.Text = $"You are running {Main.FullTitle}";
                    this.hasLatestLabel.SetText("You're on an unofficial build");
                }
                if (Main.IsBeta)
                {
                    this.Form.Text = $"You are running {Main.FullTitle}";
                    this.hasLatestLabel.SetText("You're ahead of the latest release!");
                }
                else if (Main.Version > UpdateRequest.LatestVersion)
                {
                    this.Form.Text = $"You are on a preview version of AATool ({Main.Version})";
                    this.hasLatestLabel.SetText("You're ahead of the latest release!");
                }
                else
                {
                    this.Form.Text = $"You are already on the latest version of AATool ({Main.Version})";
                    this.hasLatestLabel.SetText("Latest version already installed!");
                }
            }
        }

        private UIControl ConstructChangelistItem(string text, string icon)
        {
            var panel = new UIPanel() {
                FlexWidth = new Size(200),
                FlexHeight = new Size(16),
                Padding = new Margin(4, 0, 0, 0),
                DrawMode = DrawMode.ChildrenOnly,
            };
            var picture = new UIPicture() {
                FlexWidth = new Size(16),
                FlexHeight = new Size(16),
                Margin = new Margin(0, 0, -1, 0),
                HorizontalAlign = HorizontalAlign.Left,
                VerticalAlign = VerticalAlign.Top,
            };
            picture.SetTexture(icon);
            var label = new UITextBlock() {
                HorizontalTextAlign = HorizontalAlign.Left,
                VerticalTextAlign = VerticalAlign.Top,
                FlexHeight = new Size(16),
                Padding = new Margin(20, 0, 0, 0),
            };

            if (icon.StartsWith("bullet_"))
                this.textTinted.Add(picture);

            label.SetText(text);
            panel.AddControl(picture);
            panel.AddControl(label);
            return panel;
        }

        public override void Prepare(Display display)
        {
            base.Prepare(display);
            this.GraphicsDevice.Clear(Config.Main.BackColor);
        }

        public override void DrawThis(Display display)
        {
            var centered = new Rectangle(
                this.thumbnailBounds.Left,
                this.thumbnailBounds.Top, 
                UpdateRequest.LatestThumb.Width, 
                UpdateRequest.LatestThumb.Height);
            display.Draw(UpdateRequest.LatestThumb, centered, Color.White, Layer.Fore);
            base.DrawThis(display);
        }

        protected override void UpdateThis(Time time)
        {
            if (this.Form.IsDisposed)
                return;

            //make sure certain icons match text color
            foreach (UIPicture picture in this.textTinted)
                picture.SetTint(Config.Main.TextColor);

            base.UpdateThis(time);
        }

        private void OnClick(UIControl sender)
        {
            if (sender == this.nowButton)
                UpdateHelper.RunAAUpdate(0);
            else if (sender == this.laterButton || sender == this.closeButton)
                this.Form.Close();
            else if (sender == this.githubButton)
                _ = Process.Start(Paths.URL_GITHUB_LATEST);
        }
    }
}
