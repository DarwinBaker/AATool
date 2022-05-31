using System.Collections.Generic;
using System.IO;
using System.Text;
using AATool.Data.Objectives;
using AATool.Graphics;
using AATool.Net;
using AATool.Utilities;

namespace AATool.Data.Categories
{
    public class AllBlocks : Category
    {
        public static readonly List<string> SupportedVersions = new () {
            "1.18",
            "1.16",
        };

        public override IEnumerable<string> GetSupportedVersions() => SupportedVersions;
        public override IEnumerable<Objective> GetOverlayObjectives() => Tracker.Blocks.All.Values;

        public override int GetTargetCount() => Tracker.Blocks.Count;
        public override int GetCompletedCount() => Tracker.Blocks.PlacedCount;

        public override string GetStatus()
        {
            return Tracker.Blocks.ObtainedCount > 0
                ? $"{base.GetStatus()} (Approximately {Tracker.Blocks.ObtainedCount} Obtained)"
                : $"{base.GetStatus()} ({Tracker.Blocks.ObtainedCount} Obtained)";
        }

        public static bool SpritesLoaded { get; set; }

        public AllBlocks() : base()
        {
            this.Name      = "All Blocks";
            this.Acronym   = "AB";
            this.Objective = "Blocks";
            this.Action    = "Placed";

            SpriteSheet.Include("blocks");
        }

        public override void LoadObjectives()
        {
            Tracker.Blocks.RefreshObjectives();
            Tracker.Pickups.RefreshObjectives();
        }

        public void ClearHighlighted()
        {
            foreach (Block block in Tracker.Blocks.All.Values)
                block.Highlighted = false;
        }

        public override void Update()
        {
            if (Tracker.SavesFolderChanged || Tracker.WorldChanged)
            {
                this.ClearHighlighted();
                this.TryLoadChecklist();
            }
        }

        public string GetBlockHighlights()
        {
            var builder = new StringBuilder();
            foreach (Block block in Tracker.Blocks.All.Values)
            {
                if (block.Highlighted)
                    builder.AppendLine(block.Id);
            }
            return builder.ToString();
        }

        public void TryLoadChecklist()
        {
            if (!Tracker.IsWorking || Peer.IsClient)
                return;
            string path = Paths.System.BlockChecklistFile(ActiveInstance.Number, Tracker.WorldName);
            if (!File.Exists(path))
                return;

            try
            {
                string[] lines = File.ReadAllLines(path);
                this.ApplyChecklist(lines);
                foreach (string id in lines)
                {
                    if (Tracker.TryGetBlock(id, out Block block))
                        block.Highlighted = true;
                }
            }
            catch
            {

            }
        }

        public void ApplyChecklist(string[] lines)
        {
            if (lines is null)
                return;
            foreach (string id in lines)
            {
                if (Tracker.TryGetBlock(id, out Block block))
                    block.Highlighted = true;
            }
        }

        public void SaveChecklist()
        {
            if (!Tracker.IsWorking || Peer.IsClient)
                return;
            string path = Paths.System.BlockChecklistFile(ActiveInstance.Number, Tracker.WorldName);
            try
            {
                Directory.CreateDirectory(Paths.System.BlockChecklistsFolder);
                using (StreamWriter file = File.CreateText(path))
                {
                    foreach (Block block in Tracker.Blocks.All.Values)
                    {
                        if (block.Highlighted)
                            file.WriteLine(block.Id);
                    }
                }
            }
            catch
            {

            }
        }
    }
}
