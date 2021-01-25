using System.Collections.Generic;
using System.Xml;

namespace AATool.DataStructures
{
    public class AdvancementGroup
    {
        public Dictionary<string, Advancement> Advancements { get; private set; }
        public string Name                                  { get; private set; }
        public int CompletedCount                           { get; private set; }
        public int CompletedPercent                         { get; private set; }

        public AdvancementGroup(XmlDocument document)
        {
            //initialize members from xml 
            Advancements = new Dictionary<string, Advancement>();
            XmlNode root = document.SelectSingleNode("group");
            Name = root.Attributes["name"]?.Value;
            foreach (XmlNode advNode in root.ChildNodes)
                Advancements.Add(advNode.Attributes["id"]?.Value, new Advancement(advNode));
        }

        public void Update(AdvancementsJSON advancements)
        {
            //update all advancements in this group and count them
            CompletedCount = 0;
            foreach (var advancement in Advancements.Values)
            {
                advancement.Update(advancements);
                if (advancement.IsCompleted)
                    CompletedCount++;
            }
            CompletedPercent = (int)((double)CompletedCount / Advancements.Count * 100);
        }
    }
}
