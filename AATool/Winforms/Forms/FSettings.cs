using AATool.Net;
using AATool.Settings;
using AATool.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace AATool.Winforms.Forms
{
    public partial class FSettings : Form
    {
        private bool activated;

        public FSettings()
        {
            this.InitializeComponent();

            if (!this.DesignMode)
            {
                this.LoadSettings();
            } 
        }

        public void UpdateGameVersion() => this.main.UpdateGameVersion();
        public void UpdateOverlayWidth() => this.overlay.UpdateWidth();
        public void UpdateRainbow(Color color) => this.main.UpdateRainbow(color);

        private void OnActivated(object sender, EventArgs e)
        {
            //center window on first load
            if (!this.activated)
            {
                this.CenterToParent();
                this.activated = true;
            }      
        }

        private void LoadSettings()
        {
            this.main.LoadSettings();
            this.network.LoadSettings();
            this.overlay.LoadSettings();
        }

        private void OnButtonClick(object sender, EventArgs e)
        {
            if (sender == this.done)
            {
                this.Close();
            }
            else if (sender == this.reset)
            {
                string msg = "This will clear all customized settings including themes, your custom save path, " +
                    "and co-op info. Are you sure you want to revert to the default settings?";
                DialogResult confirmation = MessageBox.Show(this, msg, "Warning!", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Warning, 
                    MessageBoxDefaultButton.Button2);

                if (confirmation == DialogResult.Yes)
                {
                    Config.ResetToDefaults();
                    this.LoadSettings();
                }
            }
            else if (sender == this.update)
            {
                UpdateHelper.CheckAsync(false);
            }
            else if (sender == this.about)
            {
                using var dialog = new FAbout();
                dialog.ShowDialog();
            }
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            Peer.UnbindController(this.network);
        }
    }
}
