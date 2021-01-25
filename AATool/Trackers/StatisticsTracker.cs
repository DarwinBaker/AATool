using AATool.DataStructures;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace AATool.Trackers
{
    public class StatisticsTracker : Tracker
    {
        public Dictionary<string, ItemStats> ItemCountList  { get; private set; }

        public ItemStats ItemCount(string id)     => ItemCountList.TryGetValue(id, out var val) ? val : null;

        public StatisticsTracker()
        {
            JSON = new StatisticsJSON();
        }

        protected override void ParseReferences()
        {
            //load list of items to count
            ItemCountList = new Dictionary<string, ItemStats>();
            var document = new XmlDocument();
            using (var stream = File.OpenRead(Path.Combine(Paths.DIR_STATISTICS, "item_counts.xml")))
            {
                document.Load(stream);
                foreach (XmlNode itemNode in document.SelectSingleNode("items").ChildNodes)
                    ItemCountList.Add(itemNode.Attributes["id"]?.Value, new ItemStats(itemNode));
            }
        }

        protected override void ReadSave()
        {
            //update item counts
            foreach (var itemCount in ItemCountList.Values)
                itemCount.Update(JSON as StatisticsJSON);
        }
    }
}
