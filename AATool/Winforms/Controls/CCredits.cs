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
            group.SetTitle(XmlObject.ParseAttribute(node, "name", string.Empty));
            foreach (XmlNode userNode in node.ChildNodes)
                group.AddUser(ParseUser(userNode));
            return group;
        }

        private Label ParseUser(XmlNode node)
        {
            //parse user credits entry
            Label label;
            string link = XmlObject.ParseAttribute(node, "link", string.Empty);
            if (!string.IsNullOrEmpty(link))
            {
                label = new LinkLabel();
                label.Tag = link;
                (label as LinkLabel).LinkBehavior = LinkBehavior.HoverUnderline;
                label.Click += new EventHandler(OnClick);
            }
            else
                label = new Label();

            label.Image = XmlObject.ParseAttribute(node, "tier", "patreon_gold") switch {
                "gold"      => Resources.supporter_patreon_gold,
                "diamond"   => Resources.supporter_patreon_diamond,
                "netherite" => Resources.supporter_patreon_netherite,
                "beta"      => Resources.supporter_beta,
                _           => Resources.supporter_developer
            };

            label.ImageAlign = ContentAlignment.MiddleLeft;
            label.Margin = new Padding(3);
            label.Size = new Size(128, 16);
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
