using AATool.DataStructures;
using AATool.Settings;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace AATool.UI.Controls
{
    class UIPotionGroup : UIFlowPanel
    {
        public override void InitializeRecursive(Screen screen)
        {
            InitializeFromSourceDocument();

            foreach (var potion in ReadPotions())
            {
                var pot = new UIPotion();
                pot.Potion = potion;
                pot.ResizeRecursive(ContentRectangle);
                AddControl(pot);
            }
            base.InitializeRecursive(screen);
        }

        private IList<Potion> ReadPotions()
        {
            var potions = new List<Potion>();
            var doc = new XmlDocument();
            using (var stream = File.OpenRead(Path.Combine("assets", "potions", "potions.xml")))
            {
                doc.Load(stream);
                XmlNode root = doc.SelectSingleNode("potions");
                foreach (XmlNode potionNode in root.ChildNodes)
                    potions.Add(new Potion(potionNode));
            }
            return potions;
        }

        public override void DrawThis(Display display)
        {
            base.DrawThis(display);
            for (int i = 0; i < Children.Count - 1; i++)
            {
                var bounds = Children[i].Rectangle;
                display.DrawRectangle(new Rectangle(bounds.Left, bounds.Bottom - 4, 256, 2), MainSettings.Instance.BorderColor);
            }
        }
    }
}
