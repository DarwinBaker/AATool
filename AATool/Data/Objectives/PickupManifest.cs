using System.Collections.Generic;
using System.Xml;
using AATool.Data.Objectives.Pickups;
using AATool.Data.Progress;
using AATool.Utilities;

namespace AATool.Data.Objectives
{
    public class PickupManifest : IManifest
    {
        public Dictionary<string, Pickup> All { get; private set; }

        public bool TryGet(string id, out Pickup pickup) => 
            this.All.TryGetValue(id, out pickup);

        public PickupManifest()
        {
            this.All = new();
        }

        public void ClearObjectives() => this.All.Clear();

        public void RefreshObjectives()
        {
            this.ClearObjectives();
            if (XmlObject.TryGetDocument(Paths.System.StatisticsFile, out XmlDocument document))
            {
                //build list of items to count
                foreach (XmlNode node in document.DocumentElement.ChildNodes)
                {
                    var counter = Pickup.FromNode(node);
                    if (counter is not null)
                        this.All.Add(counter.Id, counter);
                }
            }
        }

        public void SetState(WorldState progress)
        {
            foreach (Pickup itemCount in this.All.Values)
                itemCount.UpdateState(progress);
        }
    }
}
