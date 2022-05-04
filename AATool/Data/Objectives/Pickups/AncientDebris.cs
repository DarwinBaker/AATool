using System;
using System.Xml;

namespace AATool.Data.Objectives.Pickups
{
    class AncientDebris : Pickup
    {
        public const string ItemId = "minecraft:ancient_debris";
        private const string ObtainDebris = "minecraft:nether/obtain_ancient_debris";
        private const string UseLodestone = "minecraft:nether/use_lodestone";
        private const string NetheriteHoe = "minecraft:husbandry/obtain_netherite_hoe";
        private const string NetheriteArmor = "minecraft:nether/netherite_armor";

        public AncientDebris(XmlNode node) : base(node) { }

        protected override void HandleCompletionOverrides()
        {
            //get netherite-related advancements
            Tracker.TryGetAdvancement(ObtainDebris, out Advancement hiddenInTheDepths);
            Tracker.TryGetAdvancement(UseLodestone, out Advancement countryLode);
            Tracker.TryGetAdvancement(NetheriteHoe, out Advancement seriousDedication);
            Tracker.TryGetAdvancement(NetheriteArmor, out Advancement coverMeInDebris);

            //ignore count if all netherite related advancements are done
            this.CompletionOverride = hiddenInTheDepths?.IsComplete() is true
                && countryLode?.IsComplete() is true
                && seriousDedication?.IsComplete() is true
                && coverMeInDebris?.IsComplete() is true;
        }

        protected override void UpdateLongStatus()
        {
            if (this.CompletionOverride)
            {
                this.FullStatus = "Done With Netherite";
            }
            else
            {
                int estimatedTNT = Math.Max(Tracker.State.TNTPickedUp - Tracker.State.TNTPlaced, 0);
                this.FullStatus = $"Debris: {this.GetTotal()}\nTNT: {estimatedTNT}";
            }
        }
    }
}
