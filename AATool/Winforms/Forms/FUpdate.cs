using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace AATool.Winforms.Forms
{
    public partial class FUpdate : Form
    {
        public FUpdate(string version, string patch)
        {
            InitializeComponent();
            Text = "Updates are Available (" + version + ")";
            patchNotes.Text = patch.Replace("\n", "\n\n").Trim();
            patchNotes.SelectionProtected = true;
            icon.Image = Image.FromFile("assets/graphics/sprites/gif/nether_portal.gif");
            icon.Enabled = true;
        }

        private void OnClick(object sender, EventArgs e)
        {
            if (sender == browser)
                Process.Start("https://github.com/DarwinBaker/AATool/releases/latest");
        }
    }
}
