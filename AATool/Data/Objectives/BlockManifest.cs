using System.Collections.Generic;
using System.IO;
using System.Xml;
using AATool.Data.Progress;
using AATool.Utilities;

namespace AATool.Data.Objectives
{
    public class BlockManifest : IManifest
    {
        public Dictionary<string, Block> All            { get; private set; }
        public Dictionary<string, List<Block>> Groups   { get; private set; }
        public int ObtainedCount { get; private set; }
        public int PlacedCount { get; private set; }
        public int Count => this.All.Count;

        public List<Block> AllBlocksList;

        public BlockManifest()
        {
            this.All = new();
            this.Groups = new();
            this.AllBlocksList = new();
        }

        public bool TryGet(string id, out Block block) =>
            this.All.TryGetValue(id, out block);

        public bool TryGetGroup(string id, out List<Block> group) =>
            this.Groups.TryGetValue(id, out group);

        public void ClearObjectives()
        {
            this.Groups.Clear();
            this.All.Clear();
            this.PlacedCount = 0;
            this.ObtainedCount = 0;
            this.AllBlocksList.Clear();
        }

        public void RefreshObjectives()
        {
            this.ClearObjectives();

            string blockFile = Path.Combine(Paths.System.ObjectiveFolder, "blocks.xml");
            if (!XmlObject.TryGetDocument(blockFile, out XmlDocument document))
                return;

            //add block groups from this version
            foreach (XmlNode groupNode in document.DocumentElement?.ChildNodes)
            {
                var group = new List<Block>();
                foreach (XmlNode blockNode in groupNode?.ChildNodes)
                {
                    //add spacer
                    if (blockNode.Name is "empty")
                    {
                        group.Add(null);
                        continue;
                    }

                    //add all blocks in group
                    var block = new Block(blockNode);
                    this.All[block.Id] = block;
                    this.AllBlocksList.Add(block);
                    group.Add(block);
                }
                this.Groups[groupNode.Name] = group;
            }
        }

        public void UpdateState(ProgressState progress)
        {
            foreach (Block block in this.All.Values)
                block.UpdateState(progress);
            this.UpdateTotal();
        }

        public void UpdateTotal()
        {
            this.PlacedCount = 0;
            this.ObtainedCount = 0;
            foreach (Block block in this.All.Values)
            {
                //update completion count
                if (block.CompletedByAnyone)
                    this.PlacedCount++;
                if (block.Obtained)
                    this.ObtainedCount++;
            }
        }

        private void ExportIdList()
        {
            string path = $"required_blocks_{Tracker.Category.CurrentVersion}";
            using (StreamWriter export = File.CreateText(path))
            {
                foreach (string id in this.All.Keys)
                    export.WriteLine(id);
            }
        }
    }
}
