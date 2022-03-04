using System;
using System.Collections.Generic;
using System.Xml;
using AATool.Configuration;
using AATool.Data;
using AATool.Graphics;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    class UIPotionGroup : UIFlowPanel
    {
        public UIPotionGroup()
        {
            this.BuildFromTemplate();
        }

        public override void InitializeThis(UIScreen screen)
        {
            foreach (Potion potion in ReadPotions())
                this.AddControl(new UIPotion() { Potion = potion });
        }

        public override void DrawThis(Canvas canvas)
        {
            if (this.SkipDraw)
                return;

            base.DrawThis(canvas);
            for (int i = 0; i < this.Children.Count - 1; i++)
            {
                Rectangle bounds = this.Children[i].Bounds;
                var splitter = new Rectangle(bounds.Left, bounds.Bottom - 6, bounds.Width, 2);
                canvas.DrawRectangle(splitter, Config.Main.BorderColor);
            }
        }

        private static IList<Potion> ReadPotions()
        {
            var potions = new List<Potion>();
            try
            {
                if (TryGetDocument(Paths.System.PotionsFile, out XmlDocument document))
                {
                    foreach (XmlNode potionNode in document.DocumentElement.ChildNodes)
                        potions.Add(new Potion(potionNode));
                }
            }
            catch { }
            return potions;
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
        }
    }
}
