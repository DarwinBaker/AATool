using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace AATool.Winforms.Forms
{
    public partial class FAbout : Form
    {
        public FAbout()
        {
            this.InitializeComponent();
        }

        private void OnClick(object sender, EventArgs e)
        {
            if (sender == this.patreon)
                Process.Start("https://www.patreon.com/_ctm");
            else if (sender == this.discord)
                MessageBox.Show("My Discord tag is CTM#0001", "Discord");
            else if (sender == this.twitch)
                Process.Start("https://www.twitch.tv/CTM_256");
            else if (sender == this.youtube)
                Process.Start("https://www.youtube.com/channel/UCdJ1FnTvTpna4VGkEyJ9_NA");
        }
    }
}
