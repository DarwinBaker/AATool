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
        public override void InitializeRecursive(UIScreen screen)
        {
            this.BuildFromTemplate();
            foreach (Potion potion in this.ReadPotions())
                this.AddControl(new UIPotion() { Potion = potion });
            base.InitializeRecursive(screen);
        }

        private IList<Potion> ReadPotions()
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
            catch (Exception e)
            { 
                Main.QuitBecause("Error loading potion recipes!", e); 
            }
            return potions;
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
    }
}
