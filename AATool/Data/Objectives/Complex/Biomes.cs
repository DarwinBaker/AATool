using System;
using System.Collections.Generic;
using System.Linq;
using AATool.Data.Categories;
using AATool.Data.Progress;

namespace AATool.Data.Objectives.Complex
{
    class Biomes : ComplexCriteriaObjective
    {
        private static readonly string[] MegaTaigaBiomes = new string[] {
            /* 1.18 */ "minecraft:old_growth_spruce_taiga", "minecraft:old_growth_pine_taiga",
            /* 1.13 */ "minecraft:giant_tree_taiga", "minecraft:giant_tree_taiga_hills",
            /* 1.12 */ "redwood_taiga", "redwood_taiga_hills",
            /* 1.11 */ "Mega Taiga", "Mega Taiga Hills",
        };

        private static readonly string[] MushroomBiomes = new string[] {
            /* 1.13 */ "minecraft:mushroom_fields", "minecraft:mushroom_field_shore",
            /* 1.12 */ "mushroom_island", "mushroom_island_shore",
            /* 1.11 */ "MushroomIsland", "MushroomIslandShore",
        };

        private static readonly string[] BadlandsBiomes = new string[] {
            /* 1.18 */ "minecraft:badlands", "minecraft:wooded_badlands", "minecraft:badlands_plateau", "minecraft:wooded_badlands_plateau",
            /* 1.13 */ "minecraft:eroded_badlands",
            /* 1.12 */ "mesa", "mesa_clear", "mesa_clear_rock",
            /* 1.11 */ "Mesa", "Mesa Plateau", "Mesa Plateau F",
        };

        private static readonly string[] BambooBiomes = new string[] {
            "minecraft:bamboo_jungle", "minecraft:bamboo_jungle_hills",
        };

        private bool onlyMushroomLeft;
        private bool onlyMegaTaigaLeft;
        private bool onlyBadlandsLeft;
        private bool onlyBambooLeft;

        protected readonly HashSet<string> RemainingIds = new ();

        public override string AdvancementId => "minecraft:adventure/adventuring_time";
        public override string Criterion => "Biome";
        public override string Action => "Visit";
        public override string PastAction => "Visited";
        protected override string ModernBaseTexture => "adventuring_time";
        protected override string OldBaseTexture => "adventuring_time_1.12";

        protected override void UpdateAdvancedState(ProgressState progress)
        {
            base.UpdateAdvancedState(progress);
            this.onlyMegaTaigaLeft = this.OnlyGroupRemaining(MegaTaigaBiomes, 2);
            this.onlyMushroomLeft = this.OnlyGroupRemaining(MushroomBiomes, 2);
            this.onlyBadlandsLeft = this.OnlyGroupRemaining(BadlandsBiomes, 3);
            this.onlyBambooLeft = this.OnlyGroupRemaining(BambooBiomes, 2);
        }

        protected override void BuildRemainingCriteriaList(CriteriaSet criteria)
        {
            this.CurrentCriteria = 0;
            this.RequiredCriteria = criteria.Count;
            this.RemainingIds.Clear();
            this.RemainingCriteria.Clear();
            foreach (Criterion criterion in criteria.All.Values)
            {
                if (criterion.IsComplete())
                {
                    this.CurrentCriteria++;
                }
                else
                {
                    _= this.RemainingCriteria.Add(criterion.Name);
                    _= this.RemainingIds.Add(criterion.Id);
                    this.LastCriterionIcon = criterion.Icon;
                }
            }
        }

        private bool OnlyGroupRemaining(string[] group, int maxRemaining)
        {
            if (Tracker.Category is not AllAdvancements or AllAchievements)
                return false;
            if (this.RemainingCriteria.Count is 0)
                return false;
            if (this.RemainingCriteria.Count > maxRemaining)
                return false;

            foreach (string biome in this.RemainingIds)
            {
                if (!group.Contains(biome))
                    return false;
            }
            return true;
        }

        protected override void ClearAdvancedState()
        {
            this.onlyMegaTaigaLeft = false;
            this.onlyMushroomLeft = false;
            this.onlyBadlandsLeft = false;
            this.onlyBambooLeft = false;
            base.ClearAdvancedState();
        }

        private static string FormatBiomeName(string name)
        {
            return name.Replace(" Mountain", " Mtn")
                .Replace(" Plateau", "-Plat")
                .Replace("Mushroom Fields", "Mushroom")
                .Replace("Mushroom Shore", "Mush-Shore")
                .Replace("Bamboo Jungle", "Bamboo")
                .Replace("Deep Cold Ocean", "Deep Cold")
                .Replace("Lukewarm Ocean", "Lukewarm")
                .Replace(" Growth", string.Empty)
                .Replace(" ", "\0");
        }

        protected override string GetShortStatus() =>
            $"{this.CurrentCriteria} / {this.RemainingCriteria}";

        protected override string GetLongStatus()
        {
            if (this.OnLastCriterion)
                return base.GetLongStatus();

            if (this.onlyMegaTaigaLeft)
                return "Still\0Needs\nMega Taiga";
            if (this.onlyMushroomLeft)
                return "Still\0Needs\nMushroom";
            if (this.onlyBadlandsLeft)
                return "Still\0Needs\nBadlands";
            if (this.onlyBambooLeft)
                return "Still\0Needs\nBamboo";

            return base.GetLongStatus();
        }

        protected override string LongStatusNormal() => 
            $"Biomes\n{this.CurrentCriteria}\0/\0{this.RequiredCriteria}";

        protected override string LongStatusLast() =>
            $"Last Biome:\n{FormatBiomeName(this.RemainingCriteria.First())}";

        protected override string GetCurrentIcon()
        {
            if (this.UseModernTexture)
            {
                if (this.CompletionOverride || this.AllCriteriaCompleted)
                    return "enchanted_diamond_boots";

                if (this.OnLastCriterion)
                    return this.LastCriterionIcon;

                if (this.onlyMegaTaigaLeft)
                    return "giant_tree_taiga";
                if (this.onlyMushroomLeft)
                    return "mushroom_fields";
                if (this.onlyBadlandsLeft)
                    return "badlands";
                if (this.onlyBambooLeft)
                    return "bamboo_jungle";

                return this.ModernBaseTexture;
            }
            else
            {
                return this.OnLastCriterion
                    ? this.LastCriterionIcon
                    : this.OldBaseTexture;
            }
        }
    }
}
