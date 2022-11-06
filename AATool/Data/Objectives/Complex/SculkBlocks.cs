using System.Collections.Generic;
using System.Linq;
using AATool.Data.Progress;

namespace AATool.Data.Objectives.Complex
{
    class SculkBlocks : ComplexObjective
    {
        public static readonly Dictionary<string, string> AllSculkBlocks = new () {
            { "minecraft:sculk", "Sculk\0Block" },
            { "minecraft:sculk_catalyst", "Catalyst"  },
            { "minecraft:sculk_shrieker", "Shrieker"  },
            { "minecraft:sculk_sensor" , "Sensor" },
            { "minecraft:sculk_vein", "Sculk\0Vein"  },
        };

        public List<string> Remaining = new ();
        public List<string> Obtained = new ();
        public List<string> Placed = new ();

        private bool AllObtained => this.Obtained.Count >= AllSculkBlocks.Count;
        private bool AllPlaced => this.Placed.Count >= AllSculkBlocks.Count;
        private bool OnLastBlock => this.Remaining.Count is 1;

        protected override void UpdateAdvancedState(ProgressState progress)
        {
            this.ClearAdvancedState();
            foreach (string block in AllSculkBlocks.Keys)
            {
                if (progress.WasUsed(block))
                {
                    this.Placed.Add(block);
                    _= this.Remaining.Remove(block);
                }
                if (progress.WasPickedUp(block))
                {
                    this.Obtained.Add(block);
                    _= this.Remaining.Remove(block);
                }
            }
            this.CompletionOverride = this.AllObtained || this.AllPlaced;
        }

        protected override void ClearAdvancedState()
        {
            this.Remaining.Clear();
            this.Remaining.AddRange(AllSculkBlocks.Keys);
            this.Obtained.Clear();
            this.Placed.Clear();
        }

        protected override string GetShortStatus() =>
            $"{this.Obtained}\0/\0{AllSculkBlocks.Count}";

        protected override string GetLongStatus()
        {
            if (this.AllPlaced)
                return "All\0Sculk\nPlaced";

            if (this.AllObtained)
                return "Obtained\nAll\0Sculk";

            if (this.OnLastBlock)
                return $"Still\0Needs\n{AllSculkBlocks[this.Remaining.First()]}";

            return $"Obtain\nSculk";
        }

        protected override string GetCurrentIcon()
        {
            if (this.OnLastBlock)
            {
                string last = this.Remaining.First().Split(':').Last();
                if (last is "sculk_catalyst" or "sculk_sensor")
                    last += "_block";
                return last;
            }              
            return "sculk_shrieker";
        }
    }
}
