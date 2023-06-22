using System;
using AATool.Data.Progress;

namespace AATool.Data.Objectives.Complex
{
    internal class SnifferEgg : Pickup
    {
        public const string ItemId = "minecraft:sniffer_egg";

        public SnifferEgg() : base(ItemId, "Eggs", 3)
        {
            this.Icon = "obtain_sniffer_egg";
        }

        protected override int GetCount(ProgressState progress)
        {
            int count = progress.TimesPickedUp(this.Id)
                - progress.TimesDropped(this.Id)
                - progress.TimesMined(this.Id);
            return Math.Max(count, 0);
        }

        protected override string GetShortStatus() => 
            $"{this.Obtained}\0/\0{this.Required}\0Eggs";

        protected override string GetLongStatus() => 
            $"{this.Obtained}\0/\0{this.Required}\0Eggs";
    }
}
