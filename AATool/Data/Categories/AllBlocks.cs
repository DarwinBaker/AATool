using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using AATool.Data.Objectives;
using AATool.Graphics;
using AATool.Net;
using AATool.Utilities;

namespace AATool.Data.Categories
{
    public class AllBlocks : Category
    {
        public const string MainTextureSet = "blocks";
        public const string HelpTextureSet = "ab_guide";

        public static readonly List<string> SupportedVersions = new () {
            "1.21",
            "1.20",
            "1.19",
            "1.18",
            "1.16",
        };

        private static bool WritingChecklistFile = false;

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

        public static bool MainSpritesLoaded { get; set; }
        public static bool HelpSpritesLoaded { get; set; }

        public int BlocksHighlightedCount { get; set; }
        public int BlocksConfirmedCount { get; set; }

        public AllBlocks() : base()
        {
            this.Name      = "All Blocks";
            this.Acronym   = "AB";
            this.Objective = "Blocks";
            this.Action    = "Placed";

            SpriteSheet.Request(MainTextureSet);
            SpriteSheet.Request(HelpTextureSet);
        }

        public override void LoadObjectives()
        {
            Tracker.Blocks.RefreshObjectives();
            Tracker.ComplexObjectives.RefreshObjectives();
        }

        public void ClearHighlighted()
        {
            foreach (Block block in Tracker.Blocks.All.Values)
            {
                if (!block.IsComplete())
                    block.Highlighted = false;
            }
        }

        public void ClearConfirmed()
        {
            foreach (Block block in Tracker.Blocks.All.Values)
            {
                if (block.IsComplete())
                    block.Highlighted = false;
            }  
        }

        public override void Update()
        {
            if (Tracker.SavesFolderChanged || Tracker.WorldChanged)
            {
                this.ClearHighlighted();
                this.ClearConfirmed();
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

        public int CountHighlightedBlocks()
        {
            int counter = 0;
            foreach (Block block in Tracker.Blocks.All.Values)
            {
                if (!block.IsComplete() && block.Highlighted)
                    counter++;
            }
            return counter;
        }

        public int CountConfirmedBlocks()
        {
            int counter = 0;
            foreach (Block block in Tracker.Blocks.All.Values)
            {
                if (block.IsComplete() && block.Highlighted)
                    counter++;
            }
            return counter;
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

            this.BlocksHighlightedCount = 0;
            this.BlocksConfirmedCount = 0;
            var list = new StringBuilder();
            foreach (Block block in Tracker.Blocks.All.Values)
            {
                if (block.Highlighted)
                {
                    list.AppendLine(block.Id);

                    //update counts
                    if (block.IsComplete())
                        this.BlocksConfirmedCount++;
                    else
                        this.BlocksHighlightedCount++;
                }
            }

            TryWriteChecklist(list.ToString());
        }

        private void TryWriteChecklist(string list)
        {
            if (WritingChecklistFile)
                return;

            WritingChecklistFile = true;
            new Thread(() => {
                try
                {
                    string path = Paths.System.BlockChecklistFile(ActiveInstance.Number, Tracker.WorldName);
                    Directory.CreateDirectory(Paths.System.BlockChecklistsFolder);
                    using (StreamWriter file = File.CreateText(path))
                        file.Write(list);
                }
                catch
                {
                }
                finally
                {
                    WritingChecklistFile = false;
                }
            }).Start();
        }

        private void TryLoadChecklist()
        {
            this.BlocksHighlightedCount = 0;
            this.BlocksConfirmedCount = 0;
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
                    //update counts
                    if (Tracker.TryGetBlock(id, out Block block))
                    {
                        block.Highlighted = true;
                        if (block.IsComplete())
                            this.BlocksConfirmedCount++;
                        else
                            this.BlocksHighlightedCount++;
                    }
                }
            }
            catch
            {

            }
        }

        private void ExportBlockList()
        {
            var list = new StringBuilder();
            foreach (KeyValuePair<string, Block> block in Tracker.Blocks.All)
                list.AppendLine($"{block.Value.Name.Replace("\n", " ")}");
            File.WriteAllText($"all_required_blocks_{Tracker.Category.CurrentMajorVersion}.txt", list.ToString());
        }
    }
}
