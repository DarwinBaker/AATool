using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace AATool.Winforms.Forms
{
    public partial class FAbout : Form
    {
        public FAbout()
        {
            InitializeComponent();
        }

        private void OnClick(object sender, EventArgs e)
        {
            if (sender == patreon)
                Process.Start("https://www.patreon.com/_ctm");
            else if (sender == discord)
                Process.Start("https://discordapp.com/users/190165309309583360");
            else if (sender == twitch)
                Process.Start("https://www.twitch.tv/CTM_256");
            else if (sender == youtube)
                Process.Start("https://www.youtube.com/channel/UCdJ1FnTvTpna4VGkEyJ9_NA");
        }
    }
}
