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
        public int PlacedCount                          { get; private set; }

        public int Count => this.All.Count;

        public BlockManifest()
        {
            this.All = new();
            this.Groups = new();
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
        }

        public void RefreshObjectives()
        {
            this.ClearObjectives();

            bool filesExist = Paths.TryGetAllFiles(Paths.System.BlocksFolder, "*.xml", 
                SearchOption.TopDirectoryOnly,
                out IEnumerable<string> files);
            if (!filesExist)
                return;

            //iterate block reference files for current game version
            foreach (string file in files)
            {
                if (!XmlObject.TryGetDocument(file, out XmlDocument document))
                    continue;

                //add blocks and group by file
                var group = new List<Block>();
                foreach (XmlNode blockNode in document.DocumentElement.ChildNodes)
                {
                    if (blockNode.Name is "block")
                    {
                        var block = new Block(blockNode);
                        this.All.Add(block.Id, block);
                        group.Add(block);
                    }
                    else if (blockNode.Name is "empty")
                    {
                        group.Add(null);
                    }
                }
                this.Groups[Path.GetFileNameWithoutExtension(file)] = group;
            }
        }

        public void UpdateStates(WorldState progress)
        {
            this.PlacedCount = 0;
            foreach (Block block in this.All.Values)
            {
                //update advancement and completion count
                block.UpdateState(progress);
                if (block.CompletedByAnyone())
                    this.PlacedCount++;
            }
        }
    }
}
