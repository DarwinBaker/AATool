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

        public bool TryGet(string id, out Pickup value) => 
            this.All.TryGetValue(id, out value);

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
                    string id = XmlObject.Attribute(node, "id", string.Empty);
                    Pickup itemCount = id switch {
                        NautilusShell.ItemId => new NautilusShell(node),
                        AncientDebris.ItemId => new AncientDebris(node),
                        WitherSkull.ItemId => new WitherSkull(node),
                        Trident.ItemId => new Trident(node),
                        EGap.ItemId => new EGap(node),
                        _ => null
                    };
                    if (itemCount is not null)
                        this.All.Add(itemCount.Id, itemCount);
                }
            }
        }

        public void UpdateStates(WorldState progress)
        {
            foreach (Pickup itemCount in this.All.Values)
                itemCount.UpdateState(progress);
        }
    }
}
