using AATool.Data.Progress;
using System;
using System.Xml;

namespace AATool.Data
{
    public class Statistic
    {
        public const string FRAME_COMPLETE   = "frame_count_complete";
        public const string FRAME_INCOMPLETE = "frame_count_incomplete";

        public string ID        { get; private set; }
        public string Name      { get; private set; }
        public int PickedUp     { get; private set; }
        public int TargetCount  { get; private set; }
        public string Icon      { get; private set; }
        public bool IsCompleted { get; private set; }
        public bool IsEstimate  { get; private set; }
        public string LongStatus  { get; private set; }
        public string ShortStatus { get; private set; }

        public string CurrentFrame  => this.IsCompleted ? FRAME_COMPLETE : FRAME_INCOMPLETE;

        public Statistic(XmlNode node)
        {
            //initialize members from xml 
            this.ID   = node.Attributes["id"]?.Value;
            this.Name = node.Attributes["name"]?.Value ?? this.ID;
            this.Icon = node.Attributes["icon"]?.Value;
            if (int.TryParse(node.Attributes["target_count"]?.Value, out int parsed))
                this.TargetCount = parsed;
            if (bool.TryParse(node.Attributes["estimate"]?.Value, out bool isEstimate))
                this.IsEstimate = isEstimate;
            if (this.Icon is null)
            {
                string[] idParts = this.ID.Split(':');
                if (idParts.Length > 0)
                    this.Icon = idParts[idParts.Length - 1];
            }
            this.UpdateLongStatus();
            this.UpdateShortStatus();
        }

        public void UpdateLongStatus()
        {
            if (this.TargetCount is 1)
            {
                this.LongStatus = this.Name;
            }
            else
            {
                this.LongStatus = this.IsEstimate && this.IsCompleted
                    ? $"{this.Name}\nComplete"
                    : $"{this.Name}\n{this.PickedUp}\0/\0{this.TargetCount}";
            }

            if (this.IsCompleted)
            {
                //handle overrides
                if (this.ID is "minecraft:trident")
                {
                    Tracker.TryGetAdvancement("minecraft:adventure/very_very_frightening", out Advancement veryVeryFrightening);
                    this.LongStatus = veryVeryFrightening?.CompletedByAnyone() is true
                        ? "Done With Thunder"
                        : "Awaiting Thunder";
                }
                else if (this.ID is "minecraft:ancient_debris")
                {
                    this.LongStatus = "Done With Netherite";
                }
                else if (this.ID is "minecraft:wither_skeleton_skull")
                {
                    Tracker.TryGetCriterion("minecraft:adventure/kill_all_mobs", "minecraft:wither", out Criterion killWither);
                    if (killWither?.CompletedByAnyone() is true)
                        this.LongStatus = "Wither Has Been Killed";
                }
            }
        }

        public void UpdateShortStatus()
        {
            if (this.TargetCount is 1)
            {
                this.ShortStatus = this.PickedUp.ToString();
            }
            else
            {
                this.ShortStatus = this.IsEstimate && this.IsCompleted
                    ? "Complete"
                    : $"{this.PickedUp} / {this.TargetCount}";
            }
        }

        public void Update(ProgressState progress)
        {
            this.PickedUp = progress.ItemCount(this.ID);
            this.IsCompleted = this.PickedUp >= this.TargetCount;
            if (!this.IsCompleted)
                this.HandleOverrides();

            this.UpdateLongStatus();
            this.UpdateShortStatus();
        }

        private void HandleOverrides()
        {
            if (this.ID is "minecraft:ancient_debris")
            {
                //ignore count if all netherite related advancements are done
                if (!Tracker.TryGetAdvancement("minecraft:nether/obtain_ancient_debris", out Advancement hiddenInTheDepths))
                    return;
                if (!Tracker.TryGetAdvancement("minecraft:nether/netherite_armor", out Advancement coverMeInDebris))
                    return;
                if (!Tracker.TryGetAdvancement("minecraft:nether/use_lodestone", out Advancement countryLode))
                    return;
                if (!Tracker.TryGetAdvancement("minecraft:husbandry/obtain_netherite_hoe", out Advancement seriousDedication))
                    return;

                this.IsCompleted = hiddenInTheDepths.CompletedByAnyone();
                this.IsCompleted &= countryLode.CompletedByAnyone();
                this.IsCompleted &= seriousDedication.CompletedByAnyone();
                this.IsCompleted &= coverMeInDebris.CompletedByAnyone();
            }
            else if (this.ID is "minecraft:enchanted_golden_apple")
            {
                //ignore picked up if eaten
                Tracker.TryGetCriterion("minecraft:husbandry/balanced_diet", "enchanted_golden_apple", out Criterion eatEgap);
                if (eatEgap?.CompletedByAnyone() is true)
                    this.IsCompleted = true;
            }
        }
    }
}
