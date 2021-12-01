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
        private UIButton button;
        private UIPicture icon;
        private UIPicture lockIcon;
        private bool repeat;

        private readonly Easing fade;

        public UIRefreshIcon()
        {
            this.BuildFromSourceDocument();
            this.fade = new Easing(Ease.Circular, 1.0, true, false);
        }

        public override void InitializeRecursive(UIScreen screen)
        {
            this.button = this.First<UIButton>("manual_sync");
            this.button.OnClick += this.OnClick;
            this.icon = this.First<UIPicture>("icon");
            this.lockIcon = this.First<UIPicture>("lock");
            base.InitializeRecursive(screen);
        }

        private void OnClick(UIControl sender)
        {
            if (sender == this.button)
            {
                if (Config.Tracker.UseRemoteWorld)
                {
                    SftpSave.Sync();
                }
                else
                {
                    TrackerSettings.LockWorld ^= true;
                    if (!TrackerSettings.LockWorld)
                        Tracker.Invalidate();
                }
            }

        }

        protected override void UpdateThis(Time time)
        {
            this.fade.Update(time);

            if (!Config.Tracker.UseRemoteWorld || Peer.IsClient)
            {
                //update style
                switch (Config.Main.RefreshIcon)
                {
                    case "xp_orb":
                        this.icon.SetTexture("xp_orb");
                        break;
                    case "compass":
                        this.icon.SetTexture("compass");
                        break;
                }
                
                float alpha = this.repeat
                    ? 20 * this.fade.In()
                    : 1 - this.fade.Out();
                this.icon.SetTint(ColorHelper.Fade(Color.White, alpha));

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
                this.button.Enabled = Tracker.SaveState is SaveFolderState.Valid;
                this.lockIcon.SetTexture(TrackerSettings.LockWorld ? "locked" : "");
                this.lockIcon.SetTint(ColorHelper.Fade(Config.Main.TextColor, 1 - (alpha * 4)));
            }  
            else
            {
                bool ready = SftpSave.State is SyncState.Ready;
                this.button.Enabled = ready;
                this.icon.SetTexture(ready ? "sync" : "syncing");
                this.icon.SetTint(Config.Main.TextColor);
                this.lockIcon.SetTexture("");
            }    
        }

        public override void DrawThis(Display display)
        {
            //glow effect
            if (this.icon.Texture is "xp_orb")
            {
                var rectangle = new Rectangle(
                    this.X - 8, 
                    this.Y - 8, 
                    this.Width + 16, 
                    this.Height + 16);

                display.Draw("xp_orb_glow", rectangle, this.icon.Tint, Layer.Glow);
            }
            base.DrawThis(display);
        }
    }
}
