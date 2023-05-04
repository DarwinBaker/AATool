using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AATool.Data.Progress;

namespace AATool.Data.Objectives.Complex
{
    internal class SnifferEgg : ComplexObjective
    {
        public const string ItemId = "minecraft:sniffer_egg";

        public bool Obtained { get; private set; }

        public SnifferEgg() : base()
        {
            this.Icon = "obtain_sniffer_egg";
        }

        protected override void UpdateAdvancedState(ProgressState progress)
        {
            this.CompletionOverride = progress.WasPickedUp(ItemId);
        }

        protected override void ClearAdvancedState()
        {
        }

        protected override string GetLongStatus() => "Sniffer Egg";
        protected override string GetShortStatus() => "Sniffer Egg";
    }
}
