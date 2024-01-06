using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using AATool.Data;
using AATool.Properties;

namespace AATool.Winforms.Controls
{
    public partial class CCreditsGroup : UserControl
    {
        readonly List<Control> pendingNetherite = new();
        readonly List<Control> pendingDiamond = new();
        readonly List<Control> pendingGold = new();

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

            label.Image = person.HighestRole switch {
                "developer" => Resources.supporter_developer,
                "beta testers" => Resources.supporter_beta,
                "special dedication" => Resources.supporter_dedication,
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

            if (person.HighestRole is "supporter_netherite")
            {
                this.pendingNetherite.Add(label);
            }
            else if (person.HighestRole is "supporter_diamond")
            {
                this.pendingDiamond.Add(label);
            }
            else if (person.HighestRole is "supporter_gold")
            {
                this.pendingGold.Add(label);
            }
            else
            {
                this.flow.Controls.Add(label);
            }
        }

        public void PopulateSupporters()
        {
            foreach (Control control in this.pendingNetherite)
                this.flow.Controls.Add(control);
            this.pendingNetherite.Clear();

            foreach (Control control in this.pendingDiamond)
                this.flow.Controls.Add(control);
            this.pendingDiamond.Clear();

            foreach (Control control in this.pendingGold)
                this.flow.Controls.Add(control);
            this.pendingGold.Clear();
        }

        private void OnClick(object sender, EventArgs e)
        {
            var link = sender as LinkLabel;
            if (!string.IsNullOrEmpty(link?.Tag?.ToString()))
                Process.Start(link.Tag.ToString());
        }
    }
}
