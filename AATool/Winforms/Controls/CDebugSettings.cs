using System;
using System.Windows.Forms;
using AATool.Graphics;
using AATool.Settings;

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
            this.layoutDebug.Checked = Config.Main.LayoutDebug;
            this.cacheDebug.Checked = Config.Main.CacheDebug;
            this.loaded = true;
        }

        private void SaveSettings()
        {
            if (!this.loaded)
                return;

            Config.Main.LayoutDebug = this.layoutDebug.Checked;
            Config.Main.CacheDebug = this.cacheDebug.Checked;
            Config.Main.Save();
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
