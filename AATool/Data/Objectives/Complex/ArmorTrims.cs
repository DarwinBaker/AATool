using System.Collections.Generic;
using System.Linq;
using AATool.Data.Progress;

namespace AATool.Data.Objectives.Complex
{
    class ArmorTrims : ComplexObjective
    {
        public static readonly List<string> Recipes = new () {
            //required for all advancements
            "minecraft:recipes/misc/silence_armor_trim_smithing_template",
            "minecraft:recipes/misc/wayfinder_armor_trim_smithing_template",
            "minecraft:recipes/misc/tide_armor_trim_smithing_template",
            "minecraft:recipes/misc/spire_armor_trim_smithing_template",
            "minecraft:recipes/misc/vex_armor_trim_smithing_template",
            "minecraft:recipes/misc/ward_armor_trim_smithing_template",
            "minecraft:recipes/misc/rib_armor_trim_smithing_template",
            "minecraft:recipes/misc/snout_armor_trim_smithing_template",
            //others
            "minecraft:recipes/misc/raiser_armor_trim_smithing_template",
            "minecraft:recipes/misc/sentry_armor_trim_smithing_template",
            "minecraft:recipes/misc/host_armor_trim_smithing_template",
            "minecraft:recipes/misc/wild_armor_trim_smithing_template",
            "minecraft:recipes/misc/eye_armor_trim_smithing_template",
            "minecraft:recipes/misc/shaper_armor_trim_smithing_template",
            "minecraft:recipes/misc/dune_armor_trim_smithing_template",
            "minecraft:recipes/misc/coast_armor_trim_smithing_template",
            "minecraft:recipes/misc/netherite_upgrade_smithing_template",
        };

        public const string AdvancementId = "minecraft:adventure/trim_with_all_exclusive_armor_patterns";
        public const string CategoryId = "custom:all_smithing_templates";

        public List<string> Required = new ();
        public List<string> Remaining = new ();
        public List<string> Obtained = new ();
        public List<string> Applied = new ();

        private bool AllObtained => this.Obtained.Count >= Required.Count && Required.Count > 0;
        private bool AllApplied => this.Applied.Count >= Required.Count && Required.Count > 0;
        private bool OnLast => this.Remaining.Count is 1;

        public ArmorTrims() : base()
        { 
        }

        protected override void UpdateAdvancedState(ProgressState progress)
        {
            this.Required.Clear();
            if (!Tracker.TryGetAdvancement(AdvancementId, out Advancement adv) || !adv.HasCriteria)
                return;

            this.Remaining.Clear();
            this.Obtained.Clear();
            this.Applied.Clear();
            foreach (ArmorTrimCriterion criterion in adv.Criteria.All.Values)
            {
                this.Required.Add(criterion.Id);
                if (criterion.Obtained)
                    this.Obtained.Add(criterion.Id);
                if (criterion.Applied)
                    this.Applied.Add(criterion.Id);
                if (!criterion.IsComplete())
                    this.Remaining.Add(criterion.Id);
            }
            this.CompletionOverride = adv.IsComplete() || this.AllApplied || this.AllObtained;
        }

        protected override void ClearAdvancedState()
        {
            this.Required.Clear();
            if (Tracker.TryGetAdvancement(AdvancementId, out Advancement adv) && adv.HasCriteria)
            {
                foreach (ArmorTrimCriterion criterion in adv.Criteria.All.Values)
                    this.Required.Add(criterion.Id);
            }

            this.Remaining.Clear();
            this.Remaining.AddRange(this.Required);
            this.Obtained.Clear();
            this.Applied.Clear();
        }

        protected override string GetShortStatus() =>
            $"{this.Obtained.Count}\0/\0{this.Required.Count}";

        protected override string GetLongStatus()
        {
            if (this.AllApplied)
                return "All\0Trims\nApplied";

            if (this.AllObtained)
                return "Obtained\nAll\0Trims";

            if (this.OnLast)
                return $"Still\0Needs\n{FriendlyName(this.Remaining.First())}";

            return $"Trims\n{this.Obtained.Count}\0/\0{this.Required.Count}";
        }

        protected override string GetCurrentIcon()
        {
            if (this.OnLast)
                return IconName(this.Remaining.First());
            return this.Icon;
        }

        internal void UpdateDynamicIcon(Time time)
        {
            if (this.Remaining.Count > 1)
            {
                const float SecondsBetweenSwaps = 5;
                int ticks = (int)(time.TotalSeconds / SecondsBetweenSwaps);
                int wrappedIndex = ticks % this.Remaining.Count;
                this.Icon = IconName(this.Remaining[wrappedIndex]);
            }
            else if (this.Remaining.Count is 0 && this.Required.Count > 0)
            {
                const float SecondsBetweenSwaps = 5;
                int ticks = (int)(time.TotalSeconds / SecondsBetweenSwaps);
                int wrappedIndex = ticks % this.Required.Count;
                this.Icon = IconName(this.Required[wrappedIndex]);
            }
        }

        public static string IconName(string id)
        {
            if (string.IsNullOrEmpty(id))
                return string.Empty;

            string fullName = id.Split(':').Last();
            string shortName = fullName.Split('_').First();
            return $"trim_{shortName}";
        }

        public static string FriendlyName(string id)
        {
            if (string.IsNullOrEmpty(id))
                return string.Empty;

            string fullName = id.Split(':').Last();
            string shortName = fullName.Split('_').First();
            if (string.IsNullOrEmpty(shortName))
                return string.Empty;

            char[] letters = shortName.ToCharArray();
            letters[0] = char.ToUpper(letters[0]);
            return new string(letters);
        }
    }
}
