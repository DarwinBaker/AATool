using System.Collections.Generic;
using System.Xml;

namespace AATool.Data
{
    public class AdvancementGroup
    {
        public Dictionary<string, Advancement> Advancements { get; private set; }
        public string Name                                  { get; private set; }
        public int Completed                                { get; private set; }

        public AdvancementGroup(XmlDocument document)
        {
            //initialize members from xml 
            this.Advancements = new ();
            this.Name = document.DocumentElement.Attributes["name"]?.Value;

            //populate advancements
            foreach (XmlNode advNode in document.DocumentElement.ChildNodes)
                this.Advancements.Add(advNode.Attributes["id"]?.Value, new Advancement(advNode));
        }

        public void Update()
        {
            //update all advancements in this group and count them
            this.Completed = 0;
            foreach (Advancement advancement in this.Advancements.Values)
            {
                if (advancement.CompletedByAnyone())
                    this.Completed++;
            }
        }

        public void CopyTo(Dictionary<string, Advancement> dictionary)
        {
            foreach (KeyValuePair<string, Advancement> advancement in this.Advancements)
                dictionary[advancement.Key] = advancement.Value;
        }
    }
}
