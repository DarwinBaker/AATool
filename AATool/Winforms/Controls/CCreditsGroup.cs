using System.Windows.Forms;

namespace AATool.Winforms.Controls
{
    public partial class CCreditsGroup : UserControl
    {
        public void SetTitle(string title) => this.label.Text = title;
        public void AddUser(Label user) => this.flow.Controls.Add(user);

        public CCreditsGroup()
        {
            this.InitializeComponent();
        }
    }
}
