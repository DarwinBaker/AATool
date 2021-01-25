using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace AATool.DataStructures
{
    public class Criterion
    {
        public string ID                { get; private set; }
        public string Name              { get; private set; }
        public string Icon              { get; private set; }
        public string AdvancementID     { get; private set; }
        public bool IsCompleted         { get; private set; }

        public void Update(HashSet<string> completed) => IsCompleted = completed.Contains(ID);

        public Criterion(XmlNode node, string advancementName)
        {
            //initialize members from xml 
            AdvancementID = advancementName;
            ID = node.Attributes["id"]?.Value;
            var idParts = ID.Split(':');
            string shortID = idParts.Length > 0 ? idParts[idParts.Length - 1] : null;
            Name = node.Attributes["name"]?.Value ?? new CultureInfo("en-US", false).TextInfo.ToTitleCase(shortID.Replace('_', ' '));
            Icon = node.Attributes["icon"]?.Value ?? shortID;
        }
    }
}
