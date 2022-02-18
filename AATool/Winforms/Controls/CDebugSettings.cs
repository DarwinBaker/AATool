using System;
using System.Windows.Forms;
using AATool.Configuration;
using AATool.Graphics;

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
            this.loaded = true;
        }

        private void SaveSettings()
        {
            if (this.loaded)
            {
                Config.Main.LayoutDebugMode.Set(this.layoutDebug.Checked);
                Config.Main.CacheDebugMode.Set(this.cacheDebug.Checked);
                Config.Main.HideRenderCache.Set(this.hideRenderCache.Checked);
                Config.Main.Save();
            }
        }

        private void OnClicked(object sender, EventArgs e)
        {
            if (sender == this.dumpAtlas)
                SpriteSheet.DumpAtlas();
            this.SaveSettings();
        }

        private void OnCheckChanged(object sender, EventArgs e) => this.SaveSettings();
    }
}
