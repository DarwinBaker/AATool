using AATool.Data;
using AATool.Graphics;
using AATool.Settings;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace AATool.UI.Controls
{
    class UIPotionGroup : UIFlowPanel
    {
        public override void InitializeRecursive(UIScreen screen)
        {
            this.BuildFromSourceDocument();
            foreach (Potion potion in this.ReadPotions())
                this.AddControl(new UIPotion() { Potion = potion });
            base.InitializeRecursive(screen);
        }

        private IList<Potion> ReadPotions()
        {
            var potions = new List<Potion>();
            try
            {
                if (TryGetDocument(Paths.PotionsFile, out XmlDocument document))
                {
                    foreach (XmlNode potionNode in document.DocumentElement.ChildNodes)
                        potions.Add(new Potion(potionNode));
                }
            }
            catch (Exception e)
            { 
                Main.QuitBecause("Error loading potion recipes!", e); 
            }
            return potions;
        }

        public override void DrawThis(Display display)
        {
            base.DrawThis(display);
            for (int i = 0; i < this.Children.Count - 1; i++)
            {
                Rectangle bounds = this.Children[i].Bounds;
                display.DrawRectangle(new Rectangle(bounds.Left, bounds.Bottom - 6, bounds.Width, 2), MainSettings.Instance.BorderColor);
            }
        }
    }
}
