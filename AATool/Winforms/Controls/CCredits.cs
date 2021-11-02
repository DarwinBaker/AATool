using AATool.Properties;
using AATool.Utilities;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace AATool.Winforms.Controls
{
    public partial class CCredits : UserControl
    {
        public CCredits()
        {
            InitializeComponent();
            ReadCredits();
        }

        private void ReadCredits()
        {
            //parse credits file
            try
            {
                var document = new XmlDocument();
                using (var stream = File.OpenRead(Paths.CreditsFile))
                {
                    document.Load(stream);
                    XmlNode root = document.SelectSingleNode("credits");
                    foreach (XmlNode groupNode in root.ChildNodes)
                        flow.Controls.Add(ParseGroup(groupNode));
                }
            }
            catch { }
        }

        private CCreditsGroup ParseGroup(XmlNode node)
        {
            //parse credits groups and add them
            var group = new CCreditsGroup();
            string groupName = XmlObject.ParseAttribute(node, "name", string.Empty);
            group.SetTitle(groupName);

            foreach (XmlNode userNode in node.ChildNodes)
                group.AddUser(this.ParseUser(userNode, groupName));
            return group;
        }

        private Label ParseUser(XmlNode node, string groupName)
        {
            //parse user credits entry
            Label label;
            string link = XmlObject.ParseAttribute(node, "link", string.Empty);
            if (!string.IsNullOrEmpty(link))
            {
                label = new LinkLabel {
                    Tag = link
                };
                (label as LinkLabel).LinkBehavior = LinkBehavior.HoverUnderline;
                label.Click += new EventHandler(this.OnClick);
            }
            else
            {
                label = new Label();
            }

            if (groupName is "Supporters")
            {
                string tier = XmlObject.ParseAttribute(node, "tier", "gold").ToLower();
                label.Image = tier switch {
                    "netherite" => Resources.supporter_netherite,
                    "diamond"   => Resources.supporter_diamond,
                    "emerald"   => Resources.supporter_emerald,
                    _ => Resources.supporter_gold
                };
            }
            else
            {
                label.Image = groupName switch {
                    "Beta Testers"       => Resources.supporter_beta,
                    "Special Dedication" => Resources.supporter_dedication,
                    _                    => Resources.supporter_developer
                };
            }

            label.ImageAlign = ContentAlignment.MiddleLeft;
            label.Margin = new Padding(3, 3, 0, 0);
            label.Size = new Size(120, 16);
            label.Text = new string(' ', 6) + XmlObject.ParseAttribute(node, "name", string.Empty);
            label.TextAlign = ContentAlignment.MiddleLeft;
            return label;
        }

        private void OnClick(object sender, EventArgs e)
        {
            var link = sender as LinkLabel;
            if (!string.IsNullOrEmpty(link?.Tag?.ToString()))
                Process.Start(link.Tag.ToString());
        }
    }
}
