using AATool.Configuration;
using AATool.Data.Objectives;
using AATool.Data.Progress;
using AATool.Graphics;
using AATool.UI.Screens;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace AATool.UI.Controls
{
    public class UITimeline : UIPanel
    {
        const int LineThickness = 4;
        const int LinePadding = 64;

        private Timeline run;
        private List<Advancement> sortedAdvancements;
        private List<(UIControl control, Objective objective, float currentX, float targetX)> items;


        private Rectangle lineRectangle;

        private DateTime start;
        private DateTime end;

        private bool relative = true;
        private bool top;
        private bool loaded;
        private int width => lineRectangle.Width;

        private static HashSet<string> important = new () {
            "minecraft:story/upgrade_tools",
            "minecraft:story/enter_the_nether",
            "minecraft:story/cure_zombie_villager",
            "minecraft:story/follow_ender_eye",
            "minecraft:story/enchant_item",

            "minecraft:nether/find_fortress",
            "minecraft:nether/find_bastion",
            "minecraft:nether/netherite_armor",
            "minecraft:nether/uneasy_alliance",
            "minecraft:minecraft:nether/ride_strider_in_overworld_lava",
            "minecraft:minecraft:nether/summon_wither",
            "minecraft:nether/all_effects",

            "minecraft:husbandry/axolotl_in_a_bucket",
            "minecraft:husbandry/kill_axolotl_target",
            "minecraft:husbandry/ride_a_boat_with_a_goat",
            "minecraft:husbandry/bred_all_animals",
            "minecraft:husbandry/balanced_diet",
            "minecraft:husbandry/complete_catalogue",
            "minecraft:husbandry/obtain_netherite_hoe",

            "minecraft:end/kill_dragon",
            "minecraft:end/elytra",
            "minecraft:end/respawn_dragon",

            "minecraft:adventure/sleep_in_bed",
            "minecraft:adventure/trade_at_world_height",
            "minecraft:adventure/two_birds_one_arrow",
            "minecraft:adventure/arbalistic",
            "minecraft:adventure/fall_from_world_height",
            "minecraft:adventure/play_jukebox_in_meadows",

            "minecraft:adventure/honey_block_slide",
            "minecraft:adventure/lightning_rod_with_villager_no_fire",
            "minecraft:adventure/totem_of_undying",
            "minecraft:adventure/walk_on_powder_snow_with_leather_boots",
            "minecraft:adventure/voluntary_exile",
            "minecraft:adventure/hero_of_the_village",
            "minecraft:adventure/very_very_frightening",
            "minecraft:adventure/adventuring_time",
            "minecraft:adventure/kill_all_mobs",
        };

        private Vector2 GetNext(Objective objective, int index)
        {
            int x = this.relative ? this.GetRelativeX(objective) : this.GetNormalizedX(index);
            int y = index % 2 is 0 ? this.lineRectangle.Top - 64 : this.lineRectangle.Bottom + 64;
            return new Vector2(x, y);
        }

        private int GetRelativeX(Objective objective)
        {
            long eventTime = objective.WhenFirstCompleted().Ticks;
            long range = (this.end.Ticks - this.start.Ticks);
            float scaled = (float)(eventTime - this.start.Ticks) / range;
            return this.lineRectangle.Left + (int)((this.lineRectangle.Width - 64) * scaled);
        }

        private int GetNormalizedX(int index)
        {
            //int range = (int)Math.Ceiling(this.run.Events.Count / 2f);
            int range = this.sortedAdvancements.Count;
            float scaled = (float)index / range;
            return this.lineRectangle.Left + (int)((this.lineRectangle.Width - 64) * scaled);
        }

        public UITimeline() : base()
        {
            this.items = new ();
            this.sortedAdvancements = new ();
        }

        private static void Sort(IList<Advancement> list, out DateTime first, out DateTime last)
        {
            Advancement temp;
            int minIndex;
            //sort advancements by order completed
            for (int i = 0; i < list.Count - 1; i++)
            {
                minIndex = i;
                for (int j = i + 1; j < list.Count; j++)
                {
                    if (list[j].WhenFirstCompleted() < list[minIndex].WhenFirstCompleted())
                        minIndex = j;
                }

                if (minIndex != i)
                {
                    temp = list[i];
                    list[i] = list[minIndex];
                    list[minIndex] = temp;
                }
            }
            first = list.FirstOrDefault()?.WhenFirstCompleted() ?? default;
            last = list.LastOrDefault()?.WhenFirstCompleted() ?? default;
        }

        public override void InitializeThis(UIScreen screen)
        {
            if (Tracker.Advancements.Count is 0)
                return;

            this.sortedAdvancements = Tracker.Advancements.AllAdvancements.Values.ToList();
            Sort(this.sortedAdvancements, out this.start, out this.end);

            //remove unimportant advancements
            for (int i = this.sortedAdvancements.Count - 1; i > 0; i--)
            {
                if (!important.Contains(this.sortedAdvancements[i].Id) || !this.sortedAdvancements[i].IsComplete())
                    this.sortedAdvancements.RemoveAt(i);
            }

            for (int i = 0; i < this.sortedAdvancements.Count; i++)
            { 
                var picture = new UITimelineEvent(this.sortedAdvancements[i], this.start, i % 2 is 0);
                picture.InitializeRecursive(this.Root());
                this.items.Add((picture, this.sortedAdvancements[i], 0, 0));
                this.AddControl(picture);
            }
        }

        public override void ResizeThis(Rectangle parent)
        {
            base.ResizeThis(parent);

            this.lineRectangle = new (
                this.Bounds.Left + LinePadding,
                this.Bounds.Center.Y - LineThickness,
                this.Bounds.Width - (LinePadding * 2),
                LineThickness * 2);
            
            var tempList = new List<(UIControl control, Objective objective, 
                float currentX, float targetX)>(this.items);
            this.items.Clear();
            for (int i = 0; i < tempList.Count; i++)
            {
                var item = tempList[i];
                Vector2 position = this.GetNext(item.objective, i);
                //item.control.MoveTo(new Point(this.lineRectangle.Center.X, (int)position.Y));
                this.items.Add((item.control, item.objective, item.control.Location.X, position.X));
            }
            this.loaded = true;
        }

        protected override void UpdateThis(Time time)
        {
            if (Input.RightClicked)
            {
                this.relative ^= true;
                this.ResizeThis(this.Bounds);
            }

            bool moved = false;
            for (int i = 0; i < this.items.Count; i++)
            {
                var item = this.items[i];
                float nextX = MathHelper.Lerp(item.currentX, item.targetX, 6f * (float)time.Delta);

                int x = (int)item.currentX;
                int y = i % 2 is 0 ? 480 : 640;
                item.control.MoveTo(new Point(x, y));
                this.items[i] = (item.control, item.objective, nextX, item.targetX);
                if (moved || Math.Abs(item.targetX - item.currentX) > 0.1f)
                {
                    moved = true;
                    UIMainScreen.Invalidate();
                }
            }
        }

        public override void DrawThis(Canvas canvas) 
        {
            if (this.SkipDraw)
                return;

            base.DrawThis(canvas);
            canvas.DrawRectangle(this.lineRectangle, Config.Main.TextColor);
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);
            this.BorderThickness = Attribute(node, "border_thickness", 1);
        }
    }
}
