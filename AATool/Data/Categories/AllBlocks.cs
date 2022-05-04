using System.Collections.Generic;
using System.IO;
using AATool.Data.Objectives;
using AATool.Graphics;
using AATool.Utilities;

namespace AATool.Data.Categories
{
    public class AllBlocks : Category
    {
        public static readonly List<string> SupportedVersions = new () {
            "1.16"
        };

        public override IEnumerable<string> GetSupportedVersions() => SupportedVersions;
        public override IEnumerable<Objective> GetOverlayObjectives() => Tracker.Blocks.All.Values;

        public override int GetTargetCount() => Tracker.Blocks.Count;
        public override int GetCompletedCount() => Tracker.Blocks.PlacedCount;

        public override string GetStatus() => $"{base.GetStatus()} (Approximately {Tracker.Blocks.ObtainedCount} Obtained)";

        public AllBlocks() : base()
        {
            this.Name      = "All Blocks";
            this.Acronym   = "AB";
            this.Objective = "Blocks";
            this.Action    = "Placed";

            SpriteSheet.Require("blocks");
        }

        public override void LoadObjectives()
        {
            Tracker.Blocks.RefreshObjectives();
            Tracker.Pickups.RefreshObjectives();
        }

        public void ClearManuallyChecked()
        {
            foreach (Block block in Tracker.Blocks.All.Values)
                block.ManuallyCompleted = false;
            Tracker.Blocks.UpdateTotal();
        }

        public override void Update()
        {
            if (Tracker.SavesFolderChanged || Tracker.WorldChanged)
                this.TryLoadChecklist();
        }

        public void TryLoadChecklist()
        {
            if (!Tracker.IsWorking)
                return;

            try
            {
                string instance = ActiveInstance.Number > 0
                    ? $"instance_{ActiveInstance.Number}-" : "";
                string path = Paths.System.BlockChecklistFile(instance, Tracker.WorldName);
                string[] lines = File.ReadAllLines(path);
                this.ClearManuallyChecked();

                foreach (string id in lines)
                {
                    if (Tracker.TryGetBlock(id, out Block block))
                        block.ManuallyCompleted = true;
                }
            }
            catch
            {

            }
        }

        public void SaveChecklist()
        {
            if (!Tracker.IsWorking)
                return;

            try
            {
                
            }
            catch
            {

            }
        }
    }
}
