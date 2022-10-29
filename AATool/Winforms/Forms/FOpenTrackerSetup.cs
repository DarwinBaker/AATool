using System;
using System.Windows.Forms;
using AATool.Configuration;
using AATool.Net;

namespace AATool.Winforms.Forms
{
    public partial class FOpenTrackerSetup : Form
    {
        public FOpenTrackerSetup()
        {
            this.InitializeComponent();
            this.aaKey.Text = Config.Tracking.OpenTrackerKey;
            this.url.Text = Config.Tracking.OpenTrackerUrl;
        }

        private void OnClick(object sender, EventArgs e)
        {
            if (sender == this.toggleKey)
            {
                this.ToggleKey();
            }
            else if (sender == this.done)
            {
                this.Close();
            }
        }

        private void ToggleKey()
        {
            bool hide = true;
            if (this.aaKey.UseSystemPasswordChar)
            {
                //show confirmation dialog
                string message = "Are you sure you want to unmask the AAKey field?";
                string title = "AAKey Reveal Confirmation";
                DialogResult result = MessageBox.Show(this, message, title,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);
                hide = result is not DialogResult.Yes;
            }
            this.aaKey.UseSystemPasswordChar = hide;
            this.toggleKey.Text = hide
                ? "Show AAKey"
                : "Hide AAKey";
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            if (sender == this.aaKey)
            {
                this.aaKey.Text = this.aaKey.Text.Replace(AAKey.Prefix, "");
                Config.Tracking.OpenTrackerKey.Set(this.aaKey.Text);
            }
            else if (sender == this.url)
            {
                Config.Tracking.OpenTrackerUrl.Set(this.url.Text);
            }
            Config.Tracking.TrySave();
        }
    }
}
