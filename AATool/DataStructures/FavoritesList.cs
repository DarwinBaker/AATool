using AATool.Utilities;
using System.Collections.Generic;
using System.Xml;

namespace AATool.DataStructures
{
    public class FavoritesList : XmlObject
    {
        public HashSet<string> Advancements;
        public HashSet<string> Criteria;
        public HashSet<string> Statistics;

        public bool IsEmpty => Advancements?.Count == 0 && Criteria?.Count == 0 && Statistics?.Count == 0;

        public FavoritesList()
        {
            Advancements = new HashSet<string>();
            Criteria = new HashSet<string>();
            Statistics = new HashSet<string>();
        }

        public void Clear()
        {
            Advancements.Clear();
            Criteria.Clear();
            Statistics.Clear();
        }

        public override void ReadDocument(XmlDocument document)
        {
            //populate lists
            Advancements.Clear();
            foreach (XmlNode node in document.SelectSingleNode("favorites/advancements").ChildNodes)
                Advancements.Add(node.InnerText);
            Criteria.Clear();
            foreach (XmlNode node in document.SelectSingleNode("favorites/criteria").ChildNodes)
                Criteria.Add(node.InnerText);
            Statistics.Clear();
            foreach (XmlNode node in document.SelectSingleNode("favorites/statistics").ChildNodes)
                Statistics.Add(node.InnerText);
        }

        public override void WriteDocument(XmlWriter writer)
        {
            writer.WriteStartElement("favorites");

                writer.WriteStartElement("advancements");
                foreach (var id in Advancements)
                    writer.WriteElementString("advancement", id);
                writer.WriteEndElement();

                writer.WriteStartElement("criteria");
                foreach (var id in Criteria)
                    writer.WriteElementString("criterion", id);
                writer.WriteEndElement();

                writer.WriteStartElement("statistics");
                foreach (var id in Statistics)
                    writer.WriteElementString("statistic", id);
                writer.WriteEndElement();

            writer.WriteEndElement();
        }
    }
}
