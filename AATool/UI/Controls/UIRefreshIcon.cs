using AATool.Data;
using AATool.Graphics;
using AATool.Net;
using AATool.Saves;
using AATool.Settings;
using AATool.UI.Screens;
using AATool.Utilities;
using AATool.Utilities.Easings;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Xml;

namespace AATool.UI.Controls
{
    class UIRefreshIcon : UIControl
    {
        private UIButton syncButton;
        private UIPicture syncIcon;
        private UIPicture layer1;
        private UIPicture layer2;
        private bool repeat;

        private readonly Easing fade;

        public UIRefreshIcon()
        {
            this.BuildFromSourceDocument();
            this.fade = new Easing(Ease.Circular, 1.0, true, false);
        }

        public override void InitializeRecursive(UIScreen screen)
        {
            this.syncButton = this.First<UIButton>("manual_sync");
            this.syncButton.OnClick += this.OnClick;
            this.syncIcon = this.syncButton.First<UIPicture>();
            this.layer1 = this.First<UIPicture>("layer1");
            this.layer2 = this.First<UIPicture>("layer2");

            base.InitializeRecursive(screen);
        }

        private void OnClick(UIControl sender)
        {
            if (sender == this.syncButton)
                SftpSave.Sync();
        }

        private void SetStates(bool layer1, bool layer2)
        {
            if (layer1)
                this.layer1.Expand();
            else
                this.layer1.Collapse();

            if (layer2)
                this.layer2.Expand();
            else
                this.layer2.Collapse();
        }

        private void SetLayers(Layer layer1, Layer layer2)
        {
            this.layer1.SetLayer(layer1);
            this.layer2.SetLayer(layer2);
        }

        private void SetTextures(string layer1, string layer2)
        {
            this.layer1.SetTexture(layer1);
            this.layer2.SetTexture(layer2);
        }

        private void SetTints(Color layer1, Color layer2)
        {
            this.layer1.SetTint(layer1);
            this.layer2.SetTint(layer2);
        }

        protected override void UpdateThis(Time time)
        {
            this.fade.Update(time);

            if (!Config.Tracker.UseRemoteWorld || Peer.IsClient)
            {
                this.layer1.Expand();
                this.layer2.Expand();
                this.syncButton.Collapse();

                //update style
                switch (Config.Main.RefreshIcon)
                {
                    case "xp_orb":
                        this.UpdateAsXpOrb();
                        break;
                    case "compass":
                        this.UpdateAsCompass();
                        break;
                }

                //update state
                if (Tracker.Invalidated || Config.Main.ValueChanged(MainSettings.REFRESH_ICON))
                {
                    this.fade.Reset();
                    this.repeat = true;
                }
                if (this.fade.IsExpired && this.repeat)
                {
                    this.fade.Reset();
                    this.repeat = false;
                }
            }  
            else
            {
                //hide refresh icon and show sync button
                this.layer1.Collapse();
                this.layer2.Collapse();
                this.syncButton.Expand();

                bool ready = SftpSave.State is SyncState.Ready;
                this.syncButton.Enabled = ready;
                this.syncIcon.SetTexture(ready ? "sync" : "syncing");
                this.syncIcon.SetTint(Config.Main.TextColor);
            }    
        }

        private void UpdateAsXpOrb()
        {
            this.SetStates(false, true);
            this.SetLayers(Layer.Main, Layer.Fore);
            this.SetTextures(string.Empty, "xp_orb");

            float alpha = this.repeat
                ? 20 * this.fade.In()
                : 1 - this.fade.Out();

            this.SetTints(Color.White, ColorHelper.Fade(Color.White, alpha));
        }

        private void UpdateAsCompass()
        {
            this.SetStates(true, true);
            this.SetLayers(Layer.Fore, Layer.Fore);
            this.SetTextures("compass_empty", "compass_needle");

            //always keep compass somewhat visible
            float alpha = this.repeat
                ? (2 * this.fade.In()) + 0.5f
                : (2 * (1 - this.fade.Out())) + 0.5f;

            Color tint = this.repeat
                ? Color.White * (alpha - 0.5f) * 2
                : Color.White * (alpha - 0.5f) * 8;

            this.SetTints(ColorHelper.Fade(Color.White, 2.5f * alpha), tint);
        }

        public override void DrawThis(Display display)
        {
            //glow effect
            if (Config.Main.RefreshIcon is "xp_orb" && !this.layer2.IsCollapsed)
            {
                Rectangle rectangle = new (this.X - 8, this.Y - 8, this.Width + 16, this.Height + 16);
                display.Draw("xp_orb_glow", rectangle, this.layer2.Tint, Layer.Glow);
            }
            base.DrawThis(display);
        }
    }
}
