using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AATool.Data.Objectives.Complex;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    public class UIRunOverview : UIPanel
    {
        private const string Rocket = "minecraft:firework_rocket";
        private const string Tnt = "minecraft:tnt";
        private const string Obsidian = "minecraft:obsidian";
        private const string Pearl = "minecraft:ender_pearl";
        private const string Shell = "minecraft:nautilus_shell";
        private const string Skull = "minecraft:wither_skull";
        private const string Debris = "minecraft:ancient_debris";
        private const string Tear = "minecraft:ghast_tear";
        private const string Crystal = "minecraft:end_crystal";
        private const string Beehive = "minecraft:bee_nest";

        private UITextBlock tnt;
        private UITextBlock gold;
        private UITextBlock obsidian;
        private UITextBlock beehives;
        private UITextBlock pearls;
        private UITextBlock shells;
        private UITextBlock debris;
        private UITextBlock skulls;
        private UITextBlock tears;
        private UIPicture tearAndCrystal;

        private bool initialized = false;

        public UIRunOverview()
        {
            this.BuildFromTemplate();
        }

        public override void InitializeThis(UIScreen screen)
        {
            base.InitializeThis(screen);
            this.TryGetFirst(out this.tnt, "tnt_count");
            this.TryGetFirst(out this.gold, "gold_count");
            this.TryGetFirst(out this.obsidian, "obsidian_count");
            this.TryGetFirst(out this.pearls, "pearl_count");
            this.TryGetFirst(out this.debris, "debris_count");
            this.TryGetFirst(out this.skulls, "skull_count");
            this.TryGetFirst(out this.shells, "shell_count");
            this.TryGetFirst(out this.tears, "tear_count");
            this.TryGetFirst(out this.tearAndCrystal, "tear_and_crystal");
            this.TryGetFirst(out this.beehives, "beehive_count");
        }

        public override void ResizeRecursive(Rectangle rectangle)
        {
            base.ResizeRecursive(rectangle);
            //this.Refresh();
        }

        protected override void UpdateThis(Time time)
        { 
            base.UpdateThis(time);
            if (Tracker.Invalidated)
                this.Refresh();
        }

        private void Refresh()
        {
            this.First<UITextBlock>("day_night_igt")?.SetText($"IGT: {Tracker.InGameTime:h':'mm':'ss}");

            this.UpdateCounts();
        }

        public override void Expand()
        {
            base.Expand();
            //this.Refresh();
        }

        private void UpdateCounts()
        {
            //tnt
            int tntCount = Tracker.State.TimesPickedUp(Tnt)
                + Tracker.State.TimesCrafted(Tnt)
                - Tracker.State.TimesUsed(Tnt)
                - Tracker.State.TimesDropped(Tnt);
            this.tnt?.SetText(Math.Max(0, tntCount).ToString());

            //gold
            int goldCount = GoldBlocks.GetPreciseEstimate(Tracker.State);
            this.gold?.SetText(goldCount.ToString());

            //obsidian
            int obsidianCount = Tracker.State.TimesPickedUp(Obsidian)
                - Tracker.State.TimesUsed(Obsidian)
                - Tracker.State.TimesDropped(Obsidian);
            this.obsidian?.SetText(Math.Max(0, obsidianCount).ToString());

            //shells
            int shellCount = Tracker.State.TimesPickedUp(Shell)
                - Tracker.State.TimesDropped(Shell);
            this.shells?.SetText($"{Math.Max(0, shellCount)}/8");

            //debris
            int debrisCount = Tracker.State.TimesPickedUp(Debris)
                - Tracker.State.TimesDropped(Debris);
            this.debris?.SetText(Math.Max(0, debrisCount).ToString());

            //beehives
            int beehiveCount = Tracker.State.TimesPickedUp(Beehive)
                - Tracker.State.TimesDropped(Beehive)
                - Tracker.State.TimesUsed(Beehive);
            this.beehives?.SetText(Math.Max(0, beehiveCount).ToString());

            //skulls
            int skullCount = Tracker.State.TimesPickedUp(Skull)
                - Tracker.State.TimesDropped(Skull)
                - Tracker.State.TimesUsed(Skull);
            this.skulls?.SetText($"{Math.Max(0, skullCount)}/3");

            //end crystals
            int crystalCount = Tracker.State.TimesCrafted(Crystal)
                + Tracker.State.TimesPickedUp(Crystal)
                - Tracker.State.TimesDropped(Crystal);
           
            if (crystalCount > 0)
            {
                this.tearAndCrystal?.SetTexture("crystal_overview");
                this.tears?.SetText($"{Math.Max(0, crystalCount)}/4");
            }
            else
            {   
                //ghast tears
                int tearCount = Tracker.State.TimesPickedUp(Tear)
                - Tracker.State.TimesCrafted(Crystal)
                - Tracker.State.TimesDropped(Tear);
                this.tears?.SetText(Math.Max(0, skullCount).ToString());

                this.tearAndCrystal?.SetTexture("tear_and_crystal");
                this.tears?.SetText($"{Math.Max(0, tearCount)}/4");
            }

            //ender pearls
            int pearlCount = + Tracker.State.TimesPickedUp(Pearl)
                - Tracker.State.TimesUsed(Pearl)
                - Tracker.State.TimesDropped(Pearl);
            this.pearls?.SetText(Math.Max(0, pearlCount).ToString());
        }
    }
}
