using System.Collections.Generic;
using System.IO;
using System.Xml;
using AATool.Data.Categories;
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
            this.ObtainedCount = 0;
        }

        public void RefreshObjectives()
        {
            this.ClearObjectives();

            //load the blocks added in each past game version up to the current game version
            foreach (string version in AllBlocks.SupportedVersions)
            {
                string blockFile = Path.Combine(Paths.System.ObjectiveFolder, "blocks.xml");
                if (!XmlObject.TryGetDocument(blockFile, out XmlDocument document))
                {
                    //error reading a required block file 
                    this.ClearObjectives();
                    return;
                }

                //add block groups from this version
                foreach (XmlNode groupNode in document.DocumentElement?.ChildNodes)
                {
                    var group = new List<Block>();
                    foreach (XmlNode blockNode in groupNode?.ChildNodes)
                    {
                        //skip spacer nodes
                        if (blockNode.Name is "empty")
                            continue;

                        //remove block from past versions if mojang has since removed it from the game
                        if (blockNode.Name is "remove")
                        {
                            string blockId = XmlObject.Attribute(blockNode, "block", string.Empty);
                            string groupId = XmlObject.Attribute(blockNode, "group", string.Empty);
                            if (this.TryGet(blockId, out Block removed))
                            {
                                this.All.Remove(blockId);
                                if (this.TryGetGroup(groupId, out List<Block> removalGroup))
                                    removalGroup.Remove(removed);
                            }
                        }
                        else
                        {
                            //add all blocks in group
                            var block = new Block(blockNode);
                            this.All[block.Id] = block;
                            group.Add(block);
                        }
                    }
                    this.Groups[groupNode.Name] = group;
                }

                //skip blocks added in versions that came after the currently selected game version
                if (version == Tracker.Category.CurrentVersion)
                    return;
            }
        }

        public void SetState(WorldState progress)
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
                if (block.CompletedByAnyone())
                    this.PlacedCount++;
                if (block.PickedUpByAnyone())
                    this.ObtainedCount++;
            }
        }
    }
}
