using System.Collections.Generic;
using System.Xml;
using AATool.Data.Objectives.Pickups;
using AATool.Data.Progress;
using AATool.Utilities;

namespace AATool.Data.Objectives
{
    public class DeathManifest : IManifest
    {
        public Dictionary<string, Death> All { get; private set; }
        public int TotalExperienced { get; private set; }
        public int Count => this.All.Count;

        public bool TryGet(string id, out Death death) =>
            this.All.TryGetValue(id, out death);

        public DeathManifest()
        {
            this.All = new();
        }

        public void ClearObjectives() => this.All.Clear();

        public void RefreshObjectives()
        {
            this.ClearObjectives();
            if (XmlObject.TryGetDocument(Paths.System.DeathMessagesFile, out XmlDocument document))
            {
                //build list of items to count
                foreach (XmlNode node in document.DocumentElement.ChildNodes)
                {
                    string id = XmlObject.Attribute(node, "id", string.Empty);
                    this.All[id] = new Death(node);
                }
            }
        }

        public void SetState(WorldState progress)
        {
            foreach (Death death in this.All.Values)
                death.UpdateState(progress);
            this.UpdateTotal();
        }

        public void UpdateTotal()
        {
            this.TotalExperienced = 0;
            foreach (Death death in this.All.Values)
            {
                //update completion count
                if (death.CompletedByAnyone())
                    this.TotalExperienced++;
            }
        }
    }
}
