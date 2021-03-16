using System.Windows.Forms;

namespace AATool.Winforms.Controls
{
    public partial class CCreditsGroup : UserControl
    {
        public void SetTitle(string title) => label.Text = title;
        public void AddUser(Label user)    => flow.Controls.Add(user);

        public CCreditsGroup()
        {
            InitializeComponent();
        }
    }
}
