using System;
using System.Diagnostics;
using System.Windows.Forms;
using AATool.Data;

namespace AATool.Winforms.Controls
{
    public partial class CCredits : UserControl
    {
        public CCredits()
        {
            this.InitializeComponent();
            this.Populate();
            this.developer.SetTitle("Developer");
            this.testers.SetTitle("Beta Testers");
            this.dedication.SetTitle("Special Dedication");
            this.supporters.SetTitle("Supporters");
        }

        private void Populate()
        {
            foreach (Credit credit in Credits.Special)
            {
                switch (credit.HighestRole)
                {
                    case "developer":
                        this.developer.Add(credit);
                        break;
                    case "beta testers":
                        this.testers.Add(credit);
                        break;
                    case "special dedication":
                        this.dedication.Add(credit);
                        break;
                    default:
                        this.supporters.Add(credit);
                        break;
                }
            }

            foreach (Credit credit in Credits.All)
                this.supporters.Add(credit);
            this.supporters.PopulateSupporters();
        }

        private void OnClick(object sender, EventArgs e)
        {
            var link = sender as LinkLabel;
            if (!string.IsNullOrEmpty(link?.Tag?.ToString()))
                Process.Start(link.Tag.ToString());
        }
    }
}
