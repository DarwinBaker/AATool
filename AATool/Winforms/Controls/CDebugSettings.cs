using System;
using System.Windows.Forms;
using AATool.Configuration;
using AATool.Data.Speedrunning;
using AATool.Graphics;
using AATool.Net;
using AATool.Net.Requests;
using AATool.UI.Screens;

namespace AATool.Winforms.Controls
{
    public partial class CDebugSettings : UserControl
    {
        private bool loaded;

        public CDebugSettings()
        {
            this.InitializeComponent();
        }

        public void LoadSettings()
        {
            this.loaded = false;
            this.layoutDebug.Checked = Config.Main.LayoutDebugMode;
            this.cacheDebug.Checked = Config.Main.CacheDebugMode;
            this.hideRenderCache.Checked = Config.Main.HideRenderCache;
            this.hideLighting.Checked = Config.Main.HideGlowEffects;
            this.loaded = true;
        }

        private void SaveSettings()
        {
            if (this.loaded)
            {
                Config.Main.LayoutDebugMode.Set(this.layoutDebug.Checked);
                Config.Main.CacheDebugMode.Set(this.cacheDebug.Checked);
                Config.Main.HideRenderCache.Set(this.hideRenderCache.Checked);
                Config.Main.HideGlowEffects.Set(this.hideLighting.Checked);
                Config.Main.TrySave();
            }
        }

        private void OnClicked(object sender, EventArgs e)
        {
            if (sender == this.dumpAtlas)
                SpriteSheet.DumpAtlas();
            else if (sender == this.refreshLeaderboards)
            {
                Leaderboard.Refresh();
                UIMainScreen.ForceLayoutRefresh();
            }           
            this.SaveSettings();
        }

        private void OnCheckChanged(object sender, EventArgs e) => this.SaveSettings();

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (sender == this.fastForward)
            {
                Main.OverlayScreen.FastForwarding = true;
            }
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (sender == this.fastForward)
            {
                Main.OverlayScreen.FastForwarding = false;
            }
        }

        private void OnMouseLeave(object sender, EventArgs e)
        {
            if (sender == this.fastForward)
            {
                Main.OverlayScreen.FastForwarding = false;
            }
        }
    }
}
