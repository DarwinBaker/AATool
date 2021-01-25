using System.Collections.Generic;
using System.Xml;

namespace AATool.DataStructures
{
    public struct Potion
    {
        public string Name                  { get; private set; }
        public string Icon                  { get; private set; }
        public List<string> Ingredients     { get; private set; }

        public Potion(XmlNode node)
        {
            //initialize members from xml 
            Icon = node.Attributes["icon"]?.Value;
            Name = node.Attributes["name"]?.Value;
            Ingredients = new List<string>();
            foreach (XmlNode ingredient in node.SelectNodes("ingredient"))
                Ingredients.Add(ingredient.Attributes["id"]?.Value);
        }
    }
}
