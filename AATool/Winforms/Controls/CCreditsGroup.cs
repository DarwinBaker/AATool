using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using AATool.Data;
using AATool.Properties;

namespace AATool.Winforms.Controls
{
    public partial class CCreditsGroup : UserControl
    {
        public void SetTitle(string title) => this.label.Text = title;

        public CCreditsGroup()
        {
            this.InitializeComponent();
        }

        public void Add(Credit person)
        {
            Label label;
            if (string.IsNullOrEmpty(person.Link))
            {
                label = new Label();
            }
            else
            {
                label = new LinkLabel {
                    Tag = person.Link,
                    LinkBehavior = LinkBehavior.HoverUnderline,
                };
                label.Click += new EventHandler(this.OnClick);
            }

            label.Image = person.Role switch {
                "developer" => Resources.supporter_developer,
                "beta_tester" => Resources.supporter_beta,
                "dedication" => Resources.supporter_dedication,
                "supporter_netherite" => Resources.supporter_netherite,
                "supporter_emerald" => Resources.supporter_emerald,
                "supporter_diamond" => Resources.supporter_diamond,
                _ => Resources.supporter_gold,
            };
            label.Text = new string(' ', 6) + person.Name;
            label.TextAlign = ContentAlignment.MiddleLeft;
            label.ImageAlign = ContentAlignment.MiddleLeft;
            label.Margin = new Padding(3, 3, 0, 0);
            label.Size = new Size(120, 16);
            this.flow.Controls.Add(label);
        }

        private void OnClick(object sender, EventArgs e)
        {
            var link = sender as LinkLabel;
            if (!string.IsNullOrEmpty(link?.Tag?.ToString()))
                Process.Start(link.Tag.ToString());
        }
    }
}
