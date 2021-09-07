using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace AATool.Winforms.Forms
{
    public partial class FUpdate : Form
    {
        public FUpdate(Version version, string patch)
        {
            this.InitializeComponent();
            this.Text = "Updates are Available (" + version + ")";
            this.patchNotes.Text = patch.Trim();
            this.patchNotes.SelectionProtected = true;
            this.icon.Image = Image.FromFile("assets/graphics/sprites/gif/nether_portal.gif");
            this.icon.Enabled = true;
        }

        private void OnClick(object sender, EventArgs e)
        {
            if (sender == this.browser)
                Process.Start("https://github.com/DarwinBaker/AATool/releases/latest");
        }
    }
}
