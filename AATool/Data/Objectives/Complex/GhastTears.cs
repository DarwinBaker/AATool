using System.Xml;
using AATool.Data.Progress;

namespace AATool.Data.Objectives.Complex
{
    public class GhastTears : ComplexObjective
    {
        public const string AdvancementId = "minecraft:end/respawn_dragon";
        public const string TearId = "minecraft:ghast_tear";
        public const string CrystalId = "minecraft:end_crystal";

        private int tears;
        private int crystals;
        private bool dragonRespawned;

        private bool HasAllTears => this.tears > 4;
        private bool HasAnyCrystals => this.crystals > 0;

        public GhastTears() : base() 
        {
            this.Icon = "uneasy_alliance";
        }

        protected override void UpdateAdvancedState(ProgressState progress)
        {
            this.dragonRespawned = progress.AdvancementCompleted(AdvancementId);

            this.tears = progress.TimesPickedUp(TearId)
                - progress.TimesDropped(TearId);

            this.crystals = progress.TimesCrafted(CrystalId)
                + progress.TimesPickedUp(CrystalId)
                - progress.TimesDropped(CrystalId);

            this.CompletionOverride = this.dragonRespawned || this.HasAllTears || this.HasAnyCrystals;
        }

        protected override string GetCurrentIcon() => 
            this.dragonRespawned || this.HasAnyCrystals ? "respawn_dragon" : "uneasy_alliance";

        protected override void ClearAdvancedState()
        {
            this.dragonRespawned = false;
            this.tears = 0;
            this.crystals = 0;
        }

        protected override string GetShortStatus()
        {
            if (this.dragonRespawned)
                return "Done";

            return this.HasAnyCrystals
                ? $"{this.crystals} / 4"
                : $"{this.tears} / 4";
        }

        protected override string GetLongStatus()
        {
            if (this.dragonRespawned)
                return $"End\0Again\nCompleted";
            return $"Tears:\0{this.tears}\nCrystals:\0{this.crystals}";
        }
    }
}
