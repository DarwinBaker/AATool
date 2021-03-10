using System.Globalization;
using System.Xml;

namespace AATool.DataStructures
{
    public struct PotionIngredient
    {
        public string ID   { get; private set; }
        public string Name { get; private set; }
        public string Icon { get; private set; }

        public PotionIngredient(XmlNode node)
        {
            //initialize members from xml 
            ID   = node.Attributes["id"]?.Value;
            Name = node.Attributes["name"]?.Value ?? ID;
            Icon = node.Attributes["icon"]?.Value ?? ID;
        }
    }
}