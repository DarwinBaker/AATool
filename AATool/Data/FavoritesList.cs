using AATool.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace AATool.Data
{
    public sealed class FavoritesList : XmlObject
    {
        public readonly HashSet<string> Advancements;
        public readonly HashSet<string> Criteria;
        public readonly HashSet<string> Statistics;

        public bool IsEmpty => this.Advancements.Any() || this.Criteria.Any() || this.Statistics.Any();

        public FavoritesList()
        {
            //initialize lists
            this.Advancements = new ();
            this.Criteria     = new ();
            this.Statistics   = new ();
        }

        public void Clear()
        {
            this.Advancements.Clear();
            this.Criteria.Clear();
            this.Statistics.Clear();
        }

        public override void ReadDocument(XmlDocument document)
        {
            //populate advancements
            this.Advancements.Clear();
            foreach (XmlNode node in document.DocumentElement.SelectSingleNode("advancements").ChildNodes)
                this.Advancements.Add(node.InnerText);

            //populate criteria
            this.Criteria.Clear();
            foreach (XmlNode node in document.DocumentElement.SelectSingleNode("criteria").ChildNodes)
                this.Criteria.Add(node.InnerText);

            //populate statistics
            this.Statistics.Clear();
            foreach (XmlNode node in document.DocumentElement.SelectSingleNode("statistics").ChildNodes)
                this.Statistics.Add(node.InnerText);
        }

        public override void WriteDocument(XmlWriter writer)
        {
            //write root node
            writer.WriteStartElement("favorites");

                //write advancements
                writer.WriteStartElement("advancements");
                foreach (string id in this.Advancements)
                    writer.WriteElementString("advancement", id);
                writer.WriteEndElement();

                //write criteria
                writer.WriteStartElement("criteria");
                foreach (string id in this.Criteria)
                    writer.WriteElementString("criterion", id);
                writer.WriteEndElement();

                //write statistics
                writer.WriteStartElement("statistics");
                foreach (string id in this.Statistics)
                    writer.WriteElementString("statistic", id);
                writer.WriteEndElement();

            writer.WriteEndElement();
        }
    }
}
