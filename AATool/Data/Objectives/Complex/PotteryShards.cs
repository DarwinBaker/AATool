using System;
using System.Collections.Generic;
using AATool.Data.Categories;
using AATool.Data.Progress;

namespace AATool.Data.Objectives.Complex
{
    class PotteryShards : ComplexObjective
    {
        public const string BrushAdvancement = "minecraft:nether/all_effects";
        public const string PotAdvancement = "minecraft:husbandry/balanced_diet";

        public const int Required = 4;

        public readonly HashSet<string> All = new() {
            "minecraft:angler_pottery_sherd",
            "minecraft:archer_pottery_sherd",
            "minecraft:arms_up_pottery_sherd",
            "minecraft:blade_pottery_sherd",
            "minecraft:brewer_pottery_sherd",
            "minecraft:burn_pottery_sherd",
            "minecraft:danger_pottery_sherd",
            "minecraft:explorer_pottery_sherd",
            "minecraft:friend_pottery_sherd",
            "minecraft:heart_pottery_sherd",
            "minecraft:heartbreak_pottery_sherd",
            "minecraft:howl_pottery_sherd",
            "minecraft:miner_pottery_sherd",
            "minecraft:mourner_pottery_sherd",
            "minecraft:plenty_pottery_sherd",
            "minecraft:prize_pottery_sherd",
            "minecraft:sheaf_pottery_sherd",
            "minecraft:shelter_pottery_sherd",
            "minecraft:skull_pottery_sherd",
            "minecraft:snort_pottery_sherd",
        };

        public int Obtained { get; private set; }

        private bool advancementsComplete;

        public PotteryShards() : base()
        {
        }

        protected override void UpdateAdvancedState(ProgressState progress)
        {
            this.Obtained = 0;
            foreach (string id in All)
                this.Obtained += GetCount(id, progress);

            this.advancementsComplete = progress.AdvancementCompleted(BrushAdvancement)
                && progress.AdvancementCompleted(PotAdvancement);

            this.CompletionOverride = this.advancementsComplete || this.Obtained >= Required;
        }

        private int GetCount(string id, ProgressState progress) =>
            Math.Max(0, progress.TimesPickedUp(id) - progress.TimesDropped(id));

        protected override void ClearAdvancedState()
        {
            this.Obtained = 0;
            this.advancementsComplete = false;
        }

        protected override string GetShortStatus()
        {
            if (this.Obtained < Required && this.advancementsComplete)
                return "Done";
            return $"{this.Obtained}\0/\0{Required} Shards";
        }

        protected override string GetLongStatus() => GetShortStatus();

        protected override string GetCurrentIcon() => "pottery_shard";
    }
}
